using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F055004Repository : RepositoryBase<F055004, Wms3plDbContext, F055004Repository>
	{

		public IQueryable<F055004> GetDatasByOrdNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var sqlParameter = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)  { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode)  { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p3", ordNo)  { SqlDbType = SqlDbType.VarChar },
			};
			var sql = @"SELECT * FROM F055004
                   WHERE DC_CODE = @p0
						   AND GUP_CODE = @p1
						   AND CUST_CODE = @p2
						   AND ORD_NO = @p3 ";

			return SqlQuery<F055004>(sql, sqlParameter.ToArray());
		}
	}
}