using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190106Repository : RepositoryBase<F190106, Wms3plDbContext, F190106Repository>
	{
		public F190106Repository(string connName, WmsTransaction wmsTransaction = null)
						: base(connName, wmsTransaction)
		{
		}
	}
}
