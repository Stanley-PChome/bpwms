using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckSnapshotStocksReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();
		private CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
		private CommonService commonService = new CommonService();

		#region 庫存快照檢核
		/// <summary>
		/// 檢查[調整單狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSrcSystem(List<ApiResponse> res, SnapshotStocksReceiptReq receipt, List<string> srcSystems)
		{
			if (!srcSystems.Contains(receipt.SrcSystem))
				res.Add(new ApiResponse { ErrorColumn = "SrcSystem", MsgCode = "20032", MsgContent = string.Format(tacService.GetMsg("20032"), "對接系統") });
		}

		/// <summary>
		/// 檢查[總頁數]是否小於等於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckTotalPage(List<ApiResponse> res, SnapshotStocksReceiptReq receipt)
		{
			if (receipt.TotalPage <= 0)
				res.Add(new ApiResponse { ErrorColumn = "TotalPage", MsgCode = "20038", MsgContent = string.Format(tacService.GetMsg("20038"), "總頁數") });
		}

		/// <summary>
		/// 檢查[當前頁]是否小於等於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckCurrentPage(List<ApiResponse> res, SnapshotStocksReceiptReq receipt)
		{
			if (receipt.CurrentPage <= 0)
				res.Add(new ApiResponse { ErrorColumn = "CurrentPage", MsgCode = "20038", MsgContent = string.Format(tacService.GetMsg("20038"), "當前頁") });
		}

		/// <summary>
		/// 檢查[當前頁]是否大於[總頁數]
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckCurrentPageExceedTotalPage(List<ApiResponse> res, SnapshotStocksReceiptReq receipt)
		{
			if (receipt.CurrentPage > receipt.TotalPage)
			{
				res.Add(new ApiResponse { ErrorColumn = "TotalPage", MsgCode = "20037", MsgContent = string.Format(tacService.GetMsg("20037"), "當前頁", "總頁數") });
				res.Add(new ApiResponse { ErrorColumn = "CurrentPage", MsgCode = "20037", MsgContent = string.Format(tacService.GetMsg("20037"), "當前頁", "總頁數") });
			}
		}
		#endregion

		#region 庫存快照明細檢核

		/// <summary>
		/// 檢查[倉別編號]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="index"></param>
		public void CheckZoneCode(List<ApiResponse> res, string dcCode, SnapshotStocksReceiptSkuModel sku, int index)
		{
			if (!commonService.CheckZoneCodeExist(dcCode, sku.ZoneCode))
				res.Add(new ApiResponse { No = Convert.ToString(index + 1), ErrorColumn = "ZoneCode", MsgCode = "20092", MsgContent = $"[第{index + 1}筆明細]{tacService.GetMsg("20092")}" });
		}

		/// <summary>
		/// 檢查[貨主編號]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="index"></param>
		public void CheckOwnerCode(List<ApiResponse> res, SnapshotStocksReceiptSkuModel sku, int index)
		{
			var result = new ApiResult { IsSuccessed = true };
			ctaService.CheckOwnerCode(ref result, sku);
			if (!result.IsSuccessed)
				res.Add(new ApiResponse { No = Convert.ToString(index + 1), ErrorColumn = "OwnerCode", MsgCode = result.MsgCode, MsgContent = $"[第{index + 1}筆明細]{result.MsgContent}" });
		}

		/// <summary>
		/// 檢查[商品等級]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="index"></param>
		public void CheckSkuLevel(List<ApiResponse> res, SnapshotStocksReceiptSkuModel sku, int index)
		{
			List<int> skuLeves = new List<int> { 0, 1 };
			if (!skuLeves.Contains(Convert.ToInt32(sku.SkuLevel)))
				res.Add(new ApiResponse { No = Convert.ToString(index + 1), ErrorColumn = "SkuLevel", MsgCode = "20032", MsgContent = $"[第{index + 1}筆明細]" + string.Format(tacService.GetMsg("20032"), "商品等級") });
		}

		/// <summary>
		/// 檢查[品號]是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="skuDatas"></param>
		/// <param name="sku"></param>
		/// <param name="index"></param>
		public void CheckSkuCode(List<ApiResponse> res, List<WcsSkuCodeModel> skuDatas, SnapshotStocksReceiptSkuModel sku, int index)
		{
			var gupCode = commonService.GetGupCode(sku.OwnerCode);
			if (!skuDatas.Where(x => x.OwnerCode == sku.OwnerCode && x.SkuCode == sku.SkuCode && x.GupCode == gupCode).Any())
				res.Add(new ApiResponse { No = Convert.ToString(index + 1), ErrorColumn = "SkuCode", MsgCode = "20030", MsgContent = $"[第{index + 1}筆明細]" + string.Format(tacService.GetMsg("20030"), sku.SkuCode) });
		}

		/// <summary>
		/// 檢查[數量]是否小於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="index"></param>
		public void CheckSkuQty(List<ApiResponse> res, SnapshotStocksReceiptSkuModel sku, int index)
		{
			if (sku.SkuQty < 0)
				res.Add(new ApiResponse { No = Convert.ToString(index + 1), ErrorColumn = "SkuQty", MsgCode = "23004", MsgContent = $"[第{index + 1}筆明細]" + tacService.GetMsg("23004") });
		}
		#endregion
	}
}
