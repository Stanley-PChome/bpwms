using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Wms3pl.Datas.F06
{
	public partial class F060402Repository
	{
    public IQueryable<F060402> GetDatasForExecute(string dcCode, string gupCode, string custCode)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"SELECT * FROM F060402 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND STATUS='0'";
      return SqlQuery<F060402>(sql, para.ToArray());
      #region 原LINQ語法
      /*
      return _db.F060402s.Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      x.STATUS == "0");
      */
      #endregion
    }


  }
}
