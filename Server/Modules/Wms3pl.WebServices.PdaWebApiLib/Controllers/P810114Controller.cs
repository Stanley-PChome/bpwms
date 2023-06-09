using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
    /// <summary>
    /// 上架移動
    /// </summary>
    public class P810114Controller : ApiController
	{
        /// <summary>
        /// 上架移動-容器查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetApprovedData")]
        public ApiResult GetApprovedData(GetApprovedDataReq req)
        {
            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(req);

            string gupCode = p81Service.GetGupCode(req.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetApprovedData", req, () =>
            {
                var service = new P810114Service(new WmsTransaction());
                return service.GetApprovedData(req, gupCode);
            });
        }

        /// <summary>
        /// 上架移動-容器移動
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/MoveContainer")]
        public ApiResult MoveContainer(MoveContainerReq req)
        {
            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(req);

            string gupCode = p81Service.GetGupCode(req.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "MoveContainer", req, () =>
            {
                var service = new P810114Service(new WmsTransaction());
                return service.MoveContainer(req, gupCode);
            });
        }

        /// <summary>
        /// 上架移動-移動完成
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/MoveCompleted")]
        public ApiResult MoveCompleted(MoveCompletedReq req)
        {
            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(req);

            string gupCode = p81Service.GetGupCode(req.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "MoveCompleted", req, () =>
            {
                var service = new P810114Service(new WmsTransaction());
                return service.MoveCompleted(req, gupCode);
            });
        }
    }
}
