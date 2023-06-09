using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700706Repository : RepositoryBase<F700706, Wms3plDbContext, F700706Repository>
	{
		/// <summary>
		/// 從行事曆取得定期工作與優化工作執行比例統計
		/// </summary>
		/// <param name="scheduleDate"></param>
		/// <returns></returns>
		public IQueryable<F700706ForSchedule> GetScheduleRefineStatistics(DateTime scheduleDate)
		{
			var sql = @"  SELECT A.DC_CODE,
								 A.SCHEDULE_DATE AS CNT_DATE,
								 
								 (DATEPART (DAY,A.SCHEDULE_DATE) - 1) AS CNT_DAY,
								 ISNULL (COUNT (CASE WHEN A.CRT_STAFF = 'Schedule' THEN 1 END), 0)
									AS TIME_QTY,
								 ISNULL (
									COUNT (
									   CASE WHEN A.CRT_STAFF = 'Schedule' AND A.STATUS = '1' THEN 1 END),
									0)
									AS TIME_FINISH_QTY,
								 ISNULL (COUNT (CASE WHEN A.CRT_STAFF = 'Refine' THEN 1 END), 0)
									AS OPTIMIZE_QTY,
								 ISNULL (
									COUNT (
									   CASE WHEN A.CRT_STAFF = 'Refine' AND A.STATUS = '1' THEN 1 END),
									0)
									AS OPTIMIZE_FINISH_QTY
							FROM F700501 A
						   WHERE A.SCHEDULE_DATE = @p0
						GROUP BY A.DC_CODE,
								 A.SCHEDULE_DATE,
								 (DATEPART (DAY,A.SCHEDULE_DATE) - 1)";

			return SqlQuery<F700706ForSchedule>(sql, new object[] { scheduleDate });
		}
	}
}
