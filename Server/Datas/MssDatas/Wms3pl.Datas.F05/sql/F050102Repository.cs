using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050102Repository : RepositoryBase<F050102, Wms3plDbContext, F050102Repository>
    {
        //訂單明細
        public IQueryable<F050102Ex> GetF050102ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", ordNo)
            };

            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A.ORD_SEQ ASC) ROWNUM,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
                          A.ORD_NO,A.ORD_SEQ,A.ITEM_CODE,A.ORD_QTY,A.NO_DELV,
                          A.SERIAL_NO,A.CRT_DATE,A.CRT_STAFF,A.CRT_NAME,
                          C.ITEM_NAME,C.ITEM_SIZE,C.ITEM_SPEC,C.ITEM_COLOR,
                          C.BUNDLE_SERIALLOC,C.BUNDLE_SERIALNO,D.B_DELV_QTY,D.A_DELV_QTY,A.MAKE_NO 
                     FROM F050102 A
									   LEFT JOIN F1903 C
											 ON A.ITEM_CODE = C.ITEM_CODE
									    AND A.GUP_CODE = C.GUP_CODE
									    AND A.CUST_CODE = C.CUST_CODE
                    LEFT JOIN (
										SELECT DC_CODE,GUP_CODE,CUST_CODE,ORD_NO,ORD_SEQ,SUM(B_DELV_QTY) B_DELV_QTY,SUM(A_DELV_QTY) A_DELV_QTY
										FROM F05030202
										GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ORD_NO,ORD_SEQ) D
                       ON D.DC_CODE = A.DC_CODE
                      AND D.GUP_CODE = A.GUP_CODE
                      AND D.CUST_CODE = A.CUST_CODE
                      AND D.ORD_NO = A.ORD_NO
                      AND D.ORD_SEQ = A.ORD_SEQ
                    WHERE A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2 
                      AND A.ORD_NO = @p3";

            var result = SqlQuery<F050102Ex>(sql, parameters.ToArray());

            return result;
        }

        public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> ordNos)
        {
            var param = new List<object> { dcCode, gupCode, custCode };

            var inSql = param.CombineSqlInParameters("ORD_NO", ordNos);

            var sql = $@" DELETE
                     FROM F050102
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 
                      AND {inSql}";

            ExecuteSqlCommand(sql, param.ToArray());
        }

        public void BulkDelete(string dcCode, string gupCode, string custCode, string ordNo, List<string> ordSeqs)
        {
            var param = new List<object> { dcCode, gupCode, custCode, ordNo };

            var inSql = param.CombineSqlInParameters("ORD_SEQ", ordSeqs);

            var sql = $@" DELETE
                              FROM F050102
                             WHERE DC_CODE = @p0
                               AND GUP_CODE = @p1
                               AND CUST_CODE = @p2 
                               AND ORD_NO = @p3
                               AND {inSql}";

            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}