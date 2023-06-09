using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
    public partial class F161602Repository : RepositoryBase<F161602, Wms3plDbContext, F161602Repository>
    {
        public IQueryable<F161602Ex> GetF161602Exs(string dcCode, string gupCode, string custCode, string rtnApplyNo)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", rtnApplyNo),
            };

            var sql = @"SELECT A.*, B.WAREHOUSE_ID SRC_WAREHOUSE_ID
                        FROM F161602 A
                        INNER JOIN F1912 B
                            ON A.DC_CODE = B.DC_CODE
                            AND (A.GUP_CODE = B.GUP_CODE OR B.GUP_CODE = '0')
                            AND (A.CUST_CODE = B.CUST_CODE OR B.CUST_CODE = '0')
                            AND A.SRC_LOC = B.LOC_CODE
                        WHERE A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.RTN_APPLY_NO = @p3
                        ORDER BY A.RTN_APPLY_SEQ";

            return SqlQuery<F161602Ex>(sql, parameters);
        }

        public void Delete(string dcCode, string gupCode, string custCode, string rtnApplyNo)
        {
            var parameters = new object[]
            {
                dcCode,
                gupCode, 
                custCode, 
                rtnApplyNo
            };

            var sql = @"DELETE FROM F161602 
                        WHERE   DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND RTN_APPLY_NO = @p3";

            ExecuteSqlCommand(sql, parameters);
        }
    }
}
