using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F15;
using System.Data.Objects;
using Wms3pl.Datas.F19;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160102Service
	{
		private WmsTransaction _wmsTransaction;
		public P160102Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F161601DetailDatas> GetF161601DetailDatas(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema);
			var data = f161601Repo.GetF161601DetailDatas(dcCode, gupCode, custCode, rtnApplyNo);
			return data;
		}

		public IQueryable<F161401ReturnWarehouse> GetF161401ReturnWarehouse(string dcCode, string gupCode, string custCode, string returnNo, string locCode, string itemCode, string itemName)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema);
			return f161601Repo.GetF161401ReturnWarehouse(dcCode, gupCode, custCode, returnNo, locCode, itemCode, itemName);
		}

		public IQueryable<PrintF161601Data> GetPrintF161601Data(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema);
			return f161601Repo.GetPrintF161601Data(dcCode, gupCode, custCode, rtnApplyNo);
		}

		public ExecuteResult PrintP160102(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161602Repo = new F161602Repository(Schemas.CoreSchema, _wmsTransaction);

			var f161601 = f161601Repo.Find(item => item.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
				&& item.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
				&& item.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
				&& item.RTN_APPLY_NO == EntityFunctions.AsNonUnicode(rtnApplyNo));

			if (f161601 == null)
				return new ExecuteResult(false, Properties.Resources.P160102Service_Trans_ApplicateNo_NotFound);

			if (f161601.STATUS != "0")
				return new ExecuteResult(true, Properties.Resources.P160102Service_Trans_ApplicateStatus_Invalid);

			var f161602s = f161602Repo.GetF161602Exs(dcCode, gupCode, custCode, rtnApplyNo).ToList();

			if (!f161602s.Any())
				return new ExecuteResult(false, Properties.Resources.P160102Service_Trans_NoItem);

			// 建立調撥單後，就結案
			f161601.STATUS = "2";
			f161601.APPROVE_DATE = DateTime.Today;
			f161601.APPROVE_STAFF = Current.Staff;
			f161601.APPROVE_NAME = Current.StaffName;
			f161601Repo.Update(f161601);


			var stockService = new StockService();
			var isPass = false;
			var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
			try
			{
				var itemCodes = f161602s.Select(x => new ItemKey { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫盤損商品，請稍待再試");

				// 依目的倉別, 來源倉別產生調撥單
				var sharedService = new SharedService(_wmsTransaction);
				var returnF1913List = new List<F1913>();
				var allocationList = new List<ReturnNewAllocation>();

				foreach (var warehouseGroup in f161602s.GroupBy(item => new { item.WAREHOUSE_ID, item.SRC_WAREHOUSE_ID }))
				{
					var stockFilters = warehouseGroup.Select((x, index) => new StockFilter
					{
						DataId = index,
						LocCode = x.SRC_LOC,
						ItemCode = x.ITEM_CODE,
						SrcWarehouseId = warehouseGroup.Key.SRC_WAREHOUSE_ID,
						isAllowExpiredItem = true,
						EnterDates = x.ENTER_DATE.HasValue ? new List<DateTime> { x.ENTER_DATE.Value } : new List<DateTime>(),
						BoxCtrlNos = new List<string> { x.BOX_CTRL_NO },
						PalletCtrlNos = new List<string> { x.PALLET_CTRL_NO },
						ValidDates = x.VALID_DATE.HasValue ? new List<DateTime> { x.VALID_DATE.Value } : new List<DateTime>(),
						MakeNos = new List<string> { x.MAKE_NO },
						Qty = x.MOVED_QTY
					}).ToList();

					var srcLocMapTarLocs = warehouseGroup.Select((x, index) => new SrcLocMapTarLoc
					{
						DataId = index,
						SrcLocCode = x.SRC_LOC,
						ItemCode = x.ITEM_CODE,
						//20200214 不看退貨上架明細的指定建議儲位(此欄位已無使用)
						//TarLocCode = x.TRA_LOC,
						TarWarehouseId = warehouseGroup.Key.WAREHOUSE_ID,
						// TarBoxCtrlNo = x.TAR_BOX_CTRL_NO,
						//TarPalletCtrlNo = x.TAR_PALLET_CTRL_NO,
						//TarMakeNo = x.TAR_MAKE_NO,
						//TarValidDate = x.TAR_VALID_DATE
					}).ToList();

					//SourceType 21 = 退貨上架申請
					//Status 1 = 已列印調撥單
					//目的DC = 一定是相同來源物流中心
					var newAllocationParam = new NewAllocationItemParam
					{
						GupCode = f161601.GUP_CODE,
						CustCode = f161601.CUST_CODE,
						SrcDcCode = f161601.DC_CODE,
						TarDcCode = f161601.DC_CODE,
						AllocationDate = DateTime.Today,
						SourceType = "21",
						SourceNo = f161601.RTN_APPLY_NO,
						IsExpendDate = true,
						SrcWarehouseId = warehouseGroup.Key.SRC_WAREHOUSE_ID,
						TarWarehouseId = warehouseGroup.Key.WAREHOUSE_ID,
						SrcStockFilterDetails = stockFilters,
						SrcLocMapTarLocs = srcLocMapTarLocs,
						ReturnStocks = returnF1913List,
						AllocationType = Shared.Enums.AllocationType.Both,
					};
					var result = sharedService.CreateOrUpdateAllocation(newAllocationParam);
					if (result.Result.IsSuccessed)
					{
						allocationList.AddRange(result.AllocationList);
						returnF1913List = result.StockList;
					}
					else
						return result.Result;
				}

				return sharedService.BulkInsertAllocation(allocationList, returnF1913List);
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}
		}

		public ExecuteResult InsertP160102(F161601 addF161601, F161602[] addF161602s)
		{
			if (addF161601 == null || addF161602s == null || !addF161602s.Any())
				return new ExecuteResult(false, Properties.Resources.P160102Service_ParamError);

			var sharedService = new SharedService(_wmsTransaction);
			var f161601Repo = new F161601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161602Repo = new F161602Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161602List = new List<F161602>();
			addF161601.RTN_APPLY_NO = sharedService.GetNewOrdCode("M");
			addF161601.RTN_APPLY_DATE = DateTime.Today;
			addF161601.STATUS = "0";
			f161601Repo.Add(addF161601);

			var result = ToChangeVaildDateAndMakeNo(addF161601, addF161602s.ToList());
			if (!result.IsSuccessed)
				return result;
			int seq = 0;
			foreach (var item in addF161602s)
			{
				seq++;
				item.DC_CODE = addF161601.DC_CODE;
				item.GUP_CODE = addF161601.GUP_CODE;
				item.CUST_CODE = addF161601.CUST_CODE;
				item.RTN_APPLY_NO = addF161601.RTN_APPLY_NO;
				item.RTN_APPLY_SEQ = seq;
				item.VALID_DATE = item.TAR_VALID_DATE;
				item.MAKE_NO = item.TAR_MAKE_NO;
				f161602List.Add(item);
			}
			f161602Repo.BulkInsert(f161602List);
			return new ExecuteResult(true, Properties.Resources.P160102Service_Create_Trans_No);
		}

		public ExecuteResult UpdateP160102(F161601 editF161601, F161602[] editF161602s)
		{
			if (editF161601 == null || editF161602s == null || !editF161602s.Any())
				return new ExecuteResult(false, Properties.Resources.P160102Service_ParamError);

			var f161601Repo = new F161601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161602Repo = new F161602Repository(Schemas.CoreSchema, _wmsTransaction);

			var f161601 = f161601Repo.Find(item => item.DC_CODE == EntityFunctions.AsNonUnicode(editF161601.DC_CODE)
								&& item.GUP_CODE == EntityFunctions.AsNonUnicode(editF161601.GUP_CODE)
								&& item.CUST_CODE == EntityFunctions.AsNonUnicode(editF161601.CUST_CODE)
								&& item.RTN_APPLY_NO == EntityFunctions.AsNonUnicode(editF161601.RTN_APPLY_NO));

			if (f161601 == null || f161601.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P160102Service_TransStatus_UnabledEdit);

			f161601.MEMO = editF161601.MEMO;
			f161601Repo.Update(f161601);

			f161602Repo.Delete(f161601.DC_CODE, f161601.GUP_CODE, f161601.CUST_CODE, f161601.RTN_APPLY_NO);
			var result = ToChangeVaildDateAndMakeNo(f161601, editF161602s.ToList());
			if (!result.IsSuccessed)
				return result;

			var sharedService = new SharedService(_wmsTransaction);
			int seq = 0;
			var f161602List = new List<F161602>();
			foreach (var item in editF161602s)
			{
				seq++;
				item.DC_CODE = f161601.DC_CODE;
				item.GUP_CODE = f161601.GUP_CODE;
				item.CUST_CODE = f161601.CUST_CODE;
				item.RTN_APPLY_NO = f161601.RTN_APPLY_NO;
				item.RTN_APPLY_SEQ = seq;
				item.VALID_DATE = item.TAR_VALID_DATE;
				item.MAKE_NO = item.TAR_MAKE_NO;
				f161602List.Add(item);
			}
			f161602Repo.BulkInsert(f161602List);

			return new ExecuteResult(true, Properties.Resources.P160102Service_UpdateTrans);
		}

		private ExecuteResult ToChangeVaildDateAndMakeNo(F161601 f161601,List<F161602> f161602s)
		{
			var cacheF1913List = new List<F1913>();
			var f191301List = new List<F191301>();
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191301Repo = new F191301Repository(Schemas.CoreSchema, _wmsTransaction);
			var isInCache = false;
			foreach (var item in f161602s)
			{
				var oldStock = FindStock(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.SRC_LOC, item.ITEM_CODE, item.ENTER_DATE.Value, item.VALID_DATE.Value, item.VNR_CODE, "0", item.MAKE_NO, item.BOX_CTRL_NO, item.PALLET_CTRL_NO, ref cacheF1913List,ref isInCache);
				if (oldStock == null)
					return new ExecuteResult(false, string.Format(Properties.Resources.P160102Service_SourceLocItemStockIsEmpty, item.SRC_LOC,item.ITEM_CODE));
				if(oldStock.QTY < item.MOVED_QTY)
					return new ExecuteResult(false, string.Format(Properties.Resources.P160102Service_SourceLocItemStockQtyisNotEnoughToUpShelfQty, item.SRC_LOC, item.ITEM_CODE));

				//沒有異動就跳過
				if (item.VALID_DATE == item.TAR_VALID_DATE && item.MAKE_NO == item.TAR_MAKE_NO)
					continue;

				var isChangeVaildDate = oldStock.VALID_DATE != item.TAR_VALID_DATE;
				var isChangeMakeNo = oldStock.MAKE_NO != item.TAR_MAKE_NO;
				if (isChangeVaildDate)
				{
					var f191301 = CreateF191301(oldStock);
					f191301.NEW_VALUE = item.TAR_VALID_DATE.Value.ToString("yyyy/MM/dd");
					f191301.WH_FIELD = "VALID_DATE";
					f191301.WH_REASON = "T01";
					f191301.WMS_NO = f161601.RTN_APPLY_NO;
					f191301List.Add(f191301);
				}
				if (isChangeMakeNo)
				{
					var f191301 = CreateF191301(oldStock);
					f191301.NEW_VALUE = item.TAR_MAKE_NO;
					f191301.WH_FIELD = "MAKE_NO";
					f191301.WH_REASON = "T05";
					f191301.WMS_NO = f161601.RTN_APPLY_NO;
					f191301List.Add(f191301);
				}

				//原庫存減少上架數
				oldStock.QTY -= item.MOVED_QTY;
				if (!isInCache)
					cacheF1913List.Add(oldStock);

				var newStock = FindStock(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.SRC_LOC, item.ITEM_CODE, item.ENTER_DATE.Value, item.TAR_VALID_DATE.Value, item.VNR_CODE, "0", string.IsNullOrWhiteSpace(item.TAR_MAKE_NO) ? "0" : item.TAR_MAKE_NO, item.BOX_CTRL_NO, item.PALLET_CTRL_NO, ref cacheF1913List,ref isInCache);
				if (newStock == null)
				{
					newStock = CreateF1913(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.SRC_LOC, item.ITEM_CODE, item.ENTER_DATE.Value, item.TAR_VALID_DATE.Value, item.VNR_CODE, "0", string.IsNullOrWhiteSpace(item.TAR_MAKE_NO) ? "0" : item.TAR_MAKE_NO, item.BOX_CTRL_NO, item.PALLET_CTRL_NO);
					newStock.QTY = item.MOVED_QTY;
					cacheF1913List.Add(newStock);
				}
				else
				{
					newStock.QTY += item.MOVED_QTY;
					if (!isInCache)
						cacheF1913List.Add(newStock);
				}
			}

			if (f191301List.Any())
				f191301Repo.BulkInsert(f191301List, "SEQ");
			var delF1913List = cacheF1913List.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF) && x.QTY <= 0).ToList();
			if(delF1913List.Any())
				f1913Repo.AsForUpdate().DeleteDataByBulkDelete(delF1913List);

			f1913Repo.AsForUpdate().BulkInsert(cacheF1913List.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF) && x.QTY > 0).ToList());
			f1913Repo.AsForUpdate().BulkUpdate(cacheF1913List.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF) && x.QTY > 0).ToList());
			return new ExecuteResult(true);
		}

		private F1913 FindStock(string dcCode,string gupCode,string custCode,string locCode,string itemCode,DateTime enterDate,DateTime validDate, string vnrCode, string serialNo, string makeNo,string boxCtrlNo,string palletCtrlNo, ref List<F1913> cacheF1913List,ref bool isInCache)
		{
			isInCache = false;
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913 = cacheF1913List.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.LOC_CODE == locCode && x.ITEM_CODE == itemCode && x.ENTER_DATE == enterDate && x.VALID_DATE == validDate && x.VNR_CODE == vnrCode && x.SERIAL_NO == serialNo && x.MAKE_NO == makeNo && x.BOX_CTRL_NO == boxCtrlNo && x.PALLET_CTRL_NO == palletCtrlNo);
			if (f1913 == null)
				f1913 = f1913Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, itemCode, locCode, validDate, enterDate, serialNo, vnrCode, boxCtrlNo, palletCtrlNo, makeNo);
			else
				isInCache = true;

			return f1913;

		}
		private F1913 CreateF1913(string dcCode, string gupCode, string custCode, string locCode, string itemCode, DateTime enterDate, DateTime validDate, string vnrCode, string serialNo, string makeNo, string boxCtrlNo, string palletCtrlNo)
		{
			return new F1913
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				LOC_CODE = locCode,
				ITEM_CODE = itemCode,
				ENTER_DATE = enterDate,
				VALID_DATE = validDate,
				VNR_CODE = vnrCode,
				SERIAL_NO = serialNo,
				MAKE_NO = makeNo,
				BOX_CTRL_NO = boxCtrlNo,
				PALLET_CTRL_NO = palletCtrlNo
			};
		}
		private F191301 CreateF191301(F1913 f1913)
		{
			return new F191301
			{
				DC_CODE = f1913.DC_CODE,
				GUP_CODE = f1913.GUP_CODE,
				CUST_CODE = f1913.CUST_CODE,
				ITEM_CODE = f1913.ITEM_CODE,
				LOC_CODE = f1913.LOC_CODE,
				BOX_CTRL_NO = f1913.BOX_CTRL_NO,
				PALLET_CTRL_NO = f1913.PALLET_CTRL_NO,
				MAKE_NO = f1913.MAKE_NO,
				SERIAL_NO = f1913.SERIAL_NO,
				QTY = f1913.QTY,
				VALID_DATE = f1913.VALID_DATE,
			};
		}
		public ExecuteResult DeleteP160102(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var f161601Repo = new F161601Repository(Schemas.CoreSchema, _wmsTransaction);

			var f161601 = f161601Repo.Find(item => item.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
				&& item.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
				&& item.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
				&& item.RTN_APPLY_NO == EntityFunctions.AsNonUnicode(rtnApplyNo));

			if (f161601 == null)
				return new ExecuteResult(false, Properties.Resources.P160102Service_Trans_ApplicateNo_NotFound_UnabledDelete);

			if (f161601.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P160102Service_TransStatus_UnabledDelete);

			f161601.STATUS = "9";
			f161601Repo.Update(f161601);
			return new ExecuteResult(true, Properties.Resources.P160102Service_DeletedTrans);
		}

		public ExecuteResult UpdateF1913ListValidDate(string dcCode, string gupCode, string custCode, List<F161601DetailDatas> upData)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Datas = f1913Repo.GetDatasByItems(dcCode, gupCode, custCode, upData.Select(o => o.ITEM_CODE).ToList());
			return new ExecuteResult(true);
		}
	}
}