using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F19000103Repository : RepositoryBase<F19000103, Wms3plDbContext, F19000103Repository>
	{
		public F19000103Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 取得存在單據類型與單據類別的所有里程碑
		/// </summary>
		/// <returns></returns>
		public IQueryable<F19000103WithF000906> GetExistsTicketAllMilestoneNo()
		{
            var query = _db.F000901s
                .Join(_db.F000906s, a => a.ORD_TYPE, b => b.TICKET_TYPE, (a, b) => new { a, b })
                .Join(_db.F19000103s, ab => new { ab.b.TICKET_TYPE, ab.b.TICKET_CLASS }, c => new { c.TICKET_TYPE,c.TICKET_CLASS }, (ab, c) => new { ab, c });
            return query.Select(x => new F19000103WithF000906
            {
                TICKET_CLASS_NAME = x.ab.b.TICKET_CLASS_NAME,
                TICKET_TYPE = x.c.TICKET_TYPE,
                TICKET_CLASS = x.c.TICKET_CLASS,
                MILESTONE_NO_A = x.c.MILESTONE_NO_A,
                MILESTONE_NO_B = x.c.MILESTONE_NO_B,
                MILESTONE_NO_C = x.c.MILESTONE_NO_C,
                MILESTONE_NO_D = x.c.MILESTONE_NO_D,
                MILESTONE_NO_E = x.c.MILESTONE_NO_E,
                MILESTONE_NO_F = x.c.MILESTONE_NO_F,
                MILESTONE_NO_G = x.c.MILESTONE_NO_G,
                MILESTONE_NO_H = x.c.MILESTONE_NO_H,
                MILESTONE_NO_I = x.c.MILESTONE_NO_I,
                MILESTONE_NO_J = x.c.MILESTONE_NO_J,
                MILESTONE_NO_K = x.c.MILESTONE_NO_K,
                MILESTONE_NO_L = x.c.MILESTONE_NO_L,
                MILESTONE_NO_M = x.c.MILESTONE_NO_M,
                MILESTONE_NO_N = x.c.MILESTONE_NO_N,
                CRT_STAFF = x.c.CRT_STAFF,
                CRT_NAME = x.c.CRT_NAME,
                CRT_DATE = x.c.CRT_DATE,
                UPD_STAFF = x.c.UPD_STAFF,
                UPD_NAME = x.c.UPD_NAME,
                UPD_DATE = x.c.UPD_DATE
            });
        }
	}
}
