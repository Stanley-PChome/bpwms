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
	public partial class F1905Repository : RepositoryBase<F1905, Wms3plDbContext, F1905Repository>
	{
		public F1905Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        /// <summary>
        /// 取出紙箱Size
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="searchCode"></param>
        /// <returns></returns>
        public IQueryable<F1905> GetCartonSize(string gupCode, string custCode, string searchCode)
        {
            var query = _db.F1903s.Join(_db.F1905s, a => new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE }, b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE }, (a, b) => new { a, b })
                .Where(x => x.b.GUP_CODE == gupCode)
                .Where(x => x.a.CUST_CODE == custCode)
                .Where(x => x.a.ISCARTON == "1");

            if (!string.IsNullOrEmpty(searchCode))
                query = query.Where(x => x.a.ITEM_CODE == searchCode || x.a.EAN_CODE1 == searchCode || x.a.EAN_CODE2 == searchCode || x.a.EAN_CODE3 == searchCode);

            return query.Select(x => x.b);
        }

        ///// <summary>
        ///// 取出紙箱Size
        ///// </summary>
        ///// <param name="gupCode"></param>
        ///// <param name="custCode"></param>
        ///// <param name="searchCode"></param>
        ///// <returns></returns>
        //public IQueryable<F1905> GetCartonSize(string gupCode,string custCode,string searchCode)
        //{
        //	var parameters = new List<SqlParameter>
        //	{
        //		new SqlParameter(":p0",  gupCode),
        //		new SqlParameter(":p1",  custCode)
        //	};

        //	var sql = @"
        //			SELECT F1905.*
        //				FROM F1903 INNER JOIN F1905
        //								ON     F1903.GUP_CODE = F1905.GUP_CODE
        //									 AND F1903.ITEM_CODE = F1905.ITEM_CODE
        //			 WHERE F1905.GUP_CODE = :p0 AND F1903.CUST_CODE = :p1 AND F1903.ISCARTON = '1' ";

        //	if (!string.IsNullOrEmpty(searchCode))
        //	{
        //		sql = sql + "AND (F1903.ITEM_CODE = :p2 OR F1903.EAN_CODE1 = :p2 OR F1903.EAN_CODE2 = :p2 OR F1903.EAN_CODE3 = :p2)";
        //		parameters.Add(new SqlParameter(":p2", searchCode));
        //	}		

        //	return SqlQuery<F1905>(sql, parameters.ToArray());
        //}

	}
}
