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
    public partial class F1953Repository : RepositoryBase<F1953, Wms3plDbContext, F1953Repository>
    {
		public IQueryable<F1953> GetF1953DataByGrpId(List<string> userGroups)
		{
			var sql = @"SELECT * FROM F1953 WHERE 1=1";
			var paramList = new List<object>();
			if (userGroups.Any())
			{
				sql += paramList.CombineSqlInParameters(" AND GRP_ID", userGroups);
			}
			var result =  SqlQuery<F1953>(sql, paramList.ToArray());
			return result;
		}
	}
}
