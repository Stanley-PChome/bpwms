using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F192401Repository : RepositoryBase<F192401, Wms3plDbContext, F192401Repository>
  {
    public int CheckAccFunction(string funcNo, string accNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", funcNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", accNo) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
SELECT 
  COUNT(*) 
FROM F192401 A
INNER JOIN F195301 B
  ON A.GRP_ID=B.GRP_ID
";
      return SqlQuery<int>(sql, para.ToArray()).First();
    }

    public void Delete(string empId, decimal? groupId = null)
    {
      string sql = @"
				DELETE F192401 WHERE EMP_ID = @p0 AND GRP_ID = @p1 
			";
      var param = new[] {
                new SqlParameter("@p0", empId),
                new SqlParameter("@p1", groupId)
            };

      ExecuteSqlCommand(sql, param);
    }

    public void Delete(decimal groupId)
    {
      string sql = @"
				DELETE F192401 WHERE  GRP_ID = @p0
			";
      var param = new[] {
                new SqlParameter("@p0", groupId)
            };

      ExecuteSqlCommand(sql, param);
    }
  }
}
