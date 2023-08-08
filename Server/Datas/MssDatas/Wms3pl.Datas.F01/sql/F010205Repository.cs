using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F01
{
	public partial class F010205Repository : RepositoryBase<F010205, Wms3plDbContext, F010205Repository>
  {
    public bool IsExistRecord(string dcCode, string gupCode, string custCode, string stockNo, string status)
    {
      var sqlParameter = new List<SqlParameter>()
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", stockNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", status) { SqlDbType = SqlDbType.Char },
      };
      var sql = @"
                SELECT TOP(1) 1
	                FROM F010205
                 WHERE DC_CODE = @p0
                   AND GUP_CODE = @p1
                   AND CUST_CODE = @p2
                   AND STOCK_NO = @p3
                   AND STATUS = @p4
                ";

      return SqlQuery<int>(sql, sqlParameter.ToArray()).Any();
    }
  }
}
