using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F195302Repository : RepositoryBase<F195302, Wms3plDbContext, F195302Repository>
	{
		public void Delete(decimal grpId, string scheduleId = "")
		{
			var sql = @"
                        DELETE F195302
                        WHERE  GRP_ID = @p0
                               AND ( CASE
                                       WHEN @p1 = '' THEN '1'
                                       ELSE SCHEDULE_ID
                                     END ) = ( CASE
                                                 WHEN @p1 = '' THEN '1'
                                                 ELSE @p1
                                               END ) 
                        ";

			var paramers = new[]
			{
								new SqlParameter("@p0", grpId),
								new SqlParameter("@p1", scheduleId ?? string.Empty)
						};

			ExecuteSqlCommand(sql, paramers);
		}

		/// <summary>
		/// 刪除未勾選的資料
		/// </summary>
		/// <param name="grpId"></param>
		/// <param name="scheduleIdList"></param>
		public void BulkDeleteByNotCheckDatas(decimal grpId, List<string> scheduleIdList)
		{
				var parameters = new List<object> { grpId };

				var sql = $@" DELETE FROM F195302
                    WHERE GRP_ID = @p0 ";

				sql += parameters.CombineSqlNotInParameters(" AND SCHEDULE_ID", scheduleIdList);

				ExecuteSqlCommand(sql, parameters.ToArray());
		}
	}
}
