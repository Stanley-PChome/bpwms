using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F192402Repository : RepositoryBase<F192402, Wms3plDbContext, F192402Repository>
	{
		public IQueryable<F192402> GetDatasByEmpId(string empId)
		{
			var sql = @" SELECT * FROM F192402 
                     WHERE EMP_ID = @p0";
			var parms = new List<SqlParameter> { new SqlParameter("@p0", empId) { SqlDbType = System.Data.SqlDbType.VarChar } };
			return SqlQuery<F192402>(sql, parms.ToArray());
		}

		public bool HasUserPermission(string dcCode,string gupCode,string custCode,string empId)
		{
			var parms = new List<SqlParameter> {
				new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3", empId) { SqlDbType = System.Data.SqlDbType.VarChar }
		};
			var sql = @" SELECT TOP (1) 1 
                     FROM F192402 
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND EMP_ID = @p3 ";
			return SqlQuery<int>(sql, parms.ToArray()).Any();
		}
	}
}
