using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
    public class WcsCollectionServices : WcsBaseService
    {
        /// <summary>
        /// 集貨等待通知
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult CollectionServices(WcsExportReq req)
        {
            var res = new ApiResult { IsSuccessed = true };
            var data = new List<ApiResponse>();

            WcsSetting.DcCode = req.DcCode;
            // 新增API Log
            res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSH_F009005, req.DcCode, req.GupCode, req.CustCode, "ExportCollectionStatus", req, () =>
            {
                // 取得物流中心服務貨主檔
                CommonService commonService = new CommonService();
                var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
                dcCustList.ForEach(item =>
                {
                    var result = ExportCollectionStatus(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
                    data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
                });
                res.Data = JsonConvert.SerializeObject(data);
                return res;
            }, true);

            return res;
        }

    private ApiResult ExportCollectionStatus(string dcCode, string gupCode, string custCode)
    {
      var result = new ApiResult { IsSuccessed = true };
      var f060702Repo = new F060702Repository(Schemas.CoreSchema);
      string url = $"v1/{dcCode}/ALL/Collection/Status";
      var successCnt = 0;

      var SettingsResendCount = GetMidApiRelmt();
      var f060702Datas = f060702Repo.getWCSCollectionData(dcCode, gupCode, custCode, SettingsResendCount).ToList();

      f060702Datas.ForEach(f060702 =>
      {
        f060702.RESENT_CNT++;
        var req = new WcsCollectionReq
        {
          OwnerCode = f060702.CUST_CODE,
          OrderCode = f060702.ORDER_CODE,
          OriOrderCode = f060702.ORI_ORDER_CODE,
          Status = f060702.STATUS
        };

        WcsSetting.DcCode = dcCode;
        ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009003, dcCode, gupCode, custCode, "WcsCollectionStatus", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null }, () =>
          {
#if (DEBUG)
            result = WcsApiFuncTest(req, "Collection");
#else
			      result = WcsApiFunc(req, url);
#endif
            if (result.IsSuccessed)
            {
              f060702.PROC_FLAG = "2";
              f060702.MESSAGE = result.MsgContent;
              successCnt++;
            }
            f060702.PROC_DATE = DateTime.Now;
            return result;

          }, false,
          (fResult) =>
          {
            if (!fResult.IsSuccessed)
            {
              //重試次數超過設定值後才改失敗
              if (f060702.RESENT_CNT >= SettingsResendCount)
                f060702.PROC_FLAG = "F";//錯誤將狀態改為F
              else
                f060702.PROC_FLAG = "T";
            }
            f060702.MESSAGE = fResult.MsgContent;

            return new ApiResult();
          });

        #region 更新 F060702 完成、錯誤、逾時狀態
        f060702Repo.Update(f060702);
        #endregion F060702 完成、錯誤、逾時狀態
      });

      return new ApiResult
      {
        IsSuccessed = true,
        MsgCode = "10005",
        MsgContent = string.Format(_tacService.GetMsg("10005"), "集貨等待通知", successCnt, f060702Datas.Count - successCnt, f060702Datas.Count),
        TotalCnt = f060702Datas.Count,
        SuccessCnt = successCnt,
        FailureCnt = f060702Datas.Count - successCnt
      };
    }


  }
}
