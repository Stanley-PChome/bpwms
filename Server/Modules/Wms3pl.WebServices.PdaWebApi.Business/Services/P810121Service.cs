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
	public class P810121Service
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

		#endregion

		#region Repository

		#region F020501Repository
		private F020501Repository _F020501Repo;
		public F020501Repository F020501Repo
		{
			get { return _F020501Repo == null ? _F020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction) : _F020501Repo; }
			set { _F020501Repo = value; }
		}
		#endregion

		#endregion Repository


		public P810121Service(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
			_p81Service = new P81Service();
		}

		#region 進貨容器待關箱資料查詢
		/// <summary>
		/// 進貨容器待關箱資料查詢
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult RecvNotCloseBindContainerQuery(RecvNotCloseBindContainerQueryReq req, string gupCode)
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

			req.ContainerCode = req.ContainerCode?.ToUpper();

			#endregion 資料檢核

			#region 資料處理
			var result = F020501Repo.GetNotCloseContainerDatas(req.DcNo, gupCode, req.CustNo, req.ContainerCode).ToList();
			if (!string.IsNullOrWhiteSpace(req.ContainerCode) && result.Any())
			{
				var f020501 = F020501Repo.GetDataById(result.First().F020501_ID);
				if (f020501.STATUS != "0")
				{
					var statusName = CommonService.GetF000904("F020501", "STATUS", f020501.STATUS).NAME;
					return new ApiResult { IsSuccessed = false, MsgCode = "23001", MsgContent = string.Format(_p81Service.GetMsg("23001"), statusName) };
				}
			}

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = result };
			#endregion
		}
		#endregion

		#region 進貨容器關箱確認
		/// <summary>
		/// 進貨容器關箱確認
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult RecvCloseBoxConfirm(RecvCloseBoxConfirmReq req, string gupCode)
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
				req.F020501_ID <= 0 ||
				accData ||
				accFunctionCount == 0 ||
				accCustCount == 0 ||
				accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			#endregion 資料檢核

			#region 資料處理
			var f020501 = F020501Repo.Find(x => x.ID == req.F020501_ID);
			try
			{
				WarehouseInRecvBindBoxService.WarehouseInService = WarehouseInService;
				var closeBoxRes = WarehouseInRecvBindBoxService.ContainerCloseBox(req.F020501_ID, null, null, false);
				if (!closeBoxRes.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "22023", MsgContent = closeBoxRes.Message };

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
						return new ApiResult { IsSuccessed = false, MsgCode = "22024", MsgContent = res.Message };
				}
				_wmsTransaction.Complete();
			}
			catch
			{ throw; }
			finally
			{
				WarehouseInService.UnlockContainerProcess(new List<string> { f020501.CONTAINER_CODE });
			}

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };
			#endregion
		}
		#endregion
	}
}