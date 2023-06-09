using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910205Repository : RepositoryBase<F910205, Wms3plDbContext, F910205Repository>
    {
        /// <summary>
		/// 讀取加工揀料單(調撥)資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="processNo"></param>
		/// <returns></returns>
		public IQueryable<PickReport> GetPickTicketReport(string dcCode, string gupCode, string custCode, string processNo)
        {
            var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY PROCESS_NO, PICK_NO) ROWNUM,PICK.DC_CODE,PICK.GUP_CODE,PICK.CUST_CODE,
                               PICK.PROCESS_NO,PICK.PICK_NO,PICK.ALLOCATION_NO,PICK.ITEM_CODE,
                               ITEM.ITEM_NAME,ITEM.ITEM_SIZE,ITEM.ITEM_SPEC,ITEM.ITEM_COLOR,
                               PICK.VALID_DATE,PICK.PICK_LOC,PICK.PICK_QTY,PICK.SERIAL_NO,LOC.WAREHOUSE_ID,
                        			 W.WAREHOUSE_NAME,W.TMPR_TYPE,X.NAME,LOC.FLOOR , PICK.MAKE_NO
                          FROM (SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PROCESS_NO,A.PICK_NO,'' ALLOCATION_NO,
                                       B.ITEM_CODE,CONVERT(varchar, B.VALID_DATE, 111) VALID_DATE,B.PICK_LOC,B.PICK_QTY,
                                       CASE B.SERIAL_NO WHEN '0' THEN NULL ELSE B.SERIAL_NO END AS SERIAL_NO,
                                       CASE B.MAKE_NO WHEN '0' THEN NULL ELSE B.MAKE_NO END AS MAKE_NO
                                  FROM F910205 A
                                       LEFT JOIN F91020501 B
                                          ON     A.DC_CODE = B.DC_CODE
                                             AND A.GUP_CODE = B.GUP_CODE
                                             AND A.CUST_CODE = B.CUST_CODE
                                             AND A.PICK_NO = B.PICK_NO
                                 WHERE     A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.PROCESS_NO = @p3
                        							 AND B.ITEM_CODE IS NOT NULL
                                UNION ALL
                                SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PROCESS_NO,A.PICK_NO,C.ALLOCATION_NO,
                                       D.ITEM_CODE,CONVERT(varchar, D.VALID_DATE, 111) VALID_DATE,D.SRC_LOC_CODE AS PICK_LOC,D.SRC_QTY AS PICK_QTY,
                                       CASE D.SERIAL_NO WHEN '0' THEN NULL ELSE D.SERIAL_NO END AS SERIAL_NO ,
                                       CASE D.MAKE_NO WHEN '0' THEN NULL ELSE D.MAKE_NO END AS MAKE_NO 
                                  FROM F910205 A
                                       LEFT JOIN F91020502 C
                                          ON     A.DC_CODE = C.DC_CODE
                                             AND A.GUP_CODE = C.GUP_CODE
                                             AND A.CUST_CODE = C.CUST_CODE
                                             AND A.PICK_NO = C.PICK_NO
                                       INNER JOIN F151002 D
                                          ON     C.DC_CODE = D.DC_CODE
                                             AND C.GUP_CODE = D.GUP_CODE
                                             AND C.CUST_CODE = D.CUST_CODE
                                             AND C.ALLOCATION_NO = D.ALLOCATION_NO
                                 WHERE     A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.PROCESS_NO = @p3) PICK
                               LEFT JOIN F1912 LOC
                                  ON PICK.DC_CODE = LOC.DC_CODE AND PICK.PICK_LOC = LOC.LOC_CODE
                               LEFT JOIN F1980 W
                                  ON LOC.DC_CODE = W.DC_CODE AND LOC.WAREHOUSE_ID = W.WAREHOUSE_ID
                               LEFT JOIN VW_F000904_LANG X
                                  ON     W.TMPR_TYPE = X.VALUE
                                     AND X.TOPIC = 'F1980'
                                     AND X.SUBTOPIC = 'TMPR_TYPE'
                        			 AND X.LANG = '{Current.Lang}'
                               LEFT JOIN F1903 ITEM
                                  ON     PICK.GUP_CODE = ITEM.GUP_CODE
                                     AND PICK.ITEM_CODE = ITEM.ITEM_CODE
                                      AND PICK.CUST_CODE = ITEM.CUST_CODE";

            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", processNo)
            };

            var result = SqlQuery<PickReport>(sql, param.ToArray());
            return result;
        }
    }
}
