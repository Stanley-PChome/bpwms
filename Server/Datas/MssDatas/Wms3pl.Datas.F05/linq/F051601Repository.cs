using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051601Repository : RepositoryBase<F051601, Wms3plDbContext, F051601Repository>
	{
		public F051601Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}
	}
}
