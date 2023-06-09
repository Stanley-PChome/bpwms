using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051503Repository : RepositoryBase<F051503, Wms3plDbContext, F051503Repository>
	{
		public F051503Repository(string connName, WmsTransaction wmsTransaction = null)
					 : base(connName, wmsTransaction)
		{
		}
	}
}
