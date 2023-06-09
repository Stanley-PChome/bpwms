using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Process.P15.Services
{
	public partial class P150201Service
	{

		#region Import 資料檢查及新增
		public ExecuteResult ImportF150201Data(string gupCode, string custCode
																		, string fileName, List<F150201ImportData> importData)
		{
			if (importData.Any(x => x.GUP_CODE != gupCode || x.CUST_CODE != custCode))
			{
				return new ExecuteResult(false, Properties.Resources.P150201Servicepart1_ImportFail);
			}
			var sharedService = new SharedService(_wmsTransaction);
			var totalCount = importData.Count;
			var failureCount = 0;
			var errMessageList = new List<string>();
			var returnF1913List = new List<F1913>();
			var allocationList = new List<KeyValuePair<int, List<F150201ImportData>>>();
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);

			//檢查儲位
			var checkLocs = importData.Select(x => new { DcCode = x.SRC_DC_CODE, LocCode = x.SRC_LOC_CODE, LocType = "1", WarehouseId = x.SRC_WAREHOUSE_ID })
											.Union(importData.Select(x => new { DcCode = x.TAR_DC_CODE, LocCode = x.TAR_LOC_CODE, LocType = "2", WarehouseId = x.TAR_WAREHOUSE_ID }))
											.Distinct().Select(x => new CheckLoc { DcCode = x.DcCode, LocCode = x.LocCode, LocType = x.LocType, WarehouseId = x.WarehouseId }).ToList();
			var resultCheckLocs = sharedService.CheckMultiLocCode(checkLocs);
			if (resultCheckLocs.Any())
			{
				foreach (var checkLoc in resultCheckLocs)
				{
					var exceptData = importData.Where(x => (checkLoc.LocType == "1" && x.SRC_DC_CODE == checkLoc.DcCode && x.SRC_LOC_CODE == checkLoc.LocCode && x.SRC_WAREHOUSE_ID == checkLoc.WarehouseId) || (checkLoc.LocType == "2" && x.TAR_DC_CODE == checkLoc.DcCode && x.TAR_LOC_CODE == checkLoc.LocCode && x.TAR_WAREHOUSE_ID == checkLoc.WarehouseId));
					failureCount += exceptData.Count();
					errMessageList.Add(checkLoc.Message);
					importData = importData.Except(exceptData).ToList();
				}
			}

			//檢查上架儲位商品
			var checkLocItems = importData.Select(x => new { DcCode = x.TAR_DC_CODE, LocCode = x.TAR_LOC_CODE, LocType = "2", WarehouseId = x.TAR_WAREHOUSE_ID, ItemCode = x.ITEM_CODE })
										.Distinct().Select(x => new CheckLocItem { DcCode = x.DcCode, LocCode = x.LocCode, LocType = x.LocType, WarehouseId = x.WarehouseId, ItemCode = x.ItemCode }).ToList();
			var resultCheckLocItems = sharedService.CheckMultiLocItem(gupCode, custCode, checkLocItems);
			if (resultCheckLocItems.Any())
			{
				foreach (var checkLocItem in resultCheckLocItems)
				{
					var exceptData = importData.Where(x => x.TAR_DC_CODE == checkLocItem.DcCode && x.TAR_LOC_CODE == checkLocItem.LocCode && x.ITEM_CODE == checkLocItem.ItemCode && x.TAR_WAREHOUSE_ID == checkLocItem.WarehouseId);
					failureCount += exceptData.Count();
					errMessageList.Add(checkLocItem.Message);
					importData = importData.Except(exceptData).ToList();
				}
			}


			//取得來源商品總庫存
			var stocks = f1913Repo.GetStockQuerys(gupCode, custCode, importData.Select(x => x.SRC_DC_CODE).Distinct().ToList(), importData.Select(x => x.SRC_LOC_CODE).Distinct().ToList(), importData.Select(x => x.ITEM_CODE).Distinct().ToList());

			//檢查來源儲位商品庫存
			var groupData3 = importData.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE, x.SRC_DC_CODE, x.SRC_WAREHOUSE_ID, x.SRC_LOC_CODE, x.ITEM_CODE });
			foreach (var groupItem in groupData3)
			{
				var qty = groupItem.Sum(x => x.QTY);
				var stockQty = stocks.Where(x => x.DC_CODE == groupItem.Key.SRC_DC_CODE && x.WAREHOUSE_ID == groupItem.Key.SRC_WAREHOUSE_ID && x.LOC_CODE == groupItem.Key.SRC_LOC_CODE && x.ITEM_CODE == groupItem.Key.ITEM_CODE).Sum(x => x.QTY);
				if (stockQty < qty)
				{
					failureCount += groupItem.Count();
					errMessageList.Add(string.Format(Properties.Resources.P150201Servicepart1_SRCDC_Code, groupItem.Key.SRC_DC_CODE, groupItem.Key.SRC_WAREHOUSE_ID, groupItem.Key.SRC_LOC_CODE, groupItem.Key.ITEM_CODE));
					importData = importData.Except(groupItem.ToList()).ToList();
				}
			}

			if (errMessageList.Any())
			{
				return new ExecuteResult(true, string.Format(Properties.Resources.P150201Servicepart1_ImportFailCount, totalCount, failureCount, Environment.NewLine, string.Format(Properties.Resources.P150201Servicepart1_FailMessage, Environment.NewLine, string.Join(Environment.NewLine, errMessageList))));
			}
			var returnAllocationList = new List<ReturnNewAllocation>();
			var isPass = false;
			var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
			var stockService = new StockService();
			try
			{
				var itemCodes = importData.Select(x => new ItemKey { DcCode = x.SRC_DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫調撥單所配庫商品，請稍待再試");

				//依相同來源物流中心、來源倉、目的物流中心、目的倉拆單
				var groupWarehouse = importData.GroupBy(x => new { x.SRC_DC_CODE, x.SRC_WAREHOUSE_ID, x.TAR_DC_CODE, x.TAR_WAREHOUSE_ID, x.CUST_CODE, x.GUP_CODE });
				//開始建立調撥單
				foreach (var allocation in groupWarehouse)
				{
					var newAllocationParam = new NewAllocationItemParam
					{
						GupCode = allocation.Key.GUP_CODE,
						CustCode = allocation.Key.CUST_CODE,
						SrcDcCode = allocation.Key.SRC_DC_CODE,
						SrcWarehouseId = allocation.Key.SRC_WAREHOUSE_ID,
						TarDcCode = allocation.Key.TAR_DC_CODE,
						TarWarehouseId = allocation.Key.TAR_WAREHOUSE_ID,
						AllocationDate = DateTime.Today,
						AllocationType = AllocationType.Both,
						IsExpendDate = true,
						SendCar = false,
						SrcStockFilterDetails = allocation.GroupBy(x => new { x.ITEM_CODE, x.SRC_LOC_CODE, x.TAR_LOC_CODE, x.MAKE_NO, x.VALID_DATE }).Select((x, rowIndex) => new StockFilter
						{
							DataId = rowIndex,
							ItemCode = x.Key.ITEM_CODE,
							LocCode = x.Key.SRC_LOC_CODE,
							Qty = x.Sum(y => y.QTY),
							isAllowExpiredItem = true,
							ValidDates = x.Key.VALID_DATE == null ? new List<DateTime>() : new List<DateTime>() { Convert.ToDateTime(x.Key.VALID_DATE) },
							MakeNos = string.IsNullOrWhiteSpace(x.Key.MAKE_NO) ? new List<string>() : new List<string>() { x.Key.MAKE_NO }
						}).ToList(),
						ReturnStocks = returnF1913List,
						SrcLocMapTarLocs = allocation.GroupBy(x => new { x.ITEM_CODE, x.SRC_LOC_CODE, x.TAR_LOC_CODE, x.MAKE_NO, x.VALID_DATE }).Select((x, rowIndex) => new SrcLocMapTarLoc
						{
							DataId = rowIndex,
							ItemCode = x.Key.ITEM_CODE,
							SrcLocCode = x.Key.SRC_LOC_CODE,
							TarLocCode = x.Key.TAR_LOC_CODE
						}).ToList()
					};
					var returnAllocationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam);
					if (returnAllocationResult.Result.IsSuccessed)
					{
						returnF1913List = returnAllocationResult.StockList;
						returnAllocationList.AddRange(returnAllocationResult.AllocationList);
					}
					else
					{
						failureCount += allocation.Count();
						errMessageList.Add(returnAllocationResult.Result.Message);
					}
				}
				if (returnAllocationList.Any())
				{
					sharedService.BulkInsertAllocation(returnAllocationList, returnF1913List);
				}
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}
			
			return new ExecuteResult(true, string.Format(Properties.Resources.P150201Servicepart1_ImportResult, totalCount, totalCount - failureCount, failureCount, returnAllocationList.Count, errMessageList.Any() ? Environment.NewLine + Environment.NewLine + string.Format(Properties.Resources.P150201Servicepart1_FailMessage, Environment.NewLine, string.Join(Environment.NewLine, errMessageList)) : ""));

		}

		#endregion


		//  P1502010000  調撥單匯出
		public IQueryable<GetF150201CSV> GetF150201CSV(string gupCode, string custCode, string SourceDcCode, string TargetDcCode, DateTime CRTDateS, DateTime CRTDateE, string TxtSearchAllocationNo, DateTime? PostingDateS, DateTime? PostingDateE, string SourceWarehouseList, string TargetWarehouseList, string StatusList, string TxtSearchSourceNo)
		{

			var repo = new F151001Repository(Schemas.CoreSchema);
			return repo.GetF150201CSV(gupCode, custCode, SourceDcCode, TargetDcCode, CRTDateS, CRTDateE, TxtSearchAllocationNo, PostingDateS, PostingDateE, SourceWarehouseList, TargetWarehouseList, StatusList, TxtSearchSourceNo);
		}
	}
}