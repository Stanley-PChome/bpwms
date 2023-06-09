using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
    public class P810108Controller : ApiController
    {
        /// <summary>
        /// 搬移作業查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetMove")]
        public ApiResult GetMoveLoc(GetMoveReq req)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(req);
            var gupCode = p81Service.GetGupCode(req.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "getMove", req, () =>
            {
                var service = new P810108Service(wmsTranstaion);
                return service.GetMove(req, gupCode);
            });
        }

        /// <summary>
        /// 搬移作業-儲位查詢
        /// </summary>
        /// <param name="getMoveLocReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetMoveLoc")]
        public ApiResult GetMoveLoc(GetMoveLocReq getMoveLocReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(getMoveLocReq);
            var gupCode = p81Service.GetGupCode(getMoveLocReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(getMoveLocReq.DcNo, gupCode, getMoveLocReq.CustNo, "getMoveLoc", getMoveLocReq, () =>
            {
                var service = new P810108Service(wmsTranstaion);
                return service.GetMoveLoc(getMoveLocReq, gupCode);
            });
        }

        /// <summary>
        /// 搬移作業-搬移確認
        /// </summary>
        /// <param name="postMoveConfirmReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/PostMoveConfirm")]
        public ApiResult PostMoveConfirm(PostMoveConfirmReq postMoveConfirmReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(postMoveConfirmReq);
            var gupCode = p81Service.GetGupCode(postMoveConfirmReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(postMoveConfirmReq.DcNo, gupCode, postMoveConfirmReq.CustNo, "postMoveConfirm", postMoveConfirmReq, () =>
            {
                var service = new P810108Service(wmsTranstaion);
                return service.PostMoveConfirm(postMoveConfirmReq);
            });
        }
    }
}
