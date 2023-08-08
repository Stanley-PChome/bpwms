using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810117Service
	{
		private WmsTransaction _wmsTransation;
		private P81Service _p81Service;
		public P810117Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
			_p81Service = new P81Service();
		}

		/// <summary>
		/// 紙箱補貨-接受任務
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult CartonReplenishAccept(CartonReplenishAcceptReq req)
		{
			var commonService = new CommonService();
			P81Service p81Service = new P81Service();
			var f056002Repo = new F056002Repository(Schemas.CoreSchema);

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
							req.ID == 0 ||
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			// 人員輸入的資料都改成大寫
			req.FuncNo = req.FuncNo.ToUpper();
			req.AccNo = req.AccNo.ToUpper();
			req.DcNo = req.DcNo.ToUpper();
			req.CustNo = req.CustNo.ToUpper();
			#endregion

			#region 資料處理
			// 取得業主編號
			ApiResult result = new ApiResult();
			var data = new CartonReplenishAcceptRes { ID = req.ID };
			string gupCode = p81Service.GetGupCode(req.CustNo);

			var f056002Res = f056002Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				() =>
				{
					var lockF0009 = f056002Repo.LockF056002();
					var updF056002 = f056002Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.ID == req.ID).FirstOrDefault();
					if (updF056002 == null)
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "21702", MsgContent = string.Format(p81Service.GetMsg("21702")), Data = data };
						return updF056002;
					}
					if (updF056002.STATUS == "1")
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "21701", MsgContent = string.Format(p81Service.GetMsg("21701"), updF056002.REPLENISH_NAME), Data = data };
						return updF056002;
					}
					updF056002.STATUS = "1";
					updF056002.REPLENISH_STAFF = Current.DefaultStaff;
					updF056002.REPLENISH_NAME = Current.DefaultStaffName;
					updF056002.REPLENISH_STARTTIME = DateTime.Now;
					f056002Repo.Update(updF056002);

					data.Info = p81Service.GetMsg("10022");
					result = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
					return updF056002;
				});

			return result;
			#endregion
		}

		/// <summary>
		/// 紙箱補貨-放棄任務
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult CartonReplenishReject(CartonReplenishRejectReq req)
		{
			var commonService = new CommonService();
			P81Service p81Service = new P81Service();
 			var f056002Repo = new F056002Repository(Schemas.CoreSchema);

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
							req.ID == 0 ||
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			// 人員輸入的資料都改成大寫
			req.FuncNo = req.FuncNo.ToUpper();
			req.AccNo = req.AccNo.ToUpper();
			req.DcNo = req.DcNo.ToUpper();
			req.CustNo = req.CustNo.ToUpper();
			#endregion

			#region 資料處理
			// 取得業主編號
			string gupCode = p81Service.GetGupCode(req.CustNo);
			f056002Repo.UpdateF056002(req.ID);
			var data = new CartonReplenishRejectRes {
				ID = req.ID,
				Info = _p81Service.GetMsg("10023")
		};
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
			#endregion
		}

		/// <summary>
		/// 紙箱補貨-查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult GetCartonReplenish(GetCartonReplenishReq req)
		{
			var commonService = new CommonService();
			P81Service p81Service = new P81Service();
			var f056002Repo = new F056002Repository(Schemas.CoreSchema);
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
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			// 人員輸入的資料都改成大寫
			req.FuncNo = req.FuncNo.ToUpper();
			req.AccNo = req.AccNo.ToUpper();
			req.DcNo = req.DcNo.ToUpper();
			req.CustNo = req.CustNo.ToUpper();
			req.Floor = req.Floor?.ToUpper();
			req.WorkStationCode = req.WorkStationCode?.ToUpper();
			req.BoxCode = req.BoxCode?.ToUpper();
			#endregion

			#region 資料處理
			// 取得業主編號
			string gupCode = p81Service.GetGupCode(req.CustNo);
			// 取得需要紙箱補貨資料
			var data = f056002Repo.GetCartonReplenish(req.DcNo, gupCode, req.CustNo, req.AccNo,req.Floor,req.WorkStationCode,req.BoxCode);
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
			#endregion
		}

    #region 紙箱補貨-完成補貨
    /// <summary>
    /// 紙箱補貨-完成補貨
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult CartonReplenishFinish(CartonReplenishFinishReq req, string gupCode)
    {
      var f056001Repo = new F056001Repository(Schemas.CoreSchema, _wmsTransation);
      var f056002Repo = new F056002Repository(Schemas.CoreSchema, _wmsTransation);
      var data = new CartonReplenishFinishRes { ID = req.ID };

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
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0 ||
              req.ID == 0 ||
              req.Qty == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
      #endregion 資料檢核

      #region 更新f056002
      var f056002 = f056002Repo.AsForUpdate().GetF056002ById(req.ID).SingleOrDefault();
      if (f056002 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = string.Format(_p81Service.GetMsg("20069")), Data = data };
      f056002.STATUS = "2";
      f056002.QTY = req.Qty;
      f056002.REPLENISH_FINISHTIME = DateTime.Now;
      f056002Repo.Update(f056002);
      #endregion 更新f056002

      #region 更新F056001
      var f056001 = f056001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f056002.DC_CODE && x.GUP_CODE == f056002.GUP_CODE && x.CUST_CODE == f056002.CUST_CODE && x.WORKSTATION_CODE == f056002.WORKSTATION_CODE && x.BOX_CODE == f056002.BOX_CODE).SingleOrDefault();
      if (f056001 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = string.Format(_p81Service.GetMsg("20069")), Data = data };
      f056001.QTY += req.Qty;
      f056001Repo.Update(f056001);
			#endregion 更新F056001

			_wmsTransation.Complete();

			data.Info = _p81Service.GetMsg("10024");
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005"), Data = data };
    }

    #endregion 紙箱補貨-完成補貨
  }
}
