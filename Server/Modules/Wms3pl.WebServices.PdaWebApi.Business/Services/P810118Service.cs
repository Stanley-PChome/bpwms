using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
  public class P810118Service
  {
    P81Service _p81Service;
    private CommonService _commonService;
    public CommonService CommonService
    {
      get
      {
        if (_commonService == null)
          _commonService = new CommonService();
        return _commonService;
      }
      set { _commonService = value; }
    }

    public SharedService _sharedService { get; set; }

    private List<ReturnNewAllocation> _returnNewAllocations;
    private WmsTransaction _wmsTransaction;
    public P810118Service(WmsTransaction wmsTransation)
    {
      _wmsTransaction = wmsTransation;
      _p81Service = new P81Service();
    }

    /// <summary>
    /// 跨庫調撥驗收入自動倉-驗收完成
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult MoveInToAutoRecvFinish(MoveInToAutoRecvFinishReq req, string gupCode)
    {
      #region 宣告
      var containerService = new ContainerService(_wmsTransaction);

      var f1980Repo = new F1980Repository(Schemas.CoreSchema);
      var f076101Repo = new F076101Repository(Schemas.CoreSchema);
      var p810110Service = new P810110Service(_wmsTransaction);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070104Repo = new F070104Repository(Schemas.CoreSchema, _wmsTransaction);

      F0701 f0701;
      F070101 f070101;
      List<F070102> f070102s;
      #endregion 宣告

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
          string.IsNullOrWhiteSpace(req.AccNo) ||
          string.IsNullOrWhiteSpace(req.DcNo) ||
          string.IsNullOrWhiteSpace(req.CustNo) ||
          //string.IsNullOrWhiteSpace(req.WarehouseId) ||
          //string.IsNullOrWhiteSpace(req.ContainerCode) ||
          //string.IsNullOrWhiteSpace(req.ItemCode) ||
          accData.Count() == 0 ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      if (string.IsNullOrWhiteSpace(req.ContainerCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };


      //檢核商品條碼是否正確
      bool isSn = false;
      var itemCodes = _p81Service.GetItemCodeByBarcode(ref isSn, req.DcNo, gupCode, req.CustNo, req.ItemCode);
      if (itemCodes == null || (itemCodes != null && !itemCodes.Any()))
        return new ApiResult { IsSuccessed = false, MsgCode = "21007", MsgContent = _p81Service.GetMsg("21007") };

      //把User畫面輸入的內容轉大寫
      req.ContainerCode = req.ContainerCode.ToUpper();
      req.ItemCode = req.ItemCode.ToUpper();

      var f070104s = new List<F070104>();
      var getF070104Res = p810110Service.GetF070104Data(ref f070104s, req.ContainerCode);
      if (!getF070104Res.IsSuccessed)
        return getF070104Res;

      #endregion 資料檢核

      #region 資料處理
      //檢查傳入的倉別編號是否有錯誤
      if (f1980Repo.GetDatas(req.DcNo, new[] { req.WarehouseId }.ToList()).FirstOrDefault() == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21024", MsgContent = _p81Service.GetMsg("21024") };

      var f076101 = f076101Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF0701 = f076101Repo.LockF076101();
          var chkF076101 = f076101Repo.Find(x => x.CONTAINER_CODE == req.ContainerCode);
          if (chkF076101 != null)
            return null;
          var newF076101 = new F076101()
          {
            CONTAINER_CODE = req.ContainerCode
          };
          f076101Repo.Add(newF076101);
          return newF076101;
        });
      if (f076101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21023", MsgContent = string.Format(_p81Service.GetMsg("21023"), req.ContainerCode) };

      //商品檢驗
      var confirmStockInRes = p810110Service.ConfirmTransferStockReceivedDataProcess(new ConfirmTransferStockReceivedDataReq
      {
        FuncNo = req.FuncNo,
        AccNo = req.AccNo,
        DcNo = req.DcNo,
        CustNo = req.CustNo,
        ContainerCode = req.ContainerCode,
        ItemCode = req.ItemCode
      }, gupCode, f070104s);
      if (!confirmStockInRes.IsSuccessed)
        return confirmStockInRes;

      #region 建立容器檔F0701,F070101,F070102
      var f0701Id = containerService.GetF0701NextId();
      f0701 = new F0701
      {
        ID = f0701Id,
        DC_CODE = req.DcNo,
        CUST_CODE = req.CustNo,
        WAREHOUSE_ID = req.WarehouseId,
        CONTAINER_CODE = req.ContainerCode,
        CONTAINER_TYPE = "0"
      };

      var f070101Id = containerService.GetF070101NextId();
      f070101 = new F070101
      {
        ID = f070101Id,
        F0701_ID = f0701Id,
        DC_CODE = req.DcNo,
        GUP_CODE = gupCode,
        CUST_CODE = req.CustNo,
        CONTAINER_CODE = req.ContainerCode,
      };

      f070102s = new List<F070102>();
		
			foreach (var f070104 in f070104s)
      {
        if (f070104.QTY > 0)
        {
          f070102s.Add(new F070102
          {
            F070101_ID = f070101Id,
            GUP_CODE = f070104.GUP_CODE,
            CUST_CODE = f070104.CUST_CODE,
            ITEM_CODE = f070104.ITEM_CODE,
            VALID_DATE = f070104.VALID_DATE,
            MAKE_NO = f070104.MAKE_NO,
            QTY = f070104.QTY,
            SERIAL_NO_LIST = f070104.SERIAL_NO_LIST,
            ORG_F070101_ID = f070101Id
          });
        }
      }
      #endregion 建立容器檔F0701,F070101,F070102

      #region 容器上架
      var containerMoveInRes = ContainerTargetMoveInProcess(req, gupCode, f070104s, f070101);
      if (!containerMoveInRes.IsSuccessed)
        return containerMoveInRes;
      #endregion 容器上架

      #region 容器資料寫入DB
      //釋放原本跨庫入的容器
      f0701Repo.DeleteF0701ByIds(f070104s.Select(x => x.F0701_ID).Distinct().ToList());

      //在ContainerTargetMoveInProcess還會異動f070101內容，因此不是設定完容器資料後就立刻新增
      if (f0701 != null)
        f0701Repo.Add(f0701);
      if (f070101 != null)
        f070101Repo.Add(f070101);
      if (f070102s != null && f070102s.Any())
        f070102Repo.BulkInsert(f070102s);
      if (f070104s != null && f070104s.Any())
        f070104Repo.BulkUpdate(f070104s);
      #endregion 容器資料寫入DB

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001") };

      #endregion 資料處理

    }

    /// <summary>
    /// 跨庫入容器上架共用服務
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <param name="f070104s"></param>
    /// <param name="f070101">待新增的容器頭檔，寫入WMS_NO&WMS_TYPE</param>
    /// <returns></returns>
    public ApiResult ContainerTargetMoveInProcess(MoveInToAutoRecvFinishReq req, string gupCode, List<F070104> f070104s, F070101 f070101)
    {
      var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);

      var returnStocks = new List<F1913>();
      var updF070104s = new List<F070104>();

      if (_sharedService == null)
        _sharedService = new SharedService(_wmsTransaction);

      // 該容器品號的進倉單號清單
      var stockNos = f070104s.Select(x => x.SOURCE_NO).Distinct().ToList();

      // 取得進倉單明細
      var f010202s = f010202Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取得進倉回檔歷程紀錄表
      var f010205s = f010205Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();



      // 取得來源儲位
      var srcLoc = _sharedService.GetSrcLoc(req.DcNo, gupCode, req.CustNo, "I");//I:進貨暫存倉
      if (srcLoc == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21802", MsgContent = _p81Service.GetMsg("21802") };

      // 產生進貨暫存倉庫存
      InsertOrUpdateStock(f070104s, srcLoc.LOC_CODE, ref returnStocks);

      #region 產生調撥上架單
      var group = from A in f070104s
                  group A by new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.SOURCE_NO, A.ITEM_SEQ } into g
                  select g;

      //呼叫調撥共用函數[不需要檢查配庫狀態IsCheckExecStatus設為false]
      var allocationParam = new NewAllocationItemParam
      {
        GupCode = gupCode,
        CustCode = req.CustNo,
        AllocationDate = DateTime.Today,
        IsExpendDate = true,
        SrcDcCode = req.DcNo,
        TarDcCode = req.DcNo,
        AllocationType = AllocationType.Both,
        ReturnStocks = returnStocks,
        AllocationTypeCode = "4",
        ContainerCode = req.ContainerCode,
        F0701_ID = f070101.F0701_ID,
        IsCheckExecStatus = false,
        isIncludeResupply = false,
        TarWarehouseId = req.WarehouseId,
        SrcWarehouseId = srcLoc.WAREHOUSE_ID,
        SourceType = "30",
        SourceNo = req.ContainerCode,
        SrcStockFilterDetails = group.Select((x, rowIndex) => new StockFilter
        {
          DataId = rowIndex,
          SrcWarehouseId = srcLoc.WAREHOUSE_ID,
          ItemCode = x.Key.ITEM_CODE,
          LocCode = srcLoc.LOC_CODE,
          Qty = x.Sum(y => y.QTY),
          ValidDates = x.Key.VALID_DATE.HasValue ? new List<DateTime> { x.Key.VALID_DATE.Value } : new List<DateTime>(),
          EnterDates = new List<DateTime> { DateTime.Today },
          BoxCtrlNos = new List<string> { "0" },
          PalletCtrlNos = new List<string> { "0" },
          MakeNos = string.IsNullOrWhiteSpace(x.Key.MAKE_NO) ? new List<string> { "0" } : new List<string> { x.Key.MAKE_NO?.Trim() },
        }).ToList(),
        SrcLocMapTarLocs = group.Select((x, rowIndex) => new SrcLocMapTarLoc
        {
          DataId = rowIndex,
          SourceType = "04",
          VnrCode = "000000",
          BoxCtrlNo = "0",
          PalletCtrlNo = "0",
          ItemCode = x.Key.ITEM_CODE,
          EnterDate = DateTime.Today,
          SrcLocCode = srcLoc.LOC_CODE,
          TarWarehouseId = req.WarehouseId,
          SourceNo = x.Key.SOURCE_NO,
          ValidDate = x.Key.VALID_DATE,
          MakeNo = x.Key.MAKE_NO,
          ReferenceNo = x.Key.SOURCE_NO,
          ReferenceSeq = x.Key.ITEM_SEQ
        }).ToList()
      };

      var createAllocResult = _sharedService.CreateOrUpdateAllocation(allocationParam);
      if (!createAllocResult.Result.IsSuccessed)
        return new ApiResult { IsSuccessed = false, MsgCode = "20756", MsgContent = string.Format(_p81Service.GetMsg("20756"), createAllocResult.Result.Message) };



      // 調撥單整批下架
      _sharedService.BulkAllocationToAllDown(createAllocResult.AllocationList);

      if (_returnNewAllocations == null)
        _returnNewAllocations = new List<ReturnNewAllocation>();

      _returnNewAllocations.AddRange(createAllocResult.AllocationList);
      returnStocks = createAllocResult.StockList;

      #endregion 產生調撥上架單


      #region 新增F02020107
      var allDetail = createAllocResult.AllocationList.SelectMany(x => x.Details).ToList();
      var addF02020107List = allDetail.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO, x.REFENCE_NO })
          .Select(x => new F02020107
          {
            DC_CODE = x.Key.DC_CODE,
            GUP_CODE = x.Key.GUP_CODE,
            CUST_CODE = x.Key.CUST_CODE,
            ALLOCATION_NO = x.Key.ALLOCATION_NO,
            PURCHASE_NO = x.Key.REFENCE_NO,
            RT_NO = x.Key.REFENCE_NO
          }).Distinct().ToList();

      if (addF02020107List.Any())
        f02020107Repo.BulkInsert(addF02020107List);
      #endregion 新增F02020107

      #region 新增F02020108

      var f02020108s = (from A in allDetail
                        join B in f070104s
                        on new { WMS_NO = A.REFENCE_NO, ITEM_SEQ = A.REFENCE_SEQ, A.CONTAINER_CODE } equals new { B.WMS_NO, B.ITEM_SEQ, B.CONTAINER_CODE }
                        group A by new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ALLOCATION_NO, A.ALLOCATION_SEQ, B.WMS_NO, B.ITEM_SEQ } into g
                        select new F02020108
                        {
                          DC_CODE = g.Key.DC_CODE,
                          GUP_CODE = g.Key.GUP_CODE,
                          CUST_CODE = g.Key.CUST_CODE,
                          STOCK_NO = g.Key.WMS_NO,
                          STOCK_SEQ = Convert.ToInt32(g.Key.ITEM_SEQ),
                          RT_NO = g.Key.WMS_NO,
                          RT_SEQ = g.Key.ITEM_SEQ,
                          ALLOCATION_NO = g.Key.ALLOCATION_NO,
                          ALLOCATION_SEQ = g.Key.ALLOCATION_SEQ,
                          REC_QTY = Convert.ToInt32(g.Sum(x => x.SRC_QTY)),
                          TAR_QTY = 0
                        }).ToList();

      if (f02020108s.Any())
        f02020108Repo.BulkInsert(f02020108s, "ID");
      #endregion

      #region 新增/更新F070101
      if (f070101 != null)
      {
        f070101.WMS_NO = createAllocResult.AllocationList.First().Master.ALLOCATION_NO;
        f070101.WMS_TYPE = "T";
      }
      #endregion

      #region 更新F070104
      f070104s.ForEach(x => x.STATUS = "2");
      updF070104s.AddRange(f070104s);
      #endregion


      if (_returnNewAllocations != null && _returnNewAllocations.Any())
      {
        var allocationResult = _sharedService.BulkInsertAllocation(_returnNewAllocations, returnStocks);
        if (!allocationResult.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "21022", MsgContent = string.Format(_p81Service.GetMsg("21022"), allocationResult.Message) };
      }

      return new ApiResult { IsSuccessed = true };
    }

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
    /// <param name="f020201s"></param>
    /// <param name="returnStocks"></param>
    private void InsertOrUpdateStock(List<F070104> f070104s, string locCode, ref List<F1913> returnStocks)
    {
      F1913Repository f1913Repo = new F1913Repository(Schemas.CoreSchema);
      F151002Repository f151002Repo = new F151002Repository(Schemas.CoreSchema);
      F020302Repository f020302Repo = new F020302Repository(Schemas.CoreSchema);

      var enterDate = DateTime.Today;
      var vnrCode = "000000";
      var palletCtrlNo = "0";
      var boxCtrlNo = "0";

      // 取得商品主檔，用以找出是否為序號綁儲位
      var f1903s = CommonService.GetProductList(f070104s.First().GUP_CODE, f070104s.First().CUST_CODE, f070104s.Select(x => x.ITEM_CODE).Distinct().ToList());
      var detail = from A in f070104s
                   join B in f1903s
                   on A.ITEM_CODE equals B.ITEM_CODE
                   select new
                   {
                     f070104 = A,
                     B.BUNDLE_SERIALLOC
                   };

      // 如果為序號綁儲位商品，找出排除後剩下可以寫入庫存的序號清單
      var itemsBySnLoc = detail.Where(x => x.BUNDLE_SERIALLOC == "1").ToList();
      if (itemsBySnLoc.Any())
      {
        foreach (var item in itemsBySnLoc)
        {
          var currItemSn = item.f070104.SERIAL_NO_LIST.Split(',').Select(x => new ItemSnModel { ITEM_CODE = item.f070104.ITEM_CODE, SERIAL_NO = x }).ToList();

          if (currItemSn.Any())
          {
            for (int i = 0; i < item.f070104.QTY; i++)
            {
              var firstItem = currItemSn.First();

              returnStocks.Add(new F1913
              {
                DC_CODE = item.f070104.DC_CODE,
                GUP_CODE = item.f070104.GUP_CODE,
                CUST_CODE = item.f070104.CUST_CODE,
                ITEM_CODE = item.f070104.ITEM_CODE,
                LOC_CODE = locCode,
                VALID_DATE = Convert.ToDateTime(item.f070104.VALID_DATE),
                ENTER_DATE = enterDate,
                VNR_CODE = vnrCode,
                SERIAL_NO = firstItem.SERIAL_NO,
                QTY = 1,
                BOX_CTRL_NO = boxCtrlNo,
                PALLET_CTRL_NO = palletCtrlNo,
                MAKE_NO = item.f070104.MAKE_NO
              });

              currItemSn.Remove(firstItem);
            }
          }
        }
      }

      // 如果不是序號綁儲位商品
      var itemsByOther = detail.Where(x => x.BUNDLE_SERIALLOC == "0").ToList();
      if (itemsByOther.Any())
      {
        foreach (var item in itemsByOther)
        {
          var validDate = Convert.ToDateTime(item.f070104.VALID_DATE);

          var returnStock = returnStocks.FirstOrDefault(o => F1913Func(o, item.f070104.DC_CODE, item.f070104.GUP_CODE, item.f070104.CUST_CODE, item.f070104.ITEM_CODE, locCode, validDate, enterDate, vnrCode, "0", boxCtrlNo, palletCtrlNo, item.f070104.MAKE_NO));
          var f1913 = returnStock ??
                      f1913Repo.Find(o => o.DC_CODE == item.f070104.DC_CODE &&
                      o.GUP_CODE == item.f070104.GUP_CODE &&
                      o.CUST_CODE == item.f070104.CUST_CODE &&
                      o.ITEM_CODE == item.f070104.ITEM_CODE &&
                      o.LOC_CODE == locCode &&
                      o.VALID_DATE == validDate &&
                      o.ENTER_DATE == enterDate &&
                      o.VNR_CODE == vnrCode &&
                      o.SERIAL_NO == "0" &&
                      o.BOX_CTRL_NO == boxCtrlNo &&
                      o.PALLET_CTRL_NO == palletCtrlNo &&
                      o.MAKE_NO == item.f070104.MAKE_NO);
          if (f1913 != null)
          {
            f1913.QTY += item.f070104.QTY;
            if (returnStock == null)
              returnStocks.Add(f1913);
          }
          else
          {
            returnStocks.Add(new F1913
            {
              DC_CODE = item.f070104.DC_CODE,
              GUP_CODE = item.f070104.GUP_CODE,
              CUST_CODE = item.f070104.CUST_CODE,
              ITEM_CODE = item.f070104.ITEM_CODE,
              LOC_CODE = locCode,
              VALID_DATE = validDate,
              ENTER_DATE = enterDate,
              VNR_CODE = vnrCode,
              SERIAL_NO = "0",
              QTY = item.f070104.QTY,
              BOX_CTRL_NO = boxCtrlNo,
              PALLET_CTRL_NO = palletCtrlNo,
              MAKE_NO = item.f070104.MAKE_NO
            });
          }
        }
      }
    }
    #endregion

  }
}
