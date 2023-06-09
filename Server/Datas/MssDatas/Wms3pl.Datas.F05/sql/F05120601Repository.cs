using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F05120601Repository : RepositoryBase<F05120601, Wms3plDbContext, F05120601Repository>
	{
		public void UpdateNotAllotStatus(string queryStatus,string updStatus)
		{
			var parms = new object[] { updStatus, DateTime.Now, Current.Staff, Current.StaffName, queryStatus };
			var sql = " UPDATE F05120601 SET STATUS = @p0,UPD_DATE = @p1,UPD_STAFF =@p2,UPD_NAME = @p3 WHERE STATUS = @p4 ";
			ExecuteSqlCommand(sql, parms);
		}

		public IQueryable<F05120601> GetPickLackAllotDatas()
		{
			var sql = @" SELECT *
                     FROM F05120601
                    WHERE STATUS = '1'
                    ORDER BY FAST_DEAL_TYPE DESC,PICK_ORD_NO ASC";
			return SqlQuery<F05120601>(sql);
		}

		public void DeleteByIds(List<Int64> ids)
		{
			var parms = new List<object>();
			if (!ids.Any())
				return;
			var sql = @" DELETE FROM F05120601 WHERE 1=1 ";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND ID", ids);
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public IQueryable<F05120601> GetCollectionOutboundDatas()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F05120601 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_ORD_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									) 
									";

			var result = SqlQuery<F05120601>(sql, parms.ToArray());
			return result;
		}

    public IQueryable<F05120601> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
      };

      var sql = @"SELECT * FROM F05120601 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND WMS_ORD_NO=@p3";
      var result = SqlQuery<F05120601>(sql, param.ToArray());
      return result;
    }

    public IQueryable<F05120601> GetDatasByPickOrdNo(string dcCode, string gupCode, string custCode, string pickNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", pickNo) { SqlDbType = System.Data.SqlDbType.VarChar },
      };

      var sql = @"SELECT * FROM F05120601 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND PICK_ORD_NO=@p3";
      var result = SqlQuery<F05120601>(sql, param.ToArray());
      return result;
    }
  }
}
