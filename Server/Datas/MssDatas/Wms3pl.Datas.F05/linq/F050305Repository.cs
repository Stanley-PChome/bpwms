using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050305Repository : RepositoryBase<F050305, Wms3plDbContext, F050305Repository>
	{
		public F050305Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050305> GetDatasByF050305s(List<F050305> f050305s, string status)
		{
			return _db.F050305s.AsNoTracking().Where(x => f050305s.Any(z => z.DC_CODE == x.DC_CODE &&
			 z.GUP_CODE == x.GUP_CODE &&
			 z.CUST_CODE == x.CUST_CODE &&
			 z.ORD_NO == x.ORD_NO &&
			 x.STATUS == status));
		}

		public IQueryable<F050305> GetDatasForExport(string dcCode, string gupCode, string custCode)
		{
			return _db.F050305s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.PROC_FLAG == "0" &&
		 string.IsNullOrWhiteSpace(x.SOURCE_TYPE))
     .OrderBy(x=>x.CRT_DATE);
		}

		public IQueryable<F050305> GetDatas(string dcCode, string gupCode, string custCode, List<string> ordNos, string status)
		{
			return _db.F050305s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS == status &&
			ordNos.Contains(x.ORD_NO));
		}
	}
}
