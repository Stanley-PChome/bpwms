using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050102Repository : RepositoryBase<F050102, Wms3plDbContext, F050102Repository>
    {
        public F050102Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F050102Ex> GetF050102Exs(string gupCode, string custCode)
        {
            var f050102Data = _db.F050102s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode);

            var f1903Data = _db.F1903s.AsNoTracking().Where(x => f050102Data.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE) &&
                                                                 x.CUST_CODE == custCode &&
                                                                 x.GUP_CODE == gupCode);

            var data = from A in f050102Data
                       join B in f1903Data
                       on new { A.ITEM_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ITEM_CODE, B.GUP_CODE, B.CUST_CODE } into subB
                       from B in subB.DefaultIfEmpty()
                       select new F050102Ex
                       {
                           DC_CODE = A.DC_CODE,
                           GUP_CODE = A.GUP_CODE,
                           CUST_CODE = A.CUST_CODE,
                           ORD_NO = A.ORD_NO,
                           ORD_SEQ = A.ORD_SEQ,
                           ITEM_CODE = A.ITEM_CODE,
                           ORD_QTY = A.ORD_QTY,
                           NO_DELV = A.NO_DELV,
                           SERIAL_NO = A.SERIAL_NO,
                           CRT_DATE = A.CRT_DATE,
                           CRT_STAFF = A.CRT_STAFF,
                           CRT_NAME = A.CRT_NAME,
                           ITEM_NAME = B.ITEM_NAME ?? null,
                           ITEM_SIZE = B.ITEM_SIZE ?? null,
                           ITEM_SPEC = B.ITEM_SPEC ?? null,
                           ITEM_COLOR = B.ITEM_COLOR ?? null,
                           BUNDLE_SERIALLOC = B.BUNDLE_SERIALLOC ?? null,
                           BUNDLE_SERIALNO = B.BUNDLE_SERIALNO ?? null,
						   MAKE_NO = A.MAKE_NO
					   };

            // RowNum
            var result = data.OrderBy(x => x.ORD_NO).ThenBy(x => x.ORD_SEQ).ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

            return result.AsQueryable();
        }
    }
}