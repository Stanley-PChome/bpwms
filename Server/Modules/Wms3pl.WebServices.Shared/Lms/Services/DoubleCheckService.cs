using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
    public class DoubleCheckService : LmsBaseService
    {
        private WmsTransaction _wmsTransaction;
        public DoubleCheckService(WmsTransaction wmsTransation = null)
        {
            _wmsTransaction = wmsTransation;
        }

        /// <summary>
        /// 複驗比例確認
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult DoubleCheckConfirm(string gupCode, DoubleCheckConfirmReq req)
        {
            #region 新增API Log
            var res = ApiLogHelper.CreateApiLogInfo(req.DcCode, gupCode, req.CustCode, "LmsApi_DoubleCheckConfirm", new { LmsApiUrl = LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "") + "wmsext-panel/api/DoubleCheck/Confirm", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = req.DcCode, GUP_CODE = gupCode, CUST_CODE = req.CustCode, CUST_IN_NO = req.CustInNo }, WmsData = isSaveWmsData ? req : null }, () =>
            {
                ApiResult result;
#if (DEBUG || TEST)
                result = LmsApiRtnMultiFuncTest(req, "wmsext-panel/api/DoubleCheck/Confirm", new List<dynamic>());
#else
				result = LmsApiRtnMultiFunc<dynamic, DoubleCheckConfirmData, ErrorData>(req, "wmsext-panel/api/DoubleCheck/Confirm");
#endif
                return result;
            }, false);
            #endregion

            return res;
        }
    }
}
