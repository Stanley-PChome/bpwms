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
    public partial class F91020601Repository : RepositoryBase<F91020601, Wms3plDbContext, F91020601Repository>
    {
        public F91020601Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得已經上架回倉產生調撥單的已回倉(已上架)或未回倉(未上架)的 ITEM_CODE, SERIAL_NO, SUM (D.A_TAR_QTY)
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="processNo"></param>
        /// <param name="backItemType"></param>
        /// <param name="isBacked"></param>
        /// <returns></returns>
        public IQueryable<ItemSumQty> GetItemSumQtys(string dcCode, string gupCode, string custCode, string processNo, string backItemType, bool isBacked)
        {
            var f91020601s = (from a in _db.F91020601s
                             select new
                             {
                                 a.DC_CODE,
                                 a.GUP_CODE,
                                 a.CUST_CODE,
                                 a.ALLOCATION_NO,
                                 a.PROCESS_NO,
                                 a.BACK_ITEM_TYPE,
                                 a.ITEM_CODE
                             }).Distinct();
            var q = from a in f91020601s
                    join b in _db.F151001s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ALLOCATION_NO }
                    equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.ALLOCATION_NO } 
                    join c in _db.F151002s on new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.ALLOCATION_NO, a.ITEM_CODE }
                    equals new { c.DC_CODE, c.GUP_CODE, c.CUST_CODE, c.ALLOCATION_NO, c.ITEM_CODE }
                    where a.DC_CODE == dcCode
                            && a.GUP_CODE == gupCode
                            && a.CUST_CODE == custCode
                            && a.PROCESS_NO == processNo
                            && a.BACK_ITEM_TYPE == backItemType
                    select new { a.ITEM_CODE, c.SERIAL_NO, c.A_TAR_QTY, b.STATUS};
            if (isBacked)
            {
                q = q.Where(c => c.STATUS != "3" && c.STATUS != "9");
            }
            else
            {
                q = q.Where(c => c.STATUS == "3");
            }

            var result = from g in q
                         group new { g.A_TAR_QTY }
                         by new { g.ITEM_CODE, g.SERIAL_NO } into gg
                         select new ItemSumQty
                         {
                             ITEM_CODE = gg.Key.ITEM_CODE,
                             SERIAL_NO = gg.Key.SERIAL_NO,
                             SumQty = gg.Sum(c => c.A_TAR_QTY)
                         };
            return result;
        }

        /// <summary>
        /// 取得已上架回倉的所有成品序號
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public IQueryable<string> GetBackedSerialNos(string dcCode, string gupCode, string custCode, string processNo)
        {
            var q = (from a in _db.F91020601s
                     join b in _db.F151001s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ALLOCATION_NO }
                     equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.ALLOCATION_NO }
                     where a.DC_CODE == dcCode
                     && a.GUP_CODE == gupCode
                     && a.CUST_CODE == custCode
                     && a.PROCESS_NO == processNo
                     && b.STATUS != "3"
                     && a.SERIAL_NO != "0"
                     && a.BACK_ITEM_TYPE == "0" // C.STATUS: 3 已下架處理, BACK_ITEM_TYPE: 回倉商品類型(0成品1揀料)
                     select a.SERIAL_NO).Distinct();
            return q;

        }
    }
}
