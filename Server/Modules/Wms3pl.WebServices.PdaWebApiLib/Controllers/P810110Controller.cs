using System.Web.Http;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 跨庫調撥作業
	/// </summary>
	public class P810110Controller : ApiController
	{
		/// <summary>
		/// 跨庫進倉驗收查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetTransferStockReceivedData")]
		public ApiResult GetTransferStockReceivedData(GetTransferStockReceivedDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetTransferStockReceivedData", req, () =>
			 {
				 var p810110Service = new P810110Service(new WmsTransaction());
				 return p810110Service.GetTransferStockReceivedData(req, gupCode);
			 });
		}

		/// <summary>
		/// 跨庫進倉驗收檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/ConfirmTransferStockReceivedData")]
		public ApiResult ConfirmTransferStockReceivedData(ConfirmTransferStockReceivedDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmTransferStockReceivedData", req, () =>
			{
				var p810110Service = new P810110Service(new WmsTransaction());
				return p810110Service.ConfirmTransferStockReceivedData(req, gupCode);
			});
		}

		/// <summary>
		/// 跨庫進倉上架查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetTransferStockInData")]
		public ApiResult GetTransferStockInData(GetTransferStockInDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetTransferStockInData", req, () =>
			 {
				 var p810110Service = new P810110Service(new WmsTransaction());
				 return p810110Service.GetTransferStockInData(req, gupCode);
			 });
		}

		/// <summary>
		/// 跨庫進倉上架確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/ConfirmTransferStockInData")]
		public ApiResult ConfirmTransferStockInData(ConfirmTransferStockInDataReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "ConfirmTransferStockInData", req, () =>
			{
				var p810110Service = new P810110Service(new WmsTransaction());
				return p810110Service.ConfirmTransferStockInData(req, gupCode);
			},(o)=>{
				// 21023訊息為此跨庫進貨容器系統正在處理中，不可解除容器鎖定
				if (o.MsgCode != "21023")
				{
					//訊息代號飛21023，解除容器鎖定
					var f076101Repo = new F076101Repository(Schemas.CoreSchema);
					f076101Repo.DeleteByContainerCode(req.ContainerCode.ToUpper());
				}
				return new ApiResult { IsSuccessed = true };
			});
		}
	}
}
