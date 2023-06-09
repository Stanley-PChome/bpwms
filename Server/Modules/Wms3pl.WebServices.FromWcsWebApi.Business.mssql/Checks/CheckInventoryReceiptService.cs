using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckInventoryReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();

		#region 盤點單檢核
		/// <summary>
		/// 檢查[盤點單狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckStatus(List<ApiResponse> res, InventoryReceiptReq receipt)
		{
			List<int> status = new List<int> { 3, 9 };
			if (!status.Contains(Convert.ToInt32(receipt.Status)))
				res.Add(new ApiResponse { No = receipt.CheckCode, ErrorColumn = "Status", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), receipt.CheckCode, "盤點單狀態") });
		}

		/// <summary>
		/// 檢查單號是否已存在F060402
		/// </summary>
		/// <param name="res"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		public void CheckCkeckCodeByF060402IsExist(List<ApiResponse> res, string dcCode, string gupCode, string custCode, string wmsNo, InventoryReceiptReq receipt)
		{
			var f060402Repo = new F060402Repository(Schemas.CoreSchema);
			var f060402s = f060402Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.DOC_ID == receipt.CheckCode);
			if (f060402s.Any())
				res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "20014", MsgContent = string.Format(tacService.GetMsg("20014"), receipt.CheckCode) });
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
		public void CheckCkeckCodeByF140101IsExistAndStatus(List<ApiResponse> res, string dcCode, string gupCode, string custCode, string wmsNo, InventoryReceiptReq receipt)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == wmsNo);
			if (f140101 == null)
				res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), receipt.CheckCode) });
			else
			{
				if (receipt.Status != 9)
				{
					if (f140101.STATUS == "9")// 檢核單據是否已取消，若是則回傳失敗訊息23002(單據己刪除)。查詢F140101.STATUS = 9
						res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "23002", MsgContent = string.Format(tacService.GetMsg("23002"), receipt.CheckCode) });
					else if (f140101.STATUS == "5") // 檢核單據是否已結案，若是則回傳失敗訊息23003(單據已結案)。查詢F140101.STATUS = 5
						res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "23003", MsgContent = string.Format(tacService.GetMsg("23003"), receipt.CheckCode) });
				}
			}
		}

		/// <summary>
		/// 檢查品項數與明細筆數是否相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSkuTotalEqualDetailCnt(List<ApiResponse> res, InventoryReceiptReq receipt)
		{
			if (receipt.SkuTotal != receipt.SkuList.Count)
				res.Add(new ApiResponse { No = receipt.CheckCode, ErrorColumn = "SkuTotal", MsgCode = "23058", MsgContent = string.Format(tacService.GetMsg("23058"), receipt.CheckCode, receipt.SkuTotal, receipt.SkuList.Count) });
		}

		/// <summary>
		/// 檢查ZoneCode必須是目前盤點倉別
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		/// <param name="f060401"></param>
		public void CheckZoneCode(List<ApiResponse> res, InventoryReceiptReq receipt, F060401 f060401)
		{
			if (f060401.WAREHOUSE_ID != receipt.ZoneCode)
				res.Add(new ApiResponse { No = receipt.CheckCode, MsgCode = "23065", MsgContent = string.Format(tacService.GetMsg("23065"), receipt.ZoneCode, f060401.WAREHOUSE_ID) });
		}
		#endregion

		#region 盤點明細檢核

		/// <summary>
		/// 檢查[商品等級]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="checkCode"></param>
		/// <param name="index"></param>
		public void CheckSkuLevel(List<ApiResponse> res, InventoryReceiptModel sku, string checkCode, int index)
		{
			List<int> skuLeves = new List<int> { 0, 1 };
			if (!skuLeves.Contains(Convert.ToInt32(sku.SkuLevel)))
				res.Add(new ApiResponse { No = checkCode, ErrorColumn = "SkuLevel", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), $"{checkCode}第{index + 1}筆明細", "商品等級") });
		}

		/// <summary>
		/// 檢查[實際盤點數量]是否有小於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="checkCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQty(List<ApiResponse> res, InventoryReceiptModel sku, string checkCode, int index)
		{
			if (sku.SkuQty < 0)
				res.Add(new ApiResponse { No = checkCode, ErrorColumn = "SkuQty", MsgCode = "23004", MsgContent = string.Format(tacService.GetMsg("23004"), $"{checkCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查[商品編號]是否存在F1903
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="checkCode"></param>
		/// <param name="index"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		public void CheckSkuCode(List<ApiResponse> res, InventoryReceiptModel sku, string checkCode,int index, string gupCode, string custCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == sku.SkuCode);
			if (f1903 == null) 
			{
				res.Add(new ApiResponse { No = checkCode, ErrorColumn = "SkuCode", MsgCode = "20030", MsgContent = string.Format(tacService.GetMsg("20030"), $"{checkCode}第{index + 1}筆明細",sku.SkuCode) });
			}
		}
		#endregion

		#region 盤點任務單號回檔紀錄檔檢核
		public bool CheckDocExist(List<ApiResponse> res, InventoryReceiptReq receipt, string dcCode)
		{
			var isAddF075107 = false;
			var f075107Repo = new F075107Repository(Schemas.CoreSchema);
			#region 新增盤點任務單號回檔紀錄檔
			var f075107Res = f075107Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF075107 = f075107Repo.LockF075107();
				var f075107 = f075107Repo.Find(o => o.DC_CODE == dcCode && o.DOC_ID == receipt.CheckCode, isForUpdate: true, isByCache: false);
				if (f075107 == null)
				{
					f075107 = new F075107 { DC_CODE = dcCode, DOC_ID = receipt.CheckCode };
					f075107Repo.Add(f075107);
					isAddF075107 = true;
				}
				else
				{
					f075107 = null; // 代表F075107已存在資料
				}
				return f075107;
			});
			if (f075107Res == null)// 代表F075107已存在資料
				res.Add(new ApiResponse { No = receipt.OwnerCode, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), receipt.OwnerCode) });
			#endregion

			return isAddF075107;
		}
		#endregion
	}
}
