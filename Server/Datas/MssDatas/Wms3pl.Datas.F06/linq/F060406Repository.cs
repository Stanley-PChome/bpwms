using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060406Repository : RepositoryBase<F060406, Wms3plDbContext, F060406Repository>
	{
		public F060406Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060406> GetDatasByInventoryAdjustConfirm(string dcCode, string gupCode, string custCode, List<string> docIds)
		{
			return _db.F060406s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			docIds.Contains(x.DOC_ID));
		}
	}
}
