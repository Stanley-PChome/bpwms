using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0500Repository : RepositoryBase<F0500, Wms3plDbContext, F0500Repository>
	{
		public string LockF0500()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0500';";
			return SqlQuery<string>(sql).FirstOrDefault();

		}
	}
}
