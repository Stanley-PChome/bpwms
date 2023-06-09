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
	public partial class F195301Repository : RepositoryBase<F195301, Wms3plDbContext, F195301Repository>
	{
		public void Delete(decimal groupId, string funCode = "")
		{
			var parms = new List<object> { groupId };
			var sql = @"DELETE F195301 WHERE  GRP_ID = @p0";
			if (!string.IsNullOrEmpty(funCode))
			{
				sql += " AND FUN_CODE = @p" + parms.Count;
				parms.Add(funCode);
			}
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public void BulkDelete(decimal groupId, List<string> funCodeList)
		{
			var parameters = new List<object> { groupId };

			var sql = $@" DELETE FROM F195301
                    WHERE GRP_ID = @p0 ";

			sql += parameters.CombineSqlInParameters(" AND FUN_CODE", funCodeList);

			ExecuteSqlCommand(sql, parameters.ToArray());
		}
	}
}
