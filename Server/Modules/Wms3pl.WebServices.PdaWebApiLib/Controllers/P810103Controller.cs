using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	/// <summary>
	/// 調撥作業
	/// </summary>
	public class P810103Controller : ApiController
	{
		/// <summary>
		/// 調撥單據查詢
		/// </summary>
		/// <param name="getAllocReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetAlloc")]
		public ApiResult GetAlloc(GetAllocReq getAllocReq)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(getAllocReq);

			string gupCode = p81Service.GetGupCode(getAllocReq.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(getAllocReq.DcNo, gupCode, getAllocReq.CustNo, "GetAlloc", getAllocReq, () =>
			 {
				 var p810103Service = new P810103Service(new WmsTransaction());
				 return p810103Service.GetAlloc(getAllocReq, gupCode);
			 });
		}

		/// <summary>
		/// 調撥單據檢核
		/// </summary>
		/// <param name="postAllocCheckReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/PostAllocCheck")]
		public ApiResult PostAllocCheck(PostAllocCheckReq postAllocCheckReq)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(postAllocCheckReq);

			string gupCode = p81Service.GetGupCode(postAllocCheckReq.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(postAllocCheckReq.DcNo, gupCode, postAllocCheckReq.CustNo, "PostAllocCheck", postAllocCheckReq, () =>
			{
				var p810103Service = new P810103Service(new WmsTransaction());
				return p810103Service.PostAllocCheck(postAllocCheckReq, gupCode);
			});
		}

		/// <summary>
		/// 調撥單據更新
		/// </summary>
		/// <param name="postAllocUpdateReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/PostAllocUpdate")]
		public ApiResult PostAllocUpdate(PostAllocUpdateReq postAllocUpdateReq)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(postAllocUpdateReq);

			string gupCode = p81Service.GetGupCode(postAllocUpdateReq.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(postAllocUpdateReq.DcNo, gupCode, postAllocUpdateReq.CustNo, "PostAllocUpdate", postAllocUpdateReq, () =>
			 {
				 var p810103Service = new P810103Service(new WmsTransaction());
				 return p810103Service.PostAllocUpdate(postAllocUpdateReq, gupCode);
			 });
		}

		/// <summary>
		/// 調撥明細查詢
		/// </summary>
		/// <param name="getAllocDetailReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetAllocDetail")]
		public ApiResult GetAllocDetail(GetAllocDetailReq getAllocDetailReq)
		{
			P81Service p81Service = new P81Service();
			p81Service.SetDefaulfStaff(getAllocDetailReq);
			string gupCode = p81Service.GetGupCode(getAllocDetailReq.CustNo);
			return ApiLogHelper.CreatePdaLogInfo(getAllocDetailReq.DcNo, gupCode, getAllocDetailReq.CustNo, "GetAllocDetail", getAllocDetailReq, () =>
			{
                var wmsTransaction = new WmsTransaction();
                if (getAllocDetailReq.IsSync == "1")
                    return new P810103Service(wmsTransaction).GetAllocDetail(getAllocDetailReq, gupCode);
                else
                    return new P810103CustomService(wmsTransaction).GetAllocDetail(getAllocDetailReq, gupCode);
            });
		}

		/// <summary>
		/// 調撥確認
		/// </summary>
		/// <param name="postAllocConfirmReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/PostAllocConfirm")]
		public ApiResult PostAllocConfirm(PostAllocConfirmReq postAllocConfirmReq)
		{
			P81Service p81Service = new P81Service();

			p81Service.SetDefaulfStaff(postAllocConfirmReq);

			string gupCode = p81Service.GetGupCode(postAllocConfirmReq.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(postAllocConfirmReq.DcNo, gupCode, postAllocConfirmReq.CustNo, "PostAllocConfirmReq", postAllocConfirmReq, () =>
			{
				var p810103Service = new P810103Service(new WmsTransaction());
				return p810103Service.PostAllocConfirm(postAllocConfirmReq, gupCode);
			});
		}
	}
}
