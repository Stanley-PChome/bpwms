
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using F19 = Wms3pl.Datas.F19;
using F15 = Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.Datas.F25;
namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910101Service
	{
		private WmsTransaction _wmsTransaction;
		public P910101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public IQueryable<F910101Ex> GetF910101Datas(string gupCode, string custCode, string bomNo, string itemCode, string status, string bomType)
		{
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			return f910101Repository.GetF910101Datas(gupCode, custCode, bomNo, itemCode, status, bomType);

		}

		public IQueryable<F910102Ex> GetF910102Datas(string gupCode, string custCode, string bomNo)
		{
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			return f910101Repository.GetF910102Datas(gupCode, custCode, bomNo);
		}

		public ExecuteResult InsertF910101(F910101Ex f910101Ex, List<F910102Ex> f910102Exs, string userId)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repository = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

			f910101Repository.Add(new F910101()
			{
				BOM_NO = f910101Ex.BOM_NO,
				ITEM_CODE = f910101Ex.ITEM_CODE,
				BOM_TYPE = f910101Ex.BOM_TYPE,
				BOM_NAME = f910101Ex.BOM_NAME,
				UNIT_ID = f910101Ex.UNIT_ID,
				CHECK_PERCENT = (float)f910101Ex.CHECK_PERCENT,
				SPEC_DESC = f910101Ex.SPEC_DESC,
				PACKAGE_DESC = f910101Ex.PACKAGE_DESC,
				STATUS = f910101Ex.STATUS,
				ISPROCESS = f910101Ex.ISPROCESS,
				CUST_CODE = f910101Ex.CUST_CODE,
				GUP_CODE = f910101Ex.GUP_CODE,
				CRT_STAFF = userId,
				CRT_DATE = DateTime.Now,
				CRT_NAME = userId
			}
			);

			var f910102Repository = new F910102Repository(Schemas.CoreSchema, _wmsTransaction);

			foreach (F910102Ex p in f910102Exs)
			{
				f910102Repository.Add(new F910102()
				{
					BOM_NO = f910101Ex.BOM_NO,
					MATERIAL_CODE = p.MATERIAL_CODE,
					COMBIN_ORDER = p.COMBIN_ORDER,
					BOM_QTY = p.BOM_QTY,
					CUST_CODE = p.CUST_CODE,
					GUP_CODE = p.GUP_CODE,
					CRT_STAFF = userId,
					CRT_DATE = DateTime.Now,
					CRT_NAME = userId
				}
				);
			}

			var f1903Entity = f1903Repository.Find(item => item.GUP_CODE == f910101Ex.GUP_CODE
														&& item.CUST_CODE == f910101Ex.CUST_CODE
														&& item.ITEM_CODE == f910101Ex.ITEM_CODE);

			if (f1903Entity == null)
			{
				return new ExecuteResult() { Message = Properties.Resources.P910101Service_GOODNO_NOTEXISTCUST };
			}

			f1903Entity.MULTI_FLAG = "1";
			f1903Repository.Update(f1903Entity);

			result.IsSuccessed = true;

			return result;
		}

		public ExecuteResult DeleteF910101(string gupCode, string custCode, string bomNo, string userId)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repository = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

			F910101 f910101Data = f910101Repository.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BOM_NO == bomNo);
			if (f910101Data != null)
			{
				f910101Data.STATUS = "9";
				f910101Data.UPD_NAME = userId;
				f910101Data.UPD_STAFF = userId;
				f910101Data.UPD_DATE = DateTime.Now;
			}

			f910101Repository.Update(f910101Data);

			// 檢查所有相同成品編號的商品狀態都被取消才 MULTI_FLAG=0
			var query = f910101Repository.Filter(item =>
						item.GUP_CODE == gupCode
						&& item.CUST_CODE == custCode
						&& item.BOM_NO != bomNo
						&& item.STATUS == "0");

			// 沒有一個是使用中，表示全部都被取消
			if (!query.Any())
			{
				var f1903Entity = f1903Repository.Find(item => item.GUP_CODE == f910101Data.GUP_CODE
															&& item.CUST_CODE == f910101Data.CUST_CODE
															&& item.ITEM_CODE == f910101Data.ITEM_CODE);

				if (f1903Entity == null)
				{
					return new ExecuteResult() { Message = Properties.Resources.P910101Service_GOODNO_NOTEXISTCUST };
				}

				f1903Entity.MULTI_FLAG = "0";
				f1903Repository.Update(f1903Entity);
			}

			result.IsSuccessed = true;
			return result;
		}

		public ExecuteResult UpdateF910101(F910101Ex f910101Ex, List<F910102Ex> f910102Exs, string userId)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			F910101 f910101Data = f910101Repository.Find(x => x.GUP_CODE == f910101Ex.GUP_CODE && x.CUST_CODE == f910101Ex.CUST_CODE && x.BOM_NO == f910101Ex.BOM_NO);
			if (f910101Data != null)
			{
				f910101Data.ITEM_CODE = f910101Ex.ITEM_CODE;
				f910101Data.BOM_TYPE = f910101Ex.BOM_TYPE;
				f910101Data.BOM_NAME = f910101Ex.BOM_NAME;
				f910101Data.UNIT_ID = f910101Ex.UNIT_ID;
				f910101Data.CHECK_PERCENT = (float)f910101Ex.CHECK_PERCENT;
				f910101Data.SPEC_DESC = f910101Ex.SPEC_DESC;
				f910101Data.PACKAGE_DESC = f910101Ex.PACKAGE_DESC;
				f910101Data.STATUS = f910101Ex.STATUS;
				f910101Data.ISPROCESS = f910101Ex.ISPROCESS;
				f910101Data.UPD_STAFF = userId;
				f910101Data.UPD_DATE = DateTime.Now;
				f910101Data.UPD_NAME = userId;
				f910101Repository.Update(f910101Data);
			}
			var f910102Repository = new F910102Repository(Schemas.CoreSchema, _wmsTransaction);
			f910102Repository.Delete(x => x.BOM_NO == f910101Ex.BOM_NO);
			foreach (F910102Ex p in f910102Exs)
			{
				f910102Repository.Add(new F910102()
				{
					BOM_NO = f910101Ex.BOM_NO,
					MATERIAL_CODE = p.MATERIAL_CODE,
					COMBIN_ORDER = p.COMBIN_ORDER,
					BOM_QTY = p.BOM_QTY,
					CUST_CODE = p.CUST_CODE,
					GUP_CODE = p.GUP_CODE,
					CRT_STAFF = userId,
					CRT_DATE = DateTime.Now,
					CRT_NAME = userId
				});
			}
			result.IsSuccessed = true;
			return result;
		}

		public IQueryable<F910402Detail> GetF910402Detail(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repo = new F910402Repository(Schemas.CoreSchema);
			return repo.GetF910402Detail(dcCode, gupCode, custCode, quoteNo);
		}

		public IQueryable<F910403Detail> GetF910403Detail(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repo = new F910403Repository(Schemas.CoreSchema);
			return repo.GetF910403Detail(dcCode, gupCode, custCode, quoteNo);
		}

		public IQueryable<F910101Ex2> GetF910101Ex2(string gupCode, string custCode, string status)
		{
			var f910101Repository = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
			return f910101Repository.GetF910101Ex2(gupCode, custCode, status);
		}

		public IQueryable<BomQtyData> GetBomQtyData(string dcCode, string gupCode, string custCode, string processNo, string type = "0")
		{
			var repo = new F910102Repository(Schemas.CoreSchema);
			if (type == "0")
				return repo.GetBomQtyData(dcCode, gupCode, custCode, processNo);
			if (type == "1")
				return repo.GetBomQtyData2(dcCode, gupCode, custCode, processNo);
			return null;
		}

		/// <summary>
		/// 同 CreateF910205, 但把原本在CLIENT做的倉別庫存判斷帶回到SERVER做
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <param name="bomData"></param>
		/// <returns></returns>
		public ExecuteResult CreateF910205(string dcCode, string gupCode, string custCode, string processNo, IEnumerable<PickData> bomData)
		{
			var sharedService = new SharedService(_wmsTransaction);
			var f910206repo = new F910206Repository(Schemas.CoreSchema, _wmsTransaction);
			// 流程:
			// 0. 先更新F910201的A_PROCESS_QTY和PROC_STATUS
			// 1. 如果有加工倉的揀料, 就一定會產生PM單號, 並且新增F910205, F91020501, F1913, F1511的資料
			// 2. 檢查有沒有非加工倉的揀料, 有的話就產生T單號, 並且新增F151001, F151002, F1913, F1511

			// 1. 產生揀料單號
			string pickNo = sharedService.GetNewOrdCode("ZW");

			// 0. 更新A_PROCESS_QTY及PROC_STATUS = 1
			var repo1 = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo2 = new F910205Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo3 = new F91020501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910201 = repo1.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == processNo);
			if (f910201 == null) return new ExecuteResult() { IsSuccessed = false };
			f910201.PROC_STATUS = "1";
			repo1.Update(f910201);


			// 1.1 寫入F910205 - 揀料單主檔，無論是否有揀料單明細都要建立，因為要跟調撥單關聯
			if (bomData.Any())
			{
				var f910205 = new F910205()
				{
					PROCESS_NO = processNo,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					PICK_NO = pickNo
				};
				repo2.Add(f910205);
			}

			#region 加工倉的揀料單
			// 1. 檢查有沒有加工倉的揀料. 如果是系統自動建的揀料單, 一定會有加工倉的揀料
			var bomDataW = bomData.Where(x => x.WAREHOUSE_TYPE == "W");
			if (bomDataW.Any())
			{
				var gBomDataWs = bomDataW.GroupBy(a => new { a.WAREHOUSE_ID });
				// Pre2: 取得來源儲位商品數
				var srcItemLocQtys = new List<StockDetail>();
				foreach (var gBomDataW in gBomDataWs)
				{
					//List<SrcItemLocQty> tmpSrcItemLocQtys;
					//var itemQtys = bomDataW.Select(a => new ItemQty { ItemCode = a.ITEM_CODE, Qty = a.A_PROCESS_QTY }).ToList();
					var itemQtys = bomDataW.Select(a => new StockFilter { ItemCode = a.ITEM_CODE, Qty = a.A_PROCESS_QTY }).ToList();
					var f1913List = new List<F1913>();
					List<StockDetail> tmpSrcItemLocQtys = new List<StockDetail>();
					var exeResult = sharedService.CheckStocks(dcCode, gupCode, custCode, "W", gBomDataW.Key.WAREHOUSE_ID, null, itemQtys, f1913List, ref tmpSrcItemLocQtys);
					//var exeResult = sharedService.GetSrcItemLocQtys(dcCode, gupCode, custCode, itemQtys, "W", out tmpSrcItemLocQtys,ref f1913List, gBomDataW.Key.WAREHOUSE_ID);
					if (!exeResult.IsSuccessed)
						return exeResult;
					if (tmpSrcItemLocQtys.Any())
						srcItemLocQtys.AddRange(tmpSrcItemLocQtys);
				}

				// 1.2 寫入F91020501 - 揀料單明細.
				int pickSeq = 0;
				foreach (var p in srcItemLocQtys)
				{
					pickSeq++;
					repo3.Add(new F91020501()
					{
						PICK_NO = pickNo,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_QTY = Convert.ToInt32(p.Qty),
						ITEM_CODE = p.ItemCode,
						PICK_SEQ = pickSeq,
						PICK_LOC = p.SrcLocCode,
						VNR_CODE = p.VnrCode,
						VALID_DATE = p.ValidDate,
						SERIAL_NO = p.SerialNo,
						ENTER_DATE = p.EnterDate,
						BOX_CTRL_NO = p.BoxCtrlNo,
						PALLET_CTRL_NO = p.PalletCtrlNo,
						MAKE_NO = p.MAKE_NO
					}
					);

					// 1.3一併寫入F1511 - 虛擬儲位檔
					F15.F1511 f1511 = new F15.F1511()
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						ORDER_NO = pickNo,
						ORDER_SEQ = pickSeq.ToString(),
						A_PICK_QTY = Convert.ToInt32(p.Qty),
						B_PICK_QTY = Convert.ToInt32(p.Qty),
						STATUS = "0",
						BOX_CTRL_NO = p.BoxCtrlNo,
						PALLET_CTRL_NO = p.PalletCtrlNo,
						MAKE_NO = p.MAKE_NO,
						ENTER_DATE = p.EnterDate,
						VALID_DATE = p.ValidDate,
						ITEM_CODE = p.ItemCode,
						SERIAL_NO = p.SerialNo,
						LOC_CODE = p.SrcLocCode
					};

					// 1.4 找出該筆Material_Code的F1913資料, 更新F1913.QTY
					var updateF1913 = UpdateF1913Qty(dcCode, gupCode, custCode, p);
					if (updateF1913.IsSuccessed == false) return updateF1913;
				}
			}
			#endregion

			#region 非加工倉的揀料單
			// 2. 檢查有沒有非加工倉的揀料. 只有自已建的揀料單才會有此狀況
			var bomDataNotW = bomData.Where(x => x.WAREHOUSE_TYPE != "W");
			if (bomDataNotW.Any())
			{

				var stockService = new StockService();
				var isPass = false;
				var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
				try
				{
					var itemCodes = bomDataNotW.Select(x => new ItemKey { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, ItemCode = x.ITEM_CODE }).Distinct().ToList();
					isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
					if (!isPass)
						return new ExecuteResult(false, "仍有程序正在配庫非加工倉商品，請稍待再試");

					var gBomDataNotWs = bomDataNotW.GroupBy(a => new { a.WAREHOUSE_TYPE, a.WAREHOUSE_ID });
					var excludeLocs = new List<F1912>();

					//調撥單資料暫存
					List<ReturnNewAllocation> allocationListTemp = new List<ReturnNewAllocation>();

					//庫存暫存
					var stockListTemp = new List<F1913>() { };

					//調撥單號
					string ticketMessage = string.Empty;

					foreach (var gBomDataNotW in gBomDataNotWs)
					{
						var allocationItems = gBomDataNotW.GroupBy(a => new { a.ITEM_CODE }).Select(g => new ItemQty { ItemCode = g.Key.ITEM_CODE, Qty = g.Sum(a => a.A_PROCESS_QTY) }).ToList();

						List<StockFilter> stockFilters = new List<StockFilter>();
						foreach (var item in allocationItems)
						{
							StockFilter dataTemp = new StockFilter() { ItemCode = item.ItemCode, Qty = item.Qty };
							stockFilters.Add(dataTemp);
						}
						NewAllocationItemParam allocationDataSet = new NewAllocationItemParam()
						{
							SourceNo = processNo,
							SourceType = "10",
							AllocationType = AllocationType.NoTarget,
							GupCode = gupCode,
							CustCode = custCode,
							SrcDcCode = dcCode,
							SrcWarehouseId = gBomDataNotW.Key.WAREHOUSE_ID,
							SrcStockFilterDetails = stockFilters,
							ReturnStocks = stockListTemp
						};
						var createAllocationResult = sharedService.CreateOrUpdateAllocation(allocationDataSet);

						if (!createAllocationResult.Result.IsSuccessed)
							return createAllocationResult.Result;
						else
						{
							ticketMessage = string.IsNullOrWhiteSpace(ticketMessage) ? string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()) : string.Format("{0},{1}", ticketMessage, string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()));
							stockListTemp = createAllocationResult.StockList;
							allocationListTemp.AddRange(createAllocationResult.AllocationList);
						}
					}

					var allocationResult = sharedService.BulkInsertAllocation(allocationListTemp, stockListTemp);

					if (!allocationResult.IsSuccessed)
						return allocationResult;

					//產生的調撥單號
					var allocationList = new List<string>();
					allocationList.AddRange(ticketMessage.Split(','));

					// 2.3. 將產生的調撥單號寫入揀料單與調撥單關聯
					if (allocationList != null && allocationList.Any())
					{
						var alloNos = allocationList.Distinct().ToList();
						var repo9 = new F91020502Repository(Schemas.CoreSchema, _wmsTransaction);
						foreach (var alloNo in alloNos)
						{
							F91020502 f91020502 = new F91020502()
							{
								PICK_NO = pickNo,
								ALLOCATION_NO = alloNo,
								DC_CODE = dcCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode
							};
							repo9.Add(f91020502);
						}
					}
				}
				finally
				{
					if (isPass)
						stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
				}
			}
			#endregion

			// 回傳結果
			return new ExecuteResult() { IsSuccessed = true, Message = pickNo };
		}

		/// <summary>
		/// 找出該筆Material_Code的F1913資料, 更新F1913.QTY
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private ExecuteResult UpdateF1913Qty(string dcCode, string gupCode, string custCode, StockDetail data)
		{
			var repo = new F19.F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			try
			{
				repo.MinusQty(dcCode, gupCode, custCode, data.ItemCode, data.SrcLocCode, data.ValidDate, data.EnterDate, data.VnrCode, data.SerialNo, Convert.ToInt32(data.Qty), Current.Staff, Current.StaffName, data.BoxCtrlNo, data.PalletCtrlNo, data.MAKE_NO);
				return new ExecuteResult() { IsSuccessed = true };
			}
			catch (Exception ex)
			{
				return new ExecuteResult() { IsSuccessed = false, Message = ex.Message };
			}

			//var f1913 = repo.Filter(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == data.ItemCode && x.LOC_CODE == data.SrcLocCode && x.BOX_CTRL_NO == data.BoxCtrlNo && x.PALLET_CTRL_NO == data.PalletCtrlNo && x.MAKE_NO == data.MAKE_NO && x.VNR_CODE == data.VnrCode && x.SERIAL_NO == data.SerialNo && x.VALID_DATE >= DateTime.Today && x.ENTER_DATE <= DateTime.Today && x.QTY > 0);
			//if (f1913 == null || f1913.Count() == 0)
			//{
			//	return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P910101Service_ITEM_LOC_CODE_NO_DATA };
			//}
			//foreach (var p in f1913)
			//{
			//	if (p.QTY >= data.Qty)
			//	{
			//		// 更新F1913.QTY
			//		p.QTY = (int)(p.QTY - data.Qty);
			//		repo.Update(p);
			//		return new ExecuteResult() { IsSuccessed = true };
			//	}
			//}
			//return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P910101Service_STOCK_INSSUFICIENT };
		}
		/// <summary>
		/// 取得物料集合
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="processNo"></param>
		/// <returns></returns>
		public IQueryable<ProcessItem> GetMaterialList(string gupCode, string custCode, string dcCode, string processNo)
		{
			var f910201Rep = new F910201Repository(Schemas.CoreSchema);
			var f910201 = f910201Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo, false);
			var bomType = "1"; //預設同拆解
			if (!string.IsNullOrEmpty(f910201.ITEM_CODE_BOM))
			{
				var f910101Rep = new F910101Repository(Schemas.CoreSchema);
				bomType = f910101Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.BOM_NO == f910201.ITEM_CODE_BOM, false).BOM_TYPE;
			}
			if (bomType == "1") //拆解或無Bom物料即流通加工單的ITEM_CODE
				return f910201Rep.GetProcessItemsNonBom(dcCode, gupCode, custCode, processNo);
			else  //組合物料即流通加工單的關聯Bom的組合前的ITEM
				return f910201Rep.GetProcessItemsByBom(dcCode, gupCode, custCode, processNo);
		}

		/// <summary>
		/// 取得成品集合
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="processNo"></param>
		/// <returns></returns>
		public IQueryable<ProcessItem> GetFinishItemList(string gupCode, string custCode, string dcCode, string processNo)
		{
			var f910201Rep = new F910201Repository(Schemas.CoreSchema);
			var f910201 = f910201Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo, false);
			var bomType = "0"; //預設同組合
			if (!string.IsNullOrEmpty(f910201.ITEM_CODE_BOM))
			{
				var f910101Rep = new F910101Repository(Schemas.CoreSchema);
				bomType = f910101Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.BOM_NO == f910201.ITEM_CODE_BOM, false).BOM_TYPE;
			}
			if (bomType == "1") //拆解成品即流通加工單的關聯Bom的拆解後的ITEM
				return f910201Rep.GetProcessItemsByBom(dcCode, gupCode, custCode, processNo);
			else  //組合或無Bom成品即流通加工單的ITEM_CODE
				return f910201Rep.GetProcessItemsNonBom(dcCode, gupCode, custCode, processNo);
		}

		public IQueryable<Wms3pl.Datas.Shared.Entities.StockData> GetStockData(string dcCode, string gupCode, string custCode, string processNo)
		{
			var repo = new F19.F1913Repository(Schemas.CoreSchema);
			return repo.GetStockDataForP910101(dcCode, gupCode, custCode, processNo);
		}

		public IQueryable<Wms3pl.Datas.Shared.Entities.StockData> GetStockData2(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var repo = new F19.F1913Repository(Schemas.CoreSchema);
			return repo.GetStockData2ForP910101(dcCode, gupCode, custCode, itemCode);
		}

		public ExecuteResult UpdateF910204(string dcCode, string gupCode, string custCode, string processNo, List<F910204> newData, List<F910204> updateData, List<F910204> removeData)
		{
			var repo = new F910204Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910201 = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);

			foreach (var p in newData)
			{
				if (repo.Find(x => x.DC_CODE == p.DC_CODE && x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE && x.PROCESS_NO == p.PROCESS_NO && x.ORDER_BY == p.ORDER_BY && x.ACTION_NO == p.ACTION_NO) == null)
				{
					repo.Add(new F910204()
					{
						ACTION_NO = p.ACTION_NO,
						ORDER_BY = p.ORDER_BY,
						PROCESS_NO = p.PROCESS_NO,
						CUST_CODE = p.CUST_CODE,
						GUP_CODE = p.CUST_CODE,
						DC_CODE = p.DC_CODE,
						LABEL_NO = p.LABEL_NO
					});
				}
			}

			foreach (var p in updateData)
			{
				var tmp = repo.Find(x => x.DC_CODE == p.DC_CODE && x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE && x.PROCESS_NO == p.PROCESS_NO && x.ORDER_BY == p.ORDER_BY && x.ACTION_NO == p.ACTION_NO);
				if (tmp == null)
				{
					return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };
				}
				else
				{
					tmp.LABEL_NO = p.LABEL_NO;
					repo.Update(tmp);
				}
			}

			foreach (var p in removeData)
			{
				var tmp = repo.Find(x => x.DC_CODE == p.DC_CODE && x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE && x.PROCESS_NO == p.PROCESS_NO && x.ORDER_BY == p.ORDER_BY && x.ACTION_NO == p.ACTION_NO);
				if (tmp == null)
				{
					return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };
				}
				else
				{
					repo.Delete(x => x.DC_CODE == p.DC_CODE && x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE && x.PROCESS_NO == p.PROCESS_NO && x.ORDER_BY == p.ORDER_BY && x.ACTION_NO == p.ACTION_NO);
				}
			}

			var f910201 = repo910201.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == processNo);
			if (f910201 == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };
			else
			{
				if (f910201.PROC_STATUS == "1")
				{
					f910201.PROC_STATUS = "2";
					repo910201.Update(f910201);
				}
			}

			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult UpdateF910203(string dcCode, string gupCode, string custCode, string processNo, List<F910004Data> newData, List<F910004Data> removeData)
		{
			var repo1 = new F910203Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo2 = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);

			// 刪除資料
			foreach (var p in removeData)
			{
				repo1.Delete(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == processNo && x.PRODUCE_NO == p.PRODUCE_NO);
			}
			// 新增資料
			foreach (var p in newData)
			{
				repo1.Add(new F910203()
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					PRODUCE_NO = p.PRODUCE_NO,
					PROCESS_NO = processNo
				});
			}
			// 更新F910201 PROC_STATUS WHERE PROC_STATUS = 2
			var f910201 = repo2.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == processNo && x.PROC_STATUS == "2");
			if (f910201 != null)
			{
				f910201.PROC_STATUS = "3";
				repo2.Update(f910201);
			}
			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult UpdateF910201(string dcCode, string gupCode, string custCode, string processNo, string processSource, string outsourceId, DateTime finishDate, string itemCode, string itemCodeBom, int processQty, int boxQty, int caseQty, string orderNo, string memo, string quoteNo, int breakQty)
		{
			ExecuteResult result = null;
			var repo = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);

			var f910201 = repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == processNo);

			var needUpdateLo = (f910201.ITEM_CODE != itemCode || f910201.PROCESS_QTY != processQty);

			f910201.QUOTE_NO = quoteNo;
			f910201.PROCESS_SOURCE = processSource;
			f910201.OUTSOURCE_ID = outsourceId;
			f910201.FINISH_DATE = finishDate;
			f910201.ITEM_CODE = itemCode;
			f910201.ITEM_CODE_BOM = itemCodeBom;
			f910201.PROCESS_QTY = processQty;
			f910201.BOX_QTY = boxQty;
			f910201.CASE_QTY = caseQty;
			f910201.ORDER_NO = orderNo;
			f910201.BREAK_QTY = breakQty;
			f910201.MEMO = memo;
			repo.Update(f910201);

			if (result == null)
				result = new ExecuteResult() { IsSuccessed = true, Message = processNo };

			return result;
		}

		public ExecuteResult DeleteF910201(string processNo, string gupCode, string custCode, string dcCode)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f910201Repo = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910201 = f910201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode &&
							 o.CUST_CODE == custCode && o.PROCESS_NO == processNo);
			if (f910201 == null)
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_ProcessNotFound };
			//只能狀態為0的才可刪除
			if (f910201.STATUS != "0")
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_Unprocessed_CantDelete };

			if (f910201.PROC_STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P910101Service_Pick_CantDeeleete);

			f910201.STATUS = "9"; //更新為9取消
			f910201Repo.Update(f910201);


			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };
			return result;
		}


		public ExecuteResult FinishProcess(string processNo, string gupCode, string custCode, string dcCode, int aProcessQty, int breakQty, string memo)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f910201Repo = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910201 = f910201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode &&
							 o.CUST_CODE == custCode && o.PROCESS_NO == processNo);
			if (f910201 == null)
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_ProcessNotFound };

			f910201.A_PROCESS_QTY += aProcessQty;
			f910201.BREAK_QTY += breakQty;
			f910201.MEMO = memo;
			f910201.STATUS = "2"; //加工完成
			f910201.FINISH_DATE = DateTime.Today;
			f910201.FINISH_TIME = DateTime.Now.ToString("HH:mm");
			f910201Repo.Update(f910201);


			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };

			return result;
		}

		public IQueryable<BackData> GetBackListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
		{
            var repo = new F910206Repository(Schemas.CoreSchema);
            var result = repo.GetBackListForP9101010500(dcCode, gupCode, custCode, processNo);
            return result;
		}

        public IQueryable<BackData> GetHistoryListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
        {
            var repo = new F910206Repository(Schemas.CoreSchema);
            var result = repo.GetHistoryListForP9101010500(dcCode, gupCode, custCode, processNo);
            return result;
        }

        public int GetProcessedItemsQty(string dcCode, string gupCode, string custCode, string processNo)
		{
			var f910204rep = new F910204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910201Rep = new F910201Repository(Schemas.CoreSchema);
			var processActions = f910204rep.GetProcessActions(dcCode, gupCode, custCode, processNo).ToList();
			var minActionNo = string.Empty;
			var maxActionNo = string.Empty;
			var minAction = processActions.OrderBy(a => a.SORT).FirstOrDefault();
			if (minAction != null) minActionNo = minAction.ACTION_NO;
			var maxAction = processActions.OrderByDescending(a => a.SORT).FirstOrDefault();
			if (maxAction != null) maxActionNo = maxAction.ACTION_NO;
			var itemType = "0"; // 成品
			var f910201 = f910201Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo, false);

			//若加工為A3 (拆解) 使用物料類 , 否則依成品類型
			if (maxActionNo == "A3") itemType = "1";
			var finishedItems = GetProcessedItems(dcCode, gupCode, custCode, processNo, maxActionNo, itemType, f910201.ITEM_CODE);

			if (finishedItems == null) return 0;
			//A3:拆解 A2組合商品
			return (maxActionNo == "A3" || maxActionNo == "A2") ? (int)finishedItems.First().SumQty : finishedItems.Count();


		}

		/// <summary>
		/// 將匯入的序號，若有組合商品的序號，則抽取出來轉換成組合後的序號，並將組合商品的序號回傳
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNos"></param>
		List<string> ConvertSerialNoToCombineSerialNos(string gupCode, string custCode, List<string> serialNos)
		{
			if (!serialNos.Any())
				return new List<string>();

			var f2501Repo = new F2501Repository(Schemas.CoreSchema);

			// 以A+B=C來表示組合商品
			var abCombinNos = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
												&& serialNos.Contains(x.SERIAL_NO)
												&& x.COMBIN_NO != null)
										.Select(x => x.COMBIN_NO)
										.Distinct()
										.ToList();

			var f2501s = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
											&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
											&& abCombinNos.Contains(x.COMBIN_NO))
									.Select(x => new { x.ITEM_CODE, x.SERIAL_NO, x.BOUNDLE_ITEM_CODE })
									.Distinct()
									.ToList();

			var combineSerailNos = f2501s.Select(x => x.SERIAL_NO).ToList();

			serialNos.RemoveAll(serialNo => combineSerailNos.Contains(serialNo));

			var combineSerialNos = f2501s.Where(x => x.BOUNDLE_ITEM_CODE == null).Select(x => x.SERIAL_NO).ToList();
			serialNos.AddRange(combineSerialNos);
			return combineSerialNos;
		}

		public ExecuteResult CreateUpdateBackDataForP91010105(string dcCode, string gupCode, string custCode, string processNo, List<BackData> newData, List<BackData> removeData, List<BackData> editData, List<string> goodSerialNos, List<string> breakSerialNos)
		{
			var f910206Rep = new F910206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Rep = new F1903Repository(Schemas.CoreSchema);
			var f910201Rep = new F910201Repository(Schemas.CoreSchema);
			var f91020601Rep = new F91020601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151001Rep = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f910204rep = new F910204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910205rep = new F910205Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			ExecuteResult result = null;

			//先檢查是否符合可回倉商品數，若有傳入回倉序號backSerialNos，則要再檢查是否在可回倉商品的序號綁儲位商品的序號內
			// 架回倉產生調撥單請使用Share專案的Function  CreateAllocationNoSource (內部會先刪除原來源單號的調撥單，再重新產生)

			// 如果是回倉明細關聯的調撥單尚未上架，狀態為3，則需先將調撥單刪除，產生新的調撥單(F151001, F151002)與上架回倉關聯檔(F91020601)
			// 如果之前建立的調撥單已經上架，則產生新的調撥單(F151001, F151002)與Insert關聯單(F91020601)
			// 產生調撥單請使用Share專案共用的產生調撥單Function，預設狀態為3

			// 將匯入的序號，若有組合商品的序號，則抽取出來轉換成組合後的序號，並將組合商品的序號回傳
			var goodCombineSerialNos = ConvertSerialNoToCombineSerialNos(gupCode, custCode, goodSerialNos);
			var breakCombineSerialNos = ConvertSerialNoToCombineSerialNos(gupCode, custCode, breakSerialNos);

			var tmpList2 = new List<BackData>();
			tmpList2.AddRange(newData);
			tmpList2.AddRange(editData);
			var backItemCodes = tmpList2.Select(a => a.ITEM_CODE).Distinct().ToList();

			var serialBundleItems = backItemCodes.Any() ? f1903Rep.InWithTrueAndCondition("ITEM_CODE", backItemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.BUNDLE_SERIALLOC == "1").ToList()
														: new List<F1903>();

			// 是否存在回倉商品類行為揀料的序號綁儲位商品 
			var existsBundleSerialLocMaterialItem = serialBundleItems.Any();

			// 若有揀料是序號綁儲位才需檢查，如果是成品則不用匯入，由系統編一個假序號寫入F2501(F2501.ISBOUNDLE=1)，假序號須符合F1903序號格式設定(ex.碼數、是否純數字(是=0,否=X)、頭碼)。
			if (existsBundleSerialLocMaterialItem && !goodSerialNos.Any() && !breakSerialNos.Any())
			{
				result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_ExistsBundleSerialLocMaterialItem_NeedUpload };
				return result;
			}

			List<F2501> goodBackf2501s = new List<F2501>();
			List<F2501> breakBackf2501s = new List<F2501>();
			//檢查要回倉的序號綁儲位商品數量是否與上傳的序號數量相符合，因序號只上傳本次新增及修改的部分，所以不需包含未回倉未異動資料
			if (existsBundleSerialLocMaterialItem || goodSerialNos.Any() || breakSerialNos.Any())
			{
				var f2501Rep = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
				//檢查良品部分
				goodBackf2501s = f2501Rep.InWithTrueAndCondition("SERIAL_NO", goodSerialNos, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
				result = CheckBackSerialNoCount(tmpList2, serialBundleItems, goodBackf2501s, true);
				if (!result.IsSuccessed)
					return result;

				//檢查報廢部分
				breakBackf2501s = f2501Rep.InWithTrueAndCondition("SERIAL_NO", breakSerialNos, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
				result = CheckBackSerialNoCount(tmpList2, serialBundleItems, breakBackf2501s, false);
				if (!result.IsSuccessed)
					return result;
			}

			var notbackItemSumQtys = f91020601Rep.GetBackItemSumQtys(dcCode, gupCode, custCode, processNo, false).ToList();
			//先前未回倉未異動的資料
			var notModifyItems = (from a in notbackItemSumQtys
														join b in tmpList2 on a.BACK_NO equals b.BACK_NO into j
														from b in j.DefaultIfEmpty()
														join c in removeData on a.BACK_NO equals c.BACK_NO into j2
														from c in j2.DefaultIfEmpty()
														where b == null && c == null
														select a).ToList();

			var processActions = f910204rep.GetProcessActions(dcCode, gupCode, custCode, processNo).ToList();
			var minActionNo = string.Empty;
			var maxActionNo = string.Empty;
			var minAction = processActions.OrderBy(a => a.SORT).FirstOrDefault();
			if (minAction != null) minActionNo = minAction.ACTION_NO;
			var maxAction = processActions.OrderByDescending(a => a.SORT).FirstOrDefault();
			if (maxAction != null) maxActionNo = maxAction.ACTION_NO;
			var f910201 = f910201Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo, false);
			result = CheckBoxCaseCount(processActions.Select(a => a.ACTION_NO).ToList(), f910201.BOX_QTY, f910201.CASE_QTY);
			if (result != null && result.IsSuccessed == false) return result;

			//已揀料可回倉物料
			var canBackMaterialItems = f910205rep.GetCanBackMaterialItem(dcCode, gupCode, custCode, processNo).ToList();
			//已使用物料
			var processedMaterialItems = GetProcessedItems(dcCode, gupCode, custCode, processNo, minActionNo, "1", f910201.ITEM_CODE);
			//已完成可回倉成品
			var finishedItems = GetProcessedItems(dcCode, gupCode, custCode, processNo, maxActionNo, "0", f910201.ITEM_CODE);
			//已回倉成品
			var backedFinishItems = f91020601Rep.GetItemSumQtys(dcCode, gupCode, custCode, processNo, "0", true).ToList();
			//已回倉物料
			var backedFinishMaterialItems = f91020601Rep.GetItemSumQtys(dcCode, gupCode, custCode, processNo, "1", true).ToList();

			if (goodSerialNos.Any() || breakSerialNos.Any())
			{
				var notModifySerialNos = notModifyItems.Select(a => a.SERIAL_NO).ToList();
				//若有傳入回倉序號backSerialNos，則檢查是否在可回倉商品的序號綁儲位商品的序號內
				result = CheckBackSerialNos(goodSerialNos,
											breakSerialNos,
											notModifySerialNos,
											canBackMaterialItems,
											processedMaterialItems,
											finishedItems,
											backedFinishItems,
											maxActionNo,
											f910201.BOX_QTY,
											f910201.CASE_QTY,
											goodCombineSerialNos,
											breakCombineSerialNos);
				if (!result.IsSuccessed)
					return result;
			}

			var modifiedBackNo = editData.Select(a => a.BACK_NO).Union(removeData.Select(b => b.BACK_NO)).ToList();
			//全部欲上架回倉資料，先取得未回倉未異動資料
			var allBackDatas = f910206Rep.GetBackList(dcCode, gupCode, custCode, processNo, isBacked: false, excludeBackNos: modifiedBackNo).ToList();
			allBackDatas.AddRange(tmpList2); //再合併新增及修改資料
																			 //檢查可上架回倉數量，需用全部上架回倉資料判斷


			var backFinishDatas = new List<BackData>();
			backFinishDatas.AddRange(backedFinishItems.GroupBy(x => x.ITEM_CODE).Select(x => new BackData { BACK_ITEM_TYPE = "0", ITEM_CODE = x.Key, GOOD_BACK_QTY = (long)x.Sum(y => y.SumQty) }));
			backFinishDatas.AddRange(backedFinishMaterialItems.GroupBy(x => x.ITEM_CODE).Select(x => new BackData { BACK_ITEM_TYPE = "1", ITEM_CODE = x.Key, GOOD_BACK_QTY = (long)x.Sum(y => y.SumQty) }));
			result = CheckProcessItemQty(f910201, canBackMaterialItems, allBackDatas, backFinishDatas, maxActionNo);

			if (!result.IsSuccessed)
				return result;

			// 1.刪除回倉明細(F910206)
			DeleteF910206s(dcCode, gupCode, custCode, processNo, removeData, f910206Rep);

			// 2. 產生回倉明細(F910206), 包含新增的回倉明細, 以及修改的回倉明細
			InsertF910206s(dcCode, gupCode, custCode, processNo, newData, f910206Rep);

			UpdateF910206s(dcCode, gupCode, custCode, processNo, editData, f910206Rep);

			// 3. 產生調撥單
			var f91020502rep = new F91020502Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得建立揀料單時產生的調撥單號，為須排除的調撥單號
			var excludeAllocationNos = f91020502rep.GetPickAllocationNos(dcCode, gupCode, custCode, processNo).ToList();
			//須排除的調撥單號，加入已回倉調撥單
			excludeAllocationNos.AddRange(f91020601Rep.GetBackedAllocationNos(dcCode, gupCode, custCode, processNo).ToList());
            //須排除的調撥單號，加入已經在上架中或上架完成的調撥單
            var processedAllocNos = f91020601Rep.GetProcessedAllocationNos(dcCode, gupCode, custCode, processNo).ToList();
            excludeAllocationNos.AddRange(processedAllocNos);


            //取得須排除的調撥單數量
            var processedDatas = f151001Rep.GetProcessedData(dcCode, gupCode, custCode, processedAllocNos);

            var itemCodes = tmpList2.Select(a => a.ITEM_CODE).ToList();
			var f1903s = f1903Rep.InWithTrueAndCondition("ITEM_CODE", itemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();


			//因未異動部分可能有先前的序號，所以先以新增及修改的部分取資料，後面在併上先前未回倉未異動的資料
			var goodBackItems = (from a in tmpList2
													 join b in f1903s on a.ITEM_CODE equals b.ITEM_CODE
													 join c in goodBackf2501s on a.ITEM_CODE equals c.ITEM_CODE into j
													 from c in j.DefaultIfEmpty()
													 where a.GOOD_BACK_QTY > 0
													 select
														 new SrcItemLocQty
														 {
															 ItemCode = a.ITEM_CODE,
															 SerialNo = (c == null) ? "" : c.SERIAL_NO,
															 Qty = ((c == null) ? (int)a.GOOD_BACK_QTY : 1) - processedDatas.Where(x => x.TAR_WAREHOUSE_ID == "G" && x.BACK_ITEM_TYPE == a.BACK_ITEM_TYPE).Sum(x => x.TAR_QTY),
															 ValidDate = (c == null) ? DateTime.Parse("9999/12/31") : c.VALID_DATE ?? DateTime.Parse("9999/12/31"),
															 EnterDate = DateTime.Today,
															 VnrCode = "000000",
															 BankItemType = a.BACK_ITEM_TYPE,
															 BoxCtrlNo = "0",
															 PalletCtrlNo = "0",
															 MakeNo = "0"
														 }).Where(x => x.Qty > 0).ToList();
			var breakBackItems = (from a in tmpList2
														join b in f1903s on a.ITEM_CODE equals b.ITEM_CODE
														join c in breakBackf2501s on a.ITEM_CODE equals c.ITEM_CODE into j
														from c in j.DefaultIfEmpty()
														where a.BREAK_BACK_QTY > 0
														select
															new SrcItemLocQty
															{
																ItemCode = a.ITEM_CODE,
																SerialNo = (c == null) ? "" : c.SERIAL_NO,
																Qty = ((c == null) ? (int)a.BREAK_BACK_QTY : 1) - processedDatas.Where(x => x.TAR_WAREHOUSE_ID == "N" && x.BACK_ITEM_TYPE == a.BACK_ITEM_TYPE).Sum(x => x.TAR_QTY),
																ValidDate = (c == null) ? DateTime.Parse("9999/12/31") : c.VALID_DATE ?? DateTime.Parse("9999/12/31"),
																EnterDate = DateTime.Today,
																VnrCode = "000000",
																BankItemType = a.BACK_ITEM_TYPE,
																BoxCtrlNo = "0",
																PalletCtrlNo = "0",
																MakeNo = "0"
															}).Where(x => x.Qty > 0).ToList();
			goodBackItems.AddRange(notModifyItems.Where(a => a.IS_GOOD == "1").Select(a => new SrcItemLocQty { ItemCode = a.ITEM_CODE, SerialNo = (a.SERIAL_NO == "0") ? "" : a.SERIAL_NO, Qty = (int)(a.SUMQTY ?? 0), VnrCode = "000000", BankItemType = a.BACK_ITEM_TYPE, BoxCtrlNo = "0", PalletCtrlNo = "0", MakeNo = "0", ValidDate = DateTime.Parse("9999/12/31"), EnterDate = DateTime.Today }).ToList());
			breakBackItems.AddRange(notModifyItems.Where(a => a.IS_GOOD != "1").Select(a => new SrcItemLocQty { ItemCode = a.ITEM_CODE, SerialNo = (a.SERIAL_NO == "0") ? "" : a.SERIAL_NO, Qty = (int)(a.SUMQTY ?? 0), VnrCode = "000000", BankItemType = a.BACK_ITEM_TYPE, BoxCtrlNo = "0", PalletCtrlNo = "0", MakeNo = "0", ValidDate = DateTime.Parse("9999/12/31"), EnterDate = DateTime.Today }).ToList());

			List<F910101ToWarehouseData> toWarehouseDatas = new List<F910101ToWarehouseData>();

			// 產生良品的調撥單(成品、揀料分開產生調撥單)
			if (goodBackItems.Any()) //如果沒有良品資料，但有刪除商品，仍要刪除先前的調撥單
			{
				//成品
				var goodBackItemsTemp = goodBackItems.Where(o => o.BankItemType == "0").ToList();
				if (goodBackItemsTemp.Any())
					toWarehouseDatas.Add(new F910101ToWarehouseData() { TarData = goodBackItemsTemp, TargetWarehouseType = "G" });
				//goodItemResult1 = sharedService.CreateOrUpdateAllocationNoSrc(dcCode, gupCode, custCode, goodBackItems.Where(o => o.BankItemType == "0").ToList(), "G", processNo, "10", dcCode, true, excludeAllocationNos);
				
				//揀料
				var goodBackItemsTemp1 = goodBackItems.Where(o => o.BankItemType == "1").ToList();
				if (goodBackItemsTemp1.Any())
					toWarehouseDatas.Add(new F910101ToWarehouseData() { TarData = goodBackItemsTemp1, TargetWarehouseType = "G" });
				//goodItemResult2 = sharedService.CreateOrUpdateAllocationNoSrc(dcCode, gupCode, custCode, goodBackItems.Where(o => o.BankItemType == "1").ToList(), "G", processNo, "10", dcCode, delBreakOldAllocation, excludeAllocationNos);
			}
			// 產生不良品的調撥單(成品、揀料分開產生調撥單)
			if (breakBackItems.Any())
			{
				//成品
				var breakBackItemsTemp = breakBackItems.Where(o => o.BankItemType == "0").ToList();
				if (breakBackItemsTemp.Any())
					toWarehouseDatas.Add(new F910101ToWarehouseData() { TarData = breakBackItemsTemp, TargetWarehouseType = "N" });

				//breakItemResult1 = sharedService.CreateOrUpdateAllocationNoSrc(dcCode, gupCode, custCode, breakBackItems.Where(o => o.BankItemType == "0").ToList(), "N", processNo, "10", dcCode, delBreakOldAllocation, excludeAllocationNos);
				//揀料
				var breakBackItemsTemp1 = breakBackItems.Where(o => o.BankItemType == "1").ToList();
				if (breakBackItemsTemp1.Any())
					toWarehouseDatas.Add(new F910101ToWarehouseData() { TarData = breakBackItemsTemp1, TargetWarehouseType = "N" });
				//breakItemResult2 = sharedService.CreateOrUpdateAllocationNoSrc(dcCode, gupCode, custCode, breakBackItems.Where(o => o.BankItemType == "1").ToList(), "N", processNo, "10", dcCode, delBreakOldAllocation, excludeAllocationNos);
			}

			//調撥單資料暫存
			List<ReturnNewAllocation> allocationListTemp = new List<ReturnNewAllocation>();

			//庫存暫存
			var stockListTemp = new List<F1913>() { };

			//刪除來源單號是加工單的調撥單(不含開立揀料單的調撥單和已回倉的調撥單)
			var deletedAllocationParam = new DeletedAllocationParam
			{
				DcCode = dcCode,
				GupCode = gupCode,
				CustCode=custCode,
				DeleteAllocationType = DeleteAllocationType.SourceNo,
				OrginalSourceNo = processNo,
				ExcludeDeleteAllocationNos = excludeAllocationNos,
				F1913List = stockListTemp,
			};
			//純上架無須回復庫存
			var delResult = sharedService.DeleteAllocation(deletedAllocationParam, false, true);
			if (!delResult.IsSuccessed)
				return delResult;
			//調撥單號
			string ticketMessage = string.Empty;

			foreach (var item in toWarehouseDatas)
			{
				NewAllocationItemParam allocationDate = new NewAllocationItemParam()
				{
					StockDetails = item.TarData.Select(o => new StockDetail
					{
						CustCode = custCode,
						GupCode = gupCode,
						SrcDcCode = dcCode,
						TarDcCode = dcCode,
						ItemCode = o.ItemCode,
						ValidDate = o.ValidDate,
						EnterDate = o.EnterDate,
						Qty = o.Qty,
						VnrCode = o.VnrCode,
						SerialNo = o.SerialNo,
						BoxCtrlNo = o.BoxCtrlNo,
						PalletCtrlNo = o.PalletCtrlNo,
						MAKE_NO = o.MakeNo
					}).ToList(),
					SrcDcCode = dcCode,
					TarDcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					ReturnStocks = stockListTemp,
					AllocationType = Shared.Enums.AllocationType.NoSource,
					SourceNo = processNo,
					SourceType = "10",
					TarWarehouseType = item.TargetWarehouseType
				};
				var createAllocationResult = sharedService.CreateOrUpdateAllocation(allocationDate);

				if (!createAllocationResult.Result.IsSuccessed)
					return createAllocationResult.Result;
				else
				{
					ticketMessage = string.IsNullOrWhiteSpace(ticketMessage) ? string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()) : string.Format("{0},{1}", ticketMessage, string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()));
					stockListTemp = createAllocationResult.StockList;
					allocationListTemp.AddRange(createAllocationResult.AllocationList);

					foreach (var allocationListItem in createAllocationResult.AllocationList)
					{
						if (allocationListItem.Master.TAR_WAREHOUSE_ID.Substring(0, 1) == "G")
						{
							var allocationListItem_gs = allocationListItem.Details.GroupBy(o => new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.VALID_DATE, o.ENTER_DATE, o.BOX_CTRL_NO, o.PALLET_CTRL_NO, o.MAKE_NO, o.SERIAL_NO, o.ITEM_CODE, o.ALLOCATION_NO, o.VNR_CODE }).ToList();

							foreach (var allocationDetialItem_g in allocationListItem_gs)
							{
								var goodBackItemTemp = goodBackItems.Where(o => o.ItemCode == allocationDetialItem_g.Key.ITEM_CODE &&
								 o.SerialNo == allocationDetialItem_g.Key.SERIAL_NO && o.BoxCtrlNo == allocationDetialItem_g.Key.BOX_CTRL_NO &&
								 o.PalletCtrlNo == allocationDetialItem_g.Key.PALLET_CTRL_NO && o.MakeNo == allocationDetialItem_g.Key.MAKE_NO &&
								 o.VnrCode == allocationDetialItem_g.Key.VNR_CODE &&
								 o.ValidDate == allocationDetialItem_g.Key.VALID_DATE && o.EnterDate == allocationDetialItem_g.Key.ENTER_DATE && o.AllocationNo == null).OrderBy(x => x.BankItemType).FirstOrDefault();
								if (goodBackItemTemp != null)
								{
									goodBackItemTemp.AllocationNo = allocationDetialItem_g.Key.ALLOCATION_NO;
									goodBackItemTemp.Qty = allocationDetialItem_g.Sum(x => x.TAR_QTY);
								}
								else
								{
									goodBackItemTemp = new SrcItemLocQty()
									{
										AllocationNo = allocationDetialItem_g.Key.ALLOCATION_NO,
										Qty = allocationDetialItem_g.Sum(x => x.TAR_QTY),
										ItemCode = allocationDetialItem_g.Key.ITEM_CODE,
										SerialNo = allocationDetialItem_g.Key.SERIAL_NO,
										ValidDate = allocationDetialItem_g.Key.VALID_DATE,
										EnterDate = allocationDetialItem_g.Key.ENTER_DATE,
										VnrCode = allocationDetialItem_g.Key.VNR_CODE,
										BoxCtrlNo = allocationDetialItem_g.Key.BOX_CTRL_NO,
										PalletCtrlNo = allocationDetialItem_g.Key.PALLET_CTRL_NO,
										MakeNo = allocationDetialItem_g.Key.MAKE_NO
									};
									goodBackItems.Add(goodBackItemTemp);
								}
							}
						}

						if (allocationListItem.Master.TAR_WAREHOUSE_ID.Substring(0, 1) == "N")
						{
							var allocationListItem_gs = allocationListItem.Details.GroupBy(o => new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.VALID_DATE, o.ENTER_DATE, o.BOX_CTRL_NO, o.PALLET_CTRL_NO, o.MAKE_NO, o.SERIAL_NO, o.ITEM_CODE, o.ALLOCATION_NO, o.VNR_CODE }).ToList();

							foreach (var allocationDetialItem_g in allocationListItem_gs)
							{
								var breakBackItemTemp = breakBackItems.Where(o => o.ItemCode == allocationDetialItem_g.Key.ITEM_CODE &&
							 o.SerialNo == allocationDetialItem_g.Key.SERIAL_NO && o.BoxCtrlNo == allocationDetialItem_g.Key.BOX_CTRL_NO &&
							 o.PalletCtrlNo == allocationDetialItem_g.Key.PALLET_CTRL_NO && o.MakeNo == allocationDetialItem_g.Key.MAKE_NO &&
							 o.VnrCode == allocationDetialItem_g.Key.VNR_CODE &&
							 o.ValidDate == allocationDetialItem_g.Key.VALID_DATE && o.EnterDate == allocationDetialItem_g.Key.ENTER_DATE && o.AllocationNo == null).FirstOrDefault();
								if (breakBackItemTemp != null)
								{
									breakBackItemTemp.AllocationNo = allocationDetialItem_g.Key.ALLOCATION_NO;
									breakBackItemTemp.Qty = allocationDetialItem_g.Sum(x => x.TAR_QTY);
								}
								else
								{
									breakBackItemTemp = new SrcItemLocQty()
									{
										AllocationNo = allocationDetialItem_g.Key.ALLOCATION_NO,
										Qty = allocationDetialItem_g.Sum(x => x.TAR_QTY),
										ItemCode = allocationDetialItem_g.Key.ITEM_CODE,
										SerialNo = allocationDetialItem_g.Key.SERIAL_NO,
										ValidDate = allocationDetialItem_g.Key.VALID_DATE,
										EnterDate = allocationDetialItem_g.Key.ENTER_DATE,
										VnrCode = allocationDetialItem_g.Key.VNR_CODE,
										BoxCtrlNo = allocationDetialItem_g.Key.BOX_CTRL_NO,
										PalletCtrlNo = allocationDetialItem_g.Key.PALLET_CTRL_NO,
										MakeNo = allocationDetialItem_g.Key.MAKE_NO
									};
									breakBackItems.Add(breakBackItemTemp);
								}
							}
						}
					}
				}
			}

			var allocationResult = sharedService.BulkInsertAllocation(allocationListTemp, stockListTemp);
			if (!allocationResult.IsSuccessed)
				return allocationResult;

            // 刪除未回倉上架回倉調撥單關聯資料
            notbackItemSumQtys = notbackItemSumQtys.Where(x => !processedAllocNos.Contains(x.ALLOCATION_NO)).ToList();
            foreach (var notbackItemSumQty in notbackItemSumQtys)
			{
				f91020601Rep.Delete(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.BACK_NO == notbackItemSumQty.BACK_NO && a.ITEM_CODE == notbackItemSumQty.ITEM_CODE && a.SERIAL_NO == notbackItemSumQty.SERIAL_NO && a.ALLOCATION_NO == notbackItemSumQty.ALLOCATION_NO);
			}
			// 新增上架回倉調撥單關聯資料
			var addF91020601List = GetF91020601s(dcCode, gupCode, custCode, processNo, newData, editData, f91020601Rep, notModifyItems, goodBackItems, breakBackItems).ToList();
			f91020601Rep.BulkInsert(addF91020601List);

			if (result != null && result.IsSuccessed == false) return result;

			//string ticketMessage = "";
			//if (goodItemResult1 != null && !string.IsNullOrEmpty(goodItemResult1.Message)) ticketMessage = goodItemResult1.Message;
			//if (goodItemResult2 != null && !string.IsNullOrEmpty(goodItemResult2.Message)) ticketMessage += (string.IsNullOrEmpty(ticketMessage) ? "" : ",") + goodItemResult2.Message;

			//if (breakItemResult1 != null && !string.IsNullOrEmpty(breakItemResult1.Message)) ticketMessage += (string.IsNullOrEmpty(ticketMessage) ? "" : ",") + breakItemResult1.Message;
			//if (breakItemResult2 != null && !string.IsNullOrEmpty(breakItemResult2.Message)) ticketMessage += (string.IsNullOrEmpty(ticketMessage) ? "" : ",") + breakItemResult2.Message;
			return new ExecuteResult(true, ticketMessage);
		}

		/// <summary>
		/// 設定序號綁儲位的成品序號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <param name="goodBackItems"></param>
		/// <param name="breakBackItems"></param>
		private void SetBundleSerialNos(string dcCode, string gupCode, string custCode, string processNo, List<SrcItemLocQty> goodBackItems, List<SrcItemLocQty> breakBackItems, List<BackData> tmpList2)
		{
			var backItems = goodBackItems.Concat(breakBackItems).ToList();

			#region 取得成品的ItemCode
			var itemCodes = backItems.Select(x => x.ItemCode).Distinct().ToList();

			var f910206Repo = new F910206Repository(Schemas.CoreSchema, _wmsTransaction);
			var comineItemCodes = f910206Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
														&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
														&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
														&& x.PROCESS_NO == EntityFunctions.AsNonUnicode(processNo)
														&& x.BACK_ITEM_TYPE == "0")
												.Select(x => x.ITEM_CODE)
												.Distinct()
												.ToList();

			// 將已在上架回倉的品號與此次新增與編輯的成品品號合併，來尋找序號綁儲位的F2501
			comineItemCodes = comineItemCodes.Concat(tmpList2.Where(backData => backData.BACK_ITEM_TYPE == "0")
											.Select(backData => backData.ITEM_CODE))
											.Distinct()
											.ToList(); // 回倉商品類型(0成品1揀料)

			itemCodes = itemCodes.Where(x => comineItemCodes.Contains(x)).ToList();
			#endregion

			if (!itemCodes.Any())
				return;

			#region 取得序號綁儲位的 F2501s
			var f1903Rep = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			itemCodes = f1903Rep.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
											&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
											&& x.BUNDLE_SERIALLOC == "1"
											&& itemCodes.Contains(x.ITEM_CODE))
								.Select(x => x.ITEM_CODE)
								.ToList();

			var f91020601Repo = new F91020601Repository(Schemas.CoreSchema, _wmsTransaction);
			// 取得已回倉與尚未回倉的已設定序號項目，之後只將尚未設定的序號填到新加入的回倉成品序號中
			var existsSerailNosForBacked = f91020601Repo.GetBackedSerialNos(dcCode, gupCode, custCode, processNo).ToList();
			var existsSerailNosForNotBacked = goodBackItems.Concat(breakBackItems).Where(x => (!string.IsNullOrEmpty(x.SerialNo) && x.SerialNo != "0")).Select(x => x.SerialNo).ToList();
			var existsSerailNos = existsSerailNosForBacked.Concat(existsSerailNosForNotBacked).Distinct().ToList();

			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f2501s = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
											&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
											&& x.PROCESS_NO == EntityFunctions.AsNonUnicode(processNo)
											&& x.COMBIN_NO != null            // 有組合的話，一定會有值
											&& x.BOUNDLE_ITEM_CODE == null        // 成品這個值一定是 null
											&& x.STATUS == "A1"             // 只讀取尚未被調撥刷讀的序號
											&& itemCodes.Contains(x.ITEM_CODE)      // 只撈出成品的品號
											&& !existsSerailNos.Contains(x.SERIAL_NO))  // 排除之前已加入過的序號
									.ToList();

			var f2501Dict = f2501s.GroupBy(x => x.ITEM_CODE).ToDictionary(g => g.Key, g => new Queue<string>(g.Select(a => a.SERIAL_NO)));
			#endregion

			// 當準備要丟進調撥單處理的良品與不良品的序號綁儲位成品，若沒有序號的話，則塞入序號
			SetSerialNoToBackItems(goodBackItems, f2501Dict);
			SetSerialNoToBackItems(breakBackItems, f2501Dict);
		}

		private static void SetSerialNoToBackItems(List<SrcItemLocQty> backItems, Dictionary<string, Queue<string>> f2501Dict)
		{
			var items = backItems.Where(x => f2501Dict.ContainsKey(x.ItemCode) && (string.IsNullOrEmpty(x.SerialNo) || x.SerialNo == "0")).ToList();

			items.ForEach(x => backItems.Remove(x));

			var SrcItemLocQtyForSeriialNos = items.SelectMany(x => Enumerable.Range(1, (int)x.Qty).Select(i =>
			{
				var SrcItemLocQtyForSeriialNo = AutoMapper.Mapper.DynamicMap<SrcItemLocQty>(x);
				SrcItemLocQtyForSeriialNo.Qty = 1;
				SrcItemLocQtyForSeriialNo.SerialNo = f2501Dict[x.ItemCode].Dequeue();
				return SrcItemLocQtyForSeriialNo;
			}));

			backItems.AddRange(SrcItemLocQtyForSeriialNos);
		}

		private static void DeleteF910206s(string dcCode, string gupCode, string custCode, string processNo, List<BackData> removeData, F910206Repository f910206Rep)
		{
			foreach (var p in removeData)
			{
				f910206Rep.Delete(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.BACK_NO == p.BACK_NO);
			}
		}

		private static void UpdateF910206s(string dcCode, string gupCode, string custCode, string processNo, List<BackData> editData, F910206Repository f910206Rep)
		{
			var backNos = editData.Select(a => a.BACK_NO).ToList();
			var editF910206s = f910206Rep.AsForUpdate().InWithTrueAndCondition("BACK_NO", backNos, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo).ToList();
			//var editF91020601s = f91020601Rep.AsForUpdate().InWithTrueAndCondition("BACK_NO", backNos, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo).ToList();
			foreach (var p in editData)
			{
				var editF910206 = editF910206s.Where(a => a.BACK_NO == p.BACK_NO).SingleOrDefault();
				if (editF910206 != null)
				{
					editF910206.BREAK_BACK_QTY = p.BREAK_BACK_QTY;
					editF910206.GOOD_BACK_QTY = p.GOOD_BACK_QTY;
					editF910206.ITEM_CODE = p.ITEM_CODE;
					editF910206.BACK_ITEM_TYPE = p.BACK_ITEM_TYPE;
					f910206Rep.Update(editF910206);
				}
			}
		}

		private static void InsertF910206s(string dcCode, string gupCode, string custCode, string processNo, List<BackData> newData, F910206Repository f910206Rep)
		{
			var f910206s = newData.Select(p =>
			{
				p.BACK_NO = (long)SharedService.GetTableSeqId("SEQ_F910206_BACK_NO");
				return new F910206()
				{
					BACK_NO = p.BACK_NO,
					BREAK_BACK_QTY = p.BREAK_BACK_QTY,
					CUST_CODE = custCode,
					GUP_CODE = gupCode,
					DC_CODE = dcCode,
					GOOD_BACK_QTY = p.GOOD_BACK_QTY,
					ITEM_CODE = p.ITEM_CODE,
					PROCESS_NO = processNo,
					BACK_ITEM_TYPE = p.BACK_ITEM_TYPE
				};
			}).ToList();

			f910206Rep.BulkInsert(f910206s);
		}

		/// <summary>
		/// 取得要新增的上架回倉調撥單關聯資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <param name="newData"></param>
		/// <param name="editData"></param>
		/// <param name="f91020601Rep"></param>
		/// <param name="notModifyItems"></param>
		/// <param name="goodBackItems"></param>
		/// <param name="breakBackItems"></param>
		/// <returns></returns>
		private static IEnumerable<F91020601> GetF91020601s(string dcCode, string gupCode, string custCode, string processNo, List<BackData> newData, List<BackData> editData, F91020601Repository f91020601Rep, List<BackItemSumQty> notModifyItems, List<SrcItemLocQty> goodBackItems, List<SrcItemLocQty> breakBackItems)
		{
			var newItems = goodBackItems.Select(a => new { SrcItemLocQty = a, IsGood = "1" }).ToList();
			newItems.AddRange(breakBackItems.Select(a => new { SrcItemLocQty = a, IsGood = "0" }).ToList());
			foreach (var newItem in newItems)
			{
				var backInfos = newData.Where(a => a.ITEM_CODE == newItem.SrcItemLocQty.ItemCode && a.BACK_ITEM_TYPE == newItem.SrcItemLocQty.BankItemType && ((newItem.IsGood == "1" && a.GOOD_BACK_QTY > 0) || (newItem.IsGood != "1" && a.BREAK_BACK_QTY > 0)))
					.Select(a => new { BackNo = a.BACK_NO, BackItemType = a.BACK_ITEM_TYPE }).ToList();
				backInfos.AddRange(editData.Where(a => a.ITEM_CODE == newItem.SrcItemLocQty.ItemCode && a.BACK_ITEM_TYPE == newItem.SrcItemLocQty.BankItemType && ((newItem.IsGood == "1" && a.GOOD_BACK_QTY > 0) || (newItem.IsGood != "1" && a.BREAK_BACK_QTY > 0)))
					.Select(a => new { BackNo = a.BACK_NO, BackItemType = a.BACK_ITEM_TYPE }).ToList());
				backInfos.AddRange(notModifyItems.Where(a => a.ITEM_CODE == newItem.SrcItemLocQty.ItemCode && a.BACK_ITEM_TYPE == newItem.SrcItemLocQty.BankItemType && a.SERIAL_NO == (string.IsNullOrEmpty(newItem.SrcItemLocQty.SerialNo) ? "0" : newItem.SrcItemLocQty.SerialNo) && newItem.IsGood == a.IS_GOOD)
					.Select(a => new { BackNo = a.BACK_NO, BackItemType = a.BACK_ITEM_TYPE }).ToList());
				foreach (var backInfo in backInfos)
				{
					yield return new F91020601
					{
						ALLOCATION_NO = newItem.SrcItemLocQty.AllocationNo,
						BACK_ITEM_TYPE = backInfo.BackItemType,
						BACK_NO = backInfo.BackNo,
						CUST_CODE = custCode,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						IS_GOOD = newItem.IsGood,
						ITEM_CODE = newItem.SrcItemLocQty.ItemCode,
						PROCESS_NO = processNo,
						QTY = (int)newItem.SrcItemLocQty.Qty,
						SERIAL_NO = string.IsNullOrEmpty(newItem.SrcItemLocQty.SerialNo) ? "0" : newItem.SrcItemLocQty.SerialNo,
					};
				}
			}
		}

		/// <summary>
		/// 是否存在回倉商品類行為揀料的序號綁儲位商品
		/// </summary>
		/// <param name="tmpList2"></param>
		/// <param name="serialBundleItems"></param>
		/// <returns></returns>
		private static bool ExistsBundleSerialLocMaterialItem(List<BackData> tmpList2, List<F1903> serialBundleItems)
		{
			var existsBundleSerialLocMaterialItemQuery = from f1913 in serialBundleItems
																									 join backData in tmpList2
																									 on f1913.ITEM_CODE equals backData.ITEM_CODE
																									 where backData.BACK_ITEM_TYPE == "1" // 回倉商品類型(0成品1揀料)
																									 select f1913;

			return existsBundleSerialLocMaterialItemQuery.Any();
		}

		private List<ItemSumQty> GetProcessedItems(string dcCode, string gupCode, string custCode, string processNo, string actionNo, string backItemType, string singleItemCode)
		{
			switch (actionNo)
			{
				case "A1": //序號刷驗
					var f910502rep = new F910502Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910502s = f910502rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9").ToList();
					return f910502s.GroupBy(a => new { a.ITEM_CODE, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = g.Key.ITEM_CODE, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A2": //組合商品
					var f910504rep = new F910504Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910504s = f910504rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9").ToList();
					if (backItemType == "0")  //成品
					{
						// 組合成品一定為組合後的商品，成品一定唯一品號，且不會是序號商品
						var combinNos = f910504s.Select(a => a.COMBIN_NO).Distinct();
						return new List<ItemSumQty> { new ItemSumQty { ITEM_CODE = singleItemCode, SERIAL_NO = "0", SumQty = combinNos.Count() } };
					}
					else //物料
						return f910504s.GroupBy(a => new { a.ITEM_CODE, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = g.Key.ITEM_CODE, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A3": //商品拆解
					var f910505rep = new F910505Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910505s = f910505rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9").ToList();
					if (backItemType == "0") //成品
						return f910505s.GroupBy(a => new { a.ITEM_CODE, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = g.Key.ITEM_CODE, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
					else //物料
					{
						// 拆解物料一定為組合的商品，物料一定唯一品號，且可能是序號商品或一般商品
						var combinNos = f910505s.Select(a => a.COMBIN_NO).Distinct();
						return new List<ItemSumQty> { new ItemSumQty { ITEM_CODE = singleItemCode, SERIAL_NO = "0", SumQty = combinNos.Count() } };
					}
				case "A4": //裝盒
									 // 會裝盒的流通加工單，一定不是組合或拆解，成品與物料為相同品號，且唯一品號
					var f910506rep = new F910506Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910506s = f910506rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9" && !string.IsNullOrEmpty(a.BOX_NO)).ToList();
					return f910506s.GroupBy(a => new { a.BOX_NO, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = singleItemCode, BOX_NO = g.Key.BOX_NO, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A5": //裝箱
									 // 會裝箱的流通加工單，一定不是組合或拆解，成品與物料為相同品號，且唯一品號
					var f910507rep = new F910507Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910507s = f910507rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9" && !string.IsNullOrEmpty(a.CASE_NO)).ToList();
					var boxNos = f910507s.Select(a => a.BOX_NO).Distinct().ToList();
					var f2501rep2 = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
					var f2501s2 = f2501rep2.InWithTrueAndCondition("BOX_SERIAL", boxNos, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
					return (from a in f2501s2
									join b in f910507s on a.BOX_SERIAL equals b.BOX_NO
									select new { b.CASE_NO, a.SERIAL_NO }).GroupBy(a => new { a.CASE_NO, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = singleItemCode, CASE_NO = g.Key.CASE_NO, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A6": //盒QC
									 // 會盒QC的流通加工單，一定不是組合或拆解，成品與物料為相同品號，且唯一品號
					var f910508rep = new F910508Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910508s = f910508rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9" && !string.IsNullOrEmpty(a.BOX_NO)).ToList();
					return f910508s.GroupBy(a => new { a.BOX_NO, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = singleItemCode, BOX_NO = g.Key.BOX_NO, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A7": //箱QC
									 // 會裝箱的流通加工單，一定不是組合或拆解，成品與物料為相同品號，且唯一品號
					var f910509rep = new F910509Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910509s = f910509rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9" && !string.IsNullOrEmpty(a.CASE_NO)).ToList();
					var boxNos3 = f910509s.Select(a => a.BOX_NO).Distinct().ToList();
					var f2501rep3 = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
					var f2501s3 = f2501rep3.InWithTrueAndCondition("BOX_SERIAL", boxNos3, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
					return (from a in f2501s3
									join b in f910509s on a.BOX_SERIAL equals b.BOX_NO
									select new { b.CASE_NO, a.SERIAL_NO }).GroupBy(a => new { a.CASE_NO, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = singleItemCode, CASE_NO = g.Key.CASE_NO, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				case "A8": //卡片QC
					var f910503rep = new F910503Repository(Schemas.CoreSchema, _wmsTransaction);
					var f910503s = f910503rep.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.PROCESS_NO == processNo && a.ISPASS == "1").Where(a => a.STATUS != "9").ToList();
					return f910503s.GroupBy(a => new { a.ITEM_CODE, a.SERIAL_NO }).Select(g => new ItemSumQty { ITEM_CODE = g.Key.ITEM_CODE, SERIAL_NO = g.Key.SERIAL_NO, SumQty = g.Count() }).ToList();
				default:
					return new List<ItemSumQty>();
			}
		}

		private ExecuteResult CheckBackSerialNos(List<string> goodSerialNos,
			List<string> breakSerialNos,
			List<string> notModifySerialNos,
			List<ItemSumQty> canBackMaterialItems,
			List<ItemSumQty> processedMaterialItems,
			List<ItemSumQty> finishedItems,
			List<ItemSumQty> backedFinishItems, string maxActionNo, int boxQty, int caseQty,
			List<string> goodCombineSerialNos,
			List<string> breakCombineSerialNos)
		{
			ExecuteResult result;
			var backSerialNos = goodSerialNos.Union(breakSerialNos).ToList();
			if (backSerialNos.Any(a => notModifySerialNos.Contains(a)))
			{
				result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_BackSerialNos };
				return result;
			}
			var canBackSerialNos = (from a in canBackMaterialItems
															join b in processedMaterialItems on new { a.ITEM_CODE, a.SERIAL_NO } equals new { b.ITEM_CODE, b.SERIAL_NO } into j
															from c in j.DefaultIfEmpty()
															where c == null
															&& a.SERIAL_NO != "0"
															select a.SERIAL_NO).Distinct().ToList();
			canBackSerialNos.AddRange((from a in finishedItems
																 join b in backedFinishItems on new { a.ITEM_CODE, a.SERIAL_NO } equals new { b.ITEM_CODE, b.SERIAL_NO } into j
																 from c in j.DefaultIfEmpty()
																 where c == null
																 && a.SERIAL_NO != "0"
																 select a.SERIAL_NO).Distinct().ToList());

			var combineSerialNos = goodCombineSerialNos.Concat(breakCombineSerialNos).ToList();
			if (backSerialNos.Any(a => !combineSerialNos.Contains(a) && !canBackSerialNos.Contains(a)))
			{
				result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_BackSerialNos_ExistBackStockSerialNo };
				return result;
			}

			if (maxActionNo == "A4" || maxActionNo == "A6")
			{
				var gBoxFinishedItems = finishedItems.GroupBy(a => new { a.BOX_NO });
				foreach (var gBoxFinishedItem in gBoxFinishedItems)
				{
					var goodContainsCount = gBoxFinishedItem.Count(a => goodSerialNos.Contains(a.SERIAL_NO));
					var breakContainsCount = gBoxFinishedItem.Count(a => breakSerialNos.Contains(a.SERIAL_NO));
					if ((goodContainsCount != boxQty && goodContainsCount != 0) || (breakContainsCount != boxQty && breakContainsCount != 0))
					{
						result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_GoodContainsBoxSerialNo };
						return result;
					}
				}
			}
			if (maxActionNo == "A5" || maxActionNo == "A7")
			{
				var gCaseFinishedItems = finishedItems.GroupBy(a => new { a.CASE_NO });
				foreach (var gCaseFinishedItem in gCaseFinishedItems)
				{
					var goodContainsCount = gCaseFinishedItem.Count(a => goodSerialNos.Contains(a.SERIAL_NO));
					var breakContainsCount = gCaseFinishedItem.Count(a => breakSerialNos.Contains(a.SERIAL_NO));
					if ((goodContainsCount != (boxQty * caseQty) && goodContainsCount != 0) || (breakContainsCount != (boxQty * caseQty) && breakContainsCount != 0))
					{
						result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_GoodContainsPackSerialNo };
						return result;
					}
				}
			}

			return new ExecuteResult { IsSuccessed = true };
		}

		

		private ExecuteResult CheckBackSerialNoCount(List<BackData> backDatas, List<F1903> serialBundleItems, List<F2501> backf2501s, bool isGood)
		{
			ExecuteResult result;

			if (!serialBundleItems.Any() && !backf2501s.Any())
			{
				return new ExecuteResult { IsSuccessed = true };
			}
			var serialBundleitemCodes = serialBundleItems.Select(a => a.ITEM_CODE);
			if (backf2501s.Any(a => !serialBundleitemCodes.Contains(a.ITEM_CODE)))
			{
				result = new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P910101Service_SerialBundleitemCodes_UploadSerialNo, (isGood) ? Properties.Resources.P910101Service_Good : Properties.Resources.P910101Service_Damage) };
				return result;
			}
			var bSerialBundleItems = (from a in backDatas
																join b in serialBundleItems on a.ITEM_CODE equals b.ITEM_CODE
																select a).GroupBy(a => new { a.ITEM_CODE })
																.Select(g => new { ItemCode = g.Key.ITEM_CODE, SumQty = g.Sum(s => (isGood) ? s.GOOD_BACK_QTY : s.BREAK_BACK_QTY) }).ToList();
			var backSerialItems = backf2501s.GroupBy(a => new { a.ITEM_CODE })
																	.Select(g => new { ItemCode = g.Key.ITEM_CODE, Qty = g.Count() }).ToList();

			var notSameQtyItems = (from a in bSerialBundleItems
														 join b in backSerialItems on a.ItemCode equals b.ItemCode
														 where a.SumQty != b.Qty
														 select a.ItemCode).ToList();
			if (notSameQtyItems.Any())
			{
				result = new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P910101Service_NotSameQtyItems, (isGood) ? Properties.Resources.P910101Service_Good : Properties.Resources.P910101Service_Damage, string.Join("、", notSameQtyItems)) };
				return result;
			}

			return new ExecuteResult { IsSuccessed = true };
		}

		private ExecuteResult CheckBoxCaseCount(List<string> actions, int boxQty, int caseQty)
		{
			ExecuteResult result;
			if (actions.Any(a => a == "A4" || a == "A6"))
			{
				if (boxQty == 0)
				{
					result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_PackQtyCantZero };
					return result;
				}
			}
			if (actions.Any(a => a == "A5" || a == "A7"))
			{
				if (caseQty == 0)
				{
					result = new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P910101Service_BoxQtyCantZero };
					return result;
				}
			}

			return new ExecuteResult { IsSuccessed = true };
		}

		public IQueryable<PickReport> GetPickTicketReport(string dcCode, string gupCode, string custCode, string processNo)
		{
			var repo = new F910205Repository(Schemas.CoreSchema);
			return repo.GetPickTicketReport(dcCode, gupCode, custCode, processNo);
		}

		/// <summary>
		/// 檢核加工回倉商品數是否正確
		/// </summary>
		/// <param name="f910201">加工單主檔</param>
		/// <param name="canBackMaterialItems">加工單已揀料料號與數量</param>
		/// <param name="nowBackDatas">本次要回倉的成品或物料資料</param>
		/// <param name="backFinishedDatas">已回倉的成品或物料資料</param>
		/// <returns></returns>
		private ExecuteResult CheckProcessItemQty(F910201 f910201, List<ItemSumQty> canBackMaterialItems, List<BackData> nowBackDatas, List<BackData> backFinishedDatas, string maxAction = "")
		{
			//canBackMaterialItems 加工單已揀料料號與數量=>只會有品號、序號、數量
			//groupBackMaterialItems 不看序號算出品號已揀料多少數量
			var groupBackMaterialItems = canBackMaterialItems.GroupBy(x => x.ITEM_CODE).Select(x => new ItemSumQty { ITEM_CODE = x.Key, SumQty = x.Sum(y => y.SumQty) }).ToList();

			//成品良品總數量
			long goodTotalQty = 0;
			//成品不良品總數量
			long badTotalQty = 0;
			//已回倉的成品
			var groupBackFinishedItem = backFinishedDatas.Where(x => x.BACK_ITEM_TYPE == "0").GroupBy(x => new { x.ITEM_CODE, x.BACK_NO } ).Select(x => new { ItemCode = x.Key.ITEM_CODE, GoodTotalQty = Convert.ToInt64(x.FirstOrDefault()?.GOOD_BACK_QTY) ,BadTotalQty = Convert.ToInt64(x.FirstOrDefault()?.BREAK_BACK_QTY) }).FirstOrDefault();
			if (groupBackFinishedItem != null)
			{
				goodTotalQty += groupBackFinishedItem.GoodTotalQty;
				badTotalQty += groupBackFinishedItem.BadTotalQty;
			}
			//本次回倉的成品
			var groupNowBackItem = nowBackDatas.Where(x => x.BACK_ITEM_TYPE == "0").GroupBy(x => new { x.ITEM_CODE, x.BACK_NO } ).Select(x => new { ItemCode = x.Key.ITEM_CODE, GoodTotalQty = Convert.ToInt64(x.FirstOrDefault()?.GOOD_BACK_QTY) , BadTotalQty = Convert.ToInt64(x.FirstOrDefault()?.BREAK_BACK_QTY) }).FirstOrDefault();
			if (groupNowBackItem != null)
			{
				goodTotalQty += groupNowBackItem.GoodTotalQty;
				badTotalQty += groupNowBackItem.BadTotalQty;
			}

            //扣掉有重複算的已回倉數量
            if (groupBackFinishedItem != null)
            {
                if (groupBackFinishedItem.GoodTotalQty > 0 && groupBackFinishedItem.BadTotalQty == 0)
                    goodTotalQty -= groupBackFinishedItem.GoodTotalQty;
                if (groupBackFinishedItem.GoodTotalQty == 0 && groupBackFinishedItem.BadTotalQty > 0)
                    badTotalQty -= groupBackFinishedItem.BadTotalQty;
            }

            //檢核成品數是否超過加工數量
            if ((goodTotalQty+badTotalQty) > f910201.PROCESS_QTY)
			{
				var unEnoughQty = (goodTotalQty + badTotalQty) - f910201.PROCESS_QTY;
				return new ExecuteResult(false, string.Format(Properties.Resources.P910101Service_BACK_STOCK_QTY_OVER_QTY, Environment.NewLine + string.Format(Properties.Resources.P910101Service_OVER, f910201.ITEM_CODE, unEnoughQty)));
			}
			//有組合編號
			if (!string.IsNullOrWhiteSpace(f910201.ITEM_CODE_BOM))
			{
				//取得BOM表設定
				var f910101Repo = new F910101Repository(Schemas.CoreSchema, _wmsTransaction);
				var f910102Repo = new F910102Repository(Schemas.CoreSchema, _wmsTransaction);
				var f910101 = f910101Repo.Find(x => x.GUP_CODE == f910201.GUP_CODE && x.CUST_CODE == f910201.CUST_CODE && x.BOM_NO == f910201.ITEM_CODE_BOM);
				var f910102s = f910102Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == f910201.GUP_CODE && x.CUST_CODE == f910201.CUST_CODE && x.BOM_NO == f910201.ITEM_CODE_BOM).ToList();

				if (f910101.BOM_TYPE == "0") //0:組合
				{
					//已回倉成品所使用的料號數量
					var usedMaterialItemQtys = (from o in f910102s
																			select new ItemSumQty
																			{
																				ITEM_CODE = o.MATERIAL_CODE,
																				SumQty = o.BOM_QTY * (goodTotalQty + badTotalQty)
																			}).ToList();
					//扣除轉成成品使用的料號
					foreach (var item in usedMaterialItemQtys)
					{
						var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == item.ITEM_CODE);
						groupBackMaterialItem.SumQty -= item.SumQty;
					}
				}
				else //拆解
				{
					//扣除未拆解成品數量
					var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == f910201.ITEM_CODE);
					groupBackMaterialItem.SumQty -= (goodTotalQty + badTotalQty);
					//產生已拆解成品料號
					var usedMaterialItemQtys = (from o in f910102s
																			select new ItemSumQty
																			{
																				ITEM_CODE = o.MATERIAL_CODE,
																				SumQty = o.BOM_QTY * groupBackMaterialItem.SumQty
																			}).ToList();
					//將已拆解成品料號寫入加工後剩餘的料號與成品數
					groupBackMaterialItems.AddRange(usedMaterialItemQtys);
				}
			}
			else
			{
				//如果有最大的加工順序編號(裝盒、裝箱)
				if (!string.IsNullOrWhiteSpace(maxAction))
				{
					switch (maxAction)
					{
						case "A4"://裝盒
						case "A6"://盒QC
							if (f910201.BOX_QTY > 0 && (goodTotalQty % f910201.BOX_QTY != 0 || badTotalQty % f910201.BOX_QTY != 0))
								return new ExecuteResult(false, Properties.Resources.P910101Service_GOOD_BACK_QTY_Pack);

							else
							{
								var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == f910201.ITEM_CODE);
								groupBackMaterialItem.SumQty -= (goodTotalQty+badTotalQty);
							}
							break;
						case "A5"://裝箱
						case "A7"://箱QC
							if (f910201.CASE_QTY > 0 && (goodTotalQty % f910201.CASE_QTY != 0 || badTotalQty % f910201.CASE_QTY != 0))
								return new ExecuteResult(false, Properties.Resources.P910101Service_GOOD_BACK_QTY_Box);
							else
							{
								var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == f910201.ITEM_CODE);
								groupBackMaterialItem.SumQty -= (goodTotalQty + badTotalQty);
							}
							break;
					}
				}
			}
			//已回倉的料號
			var groupBackFinishedMaterialItems = backFinishedDatas.Where(x => x.BACK_ITEM_TYPE == "1").GroupBy(x => x.ITEM_CODE).Select(x => new { ItemCode = x.Key, TotalQty = x.Sum(y => y.GOOD_BACK_QTY) + x.Sum(y => y.BREAK_BACK_QTY) });
			foreach (var item in groupBackFinishedMaterialItems)
			{
				var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == item.ItemCode);
				groupBackMaterialItem.SumQty -= item.TotalQty;
			}
			//本次回倉的料號
			var groupBackNowMaterialItems = nowBackDatas.Where(x => x.BACK_ITEM_TYPE == "1").GroupBy(x => x.ITEM_CODE).Select(x => new { ItemCode = x.Key, TotalQty = x.Sum(y => y.GOOD_BACK_QTY) + x.Sum(y => y.BREAK_BACK_QTY) });
			foreach (var item in groupBackNowMaterialItems)
			{
				var groupBackMaterialItem = groupBackMaterialItems.First(x => x.ITEM_CODE == item.ItemCode);
				groupBackMaterialItem.SumQty -= item.TotalQty;
			}

			var notEnoughItemMsgs = groupBackMaterialItems.Where(x => x.SumQty < 0).Select(notEnoughItem => string.Format(Properties.Resources.P910101Service_OVER, notEnoughItem.ITEM_CODE, -notEnoughItem.SumQty));
			if (notEnoughItemMsgs.Any())
				return new ExecuteResult(false, string.Format(Properties.Resources.P910101Service_BACK_STOCK_QTY_OVER_QTY, Environment.NewLine + string.Join(Environment.NewLine, notEnoughItemMsgs)));
			else
				return new ExecuteResult(true);
		}
	}
}

