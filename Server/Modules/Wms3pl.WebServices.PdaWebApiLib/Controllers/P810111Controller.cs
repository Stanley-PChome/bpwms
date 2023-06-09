using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 集貨出入場
	/// </summary>
	public class P810111Controller : ApiController
	{
		/// <summary>
		/// 集貨入場-入場檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/CheckContainerCodeForCollection")]
		public ApiResult CheckContainerCodeForCollection(CheckContainerCodeForCollectionReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "CheckContainerCodeForCollection", req, () =>
			{
				var p810111Service = new P810111Service(new WmsTransaction());
				return p810111Service.CheckContainerCodeForCollection(req, gupCode);
			});
		}

		/// <summary>
		/// 集貨入場-入場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/ConfirmContainerCodeForCollection")]
		public ApiResult ConfirmContainerCodeForCollection(ConfirmContainerCodeForCollectionReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmContainerCodeForCollection", req, () =>
			{
				var p810111Service = new P810111Service(new WmsTransaction());
				return p810111Service.ConfirmContainerCodeForCollection(req, gupCode);
			});
		}

		/// <summary>
		/// 集貨出場-可出場查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetAvailableCellCodeData")]
		public ApiResult GetAvailableCellCodeData(GetAvailableCellCodeDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetAvailableCellCodeData", req, () =>
			{
				var p810111Service = new P810111Service(new WmsTransaction());
				return p810111Service.GetAvailableCellCodeData(req, gupCode);
			});
		}

		/// <summary>
		/// 集貨出場-出場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/ConfirmCellCode")]
		public ApiResult ConfirmCellCode(ConfirmCellCodeReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmCellCode", req, () =>
			{
				var p810111Service = new P810111Service(new WmsTransaction());
				return p810111Service.ConfirmCellCode(req, gupCode);
			});
		}

	}
}
