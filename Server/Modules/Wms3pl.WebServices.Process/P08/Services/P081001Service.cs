
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P081001Service
	{
		private WmsTransaction _wmsTransaction;
		public P081001Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		private void CreateOrUpdateF140110(F140101 f140101, string locCode, string itemCode = "******", string itemName = null, int? inventoryQty = null)
		{
			var f140110Repo = new F140110Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140110 =
				f140110Repo.Find(
					o =>
						o.DC_CODE == f140101.DC_CODE && o.GUP_CODE == f140101.GUP_CODE && o.CUST_CODE == f140101.CUST_CODE && o.INVENTORY_NO == f140101.INVENTORY_NO &&
						o.LOC_CODE == locCode && o.ISSECOND == f140101.ISSECOND && o.ITEM_CODE == itemCode) ??
				f140110Repo.Find(
					o =>
						o.DC_CODE == f140101.DC_CODE && o.GUP_CODE == f140101.GUP_CODE && o.CUST_CODE == f140101.CUST_CODE && o.INVENTORY_NO == f140101.INVENTORY_NO &&
						o.LOC_CODE == locCode && o.ISSECOND == f140101.ISSECOND && o.ITEM_CODE == "******"
						);
			if (f140110 == null)
			{
				f140110 = new F140110
				{
					DC_CODE = f140101.DC_CODE,
					GUP_CODE = f140101.GUP_CODE,
					CUST_CODE = f140101.CUST_CODE,
					INVENTORY_NO = f140101.INVENTORY_NO,
					LOC_CODE = locCode.Trim(),
					ITEM_CODE = itemCode,
					ITEM_NAME = itemName,
					ISSECOND = f140101.ISSECOND,
					INVENTORY_QTY = inventoryQty
				};
				f140110Repo.Add(f140110);
			}
			else
			{
				if (f140110.ITEM_CODE != "******")
				{
					f140110.INVENTORY_QTY = inventoryQty;
					f140110.STATUS = inventoryQty == null ? "0" : "1";
					f140110Repo.Update(f140110);
				}
				else
				{
					f140110Repo.UpdateItemForSql(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO,
						f140101.ISSECOND, locCode, itemCode, itemName, f140110.ITEM_CODE);
				}
			}
		}

		private F140104 CreateF140104(F140110 f140110, string wareHouseId, DateTime validDate, int? inventoryQty, string clientName)
		{
			var f140104 = new F140104
			{
				DC_CODE = f140110.DC_CODE,
				GUP_CODE = f140110.GUP_CODE,
				CUST_CODE = f140110.CUST_CODE,
				INVENTORY_NO = f140110.INVENTORY_NO,
				LOC_CODE = f140110.LOC_CODE,
				ITEM_CODE = f140110.ITEM_CODE,
				VALID_DATE = validDate,
				ENTER_DATE = DateTime.Today,
				WAREHOUSE_ID = wareHouseId,
				QTY = 0,
				FIRST_QTY = inventoryQty,
				FST_INVENTORY_STAFF = Current.Staff,
				FST_INVENTORY_NAME = Current.StaffName,
				FST_INVENTORY_DATE = DateTime.Today,
				FST_INVENTORY_PC = clientName,
			};
			return f140104;
		}

		private F140105 CreateF140105(F140110 f140110, string wareHouseId, DateTime validDate, int? inventoryQty, string clientName)
		{
			var f140105 = new F140105
			{
				DC_CODE = f140110.DC_CODE,
				GUP_CODE = f140110.GUP_CODE,
				CUST_CODE = f140110.CUST_CODE,
				INVENTORY_NO = f140110.INVENTORY_NO,
				LOC_CODE = f140110.LOC_CODE,
				ITEM_CODE = f140110.ITEM_CODE,
				VALID_DATE = validDate,
				ENTER_DATE = DateTime.Today,
				WAREHOUSE_ID = wareHouseId,
				QTY = 0,
				SECOND_QTY = inventoryQty,
				SEC_INVENTORY_STAFF = Current.Staff,
				SEC_INVENTORY_NAME = Current.StaffName,
				SEC_INVENTORY_DATE = DateTime.Today,
				SEC_INVENTORY_PC = clientName,
			};
			return f140105;
		}

		#region 刷讀儲位

		public InventoryScanLoc GetInventoryScanLoc(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (f140101 == null)
				return new InventoryScanLoc { IsSuccess = false, Message = Properties.Resources.P081001Service_F140101IsNull };
			if (f140101.STATUS != "0" && f140101.STATUS != "1" && f140101.STATUS != "2")
				return new InventoryScanLoc { IsSuccess = false, Message = Properties.Resources.P081001Service_F140101StatusError };
			var sharedService = new SharedService();
			var result = sharedService.CheckLocCode(dcCode, locCode, Current.Staff);
			if (!result.IsSuccessed)
				return new InventoryScanLoc { IsSuccess = false, Message = result.Message };

			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			var inventoryLoc = f140104Repo.GetInventoryLoc(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO, locCode, f140101.ISSECOND);
			if (inventoryLoc == null)
			{
				if (f140101.INVENTORY_TYPE == "2") //異動盤
					return new InventoryScanLoc { IsSuccess = false, Message = Properties.Resources.P081001Service_InventoryTypeError };
				var f1980Repo = new F1980Repository(Schemas.CoreSchema);
				var warehouse = f1980Repo.GetF1980ByLocCode(dcCode, locCode);
				inventoryLoc = new InventoryScanLoc
				{
					LOC_CODE = locCode,
					WAREHOUSE_ID = warehouse.WAREHOUSE_ID,
					WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME,
					TOTAL_CNT = 0,
					TOTAL_QTY = 0
				};
			}

			var f140110Repo = new F140110Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140110List =
				f140110Repo.GetDatasByTrueAndCondition(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo &&
						o.LOC_CODE == locCode && o.ISSECOND == f140101.ISSECOND);
			if (f140110List.Any(o => o.CRT_STAFF != Current.Staff || o.CRT_NAME != Current.StaffName))
				return new InventoryScanLoc { IsSuccess = false, Message = Properties.Resources.P081001Service_StaffNameError };

			CreateOrUpdateF140110(f140101, locCode);
			if (f140101.STATUS == "0")
			{
				f140101.STATUS = "1";
				f140101Repo.Update(f140101);
			}
			inventoryLoc.IsSuccess = true;
			return inventoryLoc;
		}
		#endregion

		#region 刷讀品號或序號

		public InventoryScanItem GetInventoryScanItem(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode, string itemCodeOrSerialNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var serialNoService = new SerialNoService();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1903Item = f1903Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode  && o.CUST_CODE == custCode).Where(o=>o.ITEM_CODE == itemCodeOrSerialNo || o.EAN_CODE1 == itemCodeOrSerialNo || o.EAN_CODE2 == itemCodeOrSerialNo || o.EAN_CODE3 == itemCodeOrSerialNo).FirstOrDefault();
			//商品主檔找不到就找序號檔
			var itemCode = (f1903Item == null) ? serialNoService.GetSerialItem(gupCode, custCode, itemCodeOrSerialNo).ItemCode : f1903Item.ITEM_CODE;
			if (string.IsNullOrEmpty(itemCode))
				return new InventoryScanItem { IsSuccess = false, Message = Properties.Resources.P081001Service_ItemCodeOrSerialNoNotExist };

			if (f1903Item == null) //代表此次刷讀為序號/盒號/箱號/儲值卡盒號
			{
				f1903Item = f1903Repo.Find(o => o.GUP_CODE == gupCode && o.ITEM_CODE == itemCode && o.CUST_CODE == custCode);
				var result = serialNoService.CheckBarCode(gupCode, custCode, itemCode, itemCodeOrSerialNo);
				if (!result.IsSuccessed)
					return new InventoryScanItem { IsSuccess = false, Message = result.Message };
			}

			var f140404Repo = new F140104Repository(Schemas.CoreSchema);
			var inventoryScanItem = f140404Repo.GetInventoryScanItem(dcCode, gupCode, custCode, inventoryNo, f140101.ISSECOND,
				locCode, itemCode);

			var itemService = new ItemService();
			var f190301Repo = new F190301Repository(Schemas.CoreSchema);
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909s = f1909Repo.GetDatasByDc(dcCode).Where(x => x.GUP_CODE == gupCode).ToList();
			var inBoxQty = 0;

			var f190301 = f190301Repo.GetItemUnits(gupCode, new List<string> { itemCode }).ToList();
			if (f1909s.Count > 0)
			{
				var f1909 = f1909s.First();
				if (!string.IsNullOrEmpty(f1909.PACKCOUNT_MAX_UNIT) && f1909.PACKCOUNT_MAX_UNIT != "0")
				{
					itemService.GetInBoxQty(f190301, int.Parse(f1909.PACKCOUNT_MAX_UNIT), ref inBoxQty);
				}
			}

			if (inventoryScanItem == null)
			{
				if (f140101.INVENTORY_TYPE == "2") //異動盤
					return new InventoryScanItem { IsSuccess = false, Message = Properties.Resources.P081001Service_ItemInventoryTypeError };
				var f91000302Repo = new F91000302Repository(Schemas.CoreSchema);
				var f91000302Item = f91000302Repo.Find(o => o.ITEM_TYPE_ID == "001" && o.ACC_UNIT == f1903Item.ITEM_UNIT);
				inventoryScanItem = new InventoryScanItem
				{
					INVENTORY_QTY = 0,
					ITEM_CODE = f1903Item.ITEM_CODE,
					ITEM_NAME = f1903Item.ITEM_NAME,
					ITEM_COLOR = f1903Item.ITEM_COLOR,
					ITEM_SIZE = f1903Item.ITEM_SIZE,
					ITEM_SPEC = f1903Item.ITEM_SPEC,
					ITEM_UNIT = f91000302Item == null ? "" : f91000302Item.ACC_UNIT_NAME
				};
			}

			inventoryScanItem.INBOXTQTY = inBoxQty;

			if (f1909s.Count > 0)
			{
				var f1909 = f1909s.First();
				inventoryScanItem.PACKCOUNT_MAX_UNIT = f1909.PACKCOUNT_MAX_UNIT ?? "0";

				//顯示行動盤點畫面中最大單位數量 (maxUnit單位)、最小單位數量 (minUnit單位)
				if (f190301.Count > 0)
				{
					var boxLevelItemUnit = f190301.FirstOrDefault(x => x.UNIT_LEVEL == int.Parse(f1909.PACKCOUNT_MAX_UNIT));
					if (boxLevelItemUnit == null)
						boxLevelItemUnit = f190301.OrderByDescending(x => x.UNIT_LEVEL).First();

					var maxUnit = boxLevelItemUnit.UNIT_NAME;
					var minUnit = f190301.OrderBy(x => x.UNIT_LEVEL).First().UNIT_NAME;

					inventoryScanItem.MAXUNIT = "(" + maxUnit + ")";
					inventoryScanItem.MINUNIT = "(" + minUnit + ")";
				}
			}
			else
			{
				inventoryScanItem.PACKCOUNT_MAX_UNIT = "0";
				inventoryScanItem.MAXUNIT = "";
				inventoryScanItem.MINUNIT = "";
			}

			CreateOrUpdateF140110(f140101, locCode, f1903Item.ITEM_CODE, f1903Item.ITEM_NAME);
			inventoryScanItem.IsSuccess = true;
			return inventoryScanItem;
		}

		#endregion

		#region 輸入盤點數

		public InventoryItemQty UpdateToGetInventoryItemQty(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode, string itemCode, int qty)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var f140404Repo = new F140104Repository(Schemas.CoreSchema);
			var inventoryScanItem = f140404Repo.GetInventoryScanItem(dcCode, gupCode, custCode, inventoryNo, f140101.ISSECOND,
				locCode, itemCode);
			if (inventoryScanItem == null)
			{
				if (f140101.INVENTORY_TYPE == "2") //異動盤
					return new InventoryItemQty { IsSuccess = false, Message = Properties.Resources.P081001Service_ItemInventoryTypeError };
			}

			var f140110Repo = new F140110Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f140110Repo.Find(
				o =>
					o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo && o.LOC_CODE == locCode &&
					o.ITEM_CODE == itemCode && o.ISSECOND == f140101.ISSECOND);

			CreateOrUpdateF140110(f140101, item.LOC_CODE, item.ITEM_CODE, item.ITEM_NAME, qty);
			return new InventoryItemQty
			{
				ITEM_CODE = item.ITEM_CODE,
				ITEM_NAME = item.ITEM_NAME,
				QTY = qty,
				IsSuccess = true,
				Message = ""
			};
		}


		#endregion

		#region 清除盤點數

		public ExecuteResult ClearInventoryItemQty(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
			string itemCode)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);

			var f140110Repo = new F140110Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f140110Repo.Find(
				o =>
					o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo && o.LOC_CODE == locCode &&
					o.ITEM_CODE == itemCode && o.ISSECOND == f140101.ISSECOND);
			CreateOrUpdateF140110(f140101, item.LOC_CODE, item.ITEM_CODE, item.ITEM_NAME);
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		#endregion

		#region  更新初盤數或複盤數
		public ExecuteResult UpdateToF140104OrF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode, string clientName)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);

			var f140110Repo = new F140110Repository(Schemas.CoreSchema, _wmsTransaction);
			f140110Repo.DeleteByLocNotInventory(dcCode, gupCode, custCode, inventoryNo, f140101.ISSECOND, locCode, Current.Staff,
				Current.StaffName);
			var data = f140110Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo && o.ISSECOND == f140101.ISSECOND && o.LOC_CODE == locCode && o.STATUS == "1" && o.CRT_STAFF == Current.Staff && o.CRT_NAME == Current.StaffName).ToList();
			var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f1980Item = f1980Repo.GetF1980ByLocCode(dcCode, locCode);

			foreach (var f140110 in data)
			{
				if (f140101.ISSECOND == "0") //初盤
				{
					var f140104List =
						f140104Repo.AsForUpdate()
							.GetDatasByTrueAndCondition(
								o =>
									o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo &&
									o.LOC_CODE == f140110.LOC_CODE && o.ITEM_CODE == f140110.ITEM_CODE).ToList();
					if (!f140104List.Any())
						f140104Repo.Add(CreateF140104(f140110, f1980Item.WAREHOUSE_ID, DateTime.Parse("9999/12/31").Date, f140110.INVENTORY_QTY, clientName));
					else
					{
						var maxValidDate = f140104List.Max(o => o.VALID_DATE);
						var inventoryQty = f140110.INVENTORY_QTY;
						f140104List = f140104List.OrderByDescending(o => o.VALID_DATE).ToList();
						foreach (var f140104 in f140104List)
						{
							f140104.FIRST_QTY = (inventoryQty ?? 0) >= f140104.QTY ? (int)f140104.QTY : inventoryQty;
							inventoryQty -= f140104.FIRST_QTY ?? 0;
							f140104.FST_INVENTORY_DATE = DateTime.Now;
							f140104.FST_INVENTORY_STAFF = Current.Staff;
							f140104.FST_INVENTORY_NAME = Current.StaffName;
							f140104.FST_INVENTORY_PC = clientName;
							if (f140110.INVENTORY_QTY > 0 && inventoryQty == 0)
								inventoryQty = null;
						}
						if (inventoryQty > 0) //盤盈
						{
							var item =
								f140104List.FirstOrDefault(
									o =>
										o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo &&
										o.LOC_CODE == f140110.LOC_CODE && o.ITEM_CODE == f140110.ITEM_CODE &&
										o.VALID_DATE == maxValidDate && o.ENTER_DATE == DateTime.Today);
							if (item == null)
								f140104Repo.Add(CreateF140104(f140110, f1980Item.WAREHOUSE_ID, maxValidDate, inventoryQty, clientName));
							else
								item.FIRST_QTY += inventoryQty;
						}
						foreach (var f140104 in f140104List)
							f140104Repo.Update(f140104);
					}
				}
				else //複盤
				{
					var f140105List =
						f140105Repo.AsForUpdate()
							.GetDatasByTrueAndCondition(
								o =>
									o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo &&
									o.LOC_CODE == f140110.LOC_CODE && o.ITEM_CODE == f140110.ITEM_CODE).ToList();
					if (!f140105List.Any())
						f140105Repo.Add(CreateF140105(f140110, f1980Item.WAREHOUSE_ID, DateTime.Parse("9999/12/31").Date, f140110.INVENTORY_QTY, clientName));
					else
					{
						var maxValidDate = f140105List.Max(o => o.VALID_DATE);
						var inventoryQty = f140110.INVENTORY_QTY;
						f140105List = f140105List.OrderByDescending(o => o.VALID_DATE).ToList();
						foreach (var f140105 in f140105List)
						{
							f140105.SECOND_QTY = (inventoryQty ?? 0) >= f140105.QTY ? (int)f140105.QTY : inventoryQty;
							inventoryQty -= f140105.SECOND_QTY ?? 0;
							f140105.SEC_INVENTORY_DATE = DateTime.Now;
							f140105.SEC_INVENTORY_STAFF = Current.Staff;
							f140105.SEC_INVENTORY_NAME = Current.StaffName;
							f140105.SEC_INVENTORY_PC = clientName;
							if (f140110.INVENTORY_QTY > 0 && inventoryQty == 0)
								inventoryQty = null;
						}
						if (inventoryQty > 0) //盤盈
						{
							var item =
								f140105List.FirstOrDefault(
									o =>
										o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo &&
										o.LOC_CODE == f140110.LOC_CODE && o.ITEM_CODE == f140110.ITEM_CODE &&
										o.VALID_DATE == maxValidDate && o.ENTER_DATE == DateTime.Today);
							if (item == null)
								f140105Repo.Add(CreateF140105(f140110, f1980Item.WAREHOUSE_ID, maxValidDate, inventoryQty, clientName));
							else
								item.SECOND_QTY += inventoryQty;
						}
						foreach (var f140105 in f140105List)
							f140105Repo.Update(f140105);
					}
				}
				f140110.STATUS = "0";
				f140110Repo.Update(f140110);
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		public IQueryable<InventoryLocItem> GetInventoryLocItems(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetInventoryLocItems(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO, f140101.ISSECOND);
		}
		#endregion

	}
}

