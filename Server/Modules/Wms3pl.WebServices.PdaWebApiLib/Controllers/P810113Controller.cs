using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  /// <summary>
  /// 商品複驗
  /// </summary>
  public class P810113Controller : ApiController
  {
    /// <summary>
    /// 商品複驗-容器查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/GetRcvData")]
    public ApiResult GetRcvData(GetRcvDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetRcvData", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.GetRcvData(req, gupCode);
      });
    }

    /// <summary>
    /// 商品複驗-複驗查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/GetRecheckData")]
    public ApiResult GetRecheckData(GetRecheckDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetRecheckData", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.GetRecheckData(req, gupCode);
      });
    }

    /// <summary>
    /// 商品複驗-商品條碼檢驗
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/ConfirmInputItemData")]
    public ApiResult ConfirmInputItemData(ConfirmInputItemDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmInputItemData", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.ConfirmInputItemData(req, gupCode);
      });
    }

    /// <summary>
    /// 商品複驗-複驗開始
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/StartToReCheck")]
    public ApiResult StartToReCheck(StartToReCheckReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "StartToReCheck", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.StartToReCheck(req, gupCode);
      });
    }

    /// <summary>
    /// 商品複驗-複驗確認
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/ConfirmRecheckData")]
    public ApiResult ConfirmRecheckData(ConfirmRecheckDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmRecheckData", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.ConfirmRecheckData(req, gupCode);
      });
    }

    /// <summary>
    /// 商品複驗-不通過原因登錄與容器移轉
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/ChangeContainerData")]
    public ApiResult ConfirmRecheckDataByChange(ChangeContainerDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ChangeContainerData", req, () =>
      {
        var service = new P810113Service(new WmsTransaction());
        return service.ChangeContainerData(req, gupCode);
      });
    }
  }
}
