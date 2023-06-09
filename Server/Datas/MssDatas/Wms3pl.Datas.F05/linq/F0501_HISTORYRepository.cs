using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0501_HISTORYRepository : RepositoryBase<F0501_HISTORY, Wms3plDbContext, F0501_HISTORYRepository>
	{
		public F0501_HISTORYRepository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
