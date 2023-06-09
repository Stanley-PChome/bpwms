using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051504Repository : RepositoryBase<F051504, Wms3plDbContext, F051504Repository>
	{
		public F051504Repository(string connName, WmsTransaction wmsTransaction = null)
					 : base(connName, wmsTransaction)
		{
		}
	}
}
