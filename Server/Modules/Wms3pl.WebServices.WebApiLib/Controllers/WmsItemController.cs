using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.WebApi.Business.TransApiServices;

namespace Wms3pl.WebServices.WebApiLib.Controllers
{
	public class WmsItemController : ApiController
	{
		/// <summary>
		/// 批次新增門市主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostRetailData")]
		public ApiResult PostRetailData(PostRetailDataReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostRetailData(req);
		}

		/// <summary>
		/// 批次新增商品階層檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostItemLevel")]
		public ApiResult PostItemLevel(PostItemLevelReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostItemLevel(req);
		}

		/// <summary>
		/// 批次新增供應商主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostVendorData")]
		public ApiResult PostVendorData(PostVendorDataReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostVendorData(req);
		}

		/// <summary>
		/// 批次新增商品主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostItemData")]
		public ApiResult PostItemData(PostItemDataReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostItemData(req);
		}

		/// <summary>
		/// 批次新增商品分類主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostItemCategory")]
		public ApiResult PostItemCategory(PostItemCategoryReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostItemCategory(req);
		}

		/// <summary>
		/// 批次新增商品組合主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostItemBom")]
		public ApiResult PostItemBom(PostItemBomReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostItemBom(req);
		}

		/// <summary>
		/// 快速移轉庫存調整單
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/FlashStockTransferData")]
		public ApiResult FlashStockTransferData(FlashStockTransferDataReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.FlashStockTransferData(req);
		}

		[Authorize]
		[HttpPost]
		[Route("api/WmsItem/PostUserData")]
		public ApiResult PostUserData(PostUserDataReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.PostUserData(req);
		}
	}
}
