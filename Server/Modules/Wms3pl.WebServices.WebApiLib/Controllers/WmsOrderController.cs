using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Common;
using Wms3pl.WebServices.WebApi.Business.TransApiServices;

namespace Wms3pl.WebServices.WebApiLib.Controllers
{
	public class WmsOrderController : ApiController
	{
		/// <summary>
		/// 批次商品進倉資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsOrder/PostCreateWarehouses")]
		public ApiResult PostCreateWarehouses(PostCreateWarehousesReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostCreateWarehouses(req);
		}

		/// <summary>
		/// 批次新增訂單資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsOrder/PostCreateOrders")]
		public ApiResult PostCreateOrders(PostCreateOrdersReq req)
		{
			var comOrdService = new TransCommonService(new WmsTransaction());
			var comService = new CommonService();
			return comOrdService.PostCreateOrders(req);
		}

		/// <summary>
		/// 批次客戶退貨單資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsOrder/PostCreateReturns")]
		public ApiResult PostCreateReturns(PostCreateReturnsReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostCreateReturns(req);
		}

		/// <summary>
		/// 批次新增廠商退貨單資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsOrder/PostCreateVnrReturns")]
		public ApiResult PostCreateVnrReturns(PostCreateVendorReturnsReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostCreateVendorReturns(req);
		}

        /// <summary>
        /// 出貨更換物流商回報 2022/1/26增加
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/WmsOrder/ChangeTransportProvider")]
        public ApiResult ChangeTransportProvider(ChangeTransportProviderReq req)
        {
					var comService = new TransCommonService(new WmsTransaction());
					return comService.ChangeTransportProvider(req);
			
        }

    }
}
