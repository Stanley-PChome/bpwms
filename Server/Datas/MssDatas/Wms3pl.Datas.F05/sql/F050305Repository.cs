using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050305Repository : RepositoryBase<F050305, Wms3plDbContext, F050305Repository>
	{

		public IQueryable<F050305> GetDataByOrdNos(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode };

			int paramStartIndex = parameter.Count;
			var inSql = parameter.CombineSqlInParameters("ORD_NO", ordNos, ref paramStartIndex);

			var sql = @"
					SELECT *
					FROM F050305
					Where DC_CODE = @p0
                        And GUP_CODE = @p1
						And CUST_CODE = @p2
                        And STATUS = '2'
						And " + inSql;

			var data = SqlQuery<F050305>(sql, parameter.ToArray());

			return data;
		}

		public void UpdateProcFlag(Int64 id, string procFlag)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",procFlag){ SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p1",Current.Staff){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p2",Current.StaffName){SqlDbType = System.Data.SqlDbType.NVarChar},
				new SqlParameter("@p3",id){SqlDbType = System.Data.SqlDbType.BigInt},
        new SqlParameter("@p4", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
      };
			var sql = @" UPDATE F050305 with(rowlock,updlock) 
                   SET PROC_FLAG=@p0,TRANS_DATE = @p4,UPD_DATE = @p4,UPD_STAFF=@p1,UPD_NAME=@p2
                   WHERE ID = @p3 ";
			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
		}

    public IQueryable<F050305> GetDatasForExport_Sql(string dcCode, string gupCode, string custCode)
    {
      var param = new List<SqlParameter>
              {
                  new SqlParameter("@dcCode",     dcCode)   {SqlDbType=System.Data.SqlDbType.VarChar},
                  new SqlParameter("@gupCode",    gupCode)  {SqlDbType=System.Data.SqlDbType.VarChar},
                  new SqlParameter("@custCode",   custCode) {SqlDbType=System.Data.SqlDbType.VarChar}
              };
      string sql = $@"Select * FROM F050305 WHERE  DC_CODE=@dcCode  and  GUP_CODE = @gupCode  and CUST_CODE = @custCode
                      AND PROC_FLAG = N'0' AND (SOURCE_TYPE is null or SOURCE_TYPE = '') ORDER BY CRT_DATE";

      var result = SqlQuery<F050305>(sql, param.ToArray());
      return result;
    }


    public IQueryable<F050305> GetDatasFor_SourceType13(string dcCode, string gupCode, string custCode)
    {
      var param = new List<SqlParameter>
              {
                  new SqlParameter("@dcCode",     dcCode)   {SqlDbType=System.Data.SqlDbType.VarChar},
                  new SqlParameter("@gupCode",    gupCode)  {SqlDbType=System.Data.SqlDbType.VarChar},
                  new SqlParameter("@custCode",   custCode) {SqlDbType=System.Data.SqlDbType.VarChar}
              };
      string sql = $@"Select * FROM F050305 WHERE  DC_CODE=@dcCode  and  GUP_CODE = @gupCode  and CUST_CODE = @custCode
                      AND PROC_FLAG = N'0' AND  SOURCE_TYPE = '13' ORDER BY CRT_DATE";

      var result = SqlQuery<F050305>(sql, param.ToArray());
      return result;
    }
  }
}
