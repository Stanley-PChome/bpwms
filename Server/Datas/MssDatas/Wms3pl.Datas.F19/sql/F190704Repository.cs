using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F190704Repository : RepositoryBase<F190704, Wms3plDbContext, F190704Repository>
  {
    public void DeleteF190704ByGroupId(string gid)
    {
      var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", gid)
			};

      var sql = "DELETE FROM F190704 WHERE GRP_ID = @p0 ";

      ExecuteSqlCommand(sql, parameters.ToArray());
    }

    public void InsertF190704(string gid, string qid, string staff, string name)
    {
      var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", gid),
        new SqlParameter("@p1", qid),
        new SqlParameter("@p2", staff),
        new SqlParameter("@p3", name)
			};

      var sql = @"
INSERT INTO F190704 (GRP_ID, QID, CRT_STAFF, CRT_NAME, CRT_DATE) 
VALUES (@p0, @p1, @p2, @p3 , dbo.GetSysDate())
";

      ExecuteSqlCommand(sql, parameters.ToArray());
    }
  }
}
