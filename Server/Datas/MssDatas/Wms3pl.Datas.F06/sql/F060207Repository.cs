using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Wms3pl.Datas.F06
{
    public partial class F060207Repository
    {
        public IQueryable<F060207> GetDatasByNoProcess(string DcCode, string GupCode, string CustCode)
        {
            var sql = @"SELECT * FROM F060207 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND STATUS = '0'";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=DcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=GupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=CustCode}
            };
            return SqlQuery<F060207>(sql, para.ToArray());
        }
    
		public string LockF060207()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F060207';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public long GetF060207NextId()
		{
			var sql = @"SELECT NEXT VALUE FOR SEQ_F060207_ID";

			return SqlQuery<long>(sql).Single();
		}
	}
}
