using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P081301Service
	{
		private WmsTransaction _wmsTransaction;
		public P081301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P081301StockSumQty> GetP081301StockSumQties(string dcCode,string gupCode,string  custCode,string scanItemOrLocCode)
		{
			var repo = new F1913Repository(Schemas.CoreSchema);
			return repo.GetP081301StockSumQties(dcCode, gupCode, custCode, scanItemOrLocCode);
		}

		public IQueryable<P08130101MoveLoc> GetP08130101MoveLocs(string dcCode,string locCode)
		{
			var repo = new F1912Repository(Schemas.CoreSchema);
			return repo.GetP08130101MoveLocs(dcCode, locCode);
		}
		public IQueryable<P08130101Stock> GetP08130101Stocks(string dcCode, string gupCode, string custCode, string locCode,string itemCode)
		{
			var repo = new F1913Repository(Schemas.CoreSchema);
			return repo.GetP08130101Stocks(dcCode, gupCode,custCode,locCode,itemCode);
		}
		public ExecuteResult CheckTarLocCode(string dcCode, string locCode)
		{
			var sharedService = new SharedService();
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var result = sharedService.CheckLocCode(dcCode,locCode,Current.Staff);
			if (!result.IsSuccessed)
				return result;
			//上架儲位的倉別不可為自動倉
			if (f1980Repo.GetF1980ByLocCode(dcCode, locCode)?.DEVICE_TYPE != "0")
			{
				return new ExecuteResult(false, string.Format(Properties.Resources.P2001010000_IsAutoWarehourseIsNotAvailable, locCode));
			}
			result = sharedService.CheckLocFreeze(dcCode, locCode, "2");
			if (!result.IsSuccessed)
				return result;
			var f1912 = sharedService.GetF1912(dcCode, locCode);
			var f1980 = sharedService.GetF1980(f1912.DC_CODE, f1912.WAREHOUSE_ID);
			// 檢查是否有設定儲區
			if (f1912.AREA_CODE.Trim() == "-1")
			{
				return new ExecuteResult(false, string.Format("儲位{0}未設定儲區，不可上架", locCode));
			}
			return new ExecuteResult(true, f1980.WAREHOUSE_NAME);
		}

		public ExecuteResult CreateAllocation(string dcCode,string gupCode,string custCode,string tarLocCode,List<P08130101Stock> moveStocks)
		{
            // 要進行配庫鎖定，再呼叫建立調撥單，建立完成調撥單後再將配庫鎖定解鎖
            var isPass = false;
            var stockService = new StockService();
            var returnNewAllocationResult = new ReturnNewAllocationResult();
			      string allotBatchNo=string.Empty;
            try
            {
				        var itemCodes = moveStocks.Select(x => new ItemKey { DcCode = dcCode, GupCode = gupCode, CustCode = custCode, ItemCode = x.ITEM_CODE }).Distinct().ToList();
								allotBatchNo = "BM" + DateTime.Now.ToString("yyyyMMddHHmmss");
                isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);

                if (!isPass)
                    return new ExecuteResult { IsSuccessed = false, Message = "仍有程序正在配庫搬移單所配庫商品，請稍待再配庫" };
                else
                {
                    var sharedService = new SharedService(_wmsTransaction);
                    var srcLoc = sharedService.GetF1912(dcCode, moveStocks.First().LOC_CODE);
                    var tarLoc = sharedService.GetF1912(dcCode, tarLocCode);
                    var stockList = new List<F1913>();
                    var newAllocationParam = new NewAllocationItemParam
                    {
                        GupCode = gupCode,
                        CustCode = custCode,
                        AllocationDate = DateTime.Today,
                        SourceType = "17",
                        IsExpendDate = true,
                        SrcDcCode = dcCode,
                        SrcWarehouseId = srcLoc.WAREHOUSE_ID,
                        TarDcCode = dcCode,
                        AllocationType = AllocationType.Both,
                        ReturnStocks = stockList,
                        IsMoveOrder = true,
                        SrcStockFilterDetails = moveStocks.Select((x, rowIndex) => new StockFilter
                        {
                            DataId = rowIndex,
                            ItemCode = x.ITEM_CODE,
                            LocCode = x.LOC_CODE,
                            Qty = x.MOVE_QTY,
                            ValidDates = new List<DateTime> { x.VALID_DATE },
                            EnterDates = new List<DateTime> { x.ENTER_DATE },
                            BoxCtrlNos = new List<string> { string.IsNullOrEmpty(x.BOX_CTRL_NO) ? "0" : x.BOX_CTRL_NO },
                            PalletCtrlNos = new List<string> { string.IsNullOrEmpty(x.PALLET_CTRL_NO) ? "0" : x.PALLET_CTRL_NO },
                            SerialNos = new List<string> { string.IsNullOrEmpty(x.SERIAL_NO) ? "0" : x.SERIAL_NO },
                            MakeNos = new List<string> { string.IsNullOrEmpty(x.MAKE_NO) ? "0" : x.MAKE_NO },
                            isAllowExpiredItem = true,
                        }).ToList(),
                        SrcLocMapTarLocs = moveStocks.Select((x, rowIndex) => new SrcLocMapTarLoc
                        {
                            DataId = rowIndex,
                            ItemCode = x.ITEM_CODE,
                            SrcLocCode = x.LOC_CODE,
                            TarWarehouseId = tarLoc.WAREHOUSE_ID,
                            TarLocCode = tarLoc.LOC_CODE,
                        }).ToList(),
                        IsCheckExecStatus = !isPass
                    };
                    var allloationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam);
                    if (!allloationResult.Result.IsSuccessed)
                        return allloationResult.Result;
                    stockList = allloationResult.StockList;
                    var result = sharedService.BulkAllocationToAllUp(allloationResult.AllocationList, stockList);
                    if (!result.IsSuccessed)
                        return result;
                    var bulkInsertResult = sharedService.BulkInsertAllocation(allloationResult.AllocationList, stockList, true);
                    return bulkInsertResult;
                }
            }
            finally
            {
                if (isPass)
                    stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
            }
		}
	}
}
