using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class ExportHomeDeliveryService: LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;

		public ExportHomeDeliveryService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ExportHomeDeliveryResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult() { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			TransApiBaseService tacService = new TransApiBaseService();
			var f055005Repo = new F055005Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0003epo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var outputJsonInLog = _commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			#region 取得宅單過刷紀錄表
			var f055005s = f055005Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROC_FLAG == "0");
			//var homeDeliveryLog = f055005Repo.GetDatasByHomeDelivery(dcCode, gupCode, custCode);
			#endregion

			if (f055005s.Any())
			{
				// 取得最大筆數
				var homeDeliveryLogMax = Convert.ToInt32(_commonService.GetSysGlobalValue(dcCode, gupCode, custCode, "homeDeliveryLogMax"));
				
				// 取得執行次數
				int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(f055005s.Count()) / homeDeliveryLogMax));


				for (int i = 0; i < index; i++)
				{
					var currDatas = f055005s.Skip(i * homeDeliveryLogMax).Take(homeDeliveryLogMax).ToList();
					var homeDeliveryResults = currDatas.Select(x => new HomeDeliveryNo
					{
						WmsNo = x.WMS_NO,
						TransportCode = x.PAST_NO,
						ShipmentTime = x.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
					}).ToList();

					var currReq = new HomeDeliveryResultsReq { DcCode = dcCode, CustCode = custCode,Data = homeDeliveryResults };

					ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ExportHomeDeliveryResults", new { LmsApiUrl = LmsSetting.ApiUrl + "/Shipping/Reply", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode }, LmsData = isSaveWmsData ? currReq : null }, () =>
					{

						ApiResult result = new ApiResult() { IsSuccessed = true };
#if (DEBUG)
						result = LmsApiFuncTest(currReq, "ShipOrder/Shipping/Reply");
#else
            result = LmsApiFunc(currReq, "ShipOrder/Shipping/Reply");
#endif
						if (result.IsSuccessed)
						{
							#region 更新處理狀態
							foreach (var currData in currDatas)
							{
								currData.PROC_FLAG = "1";
								currData.TRANS_DATE = DateTime.Now;
								currData.UPD_DATE = DateTime.Now;
							}
							f055005Repo.BulkUpdate(currDatas);
							#endregion
						}
						else
						{
							var FailureDetail = ((List<ApiResponse>)result.Data);
							var wmsNos = FailureDetail.Select(x => x.WmsNo);
							var isSuccessData = currDatas.Where(x => !wmsNos.Contains(x.WMS_NO));
							foreach (var successData in isSuccessData)
							{
								successData.PROC_FLAG = "1";
								successData.TRANS_DATE = DateTime.Now;
								successData.UPD_DATE = DateTime.Now;
							}
							f055005Repo.BulkUpdate(currDatas);
							res.FailureCnt += result.FailureCnt;
							data.AddRange(FailureDetail.Select(x=>new ApiResponse {
								MsgCode = x.MsgCode,
								MsgContent = x.MsgContent,
								WmsNo = x.WmsNo,
							}));
						}
						_wmsTransaction.Complete();
						
						return result;
					}, false);
				}
			}

			if (data.Any())
			{
				res.Data = data;
			}

			res.TotalCnt = f055005s.Count();
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"單箱出貨扣帳回檔", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}
	}
}
