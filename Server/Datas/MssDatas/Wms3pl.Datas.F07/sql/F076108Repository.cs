using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F076108Repository : RepositoryBase<F076108, Wms3plDbContext, F076108Repository>
	{
    public string LockF076108()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F076108';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public void DeleteByAllocationNo(List<string> allocationNos)
    {
      if (allocationNos == null || !allocationNos.Any())
        return;

      var parms = new List<SqlParameter>();
      var sql = @"DELETE FROM F076108 WHERE ";
      sql += parms.CombineSqlInParameters(" ALLOCATION_NO", allocationNos, System.Data.SqlDbType.VarChar);

      ExecuteSqlCommand(sql, parms.ToArray());
    }

  }
}
