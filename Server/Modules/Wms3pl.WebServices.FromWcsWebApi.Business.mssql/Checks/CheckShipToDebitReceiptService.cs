using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckShipToDebitReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();
		private CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
		private CommonService commonService = new CommonService();

		#region 分揀出貨檢核

		// 檢核單號是否存在於F055001
		public void CheckShipCodeByF055001IsExist(List<ApiResponse> res, string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel box)
		{
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			var f055001 = f055001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.PAST_NO == box.ShipCode);
			// 檢查[ShipCode]是否存在[F055001.PAST_NO]，若不存在則回傳失敗訊息23001([單據:{0}]單據不存在)
			if (f055001 == null)
			{
				res.Add(new ApiResponse { No = box.ShipCode, MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), box.ShipCode) });
			}
			else
			{
				if (f055001.STATUS == "1") // 檢核單據是否已結案，若是則回傳失敗訊息23003(單據已結案)。查詢F055001.STATUS = 1
					res.Add(new ApiResponse { No = box.ShipCode, MsgCode = "23003", MsgContent = string.Format(tacService.GetMsg("23003"), box.ShipCode) });
			}
		}
		#endregion
	}
}
