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

	}
}
