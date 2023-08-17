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
	public partial class F053201Repository : RepositoryBase<F053201, Wms3plDbContext, F053201Repository>
	{
		public F053201 GetDataByF0531AndF0701Id(long f0531_ID, long f0701_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });
			sqlParameter.Add(new SqlParameter("@p1", f0701_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT TOP(1) * FROM F053201
						 WHERE F0531_ID = @p0
						   AND F0701_ID = @p1
						";

			return SqlQuery<F053201>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public IQueryable<long> GetBindingF0701Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT DISTINCT F0701_ID FROM F053201
						 WHERE F0531_ID = @p0 ";

			return SqlQuery<long>(sql, sqlParameter.ToArray());
		}

		public IQueryable<long> GetF053201IdsByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new List<SqlParameter>();
			parms.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT ID
                      FROM F053201
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3 ";
			return SqlQuery<long>(sql, parms.ToArray());
		}

		public void UpdateStatusByIds(List<long> ids, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });

			var sql = @" UPDATE F053201 SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
                     WHERE 1 = 0 ";
			sql += sqlParameter.CombineSqlInParameters(" OR ID ", ids, SqlDbType.BigInt);

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F053201> GetDatasByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT * FROM F053201
						 WHERE F0531_ID = @p0
						";

			return SqlQuery<F053201>(sql, sqlParameter.ToArray());
		}

		/// <summary>
		/// 刪除揀貨容器與跨庫/取消訂單容器的綁定
		/// </summary>
		/// <param name="ids"></param>
		/// <param name="status"></param>
		public void DeleteBindingContainer(long f0531_ID, long f0701_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });
			sqlParameter.Add(new SqlParameter("@p1", f0701_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"DELETE F053201
						 WHERE F0531_ID = @p0
						   AND F0701_ID = @p1
						";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<long> GeF0531IdsByF0701Id(long f0701_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT F0531_ID FROM F053201
						 WHERE F0701_ID = @p0
						";

			return SqlQuery<long>(sql, sqlParameter.ToArray());
		}
	}
}
