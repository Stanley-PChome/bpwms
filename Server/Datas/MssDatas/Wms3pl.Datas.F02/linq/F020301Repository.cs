using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F020301Repository : RepositoryBase<F020301, Wms3plDbContext, F020301Repository>
	{
		public F020301Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F020301> GetDatasByShopNos(string dcCode, string gupCode, string custCode, List<string> stockNo)
		{
			return _db.F020301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS != "9" &&
			stockNo.Contains(x.PURCHASE_NO));
		}


	}
}
