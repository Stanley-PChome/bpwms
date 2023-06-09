using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075106Repository : RepositoryBase<F075106, Wms3plDbContext, F075106Repository>
	{
		public string LockF075106()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075106';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075106ByKey(string dcCode, string docId)
		{
			var sql = @" DELETE F075106
                           WHERE DC_CODE =@p0 
                             AND DOC_ID =@p1 ";
			var param = new object[] { dcCode, docId };
			ExecuteSqlCommand(sql, param);
		}
	}
}
