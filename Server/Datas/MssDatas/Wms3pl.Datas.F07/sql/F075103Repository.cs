using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075103Repository : RepositoryBase<F075103, Wms3plDbContext, F075103Repository>
	{
		public string LockF075103()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075103';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075103ByKey(string custCode, string custOrdNo)
		{
			var sql = @" DELETE F075103 
                           WHERE CUST_CODE =@p0 
                             AND CUST_ORD_NO =@p1 ";
			var param = new object[] { custCode, custOrdNo };
			ExecuteSqlCommand(sql, param);
		}
	}
}
