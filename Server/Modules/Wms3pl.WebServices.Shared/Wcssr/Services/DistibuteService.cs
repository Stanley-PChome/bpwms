using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Wcssr.Services
{
    public class DistibuteService : WcssrBaseService
    {
        private WmsTransaction _wmsTransaction;
        private bool isSaveWmsData { get; set; }
        public DistibuteService(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
            var outputJsonInLog = new CommonService().GetSysGlobalValue("OutputJsonInLog");
            isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";
        }

        /// <summary>
        /// 配箱資訊同步
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult DistibuteInfoAsync(string dcCode, string gupCode, string custCode, DistibuteInfoAsyncReq req)
        {
            string url = "v1/cctv/distributing";

            // Insert log to F009007
            return ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSRAPI_F009007, dcCode, gupCode, custCode, "Wcssr_DistibuteInfoAsync", new { WcssrApiUrl = $"{WcssrSetting.ApiUrl}{url}" , WcssrApiAuthToken = WcssrSetting.ApiAuthToken, WmsCondition = new { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, Req = req }, WcssrData = isSaveWmsData ? req : null }, () =>
            {
                ApiResult result = new ApiResult();

                DistibuteInfoAsyncRes wcssrApiRes;
				var msgContent = "";
#if (DEBUG || TEST)
				wcssrApiRes = (DistibuteInfoAsyncRes)WcssrApiFuncTest(req, WcssrApiFun.Distributing);
				//合併ErrorMsg
				if (wcssrApiRes.ErrorData != null)
				{
					wcssrApiRes.ErrorData.ForEach(x => msgContent += string.IsNullOrWhiteSpace(msgContent) ? x.ErrorMsg : $",{x.ErrorMsg}");
				}
				result.IsSuccessed = wcssrApiRes.Code == 201;
				result.MsgCode = wcssrApiRes.Code.ToString();
				result.MsgContent = msgContent;
				result.Data = wcssrApiRes;
#else
				// 呼叫底層共用服務 WcssrApiFunc
				wcssrApiRes = WcssrApiFunc<DistibuteInfoAsyncReq, DistibuteInfoAsyncRes>(req, url);

				// 檢查api連線是否成功checkres=呼叫底層共用服務 CheckConn
				result = CheckConn();

				if (result.IsSuccessed && wcssrApiRes != null)
				{
					//合併ErrorMsg
					if(wcssrApiRes?.ErrorData != null)
					{
						wcssrApiRes.ErrorData.ForEach(x => msgContent += string.IsNullOrWhiteSpace(msgContent) ? x.ErrorMsg : $",{x.ErrorMsg}");
					}
				    result.IsSuccessed = wcssrApiRes.Code == 201;
				    result.MsgCode = wcssrApiRes.Code.ToString();
				    result.MsgContent = msgContent;
				    result.Data = wcssrApiRes;
				}
#endif

				return result;
            }, false);
        }

        /// <summary>
        /// 封箱資訊同步
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult SealingInfoAsync(string dcCode, string gupCode, string custCode, SealingInfoAsyncReq req)
        {
            string url = "v1/cctv/sealing";

            // Insert log to F009007
            return ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSRAPI_F009007, dcCode, gupCode, custCode, "Wcssr_SealingInfoAsync", new { WcssrApiUrl = $"{WcssrSetting.ApiUrl}{url}", WcssrApiAuthToken = WcssrSetting.ApiAuthToken, WmsCondition = new { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, Req = req }, WcssrData = isSaveWmsData ? req : null }, () =>
            {
                ApiResult result = new ApiResult();

                SealingInfoAsyncRes wcssrApiRes;
				var msgContent = "";
#if (DEBUG || TEST)
				wcssrApiRes = (SealingInfoAsyncRes)WcssrApiFuncTest(req, WcssrApiFun.Sealing);

				//合併ErrorMsg
				if (wcssrApiRes.ErrorData != null)
				{
					wcssrApiRes.ErrorData.ForEach(x => msgContent += string.IsNullOrWhiteSpace(msgContent) ? x.ErrorMsg : $",{x.ErrorMsg}");
				}

				result.IsSuccessed = wcssrApiRes.Code == 201;
				result.MsgCode = wcssrApiRes.Code.ToString();
				result.MsgContent = msgContent;
				result.Data = wcssrApiRes;
#else
                // 呼叫底層共用服務 WcssrApiFunc
                wcssrApiRes = WcssrApiFunc<SealingInfoAsyncReq, SealingInfoAsyncRes>(req, url);

                // 檢查api連線是否成功checkres=呼叫底層共用服務 CheckConn
                result = CheckConn();

                if (result.IsSuccessed && wcssrApiRes != null)
                {
					//合併ErrorMsg
					if (wcssrApiRes?.ErrorData != null)
					{
						wcssrApiRes.ErrorData.ForEach(x => msgContent += string.IsNullOrWhiteSpace(msgContent) ? x.ErrorMsg : $",{x.ErrorMsg}");
					}
                    result.IsSuccessed = wcssrApiRes.Code == 201;
                    result.MsgCode = wcssrApiRes.Code.ToString();
                    result.MsgContent = msgContent;
                    result.Data = wcssrApiRes;
                }
#endif
				return result;
            }, false);
        }
    }
}
