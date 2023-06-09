using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  public partial class F076104Repository
  {
    public string LockF076104()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F076104';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public void DeleteByContainerCode(List<string> containerCodes)
    {
      if (containerCodes == null || !containerCodes.Any())
        return;

      var parms = new List<SqlParameter>();
      var sql = @"DELETE FROM F076104 WHERE ";
      sql += parms.CombineSqlInParameters(" CONTAINER_CODE", containerCodes, System.Data.SqlDbType.VarChar);

      ExecuteSqlCommand(sql, parms.ToArray());
    }

  }
}
