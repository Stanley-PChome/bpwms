using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F05030201Repository : RepositoryBase<F05030201, Wms3plDbContext, F05030201Repository>
    {
        public F05030201Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F05030201Ex> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         wmsOrdNos.Contains(x.WMS_ORD_NO));


            var f05030201Data = _db.F05030201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         f05030101Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var result = from A in f05030201Data
                         join B in f05030101Data
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO }
                         select new F05030201Ex
                         {
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CUST_CODE = A.CUST_CODE,
                             ORD_NO = A.ORD_NO,
                             ORD_SEQ = A.ORD_SEQ,
                             ITEM_CODE = A.ITEM_CODE,
                             BOM_ITEM_CODE = A.BOM_ITEM_CODE,
                             BOM_QTY = A.BOM_QTY,
                             ORD_QTY = A.ORD_QTY,
                             SERIAL_NO = A.SERIAL_NO,
                             WMS_ORD_NO = B.WMS_ORD_NO
                         };

            return result;
        }
    }
}
