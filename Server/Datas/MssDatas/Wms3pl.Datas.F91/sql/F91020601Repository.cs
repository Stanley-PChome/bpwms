using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F91020601Repository : RepositoryBase<F91020601, Wms3plDbContext, F91020601Repository>
    {
        public IQueryable<string> GetBackedAllocationNos(string dcCode, string gupCode, string custCode, string processNo)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", processNo)
                        };

            string sql = @"
				/* 此加工單已上架回倉商品與其回倉歷史明細 */
				SELECT Distinct C.ALLOCATION_NO
				  FROM F91020601 B
				  JOIN F151001 C ON B.DC_CODE = C.DC_CODE AND B.GUP_CODE = C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE AND B.ALLOCATION_NO = C.ALLOCATION_NO
                  JOIN 
				  (
				  SELECT DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO FROM F151002 
				  WHERE STATUS = '2' 
				  GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO
				  HAVING COUNT(*) > 0 
				  ) D
				  ON D.DC_CODE = C.DC_CODE AND D.GUP_CODE = C.GUP_CODE AND D.CUST_CODE = C.CUST_CODE AND D.ALLOCATION_NO = C.ALLOCATION_NO
				 WHERE B.DC_CODE = @p0 
				   AND B.GUP_CODE = @p1
				   AND B.CUST_CODE = @p2
				   AND B.PROCESS_NO = @p3
				   AND C.STATUS <> '3' AND C.STATUS<>'9'
				";

            var result = SqlQuery<string>(sql, param.ToArray());
            return result;
        }

        public IQueryable<string> GetProcessedAllocationNos(string dcCode, string gupCode, string custCode, string processNo)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", processNo)
                        };

            string sql = @"
				/* 此加工單已上架回倉商品與其回倉歷史明細 */
				SELECT Distinct C.ALLOCATION_NO
				  FROM F91020601 B
				  JOIN F151001 C ON B.DC_CODE = C.DC_CODE AND B.GUP_CODE = C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE AND B.ALLOCATION_NO = C.ALLOCATION_NO
                  JOIN 
				  (
				  SELECT DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO FROM F151002 
				  WHERE STATUS = '2' OR A_TAR_QTY > 0
				  GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO
				  HAVING COUNT(*) > 0 
				  ) D
				  ON D.DC_CODE = C.DC_CODE AND D.GUP_CODE = C.GUP_CODE AND D.CUST_CODE = C.CUST_CODE AND D.ALLOCATION_NO = C.ALLOCATION_NO
				 WHERE B.DC_CODE = @p0 
				   AND B.GUP_CODE = @p1
				   AND B.CUST_CODE = @p2
				   AND B.PROCESS_NO = @p3
				   AND C.STATUS<>'9'
				";

            var result = SqlQuery<string>(sql, param.ToArray());
            return result;
        }

        /// <summary>
		/// 上架回倉報表: 預覽或列印當前尚未完成回倉的回倉明細調撥單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <returns></returns>
		public IQueryable<P91010105Report> GetP91010105Reports(string dcCode, string gupCode, string custCode, string processNo)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", processNo)
                        };

            var sql = @"
                 SELECT ROW_NUMBER() OVER (ORDER BY A.ITEM_CODE, A.SUG_LOC_CODE DESC) ROWNUM,A.*
                   FROM (
									 SELECT TOP 100 PERCENT
									 C.ALLOCATION_NO,
									 C.TAR_WAREHOUSE_ID,
									 G.WAREHOUSE_NAME,
									 D.GUP_NAME,
									 E.SHORT_NAME AS CUST_NAME,
									 A.ITEM_CODE,
									 F.EAN_CODE1,
									 F.ITEM_NAME,
									 F.ITEM_SIZE,
									 F.ITEM_SPEC,
									 F.ITEM_COLOR,
									 CASE WHEN LEN(B.SUG_LOC_CODE) = 9 
										THEN SUBSTRING(B.SUG_LOC_CODE,1,1) + '-' + SUBSTRING(B.SUG_LOC_CODE,2,2)  + '-' + SUBSTRING(B.SUG_LOC_CODE,4,2)  + '-' + SUBSTRING(B.SUG_LOC_CODE,6,2)  + '-' + SUBSTRING(B.SUG_LOC_CODE,8,2) 
										ELSE B.SUG_LOC_CODE 
									 END AS SUG_LOC_CODE,
									 ISNULL (SUM (B.TAR_QTY), 0) AS TAR_QTY
								FROM (SELECT DISTINCT DC_CODE,GUP_CODE,CUST_CODE,PROCESS_NO,ITEM_CODE,SERIAL_NO,ALLOCATION_NO
												FROM F91020601 ) A
									 JOIN F151002 B
										ON     A.ALLOCATION_NO = B.ALLOCATION_NO
											 AND A.DC_CODE = B.DC_CODE
											 AND A.GUP_CODE = B.GUP_CODE
											 AND A.CUST_CODE = B.CUST_CODE
											 AND A.ITEM_CODE = B.ITEM_CODE
											 AND CASE WHEN A.SERIAL_NO = '' or A.SERIAL_NO is null THEN '0' ELSE A.SERIAL_NO END = CASE WHEN B.SERIAL_NO = '' or B.SERIAL_NO is null THEN '0' ELSE B.SERIAL_NO END
									 JOIN F151001 C
										ON     B.ALLOCATION_NO = C.ALLOCATION_NO
											 AND B.DC_CODE = C.DC_CODE
											 AND B.GUP_CODE = C.GUP_CODE
											 AND B.CUST_CODE = C.CUST_CODE
									 LEFT JOIN F1929 D ON A.GUP_CODE = D.GUP_CODE
									 LEFT JOIN F1909 E
										ON A.GUP_CODE = E.GUP_CODE AND A.CUST_CODE = E.CUST_CODE
									 LEFT JOIN F1903 F
										ON A.GUP_CODE = F.GUP_CODE AND A.ITEM_CODE = F.ITEM_CODE AND A.CUST_CODE = F.CUST_CODE
									 LEFT JOIN F1980 G
										ON C.DC_CODE = G.DC_CODE AND C.TAR_WAREHOUSE_ID = G.WAREHOUSE_ID
								 WHERE     C.STATUS = '3'                   -- 尚未完成回倉的回倉明細調撥單 就是調撥單狀態為上架中(3)
									 AND A.DC_CODE = @p0
									 AND A.GUP_CODE = @p1
									 AND A.CUST_CODE = @p2
									 AND A.PROCESS_NO = @p3
							GROUP BY 
									 C.ALLOCATION_NO,
									 C.TAR_WAREHOUSE_ID,
									 G.WAREHOUSE_NAME,
									 D.GUP_NAME,
									 E.SHORT_NAME,
									 A.ITEM_CODE,
									 F.EAN_CODE1,
									 F.ITEM_NAME,
									 F.ITEM_SIZE,
									 F.ITEM_SPEC,
									 F.ITEM_COLOR,
									 B.SUG_LOC_CODE
							ORDER BY A.ITEM_CODE, B.SUG_LOC_CODE ) A";

            var result = SqlQuery<P91010105Report>(sql, param.ToArray());
            return result;
        }

        public IQueryable<BackItemSumQty> GetBackItemSumQtys(string dcCode, string gupCode, string custCode, string processNo, bool isBacked)
        {
            var param = new object[] {
                dcCode,
                gupCode,
                custCode,
                processNo
            };

            var sqlFilter = string.Empty;
            if (isBacked)
                sqlFilter = " AND C.STATUS <> '3'";
            else
                sqlFilter = " AND C.STATUS = '3'";

            string sql = @"
				/* 此加工單上架回倉商品與其回倉歷史明細 */
				SELECT B.ALLOCATION_NO, B.BACK_NO, B.ITEM_CODE, B.SERIAL_NO, B.BACK_ITEM_TYPE, B.IS_GOOD, SUM(B.QTY) SumQty
				  FROM F91020601 B
				  JOIN F151001 C ON B.DC_CODE = C.DC_CODE AND B.GUP_CODE = C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE AND B.ALLOCATION_NO = C.ALLOCATION_NO
				 WHERE B.DC_CODE = @p0 
				   AND B.GUP_CODE = @p1
				   AND B.CUST_CODE = @p2
				   AND B.PROCESS_NO = @p3 " + sqlFilter + @"
				 GROUP BY B.ALLOCATION_NO, B.BACK_NO, B.ITEM_CODE, B.SERIAL_NO, B.BACK_ITEM_TYPE, B.IS_GOOD
				";

            var result = SqlQuery<BackItemSumQty>(sql, param.ToArray());
            return result;
        }
    }
}
