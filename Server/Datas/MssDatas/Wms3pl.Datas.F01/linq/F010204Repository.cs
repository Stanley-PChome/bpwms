using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010204Repository : RepositoryBase<F010204, Wms3plDbContext, F010204Repository>
    {
        public F010204Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F010204> GetDatasForF010202s(string dcCode, string gupCode, string custCode, List<F010202> f010202s)
        {
            return _db.F010204s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           f010202s.Any(z => x.STOCK_NO == z.STOCK_NO &&
                                           x.STOCK_SEQ == z.STOCK_SEQ &&
                                           x.ITEM_CODE == z.ITEM_CODE));
        }

        public IQueryable<F010204> GetDatasForF020201s(string dcCode, string gupCode, string custCode, List<F020201> f020201s)
        {
            return _db.F010204s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           f020201s.Any(z => x.STOCK_NO == z.PURCHASE_NO &&
                                                             x.STOCK_SEQ == Convert.ToInt32(z.PURCHASE_SEQ) &&
                                                             x.ITEM_CODE == z.ITEM_CODE));
        }

        public IQueryable<F010204> GetDatasForSeqs(string dcCode, string gupCode, string custCode, string stockNo, List<int> seqs)
        {
            return _db.F010204s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           x.STOCK_NO == stockNo &&
                                           seqs.Contains(x.STOCK_SEQ));
        }

        public IQueryable<F010204> GetDatasForF020202s(string dcCode, string gupCode, string custCode, List<F020202> f020202s)
        {
            return _db.F010204s.Where(x => x.DC_CODE == dcCode &&
                                           x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           f020202s.Any(z => z.STOCK_NO == x.STOCK_NO && z.STOCK_SEQ == x.STOCK_SEQ));
        }
    }
}
