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
    public partial class F910201Repository : RepositoryBase<F910201, Wms3plDbContext, F910201Repository>
    {
        public F910201Repository(string connName, WmsTransaction wmsTransaction = null)
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
        public IQueryable<ProcessItem> GetProcessItemsByBom(string dcCode, string gupCode, string custCode, string processNo)
        {
            var q = from a in _db.F910201s
                    join b in _db.F910102s on new { p0 = a.ITEM_CODE_BOM, p1 = a.GUP_CODE, p2 = a.CUST_CODE }
                    equals new { p0 = b.BOM_NO, p1 = b.GUP_CODE, p2 = b.CUST_CODE }
                    join c in _db.F1903s on new { p0 = b.MATERIAL_CODE, p1 = b.GUP_CODE, p2 = b.CUST_CODE }
                    equals new { p0 = c.ITEM_CODE, p1 = c.GUP_CODE, p2 = c.CUST_CODE }
                    where !string.IsNullOrEmpty(a.ITEM_CODE_BOM)
                    && a.DC_CODE == dcCode
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.PROCESS_NO == processNo
                    select new ProcessItem
                    {
                        ITEM_CODE = b.MATERIAL_CODE,
                        ITEM_SPEC = c.ITEM_SPEC, 
                        ITEM_COLOR = c.ITEM_COLOR,
                        ITEM_SIZE = c.ITEM_SIZE,
                        ITEM_NAME = c.ITEM_NAME
                    };
            
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="finishDate"></param>
        /// <returns></returns>
        public IQueryable<ProduceLineStatusItem> GetProduceLineStatusItems(string dcCode, DateTime finishDate)
        {
            var notFinishedStatuses = new List<string>() { "2", "9"};
            var q = from a in _db.F910201s
                    join b in _db.F910203s on new { a.DC_CODE, a.CUST_CODE, a.GUP_CODE, a.PROCESS_NO }
                    equals new { b.DC_CODE, b.CUST_CODE, b.GUP_CODE, b.PROCESS_NO }
                    where a.DC_CODE == dcCode
                    && a.FINISH_DATE.Date == finishDate.Date
                    group new { a.STATUS, }
                    by new { b.PRODUCE_NO } into g
                    select new ProduceLineStatusItem
                    {
                        PRODUCE_NO = g.Key.PRODUCE_NO,
                        FINISHCOUNT = g.Sum(c => c.STATUS =="2" ?1:0),
                        UNFINISHCOUNT = g.Sum(c => notFinishedStatuses.Contains(c.STATUS) ? 0 : 1)
                    };
            var result = q.OrderBy(c => c.PRODUCE_NO).ToList();
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
        /// <param name="dcCode"></param>
        /// <param name="finishDate"></param>
        /// <returns></returns>
        public IQueryable<DcWmsNoStatusItem> GetWorkProcessOverFinishTimeByDc(string dcCode, DateTime finishDate)
        {

            var q = from a in _db.F910201s
                    join b in _db.F910203s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.PROCESS_NO }
                    equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.PROCESS_NO }
                    join c in _db.F910004s on new { p0 = a.DC_CODE, p1 = b.PRODUCE_NO, p2 = "0" }
                    equals new { p0 = c.DC_CODE, p1 = c.PRODUCE_NO, p2 = c.STATUS }
                    where a.DC_CODE == dcCode
                    && a.FINISH_DATE == finishDate
                    && (DateTime.Now.Subtract(a.FINISH_DATE.Add(TimeSpan.Parse(a.FINISH_TIME))).TotalSeconds > 0)
                    select new DcWmsNoStatusItem
                    {
                        WMS_NO = a.PROCESS_NO,
                        MEMO = c.PRODUCE_NAME,
                        START_DATE = a.FINISH_DATE.Add(TimeSpan.Parse(a.FINISH_TIME))
                    };
            var result = q.OrderBy(c => c.WMS_NO).ToList();
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
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="begFinishDate"></param>
        /// <param name="endFinishDate"></param>
        /// <returns></returns>
        public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItems(string dcCode, string gupCode, string custCode,
            DateTime begFinishDate, DateTime endFinishDate)
        {
            var q = from a in _db.F910201s
                    where a.DC_CODE == dcCode
                    && a.CUST_CODE == custCode
                    && a.FINISH_DATE >= begFinishDate
                    && a.FINISH_DATE < endFinishDate
                    && a.STATUS != "9"
                    group new { a.FINISH_DATE }
                    by new { a.FINISH_DATE } into g
                    select new DcWmsNoDateItem
                    {
                        WmsDate = g.Key.FINISH_DATE,
                        WmsCount = g.Count()
                    };
            var result = q.OrderBy(c => c.WmsDate).ToList();
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
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="crt_SDate"></param>
        /// <param name="crt_EDate"></param>
        /// <param name="outSourceId"></param>
        /// <returns></returns>
        public IQueryable<F910201ProcessData> GetProcessDatas(string dcCode, string gupCode, string custCode, DateTime? crt_SDate, DateTime? crt_EDate, string outSourceId)
        {
            var f910301 = from a in _db.F910301s
                          join b in _db.F910302s on new { a.DC_CODE, a.GUP_CODE, a.CONTRACT_NO }
                          equals new { b.DC_CODE, b.GUP_CODE, b.CONTRACT_NO }
                          where a.OBJECT_TYPE == "1"
                          select new
                          {
                              DC_CODE = a.DC_CODE,
                              GUP_CODE = a.GUP_CODE,
                              UNI_FORM = a.UNI_FORM,
                              PROCESS_ID = b.PROCESS_ID,
                              ENABLE_DATE = a.ENABLE_DATE,
                              DISABLE_DATE = a.DISABLE_DATE,
                              OUTSOURCE_COST = b.OUTSOURCE_COST
                          };
            var f91000301 = from a in _db.F91000301s
                            where a.ITEM_TYPE_ID == "001"
                            select a;

            var q = from a in _db.F910201s
                    from b in _db.F910401s.Where(c => (c.DC_CODE == a.DC_CODE || c.DC_CODE == "000") && c.GUP_CODE == a.GUP_CODE && a.CUST_CODE == c.CUST_CODE && a.QUOTE_NO == c.QUOTE_NO).DefaultIfEmpty()
                    join c in _db.F910402s on new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.QUOTE_NO }
                    equals new { c.DC_CODE, c.GUP_CODE, c.CUST_CODE, c.QUOTE_NO } into cc
                    from c in cc.DefaultIfEmpty()
                    join d in _db.F910001s on c.PROCESS_ID equals d.PROCESS_ID into dd
                    from d in dd.DefaultIfEmpty()
                    join e in f91000301 on new { p0 = c.UNIT_ID } equals new { p0 = e.ACC_ITEM_KIND_ID } into ee
                    from e in ee.DefaultIfEmpty()
                    join f in _db.F1928s on b.OUTSOURCE_ID equals f.OUTSOURCE_ID into ff
                    from f in ff.DefaultIfEmpty()
                    from g in f910301.Where(gg => (gg.DC_CODE == "000" || gg.DC_CODE == a.DC_CODE)
                    && a.GUP_CODE == gg.GUP_CODE
                    && f.UNI_FORM == gg.UNI_FORM
                    && c.PROCESS_ID == gg.PROCESS_ID
                    && a.CRT_DATE.AddDays(1) >= gg.ENABLE_DATE
                    && a.CRT_DATE.AddDays(1) < gg.DISABLE_DATE)
                    where a.DC_CODE == dcCode
                    select new 
                    {

                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        OUTSOURCE_ID = b.OUTSOURCE_ID,
                        CRT_DATE = a.CRT_DATE,
                        PROCESS_NO = a.PROCESS_NO,
                        PROCESS_ID = c.PROCESS_ID,
                        PROCESS_ACT = d.PROCESS_ACT,
                        UNIT_ID = c.UNIT_ID,
                        ACC_ITEM_KIND_NAME = e.ACC_ITEM_KIND_NAME,
                        OUTSOURCE_COST = g.OUTSOURCE_COST.HasValue? g.OUTSOURCE_COST.Value:0,
                        SUBTOTAL = a.PROCESS_QTY * (g.OUTSOURCE_COST.HasValue ? g.OUTSOURCE_COST.Value : 0),
                        TOTALWORKHOURS = a.PROCESS_QTY * c.WORK_HOUR
                    };            

            if (!string.IsNullOrEmpty(gupCode))
            {
                q = q.Where(c => c.GUP_CODE == gupCode);
            }

            if (!string.IsNullOrEmpty(custCode))
            {
                q = q.Where(c => c.CUST_CODE == custCode);
            }

            if (crt_SDate != null)
            {
                q = q.Where(c => c.CRT_DATE >= crt_SDate);
            }
            if (crt_EDate != null)
            {
                q = q.Where(c => c.CRT_DATE < crt_EDate);
            }
            if (!string.IsNullOrEmpty(outSourceId))
            {
                q = q.Where(c => c.OUTSOURCE_ID == outSourceId);
            }

            var result = q.Select(c => new F910201ProcessData
            {
                CRT_DATE = c.CRT_DATE,
                PROCESS_NO = c.PROCESS_NO,
                PROCESS_ID = c.PROCESS_ID,
                PROCESS_ACT = c.PROCESS_ACT,
                UNIT_ID = c.UNIT_ID,
                ACC_ITEM_KIND_NAME = c.ACC_ITEM_KIND_NAME,
                OUTSOURCE_COST = c.OUTSOURCE_COST,
                SUBTOTAL = c.SUBTOTAL,
                TOTALWORKHOURS = c.TOTALWORKHOURS
            }).Distinct().OrderBy(s => s.CRT_DATE).ToList();
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
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="settleDate"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        public IQueryable<SettleData> GetQuoteDatas(string dcCode, string gupCode, string custCode, DateTime settleDate, List<string> quotes)
        {
            var statuses = new List<string>() { "2", "3"};
            var q = from a in _db.F910201s
                    from b in _db.F910401s.Where(bb => (bb.DC_CODE == "000" || bb.DC_CODE == a.DC_CODE)
                    && a.GUP_CODE == bb.GUP_CODE
                    && a.CUST_CODE == bb.CUST_CODE
                    && a.QUOTE_NO == bb.QUOTE_NO)
                    join c in _db.F910402s on new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.QUOTE_NO }
                    equals new { c.DC_CODE, c.GUP_CODE, c.CUST_CODE, c.QUOTE_NO }
                    join d in _db.F910001s on c.PROCESS_ID equals d.PROCESS_ID into dd
                    from d in dd.DefaultIfEmpty()
                    join e in _db.F91000302s on new { p0 = c.UNIT_ID, p1 = "001" }
                    equals new { p0 = e.ACC_UNIT, p1 = e.ITEM_TYPE_ID }
                    where (a.DC_CODE == dcCode || a.DC_CODE == "000")
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.FINISH_DATE == settleDate
                    && statuses.Contains(a.STATUS)
                    && quotes.Contains(a.QUOTE_NO)
                    orderby a.PROCESS_NO
                    select new SettleData
                    {
                        CAL_DATE = settleDate,
                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE, 
                        CUST_CODE = a.CUST_CODE, 
                        WMS_NO = a.PROCESS_NO,
                        ITEM_CODE = a.ITEM_CODE, 
                        ITEM_CODE_BOM = a.ITEM_CODE_BOM,
                        QTY = a.PROCESS_QTY,
                        QUOTE_NO = a.QUOTE_NO,
                        PROCESS_ID = c.PROCESS_ID,
                        ACC_NUM = 1,
                        APPROV_FEE = c.WORK_COST,
                        ACC_UNIT_NAME = e.ACC_UNIT_NAME,
                        PACKAGE_BOX_NO = Convert.ToInt16(a.A_PROCESS_QTY),
                        BOX_NO = a.A_PROCESS_QTY
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

        //public IQueryable<P910102Data> GetP910102Datas(string dcCode, string gupCode, string custCode, DateTime beginDate, DateTime endDate, string status, string operateStatus)
        //{
           
        //    var parms = new List<object> { dcCode, gupCode, custCode, beginDate, endDate.AddDays(1) };
        //    var q = from a in _db.F910201s
        //            join b in _db.VW_F000904_LANGs on new { p0 = "PROCESS_ITEM", p1 = a.PROCESS_ITEM, p2 = Current.Lang }
        //            equals new { p0 = b.SUBTOPIC, p1 = b.VALUE, p2 = b.LANG } into bb
        //            from b in bb.DefaultIfEmpty()
        //            join c in _db.F1903s on new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE }
        //            equals new { c.GUP_CODE, c.CUST_CODE, c.ITEM_CODE } into cc
        //            from c in cc.DefaultIfEmpty()
        //            join d in _db.F91000302s on new { p0 = "001", p1 = c.ITEM_UNIT }
        //            equals new { p0 = d.ITEM_TYPE_ID, p1 = d.ACC_UNIT } into dd
        //            from d in dd.DefaultIfEmpty()
        //            join e in _db.F1903s on new { p0 = a.GUP_CODE, p1 = a.CUST_CODE, p2 = a.GOOD_CODE }
        //            equals new { p0 = e.GUP_CODE, p1 = e.CUST_CODE, p2 = e.ITEM_CODE } into ee
        //            from e in ee.DefaultIfEmpty()
        //            join f in _db.F91000302s on new { p0 = "001", p1 = e.ITEM_UNIT }
        //            equals new { p0 = f.ITEM_TYPE_ID, p1 = f.ACC_UNIT } into ff
        //            from f in ff.DefaultIfEmpty()
        //            join g in _db.VW_F000904_LANGs on new { p0 = "F910201", p1= "STATUS", p2 = a.STATUS, p3 = Current.Lang}
        //            equals new { p0= g.TOPIC, p1 = g.SUBTOPIC, p2 = g.VALUE, p3 = g.LANG} into gg
        //            from g in gg.DefaultIfEmpty()
        //            join h in _db.VW_F000904_LANGs on new { p0 = "P910201", p1 = "PROC_STATUS", p2 = a.PROC_STATUS, p3 = Current.Lang }
        //            equals new { p0 = h.TOPIC, p1 = h.SUBTOPIC, p2 = h.VALUE, p3 = h.LANG } into hh
        //            from h in hh.DefaultIfEmpty()
        //            where a.PROC_TYPE == "2" // 快速加工單 
        //            && a.DC_CODE == dcCode
        //            && a.CUST_CODE == custCode
        //            && a.CRT_DATE >= beginDate
        //            && a.CRT_DATE < endDate.AddDays(1)
        //            select new P910102Data
        //            {
        //                DC_CODE = a.DC_CODE,
        //                GUP_CODE = a.GUP_CODE,
        //                CUST_CODE = a.CUST_CODE,
        //                PROCESS_NO = a.PROCESS_NO,
        //                PROCESS_SOURCE = a.PROCESS_SOURCE,
        //                PROCESS_ITEM = a.PROCESS_ITEM,
        //                PROCESS_ITEM_NAME = b.NAME,
        //                PACK_QTY = a.PACK_QTY,
        //                ORDER_NO = a.ORDER_NO,
        //                ITEM_CODE = a.ITEM_CODE,
        //                ITEM_NAME = c.ITEM_NAME,
        //                ITEM_UNIT = d.ACC_UNIT_NAME,
        //                PROCESS_QTY = a.PROCESS_QTY,
        //                GOOD_CODE = a.GOOD_CODE,
        //                GOOD_NAME = e.ITEM_NAME,
        //                GOOD_UNIT = f.ACC_UNIT_NAME,
        //                GOOD_QTY = Convert.ToDecimal((a.PROCESS_ITEM =="3"? (a.PROCESS_QTY / a.PACK_QTY):a.PROCESS_QTY)),
        //                MEMO = a.MEMO,
        //                STATUS = a.STATUS,
        //                STATUS_NAME = g.NAME,
        //                PROC_STATUS = a.PROC_STATUS,
        //                PROC_BEGIN_TIME = a.PROC_BEGIN_DATE,
        //                PROC_END_TIME = a.PROC_END_DATE
        //            };

        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        q = q.Where(c => c.STATUS == status);
        //    }
        //    else
        //    {
        //        q = q.Where(c => c.STATUS != "9");
        //    }
        //    if (!string.IsNullOrEmpty(operateStatus))
        //    {
        //        q = q.Where(c => c.PROC_STATUS == operateStatus);
        //    }

        //    var result = q.OrderBy(s => s.PROCESS_NO).ToList();
        //    var rI = 1;
        //    foreach (var item in result)
        //    {
        //        item.ROWNUM = rI;
        //        rI++;
        //    }

        //    return result.AsQueryable();

        //}
    }
}
