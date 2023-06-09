using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F15;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020202Repository : RepositoryBase<F020202, Wms3plDbContext, F020202Repository>
    {
        public F020202Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F020202> GetDatasForF151002s(string dcCode, string gupCode, string custCode, List<string> status, List<F151002> f151002s)
        {
            return _db.F020202s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           status.Contains(x.STATUS) &&
                                           f151002s.Any(z => z.ALLOCATION_NO == x.ALLOCATION_NO &&
                                                             z.ALLOCATION_SEQ == x.ALLOCATION_SEQ));
        }
    }
}
