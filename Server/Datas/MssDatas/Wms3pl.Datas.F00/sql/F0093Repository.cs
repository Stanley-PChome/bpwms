using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  public partial class F0093Repository : RepositoryBase<F0093, Wms3plDbContext, F0093Repository>
  {
    public IQueryable<F0093> GetDatasBySchedule(string dcCode, string gupCode, string custCode, int midApiReLmt)
    {
      var parms = new List<SqlParameter>()
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", midApiReLmt) { SqlDbType = System.Data.SqlDbType.Int },
      };
      var sql = @" SELECT *
						       FROM F0093
						       WHERE DC_CODE = @p0
							       AND GUP_CODE = @p1
							       AND CUST_CODE = @p2
                     AND RESENT_CNT < @p3
                     AND STATUS IN ('0', 'T')";

      return SqlQuery<F0093>(sql, parms.ToArray());
    }
  }
}
