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
	public partial class F19000101Repository : RepositoryBase<F19000101, Wms3plDbContext, F19000101Repository>
	{
		public F19000101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F19000101> GetMilestones(decimal ticketId)
		{
            var query = _db.F19000101s.Where(x =>
                    x.TICKET_ID == ticketId);
            return query.Select(x => new F19000101
            {
                MILESTONE_ID = x.MILESTONE_ID,
                TICKET_ID = x.TICKET_ID,
                MILESTONE_NO = x.MILESTONE_NO,
                SORT_NO = x.SORT_NO,
                CRT_STAFF = x.CRT_STAFF,
                CRT_NAME = x.CRT_NAME,
                CRT_DATE = x.CRT_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_NAME = x.UPD_NAME,
                UPD_DATE = x.UPD_DATE,
            });
        }
	}
}
