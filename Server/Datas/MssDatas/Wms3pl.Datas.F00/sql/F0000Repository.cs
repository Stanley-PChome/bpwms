using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  public partial class F0000Repository : RepositoryBase<F0000, Wms3plDbContext, F0000Repository>
  {
		public Boolean LockTable(string tableName,string apiName)
		{
			var result = UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }),
				 () =>
				 {
					 F0000 resF0000 = new F0000 { ID = -1 };
					 if (LockF0000(tableName, apiName))
						 resF0000.ID = 0;
					 return resF0000;
				 });
			return result.ID == 0;
		}
		/// <summary>
		/// 排程執行鎖定，鎖定成功回傳true
		/// </summary>
		/// <param name="UpdLockTableName"></param>
		/// <param name="UpdLockApiName"></param>
		/// <returns></returns>
		public Boolean LockF0000(string tableName, string apiName)
		{
			var para = new List<SqlParameter>()
			{
				new SqlParameter("@p0",tableName) {SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",apiName) {SqlDbType = SqlDbType.VarChar}
			};
			var sql = @"Select Top 1 * From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME=@p0 AND (UPD_LOCK_API_NAME IS NULL OR UPD_LOCK_API_NAME=@p1) ORDER BY UPD_LOCK_API_NAME DESC;";
			var f0000 = SqlQuery<F0000>(sql, para.ToArray()).FirstOrDefault();
			if (f0000!= null && (string.IsNullOrWhiteSpace(f0000.IS_LOCK) || f0000.IS_LOCK == "0"))
			{
				f0000.IS_LOCK = "1";
				var sql2 = @"UPDATE F0000 SET IS_LOCK='1',UPD_DATE=dbo.GetSysDate(),UPD_STAFF=@p0,UPD_NAME=@p1
                     WHERE ID = @p2 ";
				var parm2 = new List<SqlParameter>
				{
					new SqlParameter("@p0",Current.Staff){SqlDbType = SqlDbType.VarChar},
					new SqlParameter("@p1",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
					new SqlParameter("@p2",f0000.ID){SqlDbType = SqlDbType.BigInt}
				};
				ExecuteSqlCommand(sql2, parm2.ToArray());
				return true;
			}
			return false;
		}


		public void UnlockTable(string tableName, string apiName)
		{
			var para = new List<SqlParameter>()
			{
				new SqlParameter("@p0",Current.Staff){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p1",Current.StaffName){SqlDbType=SqlDbType.NVarChar},
				new SqlParameter("@p2",tableName){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p3",apiName){SqlDbType=SqlDbType.VarChar}
			};
			var sql = @" UPDATE F0000 SET IS_LOCK='0',UPD_DATE=dbo.GetSysDate(),UPD_STAFF=@p0,UPD_NAME=@p1 
                   WHERE UPD_LOCK_TABLE_NAME = @p2 AND UPD_LOCK_API_NAME = @p3 ";
			ExecuteSqlCommand(sql, para.ToArray());
		}

		public IQueryable<F0000> GetLockDatas()
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",DateTime.Now.AddMinutes(-15)) { SqlDbType = SqlDbType.DateTime2 },
      };
      var sql = @"SELECT * From F0000 WHERE IS_LOCK='1' AND UPD_DATE<@p0";
      return SqlQuery<F0000>(sql, para.ToArray());
    }

    public IQueryable<decimal> GetTableSeqId(string tableSeqId)
    {
      var sql = string.Format(" SELECT NEXT VALUE FOR [dbo].[{0}]", tableSeqId);
      return SqlQuery<decimal>(sql);
    }
  }
}
