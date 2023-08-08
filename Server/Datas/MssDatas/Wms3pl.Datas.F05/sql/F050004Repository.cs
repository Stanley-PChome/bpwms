using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050004Repository : RepositoryBase<F050004, Wms3plDbContext, F050004Repository>
	{
		/// <summary>
		/// 取得出貨單批次參數維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public IQueryable<F050004WithF190001> GetF050004WithF190001s(string dcCode, string gupCode, string custCode)
		{
			var sqlParams = new SqlParameter[]
			{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", string.IsNullOrWhiteSpace(gupCode)?(object)DBNull.Value:gupCode),
								new SqlParameter("@p2", string.IsNullOrWhiteSpace(custCode)?(object)DBNull.Value:custCode),
								new SqlParameter("@p3", Current.Staff)
			};

			var sql = @"SELECT A.*, B.TICKET_NAME, C.DC_NAME, D.GUP_NAME, E.CUST_NAME, F.ORD_NAME, G.TICKET_CLASS_NAME,                        
										H.NAME AS SPLIT_FLOOR_NAME,
										(CASE A.MERGE_ORDER WHEN '1' THEN N'是' ELSE N'否' END ) AS MERGE_ORDER_NAME
                        FROM F050004 A
                        JOIN F190001 B
				        ON A.TICKET_ID = B.TICKET_ID
                        AND A.DC_CODE = B.DC_CODE 
                        AND A.GUP_CODE = B.GUP_CODE 
                         AND A.CUST_CODE = B.CUST_CODE 
				        LEFT JOIN F1901 C
				        ON A.DC_CODE = C.DC_CODE
				        LEFT JOIN F1929 D
				        ON A.GUP_CODE = D.GUP_CODE
				        LEFT JOIN F1909 E
                        ON A.GUP_CODE = E.GUP_CODE
                        AND A.CUST_CODE = E.CUST_CODE
				        LEFT JOIN F000901 F
				        ON B.TICKET_TYPE = F.ORD_TYPE
				        LEFT JOIN F000906 G
                        ON B.TICKET_CLASS = G.TICKET_CLASS
				        LEFT JOIN (Select H.VALUE,H.NAME From F000904 H WHERE H.TOPIC = 'F050004' AND H.SUBTOPIC = 'SPILT_PICK_TYPE') H
                        ON A.SPLIT_PICK_TYPE = H.VALUE
                        WHERE A.DC_CODE = @p0
                        AND A.GUP_CODE = CASE WHEN @p1 = '' OR @p1 IS NULL THEN A.GUP_CODE ELSE @p1 END
                        AND A.CUST_CODE = CASE WHEN @p2 = '' OR @p2 IS NULL THEN A.CUST_CODE ELSE @p2 END
									AND EXISTS (SELECT 1 FROM F190101 cc 
												INNER JOIN (SELECT * FROM F192402 
												WHERE EMP_ID = @p3) dd 
												ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
												WHERE cc.DC_CODE = A.DC_CODE AND cc.GUP_CODE = A.GUP_CODE AND cc.CUST_CODE = A.CUST_CODE) ";

			var result = SqlQuery<F050004WithF190001>(sql, sqlParams.ToArray());
			return result;
		}
		public IQueryable<F050004Ex> GetF050004Exs(List<string> ticketClass)
		{
			var parms = new List<SqlParameter>();
			
			var sql = @" SELECT A.TICKET_ID,B.TICKET_CLASS,A.ORDER_LIMIT,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DAY
										FROM F050004 A
										JOIN F190001 B
										ON B.TICKET_ID = A.TICKET_ID
										WHERE 1=1 ";
			if (ticketClass.Any())
				sql += parms.CombineSqlInParameters("AND B.TICKET_CLASS ", ticketClass, System.Data.SqlDbType.VarChar);
			else
				sql += " and 1=0 ";
			sql += " ORDER BY ISNULL(B.PRIORITY,0) ";
			return SqlQuery<F050004Ex>(sql, parms.ToArray());
		}
	}
}
