using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194715Repository : RepositoryBase<F194715, Wms3plDbContext, F194715Repository>
    {
        public F194715Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F194715> GetSettings(string allId, string customerId = null)
        {
            var result = _db.F194715s.Where(x => x.ALL_ID == allId);
            if (!string.IsNullOrWhiteSpace(customerId))
            {
                result = result.Where(x => x.CUSTOMER_ID == customerId);
            }
#if DEBUG
            result = result.Where(x => x.ISTEST == "1");
#else
             result = result.Where(x => x.ISTEST == "0");
#endif
            return result;
        }
    }
}
