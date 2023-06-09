using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910102Repository : RepositoryBase<F910102, Wms3plDbContext, F910102Repository>
    {
        public F910102Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// ToDo: 還沒加上WAREHOUSE_TYPE的判斷
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public IQueryable<BomQtyData> GetBomQtyData(string dcCode, string gupCode, string custCode, string processNo)
        {
            var f1913 = from a in _db.F1913s
                        join b in _db.F1912s on new { a.DC_CODE, a.LOC_CODE }
                        equals new { b.DC_CODE, b.LOC_CODE }
                        join c in _db.F1980s on new { p0 = b.DC_CODE, p1 = b.WAREHOUSE_ID, p2 = "W" }
                        equals new { p0 = c.DC_CODE, p1 = c.WAREHOUSE_ID, p2 = c.WAREHOUSE_TYPE }
                        select a;

            var q = from a in _db.F910102s
                    join b in _db.F910101s on new { a.GUP_CODE, a.CUST_CODE, a.BOM_NO }
                    equals new { b.GUP_CODE, b.CUST_CODE, b.BOM_NO }
                    join c in _db.F910201s on new { p0 = a.GUP_CODE, p1 = a.CUST_CODE, p2 = a.BOM_NO }
                    equals new { p0 = c.GUP_CODE, p1 = c.CUST_CODE, p2 = c.ITEM_CODE_BOM }
                    join d in f1913 on new { p0 = c.GUP_CODE, p1 = c.CUST_CODE, p2 = c.DC_CODE, p3 = a.MATERIAL_CODE }
                    equals new { p0 = d.GUP_CODE, p1 = d.CUST_CODE, p2 = d.DC_CODE, p3 = d.ITEM_CODE }
                    where b.BOM_TYPE == "0"
                    && c.DC_CODE == dcCode
                    && c.GUP_CODE == gupCode
                    && c.CUST_CODE == custCode
                    && c.PROCESS_NO == processNo
                    group d
                    by new
                    {
                        c.DC_CODE,
                        c.GUP_CODE,
                        c.CUST_CODE,
                        c.ITEM_CODE,
                        c.ITEM_CODE_BOM,
                        a.MATERIAL_CODE,
                        a.BOM_QTY,
                        c.PROCESS_QTY
                    }
                    into g
                    select new BomQtyData
                    {
                        DC_CODE = g.Key.DC_CODE,
                        GUP_CODE = g.Key.GUP_CODE,
                        CUST_CODE = g.Key.CUST_CODE,
                        ITEM_CODE = g.Key.ITEM_CODE,
                        ITEM_CODE_BOM = g.Key.ITEM_CODE_BOM,
                        MATERIAL_CODE = g.Key.MATERIAL_CODE,
                        BOM_QTY = g.Key.BOM_QTY,
                        PROCESS_QTY = g.Key.PROCESS_QTY,
                        NEED_QTY = (g.Key.BOM_QTY * g.Key.PROCESS_QTY),
                        AVAILABLE_QTY = g.Sum(c => c.QTY),
                        WAREHOUSE_TYPE = "W",
                    };
            var result = q.ToList();
            var rI = 1;
            foreach (var item in result)
            {
                item.ROWNUM = rI;
                rI++;
            }
            return result.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        public IQueryable<BomItemDetail> GetBomItemDetailList(string gupCode, string custCode, List<string> itemCodes)
        {
            var q = from a in _db.F910102s
                    join b in _db.F910101s on new { a.GUP_CODE, a.CUST_CODE, a.BOM_NO }
                    equals new { b.GUP_CODE, b.CUST_CODE, b.BOM_NO }
                    where a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && b.STATUS == "0"
                    && b.ISPROCESS == "0"
                    && itemCodes.Contains(b.ITEM_CODE)
                    select new BomItemDetail
                    {
                        GUP_CODE = a.GUP_CODE, 
                        CUST_CODE = a.CUST_CODE,
                        ITEM_CODE = b.ITEM_CODE,
                        MATERIAL_CODE = a.MATERIAL_CODE,
                        BOM_QTY = a.BOM_QTY
                    };
           
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="bomNo"></param>
        /// <returns></returns>
        public IQueryable<F910102Data> GetF910102Data(string gupCode, string custCode, string bomNo)
        {
            var q = from a in _db.F910102s
                    join b in _db.F910101s on new { p0 = a.GUP_CODE, p1 = a.CUST_CODE, p2 = a.MATERIAL_CODE, p3 = "0", p4 = "1" }
                    equals new { p0 = b.GUP_CODE, p1 = b.CUST_CODE, p2 = b.ITEM_CODE, p3 = b.STATUS, p4 = b.ISPROCESS } into bb
                    from b in bb.DefaultIfEmpty()
                    join c in _db.F910102s on new { b.GUP_CODE, b.CUST_CODE, b.BOM_NO }
                    equals new { c.GUP_CODE, c.CUST_CODE, c.BOM_NO } into cc
                    from c in cc.DefaultIfEmpty()
                    join d in _db.F1903s on new { p0 = a.GUP_CODE, p1 = a.MATERIAL_CODE, p2 = a.CUST_CODE }
                    equals new { p0 = d.GUP_CODE, p1 = d.ITEM_CODE, p2 = d.CUST_CODE } into dd
                    from d in dd.DefaultIfEmpty()
                    where a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.BOM_NO == bomNo
                    group c
                    by new { a.MATERIAL_CODE, d.ITEM_NAME, a.BOM_QTY, b.ITEM_CODE } into g
                    select new F910102Data
                    {
                        MATERIAL_CODE = g.Key.MATERIAL_CODE,
                        ITEM_NAME = g.Key.ITEM_NAME,
                        BOM_QTY = g.Key.BOM_QTY,
                        ITEM_QTY = g.Sum(c => c == null ? 0 : c.BOM_QTY),
                        MULTI_FLAG = string.IsNullOrEmpty(g.Key.ITEM_CODE) ? "0" : "1"
                    };
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="bomNos"></param>
        /// <returns></returns>
        public IQueryable<F910102> GetF910102ByBomNoDatas(string gupCode, string custCode, List<string> bomNos)
        {
            var q = from a in _db.F910102s
                    where a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && bomNos.Contains(a.BOM_NO)
                    select a;
            return q;
        }

        public IQueryable<F910102> GetDatas(string gupCode, string custCode, List<string> bomNos)
        {
            return _db.F910102s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            bomNos.Contains(x.BOM_NO));
        }
    }
}
