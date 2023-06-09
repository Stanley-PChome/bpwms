using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060203Repository : RepositoryBase<F060203, Wms3plDbContext, F060203Repository>
    {
        public F060203Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F060203> GetDatasForExecute(List<string> docIds)
        {
            return _db.F060203s.AsNoTracking().Where(x => docIds.Contains(x.DOC_ID));
        }
    }
}
