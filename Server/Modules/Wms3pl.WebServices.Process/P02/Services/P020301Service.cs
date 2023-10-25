using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;


namespace Wms3pl.WebServices.Process.P02.Services
{
  public partial class P020301Service
  {
    private WmsTransaction _wmsTransaction;

    public P020301Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    /// <summary>
    /// 取得調入調撥單查詢資料
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="allocationDate">調撥日期</param>
    /// <returns></returns>
    public IQueryable<F1510Data> GetF1510DatasByTar(string dcCode, string gupCode, string custCode, string allocationNo,
      DateTime allocationDate)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF1510DatasByTar(dcCode, gupCode, custCode, allocationNo, allocationDate);
    }


    #region 調入上架、調撥單維護過帳 修改目的儲位 取得與更新

    /// <summary>
    /// 調入調撥單取得商品可修改儲位及數量資料
    /// </summary>
    /// <param name="tarDcCode">目的物流中心</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="status">調撥單狀態</param>
    /// <param name="itemCode">商品代號</param>
    /// <returns></returns>
    public IQueryable<F1510ItemLocData> GetF1510ItemLocDatas(string tarDcCode, string gupCode, string custCode,
      string allocationNo, string status, string itemCode, DateTime validDate, string srcLocCode, string makeNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF1510ItemLocDatas(tarDcCode, gupCode, custCode, allocationNo, status.Split(','), itemCode, validDate, srcLocCode, makeNo);
    }

    Func<F151002, string, string, string, string, string, string, DateTime, DateTime, DateTime?, string, string, bool> F151002Func = FindF151002;
    private static bool FindF151002(F151002 f151002, string tarLocCode, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo, DateTime validDate, DateTime enterDate, DateTime? srcValidDate, string containerCode, string binCode)
    {
      return f151002.TAR_LOC_CODE == tarLocCode &&
                               (f151002.SRC_VALID_DATE ?? f151002.VALID_DATE) ==
                               (srcValidDate ?? validDate) &&
                               f151002.ENTER_DATE == enterDate &&
                               f151002.VNR_CODE == vnrCode &&
                               f151002.SERIAL_NO == serialNo &&
                               f151002.BOX_CTRL_NO == boxCtrlNo &&
                               f151002.PALLET_CTRL_NO == palletCtrlNo &&
                               f151002.MAKE_NO == makeNo &&
                               f151002.CONTAINER_CODE == containerCode &&
                               f151002.BIN_CODE == binCode;
    }

    Func<F1913, string, string, string, string, string, string, string, string, string, string, DateTime, DateTime, bool> F1913Func = FindF1913;
    private static bool FindF1913(F1913 f1913, string dcCode, string gupCode, string custCode, string itemCode, string locCode, string makeNo, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, DateTime validDate, DateTime enterDate)
    {
      return f1913.DC_CODE == dcCode &&
           f1913.GUP_CODE == gupCode &&
           f1913.CUST_CODE == custCode &&
           f1913.ITEM_CODE == itemCode &&
           f1913.LOC_CODE == locCode &&
           f1913.VALID_DATE == validDate &&
           f1913.ENTER_DATE == enterDate &&
           f1913.VNR_CODE == vnrCode &&
           f1913.SERIAL_NO == serialNo &&
           f1913.BOX_CTRL_NO == boxCtrlNo &&
           f1913.MAKE_NO == makeNo &&
           f1913.PALLET_CTRL_NO == palletCtrlNo;
    }

    /// <summary>
    /// 調入調撥單新增或更新儲位及數量調整
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="itemCode">品號</param>
    /// <param name="datas">更新調入調撥單儲位及數量資料</param>
    /// <returns></returns>
    public ExecuteResult InsertOrUpdateF1510LocItemData(F1510Data f1510Data, List<F1510ItemLocData> datas)
    {
      string dcCode = f1510Data.DC_CODE;
      string gupCode = f1510Data.GUP_CODE;
      string custCode = f1510Data.CUST_CODE;
      string allocationNo = f1510Data.ALLOCATION_NO;
      string itemCode = f1510Data.ITEM_CODE;
      string makeNo = f1510Data.MAKE_NO;
      string palletCtrlNo = f1510Data.PALLET_CTRL_NO;
      string boxCtrlNo = f1510Data.BOX_CTRL_NO;
      DateTime validDate = f1510Data.VALID_DATE.Value;
      var result = new ExecuteResult { IsSuccessed = true };
      if (datas.Any())
      {
        //取得原資料上架數有減少的資料
        var normalDataDecreaseQty = datas.Where(o => o.ChangeStatus == "Normal" && o.ORGINAL_QTY > o.QTY).ToList();
        //取得上架數>0且原資料上架數有增加或新增上架儲位上架數
        var dataIncreaseQty = datas.Where(o => o.QTY > 0 && o.ORGINAL_QTY < o.QTY).ToList();
        var f151001Repo = new F151001Repository(Schemas.CoreSchema);
        var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
        var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);

        var orginalDatas = f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo);
        short seqNo = 1;
        if (orginalDatas.Any())
          seqNo += orginalDatas.Max(o => o.ALLOCATION_SEQ);

        var orginalItemDatas =
          //f151002Repo.AsForUpdate().GetDatasByValidDate(dcCode, gupCode, custCode, allocationNo, itemCode, validDate).ToList();
          orginalDatas.Where(x => x.ITEM_CODE == itemCode && x.VALID_DATE == validDate).ToList();

        var f151001 = f151001Repo.Find(o => o.DC_CODE == f1510Data.DC_CODE && o.GUP_CODE == f1510Data.GUP_CODE && o.CUST_CODE == f1510Data.CUST_CODE && o.ALLOCATION_NO == f1510Data.ALLOCATION_NO);

