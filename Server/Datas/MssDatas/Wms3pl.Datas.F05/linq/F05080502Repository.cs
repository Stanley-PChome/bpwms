using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080502Repository : RepositoryBase<F05080502, Wms3plDbContext, F05080502Repository>
	{
		public F05080502Repository(string connName, WmsTransaction wmsTransaction = null)
			 : base(connName, wmsTransaction)
		{
		}
	}
}
