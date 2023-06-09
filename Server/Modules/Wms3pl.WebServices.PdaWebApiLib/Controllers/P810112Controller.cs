using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 進貨收發
	/// </summary>
	public class P810112Controller : ApiController
	{
		/// <summary>
		/// 進貨收發-收貨確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/ConfirmRcvStockData")]
		public ApiResult ConfirmRcvStockData(ConfirmRcvStockDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmRcvStockData", req, () =>
			{
				var P810112Service = new P810112Service(new WmsTransaction());
				return P810112Service.ConfirmRcvStockData(req, gupCode);
			});
		}

		/// <summary>
		/// 進貨收發-詳細資訊
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/RcvStockDetailData")]
		public ApiResult RcvStockDetailData(RcvStockDetailDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RcvStockDetailData", req, () =>
			{
				var P810112Service = new P810112Service(new WmsTransaction());
				return P810112Service.RcvStockDetailData(req, gupCode);
			});
		}
	}
}
