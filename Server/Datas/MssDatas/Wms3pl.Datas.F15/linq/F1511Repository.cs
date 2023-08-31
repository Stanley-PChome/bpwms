
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
	public partial class F1511Repository : RepositoryBase<F1511, Wms3plDbContext, F1511Repository>
	{
		public F1511Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F1511> GetDatasByF051202(string dcCode, string gupCode, string custCode, IQueryable<F051202> f051202s)
		{
			return _db.F1511s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			f051202s.Any(z =>
			z.PICK_ORD_NO == x.ORDER_NO &&
			z.PICK_ORD_SEQ == x.ORDER_SEQ));
		}

		public IQueryable<F1511WithF051202> GetF1511sByWmsOrdNo(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{

			var f050801s = _db.F050801s.AsNoTracking().Where(x => x.CUST_CODE == custCode
																										&& x.DC_CODE == dcCode
																										&& wmsOrdNos.Contains(x.WMS_ORD_NO));
			var f051202s = _db.F051202s.AsNoTracking();
			var f1511s = _db.F1511s.AsNoTracking();

			var result = from A in f050801s
									 join B in f051202s on new { A.WMS_ORD_NO, A.GUP_CODE, A.CUST_CODE, A.DC_CODE }
									 equals new { B.WMS_ORD_NO, B.GUP_CODE, B.CUST_CODE, B.DC_CODE }
									 join C in f1511s on new { ORDER_NO = B.PICK_ORD_NO, ORDER_SEQ = B.PICK_ORD_SEQ, B.GUP_CODE, B.CUST_CODE, B.DC_CODE }
									 equals new { C.ORDER_NO, C.ORDER_SEQ, C.GUP_CODE, C.CUST_CODE, C.DC_CODE }
									 group new { A, B, C } by new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO, B.ITEM_CODE, B.SERIAL_NO } into g
									 select new F1511WithF051202
									 {
										 DC_CODE = g.Key.DC_CODE,
										 GUP_CODE = g.Key.GUP_CODE,
										 CUST_CODE = g.Key.CUST_CODE,
										 WMS_ORD_NO = g.Key.WMS_ORD_NO,
										 ITEM_CODE = g.Key.ITEM_CODE,
										 SERIAL_NO = g.Key.SERIAL_NO,
										 A_PICK_QTY_SUM = g.Sum(x => x.C.A_PICK_QTY)
									 };
			return result;
		}

		public IQueryable<F1511> GetDatasByF051206LackListAllot(List<F051206LackList_Allot> data)
		{
			var result = _db.F1511s.Where(x => data.Any(z => z.ALLOCATION_NO == x.ORDER_NO &&
			z.ALLOCATION_SEQ.ToString() == x.ORDER_SEQ));
			return result;
		}


		public void GetUnmoveStockQtyByInventoryF1913ExList(string dcCode, string gupCode, string custCode, ref List<F1913Ex> f1913ExList)
		{
			var f1913Exs = f1913ExList;

			var f1511Exs = _db.F1511s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS == "0" &&
			f1913Exs.Any(z => z.LOC_CODE == x.LOC_CODE &&
			z.ITEM_CODE == x.ITEM_CODE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.ENTER_DATE == x.ENTER_DATE &&
			z.MAKE_NO == x.MAKE_NO &&
			z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
			z.PALLET_CTRL_NO == x.PALLET_CTRL_NO &&
			(z.SERIAL_NO == x.SERIAL_NO || string.IsNullOrWhiteSpace(x.SERIAL_NO)))).ToList();

			f1511Exs.ForEach(f1511 => 
			{
				if(string.IsNullOrWhiteSpace(f1511.SERIAL_NO))
					f1511.SERIAL_NO = "0";
			});

			var group = (from o in f1511Exs
									group o by new {
										o.DC_CODE,
										o.GUP_CODE,
										o.CUST_CODE,
										o.LOC_CODE,
										o.ITEM_CODE,
										o.VALID_DATE,
										o.ENTER_DATE,
										o.MAKE_NO,
										o.BOX_CTRL_NO,
										o.PALLET_CTRL_NO,
										o.SERIAL_NO
									} into g
									select g).ToList();

			f1913ExList.ForEach(f1913Ex =>
			{
				var f1511Ex = group.Where(x =>
				f1913Ex.LOC_CODE == x.Key.LOC_CODE &&
				f1913Ex.ITEM_CODE == x.Key.ITEM_CODE &&
				f1913Ex.VALID_DATE == x.Key.VALID_DATE &&
				f1913Ex.ENTER_DATE == x.Key.ENTER_DATE &&
				f1913Ex.MAKE_NO == x.Key.MAKE_NO &&
				f1913Ex.BOX_CTRL_NO == x.Key.BOX_CTRL_NO &&
				f1913Ex.PALLET_CTRL_NO == x.Key.PALLET_CTRL_NO &&
				f1913Ex.SERIAL_NO == x.Key.SERIAL_NO).FirstOrDefault();

				if (f1511Ex != null)
					f1913Ex.UNMOVE_STOCK_QTY = f1511Ex.Sum(x => x.B_PICK_QTY);
			});
		}

    public void UpdateVirtualMoved(string dcCode, string gupCode, string custCode, string ordNo, string ordSeq, int pickQty, DateTime? updDate, string updStaff, string updName)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", ordNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", ordSeq) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5", pickQty) { SqlDbType = SqlDbType.Int },
        new SqlParameter("@p6", updDate) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p7", updStaff) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p8", updName) { SqlDbType = SqlDbType.NVarChar }
      };

      var sql = @"
                UPDATE 
                  F1511
                SET 
                  A_PICK_QTY = @p5, 
                  STATUS = '1',
                  UPD_DATE = @p6, 
                  UPD_STAFF = @p7, 
                  UPD_NAME = @p8
                WHERE
                  DC_CODE = @p0
                  AND GUP_CODE = @p1
                  AND CUST_CODE = @p2
                  AND ORDER_NO = @p3
                  AND ORDER_SEQ = @p4
                ";

      ExecuteSqlCommand(sql, param.ToArray());
    }
  }
}
