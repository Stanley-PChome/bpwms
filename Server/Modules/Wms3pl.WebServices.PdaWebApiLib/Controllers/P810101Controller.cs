using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.PdaWebApi.Business.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.PdaWebApiLib.Controllers
{
	public class P810101Controller : ApiController
	{
		/// <summary>
		/// 商品主檔同步API
		/// </summary>
		/// <param name="getBatchItemReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetBatchItem")]
		public ApiResult GetBatchItem(GetBatchItemReq getBatchItemReq)
		{
			var wmsTransation = new WmsTransaction();

			P81Service p81Service = new P81Service();
			string gupCode = p81Service.GetGupCode(getBatchItemReq.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(null, gupCode, getBatchItemReq.CustNo, "getBatchItem", getBatchItemReq, () =>
			{
				var service = new P810101Service(wmsTransation);
				return service.GetBatchItem(getBatchItemReq);
			});
		}

		/// <summary>
		/// 商品序號主檔同步
		/// </summary>
		/// <param name="getBatchItemSerialReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetBatchItemSerial")]
		public ApiResult GetBatchItemSerial(GetBatchItemSerialReq getBatchItemSerialReq)
		{
			var wmsTransation = new WmsTransaction();

			P81Service p81Service = new P81Service();
			string gupCode = p81Service.GetGupCode(getBatchItemSerialReq.CustNo);


			return ApiLogHelper.CreatePdaLogInfo(null, gupCode, getBatchItemSerialReq.CustNo, "getBatchItemSerial", getBatchItemSerialReq, () =>
			{
				var service = new P810101Service(wmsTransation);
				return service.GetBatchItemSerial(getBatchItemSerialReq);
			});

		}

		/// <summary>
		/// 依據商品搜尋條件至找出品號API
		/// </summary>
		/// <param name="getItemCodeBySnReq"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("api/Pda/GetItemByCondition")]
		public ApiResult GetItemByCondition(GetItemByConditionReq req)
		{
			var wmsTransation = new WmsTransaction();

			P81Service p81Service = new P81Service();
			string gupCode = p81Service.GetGupCode(req.CustNo);

			return ApiLogHelper.CreatePdaLogInfo(null, gupCode, req.CustNo, "GetItemByCondition", req, () =>
			{
				var service = new P810101Service(wmsTransation);
				return service.GetItemByCondition(req, gupCode);
			});
		}
	}
}
