using System;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace Wms3pl.Datas.F06
{
  public partial class F06020901Repository : RepositoryBase<F06020901, Wms3plDbContext, F06020901Repository>
  {
		public IQueryable<F06020901> GetDatas(List<string> docIds)
		{
			var param = new List<object>();
				
			string sql = $@"SELECT * FROM F06020901 WHERE  ";
			if (docIds.Any())
				sql += param.CombineNotNullOrEmptySqlInParameters(" DOC_ID", docIds);
			else
				sql += "  1=0 ";
			return SqlQuery<F06020901>(sql, param.ToArray());
		}
	}
}
