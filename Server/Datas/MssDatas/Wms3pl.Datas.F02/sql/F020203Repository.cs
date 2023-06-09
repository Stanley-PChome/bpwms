using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F020203Repository : RepositoryBase<F020203, Wms3plDbContext, F020203Repository>
	{

		public string LockF020203()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F020203';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}
	}
}
