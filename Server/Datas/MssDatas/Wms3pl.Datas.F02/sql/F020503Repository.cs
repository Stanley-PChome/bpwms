using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020503Repository : RepositoryBase<F020503, Wms3plDbContext, F020503Repository>
    {
        public IQueryable<F020503> GetData(string dcCode, string gupCode, string custCode, string empId, string status)
        {
            var parms = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", empId),
                new SqlParameter("@p4", status)
            };

            string sql = $@"
              SELECT B.* 
              FROM F020501 A, F020503 B
              WHERE A.ID = B.F020501_ID
                AND A.DC_CODE = @p0
                AND A.GUP_CODE = @p1
                AND A.CUST_CODE = @p2
                AND B.EMP_ID = @p3
                AND B.STATUS = @p4
                AND A.STATUS NOT IN('6','9')";

            return SqlQuery<F020503>(sql, parms.ToArray());
        }
    }
}
