using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140103Repository : RepositoryBase<F140103, Wms3plDbContext, F140103Repository>
	{
		public F140103Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F140103> GetDataInventoryDetail(string dcCode, string gupCode, string custCode, List<string> inventoryNos)
		{
			return _db.F140103s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && inventoryNos.Contains(x.INVENTORY_NO));
		}
	}
}
