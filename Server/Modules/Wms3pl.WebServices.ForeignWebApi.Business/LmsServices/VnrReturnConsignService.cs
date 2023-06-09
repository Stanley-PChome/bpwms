using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class VnrReturnConsignService : LmsBaseService
	{

		private WmsTransaction _wmsTransaction;
		public VnrReturnConsignService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		/// <summary>
		/// 廠退出貨印單申請
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <returns></returns>
		public ExecuteResult ApplyVendorReturnOrder(string dcCode, string gupCode, string custCode, string rtnWmsNo, string deliveryWay, string allId, string sheetNum, string memo)
		{
			ExecuteResult res = new ExecuteResult { IsSuccessed = true };

			var url = "VendorReturnOrder/ShipOrder/Insert";

			// 取得傳入資料
			var lmsApiReq = GetApplyRequestData(dcCode, gupCode, custCode, rtnWmsNo, deliveryWay, allId, sheetNum, memo);
			var apiResult = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ApplyVendorReturnOrder", new { LmsApiUrl = LmsSetting.ApiUrl + url, LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode, RTN_WMS_NO = rtnWmsNo }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
				 {
#if (DEBUG)
				return LmsApiFuncTest(lmsApiReq, url);
#else
				return LmsApiFunc(lmsApiReq, url);
#endif
				 }, false);

			if (!apiResult.IsSuccessed)
			{
				res = new ExecuteResult { IsSuccessed = false, Message = apiResult.MsgContent };
			}
			else
			{
				if (deliveryWay == "1" && apiResult.Data != null)
				{
					var data = JsonConvert.DeserializeObject<List<LmsVendorReturnOrderData>>(JsonConvert.SerializeObject(apiResult.Data));
					if(data !=null && data?.Count() > 0 )
					{
						if(data[0] != null && data?[0].Details !=null && data?[0].Details.Count()>0)
						{
							res.No = data[0].Details[0].TransportCode;
						}
					}
				}
			}

			return res;
		}

		/// <summary>
		/// 回復廠退出貨資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="rtnWmsNo"></param>
		/// <returns></returns>
		public VnrReturnReq GetApplyRequestData(string dcCode, string gupCode, string custCode, string rtnWmsNo, string deliveryWay, string allId, string sheetNum, string memo)
		{
			var result = new VnrReturnReq { DcCode = dcCode, CustCode = custCode };
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);
			var f160204s = f160204Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode &&
														x.GUP_CODE == gupCode &&
														x.CUST_CODE == custCode &&
														x.RTN_WMS_NO == rtnWmsNo);

			result.Orders = new List<VnrReturnOrder>{
				new VnrReturnOrder
			{
				WmsOrdNo = rtnWmsNo,
				DeliveryType = deliveryWay =="0" ? 2 : Convert.ToInt32(deliveryWay),
				ShipProvider = deliveryWay =="0" ? null : allId,
				ShipBoxNo = Convert.ToInt32(sheetNum),
				ShipSelfMemo = string.IsNullOrWhiteSpace(memo) ? null : memo,
				VnrReturns = f160204s.Select(x=> x.CUST_ORD_NO).Distinct().Select(x=> new VnrReturnOrderDetail
				{
					CustVnrReturnNo = x
				}).ToList()
			} };

			return result;
		}

		public ExecuteLmsPdfApiResult GetVendorReturnOrderFile(string dcCode, string gupCode, string custCode, string order_no, int rePrint, int? sno)
		{
			ExecuteLmsPdfApiResult res = new ExecuteLmsPdfApiResult { IsSuccessed = true };
			var url = $"printapi/VendorReturn/{dcCode}|{order_no}";
			var lmsApiReq = new VnrReturnOrderPrintReq
			{
				MacAddr = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet).FirstOrDefault().GetPhysicalAddress().ToString(),
				Username = Current.StaffName,
				Sno = sno,
				RePrint = rePrint
			};

			var apiResult = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_GetVendorReturnOrderFile", new { LmsApiUrl = LmsSetting.ApiUrl + url, LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { OrderNo = order_no }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
			{
#if (DEBUG)
			
				return LmsApiFuncPdfTest(lmsApiReq, url);
#else
				
				return LmsApiFuncPdf(lmsApiReq, url);
#endif




			}, false);

			if (!apiResult.IsSuccessed)
			{
				res = new ExecuteLmsPdfApiResult
				{
					IsSuccessed = apiResult.IsSuccessed,
					HttpCode = apiResult.MsgCode,
					Message = apiResult.MsgContent,
					Data = ObjectToByteArray(apiResult.Data)
				};
			}
			else
			{
				res = new ExecuteLmsPdfApiResult
				{
					IsSuccessed = apiResult.IsSuccessed,
					HttpCode = apiResult.MsgCode,
					Message = apiResult.MsgContent,
					Data = ObjectToByteArray(apiResult.Data)
				};
			}

			return res;
		}

		public ExecuteLmsPdfApiResult GetVendorReturnOrderDetailFile(string dcCode, string gupCode, string custCode, string order_no, int rePrint, int? sno)
		{
			ExecuteLmsPdfApiResult res = new ExecuteLmsPdfApiResult { IsSuccessed = true };
			var lmsApiReq = new VnrReturnOrderPrintReq
			{
				MacAddr = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet).FirstOrDefault().GetPhysicalAddress().ToString(),
				Username = Current.StaffName,
				Sno = sno,
				RePrint = rePrint
			};
			var url = $"printapi/VendorReturnDetail/{dcCode}|{order_no}";
			var apiResult = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_GetVendorReturnOrderDetailFile", new { LmsApiUrl = LmsSetting.ApiUrl + url, LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { OrderNo = order_no }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
			{
#if (DEBUG)
			
				return LmsApiFuncPdfTest(lmsApiReq, url);
#else
				return LmsApiFuncPdf(lmsApiReq,url);
#endif
			}, false);

			if (!apiResult.IsSuccessed)
			{
				res = new ExecuteLmsPdfApiResult
				{
					IsSuccessed = apiResult.IsSuccessed,
					HttpCode = apiResult.MsgCode,
					Message = apiResult.MsgContent,
					Data = ObjectToByteArray(apiResult.Data)
				};
			}
			else
			{
				res = new ExecuteLmsPdfApiResult
				{
					IsSuccessed = apiResult.IsSuccessed,
					HttpCode = apiResult.MsgCode,
					Message = apiResult.MsgContent,
					Data = ObjectToByteArray(apiResult.Data)
				};
			}
			return res;
		}

		// object convert to byte[]
		private byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
}
