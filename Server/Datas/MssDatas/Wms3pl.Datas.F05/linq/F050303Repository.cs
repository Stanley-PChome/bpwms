using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.F05
{
    public partial class F050303Repository : RepositoryBase<F050303, Wms3plDbContext, F050303Repository>
	{
		public F050303Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
