using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
  /// <summary>
  /// 跨庫調撥驗收入自動倉 (A7搬家功能)
  /// </summary>
  public class P810118Controller : ApiController
  {
    /// <summary>
    /// 跨庫調撥驗收入自動倉-容器查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/MoveInToAutoContainerQuery")]
    public ApiResult CartonReplenishAccept(GetTransferStockReceivedDataReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "MoveInToAutoContainerQuery", req, () =>
      {
        var service = new P810110Service(new WmsTransaction());
        return service.GetTransferStockReceivedData(req, gupCode);
      });
    }

    /// <summary>
    /// 跨庫調撥驗收入自動倉-驗收完成
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("api/pda/MoveInToAutoRecvFinish")]
    public ApiResult MoveInToAutoRecvFinish(MoveInToAutoRecvFinishReq req)
    {
      P81Service p81Service = new P81Service();

      p81Service.SetDefaulfStaff(req);

      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "MoveInToAutoRecvFinish", req, () =>
      {
        var service = new P810118Service(new WmsTransaction());
        return service.MoveInToAutoRecvFinish(req, gupCode);
      },(o)=>{
        // 21023訊息為此跨庫進貨容器系統正在處理中，不可解除容器鎖定
        if (o.MsgCode != "21023")
        {
          //訊息代號飛21023，解除容器鎖定
          var f076101Repo = new F076101Repository(Schemas.CoreSchema);
          f076101Repo.DeleteByContainerCode(req.ContainerCode.ToUpper());
        }
        return new ApiResult { IsSuccessed = true };
      });
    }


  }
}
