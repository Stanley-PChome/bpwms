using System.Web;
using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	public class P810102Controller : ApiController
	{
		[Authorize]
		[HttpPost]
		[Route("api/Pda/PostLogout")]
		public ApiResult PostLogout(PostLogoutReq postLogoutReq)
		{
			var p81Service = new P81Service();
			p81Service.SetDefaulfStaff(postLogoutReq);
			return ApiLogHelper.CreatePdaLogInfo(null, null, null, "postLogout", postLogoutReq, () =>
			{
				var service = new P810102Service();
				return service.PostLogout(postLogoutReq);
			});
		}

		[Authorize]
		[HttpPost]
		[Route("api/Pda/PostLogin")]
		public ApiResult PostLogin(PostLoginReq postLoginReq)
		{
			var p81Service = new P81Service();
			var checkDevCode = p81Service.CheckDevCode(postLoginReq.DevCode);
			if (checkDevCode == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20151", MsgContent = p81Service.GetMsg("20151") };
			
			// 產生主機授權碼
			var scCode = AesCryptor.Current.Encode(DbSchemaHelper.GenSCCode(checkDevCode.Value));
			// 裝置驗證碼檢核成功
			HttpContext.Current.Request.Headers["SCCode"] = scCode;

			p81Service.SetDefaulfStaff(postLoginReq);

			return ApiLogHelper.CreatePdaLogInfo(null, null, null, "postLogin", postLoginReq, () =>
			{
				var service = new P810102Service();
				return service.PostLogin(postLoginReq, scCode);
			});
		}
	}
}
