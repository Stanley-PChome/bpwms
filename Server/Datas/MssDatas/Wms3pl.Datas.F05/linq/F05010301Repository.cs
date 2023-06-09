using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05010301Repository : RepositoryBase<F05010301, Wms3plDbContext, F05010301Repository>
	{
		public F05010301Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
