using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075106Repository : RepositoryBase<F075106, Wms3plDbContext, F075106Repository>
	{
		public F075106Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}
	}
}
