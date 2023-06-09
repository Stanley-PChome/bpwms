using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
    public class P810105Controller : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetStock")]
        public ApiResult GetStock(GetStockReq getStockReq)
        {
            var wmsTransation = new WmsTransaction();

            P81Service p81Service = new P81Service();

            p81Service.SetDefaulfStaff(getStockReq);

            var gupCode = p81Service.GetGupCode(getStockReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(getStockReq.DcNo, gupCode, getStockReq.CustNo, "getStock", getStockReq, () =>
            {
                var service = new P810105Service(wmsTransation);
                return service.GetStock(getStockReq);
            });
        }
    }
}
