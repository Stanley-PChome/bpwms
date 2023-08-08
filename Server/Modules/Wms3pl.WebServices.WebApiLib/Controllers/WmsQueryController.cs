using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.WebApi.Business.TransApiServices;

namespace Wms3pl.WebServices.WebApiLib.Controllers
{
	public class WmsQueryController : ApiController
	{
		/// <summary>
		/// 查詢該商品在物流中心的總量與在各倉別的數量
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsQuery/GetItemStocks")]
		public ApiResult GetItemStocks(GetItemStocksReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.GetItemStocks(req);
		}

		/// <summary>
		/// 庫存明細資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsQuery/GetItemStockDetails")]
		public ApiResult GetItemStockDetails(GetItemStockDetailsReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.GetItemStockDetails(req);
		}

		/// <summary>
		/// 商品序號查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/WmsQuery/GetItemSerials")]
		public ApiResult GetItemSerials(GetItemSerialsReq req)
		{
			var comService = new TransCommonService(new WmsTransaction());
			return comService.GetItemSerials(req);
		}
	}
}
