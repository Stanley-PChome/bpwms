using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Wms3pl.Datas.F05
{
	public partial class F0501Repository : RepositoryBase<F0501, Wms3plDbContext, F0501Repository>
	{
		public string LockF0501()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0501';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public IQueryable<F0501> GetF0501s(string dcCode,string gupCode,string custCode,List<string> itemCodes)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar}
			};
			var sql = @" SELECT *
                     FROM F0501 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
			if (itemCodes.Any())
				sql += parms.CombineSqlInParameters(" AND ITEM_CODE ", itemCodes, SqlDbType.VarChar);
			else
				sql += " AND 1 = 0 ";
			return SqlQuery<F0501>(sql, parms.ToArray());
		}
		public void UnLockByAllotBatchNo(string allotBatchNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",Current.Staff){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p2",allotBatchNo){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };
			var sql = @"  UPDATE F0501
                    SET IS_LOCK='0',ALLOT_BATCH_NO=NULL,UPD_DATE= @p3,UPD_STAFF=@p0,UPD_NAME=@p1
                    WHERE ALLOT_BATCH_NO = @p2 ";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
		}

		public IQueryable<string> GetNeedUnlockBatchNos()
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", DateTime.Now.AddMinutes(-5)) { SqlDbType = SqlDbType.DateTime2 },
			};
			var sql = @"  SELECT DISTINCT ALLOT_BATCH_NO FROM F0501
                    WHERE IS_LOCK = '1'
                    AND (CASE WHEN UPD_DATE IS NULL THEN CRT_DATE
                    ELSE UPD_DATE END) <= @p0 ";

			return SqlQuery<string>(sql, parms.ToArray());
		}
	}
}
