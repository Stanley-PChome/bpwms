using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075108Repository : RepositoryBase<F075108, Wms3plDbContext, F075108Repository>
	{
		public string LockF075108()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075108';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075108ByKey(string dcCode, string docId)
		{
			var sql = @" DELETE F075108
                           WHERE DC_CODE =@p0 
                             AND DOC_ID =@p1 ";
			var param = new object[] { dcCode, docId };
			ExecuteSqlCommand(sql, param);
		}
	}
}
