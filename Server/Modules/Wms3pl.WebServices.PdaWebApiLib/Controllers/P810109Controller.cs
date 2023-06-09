using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
    /// <summary>
    /// 序號查詢
    /// </summary>
    public class P810109Controller : ApiController
    {
        /// <summary>
        /// 序號查詢
        /// </summary>
        /// <param name="getAllocReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetSerial")]
        public ApiResult GetSerial(GetSerialReq getSerialReq)
        {
            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(getSerialReq);

            string gupCode = p81Service.GetGupCode(getSerialReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(getSerialReq.DcNo, gupCode, getSerialReq.CustNo, "GetSerial", getSerialReq, () =>
             {
                 var p810109Service = new P810109Service(new WmsTransaction());
                 return p810109Service.GetSerial(getSerialReq, gupCode);
             });
        }
    }
}
