using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F077101Repository
  {
    public F077101 GetData(string dcCode, string workType, long refId, string status)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode }, 
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = workType }, 
        new SqlParameter("@p2", SqlDbType.BigInt) { Value = refId }, 
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = status }
      };

      var sql = @"SELECT TOP 1 * FROM F077101 WHERE DC_CODE=@p0 AND WORK_TYPE=@p1 AND REF_ID=@p2 AND STATUS=@p3 ORDER BY CRT_DATE DESC";
      return SqlQuery<F077101>(sql, para.ToArray()).FirstOrDefault();
    }

  }
}
