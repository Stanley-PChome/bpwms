
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P14.Services;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P18.Services
{
	public partial class P180201Service
	{
		private WmsTransaction _wmsTransaction;
		public P180201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 庫存異動處理
		/// <summary>
		/// 取得庫存異動資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="begCrtDate"></param>
		/// <param name="endCrtDate"></param>
		/// <param name="srcType"></param>
		/// <param name="srcWmsNo"></param>
		/// <param name="procFlag"></param>
		/// <param name="allocationNo"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<StockAbnormalData> GetStockAbnormalData(string dcCode, string gupCode, string custCode,
			DateTime? begCrtDate, DateTime? endCrtDate, string srcType, string srcWmsNo, string procFlag, string allocationNo, string itemCode)
		{
			var repF191302 = new F191302Repository(Schemas.CoreSchema);
			return repF191302.GetStockAbnormalData(dcCode, gupCode, custCode,
				begCrtDate, endCrtDate, srcType, srcWmsNo, procFlag, allocationNo, itemCode);
		}

    /// <summary>
    /// 修改異常資料，產生調整單、調撥單
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ExecuteResult UpdateF191302(StockAbnormalData data)
    {
			var stockService = new StockService();
			bool isPass = false;
			var allotBatchNo = (data.PROC_FLAG == "1" ? "BT" : "BJ") + DateTime.Now.ToString("yyyyMMddHHmmss");
			try
			{
				var itemCodes = new List<ItemKey> { new ItemKey { DcCode = data.DC_CODE, GupCode = data.GUP_CODE, CustCode = data.CUST_CODE, ItemCode = data.ITEM_CODE } };
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫該筆異常商品，請稍後再試");

				var sharedService = new SharedService(_wmsTransaction);
				var f1980Repo = new F1980Repository(Schemas.CoreSchema);
				var f1913Repo = new F1913Repository(Schemas.CoreSchema);
				var repF191302 = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
				var repF151001 = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
				var returnStocks = new List<F1913>();
				var f191302 = repF191302.AsForUpdate().Find(o => o.ID == data.ID);

				if (f191302 == null)
					return new ExecuteResult(false, Properties.Resources.P080201Service_NotData);

				#region 當處理方式為1(找到商品)
				if (data.PROC_FLAG == "1")
				{
					var f151001 = repF151001.Find(o => o.DC_CODE == f191302.DC_CODE && o.GUP_CODE == f191302.GUP_CODE && o.CUST_CODE == f191302.CUST_CODE && o.ALLOCATION_NO == data.ALLOCATION_NO);
					var tmpSourceType = "";
					//來源單據，參考F000902
					switch (f191302.SRC_WMS_NO.Substring(0, 1))
					{
						case "P":
							tmpSourceType = "01";
							break;
						case "T":
							tmpSourceType = "17";
							break;
						default:
							tmpSourceType = "18";
							break;
					}


					var newAllocationParam = new NewAllocationItemParam
					{
						GupCode = f191302.GUP_CODE,
						CustCode = f191302.CUST_CODE,
						AllocationDate = DateTime.Now,
						SourceType = tmpSourceType,
						SourceNo = f191302.SRC_WMS_NO,
						IsExpendDate = true,
						SrcDcCode = f151001.TAR_DC_CODE,
						SrcWarehouseId = f191302.TAR_WAREHOUSE_ID,
						TarDcCode = f151001.SRC_DC_CODE,
						TarWarehouseId = f191302.SRC_WAREHOUSE_ID,
						AllocationType = AllocationType.Both,
						ReturnStocks = returnStocks,
						isIncludeResupply = true,
						SrcStockFilterDetails = new List<StockFilter>
					{
						new StockFilter
						{
							DataId = 0,
							ItemCode = f191302.ITEM_CODE,
							LocCode = f191302.TAR_LOC_CODE,
							Qty = data.QTY,
							ValidDates = new List<DateTime> { f191302.VALID_DATE },
							MakeNos = string.IsNullOrWhiteSpace(f191302.MAKE_NO) ? new List<string> { "0" } : new List<string> { f191302.MAKE_NO?.Trim() },
              SerialNos = string.IsNullOrWhiteSpace(f191302.SERIAL_NO) ? null : new List<string> { f191302.SERIAL_NO }
            }
          },
						SrcLocMapTarLocs = new List<SrcLocMapTarLoc>
					{
						new SrcLocMapTarLoc
						{
							DataId = 0,
							ItemCode = f191302.ITEM_CODE,
							SrcLocCode = f191302.TAR_LOC_CODE,
							TarWarehouseId = f191302.SRC_WAREHOUSE_ID,
							TarLocCode = f191302.SRC_LOC_CODE,
							ValidDate = f191302.VALID_DATE,
							MakeNo = f191302.MAKE_NO
						}
					}
					};

					var returnAllocationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam);

					if (!returnAllocationResult.Result.IsSuccessed)
						return new ExecuteResult(false, returnAllocationResult.Result.Message);

					// 該調撥單自動完成過帳。
					sharedService.BulkAllocationToAllDown(returnAllocationResult.AllocationList);
					sharedService.BulkAllocationToAllUp(returnAllocationResult.AllocationList, returnStocks);

					if (returnAllocationResult.AllocationList.Any())
						sharedService.BulkInsertAllocation(returnAllocationResult.AllocationList, returnStocks);

					f191302.PROC_WMS_NO = returnAllocationResult.AllocationList.First().Master.ALLOCATION_NO;
				}

				#endregion

				#region 當處理方式為2(確定貨損)
				if (data.PROC_FLAG == "2")
				{
					//var adjustType = data.SRC_TYPE == "1" ? "2" : "3";
					AdjustType adjustType;
					var causeMemo = string.Empty;
					var sourceType = string.Empty;
					switch (data.SRC_TYPE)
					{
						case "0":
							adjustType = AdjustType.LackStock;
							causeMemo = "揀缺庫存調整";
							sourceType = "01";
							break;
						case "1":
							adjustType = AdjustType.InventoryStock;
							causeMemo = "盤盈損調整";
							sourceType = "18";
							break;
						case "2":
							adjustType = AdjustType.AllocationLack;
							causeMemo = "調撥缺庫存調整";
							sourceType = "17";
							break;
						default:
							adjustType = AdjustType.LackStock;
							causeMemo = "揀缺庫存調整";
							sourceType = "01";
							break;
					}

					//要查詢的遺失倉類型
					if (f1913Repo.GetLocCodeStockQty(data.DC_CODE, data.GUP_CODE, data.CUST_CODE, data.ITEM_CODE, data.MAKE_NO, data.VALID_DATE, f191302.TAR_WAREHOUSE_ID, f191302.TAR_LOC_CODE) - data.QTY < 0)
						return new ExecuteResult(false, $"{f191302.TAR_WAREHOUSE_ID}{f1980Repo.GetWhName(f191302.DC_CODE, f191302.TAR_WAREHOUSE_ID)} 商品庫存不足，無法進行盤損");

					var adjustOrderService = new AdjustOrderService(_wmsTransaction);

					var adjustStockDetailList = new List<AdjustStockDetail>
					{
						new AdjustStockDetail
						{
							LocCode = f191302.TAR_LOC_CODE,
							ItemCode = f191302.ITEM_CODE,
							ValidDate = f191302.VALID_DATE,
							EnterDate = f191302.ENTER_DATE,
							MakeNo = f191302.MAKE_NO,
							PalletCtrlNo = f191302.PALLET_CTRL_NO,
							BoxCtrlNo = f191302.BOX_CTRL_NO,
							WarehouseId = f191302.TAR_WAREHOUSE_ID,
							Cause = "999",
							CasueMemo = causeMemo,
							WORK_TYPE = "1",
              AdjQty = data.QTY,
              SerialNoList = string.IsNullOrWhiteSpace(f191302.SERIAL_NO) ? null : new List<string> { f191302.SERIAL_NO }
            }
          };

					var adjustOrderParam = new AdjustOrderParam
					{
						DcCode = data.DC_CODE,
						GupCode = data.GUP_CODE,
						CustCode = data.CUST_CODE,
						AdjustType = adjustType,
						CheckSerialItem = false,
						WorkType = null,
						SourceType = sourceType,
						SourceNo = f191302.SRC_WMS_NO,
						AdjustStockDetails = adjustStockDetailList,

					};

					var result = adjustOrderService.CreateAdjustOrder(adjustOrderParam);
					if (result.IsSuccessed)
					{
						f191302.PROC_WMS_NO = result.No;
					}
					else
						return result;
					
				}
				#endregion

				#region 數量處理 (若處理數量小於異常數量時，因為只處理部分異常數量，本明細將拆成兩筆)
				if (data.QTY == f191302.QTY)
				{
					f191302.PROC_FLAG = data.PROC_FLAG;
					f191302.MEMO = string.IsNullOrWhiteSpace(data.MEMO) ? null : data.MEMO;
					repF191302.Update(f191302);
				}
				else if (data.QTY < f191302.QTY)
				{
					// 新增一筆資料，複製原本的資料，QTY=異常數量-處理數量、PROC_FLAG=0、MEMO=null
					repF191302.Add(new F191302
					{
						DC_CODE = f191302.DC_CODE,
						GUP_CODE = f191302.GUP_CODE,
						CUST_CODE = f191302.CUST_CODE,
						SRC_WMS_NO = f191302.SRC_WMS_NO,
						SRC_TYPE = f191302.SRC_TYPE,
						ALLOCATION_NO = f191302.ALLOCATION_NO,
						ALLOCATION_SEQ = f191302.ALLOCATION_SEQ,
						SRC_WAREHOUSE_ID = f191302.SRC_WAREHOUSE_ID,
						SRC_LOC_CODE = f191302.SRC_LOC_CODE,
						ITEM_CODE = f191302.ITEM_CODE,
						VALID_DATE = f191302.VALID_DATE,
						MAKE_NO = f191302.MAKE_NO,
						ENTER_DATE = f191302.ENTER_DATE,
						SERIAL_NO = f191302.SERIAL_NO,
						BOX_CTRL_NO = f191302.BOX_CTRL_NO,
						PALLET_CTRL_NO = f191302.PALLET_CTRL_NO,
						VNR_CODE = f191302.VNR_CODE,
						QTY = f191302.QTY - data.QTY,
						TAR_WAREHOUSE_ID = f191302.TAR_WAREHOUSE_ID,
						TAR_LOC_CODE = f191302.TAR_LOC_CODE,
						PROC_FLAG = "0",
					});

					// 更新F191302.QTY=處理數量、PROC_FLAG=處理方式、MEMO=備註
					f191302.QTY = data.QTY;
					f191302.PROC_FLAG = data.PROC_FLAG;
					f191302.MEMO = string.IsNullOrWhiteSpace(data.MEMO) ? null : data.MEMO;
					repF191302.Update(f191302);
				}
				#endregion
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}



			return new ExecuteResult(true);
    }

    /// <summary>
    /// 產生盤點單
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ExecuteResult CreateInventory(string dcCode, List<StockAbnormalData> datas)
		{
			var resulr = new ExecuteResult(true);
			var service = new P140101Service(_wmsTransaction);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema);
      var f1511Repo = new F1511Repository(Schemas.CoreSchema);
      var now = DateTime.Now;

			var sucessedInventoryNos = new List<string>();
			var errorMsgs = new List<string>();

			var f1912s = f1912Repo.GetDatasByLocCodesNoTracking(dcCode, datas.Select(x => x.SRC_LOC_CODE).Distinct().ToList()).ToList();

			var f1980s = f1980Repo.GetDatasByWarehouseId(dcCode, datas.Select(x => x.SRC_WAREHOUSE_ID).Distinct().ToList()).ToList();
      var data = (from A in datas
                  join B in f1980s
                  on new { WAREHOUSE_ID = A.SRC_WAREHOUSE_ID, A.DC_CODE } equals new { B.WAREHOUSE_ID, B.DC_CODE }
                  select new { StockAbnormalData = A, DeviceType = B.DEVICE_TYPE }).GroupBy(x => new { x.DeviceType, x.StockAbnormalData.DC_CODE, x.StockAbnormalData.GUP_CODE, x.StockAbnormalData.CUST_CODE }).Select(x => new
                  {
                    DeviceType = x.Key.DeviceType,
                    DcCode = x.Key.DC_CODE,
                    GupCode = x.Key.GUP_CODE,
                    CustCode = x.Key.CUST_CODE,
                    StockAbnormalDatas = (from C in x.Select(z => z.StockAbnormalData)
                                          join D in f1912s
                                          on C.SRC_LOC_CODE equals D.LOC_CODE
                                          select new
                                          {
                                            LocCode = C.SRC_LOC_CODE,
                                            ItemCode = C.ITEM_CODE,
                                            AreaCode = D.AREA_CODE,
                                            WarehouseId = C.SRC_WAREHOUSE_ID,
                                            WarehouseName = C.SRC_WAREHOUSE_NAME
                                          })
                                          .GroupBy(z => new { z.WarehouseId, z.WarehouseName })
                                          .Select(z => new
                                          {
                                            WarehouseId = z.Key.WarehouseId,
                                            WarehouseName = z.Key.WarehouseName,
                                            LocCode = z.Select(y => y.LocCode).Distinct().ToList(),
                                            AreaCode = z.Select(y => y.AreaCode).Distinct().ToList(),
                                            ItemCodes = z.Select(y => y.ItemCode).Distinct().ToList(),
                                            LocAndItemCode = z.Select(y => new { y.LocCode, y.ItemCode }).ToList()
                                          })
                                          .ToList()
                  }).ToList();


      data.ForEach(obj =>
			{
				var f140101 = new F140101
				{
					DC_CODE = obj.DcCode,
					GUP_CODE = obj.GupCode,
					CUST_CODE = obj.CustCode,
					CHECK_TOOL = obj.DeviceType,
					INVENTORY_DATE = now,
					INVENTORY_TYPE = "0",
					SHOW_CNT = "1"
				};

				obj.StockAbnormalDatas.ForEach(objSub =>
				{
          var inventoryWareHouseList = objSub.AreaCode.Select(x => new InventoryWareHouse { AREA_CODE = x, WAREHOUSE_ID = objSub.WarehouseId, WAREHOUSE_NAME = objSub.WarehouseName }).ToList();
					var inventoryItemList = objSub.ItemCodes.Select(x => new InventoryItem { ITEM_CODE = x }).ToList();

          var f1913ExList = f1913Repo.GetDatasByInventoryWareHouseList(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE,
            inventoryWareHouseList, inventoryItemList.Select(o => o.ITEM_CODE).ToList(), f140101.INVENTORY_TYPE, f140101.INVENTORY_DATE).ToList();

          var stockNotEnough = f1913ExList
            .GroupBy(g => g.ITEM_CODE)
            .Where(x => x.Sum(a => a.QTY) == 0 && x.Sum(a => a.UNMOVE_STOCK_QTY) == 0);

          if (stockNotEnough.Any())
          {
            //確認前端丟入的儲位＆品號有沒有在庫存不足的清單中
            var reqStockNotEnugh = objSub.LocAndItemCode.Where(a => stockNotEnough.Any(b => a.ItemCode == b.Key && b.Any(c => c.LOC_CODE == a.LocCode))).Distinct();
            errorMsgs.AddRange(
              reqStockNotEnugh
              .Select(x => $"異常儲位：{x.LocCode}  品號：{x.ItemCode}  {Properties.Resources.P140101Service_NoInventoryItem}")
              );
            return; //這邊是迴圈的Continue，非離開程式
          }

          var currRes = service.InsertP140101(f140101, inventoryWareHouseList, inventoryItemList);

          if (currRes.IsSuccessed)
            sucessedInventoryNos.AddRange(currRes.Message.Split(',').ToList());
          else
          {
            errorMsgs.AddRange(objSub.LocAndItemCode.GroupBy(x => new { x.LocCode, x.ItemCode }).Select(x => $"異常儲位：{x.Key.LocCode}  品號：{x.Key.ItemCode}  {currRes.Message}"));
          }
        });
			});


      if (errorMsgs.Any())
        return new ExecuteResult(false, string.Join("\r\n", errorMsgs));
      else
      {
        _wmsTransaction.Complete();
        return new ExecuteResult(true, string.Join(",", sucessedInventoryNos));
      }
				
		}
		#endregion
	}
}

