using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075101Repository : RepositoryBase<F075101, Wms3plDbContext, F075101Repository>
	{
		public F075101Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}
	}
}
