using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080401Repository : RepositoryBase<F05080401, Wms3plDbContext, F05080401Repository>
	{
		public F05080401Repository(string connName, WmsTransaction wmsTransaction = null)
			 : base(connName, wmsTransaction)
		{
		}
	}
}
