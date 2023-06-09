using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060207Repository : RepositoryBase<F060207, Wms3plDbContext, F060207Repository>
	{
		public F060207Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}
	}
}
