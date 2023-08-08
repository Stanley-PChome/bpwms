using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0536Repository : RepositoryBase<F0536, Wms3plDbContext, F0536Repository>
	{
		public F0536 GetDataByF0701Id(long f0701_Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"
                        SELECT TOP(1) *
                          FROM F0536 
                         WHERE F0701_ID = @p0
                       ";

			return SqlQuery<F0536>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateStatusByF0701Id(long f0701_Id, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0536
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE F0701_ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}
	}
}
