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
	public partial class F190106Repository : RepositoryBase<F190106, Wms3plDbContext, F190106Repository>
	{
		public void DeleteF190106ByIds(List<int> ids)
		{
			var parameters = new List<object> { };
			var sql = $"DELETE F190106 WHERE {parameters.CombineSqlInParameters(" ID ", ids)}";
			ExecuteSqlCommand(sql, parameters.ToArray());
		}
	}
}
