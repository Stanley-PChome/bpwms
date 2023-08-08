using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class ExportSysErrorNotifyService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;

		public ExportSysErrorNotifyService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

        /// <summary>
        /// LMS系統異常通知
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
		public ApiResult ExportSysErrorNotifyResults(string dcCode, string gupCode, string custCode)
    {
      var res = new ApiResult() { IsSuccessed = true };
      var tacService = new TransApiBaseService();
      var f0093Repo = new F0093Repository(Schemas.CoreSchema);
      var commonService = new CommonService();
      var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
      var isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";
      
      //取得重試次數
      var midApiReLmt = Convert.ToInt32(commonService.GetSysGlobalValue(dcCode, gupCode, custCode, "MIDAPIRELMT"));

      // 取得庫存跨倉移動紀錄表
      var f0093s = f0093Repo.GetDatasBySchedule(dcCode, gupCode, custCode, midApiReLmt);

      if (f0093s.Any())
      {
        // 取得最大筆數
        var exportSysErrorNotifyMax = Convert.ToInt32(commonService.GetSysGlobalValue(dcCode, gupCode, custCode, "ExportSysErNotifyMax"));

        // 取得執行次數
        int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(f0093s.Count()) / exportSysErrorNotifyMax));

        for (int i = 0; i < index; i++)
        {
          var currDatas = f0093s.Skip(i * exportSysErrorNotifyMax).Take(exportSysErrorNotifyMax).ToList();

          currDatas.ForEach(f0093 =>
          {
            #region 更新處理中狀態
            f0093.RESENT_CNT++;
            f0093.STATUS = "1"; // 處理中
            f0093Repo.Update(f0093);
            #endregion

            #region 組呼叫LMS系統異常通知參數
            var currReq = new SysErrorNotifyResultsReq
            {
              DcCode = dcCode,
              CustCode = custCode,
              ErrorCode = f0093.MSG_NO.Replace("API", string.Empty),
              ErrorMsg = f0093.MSG_CONTENT,
              OrderNo = f0093.WMS_NO,
              OrderType = f0093.WMS_TYPE,
              ItemCode = f0093.ITEM_CODE
            };
            #endregion

            #region 呼叫LMS系統異常通知
            ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ExportSysErrorNotifyResults", new { LmsApiUrl = LmsSetting.ApiUrl + "SystemException/Notify", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = f0093, LmsData = isSaveWmsData ? currReq : null }, () =>
            {
              var result = new ApiResult() { IsSuccessed = true };

#if (DEBUG || TEST)
  result = LmsApiRtnMultiFuncTest(currReq, "SystemException/Notify", new List<dynamic>());
#else
result = LmsApiRtnMultiFunc<dynamic, object, ErrorData>(currReq, "SystemException/Notify");
#endif
              
              if (result.IsSuccessed)
              {
                f0093.STATUS = "2"; // 完成
                res.SuccessCnt++;
              }
              else
                // 99999 代表TimeOut
                f0093.STATUS = result.MsgCode == "99999" ? "T" : "F";
              return result;
            }, false,
            (fResult) =>
            {
              if (!fResult.IsSuccessed)
              {
                //重試次數超過設定值後才改失敗
                if (f0093.RESENT_CNT >= midApiReLmt)
                  f0093.STATUS = "F";//錯誤將狀態改為F
                else
                  f0093.STATUS = "T";
              }
              return new ApiResult();
            });
            f0093Repo.Update(f0093);
            #endregion

          });
        }
      }

      res.TotalCnt = f0093s.Count();
      res.FailureCnt = res.TotalCnt - res.SuccessCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(tacService.GetMsg("10005"),
                    "LMS系統異常通知", res.SuccessCnt, res.FailureCnt, res.TotalCnt);

      return res;
    }
	}
}
