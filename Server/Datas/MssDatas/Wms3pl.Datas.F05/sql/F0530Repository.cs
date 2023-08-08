using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0530Repository : RepositoryBase<F0530, Wms3plDbContext, F0530Repository>
	{
		public IQueryable<F0530> GetF0530s(List<long> f0701_IDs)
		{
			var sqlParameter = new List<SqlParameter>();

			var sql = @"SELECT * FROM F0530
						WHERE 
						";

			sql += sqlParameter.CombineSqlInParameters(" F0701_ID ", f0701_IDs, SqlDbType.BigInt);

			return SqlQuery<F0530>(sql, sqlParameter.ToArray());
		}

		public IQueryable<F0530> GetF0530s(string dcCode, string gupCode, string custCode, List<long> f0701_IDs)
		{

			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode));
			sqlParameter.Add(new SqlParameter("@p1", gupCode));
			sqlParameter.Add(new SqlParameter("@p2", custCode));

			var sql = @"SELECT * FROM F0530
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
						";

			sql += sqlParameter.CombineSqlInParameters(" AND F0701_ID ", f0701_IDs, SqlDbType.BigInt);

			return SqlQuery<F0530>(sql, sqlParameter.ToArray());
		}

		/// <summary>
		/// 查詢已在稽核出庫作業的數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNos"></param>
		/// <returns></returns>
		public int GetMoveOutCountByPickOrdNo(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType .VarChar){ Value = custCode}
			};

			var sql = $@" 
						SELECT COUNT(*) FROM F0530
						 WHERE DC_CODE = @p0
						   AND GUP_CODE = @p1
						   AND CUST_CODE = @p2
						   AND WORK_TYPE = '1'
						";
			sql += param.CombineSqlInParameters(" AND PICK_ORD_NO ", pickOrdNos, SqlDbType.VarChar);

			return SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();
		}

		public void DeleteByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" DELETE F0530
					  WHERE F0701_ID IN (SELECT F0701_ID FROM F053201 WHERE F0531_ID = @p0) ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public void DeleteByF0701Id(long f0701_Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @" DELETE F0530
					  WHERE F0701_ID = @p0 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}
	}
}
