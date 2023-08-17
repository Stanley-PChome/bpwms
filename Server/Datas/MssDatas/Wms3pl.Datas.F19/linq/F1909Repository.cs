using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1909Repository : RepositoryBase<F1909, Wms3plDbContext, F1909Repository>
	{
		public F1909Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F1909> GetDatasByDc(string dcCode)
		{
            return _db.F1909s
                    .Join(_db.F190101s, a => new { a.GUP_CODE, a.CUST_CODE }, b => new { b.GUP_CODE, b.CUST_CODE }, (a, b) => new { a, b })
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Select(x => x.a);
		}

		public IQueryable<F1909> GetF1909Datas(string gupCode, string custName, string account)
		{
            var query = _db.F1909s.Where(x => x.STATUS != "9");
			if (!string.IsNullOrEmpty(gupCode))
                query = query.Where(x => x.GUP_CODE == gupCode);
			if (!string.IsNullOrEmpty(custName))
                query = query.Where(x => x.CUST_NAME == custName);
            return query.OrderBy(x => x.GUP_CODE).ThenBy(x => x.CUST_CODE)
                    .Select(x => x);
		}

		public IQueryable<F1909> GetAll()
		{
            return _db.F1909s.Select(x=>x);
		}

        public IQueryable<F1909> GetData(string custCode)
        {
            return _db.F1909s.Where(x => x.CUST_CODE == custCode);
        }

        /// <summary>
        /// 取得業主編號
        /// </summary>
        /// <param name="custCode">貨主編號</param>
        /// <returns></returns>
        public string GetGupCode(string custCode)
        {
            var result = _db.F1909s.AsNoTracking().Where(x => x.CUST_CODE == custCode)
                                                  .Select(x=>x.GUP_CODE)
                                                  .FirstOrDefault();
            return result;
        }
    }

}
