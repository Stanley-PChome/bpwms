using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075107Repository : RepositoryBase<F075107, Wms3plDbContext, F075107Repository>
	{
		public string LockF075107()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075107';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075107ByKey(string dcCode, string docId)
		{
			var sql = @" DELETE F075107
                           WHERE DC_CODE =@p0 
                             AND DOC_ID =@p1 ";
			var param = new object[] { dcCode, docId };
			ExecuteSqlCommand(sql, param);
		}
	}
}
