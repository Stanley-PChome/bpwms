using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060403Repository : RepositoryBase<F060403, Wms3plDbContext, F060403Repository>
	{
		public F060403Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}


		public IQueryable<F060403> GetDatasForAdjustExecute(string dcCode, string gupCode, string custCode, IQueryable<F060402> f060402s)
		{
			return _db.F060403s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			f060402s.Any(z => z.DOC_ID == x.DOC_ID &&
			z.WMS_NO == x.WMS_NO &&
			z.WAREHOUSE_ID == x.WAREHOUSE_ID));
		}
	}
}
