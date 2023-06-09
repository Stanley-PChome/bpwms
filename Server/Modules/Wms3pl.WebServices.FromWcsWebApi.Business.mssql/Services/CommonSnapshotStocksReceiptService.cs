using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 每日庫存快照回傳
	/// </summary>
	public class CommonSnapshotStocksReceiptService
	{
		#region 定義需檢核欄位、必填、型態、長度
		// 庫存快照單檢核設定
		private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "DcCode",         Type = typeof(string),   MaxLength = 16, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SrcSystem",      Type = typeof(string),   MaxLength = 1,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "AuditDate",      Type = typeof(string),   MaxLength = 19, Nullable = false ,IsDateTime = true},
			new ApiCkeckColumnModel{  Name = "TotalPage",      Type = typeof(int),      MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "CurrentPage",    Type = typeof(int),      MaxLength = 0 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "PageSize",       Type = typeof(int),      MaxLength = 0 , Nullable = false }
		};

		// 庫存快照明細檢核設定
		private List<ApiCkeckColumnModel> skuCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ZoneCode",       Type = typeof(string),   MaxLength = 5,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ShelfCode",      Type = typeof(string),   MaxLength = 16, Nullable = true },
			new ApiCkeckColumnModel{  Name = "BinCode",        Type = typeof(string),   MaxLength = 24, Nullable = true },
			new ApiCkeckColumnModel{  Name = "OwnerCode",      Type = typeof(string),   MaxLength = 12, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuCode",        Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuLevel",       Type = typeof(int),      MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ExpiryDate",     Type = typeof(string),   MaxLength = 10, Nullable = false, IsDate = true },
			new ApiCkeckColumnModel{  Name = "OutBatchCode",   Type = typeof(string),   MaxLength = 32, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuQty",         Type = typeof(int),      MaxLength = 0 , Nullable = false }
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
		public ApiResult RecevieApiDatas(SnapshotStocksReceiptReq req)
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

			// 檢核資料筆數
			int pageSize = req.PageSize != null ? Convert.ToInt32(req.PageSize) : 0;
			int skuCnt = req.StockList == null ? 0 : req.StockList.Count;
			if (req.PageSize != null && req.StockList != null && !tacService.CheckDataCount(pageSize, skuCnt))
				return new ApiResult { IsSuccessed = false, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "庫存", pageSize, skuCnt) };
			#endregion

			// 資料處理1
			return ProcessApiDatas(req);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(SnapshotStocksReceiptReq receipt)
		{
			#region 變數
			if (_wmsTransaction == null)
				_wmsTransaction = new WmsTransaction();
			var f060601Repo = new F060601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060602Repo = new F060602Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var commonService = new CommonService();
			var tacService = new TransApiBaseService();

			srcSystems = f000904Repo.GetDatas("SRC_SYSTEM", "SRC_SYSTEM").Select(x => x.VALUE).ToList();

			// 取得品號資料，用以檢核品號是否存在
			if (receipt.StockList != null)
				skuDatas = f1913Repo.GetDatasByWcsSnapshotStocks(receipt.StockList).ToList();
			#endregion

			#region 檢核
			// 資料檢查
			var res1 = CheckSnapshotStocksReceipt(receipt);
			if (!res1.IsSuccessed)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = (List<ApiResponse>)res1.Data };
			#endregion

			#region 若已經存在下位系統的庫存原始總表，修改F060601、刪除F060602

			var calDate = Convert.ToDateTime(receipt.AuditDate).AddDays(-1).ToString("yyyy/MM/dd");

			// 取得已存在的下位系統的庫存原始總表
			var f060601 = f060601Repo.GetData(receipt.DcCode, receipt.SrcSystem, calDate, Convert.ToInt32(receipt.CurrentPage));

			if (f060601 != null && f060601.PROC_FLAG == "2")
				return new ApiResult { IsSuccessed = false, MsgCode = "20012", MsgContent = tacService.GetMsg("20012") };
			else if (f060601 != null && f060601.PROC_FLAG != "2")
			{
				// 取消F060601
				f060601.PROC_FLAG = "9";
				f060601Repo.Update(f060601);

				// 刪除F060602
				f060602Repo.DeleteData(f060601.ID);
			}
			#endregion

			#region 新增F060601、F060602
			var id = f060601Repo.GetF060601NextId();

			f060601Repo.Add(new F060601
			{
				ID = id,
				DC_CODE = receipt.DcCode,
				SRC_SYSTEM = receipt.SrcSystem,
				AUDIT_DATE = receipt.AuditDate,
				CAL_DATE = calDate,
				TOTAL_PAGE = Convert.ToInt32(receipt.TotalPage),
				CURRENT_PAGE = Convert.ToInt32(receipt.CurrentPage),
				PAGE_SIZE = Convert.ToInt32(receipt.PageSize),
				PROC_FLAG = "0"
			});
			
			f060602Repo.BulkInsert(receipt.StockList.Select(sku => new F060602
			{
				F060601_ID = id,
				WAREHOUSE_ID = sku.ZoneCode,
				SHELF_CODE = sku.ShelfCode,
				BIN_CODE = sku.BinCode,
				GUP_CODE = commonService.GetGupCode(sku.OwnerCode),
				CUST_CODE = sku.OwnerCode,
				ITEM_CODE = sku.SkuCode,
				SKU_LEVLE = Convert.ToInt32(sku.SkuLevel),
				VALID_DATE = sku.ExpiryDate,
				MAKE_NO = sku.OutBatchCode,
				QTY = Convert.ToInt32(sku.SkuQty)
			}).ToList());
			#endregion

			_wmsTransaction.Complete();

			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = tacService.GetMsg("10001") };
			
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckSnapshotStocksReceipt(SnapshotStocksReceiptReq receipt)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(receipt).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(receipt).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(receipt).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(receipt).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(receipt).Data);
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
		protected ApiResult CheckDefaultSetting(SnapshotStocksReceiptReq receipt)
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
		protected ApiResult CheckColumnNotNullAndMaxLength(SnapshotStocksReceiptReq receipt)
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
			receiptCheckColumnList.ForEach(column =>
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

			if (receipt.StockList == null || (receipt.StockList != null && !receipt.StockList.Any()))
				data.Add(new ApiResponse { ErrorColumn = "StockList", MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, "StockList") });
			#endregion

			#region 檢查明細欄位必填、最大長度
			if (receipt.StockList != null && receipt.StockList.Any())
			{
				for (int i = 0; i < receipt.StockList.Count; i++)
				{
					var index = Convert.ToString(i + 1);

					var currSku = receipt.StockList[i];

					skuCheckColumnList.ForEach(sku =>
					{
						var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(currSku, sku.Name));

						// 必填
						if (!sku.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currSku, sku.Name))
								data.Add(new ApiResponse { No = index, ErrorColumn = sku.Name, MsgCode = "20034", MsgContent = $"[第{index}筆明細]" + string.Format(nullErrorMsg, sku.Name) });

						// 最大長度
						if (sku.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currSku, sku.Name, sku.MaxLength))
								data.Add(new ApiResponse { No = index, ErrorColumn = sku.Name, MsgCode = "20035", MsgContent = $"[第{index}筆明細]" + string.Format(formatErrorMsg, sku.Name) });

						// 判斷是否為日期格式(yyyy/MM/dd)字串
						if (sku.IsDate && !string.IsNullOrWhiteSpace(value))
							if (!DataCheckHelper.CheckDataIsDate(currSku, sku.Name))
								data.Add(new ApiResponse { No = index, ErrorColumn = sku.Name, MsgCode = "20036", MsgContent = $"[第{index}筆明細]" + string.Format(dateErrorMsg, sku.Name) });
					});
				}
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(SnapshotStocksReceiptReq receipt)
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
		protected ApiResult CheckCommonColumnData(SnapshotStocksReceiptReq receipt)
		{
			CheckSnapshotStocksReceiptService ciwService = new CheckSnapshotStocksReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 庫存快照單欄位資料檢核
			// 檢查庫存快照單狀態
			ciwService.CheckSrcSystem(data, receipt, srcSystems);

			// 檢查[總頁數]是否小於等於0
			ciwService.CheckTotalPage(data, receipt);

			// 檢查[當前頁]是否小於等於0
			ciwService.CheckCurrentPage(data, receipt);

			// 檢查[當前頁]是否大於[總頁數]
			ciwService.CheckCurrentPageExceedTotalPage(data, receipt);
			#endregion

			#region 明細欄位資料檢核

			for (int index = 0; index < receipt.StockList.Count; index++)
			{
				var sku = receipt.StockList[index];

				// 檢查[倉別編號]是否設定錯誤
				ciwService.CheckZoneCode(data, receipt.DcCode, sku, index);

				// 檢查[貨主編號]是否設定錯誤
				ciwService.CheckOwnerCode(data, sku, index);

				// 檢查[商品等級]是否設定錯誤
				ciwService.CheckSkuLevel(data, sku, index);

				// 檢查[品號]是否存在
				ciwService.CheckSkuCode(data, skuDatas, sku, index);

				// 檢查[數量]是否小於0
				ciwService.CheckSkuQty(data, sku, index);
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(SnapshotStocksReceiptReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
