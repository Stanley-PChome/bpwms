using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 廠退便利倉
	/// </summary>
	public class P810116Controller : ApiController
	{
		/// <summary>
		/// 廠退便利倉-進場檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/VnrConvenientBookin")]
		public ApiResult VnrConvenientBookin(VnrConvenientBookinReq req)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(req);
			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "VnrConvenientBookin", req, () =>
			{
				var p810111Service = new P810116Service(new WmsTransaction());
				return p810111Service.VnrConvenientBookin(req, gupCode);
			});
		}

		/// <summary>
		/// 廠退便利倉-進場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/VnrConvenientBookinConfirm")]
		public ApiResult VnrConvenientBookinConfirm(VnrConvenientBookinConfirmReq req)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(req);
			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "VnrConvenientBookinConfirm", req, () =>
			{
				var p810111Service = new P810116Service(new WmsTransaction());
				return p810111Service.VnrConvenientBookinConfirm(req, gupCode);
			});
		}

		/// <summary>
		/// 廠退便利倉-出場查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/VnrConvenientOutQuery")]
		public ApiResult VnrConvenientOutQuery(VnrConvenientOutQueryReq req)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(req);
			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "VnrConvenientOutQuery", req, () =>
			{
				var p810111Service = new P810116Service(new WmsTransaction());
				return p810111Service.VnrConvenientOutQuery(req, gupCode);
			});
		}

		/// <summary>
		/// 廠退便利倉-出場儲格明細
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/VnrConvenientOutCellDetail")]
		public ApiResult VnrConvenientOutCellDetail(VnrConvenientOutCellDetailReq req)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(req);
			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "VnrConvenientOutCellDetail", req, () =>
			{
				var p810111Service = new P810116Service(new WmsTransaction());
				return p810111Service.VnrConvenientOutCellDetail(req, gupCode);
			});
		}

		/// <summary>
		/// 廠退便利倉-出場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/VnrConvenientOutConfirm")]
		public ApiResult VnrConvenientOutConfirm(VnrConvenientOutConfirmReq req)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(req);
			string gupCode = p81Service.GetGupCode(req.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "VnrConvenientOutConfirm", req, () =>
			{
				var p810111Service = new P810116Service(new WmsTransaction());
				return p810111Service.VnrConvenientOutConfirm(req, gupCode);
			});
		}
	}
}
