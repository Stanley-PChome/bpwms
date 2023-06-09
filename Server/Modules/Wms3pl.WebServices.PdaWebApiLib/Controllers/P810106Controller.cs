using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  /// <summary>
  /// 出貨作業
  /// </summary>
  public class P810106Controller : ApiController
  {
    /// <summary>
    /// 揀貨單據查詢
    /// </summary>
    /// <param name="getAllocReq"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/GetPick")]
    public ApiResult GetPick(GetPickReq getPickReq)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(getPickReq);

      string gupCode = p81Service.GetGupCode(getPickReq.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(getPickReq.DcNo, gupCode, getPickReq.CustNo, "GetPick", getPickReq, () =>
       {
         var p810106Service = new P810106Service(new WmsTransaction());
         return p810106Service.GetPick(getPickReq, gupCode);
       });
    }

    /// <summary>
    /// 揀貨明細查詢
    /// </summary>
    /// <param name="getPickDetailReq"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/GetPickDetail")]
    public ApiResult GetPickDetail(GetPickDetailReq getPickDetailReq)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(getPickDetailReq);

      string gupCode = p81Service.GetGupCode(getPickDetailReq.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(getPickDetailReq.DcNo, gupCode, getPickDetailReq.CustNo, "GetPickDetail", getPickDetailReq, () =>
      {
        var p810106Service = new P810106Service(new WmsTransaction());
        return p810106Service.GetPickDetail(getPickDetailReq, gupCode);
      });
    }

    /// <summary>
    /// 揀貨單據檢核
    /// </summary>
    /// <param name="getPickDetailReq"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/PostPickCheck")]
    public ApiResult PostPickCheck(PostPickCheckReq postPickCheckReq)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(postPickCheckReq);

      string gupCode = p81Service.GetGupCode(postPickCheckReq.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(postPickCheckReq.DcNo, gupCode, postPickCheckReq.CustNo, "PostPickCheck", postPickCheckReq, () =>
      {
        var p810106Service = new P810106Service(new WmsTransaction());
        return p810106Service.PostPickCheck(postPickCheckReq, gupCode);
      });
    }

    /// <summary>
    /// 揀貨單據更新
    /// </summary>
    /// <param name="postPickUpdateReq"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/PostPickUpdate")]
    public ApiResult PostPickUpdate(PostPickUpdateReq postPickUpdateReq)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(postPickUpdateReq);

      string gupCode = p81Service.GetGupCode(postPickUpdateReq.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(postPickUpdateReq.DcNo, gupCode, postPickUpdateReq.CustNo, "PostPickUpdate", postPickUpdateReq, () =>
      {
        var p810106Service = new P810106Service(new WmsTransaction());
        return p810106Service.PostPickUpdate(postPickUpdateReq, gupCode);
      });
    }

    /// <summary>
    /// 揀貨完成確認
    /// </summary>
    /// <param name="postPickUpdateReq"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/Pda/PostPickConfirm")]
    public ApiResult PostPickConfirm(PostPickConfirmReq postPickConfirmReq)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(postPickConfirmReq);

      string gupCode = p81Service.GetGupCode(postPickConfirmReq.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(postPickConfirmReq.DcNo, gupCode, postPickConfirmReq.CustNo, "PostPickConfirm", postPickConfirmReq, () =>
      {
        var p810106Service = new P810106Service(new WmsTransaction());
        return p810106Service.PostPickConfirm(postPickConfirmReq, gupCode);
      });
    }
  }
}
