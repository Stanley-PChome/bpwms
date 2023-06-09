using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060206Repository : RepositoryBase<F060206, Wms3plDbContext, F060206Repository>
	{
		public F060206Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060206> GetDatasForDocIds(List<string> docIds)
		{
			return _db.F060206s.AsNoTracking().Where(x => docIds.Contains(x.DOC_ID));
		}
	}
}
