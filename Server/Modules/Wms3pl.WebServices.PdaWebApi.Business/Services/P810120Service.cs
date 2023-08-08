using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  public class P810120Service
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

    #region ContainerService
    private ContainerService _containerService;
    public ContainerService ContainerService
    {
      get { return _containerService == null ? _containerService = new ContainerService(_wmsTransaction) : _containerService; }
      set { _containerService = value; }
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

    #region WarehouseInRecvBindBoxService
    private WarehouseInRecvBindBoxService _warehouseInRecvBindBoxService;
    public WarehouseInRecvBindBoxService WarehouseInRecvBindBoxService
    {
      get { return _warehouseInRecvBindBoxService == null ? _warehouseInRecvBindBoxService = new WarehouseInRecvBindBoxService(_wmsTransaction) : _warehouseInRecvBindBoxService; }
      set { _warehouseInRecvBindBoxService = value; }
    }
    #endregion

    #region WarehouseInRecvService
    private WarehouseInRecvService _WarehouseInRecvService;
    public WarehouseInRecvService WarehouseInRecvService
    {
      get { return _WarehouseInRecvService == null ? _WarehouseInRecvService = new WarehouseInRecvService(_wmsTransaction) : _WarehouseInRecvService; }
      set { _WarehouseInRecvService = value; }
    }
    #endregion


    #endregion

    #region Repository

    #region F075111Repository
    private F075111Repository _F075111Repo;
    public F075111Repository F075111Repo
    {
      get { return _F075111Repo == null ? _F075111Repo = new F075111Repository(Schemas.CoreSchema, _wmsTransaction) : _F075111Repo; }
      set { _F075111Repo = value; }
    }
    #endregion

    #region F0701Repository
    private F0701Repository _F0701Repo;
    public F0701Repository F0701Repo
    {
      get { return _F0701Repo == null ? _F0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction) : _F0701Repo; }
      set { _F0701Repo = value; }
    }
    #endregion

    #region F070101Repository
    private F070101Repository _F070101Repo;
    public F070101Repository F070101Repo
    {
      get { return _F070101Repo == null ? _F070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction) : _F070101Repo; }
      set { _F070101Repo = value; }
    }
    #endregion

    #region F070102Repository
    private F070102Repository _F070102Repo;
    public F070102Repository F070102Repo
    {
      get { return _F070102Repo == null ? _F070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction) : _F070102Repo; }
      set { _F070102Repo = value; }
    }
    #endregion

    #region F0205Repository
    private F0205Repository _F0205Repo;
    public F0205Repository F0205Repo
    {
      get { return _F0205Repo == null ? _F0205Repo = new F0205Repository(Schemas.CoreSchema, _wmsTransaction) : _F0205Repo; }
      set { _F0205Repo = value; }
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

    #region F020501Repository
    private F020501Repository _F020501Repo;
    public F020501Repository F020501Repo
    {
      get { return _F020501Repo == null ? _F020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction) : _F020501Repo; }
      set { _F020501Repo = value; }
    }
    #endregion

    #region F020502Repository
    private F020502Repository _F020502Repo;
    public F020502Repository F020502Repo
    {
      get { return _F020502Repo == null ? _F020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction) : _F020502Repo; }
      set { _F020502Repo = value; }
    }
    #endregion

    #region F020603Repository
    private F020603Repository _F020603Repo;
    public F020603Repository F020603Repo
    {
      get { return _F020603Repo == null ? _F020603Repo = new F020603Repository(Schemas.CoreSchema, _wmsTransaction) : _F020603Repo; }
      set { _F020603Repo = value; }
    }
		#endregion

		#region F151001Repository
		private F151001Repository _f151001Repo;
		public F151001Repository F151001Repo
		{
			get { return _f151001Repo == null ? _f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction) : _f151001Repo; }
			set { _f151001Repo = value; }
		}
		#endregion

		#endregion Repository



		public P810120Service(WmsTransaction wmsTransation)
    {
      _wmsTransaction = wmsTransation;
      _p81Service = new P81Service();
    }


    /// <summary>
    /// 進貨容器綁定-已驗收需進行容器綁定資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult RecvNeedBindContainerQuery(RecvNeedBindContainerQueryReq req, string gupCode)
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
      var data = F020201Repo.GetRecvNeedBindContainerQuery(req.DcNo, gupCode, req.CustNo, req.WmsNo, req.ItemNo).ToList();
      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = data };
      #endregion
    }

    /// <summary>
    /// 進貨容器綁定-驗收單綁定容器資料檢核與查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerCheckAndQuery(RecvBindContainerCheckAndQueryReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.StockNo) ||
          string.IsNullOrWhiteSpace(req.StockSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };


      #endregion 資料檢核

      #region 資料處理

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.LockBindContainerAcceptenceOrder(new LockBindContainerAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        RtNo = req.RtNo,
        IsChangeUser = req.IsChangeUser,
				DeviceTool = "1"
      });
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion

      #region 組回傳結果 From F0205
      var f0205s = F0205Repo.GetDatas(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq).ToList();
      var OKType = new[] { "A", "C" };
      var needCheck = (f0205s.FirstOrDefault(x => OKType.Contains(x.TYPE_CODE))?.NEED_DOUBLE_CHECK ?? 0) == 1;

      var result = new RecvBindContainerCheckAndQueryRes
      {
        NeedCheck = needCheck,
        NeedCheckDesc = needCheck ? "需要複驗" : "不需複驗",
        SowQty = f0205s.Sum(x => x.A_QTY ?? 0),
        GoodQty = f0205s.Where(x => OKType.Contains(x.TYPE_CODE)).Sum(x => x.B_QTY),
        NoGoodQty = f0205s.Where(x => !OKType.Contains(x.TYPE_CODE)).Sum(x => x.B_QTY),

        PickWarehouse = f0205s.FirstOrDefault(x => x.TYPE_CODE == "A")?.PICK_WARE_ID ?? "",
        PickNoSowQty = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "A")?.B_QTY ?? 0) - (f0205s.FirstOrDefault(x => x.TYPE_CODE == "A")?.A_QTY ?? 0),
        PickCanBind = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "A")?.STATUS ?? "1") == "0",
        PickAreaId = (int)(f0205s.FirstOrDefault(x => x.TYPE_CODE == "A")?.ID ?? 0),

        ReplenishWarehouse = f0205s.FirstOrDefault(x => x.TYPE_CODE == "C")?.PICK_WARE_ID ?? "",
        ReplenishNoSowQty = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "C")?.B_QTY ?? 0) - (f0205s.FirstOrDefault(x => x.TYPE_CODE == "C")?.A_QTY ?? 0),
        ReplenishCanBind = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "C")?.STATUS ?? "1") == "0",
        ReplenishAreaId = (int)(f0205s.FirstOrDefault(x => x.TYPE_CODE == "C")?.ID ?? 0),

        NoGoodWarehouse = f0205s.FirstOrDefault(x => x.TYPE_CODE == "R")?.PICK_WARE_ID ?? "",
        NoGoodNoSowQty = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "R")?.B_QTY ?? 0) - (f0205s.FirstOrDefault(x => x.TYPE_CODE == "R")?.A_QTY ?? 0),
        NoGoodCanBind = (f0205s.FirstOrDefault(x => x.TYPE_CODE == "R")?.STATUS ?? "1") == "0",
        NoGoodAreaId = (int)(f0205s.FirstOrDefault(x => x.TYPE_CODE == "R")?.ID ?? 0),
      };

      #endregion

      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = result };
      #endregion
    }

    /// <summary>
    /// 進貨容器綁定-驗收單各區綁定容器資料檢核與查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerByAreaCheckAndQuery(RecvBindContainerByAreaCheckAndQueryReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          string.IsNullOrWhiteSpace(req.TypeCode) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				RtNo = req.RtNo,
				DeviceTool = "1",
			});
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion 驗收單檢查&鎖定

      #endregion 資料檢核

      #region 資料處理

      #region 組回傳結果
      var data = F020501Repo.GetRecvBindContainerByAreaCheckAndQueryRes(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq, req.TypeCode).ToList();
      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = data };
      #endregion

      #endregion
    }

    /// <summary>
    /// 進貨容器綁定-驗收單各區綁定容器放入確認
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerByAreaPutConfirm(RecvBindContainerByAreaPutConfirmReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          string.IsNullOrWhiteSpace(req.WarehouseId) ||
          string.IsNullOrWhiteSpace(req.TypeCode) ||
          string.IsNullOrWhiteSpace(req.ContainerCode) ||
          req.AreaId <= 0 ||
          req.PutQty <= 0 ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 將人員輸入的資料都改成大寫
      req.ContainerCode = req.ContainerCode?.ToUpper();
      #endregion 將人員輸入的資料都改成大寫

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				RtNo = req.RtNo,
				DeviceTool = "1",
			});
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
			#endregion

			#endregion 資料檢核

			#region 資料處理
			var res = WarehouseInRecvBindBoxService.AddContainerBindData(new AddContainerBindDataReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				PurchaseNo = req.StockNo,
				PurchaseSeq = req.StockSeq,
				RtNo = req.RtNo,
				RtSeq = req.RtSeq,
				AreaId = req.AreaId,
				WarehouseId = req.WarehouseId,
				TypeCode = req.TypeCode,
				PutQty	= req.PutQty,
				ContainerCode = req.ContainerCode
			});
			if (!res.IsSuccessed)
				return res;
			var data = res.Data as AddContainerBindDataRes;

			var result = new RecvBindContainerByAreaCheckAndQueryRes
      {
        ContainerCode = req.ContainerCode,
        Qty = req.PutQty,
        F020501_ID = data.F020501_ID,
        F020502_ID = data.F020502_ID
      };

      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = result };
      #endregion
    }

		/// <summary>
		/// 進貨容器綁定-驗收單各區移除綁定容器
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult RecvRemoveBindContainerByArea(RecvRemoveBindContainerByAreaReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          req.AreaId <= 0 ||
          req.F020501_ID <= 0 ||
          req.F020502_ID <= 0 ||
          req.Qty <= 0 ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				RtNo = req.RtNo,
				DeviceTool = "1",
			});
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion

      #endregion 資料檢核

      #region 資料處理
      var res = WarehouseInRecvBindBoxService.DeleteContainerBindData(new DeleteContainerBindDataReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        RtNo = req.RtNo,
        RtSeq = req.RtSeq,
        AreaId = req.AreaId,
        F020501_ID = req.F020501_ID,
        F020502_ID = req.F020502_ID,
        Qty = req.Qty
      });
      if (res.IsSuccessed)
        _wmsTransaction.Complete();
      return res;
      #endregion 資料處理
    }

    /// <summary>
    /// 進貨容器綁定-驗收單綁定完成
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerFinished(RecvBindContainerFinishedReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.StockNo) ||
          string.IsNullOrWhiteSpace(req.StockSeq) ||
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				RtNo = req.RtNo,
				DeviceTool = "1",
			});
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion 驗收單檢查&鎖定

      #endregion 資料檢核

      #region 資料處理
      var lockContainers = new List<string>();
			var allotationNoList = new List<string>();
			WarehouseInRecvBindBoxService.WarehouseInService = WarehouseInService;

			try
      {
				var res = WarehouseInRecvBindBoxService.RecvBindContainerFinished(new ShareRecvBindContainerFinishedReq
				{
					DcCode = req.DcNo,
					GupCode = gupCode,
					CustCode =req.CustNo,
					PurchaseNo = req.StockNo,
					RtNo = req.RtNo,
					RtSeq =req.RtSeq,
				});
				if (!res.IsSuccessed)
					return res;
				var data = res.Data as ShareRecvBindContainerFinishedRes;
				lockContainers = data.LockContainers;
				allotationNoList = data.AllocationNoList;

				_wmsTransaction.Complete();
				var containerErrorMsg = string.Empty;

				// PDA 先不要Show調撥單異常，怕訊息視窗多內容會卡住畫面，如果客戶一定要show在取消註解
				//if (allotationNoList.Any())
				//		containerErrorMsg = string.Join(",", F151001Repo.GetUnnormalAllocDatas(req.DcNo, gupCode, req.CustNo, allotationNoList));

				//因為PDA的資料處理IsSuccessed = true會把原有的訊息覆蓋掉，所以訊息改包到Data裡
				if (req.NeedCheck)
          return new ApiResult { IsSuccessed = true, MsgCode = "22021", MsgContent = _p81Service.GetMsg("22021"), Data = _p81Service.GetMsg("22021")+ containerErrorMsg };
        else
          return new ApiResult { IsSuccessed = true, MsgCode = "22022", MsgContent = _p81Service.GetMsg("22022"), Data = _p81Service.GetMsg("22022") + containerErrorMsg };
      }
      catch //丟去給上層的ApiLogHelper處理
      { throw; }
      finally
      {
        //如果有鎖定不良品容器清單[Z]，解除不良品容器鎖定
        if (lockContainers.Any())
          WarehouseInService.UnlockContainerProcess(lockContainers);
      }
      #endregion
    }

    /// <summary>
    /// 進貨容器綁定-驗收單待關箱資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerWaitClosedQuery(RecvBindContainerWaitClosedQueryReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
			{
				DcCode = req.DcNo,
				GupCode = gupCode,
				CustCode = req.CustNo,
				RtNo = req.RtNo,
				DeviceTool = "1",
			});
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion

      #endregion 資料檢核

      #region 資料處理
      var res = F020502Repo.GetRecvBindContainerWaitClosedQueryRes(req.DcNo, gupCode, req.CustNo, req.RtNo, req.RtSeq).ToList();
      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = res };
      #endregion
    }

    /// <summary>
    /// 進貨容器綁定-驗收單容器關箱確認
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult RecvBindContainerCloseBoxConfirm(RecvBindContainerCloseBoxConfirmReq req, string gupCode)
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
          string.IsNullOrWhiteSpace(req.RtNo) ||
          string.IsNullOrWhiteSpace(req.RtSeq) ||
          accData ||
          accFunctionCount == 0 ||
          accCustCount == 0 ||
          accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      #region 驗收單檢查&鎖定
      var chkProcUser = WarehouseInRecvBindBoxService.CheckIsOtherUserProc(new LockBindContainerAcceptenceOrderReq
      {
        DcCode = req.DcNo,
        GupCode = gupCode,
        CustCode = req.CustNo,
        RtNo = req.RtNo,
        DeviceTool = "1",
      });
      if (!chkProcUser.IsSuccessed)
        return chkProcUser;
      #endregion

      #endregion 資料檢核

      #region 資料處理
      var results = new List<RecvBindContainerCloseBoxConfirmRes>();

			foreach (var item in req.ContainerList)
      {
        try
        {
					_wmsTransaction = new WmsTransaction();
					WarehouseInRecvBindBoxService.WarehouseInService = WarehouseInService;
					var closeBoxRes = WarehouseInRecvBindBoxService.ContainerCloseBox(item.F020501_ID, req.RtNo, req.RtSeq, req.NeedCheck);
					if(closeBoxRes.IsSuccessed)
					{
						if (closeBoxRes.f020501 != null && closeBoxRes.f020501.STATUS == "2")
						{
							var rtNoList = closeBoxRes.f020502s.Select(a => a.RT_NO).ToList();
							var finishedRtContainerStatusList = closeBoxRes.f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
								.Select(x => new RtNoContainerStatus
								{
									DC_CODE = x.Key.DC_CODE,
									GUP_CODE = x.Key.GUP_CODE,
									CUST_CODE = x.Key.CUST_CODE,
									STOCK_NO = x.Key.STOCK_NO,
									RT_NO = x.Key.RT_NO,
									F020501_ID = closeBoxRes.f020501.ID,
									F020501_STATUS = closeBoxRes.f020501.STATUS,
									ALLOCATION_NO = closeBoxRes.f020501.ALLOCATION_NO
								}).ToList();

							var res = WarehouseInService.AfterConatinerTargetFinishedProcess(closeBoxRes.f020501.DC_CODE, closeBoxRes.f020501.GUP_CODE, closeBoxRes.f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);

							if (!res.IsSuccessed)
							{
								results.Add(new RecvBindContainerCloseBoxConfirmRes
								{
									IsSuccessed = res.IsSuccessed,
									ContainerCode = item.CONTAINER_CODE,
									Message = res.Message
								});
								continue;
							}
						}
					}
					
					if(closeBoxRes.IsSuccessed)
						_wmsTransaction.Complete();

					var containerErrorMsg = string.Empty;
					// // PDA 先不要Show調撥單異常，怕訊息視窗多內容會卡住畫面，如果客戶一定要show在取消註解
					//if (!string.IsNullOrEmpty(closeBoxRes.No))
					//	containerErrorMsg = F151001Repo.GetUnnormalAllocDatas(req.DcNo, gupCode, req.CustNo, new List<string> { closeBoxRes.No }).FirstOrDefault();

					var successMsg = "關箱完成";
					if (closeBoxRes.f020501 != null && closeBoxRes.f020501.STATUS == "1")
						successMsg += "，請送至複驗區";

					successMsg += containerErrorMsg;
					results.Add(new RecvBindContainerCloseBoxConfirmRes
					{
						IsSuccessed = closeBoxRes.IsSuccessed,
						ContainerCode = item.CONTAINER_CODE,
						Message = closeBoxRes.IsSuccessed ? successMsg : closeBoxRes.Message
					});

				}
        catch
        { throw; }
        finally
        {
          WarehouseInService.UnlockContainerProcess(new List<string> { item.CONTAINER_CODE });
					WarehouseInRecvBindBoxService = null;
					WarehouseInService = null;
				}
      }

      if (results.Any(x => x.IsSuccessed))
        F0205Repo.UpdateFields(new { STATUS = "1" },
          x => x.DC_CODE == req.DcNo && x.GUP_CODE == gupCode && x.CUST_CODE == req.CustNo && x.RT_NO == req.RtNo && x.RT_SEQ == req.RtSeq && x.STATUS == "0");
      _wmsTransaction.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = results };
			#endregion
		}


	}
}