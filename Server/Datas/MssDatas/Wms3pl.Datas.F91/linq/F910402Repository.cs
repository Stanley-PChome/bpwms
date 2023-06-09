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
    public partial class F910402Repository : RepositoryBase<F910402, Wms3plDbContext, F910402Repository>
    {
        public F910402Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

        /// <summary>
        /// 取得報價單動作分析明細報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="quoteNo"></param>
        /// <returns></returns>
        public IQueryable<F910402Report> GetF910402Reports(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = _db.F910401s
                .Join(_db.F910402s,
                a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.QUOTE_NO },
                b => new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.QUOTE_NO },
                (a, b) => new { f910401 = a, f910402 = b })
                .Join(_db.F910001s,
                a => a.f910402.PROCESS_ID,
                b => b.PROCESS_ID,
                (a, b) => new { a, f910001s = b })
                .Join(_db.F91000302s.Where(c => c.ITEM_TYPE_ID == "001"),
                a => new { p0 = a.a.f910402.UNIT_ID },
                b => new { p0 = b.ACC_UNIT },
                (a, b) => new { a, f91000302s = b })
                .Where(c => c.a.a.f910401.DC_CODE == dcCode
                && c.a.a.f910401.GUP_CODE == gupCode
                && c.a.a.f910401.CUST_CODE == custCode
                && c.a.a.f910401.QUOTE_NO == quoteNo)
                .Select(c => new F910402Report
                {
                    PROCESS_ID = c.a.a.f910402.PROCESS_ID,
                    PROCESS_ACT = c.a.f910001s.PROCESS_ACT,
                    UNIT = c.f91000302s.ACC_UNIT_NAME
                })
                .OrderBy(s => s.PROCESS_ID);
            return q;
        }

        /// <summary>
        /// 取得動作分析
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="quoteNo"></param>
        /// <returns></returns>
        public IQueryable<F910402Detail> GetF910402Detail(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = from f910402 in _db.F910402s
            join f910001 in _db.F910001s on f910402.PROCESS_ID equals f910001.PROCESS_ID into b
            from f910001 in b.DefaultIfEmpty()
            join f91000302 in _db.F91000302s on new { p0 = f910402.UNIT_ID, p1 = "001" }
            equals new { p0 = f91000302.ACC_UNIT, p1 = f91000302.ITEM_TYPE_ID } into c
            from f91000302 in c.DefaultIfEmpty()
            where f910402.DC_CODE == dcCode
            && f910402.CUST_CODE == custCode
            && f910402.QUOTE_NO == quoteNo
            select new F910402Detail
            {
                QUOTE_NO = f910402.QUOTE_NO,
                PROCESS_ID = f910402.PROCESS_ID,
                UNIT_ID = f910402.UNIT_ID,
                WORK_HOUR = f910402.WORK_HOUR,
                WORK_COST = Convert.ToDouble(f910402.WORK_COST),
                DC_CODE = f910402.DC_CODE,
                GUP_CODE = f910402.GUP_CODE,
                PROCESS_ACT = f910001 == null ? string.Empty : f910001.PROCESS_ACT,
                UNIT = f91000302 == null ? string.Empty : f91000302.ACC_UNIT_NAME
            };
            
            return q;
        }
    }
}
