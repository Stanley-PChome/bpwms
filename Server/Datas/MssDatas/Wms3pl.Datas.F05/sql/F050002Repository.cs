using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050002Repository : RepositoryBase<F050002, Wms3plDbContext, F050002Repository>
	{
		public void Delete(List<string> ordNos, string gupCode, string custCode, string dcCode)
		{
			var parameters = new List<object>
						{
								gupCode,
								custCode,
								dcCode
						};

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("ORD_NO", ordNos, ref paramStartIndex);

			string sql = @"
				Delete 
				  From F050002
				 Where GUP_CODE=@p0
				   And CUST_CODE=@p1
				   And DC_CODE=@p2
				   And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public void DeleteF050002(string ordNo)
		{
			var sqlParams = new SqlParameter[]
			{
								 new SqlParameter("@p0", ordNo)
			};

			string sql = "delete from  F050002 Where ORD_NO=@p0";

			ExecuteSqlCommand(sql, sqlParams);
		}

		/// <summary>
		/// 取得一單一品的訂單 且無庫存
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F050002> GetF05002BySingleItem(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", itemCode)
						};

			var sql = @" SELECT A.* 
                         FROM F050002 A 
                        INNER JOIN 
                        (SELECT DC_CODE,GUP_CODE,CUST_CODE,ORD_NO,COUNT(ITEM_CODE) ITEMCOUNT 
                           FROM F050002 A 
                          GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ORD_NO) B 
                           ON B.DC_CODE = A.DC_CODE 
                          AND B.GUP_CODE = A.GUP_CODE 
                          AND B.CUST_CODE = A.CUST_CODE 
                          AND B.ORD_NO = A.ORD_NO 
                          AND B.ITEMCOUNT = 1 
                        WHERE NOT EXISTS 
                        (SELECT 1  
                           FROM F1913 C 
                          INNER JOIN F1912 D 
                             ON D.DC_CODE = C.DC_CODE 
                            AND D.LOC_CODE = C.LOC_CODE 
                          INNER JOIN F1980 E 
                             ON E.DC_CODE = C.DC_CODE 
                            AND E.WAREHOUSE_ID = D.WAREHOUSE_ID 
                          WHERE E.WAREHOUSE_TYPE ='G' 
                            AND C.DC_CODE = A.DC_CODE 
                            AND C.GUP_CODE = A.GUP_CODE 
                            AND C.CUST_CODE = A.CUST_CODE 
                            AND C.ITEM_CODE = A.ITEM_CODE 
                            AND C.QTY >0 ) 
                                  AND EXISTS 
                         (SELECT 1 
                      			 FROM F050001 F 
                          WHERE F.DC_CODE = A.DC_CODE 
                            AND F.GUP_CODE = A.GUP_CODE 
                            AND F.CUST_CODE = A.CUST_CODE 
                            AND F.ORD_NO = A.ORD_NO 
                            AND F.ORD_TYPE = '1' )
                          AND A.DC_CODE =@p0 
                          AND A.GUP_CODE = @p1 
                          AND A.CUST_CODE =@p2 
                          AND A.ITEM_CODE = @p3 ";

			var result = SqlQuery<F050002>(sql, parameters.ToArray());
			return result;
		}

		public void UpdateCheckedSameItem(string gupCode, string custCode, string dcCode, string itemCode, List<string> ordNos, string checkedSameItem)
		{
			var parameters = new List<object> {
								 checkedSameItem,
                 DateTime.Now,
                Current.Staff,
								Current.StaffName,
								gupCode,
								custCode,
								dcCode,
								itemCode
						};

			var inSql = parameters.CombineSqlInParameters("ORD_NO", ordNos);

			var sql = @"
				Update F050002 
				Set CHECKED_SAMEITEM=@p0
					, UPD_DATE = @p1
					, UPD_STAFF = @p2  
					, UPD_NAME = @p3 
				 Where GUP_CODE=@p4
				   And CUST_CODE=@p5
				   And DC_CODE=@p6
				   And ITEM_CODE=@p7
				   And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public void DeleteHasAllot()
		{
			var sql = @" DELETE X FROM F050002 X
                         WHERE EXISTS (
                         SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO 
                         FROM F050001 A
                         JOIN F050301 B
                         ON A.DC_CODE = B.DC_CODE
                         AND A.GUP_CODE = B.GUP_CODE
                         AND A.CUST_CODE = B.CUST_CODE
                         AND A.ORD_NO = B.ORD_NO
                         WHERE B.PROC_FLAG IN('1','9')
                         AND X.DC_CODE = A.DC_CODE
                         AND X.GUP_CODE = A.GUP_CODE
                         AND X.CUST_CODE = A.CUST_CODE
                         AND X.ORD_NO = A.ORD_NO
                         )  ";

			ExecuteSqlCommand(sql);
		}

		public void DeleteLackOrder(string gupCode, string custCode)
		{
			var param = new List<SqlParameter>
												{
																new SqlParameter("@p0", gupCode),
																new SqlParameter("@p1", custCode)
												};

			var sql = @"  DELETE X FROM F050002 X
                          WHERE EXISTS 
                          (SELECT A.GUP_CODE,A.CUST_CODE,A.ORD_NO
                           FROM F050301 A
                           WHERE A.PROC_FLAG ='0'
                           AND X.GUP_CODE = A.GUP_CODE
                           AND X.CUST_CODE = A.CUST_CODE
                           AND X.ORD_NO = A.ORD_NO)
                           AND GUP_CODE = @p0
                           AND CUST_CODE = @p1";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			var param = new List<object> { dcCode, gupCode, custCode };

			var inSql = param.CombineSqlInParameters("ORD_NO", ordNos);

			var sql = $@" DELETE
                     FROM F050002
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 
                      AND {inSql}";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public IQueryable<F050002> GetDatasByAllotBatchNo(string allotBatchNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",allotBatchNo){SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT *
                     FROM F050002 A
                     JOIN F050001 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.ORD_NO = A.ORD_NO 
                    WHERE B.ALLOT_BATCH_NO = @p0 ";
			return SqlQuery<F050002>(sql, parms.ToArray());
		}
	}
}
