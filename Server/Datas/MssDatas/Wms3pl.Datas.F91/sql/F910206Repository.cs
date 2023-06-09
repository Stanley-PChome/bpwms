using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910206Repository : RepositoryBase<F910206, Wms3plDbContext, F910206Repository>
    {
        public IQueryable<BackData> GetBackListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", processNo)
                        };

            string sql = @"SELECT SUM(E.A_TAR_QTY) A_TAR_QTY, '' ALLOCATION_NO,A.BACK_NO,
                                   A.ITEM_CODE,
                                   D.ITEM_NAME,
                                   A.CRT_DATE,
                                   A.CRT_STAFF,
                                   A.CRT_NAME,
                                   A.UPD_DATE,
                                   A.UPD_STAFF,
                                   A.UPD_NAME,
                                   A.GOOD_BACK_QTY,
                                   A.BREAK_BACK_QTY,
                                   A.BACK_ITEM_TYPE
                            FROM F910206 A 
							JOIN F1903 D ON A.GUP_CODE = D.GUP_CODE AND A.ITEM_CODE = D.ITEM_CODE AND A.CUST_CODE = D.CUST_CODE
                            JOIN F91020601 B
                            ON     A.GUP_CODE = B.GUP_CODE
                            	 AND A.CUST_CODE = B.CUST_CODE
                            	 AND A.DC_CODE = B.DC_CODE
                            	 AND A.PROCESS_NO = B.PROCESS_NO
                            	 AND A.BACK_NO = B.BACK_NO
                            	 AND A.ITEM_CODE = B.ITEM_CODE
                            	 AND A.BACK_ITEM_TYPE = B.BACK_ITEM_TYPE
                            JOIN F151001 C
                              ON     B.DC_CODE = C.DC_CODE
                            	 AND B.GUP_CODE = C.GUP_CODE
                            	 AND B.CUST_CODE = C.CUST_CODE
                            	 AND B.ALLOCATION_NO = C.ALLOCATION_NO
                            JOIN F151002 E
                              ON     C.DC_CODE = E.DC_CODE
                            	 AND C.GUP_CODE = E.GUP_CODE
                            	 AND C.CUST_CODE = E.CUST_CODE
                            	 AND C.ALLOCATION_NO = E.ALLOCATION_NO
                     WHERE A.DC_CODE = @p0
					   AND A.GUP_CODE = @p1
					   AND A.CUST_CODE = @p2
					   AND A.PROCESS_NO = @p3
					   GROUP BY A.BACK_NO,
                                   A.ITEM_CODE,
                                   D.ITEM_NAME,
                                   A.CRT_DATE,
                                   A.CRT_STAFF,
                                   A.CRT_NAME,
                                   A.UPD_DATE,
                                   A.UPD_STAFF,
                                   A.UPD_NAME,
                                   A.GOOD_BACK_QTY,
                                   A.BREAK_BACK_QTY,
                                   A.BACK_ITEM_TYPE";

            var result = SqlQuery<BackData>(sql, param.ToArray());
            return result;
        }

        public IQueryable<BackData> GetHistoryListForP9101010500(string dcCode, string gupCode, string custCode, string processNo)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", processNo)
                        };

            string sql = @"SELECT B.ALLOCATION_NO ALLOCATION_NO,
                            	　　A.BACK_NO,
                            	　　A.ITEM_CODE,
                            	　　D.ITEM_NAME,
                            	　　A.CRT_DATE,
                            	　　A.CRT_STAFF,
                            	　　A.CRT_NAME,
                            	　　A.UPD_DATE,
                            	　　A.UPD_STAFF,
                            	　　A.UPD_NAME,
                            	　　CASE WHEN B.IS_GOOD='1' THEN C.A_TAR_QTY ELSE 0 END AS GOOD_BACK_QTY,
                            	　　CASE WHEN B.IS_GOOD='0' THEN C.A_TAR_QTY ELSE 0 END AS BREAK_BACK_QTY,
                            	　　A.BACK_ITEM_TYPE
                            FROM F910206 A 
                            JOIN F91020601 B
                              ON     A.GUP_CODE = B.GUP_CODE
                            	 AND A.CUST_CODE = B.CUST_CODE
                            	 AND A.DC_CODE = B.DC_CODE
                            	 AND A.PROCESS_NO = B.PROCESS_NO
                            	 AND A.BACK_NO = B.BACK_NO
                            	 AND A.ITEM_CODE = B.ITEM_CODE
                            	 AND A.BACK_ITEM_TYPE = B.BACK_ITEM_TYPE
                            JOIN F151002 C
                              ON     B.DC_CODE = C.DC_CODE
                            	 AND B.GUP_CODE = C.GUP_CODE
                            	 AND B.CUST_CODE = C.CUST_CODE
                            	 AND B.ALLOCATION_NO = C.ALLOCATION_NO
                            JOIN F1903 D ON A.GUP_CODE = D.GUP_CODE AND A.ITEM_CODE = D.ITEM_CODE AND A.CUST_CODE = D.CUST_CODE
                            WHERE C.A_TAR_QTY > 0
                            AND A.DC_CODE = @p0
					        AND A.GUP_CODE = @p1
					        AND A.CUST_CODE = @p2
					        AND A.PROCESS_NO = @p3";

            var result = SqlQuery<BackData>(sql, param.ToArray());
            return result;
        }

        public IQueryable<BackData> GetBackList(string dcCode, string gupCode, string custCode, string processNo, bool isBacked, List<long> excludeBackNos)
        {
            var param = new List<object> {
                dcCode,
                gupCode,
                custCode,
                processNo
            };

            var paramCount = param.Count();
            var notInSql = param.CombineSqlNotInParameters("A.BACK_NO", excludeBackNos);
            var sqlFilter = string.Empty;
            if (isBacked)
                sqlFilter = " And C.STATUS <> '3'";
            else
                sqlFilter = " And C.STATUS = '3'";

            string sql = @"
				/* 此加工單待上架回倉商品與其回倉歷史明細 */
				SELECT Distinct C.ALLOCATION_NO, A.BACK_NO, A.ITEM_CODE, D.ITEM_NAME, A.CRT_DATE, A.CRT_STAFF, A.CRT_NAME, A.UPD_DATE, A.UPD_STAFF, A.UPD_NAME, A.GOOD_BACK_QTY, A.BREAK_BACK_QTY, C.STATUS, A.BACK_ITEM_TYPE
				  FROM F910206 A /* 回倉明細 */
				  JOIN F91020601 B ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.DC_CODE = B.DC_CODE AND A.PROCESS_NO = B.PROCESS_NO AND A.BACK_NO = B.BACK_NO AND A.ITEM_CODE = B.ITEM_CODE AND A.BACK_ITEM_TYPE = B.BACK_ITEM_TYPE
				  JOIN F151001 C ON B.DC_CODE = C.DC_CODE AND B.GUP_CODE = C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE AND B.ALLOCATION_NO = C.ALLOCATION_NO
				  JOIN F1903 D ON A.GUP_CODE = D.GUP_CODE AND A.ITEM_CODE = D.ITEM_CODE	AND A.CUST_CODE = D.CUST_CODE	
				 WHERE A.DC_CODE = @p0 
				   AND A.GUP_CODE = @p1
				   AND A.CUST_CODE = @p2
				   AND A.PROCESS_NO = @p3 
				   AND " + notInSql + sqlFilter;

            var result = SqlQuery<BackData>(sql, param.ToArray());
            return result;
        }
    }
}
