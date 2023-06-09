using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F052905Repository : RepositoryBase<F052905, Wms3plDbContext, F052905Repository>
	{
		public F052905Repository(string connName, WmsTransaction wmsTransaction = null)
					 : base(connName, wmsTransaction)
		{
		}
	}
}
