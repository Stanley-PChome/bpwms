using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020201Repository : RepositoryBase<F020201, Wms3plDbContext, F020201Repository>
    {
        public F020201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        public IQueryable<F020201> GetDatas(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            return _db.F020201s
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => x.GUP_CODE == gupCode)
                .Where(x => x.CUST_CODE == custCode)
                .Where(x => x.PURCHASE_NO == purchaseNo)
                .Select(x => x);
        }

    }
}
