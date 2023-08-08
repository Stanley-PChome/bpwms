using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F02050401Repository : RepositoryBase<F02050401, Wms3plDbContext, F02050401Repository>
  {
    public IQueryable<UnnormalItemRecheckLog> GetUnnormalItemRecheckLog(long F020504_ID)
    {
      var para = new List<SqlParameter>() { new SqlParameter("@p0", F020504_ID) { SqlDbType = SqlDbType.BigInt } };
      var sql = @"
SELECT
	A.PROC_DESC,
	A.PROC_NAME,
	A.PROC_TIME,
	B.CAUSE RECHECK_CAUSE,
	A.MEMO
FROM
	F02050401 A
LEFT JOIN F1951 B ON
	A.RECHECK_CAUSE = B.UCC_CODE
	AND B.UCT_ID = 'IQ'
WHERE A.F020504_ID = @p0";

      sql = $@"
SELECT 
  ROW_NUMBER()OVER(ORDER BY aa.PROC_TIME) AS ROW_NUM,
  aa.*
FROM
({sql}) aa";

      var result = SqlQuery<UnnormalItemRecheckLog>(sql, para.ToArray());
      return result;
    }

  }
}
