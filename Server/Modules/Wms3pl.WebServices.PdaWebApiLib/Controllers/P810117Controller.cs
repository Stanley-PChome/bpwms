using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 紙箱補貨
	/// </summary>
	public class P810117Controller : ApiController
	{
		/// <summary>
		/// 紙箱補貨-接受任務
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/CartonReplenishAccept")]
		public ApiResult CartonReplenishAccept(CartonReplenishAcceptReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "CartonReplenishAccept", req, () =>
			{
				var service = new P810117Service(new WmsTransaction());
				return service.CartonReplenishAccept(req);
			});
		}

		/// <summary>
		/// 紙箱補貨-放棄任務
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/CartonReplenishReject")]
		public ApiResult CartonReplenishReject(CartonReplenishRejectReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "CartonReplenishReject", req, () =>
			{
				var service = new P810117Service(new WmsTransaction());
				return service.CartonReplenishReject(req);
			});
		}

		/// <summary>
		/// 紙箱補貨-查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetCartonReplenish")]
		public ApiResult GetCartonReplenish(GetCartonReplenishReq req)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(req);

			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "GetCartonReplenish", req, () =>
			{
				var service = new P810117Service(new WmsTransaction());
				return service.GetCartonReplenish(req);
			});
		}
	

    [Authorize]
    [HttpPost]
    [Route("api/pda/CartonReplenishFinish")]
    public ApiResult CartonReplenishFinish(CartonReplenishFinishReq req)
    {
      P81Service p81Service = new P81Service();
      p81Service.SetDefaulfStaff(req);
      string gupCode = p81Service.GetGupCode(req.CustNo);

      return ApiLogHelper.CreatePdaLogInfo(req.DcNo, gupCode, req.CustNo, "CartonReplenishFinish", req, () =>
      {
        var service = new P810117Service(new WmsTransaction());
        return service.CartonReplenishFinish(req, gupCode);
      });

    }

  }
}
