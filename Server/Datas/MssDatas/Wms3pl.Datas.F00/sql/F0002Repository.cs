using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
	public partial class F0002Repository : RepositoryBase<F0002, Wms3plDbContext, F0002Repository>
	{
		public IQueryable<F0002> getLogisticList(string dcCode)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
			};
			var sql = @"SELECT * FROM F0002 WHERE DC_CODE = @p0 ";

			return SqlQuery<F0002>(sql, param.ToArray());
		}

		public void DeleteF0002(string dcCode, string logisticCode)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = logisticCode },
			};
			var sql = @"DELETE F0002 FROM F0002 WHERE DC_CODE = @p0 AND LOGISTIC_COLDE = @p1 ";

			ExecuteSqlCommand(sql, param.ToArray());
		}
	}
}
