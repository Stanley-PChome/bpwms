using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051803Repository : RepositoryBase<F051803, Wms3plDbContext, F051803Repository>
	{
		public F051803Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}
	}
}
