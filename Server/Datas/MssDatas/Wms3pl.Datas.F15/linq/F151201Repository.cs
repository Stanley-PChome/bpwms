using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
	public partial class F151201Repository : RepositoryBase<F151201, Wms3plDbContext, F151201Repository>
	{
		public F151201Repository(string connName, WmsTransaction wmsTransaction = null)
		   : base(connName, wmsTransaction)
		{
		}
	}
}
