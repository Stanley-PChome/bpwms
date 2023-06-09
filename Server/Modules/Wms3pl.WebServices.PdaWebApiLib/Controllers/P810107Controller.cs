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
    public class P810107Controller : ApiController
    {
        /// <summary>
        /// 盤點作業-盤點單號查詢
        /// </summary>
        /// <param name="getInvReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetInv")]
        public ApiResult GetInv(GetInvReq getInvReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            var gupCode = p81Service.GetGupCode(getInvReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(getInvReq.DcNo, gupCode, getInvReq.CustNo, "getInv", getInvReq, () =>
            {
                var service = new P810107Service(wmsTranstaion);
                return service.GetInv(getInvReq, gupCode);
            });


        }

        /// <summary>
        /// 盤點作業-盤點明細查詢
        /// </summary>
        /// <param name="getDetailInvReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/GetDetailInv")]
        public ApiResult GetDetailInv(GetDetailInvReq getDetailInvReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(getDetailInvReq);
            var gupCode = p81Service.GetGupCode(getDetailInvReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(getDetailInvReq.DcNo, gupCode, getDetailInvReq.CustNo, "getDetailInv", getDetailInvReq, () =>
            {
                var wmsTransaction = new WmsTransaction();
                if (getDetailInvReq.IsSync == "1")
                    return new P810107Service(wmsTransaction).GetDetailInv(getDetailInvReq, gupCode);
                else
                    return new P810107CustomService(wmsTransaction).GetDetailInv(getDetailInvReq, gupCode);
            });
        }

        /// <summary>
        /// 盤點作業-盤點確認
        /// </summary>
        /// <param name="getDetailInvReq"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Pda/PostInvConfirm")]
        public ApiResult PostInvConfirm(PostInvConfirmReq postInvConfirmReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(postInvConfirmReq);
            var gupCode = p81Service.GetGupCode(postInvConfirmReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(postInvConfirmReq.DcNo, gupCode, postInvConfirmReq.CustNo, "postInvConfirm", postInvConfirmReq, () =>
            {
                var service = new P810107Service(wmsTranstaion);
                return service.PostInvConfirm(postInvConfirmReq, gupCode);
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/Pda/PostInvNewItem")]
        public ApiResult PostInvNewItem(PostInvNewItemReq postInvNewItemReq)
        {
            var wmsTranstaion = new WmsTransaction();

            P81Service p81Service = new P81Service();
            p81Service.SetDefaulfStaff(postInvNewItemReq);
            var gupCode = p81Service.GetGupCode(postInvNewItemReq.CustNo);

            return ApiLogHelper.CreatePdaLogInfo(postInvNewItemReq.DcNo, gupCode, postInvNewItemReq.CustNo, "postInvNewItem", postInvNewItemReq, () =>
            {
                var service = new P810107Service(wmsTranstaion);
                return service.PostInvNewItem(postInvNewItemReq, gupCode);
            });
        }
    }
}
