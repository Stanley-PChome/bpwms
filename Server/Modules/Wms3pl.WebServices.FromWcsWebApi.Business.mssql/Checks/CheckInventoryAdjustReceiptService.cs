using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckInventoryAdjustReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();

		#region 盤點調整單檢核
		/// <summary>
		/// 檢查[調整單狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckStatus(List<ApiResponse> res, InventoryAdjustReceiptReq receipt)
		{
			List<int> status = new List<int> { 1, 0 };
			if (!status.Contains(Convert.ToInt32(receipt.Status)))
				res.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = "Status", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), receipt.AdjustCode, "調整單狀態") });
		}

		/// <summary>
		/// 檢查單號是否已存在F060405
		/// </summary>
		/// <param name="res"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		public void CheckCkeckCodeByF060405IsExist(List<ApiResponse> res, string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			var f060405Repo = new F060405Repository(Schemas.CoreSchema);
			var f060405s = f060405Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.DOC_ID == receipt.AdjustCode);
			if (f060405s.Any())
				res.Add(new ApiResponse { No = receipt.AdjustCode, MsgCode = "20014", MsgContent = string.Format(tacService.GetMsg("20014"), receipt.AdjustCode) });
		}

		/// <summary>
		/// 檢核單號是否存在於F140101
		/// </summary>
		/// <param name="res"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		public void CheckCkeckCodeByF140101IsExistAndStatus(List<ApiResponse> res, string dcCode, string gupCode, string custCode, string wmsNo, InventoryAdjustReceiptReq receipt)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == wmsNo);
			if (f140101 == null)
				res.Add(new ApiResponse { No = receipt.AdjustCode, MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), receipt.AdjustCode) });
			else
			{
				if (receipt.Status != 9)
				{
					if (f140101.STATUS == "9")// 檢核單據是否已取消，若是則回傳失敗訊息23002(單據己刪除)。查詢F140101.STATUS = 9
						res.Add(new ApiResponse { No = receipt.AdjustCode, MsgCode = "23002", MsgContent = string.Format(tacService.GetMsg("23002"), receipt.AdjustCode) });
					else if (f140101.STATUS == "5") // 檢核單據是否已結案，若是則回傳失敗訊息23003(單據已結案)。查詢F140101.STATUS = 5
						res.Add(new ApiResponse { No = receipt.AdjustCode, MsgCode = "23003", MsgContent = string.Format(tacService.GetMsg("23003"), receipt.AdjustCode) });
				}
			}
		}

		/// <summary>
		/// 檢查品項數與明細筆數是否相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSkuTotalEqualDetailCnt(List<ApiResponse> res, InventoryAdjustReceiptReq receipt)
		{
			if (receipt.SkuTotal != receipt.SkuList.Count)
				res.Add(new ApiResponse { No = receipt.AdjustCode, ErrorColumn = "SkuTotal", MsgCode = "23058", MsgContent = string.Format(tacService.GetMsg("23058"), receipt.AdjustCode, receipt.SkuTotal, receipt.SkuList.Count) });
		}

		/// <summary>
		/// 檢查ZoneCode必須是目前盤點倉別
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		/// <param name="f060401"></param>
		public void CheckZoneCode(List<ApiResponse> res, InventoryAdjustReceiptReq receipt, F060404 f060404)
		{
			if (f060404.WAREHOUSE_ID != receipt.ZoneCode)
				res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "23065", MsgContent = string.Format(tacService.GetMsg("23065"), receipt.ZoneCode, f060404.WAREHOUSE_ID) });
		}
		#endregion

		#region 盤點調整明細檢核

		/// <summary>
		/// 檢查[商品等級]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="adjustCode"></param>
		/// <param name="index"></param>
		public void CheckSkuLevel(List<ApiResponse> res, InventoryAdjustReceiptSkuModel sku, string adjustCode, int index)
		{
			List<int> skuLeves = new List<int> { 0, 1 };
			if (!skuLeves.Contains(Convert.ToInt32(sku.SkuLevel)))
				res.Add(new ApiResponse { No = adjustCode, ErrorColumn = "SkuLevel", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), $"{adjustCode}第{index + 1}筆明細", "商品等級") });
		}

		/// <summary>
		/// 檢查[調整狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="adjustCode"></param>
		/// <param name="index"></param>
		public void CheckSkuStatus(List<ApiResponse> res, InventoryAdjustReceiptSkuModel sku, string adjustCode, int index)
		{
			List<int> skuStatus = new List<int> { 0, 1 };
			if (!skuStatus.Contains(Convert.ToInt32(sku.Status)))
				res.Add(new ApiResponse { No = adjustCode, ErrorColumn = "Status", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), $"{adjustCode}第{index + 1}筆明細", "調整狀態") });
		}
		#endregion

		#region 盤點調整任務單號回檔紀錄檔檢核
		public bool CheckDocExist(List<ApiResponse> res, InventoryAdjustReceiptReq receipt, string dcCode)
		{
			var isAddF075108 = false;
			var f075108Repo = new F075108Repository(Schemas.CoreSchema);
			#region 新增盤點任務單號回檔紀錄檔
			var f075108Res = f075108Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF075108 = f075108Repo.LockF075108();
				var f075108 = f075108Repo.Find(o => o.DC_CODE == dcCode && o.DOC_ID == receipt.AdjustCode, isForUpdate: true, isByCache: false);
				if (f075108 == null)
				{
					f075108 = new F075108 { DC_CODE = dcCode, DOC_ID = receipt.AdjustCode };
					f075108Repo.Add(f075108);
					isAddF075108 = true;
				}
				else
				{
					f075108 = null; // 代表F075107已存在資料
				}
				return f075108;
			});
			if (f075108Res == null)// 代表F075107已存在資料
				res.Add(new ApiResponse { No = receipt.OwnerCode, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), receipt.OwnerCode) });
			#endregion

			return isAddF075108;
		}
		#endregion
	}
}
