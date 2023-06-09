using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F195601Repository : RepositoryBase<F195601, Wms3plDbContext, F195601Repository>
	{
		public F195601Repository(string connName, WmsTransaction wmsTransaction = null)
	  : base(connName, wmsTransaction)
		{
		}
	}
}
