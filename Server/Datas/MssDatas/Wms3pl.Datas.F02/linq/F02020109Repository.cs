using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
   public partial class F02020109Repository : RepositoryBase<F02020109, Wms3plDbContext, F02020109Repository>
    {
        public F02020109Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F02020109Data> GetF02020109Datas(string dcCode, string gupCode, string custCode, string stockNo, int stockSeq)
        {
            var f02020109s = _db.F02020109s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                             && x.GUP_CODE == gupCode
                                             && x.CUST_CODE == custCode
                                             && x.STOCK_NO == stockNo
                                             && x.STOCK_SEQ == stockSeq
                                             && x.RT_NO == null);
            var f1951s = _db.F1951s.AsNoTracking().Where(x => x.UCT_ID == "RT");
            var result = (from A in f02020109s
                          join B in f1951s on A.UCC_CODE equals B.UCC_CODE into g
                          from C in g.DefaultIfEmpty()
                          select new F02020109Data
                          {
                              ID = A.ID,
                              DC_CODE = A.DC_CODE,
                              GUP_CODE = A.GUP_CODE,
                              CUST_CODE = A.CUST_CODE,
                              STOCK_NO = A.STOCK_NO,
                              STOCK_SEQ = A.STOCK_SEQ,
                              DEFECT_QTY = A.DEFECT_QTY.Value,
                              SERIAL_NO = A.SERIAL_NO,
                              UCC_CODE = A.UCC_CODE,
                              CAUSE = C.CAUSE,
                              OTHER_CAUSE = A.CAUSE,
                              WAREHOUSE_ID = A.WAREHOUSE_ID
                          }).ToList();


            return result.AsQueryable();
        }

        public IQueryable<F02020109> GetDatasByF020201s(string dcCode, string gupCode, string custCode, IQueryable<F020201> f020201s)
        {
            return _db.F02020109s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                             && x.GUP_CODE == gupCode
                                             && x.CUST_CODE == custCode
                                             && f020201s.Any(z => z.PURCHASE_NO == x.STOCK_NO &&
                                                                  z.PURCHASE_SEQ == x.STOCK_SEQ.ToString() &&
																																	z.RT_NO == x.RT_NO &&
																																	z.RT_SEQ == x.RT_SEQ));
        }

        public IQueryable<F02020109> GetDataByRtNos(string dcCode, string gupCode, string custCode, List<string> rtNos)
        {
            return _db.F02020109s.AsNoTracking().Where(x => 
            x.DC_CODE == dcCode
            && x.GUP_CODE == gupCode
            && x.CUST_CODE == custCode
            && rtNos.Contains(x.RT_NO));
        }
    }
}
