using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0533Repository : RepositoryBase<F0533, Wms3plDbContext, F0533Repository>
	{

		public void UpdateContainerCodeByF0531Id(long f0531_ID, string newContainerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", newContainerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = $@" UPDATE F0533
							SET OUT_CONTAINER_CODE = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE F0531_ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}
	}
}
