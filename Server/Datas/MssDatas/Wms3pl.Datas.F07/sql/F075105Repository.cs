using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075105Repository : RepositoryBase<F075105, Wms3plDbContext, F075105Repository>
	{
		public string LockF075105()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075105';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075105ByKey(string dcCode, string docId)
		{
			var sql = @" DELETE F075105
                           WHERE DC_CODE =@p0 
                             AND DOC_ID =@p1 ";
			var param = new object[] { dcCode, docId };
			ExecuteSqlCommand(sql, param);
		}
	}
}
