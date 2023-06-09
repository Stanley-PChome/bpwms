
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P14.Services
{
	public partial class P140101Service
	{
		private WmsTransaction _wmsTransaction;
		private SharedService _sharedService;
    public CommonService CommonService { get; set; }

		public P140101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 計算查詢回來的盤點詳細的數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public int CountInventoryDetailItems(string dcCode, string gupCode, string custCode,
				string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var f141001Repo = new F140101Repository(Schemas.CoreSchema);
			var item = f141001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (item.ISSECOND == "1") //已產生複盤 抓複盤資料
			{
				var f140105Repo = new F140105Repository(Schemas.CoreSchema);
				return f140105Repo.CountInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
			}
			//初盤資料
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.CountInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
		}

		/// <summary>
		/// 盤點詳細查詢 只查前500筆
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode,
	string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var f141001Repo = new F140101Repository(Schemas.CoreSchema);
			var item = f141001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (item.ISSECOND == "1") //已產生複盤 抓複盤資料
			{
				var f140105Repo = new F140105Repository(Schemas.CoreSchema);
				return f140105Repo.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode, item.CHECK_TOOL);
			}
			//初盤資料
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode, item.CHECK_TOOL);
		}

		/// <summary>
		/// 盤點詳細查詢 匯出Excel
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItemsExport(string dcCode, string gupCode, string custCode,
			 string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var f141001Repo = new F140101Repository(Schemas.CoreSchema);
			var item = f141001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (item.ISSECOND == "1") //已產生複盤 抓複盤資料
			{
				var f140105Repo = new F140105Repository(Schemas.CoreSchema);
				return f140105Repo.GetInventoryDetailItemsExport(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode, item.CHECK_TOOL);
			}
			//初盤資料
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetInventoryDetailItemsExport(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode, item.CHECK_TOOL);
		}

		public IQueryable<InventoryDetailItem> FindInventoryDetailItems(string dcCode, string gupCode, string custCode,
	string inventoryNo, string locCode, string itemCode, DateTime enterDate, DateTime validDate, string makeNo)
		{
			var f141001Repo = new F140101Repository(Schemas.CoreSchema);
			var item = f141001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (item.ISSECOND == "1") //已產生複盤 抓複盤資料
			{
				var f140105Repo = new F140105Repository(Schemas.CoreSchema);
				return f140105Repo.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate, makeNo);
			}
			//初盤資料
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate, makeNo);
		}

		public ExecuteResult DeleteF140101(string dcCode, string gupCode, string custCode, string inventoryNo, string checkTool)
		{
			var result = new ExecuteResult { IsSuccessed = true, Message = "" };
			var f141001Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);

			var cancelRes = sharedService.CancelInventoryTask(dcCode, gupCode, custCode, inventoryNo, checkTool);
			if (cancelRes.IsSuccessed)
			{
				var item = f141001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
				item.STATUS = "9";//取消
				f141001Repo.Update(item);
			}
			else
				result = cancelRes;

			return result;
		}

		public IQueryable<InventoryItem> GetInventoryItems(string gupCode, string custCode, string type, string lType,
			string mType, string sType,string vnrCode,string vnrName, string itemCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			return f1903Repo.GetInventoryItems(gupCode, custCode, type, lType, mType, sType,vnrCode,vnrName, itemCode);
		}
		public IQueryable<InventoryWareHouse> GetInventoryWareHouses(string dcCode, string wareHouseType, string tool)
		{
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			return f1980Repo.GetInventoryWareHouses(dcCode, wareHouseType, tool);
		}

		public IQueryable<WareHouseFloor> GetWareHouseFloor()
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			return f1912Repo.GetWareHouseFloorList();
		}

		public IQueryable<WareHouseChannel> GetWareHouseChannel()
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			return f1912Repo.GetWareHouseChannelList();
		}

		public IQueryable<WareHousePlain> GetWareHousePlain()
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			return f1912Repo.GetWareHousePlainList();
		}

		public IQueryable<F140101Expansion> GetDatasExpansion(string dcCode, string gupCode, string custCode,
string inventoryNo, string inventoryType,
DateTime? inventorySDate, DateTime? inventoryEDate,
string inventoryCycle, string inventoryYear, string inventoryMonth, string status)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			return f140101Repo.GetDatasExpansion(dcCode, gupCode, custCode, inventoryNo, inventoryType, inventorySDate, inventoryEDate, inventoryCycle, inventoryYear, inventoryMonth, status);
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="f140101"></param>
    /// <param name="inventoryWareHouseList"></param>
    /// <param name="inventoryItemList"></param>
    /// <param name="IsOutSideCheckStock">已經在外部檢查過庫存</param>
    /// <returns></returns>
    public ExecuteResult InsertP140101(F140101 f140101, List<InventoryWareHouse> inventoryWareHouseList, List<InventoryItem> inventoryItemList)
    {
      var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140102Repo = new F140102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140103Repo = new F140103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f14010101Repo = new F14010101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1909Reop = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema);
			var sharedService = new SharedService(_wmsTransaction);

			if (f140101.INVENTORY_TYPE == "1")
			{
				if (f140101Repo.IsExistDatasByTheSameCycleTimes(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
					f140101.INVENTORY_YEAR ?? 0, f140101.INVENTORY_MON ?? 0, f140101.CYCLE_TIMES ?? 0))
					return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P140101Service_SameCustYearMonthWarehouseIdExist };
				foreach (var inventoryWareHouse in inventoryWareHouseList)
				{
					if (f140102Repo.IsExistDatasByTheSameWareHouseChannel(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
						f140101.INVENTORY_YEAR ?? 0, f140101.INVENTORY_MON ?? 0, inventoryWareHouse.WAREHOUSE_ID,
												inventoryWareHouse.FLOOR_BEGIN, inventoryWareHouse.FLOOR_END,
												inventoryWareHouse.CHANNEL_BEGIN, inventoryWareHouse.CHANNEL_END,
												inventoryWareHouse.PLAIN_BEGIN, inventoryWareHouse.PLAIN_END))
					{
						return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P140101Service_SameCustYearMonthWarehouseIdDuplicate, inventoryWareHouse.WAREHOUSE_NAME) };
					}
				}
			}

			// 取得盤點商品
			var f1913ExList = f1913Repo.GetDatasByInventoryWareHouseList(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
				inventoryWareHouseList, inventoryItemList.Select(o => o.ITEM_CODE).ToList(), f140101.INVENTORY_TYPE, f140101.INVENTORY_DATE).ToList();

      //商品抽盤的話要檢查所有商品都有庫存才可建單
      if (f140101.INVENTORY_TYPE == "0")
      {
        var stockNotEnough = f1913ExList.GroupBy(g => g.ITEM_CODE)
          .Select(x => new
          {
            item_code = x.Key,
            QTY = x.Sum(a => a.QTY),
            UNMOVE_STOCK_QTY = x.Sum(a => a.UNMOVE_STOCK_QTY),
            ItemCodeAndLoc = x.Select(a => new { a.ITEM_CODE, a.LOC_CODE })
          })
          .Where(x => x.QTY == 0 && x.UNMOVE_STOCK_QTY == 0);

        //沒有庫存就回傳失敗訊息
        if (stockNotEnough.Any() || !f1913ExList.Any())
          return new ExecuteResult
          {
            IsSuccessed = false,
            Message = Properties.Resources.P140101Service_NoInventoryItem,
            No = string.Join("\r\n", stockNotEnough.SelectMany(a => a.ItemCodeAndLoc.Select(b => $"{b.ITEM_CODE},{b.LOC_CODE}")))
          };
      }

      // 過濾庫存數或虛擬未搬動庫存數有大於0的盤點商品
      f1913ExList = f1913ExList.Where(x => x.QTY > 0 || x.UNMOVE_STOCK_QTY > 0).ToList();

      var groupF1913Ex = (from o in f1913ExList
													group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.ENTER_DATE, o.VALID_DATE, o.WAREHOUSE_ID, o.LOC_CODE, o.ITEM_CODE, o.BOX_CTRL_NO, o.PALLET_CTRL_NO, o.MAKE_NO } into g
													select g).ToList();

			var addF140101List = new List<F140101>();
			var addF140102List = new List<F140102>();
			var addF140103List = new List<F140103>();
			var addF140104List = new List<F140104>();
			var addF14010101List = new List<F14010101>();
			var inventoryNos = new List<string>();

			var f1909Data = f1909Reop.GetDatasByTrueAndCondition(o => o.GUP_CODE == f140101.GUP_CODE && o.CUST_CODE == f140101.CUST_CODE).ToList();

			string flushbackByF1909 = "0";
			if (f1909Data.Any())
				flushbackByF1909 = f1909Data.FirstOrDefault().FLUSHBACK;

			if (f140101.CHECK_TOOL == "0")
			{
				// 人工，將勾選的倉合併產生一張盤點單

				var inventoryNo = sharedService.GetNewOrdCode("I");
				inventoryNos.Add(inventoryNo);

				#region F140101
				f140101.INVENTORY_NO = inventoryNo;
				f140101.STATUS = "0";
				f140101.ISCHARGE = "0";
				f140101.ISSECOND = "0";
				f140101.ITEM_CNT = f1913ExList.Select(o => o.ITEM_CODE).Distinct().Count();
				f140101.ITEM_QTY = f1913ExList.Select(o => o.QTY).Sum();
				addF140101List.Add(new F140101
				{
					INVENTORY_NO = f140101.INVENTORY_NO,
					INVENTORY_DATE = f140101.INVENTORY_DATE,
					ISCHARGE = f140101.ISCHARGE,
					FEE = f140101.FEE,
					INVENTORY_TYPE = f140101.INVENTORY_TYPE,
					INVENTORY_CYCLE = f140101.INVENTORY_CYCLE,
					INVENTORY_YEAR = f140101.INVENTORY_YEAR,
					INVENTORY_MON = f140101.INVENTORY_MON,
					CYCLE_TIMES = f140101.CYCLE_TIMES,
					SHOW_CNT = f140101.SHOW_CNT,
					STATUS = f140101.STATUS,
					ITEM_CNT = f140101.ITEM_CNT,
					ITEM_QTY = f140101.ITEM_QTY,
					MEMO = f140101.MEMO,
					PRINT_DATE = f140101.PRINT_DATE,
					POSTING_DATE = f140101.POSTING_DATE,
					ISSECOND = f140101.ISSECOND,
					DC_CODE = f140101.DC_CODE,
					GUP_CODE = f140101.GUP_CODE,
					CUST_CODE = f140101.CUST_CODE,
					CHECK_TOOL = f140101.CHECK_TOOL
				});
				#endregion

				#region F140102
				addF140102List.AddRange(inventoryWareHouseList.Select(x => new F140102
				{
					DC_CODE = f140101.DC_CODE,
					GUP_CODE = f140101.GUP_CODE,
					CUST_CODE = f140101.CUST_CODE,
					INVENTORY_NO = inventoryNo,
					AREA_CODE = x.AREA_CODE,
					WAREHOUSE_ID = x.WAREHOUSE_ID,
					FLOOR_BEGIN = x.FLOOR_BEGIN,
					FLOOR_END = x.FLOOR_END,
					CHANNEL_BEGIN = x.CHANNEL_BEGIN,
					CHANNEL_END = x.CHANNEL_END,
					PLAIN_BEGIN = x.PLAIN_BEGIN,
					PLAIN_END = x.PLAIN_END
				}).ToList());
				#endregion

				#region F140103
				if (f140101.INVENTORY_TYPE == "0" || f140101.INVENTORY_TYPE == "2")
				{
					addF140103List.AddRange(inventoryItemList.Select(x => new F140103
					{
						DC_CODE = f140101.DC_CODE,
						GUP_CODE = f140101.GUP_CODE,
						CUST_CODE = f140101.CUST_CODE,
						INVENTORY_NO = inventoryNo,
						ITEM_CODE = x.ITEM_CODE
					}).ToList());
				}
				#endregion

				#region F140104
				addF140104List.AddRange(groupF1913Ex.Select(item => new F140104
				{
					DC_CODE = item.Key.DC_CODE,
					GUP_CODE = item.Key.GUP_CODE,
					CUST_CODE = item.Key.CUST_CODE,
					INVENTORY_NO = inventoryNo,
					LOC_CODE = item.Key.LOC_CODE,
					ITEM_CODE = item.Key.ITEM_CODE,
					VALID_DATE = item.Key.VALID_DATE,
					ENTER_DATE = item.Key.ENTER_DATE,
					WAREHOUSE_ID = item.Key.WAREHOUSE_ID,
					QTY = item.Sum(c => c.QTY),
					BOX_CTRL_NO = item.Key.BOX_CTRL_NO,
					PALLET_CTRL_NO = item.Key.PALLET_CTRL_NO,
					MAKE_NO = item.Key.MAKE_NO,
					FLUSHBACK = flushbackByF1909,
					DEVICE_STOCK_QTY = item.Sum(x => x.QTY) + item.Sum(c => c.UNMOVE_STOCK_QTY),
					UNMOVE_STOCK_QTY = item.Sum(c => c.UNMOVE_STOCK_QTY)
				}).ToList());
				#endregion

				#region F14010101
				addF14010101List.AddRange(f1913ExList.Select(f1913Ex => new F14010101
				{
					DC_CODE = f1913Ex.DC_CODE,
					GUP_CODE = f1913Ex.GUP_CODE,
					CUST_CODE = f1913Ex.CUST_CODE,
					INVENTORY_NO = inventoryNo,
					LOC_CODE = f1913Ex.LOC_CODE,
					ITEM_CODE = f1913Ex.ITEM_CODE,
					VALID_DATE = f1913Ex.VALID_DATE,
					ENTER_DATE = f1913Ex.ENTER_DATE,
					QTY = f1913Ex.QTY,
					SERIAL_NO = f1913Ex.SERIAL_NO,
					BOX_CTRL_NO = f1913Ex.BOX_CTRL_NO,
					PALLET_CTRL_NO = f1913Ex.PALLET_CTRL_NO,
					MAKE_NO = f1913Ex.MAKE_NO
				}));
				#endregion

			}
			else
			{
				// 自動，依勾選的倉產生多張盤點單
				var warehouses = inventoryWareHouseList.Select(x => new { x.WAREHOUSE_ID, x.WAREHOUSE_NAME } ).Distinct().ToList();
        if (f1913ExList.All(x => x.QTY == 0))
          return new ExecuteResult { IsSuccessed = false, Message = "商品沒有實際庫存數" };
				warehouses.ForEach(warehouse => 
				{
					var currF1913ExList = f1913ExList.Where(x => x.WAREHOUSE_ID == warehouse.WAREHOUSE_ID);
					var currInventoryWareHouseList = inventoryWareHouseList.Where(x => x.WAREHOUSE_ID == warehouse.WAREHOUSE_ID);
					var currGroupF1913Ex = groupF1913Ex.Where(x => x.Key.WAREHOUSE_ID == warehouse.WAREHOUSE_ID);

					var itemCnt = currF1913ExList.Select(o => o.ITEM_CODE).Distinct().Count();
					var itemQty = currF1913ExList.Select(o => o.QTY).Sum();

					if (itemCnt > 0 && itemQty > 0)
					{
						var inventoryNo = sharedService.GetNewOrdCode("I");
						inventoryNos.Add(inventoryNo);

						#region F140101
						addF140101List.Add(new F140101
						{
							INVENTORY_NO = inventoryNo,
							INVENTORY_DATE = f140101.INVENTORY_DATE,
							ISCHARGE = "0",
							FEE = f140101.FEE,
							INVENTORY_TYPE = f140101.INVENTORY_TYPE,
							INVENTORY_CYCLE = f140101.INVENTORY_CYCLE,
							INVENTORY_YEAR = f140101.INVENTORY_YEAR,
							INVENTORY_MON = f140101.INVENTORY_MON,
							CYCLE_TIMES = f140101.CYCLE_TIMES,
							SHOW_CNT = f140101.SHOW_CNT,
							STATUS = "0",
							ITEM_CNT = itemCnt,
							ITEM_QTY = itemQty,
							MEMO = $"{warehouse.WAREHOUSE_NAME}{(string.IsNullOrWhiteSpace(f140101.MEMO) ? string.Empty : "_")}{f140101.MEMO}",
							PRINT_DATE = f140101.PRINT_DATE,
							POSTING_DATE = f140101.POSTING_DATE,
							ISSECOND = "0",
							DC_CODE = f140101.DC_CODE,
							GUP_CODE = f140101.GUP_CODE,
							CUST_CODE = f140101.CUST_CODE,
							CHECK_TOOL = f140101.CHECK_TOOL
						});
						#endregion

						#region F140102

						var currF140102List = currInventoryWareHouseList.Select(x => new F140102
						{
							DC_CODE = f140101.DC_CODE,
							GUP_CODE = f140101.GUP_CODE,
							CUST_CODE = f140101.CUST_CODE,
							INVENTORY_NO = inventoryNo,
							AREA_CODE = x.AREA_CODE,
							WAREHOUSE_ID = x.WAREHOUSE_ID,
							FLOOR_BEGIN = x.FLOOR_BEGIN,
							FLOOR_END = x.FLOOR_END,
							CHANNEL_BEGIN = x.CHANNEL_BEGIN,
							CHANNEL_END = x.CHANNEL_END,
							PLAIN_BEGIN = x.PLAIN_BEGIN,
							PLAIN_END = x.PLAIN_END
						}).ToList();

						addF140102List.AddRange(currF140102List);
						#endregion

						#region F140103
						addF140103List.AddRange(currF1913ExList.GroupBy(x => x.ITEM_CODE).Select(x => new F140103
						{
							DC_CODE = f140101.DC_CODE,
							GUP_CODE = f140101.GUP_CODE,
							CUST_CODE = f140101.CUST_CODE,
							INVENTORY_NO = inventoryNo,
							ITEM_CODE = x.Key
						}).ToList());
						#endregion

						#region F140104
						addF140104List.AddRange(currGroupF1913Ex.Select(item => new F140104
						{
							DC_CODE = item.Key.DC_CODE,
							GUP_CODE = item.Key.GUP_CODE,
							CUST_CODE = item.Key.CUST_CODE,
							INVENTORY_NO = inventoryNo,
							LOC_CODE = item.Key.LOC_CODE,
							ITEM_CODE = item.Key.ITEM_CODE,
							VALID_DATE = item.Key.VALID_DATE,
							ENTER_DATE = item.Key.ENTER_DATE,
							WAREHOUSE_ID = item.Key.WAREHOUSE_ID,
							QTY = item.Sum(c => c.QTY),
							UNMOVE_STOCK_QTY = item.Sum(c => c.UNMOVE_STOCK_QTY),
							BOX_CTRL_NO = item.Key.BOX_CTRL_NO,
							PALLET_CTRL_NO = item.Key.PALLET_CTRL_NO,
							MAKE_NO = item.Key.MAKE_NO,
							FLUSHBACK = flushbackByF1909
						}).ToList());
						#endregion

						#region F14010101
						addF14010101List.AddRange(currF1913ExList.Select(f1913Ex => new F14010101
						{
							DC_CODE = f1913Ex.DC_CODE,
							GUP_CODE = f1913Ex.GUP_CODE,
							CUST_CODE = f1913Ex.CUST_CODE,
							INVENTORY_NO = inventoryNo,
							LOC_CODE = f1913Ex.LOC_CODE,
							ITEM_CODE = f1913Ex.ITEM_CODE,
							VALID_DATE = f1913Ex.VALID_DATE,
							ENTER_DATE = f1913Ex.ENTER_DATE,
							QTY = f1913Ex.QTY,
							SERIAL_NO = f1913Ex.SERIAL_NO,
							BOX_CTRL_NO = f1913Ex.BOX_CTRL_NO,
							PALLET_CTRL_NO = f1913Ex.PALLET_CTRL_NO,
							MAKE_NO = f1913Ex.MAKE_NO
						}));
						#endregion

						#region F060401
						sharedService.CreateInventoryTask(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, inventoryNo, warehouse.WAREHOUSE_ID, f140101.ISSECOND);
						#endregion
					}
				});
			}

			if (addF140101List.Any())
				f140101Repo.BulkInsert(addF140101List);
			if (addF140102List.Any())
				f140102Repo.BulkInsert(addF140102List);
			if (addF140103List.Any())
				f140103Repo.BulkInsert(addF140103List);
			if (addF140104List.Any())
				f140104Repo.BulkInsert(addF140104List);
			if (addF14010101List.Any())
				f14010101Repo.BulkInsert(addF14010101List);

			return new ExecuteResult { IsSuccessed = true, Message = string.Join(",", inventoryNos) };
		}

		public ExecuteResult UpdateP140101(F140101 f140101, List<InventoryDetailItem> inventoryDetailItemList, string clientName)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f140101Repo.Find(
				o =>
					o.DC_CODE == f140101.DC_CODE && o.GUP_CODE == f140101.GUP_CODE && o.CUST_CODE == f140101.CUST_CODE &&
					o.INVENTORY_NO == f140101.INVENTORY_NO);
			item.ISCHARGE = f140101.ISCHARGE;
			item.FEE = f140101.FEE;
			item.MEMO = f140101.MEMO;
			item.SHOW_CNT = f140101.SHOW_CNT;
			f140101Repo.Update(item);

			if (f140101.ISSECOND == "0")
				UpdateF140104(f140101, inventoryDetailItemList, clientName);
			else if (f140101.ISSECOND == "1")
				UpdateF140105(f140101, inventoryDetailItemList, clientName);
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		private void UpdateF140104(F140101 f140101, List<InventoryDetailItem> inventoryDetailItemList, string clientName)
		{
			var delItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "D").ToList();
			var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f14010101Repo = new F14010101Repository(Schemas.CoreSchema, _wmsTransaction);


			foreach (var inventoryDetailItem in delItems)
			{
				//Todo makeNo To Key 查詢箱號用箱號查即可
				//箱號暫時以批號值塞入，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;

				f140104Repo.DeleteF140104(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO,
						inventoryDetailItem.LOC_CODE, inventoryDetailItem.ITEM_CODE, inventoryDetailItem.VALID_DATE,
						 inventoryDetailItem.ENTER_DATE, inventoryDetailItem.BOX_CTRL_NO, inventoryDetailItem.PALLET_CTRL_NO,
						 makeNo);
				f14010101Repo.AsForUpdate()
								.DeleteF14010101(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO,
												inventoryDetailItem.LOC_CODE, inventoryDetailItem.ITEM_CODE,
												inventoryDetailItem.VALID_DATE, inventoryDetailItem.ENTER_DATE,
												inventoryDetailItem.BOX_CTRL_NO, inventoryDetailItem.PALLET_CTRL_NO,
												makeNo);
			}
			var updateItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "E").ToList();
			var updF140104 = new List<F140104>();
			foreach (var inventoryDetailItem in updateItems)
			{
				//Todo makeNo To Key 查詢箱號用箱號查即可
				//箱號暫時以批號值塞入，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;

				var f140104 = f140104Repo.AsForUpdate().FindF140104(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
						f140101.INVENTORY_NO, inventoryDetailItem.LOC_CODE, inventoryDetailItem.ITEM_CODE,
						inventoryDetailItem.VALID_DATE, inventoryDetailItem.ENTER_DATE, inventoryDetailItem.BOX_CTRL_NO,
						inventoryDetailItem.PALLET_CTRL_NO, makeNo).FirstOrDefault();

				f140104.FIRST_QTY = inventoryDetailItem.FIRST_QTY;
				f140104.SECOND_QTY = inventoryDetailItem.SECOND_QTY;
				f140104.FLUSHBACK = inventoryDetailItem.FLUSHBACK;
				f140104.FST_INVENTORY_DATE = DateTime.Now;
				f140104.FST_INVENTORY_NAME = Current.StaffName;
				f140104.FST_INVENTORY_PC = clientName;
				f140104.FST_INVENTORY_STAFF = Current.Staff;
				updF140104.Add(f140104);
			}
			f140104Repo.BulkUpdate(updF140104);

			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var addItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "A").ToList();
			var addF140104List = new List<F140104>();
			var addF14010101List = new List<F14010101>();
			foreach (var inventoryDetailItem in addItems)
			{
				//Todo makeNo To Key 2019/05/06
				//板號、箱號、批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var boxCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.BOX_CTRL_NO) ? "0" : inventoryDetailItem.BOX_CTRL_NO;
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;
				var palletCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.PALLET_CTRL_NO) ? "0" : inventoryDetailItem.PALLET_CTRL_NO;

				var f140104 = new F140104
				{
					DC_CODE = f140101.DC_CODE,
					GUP_CODE = f140101.GUP_CODE,
					CUST_CODE = f140101.CUST_CODE,
					INVENTORY_NO = f140101.INVENTORY_NO,
					LOC_CODE = inventoryDetailItem.LOC_CODE,
					ITEM_CODE = inventoryDetailItem.ITEM_CODE,
					VALID_DATE = inventoryDetailItem.VALID_DATE,
					ENTER_DATE = inventoryDetailItem.ENTER_DATE,
					WAREHOUSE_ID = inventoryDetailItem.WAREHOUSE_ID,
					QTY = Convert.ToInt32(inventoryDetailItem.QTY),
					FIRST_QTY = inventoryDetailItem.FIRST_QTY,
					SECOND_QTY = inventoryDetailItem.SECOND_QTY,
					FLUSHBACK = inventoryDetailItem.FLUSHBACK,
					FST_INVENTORY_DATE = DateTime.Now,
					FST_INVENTORY_NAME = Current.StaffName,
					FST_INVENTORY_PC = clientName,
					FST_INVENTORY_STAFF = Current.Staff,
					MAKE_NO = makeNo,
					BOX_CTRL_NO = boxCtrlNo,
					PALLET_CTRL_NO = palletCtrlNo,
					DEVICE_STOCK_QTY = Convert.ToInt32(inventoryDetailItem.QTY),
					UNMOVE_STOCK_QTY = 0
				};
				addF140104List.Add(f140104);

				var data = f1913Repo.GetDatas(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, inventoryDetailItem.LOC_CODE,
						inventoryDetailItem.ITEM_CODE, inventoryDetailItem.VALID_DATE, inventoryDetailItem.ENTER_DATE, boxCtrlNo,
						palletCtrlNo, makeNo);
				foreach (var f1913 in data)
				{
					var f14010101 = new F14010101
					{
						DC_CODE = f1913.DC_CODE,
						GUP_CODE = f1913.GUP_CODE,
						CUST_CODE = f1913.CUST_CODE,
						INVENTORY_NO = f140101.INVENTORY_NO,
						LOC_CODE = f1913.LOC_CODE,
						ITEM_CODE = f1913.ITEM_CODE,
						VALID_DATE = f1913.VALID_DATE,
						ENTER_DATE = f1913.ENTER_DATE,
						SERIAL_NO = f1913.SERIAL_NO,
						QTY = f1913.QTY,
						MAKE_NO = f1913.MAKE_NO,
						BOX_CTRL_NO = f1913.BOX_CTRL_NO,
						PALLET_CTRL_NO = f1913.PALLET_CTRL_NO
					};
					addF14010101List.Add(f14010101);
				}
			}
			if (addF140104List.Any())
				f140104Repo.BulkInsert(addF140104List);
			if (addF14010101List.Any())
				f14010101Repo.BulkInsert(addF14010101List);
		}

		private void UpdateF140105(F140101 f140101, List<InventoryDetailItem> inventoryDetailItemList, string clientName)
		{
			var delItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "D").ToList();
			var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var inventoryDetailItem in delItems)
			{
				//Todo makeNo To Key 查詢箱號用箱號查即可
				//箱號暫時以批號值塞入，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;

				f140105Repo.DeleteF140105(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO, inventoryDetailItem.LOC_CODE, inventoryDetailItem.ITEM_CODE,
						inventoryDetailItem.VALID_DATE, inventoryDetailItem.ENTER_DATE, inventoryDetailItem.BOX_CTRL_NO, inventoryDetailItem.PALLET_CTRL_NO, makeNo);
			}
			var updateItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "E").ToList();
			var updF140105 = new List<F140105>();

			foreach (var inventoryDetailItem in updateItems)
			{
				//Todo makeNo To Key 查詢箱號用箱號查即可
				//箱號暫時以批號值塞入，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;

				var f140105 = f140105Repo.AsForUpdate().FindF140105(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
						f140101.INVENTORY_NO, inventoryDetailItem.LOC_CODE, inventoryDetailItem.ITEM_CODE,
						inventoryDetailItem.VALID_DATE, inventoryDetailItem.ENTER_DATE, inventoryDetailItem.BOX_CTRL_NO,
						inventoryDetailItem.PALLET_CTRL_NO, makeNo).FirstOrDefault();

				f140105.FIRST_QTY = inventoryDetailItem.FIRST_QTY;
				f140105.SECOND_QTY = inventoryDetailItem.SECOND_QTY;
				f140105.FLUSHBACK = inventoryDetailItem.FLUSHBACK;
				f140105.SEC_INVENTORY_DATE = DateTime.Now;
				f140105.SEC_INVENTORY_NAME = Current.StaffName;
				f140105.SEC_INVENTORY_PC = clientName;
				f140105.SEC_INVENTORY_STAFF = Current.Staff;

				updF140105.Add(f140105);
			}

			f140105Repo.BulkUpdate(updF140105);

			var addItems = inventoryDetailItemList.Where(o => o.ChangeStatus == "A").ToList();
			var addF140105List = new List<F140105>();

			foreach (var inventoryDetailItem in addItems)
			{
				//Todo makeNo To Key 2019/05/06
				//箱號暫時以批號值塞入，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var boxCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.BOX_CTRL_NO) ? "0" : inventoryDetailItem.BOX_CTRL_NO;
				var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;
				var palletCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.PALLET_CTRL_NO) ? "0" : inventoryDetailItem.PALLET_CTRL_NO;

				var f140105 = new F140105
				{
					DC_CODE = f140101.DC_CODE,
					GUP_CODE = f140101.GUP_CODE,
					CUST_CODE = f140101.CUST_CODE,
					INVENTORY_NO = f140101.INVENTORY_NO,
					LOC_CODE = inventoryDetailItem.LOC_CODE,
					ITEM_CODE = inventoryDetailItem.ITEM_CODE,
					VALID_DATE = inventoryDetailItem.VALID_DATE,
					ENTER_DATE = inventoryDetailItem.ENTER_DATE,
					WAREHOUSE_ID = inventoryDetailItem.WAREHOUSE_ID,
					QTY = Convert.ToInt32(inventoryDetailItem.QTY),
					FIRST_QTY = inventoryDetailItem.FIRST_QTY,
					SECOND_QTY = inventoryDetailItem.SECOND_QTY,
					FLUSHBACK = inventoryDetailItem.FLUSHBACK,
					SEC_INVENTORY_DATE = DateTime.Now,
					SEC_INVENTORY_NAME = Current.StaffName,
					SEC_INVENTORY_PC = clientName,
					SEC_INVENTORY_STAFF = Current.Staff,
					MAKE_NO = makeNo,
					BOX_CTRL_NO = boxCtrlNo,
					PALLET_CTRL_NO = palletCtrlNo,
					DEVICE_STOCK_QTY = Convert.ToInt32(inventoryDetailItem.QTY),
					UNMOVE_STOCK_QTY = 0
				};
				addF140105List.Add(f140105);
			}
			if (addF140105List.Any())
				f140105Repo.BulkInsert(addF140105List);
		}

		public ExecuteResult UpdateIsPrinted(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f140101Repo.Find(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			if (item.STATUS == "0")
			{
				item.PRINT_DATE = DateTime.Now;
				item.STATUS = "1";
				f140101Repo.Update(item);
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		public IQueryable<InventoryDetailItemsByIsSecond> GetReportData(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f140101Repo.Find(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);

			var result = f140104Repo.GetInventoryDetailItemsByIsSecond(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.INVENTORY_NO,
				item.ISSECOND).ToList();
			return result.AsQueryable();
		}

		public IQueryable<InventoryByLocDetail> GetReportData2(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f140101Repo.Find(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);

			return f140104Repo.GetInventoryByLocDetails(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.INVENTORY_NO,
				item.ISSECOND);
		}

    /// <summary>
    /// 檢查匯入的盤點品項是否存在
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="inventoryItemList"></param>
    /// <param name="mode">0:檢查品號(ITEM_CODE) 1:檢查客戶編號(CUST_ITEM_CODE)</param>
    /// <returns></returns>
    public CheckInventoryItemRes CheckInventoryItemExist(string gupCode, string custCode, List<string> inventoryItemList, string mode)
    {
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      List<F1903> f1903s;

      if (CommonService == null)
        CommonService = new CommonService();

      //先把品號轉大寫
      inventoryItemList.ForEach(x => x = x.ToUpper());

      //檢查品號模式
      if (mode == "0")
      {
        f1903s = f1903Repo.GetDatasByItems(gupCode, custCode, inventoryItemList).ToList();
        var notExistsItem = inventoryItemList.Except(f1903s.Select(x => x.ITEM_CODE));
        if (notExistsItem.Any())
          return new CheckInventoryItemRes
          {
            IsSuccessed = false,
            Message = $"貨主:{CommonService.GetGup(gupCode).GUP_NAME} 無此商品品號:{string.Join(",", notExistsItem)}"
          };
      }
      else if (mode == "1") //檢查客戶編號模式
      {
        f1903s = f1903Repo.GetDatasByCustItemCode(gupCode, custCode, inventoryItemList).ToList();
        var notExistsItem = inventoryItemList.Except(f1903s.Select(x => x.CUST_ITEM_CODE.ToUpper()));
        if (notExistsItem.Any())
          return new CheckInventoryItemRes
          {
            IsSuccessed = false,
            Message = $"貨主:{CommonService.GetGup(gupCode).GUP_NAME} 無此貨主品編:{string.Join(",", notExistsItem)}"
          };
      }
      else
        return new CheckInventoryItemRes
        {
          IsSuccessed = false,
          Message = "無法辨認的檢查模式"
        };

      var inventoryItems = f1903s.Select(x => new InventoryItem { ITEM_CODE = x.ITEM_CODE, ITEM_NAME = x.ITEM_NAME, CUST_ITEM_CODE = x.CUST_ITEM_CODE }).ToList();

      return new CheckInventoryItemRes { IsSuccessed = true, InventoryItems = inventoryItems };
    }

    #region 盤點單匯入

    /// <summary>
    /// 依照匯入盤點單結果檔回傳盤點單明細
    /// </summary>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="inventoryNo">盤點單號</param>
    /// <param name="importInventoryDetailItems">盤點單匯入結果清單</param>
    /// <returns></returns>
    public ImportInventoryDetailResult ImportInventoryDetailItems(string dcCode, string gupCode, string custCode, string inventoryNo, List<ImportInventoryDetailItem> importInventoryDetailItems)
		{
			//Repo
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.GetEnabledData(dcCode, gupCode, custCode, inventoryNo);
			var failDetailItems = new List<ExportFailInventoryDetailItem>();

			if (f140101 == null)
				return new ImportInventoryDetailResult { Result = new ExecuteResult(false, Properties.Resources.P140101Service_InventoryDetailItemsHaveDeleted) };

			var itemCodes = importInventoryDetailItems.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).Select(x => x.ItemCode).Distinct().ToList();
			var items = f1903Repo.GetDatasByItems(gupCode, custCode, itemCodes).ToList();
			var locs = f1912Repo.GetDatasByLocCodes(dcCode, gupCode, custCode, importInventoryDetailItems.Where(x => !string.IsNullOrWhiteSpace(x.LocCode)).Select(x => x.LocCode).Distinct().ToList()).ToList();
			var warehouses = f1980Repo.GetDatas(dcCode, locs.Select(x => x.WAREHOUSE_ID).Distinct().ToList()).ToList();
			var isSecond = f140101.ISSECOND == "1";
			var inventoryDetails = f140104Repo.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, isSecond, itemCodes);

			var returnList = new List<InventoryDetailItem>();

			int i = 1;
			foreach (var detailItem in importInventoryDetailItems)
			{
				string errMessage = string.Empty;
				if (!CheckData(items, locs, warehouses, isSecond, detailItem, f140101.CHECK_TOOL, ref errMessage))
				{
					failDetailItems.Add(new ExportFailInventoryDetailItem()
					{
						ROWNUM = i,
						ItemCode = detailItem.ItemCode,
						ITEM_NAME = detailItem.ITEM_NAME,
						ITEM_SPEC = detailItem.ITEM_SPEC,
						ITEM_SIZE = detailItem.ITEM_SIZE,
						ITEM_COLOR = detailItem.ITEM_COLOR,
						MAKE_NO = detailItem.MAKE_NO,
						ValidDate = detailItem.ValidDate,
						EnterDate = detailItem.EnterDate,
						WAREHOUSE_NAME = detailItem.WAREHOUSE_NAME,
						LocCode = detailItem.LocCode,
						QTY = detailItem.QTY,
						FIRST_QTY = detailItem.FIRST_QTY,
						SECOND_QTY = detailItem.SECOND_QTY,
						PALLET_CTRL_NO = "0",
						BOX_CTRL_NO = "0",
						UNMOVE_STOCK_QTY = detailItem.UNMOVE_STOCK_QTY,
						DEVICE_STOCK_QTY = f140101.CHECK_TOOL == "0" ? null : detailItem.DEVICE_STOCK_QTY,
						CUST_ITEM_CODE = detailItem.CUST_ITEM_CODE,
						EAN_CODE1 = detailItem.EAN_CODE1,
						EAN_CODE2 = detailItem.EAN_CODE2,
						EAN_CODE3 = detailItem.EAN_CODE3,
						FailMessage = errMessage
					});
					i++;
					continue;
				}
				var item = items.First(x => x.ITEM_CODE.ToUpper() == detailItem.ItemCode.ToUpper());
				var loc = locs.First(x => x.LOC_CODE.ToUpper() == detailItem.LocCode.ToUpper());
				var warehouse = warehouses.First(x => x.WAREHOUSE_ID == loc.WAREHOUSE_ID);
				var validDate = DateTime.Parse(detailItem.ValidDate);
				var enterDate = DateTime.Parse(detailItem.EnterDate);

				//Todo makeNo To Key 2019/05/06
				//板號、箱號、批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
				var palletCtrlNo = string.IsNullOrWhiteSpace(detailItem.PALLET_CTRL_NO) ? "0" : detailItem.PALLET_CTRL_NO;
				var boxCtrlNo = string.IsNullOrWhiteSpace(detailItem.BOX_CTRL_NO) ? "0" : detailItem.BOX_CTRL_NO;
				var makeNo = string.IsNullOrWhiteSpace(detailItem.MAKE_NO) ? "0" : detailItem.MAKE_NO.ToUpper();

				//由於Excel目前未加入版號、箱號欄位，所以暫時用批號查箱號，版號寫死用"0"去查
				var inventoryDetail = inventoryDetails.FirstOrDefault(x => x.LOC_CODE == detailItem.LocCode && x.ITEM_CODE == detailItem.ItemCode && x.ENTER_DATE == enterDate && x.VALID_DATE == validDate &&
				x.MAKE_NO.ToUpper() == makeNo && x.BOX_CTRL_NO == boxCtrlNo && x.PALLET_CTRL_NO == palletCtrlNo);

				if (inventoryDetail == null)
				{
					inventoryDetail = new InventoryDetailItem
					{
						ChangeStatus = "A",
						ITEM_CODE = item.ITEM_CODE,
						ITEM_NAME = item.ITEM_NAME,
						ITEM_COLOR = item.ITEM_COLOR,
						ITEM_SIZE = item.ITEM_SIZE,
						ITEM_SPEC = item.ITEM_SPEC,
						ENTER_DATE = enterDate,
						MAKE_NO = makeNo,
						VALID_DATE = validDate,
						WAREHOUSE_ID = loc.WAREHOUSE_ID,
						WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME,
						LOC_CODE = loc.LOC_CODE,
						QTY = 0,
						FIRST_QTY_ORG = 0,
						FIRST_QTY = isSecond ? (int?)null : int.Parse(detailItem.FIRST_QTY),
						SECOND_QTY_ORG = 0,
						SECOND_QTY = isSecond ? int.Parse(detailItem.SECOND_QTY) : (int?)null,
						FLUSHBACK_ORG = "0",
						FLUSHBACK = "0",
						FLUSHBACKNAME = Properties.Resources.P140101Service_Negative,
						BOX_CTRL_NO = boxCtrlNo,
						PALLET_CTRL_NO = palletCtrlNo,
						UNMOVE_STOCK_QTY = string.IsNullOrWhiteSpace(detailItem.UNMOVE_STOCK_QTY) ? 0 : Convert.ToInt32(detailItem.UNMOVE_STOCK_QTY),
						DEVICE_STOCK_QTY = f140101.CHECK_TOOL == "0" ? default(int?) : Convert.ToInt32(detailItem.DEVICE_STOCK_QTY),
						CUST_ITEM_CODE = item.CUST_ITEM_CODE,
						EAN_CODE1 = item.EAN_CODE1,
						EAN_CODE2 = item.EAN_CODE2,
						EAN_CODE3 = item.EAN_CODE3
					};
				}
				else
				{
					inventoryDetail.ChangeStatus = "E";
					inventoryDetail.FLUSHBACK = "0";
					if (isSecond)
						inventoryDetail.SECOND_QTY = int.Parse(detailItem.SECOND_QTY);
					else
						inventoryDetail.FIRST_QTY = int.Parse(detailItem.FIRST_QTY);

					inventoryDetail.UNMOVE_STOCK_QTY = Convert.ToInt32(detailItem.UNMOVE_STOCK_QTY);
					inventoryDetail.DEVICE_STOCK_QTY = f140101.CHECK_TOOL == "0" ? default(int?) : Convert.ToInt32(detailItem.DEVICE_STOCK_QTY);
				}
				returnList.Add(inventoryDetail);
			}

			return new ImportInventoryDetailResult
			{
				Result = new ExecuteResult(true),
				InventoryDetailItems = returnList,
				FailDetailItems = failDetailItems
			};
		}
		/// <summary>
		/// 資料檢查
		/// </summary>
		/// <param name="items">商品清單</param>
		/// <param name="locs">儲位清單</param>
		/// <param name="locs">倉別清單</param>
		/// <param name="isSecond">是否複盤</param>
		/// <param name="detailItem">匯入盤點結果明細</param>
		/// <returns></returns>
		private bool CheckData(List<F1903> items, List<F1912> locs, List<F1980> warehouses, bool isSecond, ImportInventoryDetailItem detailItem,string checkTool, ref string failMessage)
		{
			bool checkResult = true;

			var findItem = items.FirstOrDefault(x => x.ITEM_CODE.ToUpper() == detailItem.ItemCode.ToUpper());
			if (string.IsNullOrWhiteSpace(detailItem.ItemCode))
			{
				failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_ItemCodeNull);
				checkResult = false;
			}
			else
			{

				if (findItem == null)
				{
					failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_ItemCodeNotExit);
					checkResult = false;
				}
			}

			var findLoc = locs.FirstOrDefault(x => x.LOC_CODE.ToUpper() == detailItem.LocCode.ToUpper());
			if (string.IsNullOrWhiteSpace(detailItem.LocCode))
			{
				failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_LocCodeNull);
				checkResult = false;
			}
			else
			{
				if (findLoc == null)
				{
					failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_LocCodeNotExit);
					checkResult = false;
				}
				else
				{
					var f1980 = warehouses.Where(x => x.WAREHOUSE_ID == findLoc.WAREHOUSE_ID).FirstOrDefault();
					if (f1980 != null && ((checkTool == "0" && f1980.DEVICE_TYPE != "0") ||
					(checkTool != "0" && f1980.DEVICE_TYPE == "0")))
					{
						failMessage = MessageJoin(failMessage, checkTool == "0" ? "人工倉盤點單儲位不可以為自動倉儲位" : "自動倉盤點單儲位不可以為人工倉儲位");
						checkResult = false;
					}
				}
			}

			DateTime date;
			if (!DateTime.TryParse(detailItem.ValidDate ?? "", out date))
			{
				failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_ValidDateFail);
				checkResult = false;
			}

			if (!DateTime.TryParse(detailItem.EnterDate ?? "", out date))
			{
				failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_EnterDateFail);
				checkResult = false;
			}

			int qty;
			if (!int.TryParse(isSecond ? detailItem.SECOND_QTY : detailItem.FIRST_QTY, out qty))
			{
				failMessage = MessageJoin(failMessage, Properties.Resources.P140101Service_QTYFail);
				checkResult = false;
			}

			if (_sharedService == null)
				_sharedService = new SharedService(_wmsTransaction);

			if (findItem != null && findLoc != null)
			{
				var result = _sharedService.CheckLocCode(findLoc.LOC_CODE, findLoc.DC_CODE, findLoc.WAREHOUSE_ID, Current.Staff, findItem.ITEM_CODE, false);
				if (!result.IsSuccessed)
				{
					failMessage = MessageJoin(failMessage, result.Message);
					checkResult = false;
				}

				var f1980Repo = new F1980Repository(Schemas.CoreSchema);
				if (checkTool == "0" && f1980Repo.GetF1980ByLocCode(findLoc.DC_CODE, findLoc.LOC_CODE)?.DEVICE_TYPE =="1"  )
				{
					failMessage = MessageJoin(failMessage, "人工盤點單必須輸入人工倉儲位");
					checkResult = false;
				}
				else if(checkTool == "1" && f1980Repo.GetF1980ByLocCode(findLoc.DC_CODE, findLoc.LOC_CODE)?.DEVICE_TYPE != "1")
				{
					failMessage = MessageJoin(failMessage, "自動盤點單必須輸入自動倉儲位");
					checkResult = false;
				}
			}

			return checkResult;
		}

		private string MessageJoin(string message, string addMessage)
		{
			message = string.Format("{0}{1}", string.IsNullOrEmpty(message) ? "" : (message + ","), addMessage);
			return message;
		}
		#endregion

	}
}

