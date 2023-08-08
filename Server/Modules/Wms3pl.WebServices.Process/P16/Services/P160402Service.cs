
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160402Service
	{
		private WmsTransaction _wmsTransaction;
		public P160402Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult SaveScrapDetails(F160401 f160401, List<F160402> f160402s)
		{
			//取得原始資料
			var F160401Repo = new F160401Repository(Schemas.CoreSchema, _wmsTransaction);
			var F160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);

			if (string.IsNullOrEmpty(f160401.SCRAP_NO))
			{
				f160401.SCRAP_NO = sharedService.GetNewOrdCode("X");
				f160401.STATUS = "0";
			}
				

			var currentF160401 = F160401Repo.Find(x => x.DC_CODE == f160401.DC_CODE && x.GUP_CODE == f160401.GUP_CODE &&
																						 x.CUST_CODE == f160401.CUST_CODE && x.SCRAP_NO == f160401.SCRAP_NO);
			var currentF160402s = F160402Repo.AsForUpdate().GetF160402(f160401.DC_CODE, f160401.GUP_CODE, f160401.CUST_CODE, f160401.SCRAP_NO).ToList();

			var tmpF160401 = currentF160401 ?? f160401;
			//檢核資料
			var errorMsg = ValidData(tmpF160401, f160402s);

			if (!String.IsNullOrEmpty(errorMsg))
				return new ExecuteResult { Message = errorMsg };

			//若查無currentF160401-新增F160401
			if (currentF160401 == null)
			{
				F160401Repo.Add(f160401);
			}
			else
			{
				currentF160401.UPD_DATE = DateTime.Now;
				F160401Repo.Update(currentF160401);
			}

			//取得最大序號
			var scrapSeq = (Int16)F160402Repo.GetMaxSeq(f160401.DC_CODE, f160401.GUP_CODE, f160401.CUST_CODE, f160401.SCRAP_NO);

			if (currentF160402s == null || !currentF160402s.Any())
			{
				//新增
				if (f160402s == null || !f160402s.Any())
					return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160402Service_NoScrapDetailToUpdate };
				else
				{
					foreach (var item in f160402s)
					{
						scrapSeq++;
						item.SCRAP_NO = f160401.SCRAP_NO;
						AddF160402(item, scrapSeq);
					}
				}
			}
			else
			{
				//更新
				foreach (var item in currentF160402s)
				{
					var editF160402 = f160402s.Find(x => x.SCRAP_SEQ == item.SCRAP_SEQ);
					if (editF160402 == null)
					{
						//無對應到且原始資料有值即刪除原始資料
						F160402Repo.Delete(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE &&
							x.CUST_CODE == item.CUST_CODE && x.SCRAP_NO == item.SCRAP_NO && x.SCRAP_SEQ == item.SCRAP_SEQ);
					}
					else
					{
						//比對資料若有對應到即更新原始資料
						item.ITEM_CODE = editF160402.ITEM_CODE;
						item.SCRAP_QTY = editF160402.SCRAP_QTY;
						item.VALID_DATE = editF160402.VALID_DATE;
						item.WAREHOUSE_ID = editF160402.WAREHOUSE_ID;
						item.LOC_CODE = editF160402.LOC_CODE;
						item.SCRAP_CAUSE = editF160402.SCRAP_CAUSE;
						item.BOX_CTRL_NO = editF160402.BOX_CTRL_NO;
						item.PALLET_CTRL_NO = editF160402.PALLET_CTRL_NO;
                        item.MAKE_NO = editF160402.MAKE_NO;
						F160402Repo.Update(item);
					}
				}
				//新增
				if (f160402s != null || f160402s.Any())
				{
					//無對應到且新資料有值即新增資料
					var addF160402s = f160402s.Where(x => x.SCRAP_SEQ == 0);
					foreach(var addf160402 in addF160402s)
					{
						scrapSeq++;
						addf160402.SCRAP_NO = f160401.SCRAP_NO;
						AddF160402(addf160402, scrapSeq);
					}
				}
			}
			// 存檔成功回傳 報廢單號
			return new ExecuteResult() { IsSuccessed = true, Message = f160401.SCRAP_NO };
		}

		private void AddF160402(F160402 item, Int16 scrapSeq)
		{
			var F160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);
			item.SCRAP_SEQ = scrapSeq;
			F160402Repo.Add(item);
		}

		private string ValidData(F160401 f160401, List<F160402> f160402s)
		{
			#region 檢查報廢單
			if (f160401 != null)
			{
				switch (f160401.STATUS)
				{
					case "9":
						return Properties.Resources.P160402Service_Scrap_No_Deleted;
					case "1":
						return Properties.Resources.P160402Service_Scrap_No_Permitted;
					case "2":
						return Properties.Resources.P160402Service_Processing_Scrap_No_CannotModify;
					case "4":
						return Properties.Resources.P160402Service_Scrap_No_Close;
				}
			}
			#endregion

			#region 檢查報廢明細
			var f160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);

			if (f160402s == null || !f160402s.Any()) return string.Empty;

			

			var f160402Grops = f160402s.GroupBy(x => new
																								{
																									DC_CODE = x.DC_CODE,
																									GUP_CODE = x.GUP_CODE,
																									CUST_CODE = x.CUST_CODE,
																									ITEM_CODE = x.ITEM_CODE,
																									LOC_CODE = x.LOC_CODE,
																									WAREHOUSE_ID = x.WAREHOUSE_ID
																								})
																	.Select(x => new
																	{
																		DC_CODE = x.Key.DC_CODE,
																		GUP_CODE = x.Key.GUP_CODE,
																		CUST_CODE = x.Key.CUST_CODE,
																		ITEM_CODE = x.Key.ITEM_CODE,
																		LOC_CODE = x.Key.LOC_CODE,
																		WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
																		SCRAP_QTY = x.Sum(s => s.SCRAP_QTY)
																	}).ToList();

			foreach (var item in f160402Grops)
			{
				var results = f160402Repo.GetF160402StockSum(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ITEM_CODE, item.LOC_CODE, item.WAREHOUSE_ID)
																.ToList();
				if (results == null || !results.Any())
					return string.Format(Properties.Resources.P160402Service_Scrap_No_Stock_Sum, 
															item.ITEM_CODE,
															item.LOC_CODE,
															item.WAREHOUSE_ID);
				//其他待處理報廢單報廢數
				var alreadyScrapAllQty = f160402Repo.GetF160402ScrapSum(item.DC_CODE, item.GUP_CODE, item.CUST_CODE,
																		f160401.SCRAP_NO, item.ITEM_CODE, item.LOC_CODE, item.WAREHOUSE_ID);
				//庫存總數
				var stockSum = results.FirstOrDefault();
				//本次報廢數 > (庫存總數 - 其他待處理報廢單報廢數)
				if (item.SCRAP_QTY > (stockSum.ALL_QTY - alreadyScrapAllQty))
					return string.Format(Properties.Resources.P160402Service_Scrap_No_Stock_Sum_Insufficient,
															item.ITEM_CODE,
															item.LOC_CODE,
															item.WAREHOUSE_ID);
			}
			#endregion

			return string.Empty;
		}

		public decimal GetF160402ScrapSum(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
		{
			var F160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);
			return F160402Repo.GetF160402ScrapSum(dcCode, gupCode, custCode, scrapNo, itemCode, locCode, wareHouseId);
		}

        public IQueryable<F160402> GetF160402ScrapData(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
        {
            var F160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);
            return F160402Repo.GetF160402ScrapData(dcCode, gupCode, custCode, scrapNo, itemCode, locCode, wareHouseId);
        }

        public IQueryable<F160402AddData> GetF160402AddScrapDetails(string dcCode, string gupCode, string custCode, string wareHouseId,
			string itemCode, string locCode, string itemName, string validDateStart, string validDateEnd)
		{
			var f160402repo = new F160402Repository(Schemas.CoreSchema);
			DateTime? validDateS = null;
			DateTime? validDateE = null;
			if (!string.IsNullOrEmpty(validDateStart))
				validDateS = Convert.ToDateTime(validDateStart);
			if (!string.IsNullOrEmpty(validDateEnd))
				validDateE = Convert.ToDateTime(validDateEnd);

			var result = f160402repo.GetF160402AddScrapDetails(dcCode, gupCode, custCode, wareHouseId, itemCode, locCode, itemName, validDateS, validDateE);
			return result;
		}

		public ExecuteResult ApproveScrapDetails(F160401 f160401, List<F160402> f160402s, List<SrcItemLocQtyItem> srcItemLocQtyItems)
		{
			var F160401Repo = new F160401Repository(Schemas.CoreSchema, _wmsTransaction);
			var F160402Repo = new F160402Repository(Schemas.CoreSchema, _wmsTransaction);

			// 存檔
			var result = SaveScrapDetails(f160401, f160402s);
			if (!result.IsSuccessed) return result;

			// 核准
			if (f160401 == null || string.IsNullOrEmpty(f160401.SCRAP_NO))
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P160402Service_Create_Scrap_No_First };
			
			var approveF160401 = F160401Repo.Find(x => x.DC_CODE == f160401.DC_CODE && x.GUP_CODE == f160401.GUP_CODE &&
																								 x.CUST_CODE == f160401.CUST_CODE && x.SCRAP_NO == f160401.SCRAP_NO);
			if (approveF160401 == null)
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P160402Service_Scrao_No_NotFound };

			// 將報廢單狀態更新為"已核准"
			approveF160401.STATUS = "1";
			F160401Repo.Update(approveF160401);

			var stockService = new StockService();
			var isPass = false;
			var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");

			try
			{
				var itemCodes = srcItemLocQtyItems.Select(x => new ItemKey { DcCode = f160401.DC_CODE, GupCode = f160401.GUP_CODE, CustCode = f160401.CUST_CODE, ItemCode = x.ItemCode }).Distinct().ToList();
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫盤損商品，請稍待再試");

				// 產生調撥單
				SharedService sharedService = new SharedService(_wmsTransaction);

				List<StockFilter> stockFilters = new List<StockFilter>();

				foreach (var srcItemLocQtyItem in srcItemLocQtyItems)
				{
					StockFilter dataTemp = new StockFilter()
					{
						ItemCode = srcItemLocQtyItem.ItemCode,
						Qty = srcItemLocQtyItem.Qty,
						LocCode = srcItemLocQtyItem.LocCode,
						SrcWarehouseId = srcItemLocQtyItem.WarehouseId,
						PalletCtrlNos = new List<string>() { srcItemLocQtyItem.PalletCtrlNo },
						BoxCtrlNos = new List<string>() { srcItemLocQtyItem.BoxCtrlNo },
						MakeNos = new List<string>() { srcItemLocQtyItem.MakeNo },
						ValidDates = srcItemLocQtyItem.VALID_DATE == null ? null : new List<DateTime>() { Convert.ToDateTime(srcItemLocQtyItem.VALID_DATE) }
					};
					stockFilters.Add(dataTemp);
				}

				//調撥單資料暫存
				List<ReturnNewAllocation> allocationListTemp = new List<ReturnNewAllocation>();

				//庫存暫存
				var stockListTemp = new List<F1913>() { };

				NewAllocationItemParam allocationDataSet = new NewAllocationItemParam()
				{
					TarDcCode = f160401.DC_CODE,
					SrcDcCode = f160401.DC_CODE,
					GupCode = f160401.GUP_CODE,
					CustCode = f160401.CUST_CODE,
					SrcStockFilterDetails = stockFilters,
					SourceNo = f160401.SCRAP_NO,
					SourceType = "12",
					TarWarehouseType = "D"
				};
				var createAllocationResult = sharedService.CreateOrUpdateAllocation(allocationDataSet);

				if (!createAllocationResult.Result.IsSuccessed)
					return createAllocationResult.Result;
				else
				{
					stockListTemp = createAllocationResult.StockList;
					allocationListTemp.AddRange(createAllocationResult.AllocationList);
				}

				var allocationResult = sharedService.BulkInsertAllocation(allocationListTemp, stockListTemp);

				if (!allocationResult.IsSuccessed)
					return allocationResult;
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}


			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160402Service_Scrao_No_Permitted };
		}
	}
}

