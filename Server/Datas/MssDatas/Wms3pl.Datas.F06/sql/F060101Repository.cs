using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
  public partial class F060101Repository
  {
    public AllocTicketReportModel GetDataByTicketReport(string dcCode, string gupCode, string custCode, string allocationNo, Boolean isInProc)
    {
      var table = isInProc ? "F060101" : "F060201";
      var sql =
          $@"   SELECT
                     TOP 1 
                     A.DOC_ID,
                     A.WAREHOUSE_ID,
                     B.WAREHOUSE_NAME
                     FROM {table} A
                     JOIN F1980 B
                     ON A.DC_CODE = B.DC_CODE
                     AND A.WAREHOUSE_ID = B.WAREHOUSE_ID
                     WHERE A.DC_CODE = @p0
                     AND A.GUP_CODE = @p1
                     AND A.CUST_CODE = @p2
                     AND A.WMS_NO = @p3
                     AND A.CMD_TYPE = '1'
                     AND A.STATUS IN ('0', '1', '2', '3')
                     ORDER BY A.CRT_DATE DESC ";
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3",SqlDbType.VarChar) { Value = allocationNo },
      };
      return SqlQuery<AllocTicketReportModel>(sql, param.ToArray()).FirstOrDefault();
    }

		public IQueryable<TaskDispatchData> GetF060101Data(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var sql = @"SELECT TOP 100
						A.DOC_ID,
						A.WMS_NO ,
						(SELECT TOP 1 WAREHOUSE_NAME  FROM F1980 WHERE WAREHOUSE_ID= A.WAREHOUSE_ID AND DC_CODE = A.DC_CODE) WAREHOUSE_NAME,
						(SELECT TOP 1 NAME FROM F000904 WHERE TOPIC = 'F060101' AND SUBTOPIC = 'CMD_TYPE' AND VALUE = A.CMD_TYPE) CMD_TYPE_NAME,
						A.STATUS,
						(SELECT TOP 1 NAME FROM F000904 WHERE TOPIC = 'P2116020000' AND SUBTOPIC = 'STATUS' AND VALUE = A.STATUS) STATUS_NAME,
						A.PROC_DATE,
						A.MESSAGE ,
						A.RESENT_CNT,
						A.CRT_DATE ,
						A.UPD_DATE 
						FROM F060101 A WHERE DC_CODE = @p0
						AND A.GUP_CODE = @p1
						AND A.CUST_CODE = @p2";
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode }
			};
			if (beginCreateDate.HasValue)
			{
				sql += " AND A.CRT_DATE >= @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = beginCreateDate });
			}
			if (endCreateDate.HasValue)
			{
				sql += " AND A.CRT_DATE < @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = endCreateDate?.AddDays(1) });
			}
			if (!string.IsNullOrWhiteSpace(status))
			{
				sql += " AND A.STATUS = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = status });
			}
			if (docNums != null && docNums.Any())
			{
				sql += $" AND  A.WMS_NO IN (@p{string.Join(",@p", docNums.Select((x, index) => index + param.Count()))})";
				param.AddRange(docNums.Select(x => new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = x }));
			}
			if (taskNums != null && taskNums.Any())
			{
				sql += $" AND A.DOC_ID IN (@p{string.Join(",@p", taskNums.Select((x, index) => index + param.Count()))})";
				param.AddRange(taskNums.Select(x => new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = x }));
			}

			sql += " ORDER BY A.DOC_ID DESC";

			var result = SqlQuery<TaskDispatchData>(sql, param.ToArray());

			return result;
		}
		
		public void UpdateF060101(List<string> docIds)
		{
			if(docIds != null && docIds.Any())
			{
				var param = new List<object> { DateTime.Now, Current.Staff, Current.StaffName };

				var sql = @"UPDATE F060101 SET STATUS = '0', UPD_DATE = @p0, UPD_STAFF = @p1, UPD_NAME = @p2 WHERE 1=1";

				sql += param.CombineSqlInParameters(" AND DOC_ID", docIds);

				ExecuteSqlCommand(sql, param.ToArray());
			}
		}

		public IQueryable<F060101> GetF060101MaxDocId(string docIds)
		{
			var param = new List<object> { docIds.Substring(0,15) };
			var sql = @"SELECT TOP 1 * FROM F060101 WHERE DOC_ID = @p0 ORDER BY CRT_DATE DESC";
			return SqlQuery<F060101>(sql, param.ToArray());
		}

		public IQueryable<F060101> GetMaxDocId(List<string> wmsNos)
		{
			
			if (wmsNos.Any()|| wmsNos != null)
			{
				var param = new List<object>();
				var sql = @"SELECT * FROM F060101 WHERE DOC_ID IN (SELECT  MAX(A.DOC_ID) DOC_ID FROM (SELECT B.WMS_NO,B.DOC_ID  FROM F060101 B WHERE 1=1";
				sql += param.CombineSqlInParameters(" AND B.WMS_NO", wmsNos);
				sql += @" ) A GROUP BY A.WMS_NO)";
				return SqlQuery<F060101>(sql, param.ToArray());
			}
			else
			{
				return new List<F060101>().AsQueryable();
			}
			
			
		}

		public IQueryable<string> GetWmsNoByWmsNo(List<string> docIds)
		{
			
			if(docIds.Any() || docIds != null)
			{
				var param = new List<object>();
				var sql = @"SELECT WMS_NO FROM F060101 WHERE 1=1";
				sql += param.CombineSqlInParameters(" AND DOC_ID", docIds);
				return SqlQuery<string>(sql, param.ToArray()).Distinct();
			}
			else
			{
				return new List<string>().AsQueryable();
			}
		}

		public void UpdateCanSendToProcess(string dcCode, string gupCode, string custCode, int topRecord, string cmdType)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){Value = custCode},
				new SqlParameter("@p3",SqlDbType.VarChar){Value = cmdType},
			};
			var sql = $@" UPDATE F060101 
                     SET STATUS= '1' 
                   WHERE DOC_ID IN(
                   SELECT TOP ({topRecord}) DOC_ID 
                     FROM F060101 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND CMD_TYPE = @p3 
                      AND STATUS IN('0','T')
                    ORDER BY CRT_DATE ) 
                    AND CMD_TYPE =@p3 ";
			ExecuteSqlCommand(sql, parms.ToArray());
		}
		public IQueryable<F060101> GetCanSendCurrentProcessData(string dcCode, string gupCode, string custCode, string cmdType)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){Value = custCode},
				new SqlParameter("@p3",SqlDbType.VarChar){Value = cmdType},
			};
			var sql = @" SELECT *
                     FROM F060101 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND CMD_TYPE = @p3 
                      AND STATUS = '1'
                    ORDER BY CRT_DATE ";
			return SqlQuery<F060101>(sql, parms.ToArray());
		}

		public F060101 GetData(string cmdType, string docId)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = cmdType},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = docId}
			};
			var sql = @" SELECT * 
                     FROM F060101 
                    WHERE CMD_TYPE = @p0
                      AND DOC_ID = @p1 ";
			return SqlQuery<F060101>(sql, parms.ToArray()).FirstOrDefault();
		}

		public void UpdateToCancel(string cmdType, string docId, string status, string message)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.Char){Value = status},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = message},
				new SqlParameter("@p2",SqlDbType.DateTime2){Value = DateTime.Now},
				new SqlParameter("@p3",SqlDbType.VarChar){Value = Current.Staff},
				new SqlParameter("@p4",SqlDbType.NVarChar){Value = Current.StaffName},
				new SqlParameter("@p5",SqlDbType.VarChar){Value = cmdType},
				new SqlParameter("@p6",SqlDbType.VarChar){Value = docId},
			};
			var sql = @"UPDATE F060101 
										SET STATUS= @p0,MESSAGE=@p1,
										UPD_DATE=@p2,UPD_STAFF=@p3,UPD_NAME=@p4
                  WHERE CMD_TYPE = @p5
                    AND DOC_ID = @p6 ";
			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
		}

		public void UpdateExecResult(string cmdType, string docId, string status, string message, DateTime procDate, int resentCnt)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.Char){Value = status},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = message},
				new SqlParameter("@p2",SqlDbType.DateTime2){Value = procDate},
				new SqlParameter("@p3",SqlDbType.Int){Value = resentCnt},
				new SqlParameter("@p4",SqlDbType.DateTime2){Value = DateTime.Now},
				new SqlParameter("@p5",SqlDbType.VarChar){Value = Current.Staff},
				new SqlParameter("@p6",SqlDbType.NVarChar){Value = Current.StaffName},
				new SqlParameter("@p7",SqlDbType.VarChar){Value = cmdType},
				new SqlParameter("@p8",SqlDbType.VarChar){Value = docId},
			};
			var sql = @"UPDATE F060101 
										SET STATUS= @p0,MESSAGE=@p1,PROC_DATE=@p2,RESENT_CNT=@p3,
										UPD_DATE=@p4,UPD_STAFF=@p5,UPD_NAME=@p6
                  WHERE CMD_TYPE = @p7
                    AND DOC_ID = @p8 ";
			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());

		}

    /// <summary>
    /// 取得入庫任務任務ID流水號
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <returns></returns>
    public int GetDocIdCount(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = wmsNo },
      };

      var sql = @"SELECT 
	COUNT(*) 
FROM 
(
  SELECT 1 AS col FROM F060101 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND WMS_NO=@p3
  UNION ALL
  SELECT 1 AS col FROM F060201 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND WMS_NO=@p3
) A";

      return SqlQuery<int>(sql, para.ToArray()).FirstOrDefault();
    }

  }
}
