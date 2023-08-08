using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0535Repository : RepositoryBase<F0535, Wms3plDbContext, F0535Repository>
	{
		public void UpdateStatusByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p5", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p6", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p7", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" UPDATE F0535
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE DC_CODE = @p4 AND GUP_CODE = @p5 AND CUST_CODE = @p6 AND PICK_ORD_NO = @p7 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F0535_NotDebitOrder> GetNotDebitOrder()
		{
			var sql = @" SELECT TOP(500) DC_CODE, GUP_CODE, CUST_CODE, WMS_ORD_NO
						   FROM F0535
						  GROUP BY DC_CODE, GUP_CODE, CUST_CODE, WMS_ORD_NO
						 HAVING MIN(STATUS) = '1'
						";
			return SqlQuery<F0535_NotDebitOrder>(sql);
		}

		public void UpdateStatusByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p5", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p6", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p7", wmsOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" UPDATE F0535
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE DC_CODE = @p4 AND GUP_CODE = @p5 AND CUST_CODE = @p6 AND WMS_ORD_NO = @p7 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<string> CheckPickOrdNoIsCancel(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT F050801.WMS_ORD_NO
						   FROM F0535
						   JOIN F050801 ON F050801.DC_CODE = F0535.DC_CODE
									   AND F050801.GUP_CODE = F0535.GUP_CODE
									   AND F050801.CUST_CODE = F0535.CUST_CODE
									   AND F050801.WMS_ORD_NO = F0535.WMS_ORD_NO
						  WHERE F0535.DC_CODE = @p0
						    AND F0535.GUP_CODE = @p1
						    AND F0535.CUST_CODE = @p2
						    AND F0535.PICK_ORD_NO = @p3
						    AND F050801.STATUS  = '9'
						";
			return SqlQuery<string>(sql, sqlParameter.ToArray());
		}
	}
}
