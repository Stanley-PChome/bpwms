using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F053203Repository : RepositoryBase<F053203, Wms3plDbContext, F053203Repository>
	{
		public IQueryable<F053203> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var parms = new List<SqlParameter>();
			parms.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p3", wmsOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT *
                      FROM F053203
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND WMS_ORD_NO = @p3 ";
			return SqlQuery<F053203>(sql, parms.ToArray());
		}
	}
}
