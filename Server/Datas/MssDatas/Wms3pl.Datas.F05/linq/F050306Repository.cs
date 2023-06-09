using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050306Repository : RepositoryBase<F050306, Wms3plDbContext, F050306Repository>
	{
		public F050306Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		}
	}
}
