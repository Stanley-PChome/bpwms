using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
	public class ConsignService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;

		public ConsignService(WmsTransaction wmsTransation = null)
		{
			_wmsTransaction = wmsTransation;
		}

		/// <summary>
		/// 申請宅配單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="isScanBox"></param>
		/// <param name="SugBoxNo"></param>
		/// <returns></returns>
		public ExecuteResult ApplyConsign(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo, string isScanBox = null, string sugBoxNo = null, F055001 f055001 = null)
		{
      ExecuteResult res = new ExecuteResult { IsSuccessed = true };
			var transportCode = string.Empty;
			var apiResult2 = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ApplyConsign", new { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, WmsOrdNo = wmsOrdNo, PackageBoxNo = packageBoxNo, IsScanBox = isScanBox, SugBoxNo = sugBoxNo }, () =>
			{
				var lmsApiReq = GetApplyRequestData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo, sugBoxNo, f055001);
				var apiResult = LmsApplyConsign(lmsApiReq, gupCode);
				if (apiResult.IsSuccessed)
				{
					var rtnData = (LmsDataResult)apiResult.Data;
					ProcessData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo, rtnData, isScanBox, sugBoxNo, f055001);
					transportCode = rtnData.TransportCode;
				}
				return apiResult;
			}
			,false);

      if (!apiResult2.IsSuccessed)
        res = new ExecuteResult { IsSuccessed = apiResult2.IsSuccessed, Message = "[LMS申請宅配單]" + apiResult2.MsgCode + " " + apiResult2.MsgContent, No = transportCode };
      else
        res.No = transportCode;
      return res;
		}

		public ApiResult LmsApplyConsign(ShipOrderInsertReq req,string gupCode)
		{
			string url = "ShipOrder/Insert";

				// 取得傳入資料
				var lmsApiReq = req;

				// apiResult = &新增API Log[< 參數1 >,< 參數2 >,< 參數3 >, LmsApi_ApplyConsign,{ WMS_ORD_NO =< 參數4,PackageBoxNo =< 參數5 >} ,&LmsApiFunc1 ,false] 
				var apiResult = ApiLogHelper.CreateApiLogInfo(req.DcCode, gupCode, req.CustCode, "LmsApi_ApplyConsign", new { LmsApiUrl = $"{LmsSetting.ApiUrl}{url}", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { WMS_ORD_NO = req.Packages.WmsNo, PackageBoxNo = req.Packages.BoxNo }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
				{
#if (DEBUG || TEST)
					return LmsApiFuncTest(lmsApiReq, url);
#else
					return LmsApiFunc(lmsApiReq, url);
#endif
				}, false);

			if (apiResult.IsSuccessed)
			{
				if (apiResult.Data == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data is null" };
				var data = JsonConvert.SerializeObject(apiResult.Data);
				if (string.IsNullOrEmpty(data))
					return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data is null" };
				var rtnData = JsonConvert.DeserializeObject<LmsDataResult>(data);
				if (string.IsNullOrEmpty(rtnData.TransportCode))
					return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data 宅配單號是空白" };
				if (string.IsNullOrEmpty(rtnData.TransportProvider))
					return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "Lms Api 回傳的Data 物流商編號是空白" };
				apiResult.Data = rtnData;
			}

			return apiResult;
		}

		/// <summary>
		/// 取消宅配單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public ExecuteResult CancelConsign(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			string url = "ShipOrder/Insert";

			ExecuteResult res = new ExecuteResult { IsSuccessed = true };
			// 取得傳入資料
			var lmsApiReq = GetCancelRequestData(dcCode, gupCode, custCode, wmsOrdNo);

			// apiResult = &新增API Log[< 參數1 >,< 參數2 >,< 參數3 >, LmsApi_ApplyConsign,{ WMS_ORD_NO =< 參數4,PackageBoxNo =< 參數5 >} ,&LmsApiFunc1 ,false] 
			var apiResult = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_CancelConsign", new { LmsApiUrl = $"{LmsSetting.ApiUrl}{url}", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { WMS_ORD_NO = wmsOrdNo }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
			{
#if (DEBUG || TEST)
							return LmsApiFuncTest(lmsApiReq, url);
#else
                return LmsApiFunc(lmsApiReq, url);
#endif
						}, false);

			if (!apiResult.IsSuccessed)
				res = new ExecuteResult { IsSuccessed = false, Message = "[LMS取消申請宅配單]" + apiResult.MsgCode + " " + apiResult.MsgContent };

			return res;
		}

		#region 私有方法
		/// <summary>
		/// 取得傳入資料 (新增)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="Currf055001">外部傳入的f055001，如果沒有才去撈DB</param>
		/// <returns></returns>
		private ShipOrderInsertReq GetApplyRequestData(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo, string sugBoxNo, F055001 Currf055001 = null)
		{
			var res = new ShipOrderInsertReq { DcCode = dcCode, CustCode = custCode };

			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);

			var f050301 = f050301Repo.GetDataByWmsOrdNo(dcCode,gupCode,custCode,wmsOrdNo);

			F055001 f055001;
			if (Currf055001 == null)
			{
				f055001 = f055001Repo.Find(o => o.DC_CODE == dcCode &&
							o.GUP_CODE == gupCode &&
							o.CUST_CODE == custCode &&
							o.WMS_ORD_NO == wmsOrdNo &&
							o.PACKAGE_BOX_NO == packageBoxNo);
			}
			else
				f055001 = Currf055001;

			var f055002s = f055002Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.WMS_ORD_NO == wmsOrdNo &&
			o.PACKAGE_BOX_NO == packageBoxNo).ToList();

			if (f050301 != null && f055001 != null)
			{
				return GetApplyRequestData(f050301, f055001, f055002s, sugBoxNo);
			}

			return res;
		}

		/// <summary>
		///  取得傳入資料 (新增)
		/// </summary>
		/// <param name="f050301"></param>
		/// <param name="f055001"></param>
		/// <param name="f055002List"></param>
		/// <param name="sugBoxNo"></param>
		/// <returns></returns>
		public ShipOrderInsertReq GetApplyRequestData(F050301 f050301, F055001 f055001, List<F055002> f055002List,string sugBoxNo)
		{
			var res = new ShipOrderInsertReq { DcCode = f055001.DC_CODE, CustCode = f055001.CUST_CODE };

			res.Packages = new ShipOrderInsertPackage
			{
				CustOrdNo = f050301.CUST_ORD_NO,
				OrderType = f050301.SOURCE_TYPE == "13" ? "RO" : "SO",
				WmsNo = f055001.WMS_ORD_NO,
				BoxNo = f055001.PACKAGE_BOX_NO,
				BoxNum = string.IsNullOrWhiteSpace(f055001.BOX_NUM) ? sugBoxNo : f055001.BOX_NUM,
				ProcFlag = "0",
				Details = f055002List.GroupBy(x => new
				{
					x.DC_CODE,
					x.GUP_CODE,
					x.CUST_CODE,
					x.WMS_ORD_NO,
					x.PACKAGE_BOX_NO,
					x
				.ORD_SEQ,
					x.ITEM_CODE
				}).Select(x => new ShipOrderInsertDetail
				{
					ItemSeq = x.Key.ORD_SEQ,
					ItemCode = x.Key.ITEM_CODE,
					OutQty = Convert.ToInt32(x.Sum(z => z.PACKAGE_QTY)),
					SnList = x.Where(z => !string.IsNullOrWhiteSpace(z.SERIAL_NO)).Select(z => z.SERIAL_NO).ToArray()
				}).ToList()
			};
			return res;
		}

		/// <summary>
		/// 處理出貨單宅單資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="apiResData"></param>
		private void ProcessData(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo, LmsDataResult apiResData, string isScanBox, string SugBoxNo, F055001 f055001 = null)
		{
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			string transportCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(apiResData, "TransportCode"));

			#region 更新 F055001
			if (f055001 == null)
				f055001 = f055001Repo.AsForUpdate().Find(o => o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
					o.CUST_CODE == custCode &&
					o.WMS_ORD_NO == wmsOrdNo &&
					o.PACKAGE_BOX_NO == packageBoxNo);

			if (f055001 != null)
			{
				f055001.PAST_NO = transportCode;
        f055001.ORG_PAST_NO = transportCode;
				f055001.PRINT_FLAG = 1;
				f055001.PRINT_DATE = DateTime.Now;
				f055001.IS_CLOSED = "1";
        if (string.IsNullOrWhiteSpace(f055001.BOX_NUM))
        {
          f055001.BOX_NUM = SugBoxNo;
        }
        f055001.ORG_BOX_NUM = f055001.BOX_NUM;
        f055001.LOGISTIC_CODE = apiResData.TransportProvider;
        f055001.ORG_LOGISTIC_CODE= apiResData.TransportProvider;
        f055001Repo.Update(f055001);
			}
			#endregion

			#region 新增 F050901
			f050901Repo.Add(new F050901
			{
				CONSIGN_NO = transportCode,
				WMS_NO = wmsOrdNo,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				STATUS = "0",
				CUST_EDI_STATUS = "0",
				DISTR_EDI_STATUS = "0",
				CUST_EDI_QTY = 0,
				DISTR_USE = "01",
				DISTR_SOURCE = "0",
				DELIVID_SEQ_NAME = Convert.ToString(DataCheckHelper.GetRequireColumnValue(apiResData, "TransportProvider")),
				BOXQTY = 1
			}, "CONSIGN_ID");
			#endregion
		}

		/// <summary>
		/// 取得傳入資料 (取消)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		private ShipOrderInsertReq GetCancelRequestData(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var res = new ShipOrderInsertReq { DcCode = dcCode, CustCode = custCode };
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f050101Repo = new F050101Repository(Schemas.CoreSchema);

			var ordNo = f05030101Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.WMS_ORD_NO == wmsOrdNo).Select(x => x.ORD_NO).FirstOrDefault();

			var f050101 = f050101Repo.Find(o => o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.ORD_NO == ordNo);


			if (f050101 != null)
			{
				res.Packages = new ShipOrderInsertPackage
				{
					CustOrdNo = f050101.CUST_ORD_NO,
					OrderType = "SO",
					WmsNo = wmsOrdNo,
					BoxNo = null,
					BoxNum = null,
					ProcFlag = "D"
				};
			}

			return res;
		}
		#endregion
	}
}
