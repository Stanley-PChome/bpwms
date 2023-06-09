using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060202Repository : RepositoryBase<F060202, Wms3plDbContext, F060202Repository>
    {
        public F060202Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F060202> GetDatas(string dcCode, string gupCode, string custCode, List<string> docIds)
        {
            return _db.F060202s.AsNoTracking().Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            docIds.Contains(x.DOC_ID));
        }

        public IQueryable<F060202> GetDatasForExecute(string dcCode, string gupCode, string custCode)
        {
            return _db.F060202s.Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            x.STATUS == "0");
        }
    }
}
