using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
	public partial class F050003Repository : RepositoryBase<F050003, Wms3plDbContext, F050003Repository>
	{
		public F050003Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}


		public IQueryable<SpecialItem> GetSpecialItems(List<decimal> ticketIds, string gupCode, string custCode, string dcCode)
		{
			var f050003Data = _db.F050003s.AsNoTracking().Where(x => x.BEGIN_DELV_DATE <= DateTime.Today &&
																															 x.END_DELV_DATE >= DateTime.Today &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.DC_CODE == dcCode &&
																															 ticketIds.Contains(x.TICKET_ID) &&
																															 x.STATUS != "9");

			var f05000301Data = _db.F05000301s.AsNoTracking().Where(x => f050003Data.Select(z => z.SEQ_NO).Contains(x.SEQ_NO));

			var result = from A in f050003Data
									 join B in f05000301Data
									 on A.SEQ_NO equals B.SEQ_NO
									 select new SpecialItem
									 {
										 TICKET_ID = A.TICKET_ID,
										 ITEM_CODE = B.ITEM_CODE
									 };

			return result;
		}

		public int GetSpecialItemCount(string itemCode, string gupCode, string custCode, string dcCode)
		{
			var f050003Data = _db.F050003s.AsNoTracking().Where(x => x.BEGIN_DELV_DATE <= DateTime.Today &&
																															 x.END_DELV_DATE >= DateTime.Today &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.DC_CODE == dcCode);

			var f05000301Data = _db.F05000301s.AsNoTracking().Where(x => x.ITEM_CODE == itemCode &&
																																	 f050003Data.Select(z => z.SEQ_NO).Contains(x.SEQ_NO));

			var result = from A in f050003Data
									 join B in f05000301Data
									 on A.SEQ_NO equals B.SEQ_NO
									 select B.SEQ_NO;

			return result.Count();
		}
	}
}
