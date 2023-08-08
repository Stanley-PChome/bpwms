using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 每日庫存快照回傳
	/// </summary>
	public class CommonShipToDebitReceiptService
	{
		#region 定義需檢核欄位、必填、型態、長度
		// 分揀出貨資訊檢核設定
		private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "DcCode",         Type = typeof(string),   MaxLength = 16, Nullable = false },
			new ApiCkeckColumnModel{  Name = "OwnerCode",      Type = typeof(string),   MaxLength = 12,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "BoxTotal",       Type = typeof(int),      MaxLength = 0,  Nullable = false },
		};

		// 分揀出貨資訊細檢核設定
		private List<ApiCkeckColumnModel> boxCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ShipCode",        Type = typeof(string),   MaxLength = 50,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ShipProvider",    Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SorterCode",      Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "PortNo",          Type = typeof(string),   MaxLength = 5, Nullable = false },
			new ApiCkeckColumnModel{  Name = "BoxCode",         Type = typeof(string),   MaxLength = 6, Nullable = true },
			new ApiCkeckColumnModel{  Name = "BoxLength",       Type = typeof(decimal),  MaxLength = 10,  Nullable = true },
			new ApiCkeckColumnModel{  Name = "BoxWidth",        Type = typeof(decimal),  MaxLength = 10, Nullable = true},
			new ApiCkeckColumnModel{  Name = "BoxHeight",       Type = typeof(decimal),  MaxLength = 10, Nullable = true },
			new ApiCkeckColumnModel{  Name = "BoxWeight",       Type = typeof(decimal),  MaxLength = 10 , Nullable = true },
			new ApiCkeckColumnModel{  Name = "CreateTime",      Type = typeof(string),  MaxLength = 19 , Nullable = false, IsDate = true}
		};
		#endregion

		#region Main Method
		private WmsTransaction _wmsTransaction;

		private List<WcsSkuCodeModel> skuDatas = new List<WcsSkuCodeModel>();

		private List<string> srcSystems = new List<string>();

		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(ShipToDebitReceiptReq req)
		{
			CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核
			// 檢核參數
			if (req == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckOwnerCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核資料筆數
			int reqTotal = req.BoxTotal != null ? Convert.ToInt32(req.BoxTotal) : 0;
			if (req.BoxList == null || (req.BoxList != null && !tacService.CheckDataCount(reqTotal, req.BoxList.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "列表筆數", reqTotal, req.BoxList.Count) };
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, ShipToDebitReceiptReq receipt)
		{
			#region 變數
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			var f060701Repo = new F060701Repository(Schemas.CoreSchema);

			var commonService = new CommonService();
			var tacService = new TransApiBaseService();
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			List<string> successedReceiptCodes = new List<string>();
			#endregion

			#region 新增資料
			receipt.BoxList.ForEach(box =>
			{

				if (!string.IsNullOrWhiteSpace(box.ShipCode) && successedReceiptCodes.Contains(box.ShipCode))
				{
					var sameWmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = box.ShipCode, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), box.ShipCode) } };
					data.AddRange(sameWmsNoErrRes);
					//return new ApiResult { IsSuccessed = false, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), box.ShipCode), Data = sameWmsNoErrRes };
				}
				else
				{
					var res1 = CheckShipToDebitReceipt(dcCode, gupCode, custCode, box);
					WmsTransaction wmsTransation = new WmsTransaction();
					if (!res1.IsSuccessed)
					{
						var currDatas = (List<ApiResponse>)res1.Data;
						data.AddRange(currDatas);
						//return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = currDatas };
					}
					else
					{
						try
						{
							// 檢核單號是否存在於F055001，且檢核單據狀態
							var f055001 = f055001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.PAST_NO == box.ShipCode);
							if (f055001 == null)
							{
								var wmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = box.ShipCode, MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), box.ShipCode) } };
								data.AddRange(wmsNoErrRes);
								// return new ApiResult { IsSuccessed = false, MsgCode = "23001", MsgContent = tacService.GetMsg("23001"), Data = (List<ApiResponse>)res1.Data };

							}
							else
							{
								#region F060701
								f060701Repo.Add(new F060701
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									SHIP_CODE = box.ShipCode,
									SHIP_PROVIDER = box.ShipProvider,
									SORTER_CODE = box.SorterCode,
									PORT_NO = box.PortNo,
									BOX_CODE = box.BoxCode,
									BOX_LENGTH = box.BoxLength,
									BOX_WIDTH = box.BoxWidth,
									BOX_HEIGHT = box.BoxHeight,
									BOX_WEIGHT = box.BoxWeight,
									CREATE_TIME = Convert.ToDateTime(box.CreateTime),
									STATUS = "0"
								});
								#endregion

								// 記錄成功的單號，用以再有同單號的不處理
								successedReceiptCodes.Add(box.ShipCode);
							}
						}
						catch (Exception ex)
						{
							data.Add(new ApiResponse { MsgCode = "99999", MsgContent = ex.StackTrace.ToString(), No = box.ShipCode });
							// return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.StackTrace.ToString() };
						}
						// return new ApiResult { IsSuccessed = true };

					}
				}
				//var currRes = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_SDB, dcCode, gupCode, custCode, "WcsShipToDebit ", receipt, () =>
				//{
					
				//});
			});
			#endregion
			
			#region 組回傳資料
			res.SuccessCnt = successedReceiptCodes.Count;
			res.FailureCnt = receipt.BoxList.Count() - successedReceiptCodes.Count;
			res.TotalCnt = receipt.BoxList.Count;

			res.IsSuccessed = !data.Any();
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"分揀出貨資訊回報",
					res.SuccessCnt,
					res.FailureCnt,
					res.TotalCnt);

			res.Data = data.Any() ? data : null;
			#endregion

			return res;
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckShipToDebitReceipt(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, receipt).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, receipt).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, receipt).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, receipt).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, receipt).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			string nullErrorMsg = tacService.GetMsg("20034");
			string formatErrorMsg = tacService.GetMsg("20035");
			string dateErrorMsg = tacService.GetMsg("20036");
			string datetimeErrorMsg = tacService.GetMsg("20033");

			#region 檢查盤點單欄位必填、最大長度
			List<string> notDateColumn = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			boxCheckColumnList.ForEach(column =>
			{
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));

				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name) });

				// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
				if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
					if (!DataCheckHelper.CheckDataIsDateTime(receipt, column.Name))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20033", MsgContent = string.Format(datetimeErrorMsg, column.Name) });
			});
			#endregion
			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			CheckShipToDebitReceiptService ciwService = new CheckShipToDebitReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 明細欄位資料檢核
			ciwService.CheckShipCodeByF055001IsExist(data, dcCode, gupCode, custCode, receipt);

			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, ShipToDebitReceiptBoxModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}