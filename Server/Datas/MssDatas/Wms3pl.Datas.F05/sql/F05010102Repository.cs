using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;
using System.Collections.Generic;

namespace Wms3pl.Datas.F05
{
    public partial class F05010102Repository : RepositoryBase<F05010102, Wms3plDbContext, F05010102Repository>
    {
        public IQueryable<F05010102> GetF05010102sByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var sqlParams = new SqlParameter[]
            {
                 new SqlParameter("@p0", dcCode),
                 new SqlParameter("@p1", gupCode),
                 new SqlParameter("@p2", custCode),
                 new SqlParameter("@p3", wmsOrdNo)
            };

            var sql = @"SELECT D.*
						  FROM F050801 A
							   JOIN F05030101 B
								  ON     B.DC_CODE = A.DC_CODE
									 AND B.GUP_CODE = A.GUP_CODE
									 AND B.CUST_CODE = A.CUST_CODE
									 AND B.WMS_ORD_NO = A.WMS_ORD_NO
							   JOIN F050301 C
								  ON     C.DC_CODE = B.DC_CODE
									 AND C.GUP_CODE = B.GUP_CODE
									 AND C.CUST_CODE = B.CUST_CODE
									 AND C.ORD_NO = B.ORD_NO
							   JOIN F05010102 D
								  ON     D.DC_CODE = D.DC_CODE
									 AND D.GUP_CODE = D.GUP_CODE
									 AND D.CUST_CODE = D.CUST_CODE
									 AND D.CUST_ORD_NO = C.CUST_ORD_NO
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND A.WMS_ORD_NO = @p3";

            var result = SqlQuery<F05010102>(sql, sqlParams.ToArray());

            return result;
        }

        public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> custOrdNos)
        {
            var param = new List<object> { dcCode, gupCode, custCode };

            var inSql = param.CombineSqlInParameters("CUST_ORD_NO", custOrdNos);
            
            var sql = $@" DELETE
                     FROM F05010102
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND {inSql}";

            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
