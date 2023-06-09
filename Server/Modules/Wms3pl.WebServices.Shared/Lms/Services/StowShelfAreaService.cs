using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
    public class StowShelfAreaService : LmsBaseService
    {
        private WmsTransaction _wmsTransaction;
        public StowShelfAreaService(WmsTransaction wmsTransation = null)
        {
            _wmsTransaction = wmsTransation;
        }

        /// <summary>
        /// 上架倉別指示
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="stowType"></param>
        /// <param name="custInNo"></param>
        /// <param name="itemCodeList"></param>
        /// <returns></returns>
        public ApiResult StowShelfAreaGuide(string dcCode, string gupCode, string custCode, string stowType, string custInNo, List<string> itemCodeList)
        {
            // 取得傳入資料
            var lmsApiReq = new StowShelfAreaGuideReq
            {
                DcCode = dcCode,
                CustCode = custCode,
                StowType = stowType,
                CustInNo = custInNo,
                ItemList = itemCodeList.Select(x => new StowShelfAreaGuideItemData { ItemCode = x }).ToList()
            };

            #region 新增API Log
            var res = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_StowShelfAreaGuide", new { LmsApiUrl = LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "") + "wmsext-panel/api/StowShelfArea/Guide", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode, CustInNo = custInNo }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
            {
                ApiResult result;
#if (DEBUG || TEST)
                result = LmsApiRtnMultiFuncTest(lmsApiReq, "wmsext-panel/api/StowShelfArea/Guide", new List<dynamic>());
#else
				result = LmsApiRtnMultiFunc<dynamic, StowShelfAreaGuideData, ErrorData>(lmsApiReq, "wmsext-panel/api/StowShelfArea/Guide");
#endif
                return result;
            }, false);
            #endregion

            return res;
        }


		/// <summary>
		/// 上架倉別分配
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="custCode"></param>
		/// <param name="custInNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="qty"></param>
		/// <returns></returns>
		public ApiResult StowShelfAreaAssign(string dcCode, string custCode, string custInNo, string itemCode, int qty)
		{
			var lmsApiReq = new StowShelfAreaAssognReq
			{
				DcCode = dcCode,
				CustCode = custCode,
				CustInNo = custInNo,
				ItemCode = itemCode,
				Qty = qty
			};

			#region 新增API Log
			var res = ApiLogHelper.CreateApiLogInfo(dcCode,
				"0",
				custCode, "LmsApi_StowShelfAreaAssign",
				new
				{
					LmsApiUrl = LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "") + "wmsext-panel/api/StowShelfArea/Assign",
					LmsToken = LmsSetting.ApiAuthToken,
					WmsCondition = new { DC_CODE = dcCode, CUST_CODE = custCode, CustInNo = custInNo, ItemCode = itemCode, Qty = qty },
					WmsData = isSaveWmsData ? lmsApiReq : null
				}, () =>
				{
					ApiResult result;
#if (DEBUG || TEST)
					result = LmsApiRtnMultiFuncTest(lmsApiReq, "wmsext-panel/api/StowShelfArea/Assign", new List<dynamic>());
#else
									result = LmsApiRtnMultiFunc<dynamic, StowShelfAreaAssignData, ErrorData>(lmsApiReq, "wmsext-panel/api/StowShelfArea/Assign");
#endif
					return result;
				}, false);
      #endregion

      #region 檢查回傳的內容
      if (res.IsSuccessed)
      {
        if (res.Data == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data is null" };
        var data = JsonConvert.SerializeObject(res.Data);
        if (string.IsNullOrEmpty(data))
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data is null" };
        var resData = JsonConvert.DeserializeObject<List<StowShelfAreaAssignData>>(data);
        if (resData.Any(x => string.IsNullOrWhiteSpace(x.ShelfAreaCode)))
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "取得的上架倉別內容為空，請先設定上架倉別" };
        if (resData.Any(x => string.IsNullOrWhiteSpace(x.Type)))
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "取得的揀區/補區為空，請先設定揀/補區" };
        if (resData.Sum(x => x.Qty) != qty)
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "取得的上架合計數量不等於良品合計數量" };

        res.Data = resData;
      }
      #endregion 檢查回傳的內容

      return res;
    }
  }
}
