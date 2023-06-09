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
    public partial class F910004Repository : RepositoryBase<F910004, Wms3plDbContext, F910004Repository>
    {
        public F910004Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 列出此物流中心設定的生產線清單與其目前未結案工單數、加工數量
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public IQueryable<F910004Data> GetF910004Data(string dcCode)
        {
            //未結案工單數
            var notFinishedStatuses = new List<string>() { "0", "1" };
            var joinData = from a in _db.F910203s
                           join b in _db.F910201s on new { a.PROCESS_NO, a.DC_CODE, a.CUST_CODE, a.GUP_CODE }
                           equals new { b.PROCESS_NO, b.DC_CODE, b.CUST_CODE, b.GUP_CODE }
                           where notFinishedStatuses.Contains(b.STATUS)
                           group new { a.DC_CODE, a.PRODUCE_NO, b } by
                           new
                           {
                               a.DC_CODE,
                               a.PRODUCE_NO
                           } into g
                           select new
                           {
                               DC_CODE = g.Key.DC_CODE,
                               PRODUCE_NO = g.Key.PRODUCE_NO,
                               SUMPROCQTY = g.Sum(c => c.b.PROCESS_QTY),
                               PROCCNT = g.Select(c => c.b.PROCESS_NO).Distinct().Count()
                           };

            var q = from a in _db.F910004s
                    join b in joinData on new { a.DC_CODE, a.PRODUCE_NO } equals new { b.DC_CODE, b.PRODUCE_NO } into bb
                    from b in bb.DefaultIfEmpty()
                    where a.DC_CODE == dcCode
                    orderby a.PRODUCE_NO
                    select new F910004Data
                    {
                        PRODUCE_NO = a.PRODUCE_NO,
                        PRODUCE_NAME = a.PRODUCE_NAME,
                        PRODUCE_DESC = a.PRODUCE_DESC,
                        PROCCNT = (int?)b.PROCCNT ?? 0,
                        SUMPROCQTY = (int?)b.SUMPROCQTY ?? 0
                    };
            return q;
        }
    }
}
