using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F076108Repository : RepositoryBase<F076108, Wms3plDbContext, F076108Repository>
	{
		public F076108Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
