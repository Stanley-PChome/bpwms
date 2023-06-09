using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;

namespace Wms3pl.Datas.F01
{
    public partial class F010202Repository : RepositoryBase<F010202, Wms3plDbContext, F010202Repository>
    {
        public IQueryable<P010202SearchResult> GetF010202(string gupCode, string custCode, string dcCode, DateTime changeDateBegin, DateTime changeDateEnd)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", changeDateBegin),
                                new SqlParameter("@p4", changeDateEnd)
                        };

            string sql = $@"SELECT a.DC_CODE,
                            	   a.GUP_CODE,
                            	   a.CUST_CODE,
                            	   F.GUP_NAME,
                            	   G.CUST_NAME,
                            	   a.ALLOCATION_DATE AS ChangeDate,
                            	   ISNULL(a.SOURCE_NO,a.ALLOCATION_NO) AS ReceiptNo, b.ITEM_CODE,
                            	   E.ITEM_NAME, 
                            	   E.ITEM_SIZE, 
                            	   E.ITEM_SPEC, 
                            	   E.ITEM_COLOR,
                            	   SUM(b.A_TAR_QTY) AS ChangeNumber                                                                                                                                                                                                                                 
                            FROM F151001 a,F151002 b,F1903 E,F1929 F,F1909 G
                            WHERE a.CUST_CODE = @p2
                                  AND a.GUP_CODE = @p1
                                  AND a.DC_CODE = @p0
                                  AND b.CUST_CODE = a.CUST_CODE
                                  AND b.GUP_CODE = a.GUP_CODE
                                  AND b.DC_CODE = a.DC_CODE
                                  AND b.ITEM_CODE = E.ITEM_CODE
                                  AND b.GUP_CODE = E.GUP_CODE
                                  AND b.CUST_CODE = E.CUST_CODE
                                  AND a.ALLOCATION_NO = b.ALLOCATION_NO
                                  AND a.GUP_CODE = F.GUP_CODE
                                  AND a.GUP_CODE = G.GUP_CODE
                                  AND a.CUST_CODE = G.CUST_CODE
                                  AND a.ALLOCATION_DATE >= @p3
                                  AND a.ALLOCATION_DATE <= @p4
                                  AND a.SOURCE_TYPE = '04'
                                  GROUP BY a.DC_CODE,a.GUP_CODE,a.CUST_CODE,F.GUP_NAME,G.CUST_NAME, a.ALLOCATION_DATE, a.SOURCE_TYPE,
                            			   ISNULL(a.SOURCE_NO,a.ALLOCATION_NO), b.ITEM_CODE,E.ITEM_NAME, E.ITEM_SIZE, E.ITEM_SPEC, E.ITEM_COLOR
                            	  ORDER BY ReceiptNo";

            var result = SqlQuery<P010202SearchResult>(sql, param.ToArray());

            return result;
        }
    }
}
