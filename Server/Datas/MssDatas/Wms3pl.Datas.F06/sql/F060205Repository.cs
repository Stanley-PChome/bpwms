using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
  public partial class F060205Repository
  {

    public IQueryable<F060205> GetDatasForDocIds(List<string> docIds)
    {
      var para = new List<SqlParameter>();
      var sql = @"SELECT * FROM F060205 WHERE";
      if (docIds.Any())
      {
        sql += para.CombineSqlInParameters(" DOC_ID", docIds, SqlDbType.VarChar);
      }
      else
        sql += " 1=0";

      return SqlQuery<F060205>(sql, para.ToArray());
    }

  }
}
