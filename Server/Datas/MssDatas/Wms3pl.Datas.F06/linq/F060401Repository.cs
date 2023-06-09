using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060401Repository : RepositoryBase<F060401, Wms3plDbContext, F060401Repository>
	{
		public F060401Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F060401 GetDataByCancel(string dcCode, string gupCode, string custCode, string wmsNo)
		{
			var statusList = new List<string> { "0", "T", "F" };
			return _db.F060401s.AsNoTracking().Where(o =>
				o.DC_CODE == dcCode &&
				o.GUP_CODE == gupCode &&
				o.CUST_CODE == custCode &&
				o.WMS_NO == wmsNo &&
				o.CMD_TYPE == "1" &&
				statusList.Contains(o.STATUS)).FirstOrDefault();
		}

		public IQueryable<F060401> GetDatas(string dcCode, string gupCode, string custCode, string cmdType, List<string> statusList, int midApiRelmt)
		{
			return _db.F060401s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.CMD_TYPE == cmdType &&
			statusList.Contains(x.STATUS) &&
			x.RESENT_CNT < midApiRelmt).OrderBy(x => x.CRT_DATE);
		}
	}
}
