using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050306_HISTORYRepository : RepositoryBase<F050306_HISTORY, Wms3plDbContext, F050306_HISTORYRepository>
	{
		public F050306_HISTORYRepository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		}
	}
}
