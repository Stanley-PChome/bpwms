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
    public partial class F1908Repository : RepositoryBase<F1908, Wms3plDbContext, F1908Repository>
	{
		public F1908Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F1908 GetFirstData(string gupCode)
		{
            var query = _db.F1908s
                        .Where(x => x.GUP_CODE == gupCode);
            return query.OrderBy(x => x.GUP_CODE)
                        .ThenBy(x => x.VNR_CODE)
                        .FirstOrDefault();
		}

		/// <summary>
		/// 取得已過濾人員權限的廠商主檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="vnrCode"></param>
		/// <param name="vnrName"></param>
		/// <returns></returns>
		public IQueryable<F1908> GetAllowedF1908s(string gupCode, string vnrCode, string vnrName, string custCode)
		{
            //命名參考原始碼
            var query = _db.F1908s
                        .Join(_db.F192402s, a => a.GUP_CODE, j => j.GUP_CODE, (a, j) => new { a, j })
                        .Join(_db.F1909s, aj => aj.a.GUP_CODE, b => b.GUP_CODE, (aj, b) => new { aj, b })
                        .Where(x => x.b.CUST_CODE == custCode);
            query = query
                        .Where(x => x.aj.a.STATUS != "9")
                        .Where(x => x.aj.a.GUP_CODE == (string.IsNullOrEmpty(gupCode) ? x.aj.a.GUP_CODE : gupCode))
                        .Where(x => x.aj.a.VNR_CODE == (string.IsNullOrEmpty(vnrCode) ? x.aj.a.VNR_CODE : vnrCode))
                        .Where(x => x.aj.j.EMP_ID != Current.Staff)
                        .Where(x => x.aj.a.CUST_CODE == ((x.b.ALLOWGUP_VNRSHARE == "1") ? "0" : custCode));
            if (!string.IsNullOrEmpty(vnrName))
                query = query.Where(x => x.aj.a.VNR_NAME.Contains(vnrName));

            //原始碼: -- 用 DISTINCT 是因人員權限包含 DC_CODE 可能會重複
            return query
                    .Select(x => x.aj.a).Distinct();
		}

		public F1908 GetEnabledVnrData(string gupCode,string custCode,string vnrCode)
		{
            return _db.F1908s
                    .Where(x => x.GUP_CODE == gupCode
                    && x.CUST_CODE == custCode
                    && x.VNR_CODE == vnrCode
                    && x.STATUS != "*")
                    .Select(x=>x)
                    .FirstOrDefault();
        }
	}
}
