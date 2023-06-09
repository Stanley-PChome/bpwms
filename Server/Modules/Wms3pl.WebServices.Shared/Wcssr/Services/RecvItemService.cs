using System;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Wcssr.Services
{
    public class RecvItemService : WcssrBaseService
    {
        private WmsTransaction _wmsTransaction;
        private bool isSaveWmsData { get; set; }
        public RecvItemService(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
            var outputJsonInLog = new CommonService().GetSysGlobalValue("OutputJsonInLog");
            isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";
        }

        /// <summary>
        /// 收單驗貨上架
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RecvItemNotify(string dcCode, string gupCode, string custCode, RecvItemNotifyReq req)
        {
            string url = "v1/cctv/takingExamining";

            // Insert log to F009007
            return ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSRAPI_F009007, dcCode, gupCode, custCode, "Wcssr_RecvOrCheckOrUpStock", new { WcssrApiUrl = $"{WcssrSetting.ApiUrl}{url}" , WcssrApiAuthToken = WcssrSetting.ApiAuthToken, WmsCondition = new { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, Req = req }, WcssrData = isSaveWmsData ? req : null }, () =>
            {
                ApiResult result = new ApiResult();

                RecvItemNotifyRes wcssrApiRes;
				var msgContent = "";
#if (DEBUG || TEST)
				wcssrApiRes = (RecvItemNotifyRes)WcssrApiFuncTest(req, WcssrApiFun.TakingExamining);
#else
        // 呼叫底層共用服務 WcssrApiFunc
				wcssrApiRes = WcssrApiFunc<RecvItemNotifyReq, RecvItemNotifyRes>(req, url);
				
#endif
							// 檢查api連線是否成功checkres=呼叫底層共用服務 CheckConn
							result = CheckConn();
							if (result.IsSuccessed && wcssrApiRes != null)
							{
								//合併ErrorMsg
								if (wcssrApiRes.ErrorData != null)
								{
									var errorMsg = "";
									var errorColumn = "";
									wcssrApiRes.ErrorData.ForEach(x =>
									{
										errorMsg += string.IsNullOrWhiteSpace(errorMsg) ? x.ErrorMsg : $"，{x.ErrorMsg}";
										if (x.ErrorColumn != null)
										{
											x.ErrorColumn = x.ErrorColumn.Where(y => !string.IsNullOrWhiteSpace(y)).ToList(); // 排除List<string>內有null或空白
											errorColumn += string.IsNullOrWhiteSpace(errorColumn) ? String.Join(",", x.ErrorColumn) : $",{ String.Join(",", x.ErrorColumn)}";
										}
									});
									if(!string.IsNullOrEmpty(errorMsg) || !string.IsNullOrEmpty(errorColumn))
										msgContent = $"[WCSSR 收單驗貨上架]:{errorMsg} {errorColumn}";
								}
								result.IsSuccessed = wcssrApiRes.Code == 201;
								result.MsgCode = wcssrApiRes.Code.ToString();
								result.MsgContent = msgContent;
								result.Data = wcssrApiRes;
								return result;
							}
							return result;
            }, false);
        }
    }
}
