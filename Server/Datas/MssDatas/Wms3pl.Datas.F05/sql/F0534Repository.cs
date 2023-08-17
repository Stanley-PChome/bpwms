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
	public partial class F0534Repository : RepositoryBase<F0534, Wms3plDbContext, F0534Repository>
	{
		public void UpdateCloseInfoByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p3", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0534
							SET STATUS = '1', UPD_DATE = @p0, UPD_STAFF = @p1, UPD_NAME = @p2
					  WHERE F0701_ID IN (SELECT F0701_ID FROM F053201 WHERE F0531_ID = @p3) ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F0534_NotAllotPick> GetNotAllotPick()
		{
			var sql = @" SELECT TOP(500) DC_CODE, GUP_CODE, CUST_CODE, PICK_ORD_NO
						   FROM F0534
						  GROUP BY DC_CODE, GUP_CODE, CUST_CODE, PICK_ORD_NO
						 HAVING MIN(STATUS) IN ('1', '9')
						";
			return SqlQuery<F0534_NotAllotPick>(sql);
		}

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

			var sql = @" UPDATE F0534
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE DC_CODE = @p4 AND GUP_CODE = @p5 AND CUST_CODE = @p6 AND PICK_ORD_NO = @p7 AND STATUS = '1' ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public void UpdateStatusById(long id, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", id) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0534
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public void UpdatePartialCloseByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p3", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0534
							SET STATUS = '1', UPD_DATE = @p0, UPD_STAFF = @p1, UPD_NAME = @p2
					  WHERE F0701_ID IN (SELECT F0701_ID FROM F053201 WHERE F0531_ID = @p3) 
						AND F0701_ID NOT IN (SELECT F0701_ID FROM F0530) 
						AND STATUS = '0' ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public void UpdatePartialCloseByF0701Id(long f0701_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p3", f0701_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0534
							SET STATUS = '1', UPD_DATE = @p0, UPD_STAFF = @p1, UPD_NAME = @p2
					  WHERE F0701_ID = @p3
							AND STATUS = '0' ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}
	}
}
