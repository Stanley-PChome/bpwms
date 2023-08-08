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
	public partial class F700501Repository : RepositoryBase<F700501, Wms3plDbContext, F700501Repository>
	{
		public IQueryable<F700501Ex> GetF700501Ex(string dcCode, DateTime dateBegin,
			DateTime dateEnd, string scheduleType)
		{
			var parameters = new List<SqlParameter> 
				{
					new SqlParameter("@p0", dcCode),
					new SqlParameter("@p1", dateBegin),
					new SqlParameter("@p2", dateEnd),
					new SqlParameter("@p3", scheduleType)
				};

			var sql = $@"SELECT TOP 100 a.DC_CODE,a.SCHEDULE_NO,a.SCHEDULE_DATE,a.SCHEDULE_TIME,a.SCHEDULE_TYPE,
								a.IMPORTANCE,a.SUBJECT,a.CONTENT,a.FILE_NAME,
								(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F700501' AND SUBTOPIC='IMPORTANCE' AND
									VALUE = a.IMPORTANCE AND LANG = '{Current.Lang}') AS IMPORTANCE_TEXT,
								(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F700501' AND SUBTOPIC='SCHEDULE_TYPE' AND
									VALUE = a.SCHEDULE_TYPE AND LANG = '{Current.Lang}') AS　SCHEDULE_TYPE_TEXT														
						FROM	F700501 a
						WHERE	a.DC_CODE = @p0 AND
								a.SCHEDULE_TYPE = @p3 AND
								a.SCHEDULE_DATE >= @p1 AND
								a.SCHEDULE_DATE <= @p2
						ORDER BY a.SCHEDULE_DATE DESC,a.SCHEDULE_TIME DESC";

			var result = SqlQuery<F700501Ex>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public void UpdateF700501MessageId(string dcCode , string scheduleNo , decimal messageId)
		{
			var parameters = new List<SqlParameter>{
					new SqlParameter("@p0", dcCode),
					new SqlParameter("@p1", scheduleNo),
					new SqlParameter("@p2", messageId)
				};

			var sql = @"
						UPDATE F700501 
						SET MESSAGE_ID = @p2
						WHERE DC_CODE =@p0 AND SCHEDULE_NO =@p1 
					";

		    ExecuteSqlCommand(sql, parameters.ToArray());
			
		}
	}
}
