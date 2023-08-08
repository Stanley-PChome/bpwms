using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060403Repository
	{
    public IQueryable<F060403> GetDatasForExecute(string dcCode, string gupCode, string custCode, List<string> docIds)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"SELECT * FROM F060403 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2";
      sql += para.CombineSqlInParameters(" AND DOC_ID", docIds, SqlDbType.VarChar);
      return SqlQuery<F060403>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      return _db.F060403s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      docIds.Contains(x.DOC_ID));
      */
      #endregion
    }

  }
}
