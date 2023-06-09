using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F05290501Repository : RepositoryBase<F05290501, Wms3plDbContext, F05290501Repository>
	{
		public F05290501Repository(string connName, WmsTransaction wmsTransaction = null)
					 : base(connName, wmsTransaction)
		{
		}
	}
}
