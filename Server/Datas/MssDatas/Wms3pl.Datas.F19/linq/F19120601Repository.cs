using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F19120601Repository : RepositoryBase<F19120601, Wms3plDbContext, F19120601Repository>
	{
		public F19120601Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}

	}
}
