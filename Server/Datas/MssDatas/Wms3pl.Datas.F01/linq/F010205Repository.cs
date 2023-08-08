using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
	public partial class F010205Repository : RepositoryBase<F010205, Wms3plDbContext, F010205Repository>
	{
		public F010205Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F010205> GetDatasByStockNos(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			return _db.F010205s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			stockNos.Contains(x.STOCK_NO));
		}
	}
}
