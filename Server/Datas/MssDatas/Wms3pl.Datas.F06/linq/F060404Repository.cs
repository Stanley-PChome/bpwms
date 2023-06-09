using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060404Repository : RepositoryBase<F060404, Wms3plDbContext, F060404Repository>
	{
		public F060404Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060404> GetDatas(string dcCode, string gupCode, string custCode, string cmdType, List<string> statusList, int midApiRelmt)
		{
			return _db.F060404s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.CMD_TYPE == cmdType &&
			statusList.Contains(x.STATUS) &&
			x.RESENT_CNT < midApiRelmt).OrderBy(x => x.CRT_DATE);
		}

		public IQueryable<F060404> GetDatasByInventoryAdjustConfirm(string dcCode, string gupCode, string custCode, List<string> docIds)
		{
			return _db.F060404s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.STATUS == "2" &&
			docIds.Contains(x.DOC_ID));
		}
	}
}
