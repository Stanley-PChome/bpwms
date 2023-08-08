using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
	public partial class F010301_HISTORYRepository : RepositoryBase<F010301_HISTORY, Wms3plDbContext, F010301_HISTORYRepository>
	{
		public F010301_HISTORYRepository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

	}
}
