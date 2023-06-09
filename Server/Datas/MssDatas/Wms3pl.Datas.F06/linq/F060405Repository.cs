using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060405Repository : RepositoryBase<F060405, Wms3plDbContext, F060405Repository>
	{
		public F060405Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060405> GetDatasForExecute(string dcCode, string gupCode, string custCode)
		{
			return _db.F060405s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS == "0");
		}
	}
}
