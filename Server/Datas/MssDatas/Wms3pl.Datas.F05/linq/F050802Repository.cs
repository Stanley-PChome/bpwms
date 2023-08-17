using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050802Repository : RepositoryBase<F050802, Wms3plDbContext, F050802Repository>
    {
        public F050802Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F050802> GetF050802s(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var result = _db.F050802s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.WMS_ORD_NO == wmsOrdNo);

            return result;
        }

        public IQueryable<F050802GroupItem> GetGroupItem(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var result = _db.F050802s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.GUP_CODE == gupCode &&
                                                                x.CUST_CODE == custCode &&
                                                                x.WMS_ORD_NO == wmsOrdNo)
                                                    .GroupBy(x => new { x.WMS_ORD_NO, x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE })
                                                    .Select(x => new F050802GroupItem
                                                    {
                                                        WMS_ORD_NO = x.Key.WMS_ORD_NO,
                                                        DC_CODE = x.Key.DC_CODE,
                                                        GUP_CODE = x.Key.GUP_CODE,
                                                        CUST_CODE = x.Key.CUST_CODE,
                                                        ITEM_CODE = x.Key.ITEM_CODE,
                                                        SUM_B_SET_QTY = x.Sum(z => z.B_DELV_QTY)
                                                    });

            return result;
        }

        /// <summary>
        /// 出貨單單是否含有序號商品
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public bool HasSerialItem(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var f050802s = _db.F050802s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.GUP_CODE == gupCode &&
                                                                x.CUST_CODE == custCode &&
                                                                x.WMS_ORD_NO == wmsOrdNo);
            var f1903s = _db.F1903s.AsNoTracking().Where(x => x.BUNDLE_SERIALNO == "1"
                                                     && f050802s.Select(z => z.ITEM_CODE).Distinct().Contains(x.ITEM_CODE)
                                                     && f050802s.Select(z => z.GUP_CODE).Distinct().Contains(x.GUP_CODE));

            var result = (from A in f050802s
                          join B in f1903s
                          on new { A.ITEM_CODE, A.GUP_CODE } equals new { B.ITEM_CODE, B.GUP_CODE }
                          select A).Any();

            return result;
        }

        public IQueryable<F050802> GetDatasByHasSerial(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var result = _db.F050802s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.WMS_ORD_NO == wmsOrdNo &&
                                                 x.SERIAL_NO != null);

            return result;
        }
    }
}
