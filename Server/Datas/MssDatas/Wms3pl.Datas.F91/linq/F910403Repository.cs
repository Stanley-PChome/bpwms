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
    public partial class F910403Repository : RepositoryBase<F910403, Wms3plDbContext, F910403Repository>
    {
        public F910403Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

        /// <summary>
		/// 以報價單主檔取得除了 F910403 欄位外，也帶出商品名稱跟大分類名稱
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910403Data> GetF910403DataByQuoteNo(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = _db.F910403s
                .Join(_db.F1903s, a => new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE },
                                    b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE },
                                    (a, b) => new { f910403 = a, f1903 = b })
                .Join(_db.F1915s, a => new { p0 = a.f1903.LTYPE, p1 = a.f1903.GUP_CODE },
                                    c => new { p0 = c.ACODE, p1 = c.GUP_CODE },
                                    (a, c) => new { a, f1915 = c })
                .Where(c => c.a.f910403.DC_CODE == dcCode
                            && c.a.f910403.GUP_CODE == gupCode
                            && c.a.f910403.QUOTE_NO == quoteNo
                            && c.a.f910403.CUST_CODE == custCode)
                .Select(s => new F910403Data
                {
                    QUOTE_NO = s.a.f910403.QUOTE_NO,
                    ITEM_CODE =s.a.f910403.ITEM_CODE,
                    UNIT_ID = s.a.f910403.UNIT_ID,
                    STANDARD = s.a.f910403.STANDARD,
                    STANDARD_COST = s.a.f910403.STANDARD_COST,
                    DC_CODE =s.a.f910403.DC_CODE,
                    GUP_CODE = s.a.f910403.GUP_CODE,
                    CRT_STAFF = s.a.f910403.CRT_STAFF,
                    CRT_DATE = s.a.f910403.CRT_DATE,
                    CRT_NAME = s.a.f910403.CRT_NAME,
                    UPD_STAFF = s.a.f910403.CRT_STAFF,
                    UPD_DATE = s.a.f910403.UPD_DATE,
                    UPD_NAME = s.a.f910403.UPD_NAME,
                    ITEM_NAME = s.a.f1903.ITEM_NAME,
                    CLA_NAME = s.f1915.CLA_NAME,
                });
            return q;
        }

        /// <summary>
        /// 取得報價單耗材項目報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="quoteNo"></param>
        /// <returns></returns>
        public IQueryable<F910403Report> GetF910403Reports(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = _db.F910401s
                    .Join(_db.F910403s,
                        a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.QUOTE_NO },
                        b => new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.QUOTE_NO },
                        (a, b) => new { f910401 = a, f910403 = b })
                    .Join(_db.F91000302s,
                        a => new { p0 = a.f910403.UNIT_ID, p1 = "001" },
                        b => new { p0 = b.ACC_UNIT, p1 = b.ITEM_TYPE_ID },
                        (a, b) => new { a, f91000302 = b })
                    .Join(_db.F1903s,
                        a => new { a.a.f910403.GUP_CODE, a.a.f910403.CUST_CODE, a.a.f910403.ITEM_CODE },
                        b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE },
                        (a, b) => new { a, f1903 = b })
                    .Where(s => s.a.a.f910401.DC_CODE == dcCode
                                && s.a.a.f910401.GUP_CODE == gupCode
                                && s.a.a.f910401.CUST_CODE == custCode
                                && s.a.a.f910401.QUOTE_NO == quoteNo)
                    .Select(c => new F910403Report
                    {
                        ITEM_CODE = c.a.a.f910403.ITEM_CODE,
                        ITEM_NAME = c.f1903.ITEM_NAME,
                        UNIT = c.a.f91000302.ACC_UNIT_NAME
                    })
                    .OrderBy(o => o.ITEM_CODE);
            

            return q;
        }

        /// <summary>
        /// 取得耗材統計
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="quoteNo"></param>
        /// <returns></returns>
        public IQueryable<F910403Detail> GetF910403Detail(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = _db.F910403s
                .GroupJoin(_db.F1903s,
                            a => new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE },
                            b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE },
                            (a, b) => new { a, b })
                .SelectMany(x => x.b.DefaultIfEmpty(),(x,b) => new { f910403 = x.a, f1903 = b })
                .GroupJoin(_db.F91000302s,
                            a => new { p0 = a.f910403.UNIT_ID, p1 = "001" },
                            b => new { p0 = b.ACC_UNIT, p1 = b.ITEM_TYPE_ID },
                            (a, b) => new { a, b })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x,b) => new { a = x.a, f91000302 = b })
                .GroupJoin(_db.F1915s,
                            a => new { p0 = a.a.f1903.LTYPE, p1 = a.a.f1903.GUP_CODE, p2 = a.a.f910403.CUST_CODE },
                            b => new { p0 = b.ACODE, p1 = b.GUP_CODE, p2 = b.CUST_CODE },
                            (a, b) => new { a, b })
                .SelectMany(x => x.b.DefaultIfEmpty() , (x,b) => new { x.a, f1915 = b })
                .Where(c => c.a.a.f910403.DC_CODE == dcCode
                            && c.a.a.f910403.GUP_CODE == gupCode
                            && c.a.a.f910403.CUST_CODE == custCode
                            && c.a.a.f910403.QUOTE_NO == quoteNo)
                .Select(s => new F910403Detail
                {
                    QUOTE_NO = s.a.a.f910403.QUOTE_NO,
                    ITEM_CODE = s.a.a.f910403.ITEM_CODE,
                    UNIT_ID = s.a.a.f910403.UNIT_ID,
                    STANDARD = s.a.a.f910403.STANDARD.HasValue ? s.a.a.f910403.STANDARD.Value : 0,
                    STANDARD_COST = Convert.ToDouble(s.a.a.f910403.STANDARD_COST),
                    DC_CODE = s.a.a.f910403.DC_CODE,
                    GUP_CODE = s.a.a.f910403.GUP_CODE,
                    ITEM_NAME = s.a.a.f1903.ITEM_NAME,
                    UNIT = s.a.f91000302.ACC_UNIT_NAME,
                    LTYPE = s.f1915.CLA_NAME
                });

            return q;
        }
    }
}
