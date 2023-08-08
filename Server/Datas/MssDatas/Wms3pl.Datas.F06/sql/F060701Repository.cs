using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Wms3pl.Datas.F06
{
	public partial class F060701Repository
	{
		public IQueryable<F060701> GetDatasByStatus(string dcCode,string gupCode, string custCode)
		{
			var param = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode) {SqlDbType=System.Data.SqlDbType.VarChar},
								new SqlParameter("@p1", gupCode) {SqlDbType=System.Data.SqlDbType.VarChar},
								new SqlParameter("@p2", custCode) {SqlDbType=System.Data.SqlDbType.VarChar}
            };

			string sql = $@"SELECT * FROM F060701 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND STATUS = '0'";

			var result = SqlQuery<F060701>(sql, param.ToArray());

			return result;
		}
	}
}
