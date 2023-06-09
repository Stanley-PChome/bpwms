using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
	/// 每容器位置回報
	/// </summary>
	public class CommonContainerPositionReceiptService
	{
		#region 定義需檢核欄位、必填、型態、長度
		// 容器位置回報檢核設定
		private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OwnerCode",			Type = typeof(string),   MaxLength = 12,	Nullable = false },
			new ApiCkeckColumnModel{  Name = "DcCode",				Type = typeof(string),   MaxLength = 10,	Nullable = false },
			new ApiCkeckColumnModel{  Name = "ContainerCode",		Type = typeof(string),   MaxLength = 32,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "ContainerType",		Type = typeof(string),   MaxLength = 2,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "PositionCode",		Type = typeof(string),   MaxLength = 32,	Nullable = false },
			new ApiCkeckColumnModel{  Name = "TargetPosCode",       Type = typeof(string),   MaxLength = 32,	Nullable = false },
			new ApiCkeckColumnModel{  Name = "CreateTime",			Type = typeof(string),   MaxLength = 19,	Nullable = false ,		IsDateTime = true},
			new ApiCkeckColumnModel{  Name = "OriOrderCode",		Type = typeof(string),   MaxLength = 32,	Nullable = false },
			new ApiCkeckColumnModel{  Name = "BoxTotal",			Type = typeof(int),      MaxLength = 0,		Nullable = false },
			new ApiCkeckColumnModel{  Name = "BoxSerial",			Type = typeof(int),      MaxLength = 0,		Nullable = false }
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
		public ApiResult RecevieApiDatas(ContainerPositionReceiptReq req)
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
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			// 資料處理1
			return ProcessApiDatas(gupCode, req);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string gupCode, ContainerPositionReceiptReq receipt)
		{
			#region 變數
			if (_wmsTransaction == null)
				_wmsTransaction = new WmsTransaction();
			var f060208Repo = new F060208Repository(Schemas.CoreSchema);
			var commonService = new CommonService();
			var tacService = new TransApiBaseService();
			F060208 f060208 = new F060208();
			#endregion

			#region 檢核
			// 資料檢查
			var res1 = CheckSnapshotStocksReceipt(gupCode,receipt);
			if (!res1.IsSuccessed)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = (List<ApiResponse>)res1.Data };
			#endregion

			var f60208Res = f060208Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				f060208 = f060208Repo.AsForUpdate().GetF060208ExistData(receipt.DcCode, gupCode, receipt.OwnerCode, receipt.ContainerCode, receipt.OriOrderCode, receipt.PositionCode).FirstOrDefault();
				
				if (f060208 == null)
				{
					var f060208s = f060208Repo.GetSameContainerStatus(receipt.DcCode, gupCode, receipt.OwnerCode, receipt.ContainerCode, receipt.OriOrderCode, 0).ToList();
					f060208s.ForEach(x => { x.PROC_FLAG = 9; });
					f060208Repo.BulkUpdate(f060208s);

					f060208Repo.Add(new F060208
					{
						DC_CODE = receipt.DcCode,
						GUP_CODE = gupCode,
						CUST_CODE = receipt.OwnerCode,
						CONTAINER_CODE = receipt.ContainerCode,
						CONTAINER_TYPE = receipt.ContainerType,
						POSITION_CODE = receipt.PositionCode,
						TARGET_POS_CODE = receipt.TargetPosCode,
						CREATE_TIME = Convert.ToDateTime(receipt.CreateTime),
						ORI_ORDER_CODE = receipt.OriOrderCode,
						BOX_TOTAL = Convert.ToInt32(receipt.BoxTotal),
						BOX_SERIAL = Convert.ToInt32(receipt.BoxSerial),
						PROC_FLAG = receipt.PositionCode != receipt.TargetPosCode ? 0 : 1
					});
				}
				return f060208;
			});

			
			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = tacService.GetMsg("10001")};
			
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="currentPage"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckSnapshotStocksReceipt(string gupCode,ContainerPositionReceiptReq receipt)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, receipt).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, receipt).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, receipt).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode,receipt).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, receipt).Data);
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
		protected ApiResult CheckDefaultSetting(string gupCode, ContainerPositionReceiptReq receipt)
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
		protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, ContainerPositionReceiptReq receipt)
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
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name) });

				// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
				if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
					if (!DataCheckHelper.CheckDataIsDateTime(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = column.Name, MsgCode = "20033", MsgContent = string.Format(datetimeErrorMsg, column.Name) });
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
		protected ApiResult CheckCustomColumnType(string gupCode, ContainerPositionReceiptReq receipt)
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
		protected ApiResult CheckCommonColumnData(string gupCode, ContainerPositionReceiptReq receipt)
		{
			CheckContainerPositionReceiptService ciwService = new CheckContainerPositionReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 容器位置回報資料檢核
			// 檢查出貨單號是否存在
			ciwService.CheckOriOrderCodeExist(data, gupCode, receipt);

			//檢查容器總箱數必須大於0
			ciwService.CheckBoxTotal(data, gupCode, receipt);

			//容器箱序必須大於0
			ciwService.CheckBoxSerial(data, gupCode, receipt);

			//容器箱序不可大於容器總箱數
			ciwService.CheckBoxSerialExceedBoxTal(data, gupCode, receipt);
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string gupCode, ContainerPositionReceiptReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
