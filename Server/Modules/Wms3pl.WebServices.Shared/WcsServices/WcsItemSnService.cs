using System;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.Shared.WcsServices
{
	public class WcsItemSnService : WcsBaseService
	{
		private WmsTransaction _wmsTransaction;
		public WcsItemSnService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ApiResult ItemSn(string url, WcsSnReq req)
		{
			try
			{
#if (DEBUG)
				return WcsApiFuncTest(req, "ItemSn");
#else
        return WcsApiFunc(req, url);
#endif
			}
			catch (Exception ex)
			{
				return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.Message };
			}
		}
	}
}
