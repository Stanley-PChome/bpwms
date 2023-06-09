﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F076102Repository : RepositoryBase<F076102, Wms3plDbContext, F076102Repository>
	{
		public string LockF076102()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F076102';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DeleteByContainerCode(List<string> containerCodes)
		{
      if (containerCodes == null || !containerCodes.Any())
        return;

      var parms = new List<SqlParameter>();
      var sql = @"DELETE FROM F076102 WHERE ";
      sql += parms.CombineSqlInParameters(" CONTAINER_CODE", containerCodes, System.Data.SqlDbType.VarChar);

      ExecuteSqlCommand(sql, parms.ToArray());
		}
	}
}