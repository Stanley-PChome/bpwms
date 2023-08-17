using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F1909Repository : RepositoryBase<F1909, Wms3plDbContext, F1909Repository>
  {

    public F1909 GetF1909(string gupCode, string custCode)
    {
      var sqlParameters = new List<SqlParameter>()
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      string sql = @"
				SELECT TOP(1) * FROM F1909
				 WHERE GUP_CODE = @p0
           AND CUST_CODE = @p1
           ";

      var result = SqlQuery<F1909>(sql, sqlParameters.ToArray()).FirstOrDefault();
      return result;
    }

		public IQueryable<F1909EX> GetCustList()
		{
			var sql = @" SELECT ROW_NUMBER() OVER(ORDER BY CUST_CODE) AS ROWNUM, GUP_CODE,CUST_CODE,CUST_NAME, '' LOC_CODE
                     FROM F1909  ";
			return SqlQuery<F1909EX>(sql);
		}
  }

}
