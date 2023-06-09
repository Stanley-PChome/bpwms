using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
	public partial class F0010Repository : RepositoryBase<F0010, Wms3plDbContext, F0010Repository>
	{
		public F0010Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<DcWmsNoStatusItem> GetReturnNoHelpByDc(string dcCode, DateTime returnDate)
		{
			var query = _db.F0010s
					.Join(_db.F161201s, a => a.DC_CODE, b => b.DC_CODE, (a, b) => new { a, b })
					.Join(_db.F000903s, ab => ab.b.ORD_PROP, C => C.ORD_PROP, (R, C) => new { R, C })
					.Where(x => x.R.a.HELP_TYPE == "04")
					.Where(x => x.R.a.DC_CODE == "04")
					.Where(x => x.R.a.CRT_DATE.Date >= returnDate.Date)
					.Where(x => x.R.a.CRT_DATE.Date <= returnDate.AddDays(1).Date)
					.Where(x => x.R.a.STATUS == "0")
					.Select(x => new DcWmsNoStatusItem()
					{
						WMS_NO = x.R.b.RETURN_NO,
						MEMO = x.C.ORD_PROP_NAME,
						STAFF_NAME = x.R.a.CRT_STAFF + x.R.a.CRT_NAME,
						START_DATE = x.R.a.CRT_DATE,
					})
					.ToList();
			for (int i = 0; i < query.Count(); i++)
				query[i].ROWNUM = i + 1;
			return query.AsQueryable();
		}
	}
}