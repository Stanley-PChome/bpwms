
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F02
{
	public partial class F02020103Repository : RepositoryBase<F02020103, Wms3plDbContext, F02020103Repository>
	{

		public string LockF02020103()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F02020103';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}
		/// <summary>
		/// 刪除日期之前的資料 (不包含當日)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="date"></param>
		public void DeleteBeforeDate(string dcCode, string gupCode, string custCode, DateTime date)
		{
			string sql = @"DELETE F02020103 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND CUR_DATE < @p3";
			var param = new[] {
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", date)
						};
			ExecuteSqlCommand(sql, param);
		}
	}
}
