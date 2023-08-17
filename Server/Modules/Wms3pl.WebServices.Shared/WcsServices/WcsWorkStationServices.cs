using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.Shared.WcsServices
{
  public class WcsWorkStationServices : WcsBaseService
  {
    /// <summary>
    /// 工作站狀態同步
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult Station(WcsStationReq req, string dcCode)
    {
      var res = new ApiResult { IsSuccessed = true };
      var data = new List<ApiResponse>();
      var f055006Repo = new F055006Repository(Schemas.CoreSchema);

      WcsSetting.DcCode = dcCode;
      string url = $"v1/{dcCode}/ALL/Workstation/Status";
      var apiResult = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009004, dcCode, "0", req.OwnerCode, "WcsStationResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null }, () =>
       {
#if (DEBUG)
         res = WcsApiFuncTest(req, "Station");
#else
         res = WcsApiFunc(req, url);
#endif
         if (res.IsSuccessed)
         {
           f055006Repo.Add(new F055006
           {
             DC_CODE = dcCode,
             CUST_CODE = req.OwnerCode,
             STATION_CODE = req.StationList.FirstOrDefault().StationCode,
             STATION_TYPE = req.StationList.FirstOrDefault().StationType,
             STATUS = req.StationList.FirstOrDefault().Status
           });
         }

         return res;
       }, false);

      return apiResult;
    }
  }
}
