using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1903_ASYNCRepository : RepositoryBase<F1903_ASYNC, Wms3plDbContext, F1903_ASYNCRepository>
	{
		public IQueryable<F1903_ASYNC> GetDatas(string gupCode, string custCode, int maxRecord)
		{
			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParamers.Add(new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar });

			var sql = $@" SELECT TOP ({maxRecord}) *
                            FROM F1903_ASYNC
                           WHERE GUP_CODE = @p0
                             AND CUST_CODE = @p1
                             AND IS_ASYNC IN ('N', 'F')
                           ORDER BY CRT_DATE";

			return SqlQuery<F1903_ASYNC>(sql, sqlParamers.ToArray());
		}

		public void UpdateIsAsync(string isAsync, string batchNo, string gupCode, string custCode, List<long> f1903_IDs)
		{
			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", isAsync) { SqlDbType = System.Data.SqlDbType.Char });
			sqlParamers.Add(new SqlParameter("@p1", batchNo) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParamers.Add(new SqlParameter("@p2", DateTime.Now) { SqlDbType = System.Data.SqlDbType.DateTime2 });
			sqlParamers.Add(new SqlParameter("@p3", Current.Staff) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParamers.Add(new SqlParameter("@p4", Current.StaffName) { SqlDbType = System.Data.SqlDbType.NVarChar });
			sqlParamers.Add(new SqlParameter("@p5", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParamers.Add(new SqlParameter("@p6", custCode) { SqlDbType = System.Data.SqlDbType.VarChar });

			var sql = @"UPDATE F1903_ASYNC
                           SET IS_ASYNC = @p0,
                               BATCH_NO = @p1,
                               UPD_DATE = @p2,
                               UPD_STAFF = @p3,
                               UPD_NAME = @p4
                         WHERE 1 = 1
                       ";

			sql += sqlParamers.CombineSqlInParameters(" AND ID", f1903_IDs, System.Data.SqlDbType.BigInt);
			ExecuteSqlCommand(sql, sqlParamers.ToArray());
		}

		public bool CheckItemAsync(string gupCode, string custCode, List<string> itemCodes)
		{
			var sql = @"SELECT TOP(1) 1 FROM F1903_ASYNC
                         WHERE GUP_CODE = @p0
                           AND CUST_CODE = @p1
                           AND IS_ASYNC IN ('N', 'F')";

			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
			};

			sql += param.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, System.Data.SqlDbType.VarChar);

			var result = SqlQuery<int>(sql, param.ToArray());

			if (result != null && result.Any())
				return false;

			return true;
		}
	}
}
