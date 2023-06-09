using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
    /// <summary>
    /// 容器查詢
    /// </summary>
    public class P810115Controller : ApiController
	{
        /// <summary>
        /// 容器查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetContainerInfo")]
        public ApiResult GetContainerInfo(GetContainerInfoReq req)
        {
            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(req);

            string gupCode = p81Service.GetGupCode(req.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetContainerInfo", req, () =>
            {
                var service = new P810115Service(new WmsTransaction());
                return service.GetContainerInfo(req, gupCode);
            });
        }
    }
}
