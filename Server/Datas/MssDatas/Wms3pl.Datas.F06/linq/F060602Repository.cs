using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060602Repository : RepositoryBase<F060602, Wms3plDbContext, F060602Repository>
	{
		public F060602Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060602> GetData(List<long> ids)
		{
			return _db.F060602s.AsNoTracking().Where(x => ids.Contains(x.F060601_ID));
		}
	}
}
