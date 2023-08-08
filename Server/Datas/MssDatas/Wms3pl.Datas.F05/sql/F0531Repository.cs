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
	public partial class F0531Repository : RepositoryBase<F0531, Wms3plDbContext, F0531Repository>
	{
		public string LockF0531()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0531';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public long GetF0531NextId()
		{
			var sql = @"SELECT NEXT VALUE FOR SEQ_F0531_ID";

			return SqlQuery<long>(sql).Single();
		}

		public OutContainerInfo GetOutContainerInfo(string dcCode, string outContainerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", outContainerCode) { SqlDbType = SqlDbType.VarChar });

			var sql = @"SELECT TOP(1) A.ID AS F0531_ID, A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.OUT_CONTAINER_CODE, A.MOVE_OUT_TARGET, ISNULL(B.CROSS_NAME, '未綁定') AS CROSS_NAME, A.TOTAL, A.WORK_TYPE, A.STATUS, A.CRT_DATE, A.SOW_TYPE
						  FROM F0531 A
						  LEFT JOIN F0001 B ON B.CROSS_CODE = A.MOVE_OUT_TARGET
						 WHERE A.DC_CODE = @p0
						   AND A.OUT_CONTAINER_CODE = @p1
						";
			return SqlQuery<OutContainerInfo>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public F0531 GetDataById(long id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT TOP(1) * FROM F0531
						 WHERE ID = @p0
						";

			return SqlQuery<F0531>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateTotalAndTargetById(long id, int plusQty, string moveOutTarget = "")
		{
			var sqlPlus = string.Empty;
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", plusQty) { SqlDbType = SqlDbType.Int });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", id) { SqlDbType = SqlDbType.BigInt });
			if (!string.IsNullOrWhiteSpace(moveOutTarget))
			{
				sqlParameter.Add(new SqlParameter("@p5", moveOutTarget) { SqlDbType = SqlDbType.VarChar });
				sqlPlus = ", MOVE_OUT_TARGET = @p5 ";
			}

			var sql = $@" UPDATE F0531
							SET TOTAL = ISNULL(TOTAL, 0) + @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3 {sqlPlus}
					  WHERE ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public F0531 GetDataByF0701Id(long f0701_Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT TOP(1) * FROM F0531
						 WHERE F0701_ID = @p0
						";

			return SqlQuery<F0531>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateContainerCodeById(long id, string newContainerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", newContainerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", id) { SqlDbType = SqlDbType.BigInt });

			var sql = $@" UPDATE F0531
							SET OUT_CONTAINER_CODE = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public F0531 GetDataByContainerCode(string dcCode, string containerCode, string sowType)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", containerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", sowType) { SqlDbType = SqlDbType.Char });

			var sql = @"SELECT TOP(1) * FROM F0531
						 WHERE DC_CODE = @p0
						   AND OUT_CONTAINER_CODE = @p1
						   AND SOW_TYPE = @p2
						";

			return SqlQuery<F0531>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}
	}
}
