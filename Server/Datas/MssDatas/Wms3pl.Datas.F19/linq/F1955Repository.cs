using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1955Repository : RepositoryBase<F1955, Wms3plDbContext, F1955Repository>
	{
		public F1955Repository(string connName, WmsTransaction wmsTransaction = null)
	  : base(connName, wmsTransaction)
		{
		}
	}
}
