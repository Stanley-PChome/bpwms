using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F19
{
	public partial class F190305Repository : RepositoryBase<F190305, Wms3plDbContext, F190305Repository>
	{
    public IQueryable<F190305> GetDatas(string gupCode, string custCode, List<string> itemCodes)
    {

      var sql = @"SELECT * FROM F190305 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1";
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      sql += parameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, SqlDbType.VarChar);
      return SqlQuery<F190305>(sql, parameters.ToArray());
      #region 原LINQ
      //return _db.F190305s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
      //                                              x.CUST_CODE == custCode &&
      //                                              itemCodes.Contains(x.ITEM_CODE));
      #endregion
    }

  }
}
