using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F07010401Repository : RepositoryBase<F07010401, Wms3plDbContext, F07010401Repository>
	{

    public IQueryable<F07010401> GetDatasByF070104Ids(List<long> f070104Ids)
    {
      var param = new List<SqlParameter>();
      var sql = @"SELECT * FROM F07010401 WHERE";
      sql += param.CombineSqlInParameters(" F070104_ID", f070104Ids, SqlDbType.BigInt);

      return SqlQuery<F07010401>(sql, param.ToArray());
    }

	}
}
