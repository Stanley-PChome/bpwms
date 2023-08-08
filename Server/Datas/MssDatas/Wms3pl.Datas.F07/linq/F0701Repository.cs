using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F0701Repository : RepositoryBase<F0701, Wms3plDbContext, F0701Repository>
	{
		public F0701Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F0701 GetDatasByF0701ContainerCode(string dcCode, string containerCode)
		{
			return _db.F0701s.Where(x => x.DC_CODE == dcCode && x.CONTAINER_CODE == containerCode && x.CONTAINER_TYPE == "0").FirstOrDefault();
		}

		public IQueryable<F0701> GetDataByContainerCodes(string warehouseId, string containerType, List<string> containerCodes)
		{
			return _db.F0701s.AsNoTracking().Where(x => x.WAREHOUSE_ID == warehouseId && x.CONTAINER_TYPE == containerType && containerCodes.Contains(x.CONTAINER_CODE));
		}
	}
}
