using Wms3pl.Datas.F19;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191001Repository : RepositoryBase<F191001, Wms3plDbContext, F191001Repository>
	{
		public F191001Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

	}
}
