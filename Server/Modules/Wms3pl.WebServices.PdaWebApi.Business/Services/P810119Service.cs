using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  public class P810119Service
  {
    P81Service _p81Service;
    private WmsTransaction _wmsTransaction;

    #region Service

    #region CommonService
    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }
    #endregion

    #region StowShelfAreaService
    private StowShelfAreaService _stowShelfAreaService;
    public StowShelfAreaService StowShelfAreaService
    {
      get { return _stowShelfAreaService == null ? _stowShelfAreaService = new StowShelfAreaService() : _stowShelfAreaService; }
      set { _stowShelfAreaService = value; }
    }
    #endregion

    #region DoubleCheckService
    private DoubleCheckService _doubleCheckService;
    public DoubleCheckService DoubleCheckService
    {
      get { return _doubleCheckService == null ? _doubleCheckService = new DoubleCheckService() : _doubleCheckService; }
      set { _doubleCheckService = value; }
    }
    #endregion

    #region SharedService
    private SharedService _sharedService;
    public SharedService SharedService
    {
      get { return _sharedService == null ? _sharedService = new SharedService() : _sharedService; }
      set { _sharedService = value; }
    }
    #endregion

    #region RecvItemService
    private RecvItemService _recvItemService;
    public RecvItemService RecvItemService
    {
      get { return _recvItemService == null ? _recvItemService = new RecvItemService() : _recvItemService; }
      set { _recvItemService = value; }
    }
    #endregion

    #region WarehouseInRecvService
    private WarehouseInRecvService _warehouseInRecvService;
    public WarehouseInRecvService WarehouseInRecvService
    {
      get { return _warehouseInRecvService == null ? _warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction) : _warehouseInRecvService; }
      set { _warehouseInRecvService = value; }
    }
    #endregion

    #region UpdateItemInfoService
    private UpdateItemInfoService _updatdeItemInfoService;
    public UpdateItemInfoService UpdatdeItemInfoService
    {
      get { return _updatdeItemInfoService == null ? _updatdeItemInfoService = new UpdateItemInfoService() : _updatdeItemInfoService; }
      set { _updatdeItemInfoService = value; }
    }
    #endregion

    #region SerialNoService
    private SerialNoService _serialNoService;
    public SerialNoService SerialNoService
    {
      get { return _serialNoService == null ? _serialNoService = new SerialNoService(_wmsTransaction) : _serialNoService; }
      set { _serialNoService = value; }
    }
    #endregion

    #region WarehouseInService
    private WarehouseInService _warehouseInService;
    public WarehouseInService WarehouseInService
    {
      get { return _warehouseInService == null ? _warehouseInService = new WarehouseInService(_wmsTransaction) : _warehouseInService; }
      set { _warehouseInService = value; }
    }
    #endregion

    #endregion Service

    #region Repository

    #region F010201Repository
    private F010201Repository _F010201Repo;
    public F010201Repository F010201Repo
    {
      get { return _F010201Repo == null ? _F010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction) : _F010201Repo; }
      set { _F010201Repo = value; }
    }
    #endregion

    #region F1909Repository
    private F1909Repository _F1909Repo;
    public F1909Repository F1909Repo
    {
      get { return _F1909Repo == null ? _F1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction) : _F1909Repo; }
      set { _F1909Repo = value; }
    }
    #endregion

    #region F000904Repository
    private F000904Repository _F000904Repo;
    public F000904Repository F000904Repo
    {
      get { return _F000904Repo == null ? _F000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction) : _F000904Repo; }
      set { _F000904Repo = value; }
    }
    #endregion

    #region F010204Repository
    private F010204Repository _F010204Repo;
    public F010204Repository F010204Repo
    {
      get { return _F010204Repo == null ? _F010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction) : _F010204Repo; }
      set { _F010204Repo = value; }
    }
    #endregion

    #region F075110Repository
    private F075110Repository _F075110Repo;
    /// <summary>
    /// 此Repository使用情境較為特殊，預設不會帶入Transaction
    /// </summary>
    public F075110Repository F075110Repo
    {
      get { return _F075110Repo == null ? _F075110Repo = new F075110Repository(Schemas.CoreSchema) : _F075110Repo; }
      set { _F075110Repo = value; }
    }
    #endregion

    #region F02020101Repository
    private F02020101Repository _F02020101Repo;
    public F02020101Repository F02020101Repo
    {
      get { return _F02020101Repo == null ? _F02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction) : _F02020101Repo; }
      set { _F02020101Repo = value; }
    }
    #endregion

    #region F010202Repository
    private F010202Repository _F010202Repo;
    public F010202Repository F010202Repo
    {
      get { return _F010202Repo == null ? _F010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction) : _F010202Repo; }
      set { _F010202Repo = value; }
    }
    #endregion

    #region F1905Repository
    private F1905Repository _F1905Repo;
    public F1905Repository F1905Repo
    {
      get { return _F1905Repo == null ? _F1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction) : _F1905Repo; }
      set { _F1905Repo = value; }
    }
    #endregion

    #region F1980Repository
    private F1980Repository _F1980Repo;
    public F1980Repository F1980Repo
    {
      get { return _F1980Repo == null ? _F1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction) : _F1980Repo; }
      set { _F1980Repo = value; }
    }
    #endregion

    #region F190206Repository
    private F190206Repository _F190206Repo;
    public F190206Repository F190206Repo
    {
      get { return _F190206Repo == null ? _F190206Repo = new F190206Repository(Schemas.CoreSchema, _wmsTransaction) : _F190206Repo; }
      set { _F190206Repo = value; }
    }
    #endregion

    #region F1946Repository
    private F1946Repository _F1946Repo;
    public F1946Repository F1946Repo
    {
      get { return _F1946Repo == null ? _F1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction) : _F1946Repo; }
      set { _F1946Repo = value; }
    }
    #endregion

    #region F010205Repository
    private F010205Repository _F010205Repo;
    public F010205Repository F010205Repo
    {
      get { return _F010205Repo == null ? _F010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction) : _F010205Repo; }
      set { _F010205Repo = value; }
    }
    #endregion

    #region F02020109Repository
    private F02020109Repository _F02020109Repo;
    public F02020109Repository F02020109Repo
    {
      get { return _F02020109Repo == null ? _F02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction) : _F02020109Repo; }
      set { _F02020109Repo = value; }
    }
    #endregion

    #region F1903Repository
    private F1903Repository _F1903Repo;
    public F1903Repository F1903Repo
    {
      get { return _F1903Repo == null ? _F1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction) : _F1903Repo; }
      set { _F1903Repo = value; }
    }
    #endregion

    #region F020201Repository
    private F020201Repository _F020201Repo;
    public F020201Repository F020201Repo
    {
      get { return _F020201Repo == null ? _F020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction) : _F020201Repo; }
      set { _F020201Repo = value; }
    }
    #endregion

    #region F02020102Repository
    private F02020102Repository _F02020102Repo;
    public F02020102Repository F02020102Repo
    {
      get { return _F02020102Repo == null ? _F02020102Repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction) : _F02020102Repo; }
      set { _F02020102Repo = value; }
    }
    #endregion

    #region F020302Repository
    private F020302Repository _f020302Repo;
    public F020302Repository F020302Repo
    {
      get { return _f020302Repo == null ? _f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction) : _f020302Repo; }
      set { _f020302Repo = value; }
    }
    #endregion

    #region F02020104Repository
    private F02020104Repository _f02020104Repo;
    public F02020104Repository F02020104Repo
    {
      get { return _f02020104Repo == null ? _f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction) : _f02020104Repo; }
      set { _f02020104Repo = value; }
    }
    #endregion

    #region F020301Repository
    private F020301Repository _f020301Repo;
    public F020301Repository F020301Repo
    {
      get { return _f020301Repo == null ? _f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction) : _f020301Repo; }
      set { _f020301Repo = value; }
    }
    #endregion

    #endregion Repository

    public P810119Service(WmsTransaction wmsTransation)
    {
      _wmsTransaction = wmsTransation;
      _p81Service = new P81Service();
    }

    /// <summary>
    /// 驗收資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetStockReceivedData(GetStockReceivedDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };


      //把User畫面輸入的內容轉大寫
      req.WmsNo = req.WmsNo?.ToUpper();
      req.ItemNo = req.ItemNo?.ToUpper();


      //4.	若 WmsNo is null 而且 ItemNo is null，回傳錯誤訊息”單據號碼或商品條碼不得為空”，else 走資料處理
      if (string.IsNullOrWhiteSpace(req.WmsNo) && string.IsNullOrWhiteSpace(req.ItemNo))
        return new ApiResult { IsSuccessed = false, MsgCode = "21901", MsgContent = _p81Service.GetMsg("21901") };
      #endregion 資料檢核

      #region 資料處理
      //2.	若因為系統設定，有些功能不適合在PDA上使用，則請跳出錯誤訊息於畫面上。
      var f1909 = F1909Repo.GetF1909(gupCode, req.CustNo);
      if (f1909 != null)
      {
        //(2)	貨主要求列印進倉棧板標籤: 
        if (f1909.IS_PRINT_INSTOCKPALLETSTICKER == "1")
          return new ApiResult { IsSuccessed = false, MsgCode = "21916", MsgContent = _p81Service.GetMsg("21916") };
        //(3)	進倉單需要上傳檔案才能結案
        if (f1909.ISUPLOADFILE == "1")
          return new ApiResult { IsSuccessed = false, MsgCode = "21917", MsgContent = _p81Service.GetMsg("21917") };
      }

      var returnData = F010201Repo.GetStockReceivedDataRes(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.ItemNo).ToList();
      var errResults = new List<ApiResult>();

      if (returnData == null || !returnData.Any())
      {
        if (!string.IsNullOrWhiteSpace(req.ItemNo))
          //未完成的驗收進倉單中，品號{0}不存在
          return new ApiResult() { IsSuccessed = false, MsgCode = "21903", MsgContent = string.Format(_p81Service.GetMsg("21903"), req.ItemNo) };
        //進倉單資料不存在
        if (!string.IsNullOrEmpty(req.WmsNo))
          return new ApiResult() { IsSuccessed = false, MsgCode = "21902", MsgContent = _p81Service.GetMsg("21902") };

      }
      if (!string.IsNullOrEmpty(req.WmsNo) && returnData.First().CustCost == "MoveIn")
        return new ApiResult() { IsSuccessed = false, MsgCode = "21953", MsgContent = _p81Service.GetMsg("21953") };

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = returnData };
      #endregion
    }

    /// <summary>
    /// 驗收明細資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetStockReceivedDetData(GetStockReceivedDetDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      //4.	若  PalletLocation is null，回傳錯誤訊息”棧板的儲位條碼不得為空”
      if (string.IsNullOrWhiteSpace(req.PalletLocation))
        return new ApiResult { IsSuccessed = false, MsgCode = "21918", MsgContent = _p81Service.GetMsg("21918") };
      if (req.PalletLocation.Length > 4)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "棧板容器儲位條碼", 4) };

      req.WmsNo = req.WmsNo?.ToUpper();
      req.WmsSeq = req.WmsSeq?.ToUpper();
      req.PalletLocation = req.PalletLocation?.ToUpper();
      req.ItemCode = req.ItemCode?.ToUpper();
      req.WorkStationCode = req.WorkStationCode?.ToUpper();

      //5.	[K]= 取得是否需要呼叫影資[F0003.SYS_PATH=1 ? true : false] F0003 WHERE DC_CODE = dcNo AND AP_NAME = VideoCombinIn
      var IsVideoCombinIn = CommonService.GetSysGlobalValue(req.DcNo, "VideoCombinIn") == "1";
      //6.	如果[K]=True AND  WorkStationCode=NULL OR 空白，回傳[影資串接已開啟，必須提供工作站編號]
      if (IsVideoCombinIn && string.IsNullOrWhiteSpace(req.WorkStationCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "21919", MsgContent = _p81Service.GetMsg("21919"), Data = new GetStockReceivedDetDataRes { ErrorType = "1" } };
      //7.	如果[K]=True AND  WorkStationCode有值
      else if (IsVideoCombinIn && !string.IsNullOrWhiteSpace(req.WorkStationCode))
      {
        //(1)	[S]=取得工作站群組代碼[F000904.VALUE WHERE TOPIC=F1946,SUBTOPIC=GROUP AND NAME=驗貨區]
        var workStationGroupCode = F000904Repo.GetWorkStationGroupCode();
        //(2)	若[S]不存在，回傳 [無法識別的工作群組名稱[驗貨區]]
        if (string.IsNullOrWhiteSpace(workStationGroupCode))
          return new ApiResult { IsSuccessed = false, MsgCode = "21921", MsgContent = _p81Service.GetMsg("21921"), Data = new GetStockReceivedDetDataRes { ErrorType = "1" } };
        //(3)	若[S]存在，檢查工作站編號是否正確，WorkStationCode第一碼是否為[S]，若不是回傳 [工作站編號XXX不屬於驗貨區，必需為[S]開頭的]
        if (!string.IsNullOrWhiteSpace(workStationGroupCode) && !req.WorkStationCode.StartsWith(workStationGroupCode))
          return new ApiResult
          {
            IsSuccessed = false,
            MsgCode = "21920",
            MsgContent = string.Format(_p81Service.GetMsg("21920"), req.WorkStationCode, workStationGroupCode),
            Data = new GetStockReceivedDetDataRes { ErrorType = "1" }
          };

        var f1946 = F1946Repo.Find(x => x.DC_CODE == req.DcNo && x.WORKSTATION_CODE == req.WorkStationCode);
        if (f1946 == null)
          return new ApiResult
          {
            IsSuccessed = false,
            MsgCode = "21958",
            MsgContent = string.Format(_p81Service.GetMsg("21958"), req.WorkStationCode),
            Data = new GetStockReceivedDetDataRes { ErrorType = "1" }
          };

      }
      //8.	若 IsVirtualItem = true，表示此商品為虛擬商品，顯示錯誤訊息”此商品為虛擬商品，請使用電腦版進行驗收”
      if (req.IsVirtualItem)
        return new ApiResult { IsSuccessed = false, MsgCode = "21922", MsgContent = _p81Service.GetMsg("21922") };
      //9.	檢查進倉單的狀態
      //(1)	撈F010201 by dc+gup+cust+stock_no = WmsNo
      var f010201 = F010201Repo.FindDataByStockNoOrCustOrdNo(req.DcNo, gupCode, req.CustNo, req.WmsNo);
      //(3)	若F010201.status 不為1(驗收中)或3(已點收)，顯示錯誤訊息”進倉單狀態為{status轉中文}，不可進行驗收”
      if (!new[] { "1", "3" }.Contains(f010201.STATUS))
      {
        var statusName = F000904Repo.GetF010201StatusName(f010201.STATUS);
        return new ApiResult { IsSuccessed = false, MsgCode = "21923", MsgContent = string.Format(_p81Service.GetMsg("21923"), statusName) };
      }
      //10.	檢查進倉單該明細否還有未驗收數
      int wmsSeq = Convert.ToInt32(req.WmsSeq);
      var f010204 = F010204Repo.GetData(req.DcNo, gupCode, req.CustNo, req.WmsNo, wmsSeq);
      if (f010204 != null && f010204.STOCK_QTY <= f010204.TOTAL_REC_QTY)
        return new ApiResult { IsSuccessed = false, MsgCode = "21924", MsgContent = _p81Service.GetMsg("21924") };
      #endregion 資料檢核

      #region 資料鎖定
      var chkProcUser = LockAcceptenceOrderConfirm(new LockAcceptenceOrderReq
      {
        DcCode = f010201.DC_CODE,
        GupCode = f010201.GUP_CODE,
        CustCode = f010201.CUST_CODE,
        StockNo = f010201.STOCK_NO,
        IsChangeUser = req.IsChangeUser,
      });
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;

      #endregion 資料鎖定

      #region 資料處理

      var res = WarehouseInRecvService.GetOrCreateRecvData(f010201, IsVideoCombinIn, req.WorkStationCode, req.PalletLocation, req.WmsSeq);
      if (!res.IsSuccessed)
        return new ApiResult { IsSuccessed = res.IsSuccessed, MsgCode = res.MsgCode, MsgContent = res.MsgContent };

      var resData = res.Data as GetOrCreateRecvDataRes;
      var currentF02020101 = resData.F02020101List.First(x => x.PURCHASE_SEQ == req.WmsSeq);
      var apiInfoList = resData.ApiInfoList;

      if (currentF02020101.PALLET_LOCATION != req.PalletLocation)
        //原棧板號為 {0} 與目前棧板號 {1} 不同不可作業
        return new ApiResult { IsSuccessed = false, MsgCode = "21959", MsgContent = string.Format(_p81Service.GetMsg("21959"), currentF02020101.PALLET_LOCATION, req.PalletLocation) };


      var currentF1903 = CommonService.GetProduct(gupCode, req.CustNo, currentF02020101.ITEM_CODE);
      var recvBadQty = currentF02020101.DEFECT_QTY ?? 0; ;
      var recvGoodQty = 0;
      if (currentF02020101.CHECK_ITEM == "1")
        recvGoodQty = currentF02020101.RECV_QTY.Value - recvBadQty;

      var warehouseName = "";
      if (!string.IsNullOrWhiteSpace(currentF02020101.TARWAREHOUSE_ID))
        warehouseName = CommonService.GetWarehouse(req.DcNo, currentF02020101.TARWAREHOUSE_ID).WAREHOUSE_NAME;
      else
        warehouseName = "未提供上架倉別";

      var todayTotalRecvQty = F020201Repo.GetTodayRecvQty(req.DcNo, gupCode, req.CustNo, f010201.STOCK_NO, DateTime.Today);

      var data = new GetStockReceivedDetDataRes
      {
        StockNo = currentF02020101.PURCHASE_NO,
        StockSeq = currentF02020101.PURCHASE_SEQ,
        ItemCode = currentF02020101.ITEM_CODE,
        VnrItemCode = currentF1903.VNR_ITEM_CODE,
        OrderQty = currentF02020101.ORDER_QTY.HasValue ? currentF02020101.ORDER_QTY.Value : 0,
        TotalRecvQty = f010204?.TOTAL_REC_QTY ?? 0,
        RecvQty = currentF02020101.RECV_QTY.HasValue ? currentF02020101.RECV_QTY.Value : 0,
        RecvGoodQty = recvGoodQty,
        RecvBadQty = recvBadQty,
        CheckItem = currentF02020101.CHECK_ITEM,
        CheckSerial = currentF02020101.CHECK_SERIAL,
        TarWarehouseId = currentF02020101.TARWAREHOUSE_ID ?? "",
        TarWarehouseName = warehouseName,
        RtNo = currentF02020101.RT_NO,
        RtSeq = currentF02020101.RT_SEQ,
        BundleSerialNo = currentF1903.BUNDLE_SERIALNO,
        PoNo = f010201.SHOP_NO,
        TodayRecvQty = todayTotalRecvQty,
        ApiFailureMsgList = apiInfoList,
      };

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = data };
      #endregion
    }

    /// <summary>
    /// 進貨檢驗-驗收商品查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetRecvItemData(RecvItemDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #endregion 資料檢核

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;

      #endregion

      #region 資料處理
      //2.	[B]=撈出進倉驗收暫存資料，資料表: F02020101 + F1903 + F1980(NOLOCK)
      var result = F02020101Repo.GetRecvItemDataRes(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (result == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21902", MsgContent = _p81Service.GetMsg("21902") };

      //3.	[C]=撈出商品檢驗項目
      var checkOpts = F190206Repo.GetCheckItems(gupCode, req.CustNo, result.ItemCode).ToList();
      result.CheckOptList = checkOpts.Select(x => new CheckOpt { CheckNo = x.CHECK_NO, CheckName = x.CHECK_NAME }).ToList();

      //4.	[D] = 撈出商品材積檔
      var f1905 = F1905Repo.GetData(gupCode, req.CustNo, result.ItemCode);
      result.PackLength = f1905.PACK_LENGTH;
      result.PackWidth = f1905.PACK_WIDTH;
      result.PackHight = f1905.PACK_HIGHT;
      result.PackWeight = f1905.PACK_WEIGHT;

      //5.	[E]= 取得商品溫層清單
      var tmprList = CommonService.GetF000904s("F1903", "TMPR_TYPE").ToList()
        .Select(x => new TmprOpt
        {
          TmprNo = x.VALUE,
          TmprName = x.NAME
        }).ToList();
      result.TmprOptList = tmprList;

      result.CanEditTareWarehouse = CommonService.GetSysGlobalValue(req.DcNo, gupCode, req.CustNo, "RecvItemNeedContainer");

      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = result };
      #endregion 資料處理

    }

    /// <summary>
    /// 進貨檢驗-序號刷讀查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetSerialItemData(GetSerialItemDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          string.IsNullOrWhiteSpace(req.ItemCode) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
      #endregion

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;
      #endregion

      #region 資料處理
      var warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction);

      //2.	[B]=撈出進倉驗收暫存資料
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937") };


      // 3. 取得已收集未驗收序號清單
      var collectSerialList = warehouseInRecvService.GetCollectSerialList(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.PoNo, req.ItemCode);
      var collectSerialCnt = collectSerialList.Count;
      // 4. 取得已刷讀並通過序號清單
      var scanSerialCnt = warehouseInRecvService.GetIsPassSerialCnt(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.RtNo);
      // 5. 序號處理模式 (0: 序號抽驗模式、1: 序號收集模式) 
      //    如果已收集序號數 > 已刷讀並通過數量 且已收集序號數>=驗收數 則為序號抽驗模式 else 序號收集模式
      var checkMode = collectSerialCnt > scanSerialCnt && collectSerialCnt >= f02020101.RECV_QTY ? "0" : "1";
      // 6. 應收數 = 如果已收集序號數> 已刷讀並通過數量 則應刷數=抽驗數 else 應刷數=驗收數 
      var needQty = checkMode == "0" ? f02020101.CHECK_QTY ?? 0 : f02020101.RECV_QTY ?? 0;
      // 7. 已刷數 = 已刷讀並通過數量
      var realQty = scanSerialCnt;

      var data = new GetSerialItemDataRes
      {
        ItemCode = req.ItemCode,
        CheckMode = checkMode,
        NeedSerialQty = needQty,
        ReadSerialQty = realQty,
        ValidDate = f02020101.VALI_DATE.HasValue ? f02020101.VALI_DATE.Value.ToString("yyyy/MM/dd") : "",
        ItemSerialList = checkMode == "0" ? new List<string>() : collectSerialList
      };

      // 如果是序號收集模式才去判斷要不要新增F020301
      if (checkMode == "1")
      {
        var crtF020301Res = warehouseInRecvService.CheckAndInserF020301(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.RtNo);
        if (!crtF020301Res.IsSuccessed)
          return crtF020301Res;
      }

      //F020301寫入
      _wmsTransaction.Complete();
      #endregion
      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = data };
    }

    /// <summary>
    /// 進貨檢驗-序號登錄/序號刪除
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult AddOrDelItemSerialNo(AddOrDelItemSerialNoReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          string.IsNullOrWhiteSpace(req.CheckMode) ||
          string.IsNullOrWhiteSpace(req.ProcMode) ||
          string.IsNullOrWhiteSpace(req.ItemNo) ||
          string.IsNullOrWhiteSpace(req.SerialNo) ||
          (req.ProcMode == "0" && !req.NeedQty.HasValue) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			if (req.SerialNo.Length > 50)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "序號", 50) };

			#endregion

			#region 資料鎖定
			var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;
      #endregion

      // 序號轉大寫
      req.SerialNo = req.SerialNo.ToUpper();

      if (req.ProcMode == "0")
        return AddSerialNo(req, gupCode);
      else
        return DelSerialNo(req, gupCode);
    }

    /// <summary>
    /// 進貨檢驗-序號登錄
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    private ApiResult AddSerialNo(AddOrDelItemSerialNoReq req, string gupCode)
    {

      var warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction);
      ApiResult res;
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937") };

      var isPassSerialCnt = warehouseInRecvService.GetIsPassSerialCnt(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.RtNo);
      if (isPassSerialCnt + 1 > req.NeedQty)
        return new ApiResult { IsSuccessed = false, MsgCode = "21908", MsgContent = _p81Service.GetMsg("21908") };

      if (req.CheckMode == "1") // 序號收集模式
        res = warehouseInRecvService.AddCollectSerialAndScanLog(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.PoNo, req.RtNo, req.ItemNo, new List<string> { req.SerialNo });
      else // 序號抽驗模式
        res = warehouseInRecvService.RandomCheckSerial(req.DcNo, gupCode, req.CustNo, req.ItemNo, req.WmsNo, req.WmsSeq, req.PoNo, req.RtNo, req.SerialNo);

      if (res.IsSuccessed && isPassSerialCnt + 1 >= req.NeedQty && f02020101.CHECK_SERIAL == "0")
        F02020101Repo.UpdateCheckSerial(f02020101.DC_CODE, f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.PURCHASE_NO, f02020101.PURCHASE_SEQ, f02020101.RT_NO, f02020101.RT_SEQ, "1");

      if (res.IsSuccessed)
        _wmsTransaction.Complete();

      if (res.MsgCode == "21904")
        res.MsgContent = ((List<SerialNoResult>)res.Data).First().Message;
      return res;

    }

    /// <summary>
    /// 進貨檢驗-序號刪除
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    private ApiResult DelSerialNo(AddOrDelItemSerialNoReq req, string gupCode)
    {
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937") };

      var warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction);
      ApiResult res;
      if (req.CheckMode == "1") // 序號收集模式
      {
        res = warehouseInRecvService.RemoveCollectSerialAndScanLog(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.RtNo, req.ItemNo, req.SerialNo, req.PoNo);
      }
      else // 序號抽驗模式
      {
        res = warehouseInRecvService.RemoveScanSerialLog(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.RtNo, req.SerialNo);
      }
      var isPassSerialCnt = warehouseInRecvService.GetIsPassSerialCnt(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.RtNo);
      req.NeedQty = req.CheckMode == "1" ? f02020101.RECV_QTY : f02020101.CHECK_QTY;

      if (res.IsSuccessed && isPassSerialCnt - 1 < req.NeedQty && f02020101.CHECK_SERIAL == "1")
        F02020101Repo.UpdateCheckSerial(f02020101.DC_CODE, f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.PURCHASE_NO, f02020101.PURCHASE_SEQ, f02020101.RT_NO, f02020101.RT_SEQ, "0");

      if (res.IsSuccessed)
        _wmsTransaction.Complete();

      return res;
    }

    /// <summary>
    /// 進貨檢驗-取得不良品登錄查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetDefectItemData(GetDefectItemDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
      #endregion

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;
      #endregion


      #region 資料處理

      //2.	[B]=撈出進倉驗收暫存資料
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937") };

      var f1980Reo = new F1980Repository(Schemas.CoreSchema);
      var warehouseList = f1980Reo.GetDefectWarehouseList(req.DcNo).ToList();
      var f1951Repo = new F1951Repository(Schemas.CoreSchema);
      var causeList = f1951Repo.GetDefectCauseList().ToList();
      var details = F02020109Repo.GetDefectInfoByUnRecvFinish(req.DcNo, gupCode, req.CustNo, req.WmsNo, int.Parse(req.WmsSeq)).ToList();
      var res = new GetDefectItemDataRes
      {
        ValidDate = f02020101.VALI_DATE.HasValue ? f02020101.VALI_DATE.Value.ToString("yyyy/MM/dd") : "",
        DefectDetail = details,
        WarehouseList = warehouseList,
        CauseList = causeList,
        DfWarehouse = details.Any() ? warehouseList.FirstOrDefault(x => x.WarehouseId == details.First().WarehouseId) ?? warehouseList.First() : warehouseList.First()
      };

      #endregion

      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = res };
    }

    /// <summary>
    /// 進貨檢驗-不良品登錄/刪除
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult AddOrDelDefectItem(AddOrDelDefectItemReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.ProcMode) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 新增不良品明細 參數檢查
      if (req.ProcMode == "0" && (
        string.IsNullOrWhiteSpace(req.WmsNo) ||
        string.IsNullOrWhiteSpace(req.WmsSeq) ||
        string.IsNullOrWhiteSpace(req.RtNo) ||
        string.IsNullOrWhiteSpace(req.RtSeq) ||
        string.IsNullOrWhiteSpace(req.ItemNo) ||
        string.IsNullOrWhiteSpace(req.WarehouseId) ||
        string.IsNullOrWhiteSpace(req.UccCode) ||
        !req.Qty.HasValue))
      {
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
      }
      // 刪除不良品明細 參數檢查
      if (req.ProcMode == "1" && !req.DefectId.HasValue)
      {
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
      }

      // 檢查數量不可小於等於0
      if (req.ProcMode == "0" && req.Qty <= 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20059", MsgContent = _p81Service.GetMsg("20059") };

			if (req.Memo.Length > 50)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "備註", 50) };

      //把User畫面輸入的內容轉大寫
      req.SerialNo = req.SerialNo?.ToUpper();
      #endregion

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;
      #endregion

      #region 資料處理

      //2.	[B]=撈出進倉驗收暫存資料
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937") };


      if (req.ProcMode == "0" && f02020101.RECV_QTY - f02020101.DEFECT_QTY - req.Qty < 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "21949", MsgContent = _p81Service.GetMsg("21949") };


      var warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction);
      ApiResult res;
      if (req.ProcMode == "0") // 新增
        res = warehouseInRecvService.AddDefect(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.PoNo, req.WarehouseId, req.UccCode, req.ItemNo, req.Qty.Value, req.SerialNo, req.Memo);
      else // 刪除
        res = warehouseInRecvService.DelDefect(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.WmsSeq, req.DefectId.Value);

      if (res.IsSuccessed)
        _wmsTransaction.Complete();

      var details = F02020109Repo.GetDefectInfoByUnRecvFinish(req.DcNo, gupCode, req.CustNo, req.WmsNo, int.Parse(req.WmsSeq)).ToList();

      res.Data = details;

      #endregion


      return res;
    }

    /// <summary>
    /// 儲存商品驗收註記
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult SaveRecvItemMemo(SaveRecvItemMemoReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.ItemNo) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
			if (req.RcvMemo.Length > 200)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "驗收註記", 200) };


			//把User畫面輸入的內容轉大寫
			req.ItemNo = req.ItemNo?.ToUpper();

      #endregion 資料檢核

      #region 資料處理
      //2.	更新商品驗收註記
      F1903Repo.UpdateRcvMemo(gupCode, req.CustNo, req.ItemNo, req.RcvMemo);

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };
      #endregion
    }

    /// <summary>
    /// 取得物流中心進貨可上架倉別清單
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetWarhouseInDcTarWarehouseList(GetWarhouseInDcTarWarehouseListReq req)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
          string.IsNullOrWhiteSpace(req.AccNo) ||
          string.IsNullOrWhiteSpace(req.DcNo) ||
          accData ||
          accFunctionCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #endregion 資料檢核

      #region 資料處理
      //2.	取得可上架倉別編號
      var f1980s = F1980Repo.GetDatasForNotWarehouseTypes(req.DcNo, new List<string> { "T", "I", "D", "M" }).ToList();

      var data = f1980s.Select(s => new GetWarhouseInDcTarWarehouseListRes
      {
        WarehouseId = s.WAREHOUSE_ID,
        WarehouseName = s.WAREHOUSE_NAME,
      }).ToList();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = data };
      #endregion
    }

    /// <summary>
    /// 儲存商品驗收結果
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult CheckandSaveRecvItemData(CheckandSaveRecvItemDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) || string.IsNullOrWhiteSpace(req.AccNo) ||
          string.IsNullOrWhiteSpace(req.DcNo) || string.IsNullOrWhiteSpace(req.CustNo) ||
          string.IsNullOrWhiteSpace(req.WmsNo) || string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) || string.IsNullOrWhiteSpace(req.RtSeq) ||
          string.IsNullOrWhiteSpace(req.ItemNo) || string.IsNullOrWhiteSpace(req.NeedExpired) ||
          string.IsNullOrWhiteSpace(req.BundleSerialNo) || string.IsNullOrWhiteSpace(req.TmprType) ||
          string.IsNullOrWhiteSpace(req.IsApple) || string.IsNullOrWhiteSpace(req.IsPrintItemId) ||
          string.IsNullOrWhiteSpace(req.IsPrecious) ||
          string.IsNullOrWhiteSpace(req.IsFragile) || string.IsNullOrWhiteSpace(req.IsEasyLose) ||
          string.IsNullOrWhiteSpace(req.IsMagentic) || string.IsNullOrWhiteSpace(req.IsSpill) ||
          string.IsNullOrWhiteSpace(req.IsPerishable) || string.IsNullOrWhiteSpace(req.IsTempControl) ||
          string.IsNullOrWhiteSpace(req.IsFirstInDate) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      if (req.CheckNoList == null ||
          req.CheckNoList.Count == 0 ||
          req.CheckNoList.Any(a => string.IsNullOrWhiteSpace(a.CheckNo)))
        //效期商品必須填入總保存天數
        return new ApiResult { IsSuccessed = false, MsgCode = "21960", MsgContent = _p81Service.GetMsg("21960") };

			if (req.EanCode1.Length > 26)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "國際條碼一", 26) };
			if (req.EanCode2.Length > 26)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "國際條碼二", 26) };
			if (req.EanCode3.Length > 26)
				return new ApiResult { IsSuccessed = false, MsgCode = "21962", MsgContent = string.Format(_p81Service.GetMsg("21962"), "國際條碼三", 26) };

			//人員輸入的資料都改成大寫
			req.WmsNo = req.WmsNo?.ToUpper();
      req.WmsSeq = req.WmsSeq?.ToUpper();
      req.RtNo = req.RtNo?.ToUpper();
      req.RtSeq = req.RtSeq?.ToUpper();
      req.ItemNo = req.ItemNo?.ToUpper();
      req.EanCode1 = req.EanCode1?.ToUpper();
      req.EanCode2 = req.EanCode2?.ToUpper();
      req.EanCode3 = req.EanCode3?.ToUpper();
      req.NeedExpired = req.NeedExpired?.ToUpper();
      req.BundleSerialNo = req.BundleSerialNo?.ToUpper();
      req.TmprType = req.TmprType?.ToUpper();
      req.IsApple = req.IsApple?.ToUpper();
      req.IsPrintItemId = req.IsPrintItemId?.ToUpper();
      req.ValidDate = req.ValidDate?.ToUpper();
      req.IsPrecious = req.IsPrecious?.ToUpper();
      req.IsFragile = req.IsFragile?.ToUpper();
      req.IsEasyLose = req.IsEasyLose?.ToUpper();
      req.IsMagentic = req.IsMagentic?.ToUpper();
      req.IsSpill = req.IsSpill?.ToUpper();
      req.IsPerishable = req.IsPerishable?.ToUpper();
      req.IsTempControl = req.IsTempControl?.ToUpper();
      req.IsFirstInDate = req.IsFirstInDate?.ToUpper();
      req.TarWarehouseId = req.TarWarehouseId?.ToUpper();
      req.CheckNoList.ForEach(x => x.CheckNo = x.CheckNo?.ToUpper());

      if (req.RecvQty < 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "21950", MsgContent = _p81Service.GetMsg("21950") };

      //如果為效期商品NeedExpired=1，ValidDate的空白或NULL，回傳[效期商品必須填入效期]
      if (req.NeedExpired == "1")
      {
        if (string.IsNullOrWhiteSpace(req.ValidDate))
          return new ApiResult { IsSuccessed = false, MsgCode = "21932", MsgContent = _p81Service.GetMsg("21932") };
        if(!req.SaveDay.HasValue)
          return new ApiResult { IsSuccessed = false, MsgCode = "21961", MsgContent = _p81Service.GetMsg("21961") };
      }
      //商品效期欄位檢查
      if (!DateTimeHelper.CheckDate(req.ValidDate, new List<String> { "yyyy/MM/dd", "yyyyMMdd", "yyyy/M/d" }))
        return new ApiResult { IsSuccessed = false, MsgCode = "21957", MsgContent = _p81Service.GetMsg("21957") }; //驗收效期欄位資料格式不正確，請人員再次確認
      var validDate = DateTimeHelper.ConversionDate(req.ValidDate);

      //[K]=取得貨主主檔
      var f1909 = F1909Repo.GetF1909(gupCode, req.CustNo);
      if (f1909 != null)
      {
        //6.	如果[K].NEED_ITEMSPEC=1，長/寬/高如果都為1，回傳[新品規格的長寬高需大於1]
        if (f1909.NEED_ITEMSPEC == "1" && req.PackLength == 1 && req.PackWidth == 1 && req.PackHight == 1)
          return new ApiResult { IsSuccessed = false, MsgCode = "21930", MsgContent = _p81Service.GetMsg("21930") };
        //7.	如果[K].NEED_ITEMSPEC=0，長/寬/高/重量任一個為0，回傳[商品長/寬/高/重量必需大於0]
        if (f1909.NEED_ITEMSPEC == "0" && (req.PackLength == 0 || req.PackWidth == 0 || req.PackHight == 0 || req.PackWeight == 0))
          return new ApiResult { IsSuccessed = false, MsgCode = "21931", MsgContent = _p81Service.GetMsg("21931") };
      }

      //如果為效期商品NeedExpired=1
      if (req.NeedExpired == "1")
      {
        //(2)	保存天數SaveQty<=30，回傳[效期商品保存天數必須大於30天]
        if (req.SaveDay <= 30)
          return new ApiResult { IsSuccessed = false, MsgCode = "21933", MsgContent = _p81Service.GetMsg("21933") };
        //(3)	效期必須大於今天
        if (validDate <= DateTime.Today)
          return new ApiResult { IsSuccessed = false, MsgCode = "21934", MsgContent = _p81Service.GetMsg("21934") };
        //(4)	如果[K].VALID_DATE_CHKYEAR!=NULL或空白且系統日+[K].VALID_DATE_CHKYEAR(年)<效期，回傳[效期不可超過{[K].VALID_DATE_CHKYEAR }年]
        if (f1909.VALID_DATE_CHKYEAR.HasValue && DateTime.Today.AddYears((int)f1909.VALID_DATE_CHKYEAR.Value) < validDate)
          return new ApiResult { IsSuccessed = false, MsgCode = "21935", MsgContent = string.Format(_p81Service.GetMsg("21935"), (int)f1909.VALID_DATE_CHKYEAR.Value) };
      }
      //9.	如果為非效期商品 NeedExpired=0，如果ValidDate<>9999/12/31，回傳[非效期商品，不可調整效期，請設定為9999/12/31]
      else if (req.NeedExpired == "0" && validDate != new DateTime(9999, 12, 31))
        return new ApiResult { IsSuccessed = false, MsgCode = "21936", MsgContent = _p81Service.GetMsg("21936") };

      #endregion 資料檢核

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;

      #endregion

      #region 資料處理
      var checkList = new List<CheckItem>();
      req.CheckNoList.ForEach(x => checkList.Add(new CheckItem { CheckNo = x.CheckNo }));
      var result = WarehouseInRecvService.SaveRecvItem(new SaveRecvItemParam
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        PurchaseNo = req.WmsNo,
        PurchaseSeq = req.WmsSeq,
        RtNo = req.RtNo,
        RtSeq = req.RtSeq,
        ItemNo = req.ItemNo,
        EanCode1 = req.EanCode1,
        EanCode2 = req.EanCode2,
        EanCode3 = req.EanCode3,
        NeedExpired = req.NeedExpired,
        SaveDay = req.SaveDay,
        AllowDelvDay = req.AllowDelvDay,
        AllowShipDay = req.AllowShipDay,
        BundleSerialNo = req.BundleSerialNo,
        PackLength = req.PackLength,
        PackWidth = req.PackWidth,
        PackHight = req.PackHight,
        PackWeight = req.PackWeight,
        TmprType = req.TmprType,
        IsApple = req.IsApple,
        RecvQty = req.RecvQty,
        IsPrintItemId = req.IsPrintItemId,
        ValidDate = validDate,
        IsPrecious = req.IsPrecious,
        IsFragile = req.IsFragile,
        IsEasyLose = req.IsEasyLose,
        IsMagentic = req.IsMagentic,
        IsSpill = req.IsSpill,
        IsPerishable = req.IsPerishable,
        IsTempControl = req.IsTempControl,
        IsFirstInDate = req.IsFirstInDate,
        TarWarehouseId = req.TarWarehouseId,
        CheckItemList = checkList,
        PoNo = req.PoNo
      });
      if (!result.IsSuccessed)
        return result;

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };
      #endregion
    }


    /// <summary>
    /// 鎖定進貨單控管表確認
    /// </summary>
    /// <param name="req">傳入參數</param>
    /// <returns></returns>
    private ApiResult LockAcceptenceOrderConfirm(LockAcceptenceOrderReq req)
    {
      #region 進貨單檢查&鎖定
      var f075110 = F075110Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF075110 = F075110Repo.LockF075110();
          var chkF075110 = F075110Repo.Find(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_NO == req.StockNo && x.STATUS == "0");
          if (chkF075110 == null)
          {
            var newF075110 = new F075110()
            {
              DC_CODE = req.DcCode,
              GUP_CODE = req.GupCode,
              CUST_CODE = req.CustCode,
              WMS_NO = req.StockNo,
              STATUS = "0",
              DEVICE_TOOL = "1",
              CRT_DATE = DateTime.Now,
              CRT_STAFF = Current.Staff,
              CRT_NAME = Current.StaffName
            };
            F075110Repo.Add(newF075110, true);
            return newF075110;
          }
          else //chkF075111 != null
          {
            if (chkF075110.DEVICE_TOOL == "0")
            {
              chkF075110.STATUS = "1";  //不可使用
              return chkF075110;
            }
            else //chkF075111.DEVICE_TOOL == "1"
            {
              if (chkF075110.CRT_STAFF == Current.Staff)
              {
                return chkF075110;
              }
              else //chkF075111.CRT_STAFF != req.AccNo
              {
                if (req.IsChangeUser)
                {
                  chkF075110.STATUS = "2"; //人員已確認更換人員
                  F075110Repo.UpdateFields(new { chkF075110.STATUS }, x => x.ID == chkF075110.ID);

                  var newF075110 = new F075110()
                  {
                    DC_CODE = req.DcCode,
                    GUP_CODE = req.GupCode,
                    CUST_CODE = req.CustCode,
                    WMS_NO = req.StockNo,
                    STATUS = "0", //可以使用
                    DEVICE_TOOL = "1",
                    CRT_DATE = DateTime.Now,
                    CRT_STAFF = Current.Staff,
                    CRT_NAME = Current.StaffName
                  };
                  F075110Repo.Add(newF075110, true);
                  return newF075110;
                }
                else
                {
                  chkF075110.STATUS = "2";  //等待人員確認
                  return chkF075110;
                }
              }
            }
          }
        });

      if (f075110.STATUS == "2")      // 等待人員確認
        return new ApiResult { IsSuccessed = false, MsgCode = "30001", MsgContent = "該單據已有人員(%s)進行作業中，請問是否要更換作業人員?".Replace("(%s)", $"({f075110.CRT_NAME})") };
      else if (f075110.STATUS == "1") // 不可使用
        return new ApiResult { IsSuccessed = false, MsgCode = "21940", MsgContent = string.Format("這張進倉單已被{0}使用{1}作業，不可使用", f075110.CRT_NAME, f075110.DEVICE_TOOL == "0" ? "電腦版" : "PDA") };

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
      #endregion
    }

    /// <summary>
    /// 驗收完成
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvItemCompleted(RecvItemCompletedReq req, string gupCode)
    {
      var apiInfoList = new List<string>();
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      //把User畫面輸入的內容轉大寫
      req.WmsNo = req.WmsNo?.ToUpper();
      req.WmsSeq = req.WmsSeq?.ToUpper();
      req.RtNo = req.RtNo?.ToUpper();
      req.RtSeq = req.RtSeq?.ToUpper();
      req.WorkStationCode = req.WorkStationCode?.ToUpper();

      #endregion 資料檢核

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        DeviceTool = "1",
        IsChangeUser = false,
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;

      #endregion

      #region 資料處理
      //2.	[B]=取得進倉單資料
      var f010201 = F010201Repo.GetDatasByStockNo(req.DcNo, gupCode, req.CustNo, req.WmsNo);
      if (f010201 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21902", MsgContent = _p81Service.GetMsg("21902"), Data = new RecvItemCompletedRes { IsGoQueryPage = true } };
      if (f010201.USER_CLOSED == "1")
        return new ApiResult { IsSuccessed = false, MsgCode = "21963", MsgContent = _p81Service.GetMsg("21963"), Data = new RecvItemCompletedRes { IsGoQueryPage = true } };
      //3.	再次檢查進倉單狀態，確認是否可以驗收
      if (!new[] { "1", "3" }.Contains(f010201.STATUS))
      {
        var statusName = F000904Repo.GetF010201StatusName(f010201.STATUS);
        return new ApiResult { IsSuccessed = false, MsgCode = "21923", MsgContent = string.Format(_p81Service.GetMsg("21923"), statusName), Data = new RecvItemCompletedRes { IsGoQueryPage = true } };
      }
      //4.	[C]= 取得驗收暫存檔資料
      var f02020101 = F02020101Repo.GetF02020101ByRt(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      //5.	如果[C]無資料，(1)	回傳[進倉單驗收暫存資料不存在]
      if (f02020101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21937", MsgContent = _p81Service.GetMsg("21937"), Data = new RecvItemCompletedRes { IsGoQueryPage = true } };
      //6.	如果[C]有資料，[C].CHECK_ITEM <>1
      if (f02020101.CHECK_ITEM != "1")
        return new ApiResult { IsSuccessed = false, MsgCode = "21941", MsgContent = _p81Service.GetMsg("21941"), Data = new RecvItemCompletedRes { IsGoQueryPage = false } };
      //7.	檢查商品資料是否為序號商品
      var f1903 = CommonService.GetProduct(f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.ITEM_CODE);
      if (f1903.BUNDLE_SERIALNO == "1" && f02020101.CHECK_SERIAL == "0")
        return new ApiResult { IsSuccessed = false, MsgCode = "21942", MsgContent = _p81Service.GetMsg("21942"), Data = new RecvItemCompletedRes { IsGoQueryPage = false } };

      if (f02020101.CHECK_SERIAL == "1")
      {
        var collectSnList = WarehouseInRecvService.GetCollectSerialList(f02020101.DC_CODE, f02020101.GUP_CODE, f02020101.CUST_CODE, f02020101.PURCHASE_NO, f010201.SHOP_NO, f02020101.ITEM_CODE);
        if (collectSnList.Count != f02020101.RECV_QTY)
          return new ApiResult { IsSuccessed = false, MsgCode = "21956", MsgContent = _p81Service.GetMsg("21956"), Data = new RecvItemCompletedRes { IsGoQueryPage = false } };
      }

      //8.	更新驗收暫存檔已檢驗該明細=1(是)
      F02020101Repo.UpdateFields(
          new { CHECK_DETAIL = "1", },
          x => x.DC_CODE == f02020101.DC_CODE &&
               x.GUP_CODE == f02020101.GUP_CODE &&
               x.CUST_CODE == f02020101.CUST_CODE &&
               x.PURCHASE_NO == f02020101.PURCHASE_NO &&
               x.PURCHASE_SEQ == f02020101.PURCHASE_SEQ &&
               x.RT_NO == f02020101.RT_NO &&
               x.RT_SEQ == f02020101.RT_SEQ);
      //9.	檢查驗收單是否還有其他明細還沒驗收完成
      var IsAcceptanceComplete = F02020101Repo.IsAcceptanceComplete(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq);
      if (!IsAcceptanceComplete)
      {
        _wmsTransaction.Complete();
        return new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "21943",
          MsgContent = _p81Service.GetMsg("21943"),
          Data = new RecvItemCompletedRes { StockNo = f02020101.PURCHASE_NO, RtNo = f02020101.RT_NO, IsGoQueryPage = true }
        };
      }
      //12.	[D]= 取得驗收單所有明細
      var f02020101s = F02020101Repo.GetF02020101sByRtNo(req.DcNo, gupCode, req.CustNo, req.RtNo).ToList();
      //13.	更新[D].CHECK_DETAIL = 1 WHERE RT_SEQ = RtSeq
      f02020101s.First(x => x.RT_SEQ == req.RtSeq).CHECK_DETAIL = "1";
      // 若所有驗收明細驗收數量為0，不允許驗收確認，請人員做刪除驗收紀錄
      if (f02020101s.All(x => x.RECV_QTY == 0))
      {
        return new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "21952",
          MsgContent = _p81Service.GetMsg("21952"),
          Data = new RecvItemCompletedRes { IsGoQueryPage = false }
        };
      }

      var f1909 = F1909Repo.GetF1909(gupCode, req.CustNo);
      var confirmResult = WarehouseInRecvService.AcceptanceConfirm(new AcceptanceConfirmPara
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        PurchaseNo = req.WmsNo,
        RTNo = req.RtNo,
        f02020101s = f02020101s,
        f010201 = f010201,
        IsPickLocFirst = f1909 != null ? f1909.ISPICKLOCFIRST == "1" : false,
      });
      if (!confirmResult.IsSuccessed)
        return new ApiResult { IsSuccessed = false, MsgCode = "20052", MsgContent = confirmResult.Message, Data = new RecvItemCompletedRes { IsGoQueryPage = false } };

      //26.	[K]= 取得是否需要呼叫影資[F0003.SYS_PATH=1 ? true : false] F0003 WHERE DC_CODE = dcNo AND AP_NAME = VideoCombinIn
      var IsVideoCombinIn = CommonService.GetSysGlobalValue(req.DcNo, "VideoCombinIn") == "1";
      if (IsVideoCombinIn)
      {
        var itemCodeList = f02020101s.Select(s => s.ITEM_CODE).ToList();
        foreach (var itemCode in itemCodeList)
        {
          RecvItemNotifyReq notifyReq = new RecvItemNotifyReq()
          {
            WhId = req.DcNo,   //倉庫代碼
            OrderNo = req.RtNo, //驗收單號
            WorkStationId = req.WorkStationCode,  //工作站ID
            SkuId = itemCode,  //商品ID(不用填寫)
            TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") //時間戳記
          };
          var apiresult = RecvItemService.RecvItemNotify(req.DcNo, gupCode, req.CustNo, notifyReq);
          if (!apiresult.IsSuccessed)
            apiInfoList.Add(apiresult.MsgContent);
        }
      }

      var data = new RecvItemCompletedRes
      {
        StockNo = req.WmsNo,
        RtNo = req.RtNo,
        IsGoQueryPage = true,
        ApiFailureMsgList = apiInfoList,
      };
      _wmsTransaction.Complete();
      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = data };
      #endregion
    }

    /// <summary>
    /// 刪除驗收紀錄
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult DeleteRecvItemData(DelectRecvItemDataReq req, string gupCode)
    {
      #region 資料檢核

      // 帳號檢核
      var accData = !_p81Service.CheckAcc(req.AccNo).Any();

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
          string.IsNullOrWhiteSpace(req.WmsNo) ||
          string.IsNullOrWhiteSpace(req.WmsSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      //人員輸入的資料都改成大寫
      req.WmsNo = req.WmsNo?.ToUpper();
      req.WmsSeq = req.WmsSeq?.ToUpper();
      req.RtNo = req.RtNo?.ToUpper();

      #endregion 資料檢核

      #region 資料鎖定
      var lockStockNo = WarehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        StockNo = req.WmsNo,
        IsChangeUser = false,
        DeviceTool = "1"
      });
      if (!lockStockNo.IsSuccessed)
        return lockStockNo;

      #endregion

      #region 資料處理

      var deleteResult = WarehouseInRecvService.DeleteAcceptanceData(new DeleteAcceptanceDataParam
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        PurchaseNo = req.WmsNo,
        PurchaseSeq = req.WmsSeq,
        RTNo = req.RtNo,
      });
      if (!deleteResult.IsSuccessed)
        return new ApiResult { IsSuccessed = false, MsgCode = deleteResult.MsgCode, MsgContent = deleteResult.MsgContent };

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };
      #endregion
    }

  }
}



