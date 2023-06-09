using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.F54
{
	public partial class F540102Repository : RepositoryBase<F540102, Wms3plDbContext, F540102Repository>
	{
		public F540102Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{

		}
	}
}
