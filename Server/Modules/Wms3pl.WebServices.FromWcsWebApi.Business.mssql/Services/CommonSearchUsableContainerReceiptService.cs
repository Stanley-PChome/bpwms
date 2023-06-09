using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
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
	public class CommonSearchUsableContainerReceiptService
	{
		#region 定義需檢核欄位、必填、型態、長度
		// 容器釋放狀態設定
		private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OwnerCode",      Type = typeof(string),   MaxLength = 12,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "DcCode",         Type = typeof(string),   MaxLength = 16, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ContainerTotal", Type = typeof(string),   MaxLength = 5 , Nullable = false }
		};

		// 容器釋放狀態明細檢核設定
		private List<ApiCkeckColumnModel> containerCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ContainerCode",  Type = typeof(string),   MaxLength = 32,  Nullable = false },
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
		public ApiResult RecevieApiDatas(SearchUsableContainerReceiptReq req)
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
			int reqTotal = req.ContainerTotal != null ? Convert.ToInt32(req.ContainerTotal) : 0;
			if (req.ContainerList == null || (req.ContainerList != null && !tacService.CheckDataCount(reqTotal, req.ContainerList.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "容器釋放狀態", reqTotal, req.ContainerList.Count) };

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			#endregion

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptReq receipt)
		{
			#region 變數
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var f060202Repo = new F060202Repository(Schemas.CoreSchema);

			var commonService = new CommonService();
			var tacService = new TransApiBaseService();
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			List<string> successedReceiptCodes = new List<string>();
			#endregion


			receipt.ContainerList.ForEach(container => {
				if (!string.IsNullOrWhiteSpace(container.ContainerCode) && successedReceiptCodes.Contains(container.ContainerCode))
				{
					var sameWmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = container.ContainerCode, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), container.ContainerCode) } };
					data.AddRange(sameWmsNoErrRes);
					//return new ApiResult { IsSuccessed = false, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), container.ContainerCode), Data = sameWmsNoErrRes };
				}
				else
				{
					var res1 = CheckSearchUsableContainerReceipt(dcCode, gupCode, custCode, container);
					WmsTransaction wmsTransation = new WmsTransaction();
					if (!res1.IsSuccessed)
					{
						var currDatas = (List<ApiResponse>)res1.Data;
						data.AddRange(currDatas);
					}
					else
					{
						try
						{
							// 檢核容器編號是否存在於F0701
							var f0701 = f0701Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode &&
																				x.CUST_CODE == custCode &&
																				x.CONTAINER_CODE == container.ContainerCode).FirstOrDefault();
              
              //檢查是否有收到出庫任務回傳API，但尚未執行匯入排程的狀態
              var hasJobUnprocess = f060202Repo.CheckUnporcessContainerData(dcCode, container.ContainerCode);

              if (f0701 == null && hasJobUnprocess)
              {
                data.Add(new ApiResponse { MsgCode="10004", MsgContent = tacService.GetMsg("10004"), No = container.ContainerCode, Status = 0 });
							}
							else
							{
								data.Add(new ApiResponse { MsgCode = "10004", MsgContent = tacService.GetMsg("10004"), No = container.ContainerCode, Status = 1 });
							}
							successedReceiptCodes.Add(container.ContainerCode);
						}
						catch (Exception ex)
						{
							data.Add(new ApiResponse { MsgCode = "99999", MsgContent = ex.StackTrace.ToString(), No = container.ContainerCode, Status = 2 });
							//return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.StackTrace.ToString() };
						}
					}
				}
			});

			#region 組回傳資料
			res.SuccessCnt = successedReceiptCodes.Count;
			res.FailureCnt = receipt.ContainerList.Count() - successedReceiptCodes.Count;
			res.TotalCnt = receipt.ContainerList.Count;

			res.IsSuccessed = true;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"容器釋放狀態",
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
		private ApiResult CheckSearchUsableContainerReceipt(string dcCode,string gupCode,string custCode, SearchUsableContainerReceiptContainerModel receipt)
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
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptContainerModel receipt)
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
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptContainerModel receipt)
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
			#endregion

			#region 檢查明細欄位必填、最大長度
			containerCheckColumnList.ForEach(column =>
			{
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));

				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name), Status = 2 });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name), Status = 2 });

				// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
				if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
					if (!DataCheckHelper.CheckDataIsDateTime(receipt, column.Name))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20033", MsgContent = string.Format(datetimeErrorMsg, column.Name), Status = 2 });
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
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptContainerModel receipt)
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
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptContainerModel receipt)
		{
			CheckSnapshotStocksReceiptService ciwService = new CheckSnapshotStocksReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);

			#region 明細欄位資料檢核
			

			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, SearchUsableContainerReceiptContainerModel receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
