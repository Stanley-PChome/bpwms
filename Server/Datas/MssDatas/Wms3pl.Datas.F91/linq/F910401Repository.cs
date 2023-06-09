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
    public partial class F910401Repository : RepositoryBase<F910401, Wms3plDbContext, F910401Repository>
    {
        public F910401Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

        /// <summary>
		/// 取得報價單報表主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910401Report> GetF910401Report(string dcCode, string gupCode, string custCode, string quoteNo)
        {
            var q = _db.F910401s.Join(_db.F1928s, a => a.OUTSOURCE_ID, b => b.OUTSOURCE_ID, (a, b) => new
            {
                a,
                b
            }).Where(c => c.a.QUOTE_NO == quoteNo && c.a.DC_CODE == dcCode && c.a.CUST_CODE == custCode)
            .Select(s => new F910401Report
            {
                QUOTE_NO = s.a.QUOTE_NO,
                QUOTE_NAME = s.a.QUOTE_NAME,
                APPLY_PRICE = s.a.APPLY_PRICE,
                OUTSOURCE_NAME = s.b.OUTSOURCE_NAME,
                CONTACT = s.b.CONTACT
            }).Take(1);
            return q;
        }       
    }
}
