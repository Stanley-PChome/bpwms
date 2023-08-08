using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

using System.Data.SqlClient;
using System.Collections.Generic;

namespace Wms3pl.Datas.F07
{
	public partial class F075101Repository : RepositoryBase<F075101, Wms3plDbContext, F075101Repository>
	{
		public string LockF075101()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075101';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075101ByKey(string custCode, string custOrdNo)
		{
			var sql = @" DELETE F075101 
                           WHERE CUST_CODE =@p0 
                             AND CUST_ORD_NO =@p1 ";
			var param = new object[] { custCode, custOrdNo };
			ExecuteSqlCommand(sql, param);
		}

    public F075101 GetF075101Data(string custCode, string custOrdNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@custCode",     custCode)   {SqlDbType=System.Data.SqlDbType.VarChar},
        new SqlParameter("@custOrdNo",    custOrdNo)  {SqlDbType=System.Data.SqlDbType.VarChar},
      };
      string sql = $@"Select * FROM F075101 WHERE  cust_Code=@custCode  and  cust_Ord_No = @custOrdNo ";

      var result = SqlQuery<F075101>(sql, param.ToArray()).FirstOrDefault();
      return result;
    }
  }
}
