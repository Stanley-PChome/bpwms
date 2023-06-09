using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075103Repository : RepositoryBase<F075103, Wms3plDbContext, F075103Repository>
	{
		public F075103Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}
	}
}
