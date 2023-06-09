using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050006Repository : RepositoryBase<F050006, Wms3plDbContext, F050006Repository>
	{
		public F050006Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050006> GetAllDatas()
		{
			var result = _db.F050006s;
			return result;
		}
	}
}
