using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190105Repository : RepositoryBase<F190105, Wms3plDbContext, F190105Repository>
	{
		public F190105Repository(string connName, WmsTransaction wmsTransaction = null)
						: base(connName, wmsTransaction)
		{
		}

	}
}
