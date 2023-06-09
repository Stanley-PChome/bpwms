using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051801Repository : RepositoryBase<F051801, Wms3plDbContext, F051801Repository>
	{
		public F051801Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}
	}
}
