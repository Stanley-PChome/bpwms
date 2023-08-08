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
	public partial class F190002Repository : RepositoryBase<F190002, Wms3plDbContext, F190002Repository>
	{
		public F190002Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F190002> GetDatas(List<decimal> ticketIds)
		{
            var query = _db.F190002s.Where(x => ticketIds.Contains(x.TICKET_ID));
            return query;
        }
    }
}
