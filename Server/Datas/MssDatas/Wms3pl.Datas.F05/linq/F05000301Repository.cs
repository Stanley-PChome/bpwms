using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F05000301Repository : RepositoryBase<F05000301, Wms3plDbContext, F05000301Repository>
	{
		public F05000301Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

	}
}
