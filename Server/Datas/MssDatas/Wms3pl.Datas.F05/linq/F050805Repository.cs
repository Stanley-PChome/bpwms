using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050805Repository : RepositoryBase<F050805, Wms3plDbContext, F050805Repository>
	{
		public F050805Repository(string connName, WmsTransaction wmsTransaction = null)
			 : base(connName, wmsTransaction)
		{
		}
	}
}
