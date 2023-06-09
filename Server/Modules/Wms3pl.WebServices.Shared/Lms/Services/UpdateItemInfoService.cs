using System;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
  public class UpdateItemInfoService : LmsBaseService
  {
    /// <summary>
    /// 更新上層系統商品主檔
    /// </summary>
    /// <param name="gupCode">貨主編號</param>
    /// <param name="custCode">客戶品號</param>
    /// <param name="custItemCode"></param>
    /// <param name="BundleSerialno">序號商品 (0: 否 1: 是)</param>
    /// <param name="ItemBarCode">貼標用的條碼</param>
    /// <returns></returns>
    public ExecuteResult UpdateItemInfo(string custCode, string custItemCode, string BundleSerialno, string ItemBarCode)
    {
      var Url = "Items/Reply";

      var req = new UpdateItemInfoReq
      {
        CustCode = custCode,
        CustItemCode = custItemCode,
        BundleSerialno = BundleSerialno,
        ItemBarCode = ItemBarCode
      };

      var apiRes = ApiLogHelper.CreateApiLogInfo(null, null, custCode, "LMS_UpdateItemInfo", new { LmsApiUrl = LmsSetting.ApiUrl + Url, LmsToken = LmsSetting.ApiAuthToken, LmsData = req },
        () =>
        {
          var apiResult = new ApiResult();
#if (DEBUG)
          apiResult = LmsApiFuncTest(req, Url);
#else
          apiResult = LmsApiFunc(req, Url);
#endif
          return apiResult;
        });

      return new ExecuteResult(apiRes.IsSuccessed, $"[LMS更新商品主檔]{apiRes.MsgCode} {apiRes.MsgContent}");

    }

  }
}
