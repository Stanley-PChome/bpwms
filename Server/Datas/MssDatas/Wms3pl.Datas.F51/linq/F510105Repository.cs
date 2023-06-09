using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510105Repository : RepositoryBase<F510105, Wms3plDbContext, F510105Repository>
	{
		public F510105Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F510105> GetDatasByCalDate(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			return _db.F510105s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			Convert.ToDateTime(x.CAL_DATE) < calDate);
		}

		public IQueryable<F510105> GetDatasEqualCalDate(string dcCode, string gupCode, string custCode, string calDate)
		{
			return _db.F510105s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.CAL_DATE == calDate);
		}

		public void BulkDelete(List<F510105> data)
		{
			SqlBulkDeleteForAnyCondition(data, "F510105", new List<string> { "ID" });
		}

		public List<F510105> GetAddDatasByStockSnapshot(string dcCode, string gupCode, string custCode, DateTime now)
		{
			//(1) AA = 撈loc_code from F1912 by dc_code+warehouse_id
			//(2) 撈 F1913.*, sum(QTY) wms庫存
			//	條件: loc_code in (AA) + dc_code
			//	條件: group by GUP_CODE、CUST_CODE、ITEM_CODE、VALID_DATE、MAKE_NO
			//(3) 另外撈 sum(F1511.B_PICK_QTY) 虛擬帳庫存
			//	條件: status = 0 + loc_code in (AA) + dc_code
			//	條件: group by GUP_CODE、CUST_CODE、ITEM_CODE、VALID_DATE、MAKE_NO
			//	欄位請參考5.7.4
			//(4) 當(3)與(2)以下條件相同時，表示寫入同一筆資料
			//	DC_CODE、GUP_CODE、CUST_CODE、WAREHOUSE_ID、LOC_CODE、ITEM_CODE、VALID_DATE、MAKE_NO

			var f1980s = _db.F1980s.AsNoTracking().Where(o => o.DC_CODE == dcCode && o.DEVICE_TYPE != "0");

			var warehouseIds = f1980s.Select(x => x.WAREHOUSE_ID);

			var calDate = now.ToString("yyyy/MM/dd");

			var f1912s = _db.F1912s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			warehouseIds.Contains(x.WAREHOUSE_ID));

			var locCodes = f1912s.Select(x => x.LOC_CODE);

			#region 實際庫存數
			var f1913s = _db.F1913s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			locCodes.Contains(x.LOC_CODE));

			var stockDatas = (from A in f1913s
												join B in f1912s
												on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
												select new StockSnapshotData
												{
													WAREHOUSE_ID = B.WAREHOUSE_ID,
													LOC_CODE = A.LOC_CODE,
													ITEM_CODE = A.ITEM_CODE,
													VALID_DATE = A.VALID_DATE,
													MAKE_NO = A.MAKE_NO,
													QTY = Convert.ToInt32(A.QTY)
												}).ToList();
			#endregion

			#region 虛擬未搬動庫存數
			var f1511s = _db.F1511s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			locCodes.Contains(x.LOC_CODE) &&
			x.STATUS == "0");

			var virtualDatas = (from A in f1511s
													join B in f1912s
													on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
													select new StockSnapshotData
													{
														WAREHOUSE_ID = B.WAREHOUSE_ID,
														LOC_CODE = A.LOC_CODE,
														ITEM_CODE = A.ITEM_CODE,
														VALID_DATE = Convert.ToDateTime(A.VALID_DATE),
														MAKE_NO = A.MAKE_NO,
														B_PICK_QTY = A.B_PICK_QTY
													}).ToList();
			#endregion

			stockDatas = stockDatas.Union(virtualDatas).ToList();

			var datas = stockDatas.GroupBy(x => new { x.WAREHOUSE_ID, x.LOC_CODE, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO })
				.Select(x => new F510105
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
					CAL_DATE = calDate,
					LOC_CODE = x.Key.LOC_CODE,
					ITEM_CODE = x.Key.ITEM_CODE,
					VALID_DATE = x.Key.VALID_DATE,
					MAKE_NO = x.Key.MAKE_NO,
					WMS_QTY = x.Sum(z => z.QTY),
					WCS_QTY = 0,
					BOOKING_QTY = x.Sum(z => z.B_PICK_QTY),
					DIFF_QTY = 0,
					PROC_FLAG = "0"
				}).ToList();

			return datas;
		}

		public IQueryable<F510105> GetDataByPending(string dcCode, string calDate)
		{
			return _db.F510105s.Where(x => 
			x.DC_CODE == dcCode &&
			x.CAL_DATE == calDate &&
			x.PROC_FLAG == "0");
		}
	}
}
