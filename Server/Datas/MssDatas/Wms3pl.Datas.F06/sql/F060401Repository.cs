using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060401Repository
	{
		public void UpdateF060401(List<string> docIds)
		{
			if (docIds != null && docIds.Any())
			{
				var param = new List<object> { Current.Staff, Current.StaffName };
				var sql = @"UPDATE F060401 SET STATUS = '0', UPD_DATE = dbo.GetSysDate(), UPD_STAFF = @p0, UPD_NAME = @p1 WHERE 1=1";

				sql += param.CombineSqlInParameters(" AND DOC_ID", docIds);

				ExecuteSqlCommand(sql, param.ToArray());
			}
		}

		public IQueryable<TaskDispatchData> GetF060401Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate,string status,List<string> docNums, List<string> taskNums)
		{
			var sql = @"SELECT TOP 100
						A.DOC_ID,
						A.WMS_NO,
						(SELECT TOP 1 WAREHOUSE_NAME  FROM F1980 WHERE WAREHOUSE_ID= A.WAREHOUSE_ID) WAREHOUSE_NAME,
						(SELECT TOP 1 NAME FROM F000904 WHERE TOPIC = 'F060401' AND SUBTOPIC = 'CMD_TYPE' AND VALUE = A.CMD_TYPE) CMD_TYPE_NAME,
						A.STATUS,
						(SELECT TOP 1 NAME FROM F000904 WHERE TOPIC = 'P2116020000' AND SUBTOPIC = 'STATUS' AND VALUE = A.STATUS) STATUS_NAME,
						A.PROC_DATE,
						A.MESSAGE ,
						A.RESENT_CNT,
						A.CRT_DATE ,
						A.UPD_DATE,
						(CASE A.ISSECOND WHEN '0' THEN '否' WHEN '1' THEN '是' END) ISSECOND
						FROM F060401 A WHERE A.DC_CODE = @p0
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

		public IQueryable<F060401> GetMaxDocId(List<string> wmsNos)
		{

			if (wmsNos.Any() || wmsNos != null)
			{
				var param = new List<object>();
				var sql = @"SELECT * FROM F060401 WHERE DOC_ID IN (SELECT  MAX(A.DOC_ID) DOC_ID FROM (SELECT B.WMS_NO,B.DOC_ID  FROM F060401 B WHERE 1=1";
				sql += param.CombineSqlInParameters(" AND B.WMS_NO", wmsNos);
				sql += @" ) A GROUP BY A.WMS_NO)";
				return SqlQuery<F060401>(sql, param.ToArray());
			}
			else
			{
				return new List<F060401>().AsQueryable();
			}


		}

		public IQueryable<string> GetWmsNoByWmsNo(List<string> docIds)
		{

			if (docIds.Any() || docIds != null)
			{
				var param = new List<object>();
				var sql = @"SELECT WMS_NO FROM F060401 WHERE 1=1";
				sql += param.CombineSqlInParameters(" AND DOC_ID", docIds);
				return SqlQuery<string>(sql, param.ToArray()).Distinct();
			}
			else
			{
				return new List<string>().AsQueryable();
			}
		}
	}
}
