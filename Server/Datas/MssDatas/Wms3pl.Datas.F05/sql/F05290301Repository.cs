using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F05290301Repository : RepositoryBase<F05290301, Wms3plDbContext, F05290301Repository>
	{
		public IQueryable<F05290301> GetDatasByPick(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
			var sql = @" SELECT  *
                      FROM F05290301
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3";
			return SqlQuery<F05290301>(sql, parms);
		}
		public IQueryable<F05290301> GetDatasByItem(string dcCode, string gupCode, string custCode, string pickOrdNo,List<string> itemCodes)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, pickOrdNo };
			var sql = @" SELECT  *
                      FROM F05290301
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3  ";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND ITEM_CODE", itemCodes);
			sql += " ORDER BY PICK_LOC_NO ";
			return SqlQuery<F05290301>(sql, parms.ToArray());
		}
		

	}
}
