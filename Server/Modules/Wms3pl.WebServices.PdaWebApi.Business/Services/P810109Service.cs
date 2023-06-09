using System.Linq;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810109Service
	{
		private WmsTransaction _wmsTransation;
		public P810109Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 序號查詢
		/// </summary>
		/// <param name="getSerialReq"></param>
		/// <returns></returns>
		public ApiResult GetSerial(GetSerialReq getSerialReq, string gupCode)
		{
			var p81Service = new P81Service();

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(getSerialReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(getSerialReq.FuncNo, getSerialReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(getSerialReq.CustNo, getSerialReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(getSerialReq.DcNo, getSerialReq.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(getSerialReq.FuncNo) ||
					string.IsNullOrWhiteSpace(getSerialReq.AccNo) ||
					string.IsNullOrWhiteSpace(getSerialReq.DcNo) ||
					string.IsNullOrWhiteSpace(getSerialReq.CustNo) ||
					!accData.Any() ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 品號、序號擇一必填
			if (string.IsNullOrWhiteSpace(getSerialReq.ItemNo) && string.IsNullOrWhiteSpace(getSerialReq.Sn))
				return new ApiResult { IsSuccessed = false, MsgCode = "20851", MsgContent = p81Service.GetMsg("20851") };

			#endregion

			#region 資料處理
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
            return new ApiResult
            {
                IsSuccessed = true,
                MsgCode = "10001",
                MsgContent = p81Service.GetMsg("10001"),
                Data = f2501Repo.GetP810109Data(
                    gupCode,
                    getSerialReq.CustNo,
                    getSerialReq.ItemNo,
                    getSerialReq.Sn).ToList()
            };
			#endregion
		}
	}
}
