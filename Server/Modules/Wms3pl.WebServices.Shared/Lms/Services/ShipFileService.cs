using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
	public class ShipFileService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
		public ShipFileService(WmsTransaction wmsTransation = null)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ShipFile(string dcCode, string gupCode, string custCode, string url, int rePrint)
		{

			var lmsApiReq = new VnrReturnOrderPrintReq
			{
				MacAddr = null,
				Username = null,
				RePrint = rePrint
			};

			#region 新增API Log
			var res = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ShipFile", new { LmsApiUrl = url, LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode, URL = url, RePrint = rePrint }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
			{
				ApiResult result;
				result = LmsApiFuncFile(lmsApiReq, url);
				return result;
			}, false);
			#endregion
			return res;
		}
	}
}
