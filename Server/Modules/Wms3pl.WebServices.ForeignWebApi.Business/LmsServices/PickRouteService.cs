using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class PickRouteService: LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
		public PickRouteService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="pickOrdNo">揀貨單號</param>
		/// <param name="routeType">路順類型[1=訂單揀 2=廠退揀]</param>
		/// <param name="pickLocList">揀貨儲位編號清單</param>
		/// <returns></returns>
		public ApiResult GetLmsPickRoute(string dcCode, string gupCode, string custCode, string pickOrdNo, int routeType, List<string> pickLocList)
		{

			var commonService = new CommonService();
			var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			var lmsApiReq = new { DcCode = dcCode, RouteType = routeType, LocCodeList = pickLocList };
			
			#region 新增API Log
			var res = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_PickRouteResults", new { LmsApiUrl = LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "") + "wmsext-panel/api/PickingRoute/Calculate", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode, PickOrdNo = pickOrdNo }, LmsData = isSaveWmsData ? lmsApiReq : null }, () =>
			{
				ApiResult result;
#if (DEBUG)
                result = LmsApiRtnMultiFuncTest(lmsApiReq, "wmsext-panel/api/PickingRoute/Calculate", pickLocList.Cast<dynamic>().ToList());
#else
				result = LmsApiRtnMultiFunc<dynamic, PickLocRouteData, ErrorData>(lmsApiReq, "wmsext-panel/api/PickingRoute/Calculate");
#endif
				return result;
			}, false);
			#endregion


			if (!res.IsSuccessed)
			{
				return res;
			}

			var datas = res.Data as PickLocRouteData[];
			var pkAreaRouteBase = (int)Math.Pow(10, pickLocList.Count.ToString().Length);
			for (var index = 0; index < datas.Length; index++) {
				var pkAreaRouteBaseSeq = pkAreaRouteBase * index;

				var routeList = datas[index].RouteList;
				foreach (var route in routeList)
				{
					route.RouteSeq = route.RouteSeq + pkAreaRouteBaseSeq;
				}
			}


			return res;
		}
	}
}
