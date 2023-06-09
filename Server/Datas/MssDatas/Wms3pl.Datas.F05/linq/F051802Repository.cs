using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051802Repository : RepositoryBase<F051802, Wms3plDbContext, F051802Repository>
	{
		public F051802Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}
	}
}
