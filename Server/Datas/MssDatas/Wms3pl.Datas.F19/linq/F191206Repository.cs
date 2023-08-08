using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191206Repository : RepositoryBase<F191206, Wms3plDbContext, F191206Repository>
	{
		public F191206Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}


	}
}
