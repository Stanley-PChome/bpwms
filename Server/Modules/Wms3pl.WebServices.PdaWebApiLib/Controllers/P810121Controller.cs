using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 進貨容器綁定
	/// </summary>
	public class P810121Controller : ApiController
	{
		/// <summary>
		/// 取得進貨容器待關箱資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/pda/RecvNotCloseBindContainerQuery")]
		public ApiResult RecvNotCloseBindContainerQuery(RecvNotCloseBindContainerQueryReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvNotCloseBindContainerQuery", req, () =>
			{
				var service = new P810121Service(new WmsTransaction());
				return service.RecvNotCloseBindContainerQuery(req, gupCode);
			});
		}

		/// <summary>
		/// 進貨容器關箱確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/pda/RecvCloseBoxConfirm")]
		public ApiResult RecvCloseBoxConfirm(RecvCloseBoxConfirmReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "RecvCloseBoxConfirm", req, () =>
			{
				var service = new P810121Service(new WmsTransaction());
				return service.RecvCloseBoxConfirm(req, gupCode);
			});
		}

	}
}
