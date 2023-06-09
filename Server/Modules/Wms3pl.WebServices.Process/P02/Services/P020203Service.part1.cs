using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P19.Services;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
  public partial class P020203Service
  {

    #region 取得商品資訊F1903
    private List<F1903> _tempF1903List;
    /// <summary>
    /// 取得商品資訊F1903
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    protected F1903 GetF1903(string gupCode, string custCode, string itemCode)
    {
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      if (_tempF1903List == null)
        _tempF1903List = new List<F1903>();
      var f1903 = _tempF1903List.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode);
      if (f1903 == null)
      {
        f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode);
        if (f1903 != null)
          _tempF1903List.Add(f1903);
      }
      return f1903;
    }
    #endregion

    #region 產生驗收檔F020201
    /// <summary>
    /// 產生驗收檔F020201
    /// </summary>
    /// <param name="tmp"></param>
    /// <returns></returns>
    protected F020201 CreateF020201(F02020101 tmp, String RT_MODE = "0")
    {
      return new F020201
      {
        CHECK_ITEM = tmp.CHECK_ITEM,
        CHECK_QTY = tmp.CHECK_QTY,
        CHECK_SERIAL = tmp.CHECK_SERIAL,
        DC_CODE = tmp.DC_CODE,
        GUP_CODE = tmp.GUP_CODE,
        CUST_CODE = tmp.CUST_CODE,
        ISPRINT = tmp.ISPRINT,
        ISSPECIAL = tmp.ISSPECIAL,
        ISUPLOAD = tmp.ISUPLOAD,
        ITEM_CODE = tmp.ITEM_CODE,
        MADE_DATE = tmp.MADE_DATE,
        ORDER_QTY = tmp.ORDER_QTY,
        PURCHASE_NO = tmp.PURCHASE_NO,
        PURCHASE_SEQ = tmp.PURCHASE_SEQ,
        RECV_QTY = tmp.RECV_QTY,
        SPECIAL_CODE = tmp.SPECIAL_CODE,
        SPECIAL_DESC = tmp.SPECIAL_DESC,
        STATUS = tmp.STATUS,
        VALI_DATE = tmp.VALI_DATE,
        VNR_CODE = tmp.VNR_CODE,
        RT_NO = tmp.RT_NO,
        RT_SEQ = tmp.RT_SEQ,
        RECE_DATE = DateTime.Today,
        IN_DATE = tmp.IN_DATE,
        QUICK_CHECK = tmp.QUICK_CHECK,
        TARWAREHOUSE_ID = tmp.TARWAREHOUSE_ID,
        MAKE_NO = tmp.MAKE_NO,
        RT_MODE = RT_MODE
      };
    }
    #endregion

    #region 產生刷讀紀錄F02020104 By互賣訂單,內部交易 包裝刷驗紀錄檔
    /// <summary>
    /// 互賣訂單,內部交易 包裝刷驗紀錄檔
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="f05500101s"></param>
    /// <param name="maxLogSeq"></param>
    /// <returns></returns>
    public List<F02020104> CreateF02020104s(F02020101 tmp, List<F05500101> f05500101s, int maxLogSeq)
    {
      return f05500101s.Select((x, index) => new F02020104
      {
        DC_CODE = tmp.DC_CODE,
        GUP_CODE = tmp.GUP_CODE,
        CUST_CODE = tmp.CUST_CODE,
        PURCHASE_NO = tmp.PURCHASE_NO,
        PURCHASE_SEQ = tmp.PURCHASE_SEQ,
        SERIAL_NO = x.SERIAL_NO,
        ITEM_CODE = x.ITEM_CODE,
        STATUS = string.IsNullOrWhiteSpace(x.STATUS) ? null : x.STATUS,
        ISPASS = "1",
        LOG_SEQ = index + maxLogSeq
      }).ToList();
    }
    #endregion

    #region 序號檢核與更新序號狀態
    /// <summary>
    /// 序號檢核與更新序號狀態
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="ordProp"></param>
    /// <param name="f020302Datas"></param>
    /// <returns></returns>
    protected ExecuteResult UpdateF2501(F02020101 tmp, string ordProp, List<F020302> f020302s, List<F02020109> f02020109s)
    {
			var logSvc = new LogService("商品檢驗_" + DateTime.Now.ToString("yyyyMMdd"));
			SerialNoService.CommonService = CommonService;
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = SerialNoService.CheckLargeSerialNoFull(tmp.DC_CODE, tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, f020302s.Select(x => x.SERIAL_NO).Distinct().ToArray(), "A1").ToList();
			if(result.Any(x=> !x.Checked))
			{
				return new ExecuteResult(false, string.Join(Environment.NewLine, result.Select(x => x.Message)));
			}
			else
			{
				var addF2501List = new List<F2501>();
				var updF2501List = new List<F2501>();
				var delSnList = new List<string>();
        Boolean needMarkActivated;
				foreach (var item in f020302s)
				{
					var logDt = DateTime.Now;
					var checkSerialList = result.First(x => x.SerialNo == item.SERIAL_NO);

          // No.2091 若當前序號在F02020109中查得到，更新F2501需要註記為不良品
          if (f02020109s.Select(o => o.SERIAL_NO).Contains(item.SERIAL_NO))
            needMarkActivated = true;
          else
            needMarkActivated = false;

          //更新F2501
          var updResult = SerialNoService.UpdateSerialNoFull(ref addF2501List,ref updF2501List,ref delSnList,item.DC_CODE, item.GUP_CODE, item.CUST_CODE, "A1", checkSerialList,
														tmp.PURCHASE_NO, tmp.VNR_CODE, item.VALID_DATE, null,
														item.PO_NO, ordProp, null, item.PUK,
														item.CELL_NUM, item.BATCH_NO, null, needMarkActivated);
					if (!updResult.IsSuccessed)
						return updResult;
					else
						item.STATUS = "1";
					logSvc.Log($"更新序號{item.SERIAL_NO} {logSvc.DateDiff(logDt, DateTime.Now)}");
				}

				if (delSnList.Any())
					f2501Repo.DeleteBySnList(tmp.GUP_CODE, tmp.CUST_CODE, delSnList);
				if (addF2501List.Any())
					f2501Repo.BulkInsert(addF2501List);
				if (updF2501List.Any())
					f2501Repo.BulkUpdate(updF2501List);
			}
      return new ExecuteResult(true);
    }

    #endregion

    #region 產生進貨暫存倉庫存資料
    Func<F1913, string, string, string, string, string, DateTime, DateTime, string, string, string, string, string, bool> F1913Func = Find1913;
    private static bool Find1913(F1913 f1913, string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
    {
      return f1913.DC_CODE == dcCode && f1913.GUP_CODE == gupCode && f1913.CUST_CODE == custCode && f1913.LOC_CODE == locCode && f1913.ITEM_CODE == itemCode && f1913.VALID_DATE == validDate && f1913.ENTER_DATE == enterDate && f1913.VNR_CODE == vnrCode && f1913.SERIAL_NO == serialNo && f1913.BOX_CTRL_NO == boxCtrlNo && f1913.PALLET_CTRL_NO == palletCtrlNo && f1913.MAKE_NO == makeNo;
    }

    /// <summary>
    /// 產生進貨暫存倉庫存資料
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="locCode"></param>
    /// <param name="f020302s"></param>
    /// <param name="returnStocks"></param>
    protected void InsertOrUpdateStock(F02020101 tmp, string locCode, List<F020302> f020302s, ref List<F1913> returnStocks)
    {
      var validDate = DateTime.MaxValue.Date;
      if (tmp.VALI_DATE.HasValue)
        validDate = tmp.VALI_DATE.Value;
      var enterDate = DateTime.Today;
      var vnrCode = "000000";
      var palletCtrlNo = "0";
      var makeNo = tmp.MAKE_NO?.Trim();
      if (string.IsNullOrWhiteSpace(makeNo))
        makeNo = "0";
      var boxCtrlNo = "0";

      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var serialData = new List<string>() { "0" };
      var f1903 = GetF1903(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE);
      if (f1903.BUNDLE_SERIALLOC == "1") //序號綁儲位
      {
        serialData = f020302s.Select(o => o.SERIAL_NO).ToList();
        if (!serialData.Any())
          serialData.Add("0");
      }
      bool hasSerial = serialData.Any(x => x != "0");

      foreach (var serialNo in serialData)
      {
        var returnStock = returnStocks.FirstOrDefault(o => F1913Func(o, tmp.DC_CODE, tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, locCode, validDate, enterDate, vnrCode, serialNo, boxCtrlNo, palletCtrlNo, makeNo));
        var f1913 = returnStock ??
              f1913Repo.Find(o => o.DC_CODE == tmp.DC_CODE && o.GUP_CODE == tmp.GUP_CODE && o.CUST_CODE == tmp.CUST_CODE && o.ITEM_CODE == tmp.ITEM_CODE && o.LOC_CODE == locCode && o.VALID_DATE == validDate && o.ENTER_DATE == enterDate && o.VNR_CODE == vnrCode && o.SERIAL_NO == serialNo && o.BOX_CTRL_NO == boxCtrlNo && o.PALLET_CTRL_NO == palletCtrlNo && o.MAKE_NO == makeNo);
        if (f1913 != null)
        {
          f1913.QTY += (!hasSerial) ? (tmp.RECV_QTY ?? 0) : 1;
          if (returnStock == null)
            returnStocks.Add(f1913);
        }
        else
        {
          f1913 = new F1913
          {
            DC_CODE = tmp.DC_CODE,
            GUP_CODE = tmp.GUP_CODE,
            CUST_CODE = tmp.CUST_CODE,
            ITEM_CODE = tmp.ITEM_CODE,
            LOC_CODE = locCode,
            VALID_DATE = validDate,
            ENTER_DATE = enterDate,
            VNR_CODE = vnrCode,
            SERIAL_NO = serialNo,
            QTY = (!hasSerial) ? (tmp.RECV_QTY ?? 0) : 1,
            BOX_CTRL_NO = boxCtrlNo,
            PALLET_CTRL_NO = palletCtrlNo,
            MAKE_NO = makeNo
          };
          returnStocks.Add(f1913);
        }
      }
    }

    #endregion

    #region 產生商品驗收調撥單
    /// <summary>
    /// 產生商品驗收調撥單
    /// </summary>
    /// <returns></returns>
    protected ReturnNewAllocationResult CreateAcceptanceAllocation(AcceptanceConfirmParam acp, F1912 srcLoc, List<F1913> returnStocks, List<F02020101> f02020101s)
    {

      var group = f02020101s.GroupBy(x => new { x.ITEM_CODE, x.VALI_DATE, x.MAKE_NO, x.TARWAREHOUSE_ID });
      var newAllocationParam = new NewAllocationItemParam
      {
        GupCode = acp.GupCode,
        CustCode = acp.CustCode,
        AllocationDate = DateTime.Today,
        SourceType = "04",
        SourceNo = acp.PurchaseNo,
        IsExpendDate = true,
        SrcDcCode = acp.DcCode,
        SrcWarehouseId = srcLoc.WAREHOUSE_ID,
        TarDcCode = acp.DcCode,
        AllocationType = AllocationType.Both,
        ReturnStocks = returnStocks,
        isIncludeResupply = true,
        SrcStockFilterDetails = group.Select((x, rowIndex) => new StockFilter
        {
          DataId = rowIndex,
          ItemCode = x.Key.ITEM_CODE,
          LocCode = srcLoc.LOC_CODE,
          Qty = x.Sum(y => y.RECV_QTY) ?? 0,
          ValidDates = x.Key.VALI_DATE.HasValue ? new List<DateTime> { x.Key.VALI_DATE.Value } : new List<DateTime>(),
          EnterDates = new List<DateTime> { DateTime.Today },
          BoxCtrlNos = new List<string> { "0" },
          MakeNos = string.IsNullOrWhiteSpace(x.Key.MAKE_NO) ? new List<string> { "0" } : new List<string> { x.Key.MAKE_NO?.Trim() },
        }).ToList(),
        SrcLocMapTarLocs = group.Select((x, rowIndex) => new SrcLocMapTarLoc
        {
          DataId = rowIndex,
          ItemCode = x.Key.ITEM_CODE,
          SrcLocCode = srcLoc.LOC_CODE,
          TarWarehouseId = x.Key.TARWAREHOUSE_ID,
          ValidDate = x.Key.VALI_DATE,
          MakeNo = x.Key.MAKE_NO
        }).ToList()
      };

      // 將序號的ItemCode 填入SerialNos
      var serialItemCodes = returnStocks.Where(x => x.SERIAL_NO != "0").Select(x => x.ITEM_CODE).Distinct().ToList();

      serialItemCodes.ForEach(itemCode =>
      {
        int index = 0;

              // 找出符合ItemCode執行迴圈
              newAllocationParam.SrcStockFilterDetails.Where(x => x.ItemCode == itemCode).ToList().ForEach(item =>
              {
                  // 找出符合的商品
                  var serialNos = returnStocks.Where(x => x.ITEM_CODE == item.ItemCode &&
                                                          x.LOC_CODE == item.LocCode &&
                                                          x.VALID_DATE == item.ValidDates.FirstOrDefault() &&
                                                          x.ENTER_DATE == item.EnterDates.FirstOrDefault() &&
                                                          x.BOX_CTRL_NO == item.BoxCtrlNos.FirstOrDefault() &&
                                                          x.MAKE_NO == item.MakeNos.FirstOrDefault());

            if (serialNos.Any())
            {
                    // 因上層有將不良品往最後擺，所以只需依順序找出SerialNo
                    int takeCnt = Convert.ToInt32(item.Qty);
              item.SerialNos = serialNos.Skip(index).Take(takeCnt).Select(x => x.SERIAL_NO).ToList();
              index += takeCnt;
            }
          });
      });

      //7.設定預設的建議儲位
      if (acp.IsPickLocFirst)
      {
        SetDefaultSugLocCode(acp.DcCode, acp.GupCode, acp.CustCode, newAllocationParam.SrcLocMapTarLocs);
      }

      var sharedSerivce = new SharedService(_wmsTransaction);
      var returnAllocationResult = sharedSerivce.CreateOrUpdateAllocation(newAllocationParam);
      if (returnAllocationResult.Result.IsSuccessed)
        sharedSerivce.BulkAllocationToAllDown(returnAllocationResult.AllocationList);
      return returnAllocationResult;
    }

    #endregion

    #region 產生商品驗收貼紙
    /// <summary>
    /// 產生商品驗收貼紙
    /// </summary>
    /// <param name="acp"></param>
    /// <param name="newAllocations"></param>
    /// <param name="f02020101s"></param>
    /// <returns></returns>
    private void CreateAcceptanceStickers(AcceptanceConfirmParam acp, List<ReturnNewAllocation> newAllocations, List<F02020101> f02020101s)
    {
      var addF010203List = new List<F010203>();
      var details = from m in newAllocations
                    from d in m.Details
                    select d;
      var f190305Repo = new F190305Repository(Schemas.CoreSchema);
      var f190301Repo = new F190301Repository(Schemas.CoreSchema);
      var f9100302Repo = new F91000302Repository(Schemas.CoreSchema);
      var f010203Repo = new F010203Repository(Schemas.CoreSchema, _wmsTransaction);
      var itemService = new ItemService();
      var itemCodes = details.Select(x => x.ITEM_CODE).Distinct().ToList();
      var itemUnitQtyList = itemService.GetSysItemUnitQtyList(acp.GupCode, acp.CustCode, itemCodes, Datas.Shared.Enums.SysUnit.Case);
      var itemUnitQtyByUnitNameList = itemService.GetItemUnitQtyByUnitNameList(acp.GupCode, acp.CustCode, itemCodes, "小包裝");
      var f190305s = f190305Repo.GetDatasByItems(acp.GupCode, acp.CustCode, itemCodes);
      var f91000302s = f9100302Repo.GetAccUnitList("001");
      var gRecvDatas = f02020101s.GroupBy(x => new { x.RT_NO, x.ITEM_CODE, MAKE_NO = string.IsNullOrWhiteSpace(x.MAKE_NO) ? "0" : x.MAKE_NO, x.VALI_DATE }).ToList();

      //棧板編號
      var seq = 1;
      foreach (var item in details)
      {
        var f1903 = GetF1903(item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE);
        var f190305 = f190305s.FirstOrDefault(x => x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE);
        var f91000302 = f91000302s.FirstOrDefault(x => x.ACC_UNIT == f1903.ITEM_UNIT);
        //共用的計算資料
        var itemUnits = itemUnitQtyList.Where(x => x.ItemCode == item.ITEM_CODE).FirstOrDefault();
        //箱入數
        var inCaseQty = itemUnits == null ? item.TAR_QTY : itemUnits.Qty;
        //小包裝數
        var itenUnitQty = itemUnitQtyByUnitNameList.Where(x => x.ItemCode == item.ITEM_CODE).FirstOrDefault();
        var inPackageQty = itenUnitQty != null && itenUnitQty.Qty > 0 ? itenUnitQty.Qty : default(int?);

        //上架數計算
        //上架箱數
        var totalTarCaseQty = item.TAR_QTY / inCaseQty;
        //上架零散數
        var otherTarQty = item.TAR_QTY % inCaseQty;
        //棧板最多可放幾箱
        var palletMaxCaseQty = (f190305 == null || f190305.PALLET_LEVEL_CASEQTY < 1 || f190305.PALLET_LEVEL_CNT < 1) ? totalTarCaseQty : f190305.PALLET_LEVEL_CASEQTY * f190305.PALLET_LEVEL_CNT;
        //使用棧板數
        var palletQty = totalTarCaseQty / palletMaxCaseQty + (totalTarCaseQty % palletMaxCaseQty > 0 ? 1 : 0);
        if (palletQty == 0)
          palletQty = 1; //至少一板
                         //驗收數計算
                         //取得驗收單資料
        var recvItem = gRecvDatas.FirstOrDefault(x => x.Key.RT_NO == acp.RTNo && x.Key.ITEM_CODE == item.ITEM_CODE && x.Key.VALI_DATE == item.VALID_DATE && x.Key.MAKE_NO == item.MAKE_NO);
        var recvQty = recvItem.Sum(x => x.RECV_QTY ?? 0);
        //驗收箱數
        long totalRecvCaseQty = recvQty / inCaseQty;
        //驗收零散數
        long otherRecvQty = recvQty % inCaseQty;

        for (var i = 0; i < palletQty; i++)
        {
          //上架數計算
          //上架箱數
          long tarCaseQty = 0;
          //上架零散數
          long tarOtherQty = 0;
          if (totalTarCaseQty > palletMaxCaseQty)
          {
            tarCaseQty = palletMaxCaseQty;
            tarOtherQty = 0;
            totalTarCaseQty -= palletMaxCaseQty;
          }
          else
          {
            tarCaseQty = totalTarCaseQty;
            tarOtherQty = otherTarQty;
          }

          f010203Repo.Add(new F010203
          {
            DC_CODE = item.DC_CODE,
            GUP_CODE = item.GUP_CODE,
            CUST_CODE = item.CUST_CODE,
            STICKER_NO = item.ALLOCATION_NO + seq.ToString().PadLeft(4, '0'),
            STOCK_NO = acp.PurchaseNo,
            PALLET_NO = seq.ToString().PadLeft(4, '0'),
            ITEM_CODE = item.ITEM_CODE,
            //商品條碼
            ENA_CODE1 = f1903.EAN_CODE1,
            //外箱條碼
            ENA_CODE3 = f1903.EAN_CODE3,
            //商品箱入數(箱)
            ITEM_CASE_QTY = (int)inCaseQty,
            //商品箱入數(包)
            ITEM_PACKAGE_QTY = inPackageQty,
            //棧板每層箱數
            PALLET_LEVEL_CASEQTY = (f190305 == null) ? 1 : f190305.PALLET_LEVEL_CASEQTY,
            //棧板層數
            PALLET_LEVEL_CNT = (f190305 == null) ? 1 : f190305.PALLET_LEVEL_CNT,
            //訂貨箱數
            ORDER_CASE_QTY = 0,
            //訂貨零散數
            ORDER_OTHER_QTY = 0,
            //入庫日
            ENTER_DATE = item.ENTER_DATE,
            //效期
            VALID_DATE = item.VALID_DATE,
            //驗收單號
            RT_NO = acp.RTNo,
            STICKER_TYPE = "2",
            //儲位
            LOC_CODE = item.SUG_LOC_CODE,
            //調撥單號
            ALLOCATION_NO = item.ALLOCATION_NO,
            //驗收數
            RECV_QTY = recvQty,
            //棧板貼紙數
            STICKER_REF = string.Format("{0}-{1}", i + 1, palletQty),
            //上架數
            TAR_QTY_DESC = string.Format("{0}*{1}{2}", tarCaseQty, inCaseQty, tarOtherQty > 0 ? string.Format("+{0}{1}", tarOtherQty, f91000302 == null ? "" : f91000302.ACC_UNIT_NAME) : " "),
            //驗收數(換算後)
            RECV_QTY_DESC = string.Format("{0}*{1}{2}", totalRecvCaseQty, inCaseQty, otherRecvQty > 0 ? string.Format("+{0}{1}", otherRecvQty, f91000302 == null ? "" : f91000302.ACC_UNIT_NAME) : " ")
          });
          seq++;
        }
      }
    }

    #endregion

    #region 一單一品訂單處理
    /// <summary>
    /// 一單一品訂單處理
    /// </summary>
    /// <param name="notVirtualItemList"></param>
    /// <returns></returns>
    private List<F050001> CreateSingleItemOrders(ref List<F02020101> notVirtualItemList)
    {
      var updF050001List = new List<F050001>();
      var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050002Repo = new F050002Repository(Schemas.CoreSchema);
      var gItem = notVirtualItemList.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE }).Select(x => new { x.Key.DC_CODE, x.Key.GUP_CODE, x.Key.CUST_CODE, x.Key.ITEM_CODE, RECV_QTY = x.Sum(y => y.RECV_QTY) });
      foreach (var item in gItem)
      {
        var data = f050002Repo.GetF05002BySingleItem(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE);
        var sumOrdQty = data.Sum(x => x.ORD_QTY);
        if (!data.Any())
          continue;
        if (sumOrdQty > item.RECV_QTY)
          continue;
        foreach (var orderDetail in data)
        {
          var order = f050001Repo.Find(x => x.DC_CODE == orderDetail.DC_CODE && x.GUP_CODE == orderDetail.GUP_CODE && x.CUST_CODE == orderDetail.CUST_CODE && x.ORD_NO == orderDetail.ORD_NO);
          order.TYPE_ID = "I";//訂單改成進貨暫存倉出貨
          updF050001List.Add(order);
        }
        //驗收數扣除一單一品訂購數
        var tmpListByItems = notVirtualItemList.Where(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE);
        foreach (var tmpItem in tmpListByItems)
        {
          if (tmpItem.RECV_QTY >= sumOrdQty)
          {
            tmpItem.RECV_QTY -= sumOrdQty;
            sumOrdQty = 0;
          }
          else
          {
            sumOrdQty -= tmpItem.RECV_QTY ?? 0;
            tmpItem.RECV_QTY = 0;
          }
        }
      }
      return updF050001List;
    }
    #endregion

    #region  貨主驗收後不進行上傳檔案處理
    /// <summary>
    /// 貨主驗收後不進行上傳檔案處理
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseNo"></param>
    /// <returns></returns>
    public ExecuteResult RecvCompleteCustNoUpload(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, string RT_MODE)
    {
      var f1909Repo = new F1909Repository(Schemas.CoreSchema);
      var f1909 = f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
      //是否驗收後上傳檔案 如果值為0, 且進貨數等於驗收總量則直接更新進倉單狀態至結案
      if (f1909 != null && f1909.ISUPLOADFILE == "0")
      {
        if (RT_MODE == "0")
          PurchaseClosed(dcCode, gupCode, custCode, purchaseNo, rtNo);
        else //RT_MODE=="1" 改呼叫容器用進倉進倉單結案方法
        {
          var P020206Srv = new P020206Service(_wmsTransaction);
          P020206Srv.PurchaseClosed(dcCode, gupCode, custCode, purchaseNo, rtNo);
        }
      }
      return new ExecuteResult(true);
    }
    /// <summary>
    /// 進倉單結案
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseNo"></param>
    /// <param name="rtNo"></param>
    private void PurchaseClosed(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
    {
      var sharedService = new SharedService(_wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010202Repo = new F010202Repository(Schemas.CoreSchema);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010201 = f010201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
      var f010202s = f010202Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo).ToList();
      var f020201s = f020201Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo).ToList();

      //更新驗收檔狀態
      var updF020201s = f020201s.Where(x => x.RT_NO == rtNo).ToList();
      updF020201s.ForEach(x =>
      {
        x.STATUS = "2";//已上傳
        f020201Repo.Update(x);
      });

      // 刪除暫存驗收檔
      f02020101Repo.Delete(x => x.DC_CODE == dcCode
              && x.GUP_CODE == gupCode
              && x.CUST_CODE == custCode
              && x.PURCHASE_NO == purchaseNo
              && x.RT_NO == rtNo);

      //檢核進倉單是否所有商品都已經驗收完成 如果都完成就結案 
      var isAllRecv = true;
      foreach (var item in f010202s)
      {
        var sumRecvQty = f020201s.Where(x => x.PURCHASE_SEQ == item.STOCK_SEQ.ToString()).Sum(x => x.RECV_QTY);
        if (sumRecvQty < item.STOCK_QTY)
        {
          isAllRecv = false;
          break;
        }
      }
      if (isAllRecv)
      {
        //更新進倉單為已結案
        f010201.STATUS = "2";//結案
        f010201Repo.Update(f010201);
        //更新來源單據狀態為已結案
        sharedService.UpdateSourceNoStatus(SourceType.Stock, dcCode, gupCode, custCode, purchaseNo, f010201.STATUS);
      }
    }

    #endregion

    #region 設定預設建議揀貨儲位
    /// <summary>
    /// 設定預設建議儲位(取得此商品已存在庫存的第一個儲位)
    /// 必須倉別為良品倉
    /// 必須儲區型態為揀貨區(A)
    /// 必須數量>0
    /// 必須儲位非凍結進(02)和凍結進出(04)
    /// </summary>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="srcLocMapTarLocs">來源商品儲位對應目的儲位設定</param>
    private void SetDefaultSugLocCode(string dcCode, string gupCode, string custCode, List<SrcLocMapTarLoc> srcLocMapTarLocs)
    {
      var f1913Repo = new F1913Repository(Schemas.CoreSchema);
      var items = f1913Repo.GetSuggestLocsByStock(dcCode, gupCode, custCode, srcLocMapTarLocs.Select(o => o.ItemCode).Distinct().ToList());
      foreach (var item in srcLocMapTarLocs)
      {
        SuggestLocItem loc;
        if (string.IsNullOrWhiteSpace(item.TarWarehouseId)) // 未指定上架倉庫 =>取得此商品庫存第一個儲位
          loc = items.FirstOrDefault(o => o.ITEM_CODE == item.ItemCode);
        else //指定上架倉庫 =>取得此指定上架倉庫此商品庫存第一個儲位
          loc = items.FirstOrDefault(x => x.WAREHOUSE_ID == item.TarWarehouseId && x.ITEM_CODE == item.ItemCode);
        //如果有找到儲位 設定此商品目的儲位
        if (loc != null)
        {
          item.TarLocCode = loc.LOC_CODE;
          item.TarWarehouseId = loc.WAREHOUSE_ID;
        }
      }
    }
    #endregion

    #region 不良品拆單
    public void CreateDefectItem(ref List<F02020101> hasRecvList, List<F02020109> f02020109s)
    {
      hasRecvList = (from A in hasRecvList
                     join B in f02020109s on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, STOCK_NO = A.PURCHASE_NO, STOCK_SEQ = Convert.ToInt32(A.PURCHASE_SEQ) }
                     equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO, B.STOCK_SEQ } into g
                     from C in g.DefaultIfEmpty()
                     select new
                     {
                       // 建立正常調撥單
                       A.PURCHASE_NO,
                       A.PURCHASE_SEQ,
                       A.VNR_CODE,
                       A.ITEM_CODE,
                       A.RECE_DATE,
                       A.VALI_DATE,
                       A.MADE_DATE,
                       A.ORDER_QTY,
                       RECV_QTY = A.RECV_QTY - g.Sum(x => x.DEFECT_QTY ?? 1),
                       A.CHECK_QTY,
                       A.STATUS,
                       A.CHECK_ITEM,
                       A.CHECK_SERIAL,
                       A.ISPRINT,
                       A.ISUPLOAD,
                       A.DC_CODE,
                       A.GUP_CODE,
                       A.CUST_CODE,
                       A.CRT_STAFF,
                       A.CRT_DATE,
                       A.UPD_STAFF,
                       A.UPD_DATE,
                       A.CRT_NAME,
                       A.UPD_NAME,
                       A.ISSPECIAL,
                       A.SPECIAL_DESC,
                       A.SPECIAL_CODE,
                       A.RT_NO,
                       A.RT_SEQ,
                       A.IN_DATE,
                       A.TARWAREHOUSE_ID,
                       A.QUICK_CHECK,
                       A.MAKE_NO
                     }).Distinct().Select(x => new F02020101
                     {
                       PURCHASE_NO = x.PURCHASE_NO,
                       PURCHASE_SEQ = x.PURCHASE_SEQ,
                       VNR_CODE = x.VNR_CODE,
                       ITEM_CODE = x.ITEM_CODE,
                       RECE_DATE = x.RECE_DATE,
                       VALI_DATE = x.VALI_DATE,
                       MADE_DATE = x.MADE_DATE,
                       ORDER_QTY = x.ORDER_QTY,
                       RECV_QTY = x.RECV_QTY,
                       CHECK_QTY = x.CHECK_QTY,
                       STATUS = x.STATUS,
                       CHECK_ITEM = x.CHECK_ITEM,
                       CHECK_SERIAL = x.CHECK_SERIAL,
                       ISPRINT = x.ISPRINT,
                       ISUPLOAD = x.ISUPLOAD,
                       DC_CODE = x.DC_CODE,
                       GUP_CODE = x.GUP_CODE,
                       CUST_CODE = x.CUST_CODE,
                       CRT_STAFF = x.CRT_STAFF,
                       CRT_DATE = x.CRT_DATE,
                       UPD_STAFF = x.UPD_STAFF,
                       UPD_DATE = x.UPD_DATE,
                       CRT_NAME = x.CRT_NAME,
                       UPD_NAME = x.UPD_NAME,
                       ISSPECIAL = x.ISSPECIAL,
                       SPECIAL_DESC = x.SPECIAL_DESC,
                       SPECIAL_CODE = x.SPECIAL_CODE,
                       RT_NO = x.RT_NO,
                       RT_SEQ = x.RT_SEQ,
                       IN_DATE = x.IN_DATE,
                       TARWAREHOUSE_ID = x.TARWAREHOUSE_ID,
                       QUICK_CHECK = x.QUICK_CHECK,
                       MAKE_NO = x.MAKE_NO
                     }).Concat(from D in hasRecvList
                               join E in f02020109s on new { D.DC_CODE, D.GUP_CODE, D.CUST_CODE, STOCK_NO = D.PURCHASE_NO, STOCK_SEQ = Convert.ToInt32(D.PURCHASE_SEQ) }
                               equals new { E.DC_CODE, E.GUP_CODE, E.CUST_CODE, E.STOCK_NO, E.STOCK_SEQ }
                               group E by new
                               {
                                 //建立不良品調撥單
                                 D.PURCHASE_NO,
                                 D.PURCHASE_SEQ,
                                 D.VNR_CODE,
                                 D.ITEM_CODE,
                                 D.RECE_DATE,
                                 D.VALI_DATE,
                                 D.MADE_DATE,
                                 D.ORDER_QTY,
                                 D.RECV_QTY,
                                 D.CHECK_QTY,
                                 D.STATUS,
                                 D.CHECK_ITEM,
                                 D.CHECK_SERIAL,
                                 D.ISPRINT,
                                 D.ISUPLOAD,
                                 D.DC_CODE,
                                 D.GUP_CODE,
                                 D.CUST_CODE,
                                 D.CRT_STAFF,
                                 D.CRT_DATE,
                                 D.UPD_STAFF,
                                 D.UPD_DATE,
                                 D.CRT_NAME,
                                 D.UPD_NAME,
                                 D.ISSPECIAL,
                                 D.SPECIAL_DESC,
                                 D.SPECIAL_CODE,
                                 D.RT_NO,
                                 D.RT_SEQ,
                                 D.IN_DATE,
                                 TARWAREHOUSE_ID = E.WAREHOUSE_ID,
                                 D.QUICK_CHECK,
                                 D.MAKE_NO,
                                 DEFECT_QTY = E.DEFECT_QTY ?? 1
                               } into g
                               select new F02020101
                               {
                                 PURCHASE_NO = g.Key.PURCHASE_NO,
                                 PURCHASE_SEQ = g.Key.PURCHASE_SEQ,
                                 VNR_CODE = g.Key.VNR_CODE,
                                 ITEM_CODE = g.Key.ITEM_CODE,
                                 RECE_DATE = g.Key.RECE_DATE,
                                 VALI_DATE = g.Key.VALI_DATE,
                                 MADE_DATE = g.Key.MADE_DATE,
                                 ORDER_QTY = g.Key.ORDER_QTY,
                                 RECV_QTY = g.Sum(x => x.DEFECT_QTY),
                                 CHECK_QTY = g.Key.CHECK_QTY,
                                 STATUS = g.Key.STATUS,
                                 CHECK_ITEM = g.Key.CHECK_ITEM,
                                 CHECK_SERIAL = g.Key.CHECK_SERIAL,
                                 ISPRINT = g.Key.ISPRINT,
                                 ISUPLOAD = g.Key.ISUPLOAD,
                                 DC_CODE = g.Key.DC_CODE,
                                 GUP_CODE = g.Key.GUP_CODE,
                                 CUST_CODE = g.Key.CUST_CODE,
                                 CRT_STAFF = g.Key.CRT_STAFF,
                                 CRT_DATE = g.Key.CRT_DATE,
                                 UPD_STAFF = g.Key.UPD_STAFF,
                                 UPD_DATE = g.Key.UPD_DATE,
                                 CRT_NAME = g.Key.CRT_NAME,
                                 UPD_NAME = g.Key.UPD_NAME,
                                 ISSPECIAL = g.Key.ISSPECIAL,
                                 SPECIAL_DESC = g.Key.SPECIAL_DESC,
                                 SPECIAL_CODE = g.Key.SPECIAL_CODE,
                                 RT_NO = g.Key.RT_NO,
                                 RT_SEQ = g.Key.RT_SEQ,
                                 IN_DATE = g.Key.IN_DATE,
                                 TARWAREHOUSE_ID = g.Key.TARWAREHOUSE_ID,
                                 QUICK_CHECK = g.Key.QUICK_CHECK,
                                 MAKE_NO = g.Key.MAKE_NO
                               }).ToList();
    }
    #endregion

    #region 更新不良品暫存檔資料
    protected List<F02020109> CreateF02020109(F02020101 tmp, List<F02020109> f02020109s)
    {

      return f02020109s.Where(x => x.DC_CODE == tmp.DC_CODE
                      && x.GUP_CODE == tmp.GUP_CODE
                      && x.CUST_CODE == tmp.CUST_CODE
                      && x.STOCK_NO == tmp.PURCHASE_NO
                      && x.STOCK_SEQ == Convert.ToInt32(tmp.PURCHASE_SEQ))
                      .Select(x => new F02020109
                      {
                        ID = x.ID,
                        DC_CODE = x.DC_CODE,
                        GUP_CODE = x.GUP_CODE,
                        CUST_CODE = x.CUST_CODE,
                        STOCK_NO = x.STOCK_NO,
                        STOCK_SEQ = x.STOCK_SEQ,
                        DEFECT_QTY = x.DEFECT_QTY,
                        SERIAL_NO = x.SERIAL_NO,
                        UCC_CODE = x.UCC_CODE,
                        CAUSE = x.CAUSE,
                        CRT_DATE = x.CRT_DATE,
                        CRT_NAME = x.CRT_NAME,
                        CRT_STAFF = x.CRT_STAFF,
                        RT_NO = tmp.RT_NO,
                        RT_SEQ = tmp.RT_SEQ,
                        WAREHOUSE_ID = x.WAREHOUSE_ID
                      }).ToList();
    }
    #endregion
    #region 
    public List<F02020104> CreateF02020104sExistF020302s(F02020101 tmp, List<F020302> f020302Data, List<F2501> f2501Data, int maxLogSeq)
    {
      List<F02020104> res = new List<F02020104>();

      for (int index = 0; index < f020302Data.Count; index++)
      {
        var currData = f020302Data[index];

        var status = f2501Data.Where(y => y.SERIAL_NO == currData.SERIAL_NO).Select(z => z.STATUS).SingleOrDefault();

        res.Add(new F02020104
        {
          PURCHASE_NO = tmp.PURCHASE_NO,
          PURCHASE_SEQ = tmp.PURCHASE_SEQ,
          LOG_SEQ = index + 1 + maxLogSeq,
          ITEM_CODE = currData.ITEM_CODE,
          SERIAL_NO = currData.SERIAL_NO,
          STATUS = string.IsNullOrWhiteSpace(status) ? null : status,
          ISPASS = "1",
          DC_CODE = currData.DC_CODE,
          GUP_CODE = currData.GUP_CODE,
          CUST_CODE = currData.CUST_CODE,
          RT_NO = tmp.RT_NO,
          BATCH_NO = currData.BATCH_NO
        });
      }

      return res;
    }
    #endregion

    #region 驗收確認_新版
    /// <summary>
    /// 驗收確認_新版
    /// </summary>
    /// <param name="acp"></param>
    /// <returns></returns>
    public virtual AcceptanceReturnData AcceptanceConfirm(AcceptanceConfirmParam acp)
    {
      var logSvc = new LogService("商品檢驗_" + DateTime.Now.ToString("yyyyMMdd"));

      var logDt = DateTime.Now;
      var result1 = CheckPurchaseNo(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo);
      if (!result1.IsSuccessed)
        return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = result1 };
      logSvc.Log($"CheckPurchaseNo {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      //repo
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var sharedSerivce = new SharedService(_wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020103Repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
      var f190904Repo = new F190904Repository(Schemas.CoreSchema);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
      var result = new AcceptanceReturnData { RT_NO = acp.RTNo };
      var addF020201List = new List<F020201>();
      var updF02020101List = new List<F02020101>();
      var addF02020104List = new List<F02020104>();
      var updF020302List = new List<F020302>();
      var updF02020109List = new List<F02020109>();
      var returnStocks = new List<F1913>();
      ReturnNewAllocationResult allocationResult = null;
      logSvc.Log($"NewRepo {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      var today = DateTime.Today;
      //取得來源儲位
      var srcLoc = sharedSerivce.GetSrcLoc(acp.DcCode, acp.GupCode, acp.CustCode, "I");//I:進貨暫存倉
      if (srcLoc == null)
        return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.SrcLocNotExist } };
      var srcLocCode = srcLoc.LOC_CODE;
      //取得虛擬商品的儲位	
      var f190904Item = f190904Repo.Find(o => o.DC_CODE == acp.DcCode && o.GUP_CODE == acp.GupCode && o.CUST_CODE == acp.CustCode);
      string virtualItemLocCode = f190904Item == null ? string.Empty : f190904Item.LOC_CODE;

      //取得進倉單主檔
      var f010201 = f010201Repo.Find(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo);

      //取得此驗收單號所有進貨暫存資料
      var f02020101s = f02020101Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo && x.RT_NO == acp.RTNo).ToList();

      //取得此進倉單已驗收資料
      var f020201s = f020201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();

      //取得未匯入進倉序號資料
      var f020302s = f020301Repo.AsForUpdate().GetF020302s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo).ToList();

      //取得進倉序號刷讀紀錄
      List<F02020104> f02020104s = null;
			if (f02020104s == null)
				f02020104s = f02020104Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
			var maxSeq = !f02020104s.Any() ? 0 :f02020104s.Max(x => x.LOG_SEQ);

			//取得此驗收單號本次進貨驗收暫存資料
			var tmpList = f02020101s.Where(x => x.STATUS == "0" && x.RECV_QTY > 0 && x.CHECK_ITEM == "1").ToList();

      //取得不良品站存檔資料
      var f02020109s = f02020109Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo).Where(x => string.IsNullOrWhiteSpace(x.RT_NO)).ToList();
      logSvc.Log($"取得資料 {logSvc.DateDiff(logDt, DateTime.Now)}");

      var rtSeq = 1;
      var virtualtmpList = new List<F02020101>();
      foreach (var tmp in tmpList)
      {
        logDt = DateTime.Now;
        if (tmp.VALI_DATE < today)
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.P020203Service_AcceptanceItemValidDateMustOverToday, tmp.ITEM_CODE) } };

				var f1903 = CommonService.GetProduct(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE); 
        logSvc.Log($"GetF1903 {logSvc.DateDiff(logDt, DateTime.Now)}");

        //越庫商品
        if (f1903.C_D_FLAG == "1")
          result.IsOverWarehouseItem = true;
        //如果此商品為序號商品,但未完成序號刷讀檢核，本次跳過此筆驗收
        if (f1903.BUNDLE_SERIALNO == "1" && tmp.CHECK_SERIAL == "0")
          continue;
        //取得此筆已驗收數
        var sumRecvQty = f020201s.Where(x => x.PURCHASE_SEQ == tmp.PURCHASE_SEQ).Sum(x => x.RECV_QTY);
        //已驗收數+本次驗收數超過訂購數則跳過此筆驗收
        //if (sumRecvQty + tmp.RECV_QTY > tmp.ORDER_QTY)
        //	continue;

        //更新驗收暫存檔
        tmp.STATUS = "1"; //已驗收待上傳
        tmp.RECE_DATE = today;
        tmp.RT_SEQ = rtSeq.ToString();
        updF02020101List.Add(tmp);

        //即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F02020101批號
        var currRtSeq = GetItemMakeRtSeq(tmp);

        // 回填批號
        tmp.MAKE_NO = $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }";

        logDt = DateTime.Now;
        //產生驗收明細
        addF020201List.Add(CreateF020201(tmp));
        logSvc.Log($"產生驗收明細 {logSvc.DateDiff(logDt, DateTime.Now)}");

        logDt = DateTime.Now;
        // 更新不良品暫存檔 
        updF02020109List.AddRange(CreateF02020109(tmp, f02020109s));
        logSvc.Log($"更新不良品暫存檔 {logSvc.DateDiff(logDt, DateTime.Now)}");

        rtSeq++;

        logDt = DateTime.Now;
        #region 虛擬商品儲位檢核
        //是否為虛擬商品
        bool isVirtualItem = !string.IsNullOrEmpty(f1903.VIRTUAL_TYPE);
        if (isVirtualItem && string.IsNullOrEmpty(virtualItemLocCode))
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.VirtualItemNotExist } };

        if (isVirtualItem)
          virtualtmpList.Add(tmp);
        #endregion
        logSvc.Log($"虛擬商品儲位檢核 {logSvc.DateDiff(logDt, DateTime.Now)}");

        //商品未使用序號(取驗收數量)
        var itemF020302s = f020302s.Where(x => x.ITEM_CODE == tmp.ITEM_CODE && x.STATUS == "0").Take(tmp.RECV_QTY ?? 0).ToList();

        logDt = DateTime.Now;
        #region 序號商品更新狀態
        if (tmp.CHECK_SERIAL == "1")
        {
          if (itemF020302s.Any())
          {
            // 增加寫入F02020104 此序號，如果不存在才寫入 SERAIL_NO = 序號,ISPASS = 1,RT_NO = 驗收單號
            var serialNoData = f02020104s.Where(x => x.PURCHASE_SEQ == tmp.PURCHASE_SEQ && x.RT_NO == tmp.RT_NO).Select(x => x.SERIAL_NO).ToList();
            var f020302Data = itemF020302s.Where(x => !serialNoData.Contains(x.SERIAL_NO)).ToList();
            var f2501Data = CommonService.GetItemSerialList(tmp.GUP_CODE,tmp.CUST_CODE, serialNoData).ToList();
            addF02020104List.AddRange(CreateF02020104sExistF020302s(tmp, f020302Data, f2501Data, maxSeq));

            //更新序號狀態
            var checkResult = UpdateF2501(tmp, f010201.ORD_PROP, itemF020302s, f02020109s);
            if (!checkResult.IsSuccessed)
              return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = checkResult.Message } };
          }
        }
        #endregion
        logSvc.Log($"序號商品更新狀態 {logSvc.DateDiff(logDt, DateTime.Now)}");

        logDt = DateTime.Now;
        #region 產生進貨暫存倉庫存資料
        InsertOrUpdateStock(tmp, isVirtualItem ? virtualItemLocCode : srcLocCode, itemF020302s, ref returnStocks);
        #endregion
        logSvc.Log($"產生進貨暫存倉庫存資料 {logSvc.DateDiff(logDt, DateTime.Now)}");

        if (itemF020302s.Any())
          updF020302List.AddRange(itemF020302s);
      }

      var notVirtualItemList = tmpList.Except(virtualtmpList).ToList();
      var hasRecvList = notVirtualItemList.Where(x => (x.RECV_QTY ?? 0) > 0).ToList();

      logDt = DateTime.Now;
      #region 不良品拆單
      CreateDefectItem(ref hasRecvList, f02020109s);
      #endregion
      logSvc.Log($"不良品拆單 {logSvc.DateDiff(logDt, DateTime.Now)}");

      if (hasRecvList.Any())
      {
        logDt = DateTime.Now;
        #region 產生調撥單
        // 因不良品序號若沒有在後面，後面產生調撥單Group將會錯亂把不良序號 歸類在良品倉扣帳
        // 找出不良品序號
        var defectSerialNos = f02020109s.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)).Select(x => x.SERIAL_NO);
        if (defectSerialNos.Any())
        {
          // 將序號綁儲位不良品放到最後
          var defectReturnStocks = returnStocks.Where(x => defectSerialNos.Contains(x.SERIAL_NO));

          returnStocks = returnStocks.Except(defectReturnStocks).ToList();

          returnStocks.AddRange(defectReturnStocks);
        }

        allocationResult = CreateAcceptanceAllocation(acp, srcLoc, returnStocks.Where(x => x.LOC_CODE != virtualItemLocCode).ToList(), hasRecvList);
        if (!allocationResult.Result.IsSuccessed)
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = allocationResult.Result.Message } };
        #endregion
        logSvc.Log($"產生調撥單 {logSvc.DateDiff(logDt, DateTime.Now)}");

        logDt = DateTime.Now;
        #region 計算版標數
        CreateAcceptanceStickers(acp, allocationResult.AllocationList, hasRecvList);
        #endregion
        logSvc.Log($"計算版標數 {logSvc.DateDiff(logDt, DateTime.Now)}");

        logDt = DateTime.Now;
        #region 設定驗收單與調撥單關係
        var f02020107s = f02020107Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo && x.RT_NO == acp.RTNo).ToList();
        foreach (var item in allocationResult.AllocationList)
        {
          if (!f02020107s.Any(x => x.ALLOCATION_NO == item.Master.ALLOCATION_NO))
          {
            var f02020107 = new F02020107
            {
              DC_CODE = acp.DcCode,
              GUP_CODE = acp.GupCode,
              CUST_CODE = acp.CustCode,
              ALLOCATION_NO = item.Master.ALLOCATION_NO,
              PURCHASE_NO = acp.PurchaseNo,
              RT_NO = acp.RTNo
            };
            f02020107Repo.Add(f02020107);
          }
        }

        #endregion
        logSvc.Log($"設定驗收單與調撥單關係 {logSvc.DateDiff(logDt, DateTime.Now)}");
      }

      logDt = DateTime.Now;
      #region 產生虛擬商品庫存
      if (virtualtmpList.Any())
      {
        result.HasVirtualItem = true;
        var virtalItemReturnStocks = returnStocks.Where(x => x.LOC_CODE == virtualItemLocCode).ToList();
        var createStocks = virtalItemReturnStocks.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        var updateStocks = virtalItemReturnStocks.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        if (createStocks.Any())
          f1913Repo.BulkInsert(createStocks);
        if (updateStocks.Any())
          f1913Repo.BulkUpdate(updateStocks);

        List<F02020108> f02020108s = new List<F02020108>();
        List<F020201> f020201List = addF020201List.Where(x => virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

        // 新增進倉單明細、驗收單明細與調撥單明細的關聯表[F02020108]
        warehouseInService.CreateF02020108s(acp.DcCode, acp.GupCode, acp.CustCode, f020201List, null, null, ref f02020108s, f02020109s, true);

        // 新增進倉驗收上架結果表[F010204]
        warehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List, true);

        // 新增進倉回檔歷程紀錄表[F010205]
        var f02020108sByCrtF010205 = f02020108s.Select(x => new F02020108
        {
          ALLOCATION_SEQ = x.ALLOCATION_SEQ,
          CUST_CODE = x.CUST_CODE,
          DC_CODE = x.DC_CODE,
          GUP_CODE = x.GUP_CODE,
          REC_QTY = x.REC_QTY,
          RT_NO = x.RT_NO,
          RT_SEQ = x.RT_SEQ,
          STOCK_NO = x.STOCK_NO,
          STOCK_SEQ = x.STOCK_SEQ,
          TAR_QTY = x.TAR_QTY
        }).ToList();
        warehouseInService.CreateF010205s(f02020108sByCrtF010205, status: "2");

        // 新增進倉回檔歷程紀錄表[F010205]
        warehouseInService.CreateF010205s(f02020108s, status: "3");

        // 新增進倉上架歷程表[F020202]
        warehouseInService.CreateF020202s(f020201List, returnStocks);
      }
      #endregion
      logSvc.Log($"產生虛擬商品庫存 {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      #region 整批寫入調撥單
      if (allocationResult != null)
      {
        var allocationExecResult = sharedSerivce.BulkInsertAllocation(allocationResult.AllocationList, allocationResult.StockList);
        if (!allocationExecResult.IsSuccessed)
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = allocationExecResult.Message } };

        List<F02020108> f02020108s = new List<F02020108>();
        List<F020201> f020201List = addF020201List.Where(x => !virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();
        // 新增進倉單明細、驗收單明細與調撥單明細的關聯表[F02020108]
        allocationResult.AllocationList.ForEach(newAllocation =>
        {
          warehouseInService.CreateF02020108s(acp.DcCode, acp.GupCode, acp.CustCode, f020201List, newAllocation.Master, newAllocation.Details, ref f02020108s, f02020109s);
        });

        // 新增進倉驗收上架結果表[F010204]
        warehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List);

        // 新增進倉回檔歷程紀錄表[F010205]
        var f02020108sByCrtF010205 = f02020108s.Select(x => new F02020108
        {
          ALLOCATION_SEQ = x.ALLOCATION_SEQ,
          CUST_CODE = x.CUST_CODE,
          DC_CODE = x.DC_CODE,
          GUP_CODE = x.GUP_CODE,
          REC_QTY = x.REC_QTY,
          RT_NO = x.RT_NO,
          RT_SEQ = x.RT_SEQ,
          STOCK_NO = x.STOCK_NO,
          STOCK_SEQ = x.STOCK_SEQ,
          TAR_QTY = x.TAR_QTY
        }).ToList();
        warehouseInService.CreateF010205s(f02020108sByCrtF010205, status: "2");
      }
      else
      {
        #region 產生一單一品進貨暫存倉庫存(進貨全都是一單一品要從進貨暫存倉出貨)
        var notVirtalItemReturnStocks = returnStocks.Where(x => x.LOC_CODE != virtualItemLocCode).ToList();
        var createStocks = notVirtalItemReturnStocks.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        var updateStocks = notVirtalItemReturnStocks.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        if (createStocks.Any())
          f1913Repo.BulkInsert(createStocks);
        if (updateStocks.Any())
          f1913Repo.BulkUpdate(updateStocks);
        #endregion
      }
      #endregion
      logSvc.Log($"整批寫入調撥單 {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      if (addF020201List.Any())
        f020201Repo.BulkInsert(addF020201List);
      if (addF02020104List.Any())
        f02020104Repo.BulkInsert(addF02020104List);
      if (updF020302List.Any())
        f020302Repo.BulkUpdate(updF020302List);
      if (updF02020109List.Any())
        f02020109Repo.BulkUpdate(updF02020109List);
      logSvc.Log($"Bulk {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;

      #region 新增/更新 商品廠商對應表(F190303)
      var f190303Service = new P190303Service(_wmsTransaction);
      f190303Service.AddorUpdateF190303Data(tmpList.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE, x.VNR_CODE, x.PURCHASE_NO }).Select(x => new F190303
      {
        GUP_CODE = x.Key.GUP_CODE,
        CUST_CODE = x.Key.CUST_CODE,
        ITEM_CODE = x.Key.ITEM_CODE,
        VNR_CODE = x.Key.VNR_CODE,
        SOURCE_NO = x.Key.PURCHASE_NO
      }).ToList());
      #endregion
      logSvc.Log($"新增/更新 商品廠商對應表(F190303) {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      #region 更新進場管理離場時間
      var updF020103s = f020103Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
      var outTime = DateTime.Now.ToString("HHmm");
      updF020103s.ForEach((x) => { if (string.IsNullOrEmpty(x.OUTTIME)) { x.OUTTIME = outTime; } });
      if (updF020103s.Any())
        f020103Repo.BulkUpdate(updF020103s);
      #endregion
      logSvc.Log($"更新進場管理離場時間 {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      #region 更新入庫狀態
      f010201.CHECKCODE_EDI_STATUS = "1";
      f010201Repo.Update(f010201);
      #endregion
      logSvc.Log($"更新入庫狀態 {logSvc.DateDiff(logDt, DateTime.Now)}");

      logDt = DateTime.Now;
      #region 驗收序號回傳
      result.AcceptanceSerialDatas = updF020302List.Select(x => new AcceptanceSerialData
      {
        ITEM_CODE = x.ITEM_CODE,
        ITEM_NAME = GetF1903(x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE).ITEM_NAME,
        SERIAL_NO = x.SERIAL_NO
      }).ToList();
      #endregion
      logSvc.Log($"驗收序號回傳 {logSvc.DateDiff(logDt, DateTime.Now)}");

      result.ExecuteResult = new ExecuteResult(true);

      return result;
    }

    /// <summary>
    /// 即時新增/更新 驗收批號的流水號紀錄檔(F020203)
    /// </summary>
    /// <param name="acp"></param>
    /// <param name="f020203Repo"></param>
    /// <param name="today"></param>
    /// <param name="tmp"></param>
    /// <returns></returns>
    public int GetItemMakeRtSeq(F02020101 tmp)
    {
      var f020203Repo = new F020203Repository(Schemas.CoreSchema);
      var f020203 = f020203Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
              () =>
              {
                var lockF020203 = f020203Repo.LockF020203();
                var currF020203 = f020203Repo.GetDataByKey(tmp.DC_CODE, tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE, DateTime.Today);
                if (currF020203 == null)
                {
                  currF020203 = new F020203
                  {
                    DC_CODE = tmp.DC_CODE,
                    GUP_CODE = tmp.GUP_CODE,
                    CUST_CODE = tmp.CUST_CODE,
                    ITEM_CODE = tmp.ITEM_CODE,
                    RT_DATE = DateTime.Today,
                    RT_SEQ = 1
                  };
                  // 若沒有在資料庫則新增 目前已使用流水號
                  f020203Repo.Add(currF020203);
                }
                else
                {
                  currF020203.RT_SEQ++;
                  f020203Repo.Update(currF020203);
                }
                return currF020203;
              });
      return f020203.RT_SEQ;
    }

    #endregion

    #region 取得商品檢驗明細清單
    /// <summary>
    /// 取得商品檢驗明細清單
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="purchaseNo">進倉單號</param>
    /// <param name="rtNo">驗收單號</param>
    /// <param name="isPickLocFirst">是否取得揀貨儲位</param>
    /// <returns></returns>
    public List<P020203Data> GetP020203Datas(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, bool isPickLocFirst)
    {
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var datas = f02020101Repo.GetP020203Datas(dcCode, gupCode, custCode, purchaseNo, rtNo).ToList();

      #region 計算包裝參考
      var f190301Repo = new F190301Repository(Schemas.CoreSchema);
      var itemService = new ItemService();

      foreach (var item in datas)
      {
        var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, custCode, new List<ItemCodeQtyModel> { new ItemCodeQtyModel { ItemCode = item.ITEM_CODE, Qty = item.RECV_QTY ?? 0 } });
        var currRef = itemPackageRefs.Where(x => x.ItemCode == item.ITEM_CODE).FirstOrDefault();
        item.UNIT_TRANS = currRef == null ? null : currRef.PackageRef;
      }
      #endregion

      #region 若F010205不存在且STATUS = 5，新增一筆F010205
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);
      F010201 f010201 = f010201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
      //
      var existF010205 = f010205Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo && x.STATUS == "5");
      if (existF010205 == null || !existF010205.Any())
      {
        f010205Repo.Add(new F010205
        {
          DC_CODE = f010201.DC_CODE,
          GUP_CODE = f010201.GUP_CODE,
          CUST_CODE = f010201.CUST_CODE,
          STOCK_NO = f010201.STOCK_NO,
          STATUS = "5",
          PROC_FLAG = "1",
          TRANS_DATE = DateTime.Now
        });
      }

      #endregion

      return datas;
    }

    #endregion

    #region 取消驗收
    public ExecuteResult CancelAcceptance(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
    {
      var result = CheckPurchaseNo(dcCode, gupCode, custCode, purchaseNo);
      if (!result.IsSuccessed)
        return result;
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020102Repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);
      var f020201s = f020201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo && x.RT_NO == rtNo).ToList();
      if (f020201s.Any())
        return new ExecuteResult(false, Properties.Resources.PurchaseHasRecvComplete);

      f02020101Repo.Delete(dcCode, gupCode, custCode, purchaseNo, rtNo);
      f02020102Repo.Delete(dcCode, gupCode, custCode, purchaseNo, rtNo);
      //先刪除F020302 再刪F020301 最後才刪F02020104 有順序性 
      f020302Repo.DeleteWithCancelAcceptance(dcCode, gupCode, custCode, purchaseNo, rtNo);
      f020301Repo.Delete(dcCode, gupCode, custCode, purchaseNo);
      f02020104Repo.Delete(dcCode, gupCode, custCode, purchaseNo, rtNo);

      return new ExecuteResult(true);
    }
    #endregion

    public ExecuteResult CheckRTModeValue(String RT_MODE)
    {
      if (!new String[] { "0", "1" }.Contains(RT_MODE))
        return new ExecuteResult() { IsSuccessed = false, Message = "無法辨識RT_MODE參數內容" };
      else
        return new ExecuteResult(true);
    }

		public ExecuteResult CallRecvVedio(string dcCode,string gupCode,string custCode,string stockNo,List<string> itemCodeList = null)
		{
			var result = new ExecuteResult(true);
			var setting = CommonService.GetSysGlobalValue(dcCode, "VideoCombinIn");
			if(setting == "1") // 開啟影資
			{
				var workStationService = new WorkstationService();
				var f910501Repo = new F910501Repository(Schemas.CoreSchema);
				var device = f910501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.DEVICE_IP == Current.DeviceIp).FirstOrDefault();
				result = workStationService.CheckWorkStationCode(device, "驗貨區");
				if (!result.IsSuccessed)
					return result;
				var recvItemService = new RecvItemService();
				RecvItemNotifyReq req;
				if(itemCodeList == null)
				{
					 req = new RecvItemNotifyReq()
					{
						WhId = dcCode,   //倉庫代碼
						OrderNo = stockNo, //收貨單號
						WorkStationId = device.WORKSTATION_CODE,  //工作站ID
						//SkuId ="",  //商品ID(不用填寫)
						TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") //時間戳記
					};
					var apiresult = recvItemService.RecvItemNotify(dcCode, gupCode, custCode, req);
					result.Message = apiresult.MsgContent;
				}
				else
				{
					foreach(var itemCode in itemCodeList)
					{
						req = new RecvItemNotifyReq()
						{
							WhId = dcCode,   //倉庫代碼
							OrderNo = stockNo, //收貨單號
							WorkStationId = device.WORKSTATION_CODE,  //工作站ID
							SkuId =itemCode,  //商品ID(不用填寫)
							TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") //時間戳記
						};
						var apiresult = recvItemService.RecvItemNotify(dcCode, gupCode, custCode, req);
						result.Message += apiresult.MsgContent;
					}
				}
			}
			return result;
		}


	}
}
