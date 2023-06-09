using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P14.Services
{
	public partial class P140104Service
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;
		public P140104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<InventoryDetailItemsByIsSecond> GetInventoryDetailItemsByIsSecond(string dcCode, string gupCode, string custCode,
			string inventoryNo, string isSecond, string wareHouseId, string itemCodes, string differencesRangeStart, string differencesRangeEnd, string isRepotTag, string showCnt)
		{
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetInventoryDetailItemsByIsSecond(dcCode, gupCode, custCode,
				inventoryNo, isSecond, wareHouseId, itemCodes, differencesRangeStart, differencesRangeEnd, isRepotTag, "1", showCnt);
		}

		public IQueryable<P140102ReportData> GetP140102ReportData(string dcCode, string gupCode, string custCode, string inventoryNo, string isSecond)
		{
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			return f140104Repo.GetP140102ReportData(dcCode, gupCode, custCode, inventoryNo, isSecond);
		}

		public ExecuteResult CheckInventoryDetailHasEnterQty(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			var isHasEnterQty = f140104Repo.CheckInventoryDetailHasEnterQty(dcCode, gupCode, custCode, inventoryNo, f140101.ISSECOND);
			return new ExecuteResult(isHasEnterQty, (isHasEnterQty) ? "" : string.Format(Properties.Resources.P140104Service_InventoryNotFinish, inventoryNo));
		}

		public ExecuteResult CheckInventoryIsFinish(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);

			if (f140101.ISSECOND == "0")
			{
				var f140104Repo = new F140104Repository(Schemas.CoreSchema);
				var isHasEnterQty = f140104Repo.CheckFirstInventoryIsFinish(dcCode, gupCode, custCode, inventoryNo);
				return new ExecuteResult(isHasEnterQty, (isHasEnterQty) ? "" : string.Format(Properties.Resources.P140104Service_FirstInventoryNotFinish, inventoryNo));
			}

			if (f140101.ISSECOND == "1")
			{
				var f140105Repo = new F140105Repository(Schemas.CoreSchema);
				var isHasEnterQty = f140105Repo.CheckSecondInventoryIsExist(dcCode, gupCode, custCode, inventoryNo);
				return new ExecuteResult(isHasEnterQty, (isHasEnterQty) ? "" : string.Format(Properties.Resources.P140104Service_SecondInventoryNotExist, inventoryNo));
			}

			return new ExecuteResult(true);
		}


		public ExecuteResult ReInsertF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string clientName)
		{
			var result = new ExecuteResult { IsSuccessed = true, Message = "" };
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var statusData = f000904Repo.GetF000904Data("F140102", "STATUS");

			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060401Repo = new F060401Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			var data = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			//避免畫面停留過久，導致要[重新產生盤點單]時，狀態已經被變更至下一階段
			if (statusData.Select(x => x.VALUE).Contains(data.STATUS) == false)
			{
				return new ExecuteResult(false, Properties.Resources.P140104Service_StatusDataHasChanged);
			}
			//更新沒做初盤的都設為0
			var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransaction);
			var checkF140104 = f140104Repo.AsForUpdate().GetF140104FirstQtyNull(dcCode, gupCode, custCode, inventoryNo);
			foreach (var f140104 in checkF140104)
			{
				f140104.FIRST_QTY = 0;
				f140104.FST_INVENTORY_DATE = DateTime.Now;
				f140104.FST_INVENTORY_NAME = Current.StaffName;
				f140104.FST_INVENTORY_PC = clientName;
				f140104.FST_INVENTORY_STAFF = Current.Staff;
				f140104Repo.Update(f140104);
			}
			var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransaction);
			//先清除F140105相同的盤點單號資料
			f140105Repo.Delete(dcCode, gupCode, custCode, inventoryNo);

			//再從初盤資料F140104轉入
			if (data.CHECK_TOOL == "0") // 人工倉
				f140105Repo.InsertFromF140104ByManual(dcCode, gupCode, custCode, inventoryNo, Current.Staff, Current.StaffName);
			else
				f140105Repo.InsertFromF140104ByAuto(dcCode, gupCode, custCode, inventoryNo, Current.Staff, Current.StaffName);

			//同時更新F140101主檔，狀態=>2 (代表複盤)，是否有產生複盤資料=>1 (代表是)
			data.STATUS = "2";
			data.ISSECOND = "1";
			f140101Repo.Update(data);

			// F140101.CHECK_TOOL<> 0(非人工倉盤點)，寫入盤點任務資料表F060401。
			if (data.CHECK_TOOL != "0")
			{
				var f060401 = f060401Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == data.DC_CODE && o.GUP_CODE == data.GUP_CODE && o.CUST_CODE == data.CUST_CODE && o.WMS_NO == data.INVENTORY_NO).FirstOrDefault();
				if (f060401 != null)
					sharedService.CreateInventoryTask(data.DC_CODE, data.GUP_CODE, data.CUST_CODE, data.INVENTORY_NO, f060401.WAREHOUSE_ID, "1");
			}

			return result;
		}

		public ExecuteResult CheckF140105Exist(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var f140105Repo = new F140105Repository(Schemas.CoreSchema);
			var checkF140105Exist = f140105Repo.CheckF140105Exist(dcCode, gupCode, custCode, inventoryNo);
			return new ExecuteResult() { IsSuccessed = checkF140105Exist };
		}

		public IQueryable<InventoryQueryDataForDc> GetInventoryQueryDatasForDc(string dcCode, string gupCode, string custCode,
	string inventoryNo, string sortByCount, string warehouseId, string itemCodes)
		{
			var repF140101 = new F140101Repository(Schemas.CoreSchema);
			return repF140101.GetInventoryQueryDatasForDc(dcCode, gupCode, custCode, inventoryNo, sortByCount, warehouseId, itemCodes);
		}

		public ExecuteResult InsertF140106(string dcCode, string gupCode, string custCode, string inventoryNo, string clientName, List<InventoryDetailItemsByIsSecond> datas)
		{
			#region 變數資料
			var today = DateTime.Today;
			var result = new ExecuteResult { IsSuccessed = true, Message = "" };
			var sharedService = new SharedService(_wmsTransaction);
			var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140106Repo = new F140106Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140107Repo = new F140107Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200103Repo = new F200103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var returnNewAllocationListAutoDiff = new List<ReturnNewAllocation>();
			var returnNewAllocationList = new List<ReturnNewAllocation>();
			var adjustService = new AdjustOrderService(_wmsTransaction);
			var addF140107List = new List<F140107>();
			var adjustStockDetailList = new List<AdjustStockDetail>();

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var returnStocks = new List<F1913>();
			var errorMsg = new List<string>();
			var selectedDatas = datas.Where(x => x.IsSelected).ToList();
			var autoWhIsDiff = selectedDatas.All(x => Convert.ToInt32(x.DIFF_QTY) == 0); // 自動倉是否全都沒有盤差

			// 盤點單資料
			var f140101 = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
      if (f140101.STATUS == "5")
        return new ExecuteResult(false, "盤點單狀態為已結案，請重新查詢");

      // 取得盤點盤損倉
      var inventoryLossWHId = _commonService.GetSysGlobalValue(dcCode, gupCode, custCode, "InventoryLossWHId");
      if (string.IsNullOrWhiteSpace(inventoryLossWHId))
				return new ExecuteResult(false, Properties.Resources.P1401020000_InventoryLossWHIdNotSetting);

			// 取得盤點盤損倉第一個儲位資料
			var f1912 = f1912Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.WAREHOUSE_ID == inventoryLossWHId).FirstOrDefault();
			#endregion

			#region 避免畫面停留過久，導致要[盤點單結算]時，狀態已經被變更至下一階段
			var statusData = f000904Repo.GetF000904Data("F140102", "STATUS");

			var data = f140101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);

			if (statusData.Select(x => x.VALUE).Contains(data.STATUS) == false)
				return new ExecuteResult(false, Properties.Resources.P140104Service_StatusDataHasChanged);
			#endregion

			#region 盤損處理(產生調撥單)
			// 盤損的資料
			var lossDatas = selectedDatas.Where(x => Convert.ToInt32(x.STOCK_DIFF_QTY) < 0 && Convert.ToInt32(x.QTY) > 0).ToList();

			if (lossDatas.Any())
			{
				var stockService = new StockService();
				var isPass = false;
				var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
				try
				{
					var itemCodes = lossDatas.Select(x => new ItemKey { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, ItemCode = x.ITEM_CODE }).Distinct().ToList();
					isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
					if (!isPass)
						return new ExecuteResult(false, "仍有程序正在配庫盤損商品，請稍待再試");

					if (f140101.CHECK_TOOL == "0")
					{
						#region 人工倉，若有多筆來源倉庫，拆多筆調撥單
						var warehouseIds = lossDatas.Select(x => x.WAREHOUSE_ID).Distinct().ToList();

						warehouseIds.ForEach(warehouseId =>
						{
							var currWarehouseIdDatas = lossDatas.Where(x => x.WAREHOUSE_ID == warehouseId).ToList();

							var newAllocationParam = new NewAllocationItemParam
							{
								GupCode = gupCode,
								CustCode = custCode,
								AllocationDate = today,
								SourceType = "18",
								SourceNo = inventoryNo,
								IsExpendDate = true,
								SrcDcCode = dcCode,
								SrcWarehouseId = warehouseId,
								TarDcCode = dcCode,
								TarWarehouseId = inventoryLossWHId,
								AllocationType = AllocationType.Both,
								ReturnStocks = returnStocks,
								isIncludeResupply = true,
								SrcStockFilterDetails = currWarehouseIdDatas.Select((obj, index) => new StockFilter
								{
									DataId = index,
									ItemCode = obj.ITEM_CODE,
									LocCode = obj.LOC_CODE,
									Qty = Math.Abs(Convert.ToInt32(obj.STOCK_DIFF_QTY)) >= Convert.ToInt32(obj.QTY) ? Convert.ToInt32(obj.QTY) : Math.Abs(Convert.ToInt32(obj.STOCK_DIFF_QTY)),
									ValidDates = new List<DateTime> { obj.VALID_DATE },
									EnterDates = new List<DateTime> { obj.ENTER_DATE },
									BoxCtrlNos = new List<string> { "0" },
									MakeNos = string.IsNullOrWhiteSpace(obj.MAKE_NO) ? new List<string> { "0" } : new List<string> { obj.MAKE_NO?.Trim() },
									PalletCtrlNos = new List<string> { obj.PALLET_CTRL_NO }
								}).ToList(),
								SrcLocMapTarLocs = currWarehouseIdDatas.Select((obj, index) => new SrcLocMapTarLoc
								{
									DataId = index,
									ItemCode = obj.ITEM_CODE,
									SrcLocCode = obj.LOC_CODE,
									TarWarehouseId = inventoryLossWHId,
									TarLocCode = f1912 == null ? null : f1912.LOC_CODE,
									ValidDate = obj.VALID_DATE,
									EnterDate = obj.ENTER_DATE,
									MakeNo = string.IsNullOrWhiteSpace(obj.MAKE_NO) ? "0" : obj.MAKE_NO,
									PalletCtrlNo = obj.PALLET_CTRL_NO
								}).ToList()
							};

							var returnAllocationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam);

							if (returnAllocationResult.Result.IsSuccessed)
							{
								returnStocks = returnAllocationResult.StockList;

								if (returnAllocationResult.AllocationList.Any())
								{
									returnNewAllocationList.AddRange(returnAllocationResult.AllocationList);

									// 回填成功產生的調撥單號
									currWarehouseIdDatas.ForEach(obj => { obj.PROC_WMS_NO = returnAllocationResult.AllocationList[0].Master.ALLOCATION_NO; });
								}
							}
							else
							{
								errorMsg.Add(returnAllocationResult.Result.Message);
							}
						});
						#endregion
					}
					else
					{
						#region 自動倉，每一筆明細產生一筆調撥單
						lossDatas.ForEach(obj =>
						{
							// 庫差數取絕對值
							var inventoryDiffQty = Math.Abs(Convert.ToInt32(obj.STOCK_DIFF_QTY));
							var wmsQty = Convert.ToInt32(obj.QTY);
							// 調撥數為庫差數取絕對值，若調撥數>=WMS庫存數(QTY)，則調撥數=WMS庫存數(QTY)
							var qty = inventoryDiffQty >= wmsQty ? wmsQty : inventoryDiffQty;

							var newAllocationParam = new NewAllocationItemParam
							{
								GupCode = gupCode,
								CustCode = custCode,
								AllocationDate = today,
								SourceType = "18",
								SourceNo = inventoryNo,
								IsExpendDate = true,
								SrcDcCode = dcCode,
								SrcWarehouseId = obj.WAREHOUSE_ID,
								TarDcCode = dcCode,
								TarWarehouseId = inventoryLossWHId,
								AllocationType = AllocationType.Both,
								ReturnStocks = returnStocks,
								isIncludeResupply = true,
								SrcStockFilterDetails = new List<StockFilter>
							{
								new StockFilter
								{
									DataId = 0,
									ItemCode = obj.ITEM_CODE,
									LocCode = obj.LOC_CODE,
									Qty = qty,
									ValidDates = new List<DateTime> { obj.VALID_DATE },
									MakeNos = string.IsNullOrWhiteSpace(obj.MAKE_NO) ? new List<string> { "0" } : new List<string> { obj.MAKE_NO?.Trim() },
																		EnterDates = new List<DateTime> { obj.ENTER_DATE },
																		BoxCtrlNos = new List<string> { obj.BOX_CTRL_NO },
																		PalletCtrlNos = new List<string> { obj.PALLET_CTRL_NO }
								}
							},
								SrcLocMapTarLocs = new List<SrcLocMapTarLoc>
							{
								new SrcLocMapTarLoc
								{
									DataId = 0,
									ItemCode = obj.ITEM_CODE,
									SrcLocCode = obj.LOC_CODE,
									TarWarehouseId = inventoryLossWHId,
									TarLocCode = f1912 == null ? null : f1912.LOC_CODE,
									ValidDate = obj.VALID_DATE,
									MakeNo = string.IsNullOrWhiteSpace(obj.MAKE_NO) ? "0" : obj.MAKE_NO,
																		EnterDate = obj.ENTER_DATE,
																		BoxCtrlNo = obj.BOX_CTRL_NO,
																		PalletCtrlNo = obj.PALLET_CTRL_NO
								}
							}
							};

							var returnAllocationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam);

							if (returnAllocationResult.Result.IsSuccessed)
							{
								returnStocks = returnAllocationResult.StockList;

								if (returnAllocationResult.AllocationList.Any())
								{
									if ((Convert.ToInt32(obj.DIFF_QTY) != 0))
										returnNewAllocationListAutoDiff.AddRange(returnAllocationResult.AllocationList);
									else
										returnNewAllocationList.AddRange(returnAllocationResult.AllocationList);

									// 回填成功產生的調撥單號
									obj.PROC_WMS_NO = returnAllocationResult.AllocationList[0].Master.ALLOCATION_NO;
								}
							}
							else
							{
								errorMsg.Add(returnAllocationResult.Result.Message);
							}
						});
						#endregion
					}

					if (errorMsg.Any())
						return new ExecuteResult { IsSuccessed = false, Message = string.Join("\r\n", errorMsg) };

					#region 人工倉盤點
					if (f140101.CHECK_TOOL == "0")
					{
						// 將產生的調撥單並呼叫調撥共用函數BulkAllocationToAllUp()
						if (returnNewAllocationList.Any())
						{
							var addF140107Datas = new List<F140107>();

							sharedService.BulkAllocationToAllUp(returnNewAllocationList, returnStocks, false);

							returnNewAllocationList.ForEach(obj =>
							{
								// 新增庫存異常
								sharedService.CreateStockaBnormal(dcCode, gupCode, custCode, obj.Master, obj.Details);

								// 新增盤點單盤盤損
								sharedService.CreateF140107(dcCode, gupCode, custCode, obj.Master, obj.Details);
							});

							if (addF140107Datas.Any())
								f140107Repo.BulkInsert(addF140107Datas);
						}

					}
					#endregion

					#region 非人工倉盤點
					if (f140101.CHECK_TOOL != "0")
					{
						//自動倉沒有盤差
						//=>(盤損)調撥單直接上架 + 庫存異常處理
						//=> 盤點單狀態 = 5(結案)
						if (returnNewAllocationList.Any())
						{
							sharedService.BulkAllocationToAllDown(returnNewAllocationList);
							// 上架
							sharedService.BulkAllocationToAllUp(returnNewAllocationList, returnStocks, false);

							var addF140107Datas = new List<F140107>();

							returnNewAllocationList.ForEach(obj =>
							{
								// 庫存異常處理
								sharedService.CreateStockaBnormal(dcCode, gupCode, custCode, obj.Master, obj.Details);

								// 新增盤點單盤損
								sharedService.CreateF140107(dcCode, gupCode, custCode, obj.Master, obj.Details);
							});

							if (addF140107Datas.Any())
								f140107Repo.BulkInsert(addF140107Datas);
						}

						// 自動倉有盤差
						// 調撥單全部下架
						// 調整調撥單狀態為上架處理中
						// 將有差異的調撥單加回無差異清單中一起進行調撥單寫入DB
						if (returnNewAllocationListAutoDiff.Any())
						{

							sharedService.BulkAllocationToAllDown(returnNewAllocationListAutoDiff);
							// 將產生的調撥單更新狀態為上架處理中(4)
							returnNewAllocationListAutoDiff.ForEach(obj => { obj.Master.STATUS = "4"; });

							returnNewAllocationList.AddRange(returnNewAllocationListAutoDiff);

						}
					}
					#endregion

					#region 將產生調撥單呼叫調撥共用函數BulkInsertAllocation
					if (returnNewAllocationList.Any())
						sharedService.BulkInsertAllocation(returnNewAllocationList, returnStocks);

					#endregion
				}
				finally
				{
					if (isPass)
						stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
				}
			}
			#endregion

			#region 盤盈處理(產生調整單)
			// 盤盈的項目
			var surplusDatas = new List<InventoryDetailItemsByIsSecond>();
			if (f140101.CHECK_TOOL == "0")
				surplusDatas = selectedDatas.Where(x =>  Convert.ToInt32(x.STOCK_DIFF_QTY) > 0).ToList();
			else
				surplusDatas = selectedDatas.Where(x => (Convert.ToInt32(x.STOCK_DIFF_QTY) > 0 && Convert.ToInt32(x.DIFF_QTY) == 0)).ToList();

			if (surplusDatas.Any())
			{
				foreach(var item in surplusDatas)
				{
					var adjQty = Convert.ToInt32(item.SECOND_QTY == null ? item.FIRST_QTY : item.SECOND_QTY) - Convert.ToInt32(item.QTY) -  (string.IsNullOrWhiteSpace(item.UNMOVE_STOCK_QTY) ? 0 :  Convert.ToInt32(item.UNMOVE_STOCK_QTY));
					adjustStockDetailList.Add(new AdjustStockDetail
					{
						LocCode = item.LOC_CODE,
						ItemCode = item.ITEM_CODE,
						ValidDate = item.VALID_DATE,
						EnterDate = item.ENTER_DATE,
						MakeNo = item.MAKE_NO,
						PalletCtrlNo = item.PALLET_CTRL_NO,
						BoxCtrlNo = item.BOX_CTRL_NO,
						WarehouseId = item.WAREHOUSE_ID,
						Cause = "999",
						CasueMemo = "盤盈損調整",
						WORK_TYPE = "0",
						AdjQty = adjQty,
					});

					addF140107List.Add(new F140107
					{
						INVENTORY_NO = f140101.INVENTORY_NO,
						WAREHOUSE_ID = item.WAREHOUSE_ID,
						LOC_CODE = item.LOC_CODE,
						ITEM_CODE = item.ITEM_CODE,
						VALID_DATE = item.VALID_DATE,
						ENTER_DATE = item.ENTER_DATE,
						PROFIT_QTY = adjQty,
						LOSS_QTY = 0,
						FLUSHBACK = item.FLUSHBACK,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						BOX_CTRL_NO = item.BOX_CTRL_NO,
						PALLET_CTRL_NO = item.PALLET_CTRL_NO,
						MAKE_NO = item.MAKE_NO
					});

				}

				var adjustOrderParam = new AdjustOrderParam
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					AdjustType = AdjustType.InventoryStock,
					CheckSerialItem = false,
					SourceType = "18",
					SourceNo = f140101.INVENTORY_NO,
					WorkType = null,
					AdjustStockDetails = adjustStockDetailList
				};
				var adjResult = adjustService.CreateAdjustOrder(adjustOrderParam);

				if (!adjResult.IsSuccessed)
					return adjResult;

				f140107Repo.BulkInsert(addF140107List);


        // 回填成功產生的調整單號
        surplusDatas.ForEach(obj => { obj.PROC_WMS_NO = adjResult.No; });
			}

      #endregion

      #region 自動倉 調整F140104、是否下發盤點調整任務F060404 
      // 自動倉只要有盤差資料就得下發
      if (f140101.CHECK_TOOL != "0")
			{
				if (datas.Where(x => Convert.ToInt32(x.DIFF_QTY) != 0).Any())
					sharedService.CreateInventoryAdjustTask(dcCode, gupCode, custCode, inventoryNo);
			}
      #endregion

      #region 更新F140101.STATUS
      //如果前面的盤損沒有異動＆是人工盤點，就把狀態改為完成
      if (f140101.STATUS != "5" && f140101.CHECK_TOOL == "0")
      {
        f140101.STATUS = "5";
        f140101.POSTING_DATE = DateTime.Now;
        f140101Repo.Update(f140101);
      }
      else if (f140101.CHECK_TOOL != "0")
      {
        if (datas.Where(x => Convert.ToInt32(x.DIFF_QTY) != 0).Any())
        {
          // 更新F140101.STATUS = 3(已確認)
          f140101.STATUS = "3";
          f140101Repo.Update(f140101);
        }

        if (datas.All(x => Convert.ToInt32(x.DIFF_QTY) == 0))
        {
          // 盤點單狀態=5(結案)
          f140101.STATUS = "5";
          f140101.POSTING_DATE = DateTime.Now;
          f140101Repo.Update(f140101);
        }
      }

      #endregion

      #region 新增F140106
      //初盤，由F140104寫至F140106；複盤，由F140105寫至F140106
      var addF140106s = f140106Repo.InsertFromF140104AndF140105(data.ISSECOND == "1", dcCode, gupCode, custCode, inventoryNo, f140101.CHECK_TOOL, datas);
			if (addF140106s.Any())
				f140106Repo.BulkInsert(addF140106s);
			#endregion

			return result;
		}

		public ExecuteResult CaseClosedP140102(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			#region 變數資料
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			var f140105Repo = new F140105Repository(Schemas.CoreSchema);
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			#endregion

			// 盤點單資料
			var f140101 = f140101Repo.AsForUpdate().Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo);
			
			if (f140101.ISSECOND == "0")// 初盤
			{
				// 檢查是否此盤點單已盤點完成?  還沒盤點完成=>顯示訊息"此盤點單尚未盤點，不可以將盤點單結案"
				var f140104s = f140104Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo).ToList();
				if (f140104s.All(x => x.FIRST_QTY == null))
					return new ExecuteResult(false, "此盤點單尚未盤點，不可以將盤點單結案");

				// 檢查此盤點是否無盤差且無庫差?  有其中一項顯示訊息"此盤點單有盤差數或庫差數，不可以將盤點單結案"
				if (!f140104s.All(x => ((x.FIRST_QTY ?? 0) - x.DEVICE_STOCK_QTY) == 0 && (x.FIRST_QTY ?? 0 ) - (x.QTY + x.UNMOVE_STOCK_QTY) == 0))
					return new ExecuteResult(false, "此盤點單有盤差數或庫差數，不可以將盤點單結案");
			}
			else// 複盤
			{
				// 檢查是否此盤點單已盤點完成?  還沒盤點完成=>顯示訊息"此盤點單尚未盤點，不可以將盤點單結案"
				var f140105s = f140105Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo).ToList();
				if (f140105s.All(x => x.SECOND_QTY == null))
					return new ExecuteResult(false, "此盤點單尚未盤點，不可以將盤點單結案");

				// 檢查此盤點是否無盤差且無庫差?  有其中一項顯示訊息"此盤點單有盤差數或庫差數，不可以將盤點單結案"
				if (!f140105s.All(x => ((x.SECOND_QTY ?? 0) - x.DEVICE_STOCK_QTY) == 0 && (x.SECOND_QTY ?? 0) - (x.QTY + x.UNMOVE_STOCK_QTY) == 0))
					return new ExecuteResult(false, "此盤點單有盤差數或庫差數，不可以將盤點單結案");
			}

			// 盤點單狀態=5(結案)
			f140101.STATUS = "5";
			f140101.POSTING_DATE = DateTime.Now;
			f140101Repo.Update(f140101);
			return new ExecuteResult(true, "結案成功");
		}
	}
}
