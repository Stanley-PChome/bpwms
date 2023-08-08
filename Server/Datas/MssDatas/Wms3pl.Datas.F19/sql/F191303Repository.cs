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
	public partial class F191303Repository : RepositoryBase<F191303, Wms3plDbContext, F191303Repository>
	{
    public IQueryable<F191303> GetProcessData(string dcCode, string gupCode, string custCode)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType=System.Data.SqlDbType.VarChar},
        new SqlParameter("@p1", gupCode) {SqlDbType=System.Data.SqlDbType.VarChar},
        new SqlParameter("@p2", custCode) {SqlDbType=System.Data.SqlDbType.VarChar}
      };

      var sql = @"SELECT * FROM F191303 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND PROC_FLAG = '0'";

      return SqlQuery<F191303>(sql, parameters.ToArray());
    }
	}
}
