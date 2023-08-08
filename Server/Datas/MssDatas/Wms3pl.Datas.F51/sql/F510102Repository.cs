using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510102Repository : RepositoryBase<F510102, Wms3plDbContext, F510102Repository>
    {
		/// <summary>
		/// 備份每日庫存數
		/// </summary>
		/// <param name="settleDate"></param>
		public void InsertStockByDate(DateTime settleDate)
		{
			var sql = @"INSERT INTO F510102 
								  SELECT @p0 CAL_DATE, A.* FROM F1913 A";
			var param = new SqlParameter[]
			{
				new SqlParameter("@p0", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}
		public IQueryable<StockSettleData> GetLocSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                    SELECT A.DC_CODE,
													 A.GUP_CODE,
													 A.CUST_CODE,
													A.ITEM_CODE,
										       SUM(ISNULL (A.END_QTY,0) + ISNULL(C.A_PICK_QTY, 0)) END_QTY
										FROM (  SELECT @p0 DC_CODE,
										               A.GUP_CODE,
										               A.CUST_CODE,
										               A.ITEM_CODE,
										               SUM (ISNULL (B.QTY, 0)) END_QTY
										          FROM F1903 A
										               LEFT JOIN F510102 B
										                  ON     A.GUP_CODE = B.GUP_CODE
										                     AND A.CUST_CODE = B.CUST_CODE
										                     AND A.ITEM_CODE = B.ITEM_CODE
										                     AND B.DC_CODE = @p0
										                     AND B.CAL_DATE = @p3
										         WHERE B.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
										      GROUP BY B.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE) A
										     LEFT JOIN F510104 C
										        ON     A.GUP_CODE = C.GUP_CODE
										           AND A.CUST_CODE = C.CUST_CODE
										           AND A.ITEM_CODE = C.ITEM_CODE
										           AND C.DC_CODE = @p0
										           AND C.CAL_DATE = @p3
											 GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE";

			var param = new SqlParameter[]
			{
				new SqlParameter("@p0",dcCode),
				new SqlParameter("@p1",gupCode),
				new SqlParameter("@p2",custCode),
				new SqlParameter("@p3",calDate)
			};
			return SqlQuery<StockSettleData>(sql, param);
		}
	}

}
