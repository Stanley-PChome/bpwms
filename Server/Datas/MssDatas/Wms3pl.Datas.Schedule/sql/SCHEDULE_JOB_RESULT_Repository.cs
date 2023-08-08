using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;
using System.Data;

namespace Wms3pl.Datas.Schedule
{
	public partial class SCHEDULE_JOB_RESULTRepository : RepositoryBase<SCHEDULE_JOB_RESULT, Wms3plDbContext, SCHEDULE_JOB_RESULTRepository>
	{
        public int? InsertLog(string dcCode, string gupCode, string custCode, string scheduleName, string isSuccess, string message)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", scheduleName));
            parm.Add(new SqlParameter("@p4", isSuccess));
            parm.Add(new SqlParameter("@p5", message));
            parm.Add(new SqlParameter("@p6", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            var sql = @" 
                    BEGIN
                    declare @new_id bigint;
                    INSERT INTO SCHEDULE_JOB_RESULT(DC_CODE,GUP_CODE,CUST_CODE,NAME,IS_SUCCESSFUL,PARENT_ID,EXEDATE,MESSAGE,SELECT_DATE)
		                    VALUES(@p0,@p1,@p2,@p3,@p4,null,@p6,@p5,null);
		                    --ALUES(1,1,1,1,1,null,@p6,1,null);
                    SELECT @new_id = CAST(current_value as bigint) 
                    FROM sys.sequences WHERE name = 'SEQ_SCHEDULE_JOB_RESULT_ID'  
                    select @new_id ID;
                    END

                    ;";
            return SqlQuery<int>(sql, parm.ToArray()).Single();
        }
        public void UpdateIsSuccess(int id, string isSuccess, string message = "")
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", id));
            parm.Add(new SqlParameter("@p1", isSuccess));
            parm.Add(new SqlParameter("@p2", message));
            var sql = @" UPDATE SCHEDULE_JOB_RESULT 
                      SET IS_SUCCESSFUL = @p1,
                          MESSAGE = @p2
                    WHERE ID = @p0 ";
            ExecuteSqlCommand(sql, parm.ToArray());
        }

        /// <summary>
        /// 清除log檔紀錄，清除最舊的1000筆
        /// </summary>
        /// <param name="DeleteDate">要清除的日期</param>
        public void RemoveHistroyLog(DateTime DeleteDate)
        {
            var sql = @"
                WITH T
                    AS (SELECT TOP 1000 * FROM SCHEDULE_JOB_RESULT WHERE EXEDATE < @p0 ORDER BY ID)
                DELETE FROM T";
            var para = new List<SqlParameter>()
                    { new SqlParameter("@p0",System.Data.SqlDbType.DateTime2){Value=DeleteDate} };

            ExecuteSqlCommand(sql, para.ToArray());
            if (_wmsTransaction != null)
                _wmsTransaction.Complete();
        }

    }
}
