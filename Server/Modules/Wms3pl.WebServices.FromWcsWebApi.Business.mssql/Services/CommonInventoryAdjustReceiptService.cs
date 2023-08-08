using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 盤點調整完成結果回傳
	/// </summary>
	public class CommonInventoryAdjustReceiptService
	{
		#region 定義需檢核欄位、必填、型態、長度
		// 盤點調整單檢核設定
		private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OwnerCode",      Type = typeof(string),   MaxLength = 12, Nullable = false },
			new ApiCkeckColumnModel{  Name = "DcCode",         Type = typeof(string),   MaxLength = 16, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ZoneCode",       Type = typeof(string),   MaxLength = 5,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "AdjustCode",     Type = typeof(string),   MaxLength = 32,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "CheckCode",      Type = typeof(string),   MaxLength = 32, Nullable = false },
			new ApiCkeckColumnModel{  Name = "Status",         Type = typeof(int),      MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuTotal",       Type = typeof(int),      MaxLength = 0 , Nullable = false }
		};

		// 盤點調整明細檢核設定
		private List<ApiCkeckColumnModel> skuCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "SkuCode",       Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ExpiryDate",    Type = typeof(string),   MaxLength = 10, Nullable = false, IsDate = true },
			new ApiCkeckColumnModel{  Name = "OutBatchCode",  Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuQty",        Type = typeof(int),      MaxLength = 0 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuLevel",      Type = typeof(int),      MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "Status",				Type = typeof(int),      MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ShelfCode",     Type = typeof(string),   MaxLength = 16, Nullable = true },
			new ApiCkeckColumnModel{  Name = "BinCode",       Type = typeof(string),   MaxLength = 24, Nullable = true }
		};
		#endregion

		#region Private Property
		/// <summary>
		/// 紀錄有新增過F075108的DOC_ID，用以若檢核失敗 找出是否有新增，用以刪除
		/// </summary>
		private List<string> _IsAddF075108DocIdList = new List<string>();
		private F060404 _f060404;
		#endregion

		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(InventoryAdjustReceiptReq req)
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

			// 檢核倉庫編號 必填、是否存在
			ctaService.CheckZoneCode(ref res, req);
			if (!res.IsSuccessed)
				return res;
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, InventoryAdjustReceiptReq receipt)
		{
			#region 變數
			var res = new ApiResult();
			var wmsTransaction = new WmsTransaction();
			var tacService = new TransApiBaseService();
			var f060404Repo = new F060404Repository(Schemas.CoreSchema);
			var f060405Repo = new F060405Repository(Schemas.CoreSchema, wmsTransaction);
			var f060406Repo = new F060406Repository(Schemas.CoreSchema, wmsTransaction);
			var f075108Repo = new F075108Repository(Schemas.CoreSchema, wmsTransaction);
			#endregion

			#region 檢核
			// 檢查有無盤點調整任務派發作業內
			_f060404 = f060404Repo.GetDatasByTrueAndCondition(o =>
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.DOC_ID == receipt.AdjustCode).FirstOrDefault();
			if (_f060404 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20013", MsgContent = string.Format(tacService.GetMsg("20013"), receipt.AdjustCode) };

			// 資料檢查
			var res1 = CheckInventoryAdjustReceipt(dcCode, gupCode, custCode, _f060404.WMS_NO, receipt);
			if (!res1.IsSuccessed)
			{
				if (_IsAddF075108DocIdList.Contains(receipt.AdjustCode))
				{
					f075108Repo.DelF075108ByKey(dcCode, receipt.AdjustCode);
					_IsAddF075108DocIdList = _IsAddF075108DocIdList.Where(x => x != receipt.AdjustCode).ToList();
				}
				wmsTransaction.Complete();
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = (List<ApiResponse>)res1.Data };
			}				
			#endregion

			#region 新增資料

			#region F060405
			f060405Repo.Add(new F060405
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WAREHOUSE_ID = receipt.ZoneCode,
				DOC_ID = _f060404.DOC_ID,
				WMS_NO = _f060404.WMS_NO,
				CHECK_CODE = receipt.CheckCode,
				STATUS = "0",
				M_STATUS = receipt.Status.ToString(),
				SKUTOTAL = receipt.SkuTotal
			});
			#endregion

			#region F060406
			f060406Repo.BulkInsert(receipt.SkuList.Select(sku => new F060406
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WAREHOUSE_ID = receipt.ZoneCode,
				DOC_ID = _f060404.DOC_ID,
				WMS_NO = _f060404.WMS_NO,
				SKUCODE = sku.SkuCode,
				SKUQTY = Convert.ToInt32(sku.SkuQty),
				EXPIRYDATE = sku.ExpiryDate,
				OUTBATCHCODE = sku.OutBatchCode,
				SKULEVEL = sku.SkuLevel,
				SHELFCODE = sku.ShelfCode,
				BINCODE = sku.BinCode,
				STATUS = sku.Status.ToString()
			}).ToList());
			#endregion

			wmsTransaction.Complete();

			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = tacService.GetMsg("10001") };
			#endregion
		}


		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckInventoryAdjustReceipt(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, wmsNo, receipt).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, wmsNo, receipt).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, wmsNo, receipt).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, wmsNo, receipt).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, wmsNo, receipt).Data);
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
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			string nullErrorMsg = tacService.GetMsg("20058");
			string formatErrorMsg = tacService.GetMsg("20059");
			string dateErrorMsg = tacService.GetMsg("20075");

			#region 檢查盤點單欄位必填、最大長度
			List<string> notDateColumn = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			receiptCheckColumnList.ForEach(column =>
			{
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));

				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = column.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.AdjustCode, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = column.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, receipt.AdjustCode, column.Name) });
			});

			if (receipt.SkuList == null || (receipt.SkuList != null && !receipt.SkuList.Any()))
				data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = "SkuList", MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.AdjustCode, "SkuList") });
			#endregion

			#region 檢查明細欄位必填、最大長度
			if (receipt.SkuList != null && receipt.SkuList.Any())
			{
				for (int i = 0; i < receipt.SkuList.Count; i++)
				{
					var currSku = receipt.SkuList[i];

					skuCheckColumnList.ForEach(sku =>
					{
						var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(currSku, sku.Name));

						// 必填
						if (!sku.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currSku, sku.Name))
								data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = sku.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.AdjustCode}第{i + 1}筆明細", sku.Name) });

						// 最大長度
						if (sku.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currSku, sku.Name, sku.MaxLength))
								data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = sku.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.AdjustCode}第{i + 1}筆明細", sku.Name) });

						// 判斷是否為日期格式(yyyy/MM/dd)字串
						if (sku.IsDate && !string.IsNullOrWhiteSpace(value))
							if (!DataCheckHelper.CheckDataIsDate(currSku, sku.Name))
								data.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = sku.Name, MsgCode = "20075", MsgContent = string.Format(dateErrorMsg, $"{receipt.AdjustCode}第{i + 1}筆明細", sku.Name) });
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
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			CheckInventoryAdjustReceiptService ciwService = new CheckInventoryAdjustReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 盤點調整單欄位資料檢核
			// 檢查資料庫任務單號是否存在
			var isAdd = ciwService.CheckDocExist(data, receipt, dcCode);
			if (isAdd)
				_IsAddF075108DocIdList.Add(receipt.AdjustCode);

			// 檢查盤點調整單狀態
			ciwService.CheckStatus(data, receipt);

			// 檢查單號是否已存在F060405
			ciwService.CheckCkeckCodeByF060405IsExist(data, dcCode, gupCode, custCode, wmsNo, receipt);

			// 檢核單號是否存在於F140101，且檢核單據狀態
			ciwService.CheckCkeckCodeByF140101IsExistAndStatus(data, dcCode, gupCode, custCode, wmsNo, receipt);

			// 檢查品項數與明細筆數是否相同
			ciwService.CheckSkuTotalEqualDetailCnt(data, receipt);

			// 檢查ZoneCode必須是目前盤點倉別
			ciwService.CheckZoneCode(data, receipt, _f060404);
			#endregion

			#region 明細欄位資料檢核

			for (int index = 0; index < receipt.SkuList.Count; index++)
			{
				var sku = receipt.SkuList[index];

				// 檢查[商品等級]是否設定錯誤
				ciwService.CheckSkuLevel(data, sku, receipt.AdjustCode, index);

				// 檢查[調整狀態]是否設定錯誤
				ciwService.CheckSkuStatus(data, sku, receipt.AdjustCode, index);
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