        //主要原因:因建議儲位找不到(預設:000000) 造成沒有倉別ID 以下做特殊處理
        //if (!normalDataDecreaseQty.Any())
        //{	
        //	var tmpData = AutoMapper.Mapper.DynamicMap<F1510ItemLocData>(dataIncreaseQty.First());
        //	tmpData.ORGINAL_QTY = dataIncreaseQty.Sum(o=>o.QTY);
        //	tmpData.QTY = 0;
        //	tmpData.ChangeStatus = "Normal";
        //	tmpData.TAR_LOC_CODE = "000000000";
        //	normalDataDecreaseQty.Add(tmpData);
        //}
        var updF151002List = new List<F151002>();
        var addF151002List = new List<F151002>();
        foreach (var f1510ItemLocData in normalDataDecreaseQty)
        {
          var orginalTarLocDataDecreases = orginalItemDatas.Where(o => o.TAR_LOC_CODE == f1510ItemLocData.TAR_LOC_CODE && o.MAKE_NO == makeNo && o.BOX_CTRL_NO == boxCtrlNo && o.PALLET_CTRL_NO == palletCtrlNo);
          //取得原資料可減少的上架數
          var decreaseQty = f1510ItemLocData.ORGINAL_QTY - f1510ItemLocData.QTY;
          //取得等於此上架儲位所有資料
          foreach (var orginalTarLocDataDecrease in orginalTarLocDataDecreases)
          {
            if (orginalTarLocDataDecrease.TAR_QTY > 0)
            {
              do
              {
                var item = dataIncreaseQty.FirstOrDefault(o => o.ORGINAL_QTY < o.QTY);
                if (item != null)
                {
                  var orginTarLocDataIncrease = addF151002List
                    .FirstOrDefault(o => F151002Func(o, item.TAR_LOC_CODE, orginalTarLocDataDecrease.VNR_CODE, orginalTarLocDataDecrease.SERIAL_NO, orginalTarLocDataDecrease.BOX_CTRL_NO, orginalTarLocDataDecrease.PALLET_CTRL_NO, orginalTarLocDataDecrease.MAKE_NO, orginalTarLocDataDecrease.VALID_DATE, orginalTarLocDataDecrease.ENTER_DATE, orginalTarLocDataDecrease.SRC_VALID_DATE, orginalTarLocDataDecrease.CONTAINER_CODE, orginalTarLocDataDecrease.BIN_CODE));
                  orginTarLocDataIncrease = orginTarLocDataIncrease ?? updF151002List
                    .FirstOrDefault(o => F151002Func(o, item.TAR_LOC_CODE, orginalTarLocDataDecrease.VNR_CODE, orginalTarLocDataDecrease.SERIAL_NO, orginalTarLocDataDecrease.BOX_CTRL_NO, orginalTarLocDataDecrease.PALLET_CTRL_NO, orginalTarLocDataDecrease.MAKE_NO, orginalTarLocDataDecrease.VALID_DATE, orginalTarLocDataDecrease.ENTER_DATE, orginalTarLocDataDecrease.SRC_VALID_DATE, orginalTarLocDataDecrease.CONTAINER_CODE, orginalTarLocDataDecrease.BIN_CODE));
                  var isAddOrUpdateData = orginTarLocDataIncrease != null;
                  orginTarLocDataIncrease = orginTarLocDataIncrease ?? orginalItemDatas
                    .FirstOrDefault(o => F151002Func(o, item.TAR_LOC_CODE, orginalTarLocDataDecrease.VNR_CODE, orginalTarLocDataDecrease.SERIAL_NO, orginalTarLocDataDecrease.BOX_CTRL_NO, orginalTarLocDataDecrease.PALLET_CTRL_NO, orginalTarLocDataDecrease.MAKE_NO, orginalTarLocDataDecrease.VALID_DATE, orginalTarLocDataDecrease.ENTER_DATE, orginalTarLocDataDecrease.SRC_VALID_DATE, orginalTarLocDataDecrease.CONTAINER_CODE, orginalTarLocDataDecrease.BIN_CODE));

                  //增加的上架數
                  long itemIncreaseQty = item.QTY - item.ORGINAL_QTY;
                  if (itemIncreaseQty >= decreaseQty)
                    itemIncreaseQty = decreaseQty;

                  #region 下架、上架數量減少處理
                  var qty = orginalTarLocDataDecrease.TAR_QTY - itemIncreaseQty;
                  if (qty < 0)
                    itemIncreaseQty = orginalTarLocDataDecrease.TAR_QTY;
                  //多出來的實際下架數
                  long itemIncreaseAQty = 0;
                  // 有下架倉的才需要扣SRC_QTY
                  if (!string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
                  {
                    orginalTarLocDataDecrease.SRC_QTY -= itemIncreaseQty;

                    if (orginalTarLocDataDecrease.SRC_QTY < orginalTarLocDataDecrease.A_SRC_QTY)
                    {
                      itemIncreaseAQty = orginalTarLocDataDecrease.A_SRC_QTY - orginalTarLocDataDecrease.SRC_QTY;
                      orginalTarLocDataDecrease.A_SRC_QTY = orginalTarLocDataDecrease.SRC_QTY;
                    }
                  }
                  orginalTarLocDataDecrease.TAR_QTY -= itemIncreaseQty;
                  #endregion 下架、上架數量減少處理

                  #region 下架、上架數量增加處理
                  if (orginTarLocDataIncrease != null)
                  {
                    if (!string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
                    {
                      orginTarLocDataIncrease.SRC_QTY += itemIncreaseQty + (orginalTarLocDataDecrease.TAR_QTY > 0
                      ? 0
                      : orginalTarLocDataDecrease.SRC_QTY);
                    }

                    orginTarLocDataIncrease.A_SRC_QTY += itemIncreaseAQty + (orginalTarLocDataDecrease.TAR_QTY > 0
                        ? 0
                        : orginalTarLocDataDecrease.A_SRC_QTY);
                    orginTarLocDataIncrease.TAR_QTY += itemIncreaseQty;
                    if (!isAddOrUpdateData)
                    {
                      orginTarLocDataIncrease.STATUS = "1"; //增加上架數，有可能是加在status=2 & TAR_QTY=0的資料，所以要把狀態改回
                      updF151002List.Add(orginTarLocDataIncrease);
                    }
                  }
                  else
                    addF151002List.Add(CreateF151002(orginalTarLocDataDecrease, ref seqNo, item.TAR_LOC_CODE, itemIncreaseQty, itemIncreaseAQty, !string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID)));
                  #endregion 下架、上架數量增加處理

                  item.QTY -= (int)itemIncreaseQty;
                  decreaseQty -= (int)itemIncreaseQty;
                }
                else
                  break;
                if (decreaseQty <= 0)
                  break;
              } while (dataIncreaseQty.Any(o => o.ORGINAL_QTY < o.QTY) && orginalTarLocDataDecrease.TAR_QTY > 0);
              if (orginalTarLocDataDecrease.TAR_QTY == 0)
                orginalTarLocDataDecrease.STATUS = "2"; //如果都被移除的話，就直接改成上架完成，避免之後過帳不會更新F151001.STATUS

              updF151002List.Add(orginalTarLocDataDecrease);

            }
          }
        }

				foreach (var f151002 in updF151002List)
        { 
          if (string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
          {
            f151002.SUG_LOC_CODE = f151002.TAR_LOC_CODE;
            f151002.SRC_LOC_CODE = f151002.TAR_LOC_CODE;
          }

					f151002Repo.Update(f151002);
				}

				foreach (var f151002 in addF151002List)
        {
          if (string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
          {
            f151002.SUG_LOC_CODE = f151002.TAR_LOC_CODE;
            f151002.SRC_LOC_CODE = f151002.TAR_LOC_CODE;
          }

          f151002Repo.Add(f151002);
        }

        // 在調撥上架時，調撥明細與虛擬儲位PK是對應的，故也要編輯F1511虛擬儲位
        if (updF151002List.Any())
        {
          var udpF1511List = f1511Repo.AsForUpdate().InWithTrueAndCondition("ORDER_SEQ", updF151002List.Select(x => x.ALLOCATION_SEQ).ToList(), x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORDER_NO == allocationNo);
          foreach (var f151002 in updF151002List)
          {
            var f1511 = udpF1511List.FirstOrDefault(x => x.DC_CODE == f151002.DC_CODE && x.GUP_CODE == f151002.GUP_CODE && x.CUST_CODE == f151002.CUST_CODE && x.ORDER_NO == f151002.ALLOCATION_NO && x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString());
            if (f1511 != null)
            {
              f1511.B_PICK_QTY = (int)f151002.SRC_QTY;
              //如果User是全部上架到另一個儲位，就把原本的虛擬儲位清掉
              if (f151002.SRC_QTY == 0)
              {
                f1511.A_PICK_QTY = (int)f151002.SRC_QTY;
                f1511.STATUS = "2";
              }
              f1511Repo.Update(f1511);
            }
          }
        }

        foreach (var f151002 in addF151002List)
        {
          f1511Repo.Add(new F1511
          {
            DC_CODE = f151002.DC_CODE,
            GUP_CODE = f151002.GUP_CODE,
            CUST_CODE = f151002.CUST_CODE,
            ORDER_NO = f151002.ALLOCATION_NO,
            ORDER_SEQ = f151002.ALLOCATION_SEQ.ToString(),
            STATUS = "0",
            B_PICK_QTY = (int)f151002.SRC_QTY,
            A_PICK_QTY = 0,
            ITEM_CODE = f151002.ITEM_CODE,
            VALID_DATE = f151002.VALID_DATE,
            ENTER_DATE = f151002.ENTER_DATE,
            SERIAL_NO = f151002.SERIAL_NO,
            LOC_CODE = f151002.SRC_LOC_CODE,
            MAKE_NO = f151002.MAKE_NO,
            BOX_CTRL_NO = f151002.BOX_CTRL_NO,
            PALLET_CTRL_NO = f151002.PALLET_CTRL_NO
          });
        }
      }
      return result;
    }

    private F151002 CreateF151002(F151002 orginalItem, ref short seqNo, string tarLocCode, long qty, long aQty, bool srcWhHasValue)
    {
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151002 = new F151002
      {
        ALLOCATION_DATE = orginalItem.ALLOCATION_DATE,
        ALLOCATION_NO = orginalItem.ALLOCATION_NO,
        ALLOCATION_SEQ = seqNo,
        ORG_SEQ = orginalItem.ORG_SEQ,
        DC_CODE = orginalItem.DC_CODE,
        GUP_CODE = orginalItem.GUP_CODE,
        CUST_CODE = orginalItem.CUST_CODE,
        STATUS = orginalItem.STATUS,
        ITEM_CODE = orginalItem.ITEM_CODE,
        SRC_LOC_CODE = orginalItem.SRC_LOC_CODE,
        SUG_LOC_CODE = orginalItem.SUG_LOC_CODE,
        TAR_LOC_CODE = tarLocCode,
        A_SRC_QTY = aQty + (orginalItem.TAR_QTY > 0 ? 0 : orginalItem.A_SRC_QTY),
        A_TAR_QTY = 0,
        SRC_QTY = srcWhHasValue ? qty + (orginalItem.TAR_QTY > 0 ? 0 : orginalItem.SRC_QTY) : 0,
        TAR_QTY = qty,
        VALID_DATE = orginalItem.VALID_DATE,
        VNR_CODE = orginalItem.VNR_CODE,
        ENTER_DATE = orginalItem.ENTER_DATE,
        SRC_VALID_DATE = orginalItem.SRC_VALID_DATE,
        BOX_CTRL_NO = orginalItem.BOX_CTRL_NO,
        MAKE_NO = orginalItem.MAKE_NO,
        PALLET_CTRL_NO = orginalItem.PALLET_CTRL_NO,
        CONTAINER_CODE=orginalItem.CONTAINER_CODE,
        BIN_CODE=orginalItem.BIN_CODE,
        RECEIPTFLAG=orginalItem.RECEIPTFLAG,
        SOURCE_TYPE = orginalItem.SOURCE_TYPE,
        SOURCE_NO = orginalItem.SOURCE_NO,
        REFENCE_NO = orginalItem.REFENCE_NO,
        REFENCE_SEQ = orginalItem.REFENCE_SEQ,
				SRC_STAFF = orginalItem.SRC_STAFF,
				SRC_NAME = orginalItem.SRC_NAME,
				SRC_DATE = orginalItem.SRC_DATE,
				SERIAL_NO = orginalItem.SERIAL_NO,
				BOX_NO = orginalItem.BOX_NO,
				CHECK_SERIALNO = orginalItem.CHECK_SERIALNO,
				SRC_MAKE_NO = orginalItem.SRC_MAKE_NO,
				STICKER_PALLET_NO = orginalItem.STICKER_PALLET_NO,
				TAR_MAKE_NO = orginalItem.TAR_MAKE_NO,
				TAR_DATE = orginalItem.TAR_DATE,
				TAR_STAFF = orginalItem.TAR_STAFF,
				TAR_NAME = orginalItem.TAR_NAME,
				TAR_VALID_DATE = orginalItem.TAR_VALID_DATE
			
      };
      seqNo++;
      return f151002;
    }

    #endregion

    #region 調入調撥單調入上架確認更新 or 調撥單維護過帳

    /// <summary>
    /// 調入調撥單調入上架確認更新 or 調撥單過帳
    /// </summary>
    /// <param name="allocationNo"></param>
    /// <param name="datas">調入調撥單上架資料</param>
    /// <param name="tarDcCode">上架的物流中心</param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="isUpdateSerialStatus"></param>
    /// <param name="IsAllocationPosting">調撥確認是否要直接過帳</param>
    /// <returns></returns>
    public ExecuteResult UpdateF1510Data(string tarDcCode, string gupCode, string custCode, string allocationNo,
      List<F1510Data> datas, bool isUpdateSerialStatus = false, Boolean IsAllocationPosting = false)
    {
      ExecuteResult result = null;
      datas = datas.Where(o => o.IsSelected).ToList(); //只有勾選的才上架
                                                       //更新F151001狀態為已上架(5)
      var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
      var f191204Repo = new F191204Repository(Schemas.CoreSchema, _wmsTransaction);
      var sharedService = new SharedService(_wmsTransaction);
      var stockService = new StockService(_wmsTransaction);
      var details = new List<AllocationConfirmDetail>();
      sharedService.StockService = stockService;
      var f151001 = f151001Repo.Find(
        o =>
          o.ALLOCATION_NO == allocationNo && o.TAR_DC_CODE == tarDcCode && o.GUP_CODE == gupCode &&
          o.CUST_CODE == custCode);

      if (f151001.STATUS == "5")
        return new ExecuteResult(false, "此調撥單已結案，請重新查詢！");

      // 若來源倉為自動倉且調撥單狀態為0待處理1已列印調撥單2:下架處理中，不可手動過帳
      List<string> statusList = new List<string> { "0", "1", "2" };
      if (f1980Repo.CheckAutoWarehouse(f151001.SRC_DC_CODE, f151001.SRC_WAREHOUSE_ID) && statusList.Any(x => x == f151001.STATUS))
        return new ExecuteResult(false, "來源倉為自動倉，不可以手動過帳");

      // 若目的倉為自動倉，不可手動過帳
      if (f1980Repo.CheckAutoWarehouse(f151001.TAR_DC_CODE, f151001.TAR_WAREHOUSE_ID))
        return new ExecuteResult(false, "目的倉為自動倉，不可以手動過帳");

      // 檢查是否有缺貨待處理
      var f151003s = f151003Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == tarDcCode &&
      o.GUP_CODE == gupCode &&
      o.CUST_CODE == custCode &&
      o.ALLOCATION_NO == allocationNo &&
      o.STATUS == "0");
      if (f151003s.Any())
        return new ExecuteResult(false, "該調撥單有缺貨待確認，需全部確認後才可以進行過帳");

      // 檢核儲位庫存與傳入調撥明細是否有混品
      var checkMixRes = sharedService.CheckItemMixLoc(datas.Select(x => new CheckItemTarLocAndParamsMixLoc
      {
        DcCode = x.DC_CODE,
        GupCode = x.GUP_CODE,
        CustCode = x.CUST_CODE,
        ItemCode = x.ITEM_CODE,
        TarLocCode = x.TAR_LOC_CODE
      }).ToList());
      if (!checkMixRes.IsSuccessed)
        return checkMixRes;

      //針對商品不可混批進行檢核(可能同一商品上架同一個儲位明細有混效期) 
      var checkItemMixLoc = sharedService.CheckItemMixBatch(datas.GroupBy(o => new { o.GUP_CODE, o.CUST_CODE, o.ITEM_CODE, o.TAR_LOC_CODE }).Select(o => new CheckItemTarLocMixLoc
      {
        GupCode = o.Key.GUP_CODE,
        CustCode = o.Key.CUST_CODE,
        ItemCode = o.Key.ITEM_CODE,
        TarLocCode = o.Key.TAR_LOC_CODE,
        CountValidDate = o.Select(c => c.VALID_DATE).Distinct().Count()
      }).ToList());
      if (!checkItemMixLoc.IsSuccessed)
        return checkItemMixLoc;

      var f151002s = f151002Repo.AsForUpdate().GetDatas(f151001.DC_CODE, gupCode, custCode, allocationNo).ToList();
			var items = f151002s.Select(o => o.ITEM_CODE).Distinct().ToList();
			var srcLocCodes = f151002s.Select(o => o.SRC_LOC_CODE).Distinct().ToList();
			var tarLocCodes = f151002s.Select(o => o.TAR_LOC_CODE).Distinct().ToList();
      var f1913s = f1913Repo.AsForUpdate().GetDatasByItems(tarDcCode, gupCode, custCode, items).Where(x => x.QTY > 0).ToList();
      var f1913ByTarLocs = f1913Repo.GetDatasByLocs(tarDcCode, gupCode, custCode, tarLocCodes).ToList();
			var f1903s = f1903Repo.GetDatasByItems(gupCode, custCode, items).ToList();
			var f1909Item = f1909Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			var f1980s = f1980Repo.GetWareHouseTmprTypeByLocCode(tarDcCode, tarLocCodes).ToList();

      var srcF1912s = f1912Repo.GetDatasByLocCodes(f151001.DC_CODE, srcLocCodes);
      var tarF1912s = f1912Repo.GetDatasByLocCodes(tarDcCode, tarLocCodes);
      var allocationType = AllocationType.Both;
      if (string.IsNullOrEmpty(f151001.TAR_WAREHOUSE_ID))
        allocationType = AllocationType.NoTarget;
      else if (string.IsNullOrEmpty(f151001.SRC_WAREHOUSE_ID))
        allocationType = AllocationType.NoSource;

      foreach (var f1510Data in datas)
      {
        var srcLocCode = f1510Data.SRC_LOC_CODE;
        var sugLocCode = f1510Data.SUG_LOC_CODE;
        var tarLocCode = f1510Data.TAR_LOC_CODE;
        if (allocationType == AllocationType.NoSource)
          srcLocCode = sugLocCode;
        else if (allocationType == AllocationType.NoTarget)
        {
          sugLocCode = srcLocCode;
          tarLocCode = srcLocCode;
        }

        var itemDetails = f151002s.Where(
            o =>
              o.ITEM_CODE == f1510Data.ITEM_CODE && o.SRC_LOC_CODE == srcLocCode &&
              o.SUG_LOC_CODE == sugLocCode && o.TAR_LOC_CODE == tarLocCode);

        if (!string.IsNullOrWhiteSpace(f1510Data.ALLOCATION_SEQ_LIST))
        {
          var seqList = f1510Data.ALLOCATION_SEQ_LIST.Split(',');
          var seqs = Array.ConvertAll<string, int>(seqList, int.Parse);
          itemDetails = itemDetails.Where(o => seqs.Contains(o.ALLOCATION_SEQ));
        }

        if (f1510Data.VALID_DATE.HasValue)
          itemDetails = itemDetails.Where(x => x.VALID_DATE == f1510Data.VALID_DATE.Value).ToList();

        if (itemDetails.All(o => o.STATUS == "2")) //如果此筆資料已經過帳就跳過避免重複過帳
          continue;

        //檢查上架儲位是否有其他貨主使用
        var check = sharedService.CheckNowCustCodeLoc(tarDcCode, tarLocCode, custCode);
        if (!check.IsSuccessed)
          return check;

        foreach (var f151002 in itemDetails)
        {
          if (f151002.STATUS == "2") //如果此筆資料已經過帳就跳過避免重複過帳
            continue;

          #region 檢查商品儲位溫層
          var f1903Item = f1903s.First(o => o.ITEM_CODE == f151002.ITEM_CODE);
          var f1980Item = f1980s.First(o => o.LOC_CODE == f151002.TAR_LOC_CODE && o.DC_CODE == tarDcCode);
          //  商品溫度              倉別溫層
          //  02(恆溫),03(冷藏) =>  02(低溫)
          //  01(常溫)          =>  01(常溫)
          //  04(冷凍)          =>  03(冷凍)
          if (!sharedService.GetWareHouseTmprByItemTmpr(f1903Item.TMPR_TYPE).Split(',').Contains(f1980Item.TMPR_TYPE))
          {
            return new ExecuteResult
            {
              IsSuccessed = false,
              Message =
                string.Format("儲位{0}溫層({1})不符合商品{2}溫層({3})", f151002.TAR_LOC_CODE,
                  sharedService.GetWareHouseTmprName(f1980Item.TMPR_TYPE), f1903Item.ITEM_NAME,
                  sharedService.GetItemTmprName(f1903Item.TMPR_TYPE))
            };
          }
          #endregion

					#region 檢核商品儲位
					//檢查該商品是否符合儲位混商品
					var message = string.Empty;
					if (!((f1909Item != null && f1909Item.MIX_LOC_ITEM == "1") || (f1903Item != null && f1903Item.LOC_MIX_ITEM == "1")) && f1913ByTarLocs.Any(o => o.LOC_CODE == f151002.TAR_LOC_CODE && o.ITEM_CODE != f151002.ITEM_CODE))
					{
						message += "該商品無法同一儲位混品\n";
					}
					//檢查該商品是否符合儲位混批(效期)
					if (f1903Item != null && f1903Item.MIX_BATCHNO == "0" &&
						f1913s.Any(
							o =>
								o.ITEM_CODE == f151002.ITEM_CODE && o.LOC_CODE == f151002.TAR_LOC_CODE && o.VALID_DATE != f151002.VALID_DATE))
					{
						message += "該商品無法混批(效期)\n";
					}
					//01:使用中    02:凍結進    03:凍結出    04:凍結進出 (F1943)
					// 來源倉 : 只檢查 03:凍結出    04:凍結進出
					// 目的倉 : 只檢查 02:凍結進    04:凍結進出
					//檢查來源儲位是否凍結
					var srcF1912 = srcF1912s.First(o => o.LOC_CODE == f151002.SRC_LOC_CODE);
					if (srcF1912.NOW_STATUS_ID == "03" || srcF1912.NOW_STATUS_ID == "04")
					{
						message += "該商品來源儲位已凍結\n";
					}
					//檢查目的儲位是否凍結
					var tarF1912 = tarF1912s.First(o => o.LOC_CODE == f151002.TAR_LOC_CODE);
					if (tarF1912.NOW_STATUS_ID == "02" || tarF1912.NOW_STATUS_ID == "04")
					{
						message += "該商品目的儲位已凍結\n";
					}
          if (!string.IsNullOrWhiteSpace(message))
          {
            message = message.TrimEnd('\n');
            return new ExecuteResult { IsSuccessed = false, Message = message };
          }
					#endregion

          var qty = 0;
          if (allocationType == AllocationType.NoSource || allocationType == AllocationType.Both)
            qty = Convert.ToInt32(f151002.TAR_QTY);
          else
            qty = Convert.ToInt32(f151002.A_SRC_QTY);

          details.Add(new AllocationConfirmDetail { Seq = f151002.ALLOCATION_SEQ, TarLocCode = f1510Data.TAR_LOC_CODE, Qty = qty });
        }
      }

			#region 過帳
			var param = new AllocationConfirmParam
			{
				DcCode = f151001.DC_CODE,
				GupCode = gupCode,
				CustCode = custCode,
				AllocNo = f151001.ALLOCATION_NO,
				Operator = Current.Staff,
				Details = details
			};
			sharedService.AllocationConfirm(param, IsAllocationPosting);
      stockService.SaveChange();
			#endregion

      return result ?? (result = new ExecuteResult { IsSuccessed = true });
    }

