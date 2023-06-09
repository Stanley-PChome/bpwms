using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F0500Repository : RepositoryBase<F0500, Wms3plDbContext, F0500Repository>
	{
		public F0500Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F0500> GetDatas()
		{
            var result = _db.F0500s;
            return result;
		}
	}
}
