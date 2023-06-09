using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F076101Repository : RepositoryBase<F076101, Wms3plDbContext, F076101Repository>
	{
		public string LockF076101()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F076101';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DeleteByContainerCode(string containerCode)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",containerCode){ SqlDbType = System.Data.SqlDbType.VarChar}
			};
			var sql = @"DELETE FROM F076101 WHERE CONTAINER_CODE =@p0";
			ExecuteSqlCommand(sql, parms.ToArray());
		}
	}
}
