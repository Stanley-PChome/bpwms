using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F190105Repository : RepositoryBase<F190105, Wms3plDbContext, F190105Repository>
  {
    public IQueryable<F190105> GetF190105Data(string dcCode)
    {
      var sql = @"SELECT * FROM F190105 WHERE DC_CODE = @p0";

      var parameter = new List<SqlParameter>()
      {
        new SqlParameter("@p0", dcCode),
      };



      return SqlQuery<F190105>(sql, parameter.ToArray());
    }

    public IQueryable<F190105> GetDatas(List<string> dcCodes)
    {
      if (dcCodes == null || !dcCodes.Any())
        return null;

      var sql = @"SELECT * FROM F190105 WHERE ";

      var parameter = new List<SqlParameter>();
      sql += parameter.CombineSqlInParameters(" DC_CODE", dcCodes, System.Data.SqlDbType.VarChar);
      return SqlQuery<F190105>(sql, parameter.ToArray());
    }

  }
}
