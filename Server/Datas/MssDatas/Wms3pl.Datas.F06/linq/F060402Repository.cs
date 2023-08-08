using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060402Repository : RepositoryBase<F060402, Wms3plDbContext, F060402Repository>
	{
		public F060402Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060402> GetDatasForAdjustExecute(string dcCode, string gupCode, string custCode, List<string> wmsNos)
		{
			return _db.F060402s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			wmsNos.Contains(x.WMS_NO) &&
			x.STATUS == "2");
		}
	}
}
