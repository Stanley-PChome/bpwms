using Wms3pl.DBCore;
using System;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Wms3pl.Datas.F06
{
  public partial class F06020902Repository : RepositoryBase<F06020902, Wms3plDbContext, F06020902Repository>
  {
		public IQueryable<F06020902> GetDatas(List<string> docIds)
		{
			var param = new List<object>();

			string sql = $@"SELECT * FROM F06020902 WHERE ";
			if (docIds.Any())
				sql += param.CombineNotNullOrEmptySqlInParameters(" DOC_ID", docIds);
			else
				sql += " 1=0 ";
			return SqlQuery<F06020902>(sql, param.ToArray());
		}
	}
}
