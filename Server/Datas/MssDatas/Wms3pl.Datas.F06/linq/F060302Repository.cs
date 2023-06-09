using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060302Repository : RepositoryBase<F060302, Wms3plDbContext, F060302Repository>
	{
		public F060302Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060302> GetDatasByF07001s(List<F0701> f0701s, List<string> statusList)
		{
			return _db.F060302s.AsNoTracking().Where(x =>
			f0701s.Any(z => z.DC_CODE == x.DC_CODE &&
			z.CUST_CODE == x.CUST_CODE &&
			z.WAREHOUSE_ID == x.WAREHOUSE_ID &&
			z.CONTAINER_CODE == x.CONTAINER_CODE) &&
			statusList.Contains(x.STATUS));
		}

		public IQueryable<F060302> GetWcsExecuteDatas(string dcCode, string custCode, List<string> statusList, int midApiRelmt)
		{
			var result = _db.F060302s.Where(x => 
			x.DC_CODE == dcCode &&
			x.CUST_CODE == custCode &&
			statusList.Contains(x.STATUS) &&
			x.RESENT_CNT < midApiRelmt);
			return result;
		}

		public IQueryable<F060302>  GetDatasByInWarehouseReceipt(string dcCode, string custCode, string warehouseId, string containerCode, List<string> statusList)
		{
			return _db.F060302s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.CUST_CODE == custCode &&
			x.WAREHOUSE_ID == warehouseId &&
			x.CONTAINER_CODE == containerCode &&
			statusList.Contains(x.STATUS));
		}
	}
}
