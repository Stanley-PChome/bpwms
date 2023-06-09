using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F05010102Repository : RepositoryBase<F05010102, Wms3plDbContext, F05010102Repository>
    {
        public F05010102Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得商品序號資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="ordNo"></param>
        /// <returns></returns>
        public IQueryable<F05010102OSerialNo> GetF05010102SerialNoData(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var f05010102Data = _db.F05010102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.CUST_ORD_NO == ordNo);

            var f1903Data = _db.F1903s.AsNoTracking().Where(x => x.CUST_CODE == custCode &&
                                                                 x.GUP_CODE == gupCode &&
                                                                 f05010102Data.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE));

            var result = from A in f05010102Data
                         join B in f1903Data
                         on new { A.ITEM_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ITEM_CODE, B.GUP_CODE, B.CUST_CODE } into subB
                         from B in subB.DefaultIfEmpty()
                         select new F05010102OSerialNo
                         {
                             SERIAL_NO = A.SERIAL_NO,
                             ITEM_CODE = B.ITEM_CODE ?? null,
                             ITEM_NAME = B.ITEM_NAME ?? null
                         };

            return result;
        }
    }
}
