using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1956Repository : RepositoryBase<F1956, Wms3plDbContext, F1956Repository>
	{
		public F1956Repository(string connName, WmsTransaction wmsTransaction = null)
	  : base(connName, wmsTransaction)
		{
		}
	}
}
