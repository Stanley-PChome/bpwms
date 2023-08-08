using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  /// <summary>
  /// 進貨容器綁定
  /// </summary>
  public class P810120Controller : ApiController
  {
    /// <summary>
    /// 進貨容器綁定-已驗收需進行容器綁定資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvNeedBindContainerQuery")]
    public ApiResult RecvNeedBindContainerQuery(RecvNeedBindContainerQueryReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvNeedBindContainerQuery", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvNeedBindContainerQuery(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-已驗收需進行容器綁定資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerCheckAndQuery")]
    public ApiResult RecvBindContainerCheckAndQuery(RecvBindContainerCheckAndQueryReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerCheckAndQuery", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerCheckAndQuery(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單各區綁定容器資料檢核與查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerByAreaCheckAndQuery")]
    public ApiResult RecvBindContainerByAreaCheckAndQuery(RecvBindContainerByAreaCheckAndQueryReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerByAreaCheckAndQuery", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerByAreaCheckAndQuery(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單各區綁定容器放入確認
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerByAreaPutConfirm")]
    public ApiResult RecvBindContainerByAreaPutConfirm(RecvBindContainerByAreaPutConfirmReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerByAreaPutConfirm", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerByAreaPutConfirm(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單各區綁定容器放入確認
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvRemoveBindContainerByArea")]
    public ApiResult RecvRemoveBindContainerByArea(RecvRemoveBindContainerByAreaReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvRemoveBindContainerByArea", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvRemoveBindContainerByArea(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單綁定完成
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerFinished")]
    public ApiResult RecvBindContainerFinished(RecvBindContainerFinishedReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerFinished", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerFinished(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單待關箱資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerWaitClosedQuery")]
    public ApiResult RecvBindContainerWaitClosedQuery(RecvBindContainerWaitClosedQueryReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerWaitClosedQuery", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerWaitClosedQuery(req, gupCode);
      });
    }

    /// <summary>
    /// 進貨容器綁定-驗收單待關箱資料查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/RecvBindContainerCloseBoxConfirm")]
    public ApiResult RecvBindContainerCloseBoxConfirm(RecvBindContainerCloseBoxConfirmReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvBindContainerCloseBoxConfirm", req, () =>
      {
        var service = new P810120Service(new WmsTransaction());
        return service.RecvBindContainerCloseBoxConfirm(req, gupCode);
      });
    }

  }
}
