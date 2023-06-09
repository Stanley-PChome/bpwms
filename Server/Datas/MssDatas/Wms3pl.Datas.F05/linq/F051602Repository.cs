using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051602Repository : RepositoryBase<F051602, Wms3plDbContext, F051602Repository>
	{
		public F051602Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}
	}
}
