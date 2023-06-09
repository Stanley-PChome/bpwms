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
    public partial class F910301Repository : RepositoryBase<F910301, Wms3plDbContext, F910301Repository>
    {
        public F910301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F910301Data> GetContractDatas(string dcCode, string gupCode, string contractNo, string objectType,
            DateTime beginCreateDate, DateTime endCreateDate, string uniForm)
        {
            var f910301 = _db.F910301s.AsNoTracking().Where(x => x.OBJECT_TYPE == objectType &&
                                                                 x.DC_CODE == dcCode &&
                                                                 x.GUP_CODE == gupCode &&
                                                                 x.CRT_DATE >= beginCreateDate &&
                                                                 x.CRT_DATE <= endCreateDate);

            var f1909 = _db.F1909s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                             x.STATUS != "9");

            var f1928 = _db.F1928s.AsNoTracking().Where(x => x.STATUS != "9");

            if (!string.IsNullOrWhiteSpace(contractNo))
            {
                f910301 = f910301.Where(x => x.CONTRACT_NO == contractNo);
            }

            if (!string.IsNullOrWhiteSpace(uniForm))
            {
                f910301 = f910301.Where(x => x.UNI_FORM == uniForm);

                f1909 = f1909.Where(x => x.UNI_FORM == uniForm);

                f1928 = f1928.Where(x => x.UNI_FORM == uniForm);
            }

            var result = from A in f910301
                         join B in f1909
                         on new { A.GUP_CODE, A.UNI_FORM } equals new { B.GUP_CODE, B.UNI_FORM } into subB
                         from B in subB.DefaultIfEmpty()
                         join C in f1928
                         on A.UNI_FORM equals C.UNI_FORM into subC
                         from C in subC.DefaultIfEmpty()
                         select new F910301Data
                         {
                             CONTRACT_NO = A.CONTRACT_NO,
                             ENABLE_DATE = A.ENABLE_DATE,
                             DISABLE_DATE = A.DISABLE_DATE,
                             OBJECT_TYPE = A.OBJECT_TYPE,
                             UNI_FORM = A.UNI_FORM,
                             STATUS = A.STATUS,
                             MEMO = A.MEMO,
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CRT_STAFF = A.CRT_STAFF,
                             CRT_DATE = A.CRT_DATE,
                             UPD_STAFF = A.UPD_STAFF,
                             UPD_DATE = A.UPD_DATE,
                             CRT_NAME = A.CRT_NAME,
                             UPD_NAME = A.UPD_NAME,
                             CYCLE_DATE = A.CYCLE_DATE,
                             CLOSE_CYCLE = A.CLOSE_CYCLE,
                             OBJECT_NAME = A.OBJECT_TYPE == "0" ? B.CUST_NAME ?? null : C.OUTSOURCE_NAME ?? null,
                             CONTACT = A.OBJECT_TYPE == "0" ? B.CONTACT ?? null : C.CONTACT ?? null,
                             TEL = A.OBJECT_TYPE == "0" ? B.TEL ?? null : C.TEL ?? null,
                             DUEDATE = Convert.ToDecimal(Math.Ceiling(new TimeSpan(A.DISABLE_DATE.Ticks - DateTime.Now.Ticks).TotalDays)),
                             CUST_CODE = B.CUST_CODE ?? null
                         };

            return result;
        }

        //public IQueryable<F910301Report> GetContractReports(string dcCode, string gupCode, string contractNo)
        //{
        //    #region SubQueryA
        //    var subQueryA = from a in _db.F910301s
        //             join b in _db.F1909s.Where(c => c.STATUS != "9") on new { a.GUP_CODE, a.UNI_FORM }
        //             equals new { b.GUP_CODE, b.UNI_FORM } into bb
        //             from b in bb.DefaultIfEmpty()
        //             join c in _db.F1928s.Where(c => c.STATUS != "9") on a.UNI_FORM equals c.UNI_FORM into cc
        //             from c in cc.DefaultIfEmpty()
        //             select new
        //             {
        //                 a,
        //                 OBJECT_NAME = a.OBJECT_TYPE == "0" ? b.CUST_NAME : c.OUTSOURCE_NAME,
        //                 CONTACT = a.OBJECT_TYPE == "0" ? b.CONTACT : c.CONTACT,
        //                 TEL = a.OBJECT_TYPE == "0" ? b.TEL : c.TEL
        //             };
        //    #endregion
        //    #region SubQueryB
        //    var subQueryB = from a in _db.F910301s
        //                    join b in _db.F910302s on new { a.DC_CODE, a.GUP_CODE, a.CONTRACT_NO }
        //                    equals new { b.DC_CODE, b.GUP_CODE, b.CONTRACT_NO } into bb
        //                    from b in bb.DefaultIfEmpty()
        //                    join c in _db.F1909s.Where(c => c.STATUS != "9") on new { a.GUP_CODE, a.UNI_FORM }
        //                    equals new { c.GUP_CODE, c.UNI_FORM } into cc
        //                    from c in cc.DefaultIfEmpty()
        //                    join d in _db.F910401s on new { b.DC_CODE, b.GUP_CODE, c.CUST_CODE, b.QUOTE_NO }
        //                    equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.QUOTE_NO } into dd
        //                    from d in dd.DefaultIfEmpty()
        //                    join e in _db.F910003s on b.ITEM_TYPE equals e.ITEM_TYPE into ee
        //                    from e in ee.DefaultIfEmpty()
        //                    join f in _db.F91000302s on new { p0 = b.UNIT_ID, p1 = "001" }
        //                    equals new { p0 = f.ACC_UNIT, p1 = f.ITEM_TYPE_ID } into ff
        //                    from f in ff.DefaultIfEmpty()
        //                    join g in _db.F910001s on b.PROCESS_ID equals g.PROCESS_ID into gg
        //                    from g in gg.DefaultIfEmpty()
        //                    select new 
        //                    {
        //                        b,
        //                        CONTRACT_TYPE = b.CONTRACT_TYPE,
        //                        CONTRACT_TYPENAME = b.CONTRACT_TYPE == "0"? "主約" : "附約",
        //                        QUOTE_NAME = d.QUOTE_NAME,
        //                        ITEM_TYPE_NAME = e.ITEM_TYPE,
        //                        UNIT = f.ACC_UNIT_NAME,
        //                        PROCESS_ACT = g.PROCESS_ACT
        //                    };
        //    #endregion
        //    var q = from a in subQueryA
        //            join b in subQueryB on new { a.a.CONTRACT_NO, a.a.DC_CODE, a.a.GUP_CODE }
        //            equals new { b.b.CONTRACT_NO, b.b.DC_CODE, b.b.GUP_CODE }
        //            where a.a.DC_CODE == dcCode
        //            && a.a.GUP_CODE == gupCode
        //            && a.a.CONTRACT_NO == contractNo
        //            select new F910301Report
        //            {
        //                CONTRACT_NO = a.a.CONTRACT_NO,
        //                MAIN_ENABLE_DATE = a.a.ENABLE_DATE.ToString("yyyy/MM/dd"),
        //                MAIN_DISABLE_DATE = a.a.DISABLE_DATE.ToString("yyyy/MM/dd"),
        //                OBJECT_NAME = a.OBJECT_NAME,
        //                CONTACT = a.CONTACT,
        //                TEL = a.TEL,
        //                UNI_FORM = a.a.UNI_FORM,
        //                MEMO = a.a.MEMO,
        //                CONTRACT_TYPENAME = b.CONTRACT_TYPENAME,
        //                SUB_CONTRACT_NO = b.b.SUB_CONTRACT_NO,
        //                ITEM_TYPE_NAME = b.ITEM_TYPE_NAME,
        //                QUOTE_NAME = b.QUOTE_NAME,
        //                UNIT = b.UNIT,
        //                WORK_HOUR = b.b.WORK_HOUR.ToString(),
        //                TASK_PRICE = b.b.TASK_PRICE.ToString(),
        //                ENABLE_DATE = b.b.ENABLE_DATE.ToString("yyyy/MM/dd"),
        //                DISABLE_DATE = b.b.DISABLE_DATE.ToString("yyyy/MM/dd"),
        //                PROCESS_ACT = b.PROCESS_ACT,
        //                OUTSOURCE_COST = b.b.OUTSOURCE_COST.ToString(),
        //                APPROVE_PRICE = b.b.APPROVE_PRICE.ToString()
        //            };
           

        //    return q;
        //}

        public IQueryable<F910401> GetF910301WithF910401(string gupCode, string dcCode, string uniForm, string enableDate)
        {
            var enableDateDt = Convert.ToDateTime(enableDate);
            var inQuery = from a in _db.F910301s
                          join b in _db.F910302s on new { a.DC_CODE, a.GUP_CODE, a.CONTRACT_NO }
                          equals new { b.DC_CODE, b.GUP_CODE, b.CONTRACT_NO }
                          where a.STATUS != "9"
                          && (a.DC_CODE == dcCode || a.DC_CODE == "000")
                          && a.GUP_CODE == gupCode
                          && a.UNI_FORM == uniForm
                          && a.ENABLE_DATE <= enableDateDt
                          && a.DISABLE_DATE > enableDateDt
                          select b.QUOTE_NO;
           
            var q = (from a in _db.F910401s
                    where a.ENABLE_DATE <= enableDateDt
                    && a.DISABLE_DATE > enableDateDt
                    && inQuery.Contains(a.QUOTE_NO)
                    select a).Distinct();           
            return q;
        }

        public IQueryable<ContractSettleData> GetContractSettleDatas(DateTime settleDate)
        {
            var q = from a in _db.F910301s
                    join b in _db.F910302s on new { a.DC_CODE, a.GUP_CODE, a.CONTRACT_NO }
                    equals new { b.DC_CODE, b.GUP_CODE, b.CONTRACT_NO }
                    join c in _db.F1909s on a.UNI_FORM equals c.UNI_FORM
                    from d in _db.F500103s.Where(cc => (cc.DC_CODE == "000" || cc.DC_CODE == b.DC_CODE)
                    && cc.GUP_CODE == b.GUP_CODE
                    && cc.QUOTE_NO == b.QUOTE_NO
                    ).DefaultIfEmpty()
                    from e in _db.F199007s.Where(ee => (ee.DC_CODE == "000" || ee.DC_CODE == b.DC_CODE)
                    && ee.GUP_CODE == b.GUP_CODE
                    && ee.ACC_PROJECT_NO == b.QUOTE_NO).DefaultIfEmpty()
                    from f in _db.F500104s.Where(ff => (ff.DC_CODE == "000" || ff.DC_CODE == b.DC_CODE)
                    && ff.GUP_CODE == b.GUP_CODE
                    && ff.QUOTE_NO == b.QUOTE_NO).DefaultIfEmpty()
                    where b.ENABLE_DATE <= settleDate
                    && b.DISABLE_DATE > settleDate
                    && a.STATUS == "0"
                    select new ContractSettleData
                    {
                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = c.CUST_CODE,
                        CONTRACT_NO = a.CONTRACT_NO,
                        ENABLE_DATE = a.ENABLE_DATE,
                        DISABLE_DATE = a.DISABLE_DATE,
                        CYCLE_DATE = a.CYCLE_DATE,
                        CLOSE_CYCLE = a.CLOSE_CYCLE,
                        SUB_CONTRACT_NO = b.SUB_CONTRACT_NO,
                        ITEM_TYPE = b.ITEM_TYPE,
                        QUOTE_NO = b.QUOTE_NO,
                        UNIT_ID = b.UNIT_ID,
                        TASK_PRICE = b.TASK_PRICE,
                        WORK_HOUR = b.WORK_HOUR,
                        PROCESS_ID = b.PROCESS_ID,
                        OUTSOURCE_COST = b.OUTSOURCE_COST,
                        APPROVE_PRICE = b.APPROVE_PRICE,
                        ACC_ITEM_KIND_ID = b.ITEM_TYPE =="003"? f.ACC_ITEM_KIND_ID:d.ACC_ITEM_KIND_ID,
                        ACC_KIND = e.ACC_KIND
                    };

            return q;
        }

        public IQueryable<SettleReportData> GetSettleReportDatas(DateTime settleDate)
        {
            var q = from a in _db.F910301s
                    join b in _db.F910302s on new { a.DC_CODE, a.GUP_CODE, a.CONTRACT_NO }
                    equals new { b.DC_CODE, b.GUP_CODE, b.CONTRACT_NO }
                    join c in _db.F1909s on a.UNI_FORM equals c.UNI_FORM into cc
                    from c in cc.DefaultIfEmpty()
                    join d in _db.F1928s on a.UNI_FORM equals d.UNI_FORM into dd
                    from d in dd.DefaultIfEmpty()
                    where b.ENABLE_DATE <= settleDate
                    && b.DISABLE_DATE > settleDate
                    && a.STATUS == "0"
                    select new SettleReportData
                    {
                        CNT_DATE = settleDate,
                        CONTRACT_NO = a.CONTRACT_NO,
                        QUOTE_NO = b.QUOTE_NO,
                        ITEM_TYPE_ID = b.ITEM_TYPE,
                        ACC_ITEM_NAME = string.Empty,
                        PRICE = Convert.ToInt32(b.APPROVE_PRICE),
                        PRICE_CNT = 0,
                        COST = 0,
                        AMOUNT = 0,
                        IS_TAX = "0",
                        DC_CODE = a.DC_CODE,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = c.CUST_CODE,
                        OUTSOURCE_ID = d.OUTSOURCE_ID,
                        CNT_DATE_S = settleDate.AddDays(1).AddMonths(-1),
                        CYCLE_DATE = a.CYCLE_DATE,
                        ITEM_TYPE = b.ITEM_TYPE
                    };
            var result = q.OrderBy(c => c.CONTRACT_NO).ToList();
            var rI = 1;
            foreach (var item in result)
            {
                item.ROWNUM = rI;
                rI++;
            }
            return result.AsQueryable();
        }
    }
}
