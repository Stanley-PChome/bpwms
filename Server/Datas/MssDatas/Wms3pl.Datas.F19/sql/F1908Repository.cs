using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1908Repository : RepositoryBase<F1908, Wms3plDbContext, F1908Repository>
	{

    public IQueryable<F1908> GetDatas(string gupCode, string custCode, List<string> vnrCodes)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
      };

      var sql = @"SELECT * FROM F1908 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1";

      sql += para.CombineSqlInParameters(" AND VNR_CODE", vnrCodes, SqlDbType.VarChar);

      return SqlQuery<F1908>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      var query = _db.F1908s
                  .Where(x => x.GUP_CODE == gupCode)
                  .Where(x => x.CUST_CODE == custCode)
                  .Where(x => vnrCodes.Contains(x.VNR_CODE));
      return query.Select(x => x);
      */
      #endregion
    }

  }
}
