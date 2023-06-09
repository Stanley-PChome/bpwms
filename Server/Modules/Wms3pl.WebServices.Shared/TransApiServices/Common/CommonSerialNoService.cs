using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	public class CommonSerialNoService
	{
		#region 定義需檢核欄位、必填、型態、長度
		private List<ApiCkeckColumnModel> serialNoCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "DcCode",    Type = typeof(string),   MaxLength = 3,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "CustCode",  Type = typeof(string),   MaxLength = 12, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ItemCode",  Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SearchType",Type = typeof(string),   MaxLength = 1,  Nullable = false }
		};
		#endregion
		

		/// <summary>
		/// 商品序號查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult GetItemSerials(GetItemSerialsReq req)
		{
			var commonService = new CommonService();
			var tacService = new TransApiBaseService();
			var gupCode = commonService.GetGupCode(req.CustCode);

			#region 資料檢核
			var chkRes = CheckParam(req.DcCode, gupCode, req.CustCode, req);
			if (!chkRes.IsSuccessed)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = chkRes.Data };
			#endregion

			#region 取得資料
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			var status = string.Empty;
			var searchTypeList = new List<SearchTypeModel> {
				new SearchTypeModel{ SearchType = "1", STATUS = "A1" },
				new SearchTypeModel{ SearchType = "2", STATUS = "C1" },
				new SearchTypeModel{ SearchType = "3", STATUS = "D2" }
			};
			
			if (req.SearchType != "0")
				status = searchTypeList.Where(x => x.SearchType == req.SearchType).First().STATUS;

			var f2501s = f2501Repo.GetSnByItemStatus(gupCode, req.CustCode, req.ItemCode, status).ToList();

			var data = (from A in f2501s
									join B in searchTypeList
									on A.STATUS equals B.STATUS
									select new
									{
										A.SERIAL_NO,
										B.SearchType
									}).GroupBy(x => x.SearchType)
								 .Select(x => new GetItemSerialsRes
								 {
									 SearchType = x.Key,
									 SnList = x.Select(z => z.SERIAL_NO).ToList()
								 }).OrderBy(x => x.SearchType).ToList();

			return new ApiResult { IsSuccessed = true, MsgCode = "10004", MsgContent = tacService.GetMsg("10004"), Data = data };
			#endregion
		}

		#region Protected 檢核
		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		private ApiResult CheckParam(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, req).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, req).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, req).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, req).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, req).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}

		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
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
		/// <param name="req"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 檢查廠商退貨單欄位必填、最大長度
			List<string> isNullList = new List<string>();
			List<string> isExceedMaxLenthList = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			serialNoCheckColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(req, column.Name))
						isNullList.Add(column.Name);
				}

				// 最大長度
				if (column.MaxLength > 0)
				{
					if (!DataCheckHelper.CheckDataMaxLength(req, column.Name, column.MaxLength))
						isExceedMaxLenthList.Add(column.Name);
				}
			});

			// 必填訊息
			if (isNullList.Any())
				data.Add(new ApiResponse { MsgCode = "20034", MsgContent = string.Format(tacService.GetMsg("20034"), string.Join("、", isNullList)) });

			// 最大長度訊息
			if (isExceedMaxLenthList.Any())
				data.Add(new ApiResponse { MsgCode = "20035", MsgContent = string.Format(tacService.GetMsg("20035"), string.Join("、", isExceedMaxLenthList)) });
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
		/// <param name="req"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
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
		/// <param name="req"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
		{
			CheckTransApiService ctaService = new CheckTransApiService();
			CheckSerialNoService cvrService = new CheckSerialNoService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 檢核物流中心 必填、是否存在
			var chkDcRes = new ApiResult { IsSuccessed = true };
			ctaService.CheckDcCode(ref chkDcRes, req);
			if (!chkDcRes.IsSuccessed)
				data.Add(new ApiResponse { MsgCode = chkDcRes.MsgCode, MsgContent = chkDcRes.MsgContent, ErrorColumn = "DcCode" });

			// 檢核貨主編號 必填、是否存在
			var chkCustRes = new ApiResult { IsSuccessed = true };
			ctaService.CheckCustCode(ref chkCustRes, req);
			if (!chkCustRes.IsSuccessed)
				data.Add(new ApiResponse { MsgCode = chkCustRes.MsgCode, MsgContent = chkCustRes.MsgContent, ErrorColumn = "CustCode" });

			// 檢核品號是否存在
			cvrService.CheckItemCode(ref data, req, gupCode);

			// 檢核搜尋的序號狀態
			cvrService.CheckSearchType(ref data, req);

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, GetItemSerialsReq req)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
