using System;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Wms3pl.Datas.F06
{
  public partial class F060209Repository : RepositoryBase<F060209, Wms3plDbContext, F060209Repository>
  {
		public IQueryable<F060209> GetDatas(string dcCode, string gupCode, string custCode,string procFlag,int topRecord)
		{
			var param = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", procFlag)
						};
			string sql = $@"SELECT TOP({topRecord}) * FROM F060209 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND PROC_FLAG = @p3 ORDER BY CRT_DATE ";

			return SqlQuery<F060209>(sql, param.ToArray());
		}
	}
}