    #endregion

    #region 調撥單維護

    public IQueryable<F1510Data> GetF1510Data(string dcCode, string gupCode, string custCode, string allocationNo,
      string allocationDate, string status, string userId, string makeNo, DateTime enterDate, string srcLocCode)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF1510Data(dcCode, gupCode, custCode, allocationNo, allocationDate, status, userId, makeNo, enterDate, srcLocCode);
    }


    #endregion

    #region 調撥單維護過帳 - 序號綁儲位

    public IQueryable<AllocationBundleSerialLocCount> GetAllocationBundleSerialLocCount(string dcCode, string gupCode,
      string custCode, string allocationNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetAllocationBundleSerialLocCount(dcCode, gupCode, custCode, allocationNo, Current.Staff);
    }

    public IQueryable<F15100101Data> GetF15100101Data(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF15100101Data(dcCode, gupCode, custCode, allocationNo, Current.Staff);
    }

    /// <summary>
    /// 取得調入調撥單序號已綁儲位資料
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="checkSerialNo">是否已刷讀序號(0:未刷讀,1:已刷讀)</param>
    /// <returns></returns>
    public IQueryable<F1510BundleSerialLocData> GetF1510BundleSerialLocDatas(string dcCode, string gupCode,
      string custCode, string allocationNo, string checkSerialNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF1510BundleSerialLocDatas(dcCode, gupCode, custCode, allocationNo, checkSerialNo, Current.Staff);
    }

    /// <summary>
    /// 調入調撥單新增或更新序號綁商品刷讀資料
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="serialorBoxNo">序號或箱號</param>
    /// <param name="locCode">儲位代號</param>
    /// <param name="itemCode">品號</param>
    /// <returns></returns>
    public ExecuteResult InsertOrUpdateF1510BundleSerialLocData(string dcCode, string gupCode,
      string custCode, string allocationNo, string serialorBoxNo, string locCode, string itemCode)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var f151001 = f151001Repo.Find(
        o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
      var allocationType = AllocationType.Both;
      if (string.IsNullOrEmpty(f151001.TAR_WAREHOUSE_ID))
        allocationType = AllocationType.NoTarget;
      else if (string.IsNullOrEmpty(f151001.SRC_WAREHOUSE_ID))
        allocationType = AllocationType.NoSource;

      var f15100101Repo = new F15100101Repository(Schemas.CoreSchema, _wmsTransaction);
      var data =
        f15100101Repo.GetDatasByTrueAndCondition(
          o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo)
          .ToList();
      var maxLogSeq = (data.Any()) ? data.Max(o => o.LOG_SEQ) : 0;
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var unCheckSerialNoF151002List =
    f151002Repo.AsForUpdate()
      .GetDatasByTrueAndCondition(
        o =>
          o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo).Where(x => new List<string> { "0", "1" }.Contains(x.STATUS)).ToList();
      return CheckAndSaveScanSerial(dcCode, gupCode, custCode, allocationNo, serialorBoxNo, locCode, itemCode, ref maxLogSeq, unCheckSerialNoF151002List, allocationType);
    }

    public ExecuteResult ImportSerialNoLoc(string dcCode, string gupCode, string custCode, string allocationNo, List<ImportBundleSerialLoc> importBundleSerialLocList)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var f151001 = f151001Repo.Find(
        o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
      var allocationType = AllocationType.Both;
      if (string.IsNullOrEmpty(f151001.TAR_WAREHOUSE_ID))
        allocationType = AllocationType.NoTarget;
      else if (string.IsNullOrEmpty(f151001.SRC_WAREHOUSE_ID))
        allocationType = AllocationType.NoSource;

      var f15100101Repo = new F15100101Repository(Schemas.CoreSchema, _wmsTransaction);
      var data =
        f15100101Repo.GetDatasByTrueAndCondition(
          o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo)
          .ToList();
      var maxLogSeq = (data.Any()) ? data.Max(o => o.LOG_SEQ) : 0;
      var sharedService = new SharedService();
      var serialNoService = new SerialNoService();
      var f1912Repo = new F1912Repository(Schemas.CoreSchema);
      int successCount = 0;
      int failureCount = 0;
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var unCheckSerialNoF151002List =
      f151002Repo.AsForUpdate()
        .GetDatasByTrueAndCondition(
          o =>
            o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo).ToList();

      foreach (var importBundleSerialLoc in importBundleSerialLocList)
      {
        if (string.IsNullOrEmpty(importBundleSerialLoc.LOC_CODE))
        {
          failureCount++;
          maxLogSeq++;
          f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, "",
            importBundleSerialLoc.SERIAL_NO, importBundleSerialLoc.LOC_CODE, "", false, "儲位不可為空白"));
          continue;
        }
        if (string.IsNullOrEmpty(importBundleSerialLoc.SERIAL_NO))
        {
          failureCount++;
          maxLogSeq++;
          f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, "",
            importBundleSerialLoc.SERIAL_NO, importBundleSerialLoc.LOC_CODE, "", false, "序號不可為空白"));
          continue;
        }
        var item = f1912Repo.Find(o => o.DC_CODE == dcCode && o.LOC_CODE == importBundleSerialLoc.LOC_CODE);
        if (item != null)
        {
          var serialItem = serialNoService.GetSerialItem(gupCode, custCode, importBundleSerialLoc.SERIAL_NO);
          if (!serialItem.Checked)
          {
            failureCount++;
            maxLogSeq++;
            f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, "",
              importBundleSerialLoc.SERIAL_NO, importBundleSerialLoc.LOC_CODE, "", false, serialItem.Message));
          }
          else
          {
            var result = sharedService.CheckLocCode(importBundleSerialLoc.LOC_CODE, dcCode, item.WAREHOUSE_ID, Current.Staff, serialItem.ItemCode);
            if (!result.IsSuccessed)
            {
              failureCount++;
              maxLogSeq++;
              f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, "",
                importBundleSerialLoc.SERIAL_NO, importBundleSerialLoc.LOC_CODE, "", false, result.Message));
            }
            else
            {
              var checkResult = CheckAndSaveScanSerial(dcCode, gupCode, custCode, allocationNo, importBundleSerialLoc.SERIAL_NO,
                importBundleSerialLoc.LOC_CODE, serialItem.ItemCode, ref maxLogSeq, unCheckSerialNoF151002List, allocationType);
              if (checkResult.IsSuccessed)
                successCount++;
              else
                failureCount++;
            }
          }

        }
        else
        {
          failureCount++;
          maxLogSeq++;
          f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, "",
            importBundleSerialLoc.SERIAL_NO, importBundleSerialLoc.LOC_CODE, "", false, "儲位不存在"));
        }
      }
      return new ExecuteResult { IsSuccessed = true, Message = string.Format("匯入成功{0}筆,失敗{1}筆,共{2}筆", successCount, failureCount, importBundleSerialLocList.Count) };
    }

    private ExecuteResult CheckAndSaveScanSerial(string dcCode, string gupCode,
      string custCode, string allocationNo, string serialorBoxNo, string locCode, string itemCode, ref long maxLogSeq, List<F151002> unCheckSerialNoF151002List, AllocationType allocationType)
    {
      var f15100101Repo = new F15100101Repository(Schemas.CoreSchema, _wmsTransaction);
      var result = new ExecuteResult { IsSuccessed = true, Message = "" };
      var serialNoService = new SerialNoService();
      var barcodeData = serialNoService.BarcodeInspection(gupCode, custCode, serialorBoxNo);
      //如果是barcode是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
      if (barcodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
        barcodeData.Barcode = BarcodeType.SerialNo;

      //組合商品處理
      var combinF2501List = new List<F2501>();
      var combinItemCode = string.Empty;
      var isCombinItem = serialNoService.IsCombinItem(gupCode, custCode, serialorBoxNo, out combinF2501List, out combinItemCode);
      if (isCombinItem)
      {
        itemCode = combinItemCode;
        var item = combinF2501List.FirstOrDefault(o => o.ITEM_CODE == combinItemCode);
        if (item != null)
          serialorBoxNo = item.SERIAL_NO;
      }

      var serialNoResults = serialNoService.CheckSerialNoFull(dcCode, gupCode, custCode, itemCode, serialorBoxNo, allocationType == AllocationType.NoTarget ? "C1" : "A1",
        ProcessWork.ScanSerial, "A1,C1,D2");
      //寫入序號刷讀log F15100101
      if (!serialNoResults.First().Checked)
      {
        maxLogSeq++;
        f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, itemCode,
          serialNoResults.First().SerialNo, locCode,
          serialNoResults.First().CurrentlyStatus, serialNoResults.First().Checked, serialNoResults.First().Message));
        return new ExecuteResult { IsSuccessed = false, Message = serialNoResults.First().Message };
      }


      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var unCheckSerialNoF151002Item = unCheckSerialNoF151002List.Where(o => o.ITEM_CODE == itemCode);
      if (!unCheckSerialNoF151002Item.Any(o => o.CHECK_SERIALNO == "0"))
      {
        maxLogSeq++;
        f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, itemCode,
          serialorBoxNo, locCode,
          "", false, "此商品已完全刷讀完畢,無法在刷讀此" + barcodeData.BarcodeText));
        return new ExecuteResult { IsSuccessed = false, Message = "此商品已完全刷讀完畢,無法在刷讀此" + barcodeData.BarcodeText };

      }
      var serialNoList = serialNoResults.Select(o => o.SerialNo).ToList();
      if (unCheckSerialNoF151002List.Any(
        o => o.CHECK_SERIALNO == "1" && serialNoList.Contains(o.SERIAL_NO)))
      {
        maxLogSeq++;
        f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, itemCode,
          serialorBoxNo, locCode,
          "", false, "此" + barcodeData.BarcodeText + "已刷讀"));
        return new ExecuteResult { IsSuccessed = false, Message = "此" + barcodeData.BarcodeText + "已刷讀" };
      }

      if (unCheckSerialNoF151002Item.Sum(o => allocationType == AllocationType.NoTarget ? o.SRC_QTY : o.TAR_QTY) < serialNoList.Count)
      {
        maxLogSeq++;
        f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, itemCode,
          serialorBoxNo, locCode,
          "", false, "商品數量不足"));
        return new ExecuteResult { IsSuccessed = false, Message = "商品數量不足" };
      }
      if (!unCheckSerialNoF151002Item.Any(o => serialNoList.Contains(o.SERIAL_NO)))
      {
        maxLogSeq++;
        f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, 0, maxLogSeq, itemCode,
          serialorBoxNo, locCode,
          "", false, "此調撥單無此商品序號"));
        return new ExecuteResult { IsSuccessed = false, Message = "此調撥單無此商品序號" };

      }

      foreach (var serialNoResult in serialNoResults)
      {
        var item = unCheckSerialNoF151002Item.FirstOrDefault(o => o.SERIAL_NO == serialNoResult.SerialNo);
        if (item != null)
        {
          if (allocationType == AllocationType.NoTarget && item.SRC_LOC_CODE != locCode)
          {
            maxLogSeq++;
            f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, item.ALLOCATION_SEQ, maxLogSeq, itemCode,
              serialNoResult.SerialNo, locCode,
              serialNoResult.CurrentlyStatus, false, "請刷讀下架儲位"));
            return new ExecuteResult { IsSuccessed = false, Message = "請刷讀下架儲位" };
          }
          item.CHECK_SERIALNO = "1";
          if (allocationType != AllocationType.NoTarget)
            item.TAR_LOC_CODE = locCode;
          f151002Repo.Update(item);
          maxLogSeq++;
          f15100101Repo.Add(CreateF15100101(dcCode, gupCode, custCode, allocationNo, item.ALLOCATION_SEQ, maxLogSeq,
            serialNoResult.ItemCode,
            serialNoResult.SerialNo, locCode,
            serialNoResult.CurrentlyStatus, serialNoResult.Checked, serialNoResult.Message));
        }
      }

      return result;
    }


    private F15100101 CreateF15100101(string dcCode, string gupCode, string custCode, string allocationNo, short allocationSeq,
      long logSeq, string itemCode, string serialNo, string locCode, string status, bool isPass, string message)
    {
      var f15100101 = new F15100101
      {
        DC_CODE = dcCode,
        GUP_CODE = gupCode,
        CUST_CODE = custCode,
        ALLOCATION_NO = allocationNo,
        ALLOCATION_SEQ = allocationSeq,
        ISPASS = isPass ? "1" : "0",
        MESSAGE = message,
        ITEM_CODE = itemCode,
        SERIAL_NO = serialNo,
        SERIAL_STATUS = status,
        LOG_SEQ = logSeq,
        LOC_CODE = locCode,
        STATUS = "0"
      };
      return f15100101;
    }
    /// <summary>
    /// 刪除刷讀資料 
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="itemCode">商品代號</param>
    /// <param name="serialNo">序號</param>
    /// <returns></returns>
    public ExecuteResult DeleteF1510BundleSerialLocData(string dcCode, string gupCode,
      string custCode, string allocationNo, string itemCode, string serialNo)
    {
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var orginalData = f151002Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, allocationNo, itemCode);
      var serialNoService = new SerialNoService();
      var barcodeData = serialNoService.BarcodeInspection(gupCode, custCode, serialNo.Remove(serialNo.Length - 3, 3) + "000");
      //如果是barcode是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
      if (barcodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
        barcodeData.Barcode = BarcodeType.SerialNo;
      var serialNoList = new List<string>();
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
      var item = f2501Repo.Find(
        o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == itemCode && o.SERIAL_NO == serialNo);
      switch (barcodeData.Barcode)
      {
        case BarcodeType.BatchNo:
          if (serialNoService.IsBatchNoItem(gupCode, custCode, itemCode)) //必須商品類別為儲值卡盒號
          {
            for (var i = 1; i <= 200; i++)
              serialNoList.Add(serialNo.Remove(serialNo.Length - 3, 3) + i.ToString().PadLeft(3, '0'));
            //檢查已榜定儲位序號數是否是200個
            var dataCount = orginalData.Count(o => !string.IsNullOrEmpty(o.SERIAL_NO) && serialNoList.Contains(o.SERIAL_NO));

            if (dataCount != 200) //不等於200個 代表他是一般序號
            {
              serialNoList.Clear();
              serialNoList.Add(serialNo);
            }
          }
          else
            serialNoList.Add(serialNo);
          break;
        case BarcodeType.BoxSerial:
          serialNoList.AddRange(f2501Repo.GetDatasByBoxSerial(gupCode, custCode, item.BOX_SERIAL).Select(o => o.SERIAL_NO).ToList());
          break;
        case BarcodeType.CaseNo:
          serialNoList.AddRange(f2501Repo.GetDatasByCaseNo(gupCode, custCode, item.CASE_NO).Select(o => o.SERIAL_NO).ToList());
          break;
        case BarcodeType.SerialNo:
          serialNoList.Add(serialNo);
          break;
      }
      var result = new ExecuteResult { IsSuccessed = true, Message = "" };
      var f15100101Repo = new F15100101Repository(Schemas.CoreSchema, _wmsTransaction);
      //var updateF151002List = new List<F151002>();
      foreach (var sn in serialNoList)
      {
        var orginalItem = orginalData.FirstOrDefault(o => o.SERIAL_NO == sn);
        if (orginalItem != null)
        {
          orginalItem.CHECK_SERIALNO = "0";
          f151002Repo.Update(orginalItem);
        }
        var f15100101 = f15100101Repo.AsForUpdate()
          .GetDatasByTrueAndCondition(
            o =>
              o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo &&
              o.SERIAL_NO == serialNo && o.STATUS == "0" && o.ISPASS == "1")
          .FirstOrDefault();
        if (f15100101 != null)
        {
          f15100101.STATUS = "9";
          f15100101Repo.Update(f15100101);
        }

      }
      return result;
    }
    #endregion

  }
}

