using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191304Repository : RepositoryBase<F191304, Wms3plDbContext, F191304Repository>
	{
		public F191304Repository(string connName, WmsTransaction wmsTransaction = null)
						 : base(connName, wmsTransaction)
		{
		}

		public string LockF191304()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F191304';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}
	}
}
