using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F05010101Repository : RepositoryBase<F05010101, Wms3plDbContext, F05010101Repository>
    {
        public F05010101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<string> CheckF05010101SelectAll(List<string> ordNos)
        {
            var data = from A in _db.F05010101s.AsNoTracking().Where(x => ordNos.Contains(x.SMALL_ORD_NO))
                       join B in _db.F05010101s.AsNoTracking().Where(x => !ordNos.Contains(x.SMALL_ORD_NO))
                       on new { A.ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE } into subB
                       from B in subB.DefaultIfEmpty()
                       select new
                       {
                           SMALL_ORD_NO = B.SMALL_ORD_NO ?? null
                       };

            var result = data.Where(x => x.SMALL_ORD_NO != null).Select(x => x.SMALL_ORD_NO.ToString()).Distinct();

            return result;
        }

        public IQueryable<F05010101> GetF05010101ByOrdNo(string gupCode, string custCode, string dcCode, string ordNo)
        {
            var result = _db.F05010101s.Where(x => x.GUP_CODE == gupCode &&
                                                   x.CUST_CODE == custCode &&
                                                   x.DC_CODE == dcCode &&
                                                   x.ORD_NO == ordNo);

            return result;
        }
        public IQueryable<F05010101> GetDatas(string gupCode, string custCode, List<string> ordNos)
        {
            var result = _db.F05010101s.Where(x => x.GUP_CODE == gupCode &&
                                                   x.CUST_CODE == custCode &&
                                                   ordNos.Contains(x.ORD_NO));

            return result;
        }
    }
}