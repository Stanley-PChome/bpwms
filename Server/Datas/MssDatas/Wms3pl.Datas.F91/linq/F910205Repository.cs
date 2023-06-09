using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910205Repository : RepositoryBase<F910205, Wms3plDbContext, F910205Repository>
    {
        public F910205Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public IQueryable<ItemSumQty> GetCanBackMaterialItem(string dcCode, string gupCode, string custCode, string processNo)
        {
            #region UNION ALL
            var unionA = (from a in _db.F910205s
                          join b in _db.F91020501s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.PICK_NO }
                          equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.PICK_NO }
                          select new
                          {
                              ITEM_CODE = b.ITEM_CODE,
                              SERIAL_NO = string.IsNullOrEmpty(b.SERIAL_NO) ? "0" : b.SERIAL_NO,
                              Qty = b.PICK_QTY,
                              DC_CODE = a.DC_CODE,
                              GUP_CODE = a.GUP_CODE,
                              CUST_CODE = a.CUST_CODE,
                              PROCESS_NO = a.PROCESS_NO,

                          }).Concat(
                from a in _db.F910205s
                join b in _db.F91020502s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.PICK_NO}
                equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.PICK_NO}
                join c in _db.F151001s on new { p0 = b.DC_CODE, p1 =b.GUP_CODE, p2 = b.CUST_CODE, p3 = b.ALLOCATION_NO, p4 = "5"}
                equals new { p0 = c.DC_CODE, p1 = c.GUP_CODE, p2 = c.CUST_CODE, p3= c.ALLOCATION_NO, p4 = c.STATUS}
                join d in _db.F151002s on new { p0 = c.DC_CODE, p1 = c.GUP_CODE, p2 = c.CUST_CODE, p3 = c.ALLOCATION_NO }
                equals new { p0 = d.DC_CODE, p1 = d.GUP_CODE, p2 = d.CUST_CODE, p3 = d.ALLOCATION_NO}
                select new
                {
                    ITEM_CODE = d.ITEM_CODE,
                    SERIAL_NO = string.IsNullOrEmpty(d.SERIAL_NO) ? "0" : d.SERIAL_NO,
                    Qty = Convert.ToInt32( d.A_SRC_QTY),
                    DC_CODE = a.DC_CODE,
                    GUP_CODE = a.GUP_CODE,
                    CUST_CODE = a.CUST_CODE,
                    PROCESS_NO = a.PROCESS_NO,

                });
            #endregion
            var q = from a in unionA
                    where a.DC_CODE == dcCode
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.PROCESS_NO == processNo
                    group new { a.Qty }
                    by new { a.ITEM_CODE, a.SERIAL_NO } into g
                    select new ItemSumQty
                    {
                        ITEM_CODE = g.Key.ITEM_CODE,
                        SERIAL_NO = g.Key.SERIAL_NO,
                        SumQty = g.Sum(s => s.Qty)
                    };
          
            return q;
        }

 
    }
}
