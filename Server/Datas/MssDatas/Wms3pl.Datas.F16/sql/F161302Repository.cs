using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161302Repository : RepositoryBase<F161302, Wms3plDbContext, F161302Repository>
	{
		public IQueryable<F161302> GetCheckExist(string dcCode, string returnNo, string pastNo, string barCode)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", returnNo),
				new SqlParameter("@p2", pastNo)
			};
			var sql = @"SELECT *
									  FROM F161302
									 WHERE DC_CODE = @p0
										 AND RETURN_NO = @p1
										 AND PAST_NO = @p2  ";

			if (!string.IsNullOrEmpty(barCode))
			{
				sql += string.Format("AND EAN_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}",parameters.Count), barCode));
			}

			var result = SqlQuery<F161302>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}
	}
}
