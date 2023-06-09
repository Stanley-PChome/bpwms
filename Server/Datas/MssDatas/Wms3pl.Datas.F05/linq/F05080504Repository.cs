using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080504Repository : RepositoryBase<F05080504, Wms3plDbContext, F05080504Repository>
	{
		public F05080504Repository(string connName, WmsTransaction wmsTransaction = null)
			 : base(connName, wmsTransaction)
		{
		}
	}
}
