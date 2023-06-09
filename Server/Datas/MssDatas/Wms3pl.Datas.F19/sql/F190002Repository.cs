using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190002Repository : RepositoryBase<F190002, Wms3plDbContext, F190002Repository>
	{
		public IQueryable<F190002Data> GetF190002Data(string dcCode, string gupCode, string custCode)
		{
			var sqlParams = new SqlParameter[]{
				new SqlParameter("@p0",dcCode ),
				new SqlParameter("@p1",gupCode ),
				new SqlParameter("@p2",custCode ),
				new SqlParameter("@p3", Current.Staff)
			};
            #region SQL
            var sql = @"
                        SELECT TICKET_ID,
                               Sum(SWAREHOUSE) SWAREHOUSE,
                               Sum(TWAREHOUSE) TWAREHOUSE,
                               --Sum(OWAREHOUSE) OWAREHOUSE,
                               --Sum(BWAREHOUSE) BWAREHOUSE,
                               Sum(GWAREHOUSE) GWAREHOUSE,
                               Sum(NWAREHOUSE) NWAREHOUSE,
                               Sum(WWAREHOUSE) WWAREHOUSE,
                               Sum(RWAREHOUSE) RWAREHOUSE,
                               Sum(DWAREHOUSE) DWAREHOUSE,
                               Sum(MWAREHOUSE) MWAREHOUSE,
                               --Sum(UWAREHOUSE) UWAREHOUSE,
                               --Sum(VWAREHOUSE) VWAREHOUSE,
                               TICKET_NAME,
                               TICKET_TYPE,
                               TICKET_CLASS,
                               ORD_NAME,
                               TICKET_CLASS_NAME,
                               CUST_NAME,
                               DC_CODE,
                               GUP_CODE,
                               CUST_CODE
                        FROM   (SELECT a.TICKET_ID,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'S' THEN 1
                                         ELSE 0
                                       END SWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'T' THEN 1
                                         ELSE 0
                                       END TWAREHOUSE,
                                       --CASE
                                         --WHEN b.WAREHOUSE_TYPE = 'O' THEN 1
                                         --ELSE 0
                                       --END OWAREHOUSE,
                                       --CASE
                                         --WHEN b.WAREHOUSE_TYPE = 'B' THEN 1
                                         --ELSE 0
                                       --END BWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'G' THEN 1
                                         ELSE 0
                                       END GWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'N' THEN 1
                                         ELSE 0
                                       END NWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'W' THEN 1
                                         ELSE 0
                                       END WWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'R' THEN 1
                                         ELSE 0
                                       END RWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'D' THEN 1
                                         ELSE 0
                                       END DWAREHOUSE,
                                       CASE
                                         WHEN b.WAREHOUSE_TYPE = 'M' THEN 1
                                         ELSE 0
                                       END MWAREHOUSE,
                                       --CASE
                                         --WHEN b.WAREHOUSE_TYPE = 'U' THEN 1
                                         --ELSE 0
                                      -- END UWAREHOUSE,
                                       --CASE
                                         --WHEN b.WAREHOUSE_TYPE = 'V' THEN 1
                                         --ELSE 0
                                       --END VWAREHOUSE,
                                       a.TICKET_NAME,
                                       a.TICKET_TYPE,
                                       a.TICKET_CLASS,
                                       c.ORD_NAME,
                                       d.TICKET_CLASS_NAME,
                                       e.CUST_NAME,
                                       a.DC_CODE,
                                       a.GUP_CODE,
                                       a.CUST_CODE
                                FROM   F190001 a
                                       LEFT JOIN F190002 b
                                              ON a.TICKET_ID = b.TICKET_ID
                                       LEFT JOIN F000901 c
                                              ON a.TICKET_TYPE = c.ORD_TYPE
                                       LEFT JOIN F000906 d
                                              ON a.TICKET_CLASS = d.TICKET_CLASS
                                       LEFT JOIN F1909 e
                                              ON a.GUP_CODE = e.GUP_CODE
                                                 AND a.CUST_CODE = e.CUST_CODE
                                WHERE  a.DC_CODE = @p0
                                       AND a.GUP_CODE = ( CASE
                                                            WHEN @p1 = '' THEN a.GUP_CODE
                                                            ELSE @p1
                                                          END )
                                       AND a.CUST_CODE = ( CASE
                                                             WHEN @p2 = '' THEN a.CUST_CODE
                                                             ELSE @p2
                                                           END )
                                       AND EXISTS (SELECT 1
                                                   FROM   F190101 cc
                                                          INNER JOIN (SELECT *
                                                                      FROM   F192402
                                                                      WHERE  EMP_ID = @p3) dd
                                                                  ON cc.DC_CODE = dd.DC_CODE
                                                                     AND cc.GUP_CODE = dd.GUP_CODE
                                                                     AND cc.CUST_CODE = dd.CUST_CODE
                                                   WHERE  cc.DC_CODE = a.DC_CODE
                                                          AND cc.GUP_CODE = a.GUP_CODE
                                                          AND cc.CUST_CODE = a.CUST_CODE)) Tmp
                        GROUP  BY TICKET_ID,
                                  TICKET_NAME,
                                  TICKET_TYPE,
                                  TICKET_CLASS,
                                  ORD_NAME,
                                  TICKET_CLASS_NAME,
                                  CUST_NAME,
                                  DC_CODE,
                                  GUP_CODE,
                                  CUST_CODE 
                        ";
            #endregion
            return SqlQuery<F190002Data>(sql, sqlParams);
		}
	}
}
