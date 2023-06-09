using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.F06
{
	public partial class F060802Repository
    {
		#region 分揀異常回報查詢
		public IQueryable<F060802Data> GetF060802Data(string dcCode, string gupCode, string custCode, DateTime? beginCreatDate, DateTime? endCreatDate, string sorterCode,string abnormalCode, string abnormalType)
		{
			var sql = @"SELECT A.ID,
							A.SORTER_CODE,
							(SELECT TOP 1 NAME FROM F000904 WHERE TOPIC ='F060802' AND SUBTOPIC ='ABNORMAL_TYPE' AND VALUE = A.ABNORMAL_TYPE) ABNORMAL_TYPE_NAME,
							A.RECORD_TIME,
							A.ABNORMAL_MSG,
							A.ABNORMAL_CODE,
							A.CRT_DATE  FROM F060802 A 
							WHERE A.DC_CODE = @p0
							AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2";

			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
			};

			if (beginCreatDate.HasValue)
			{
				sql += " AND A.CRT_DATE >= @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = beginCreatDate });
			}
			if (endCreatDate.HasValue)
			{
				sql += " AND A.CRT_DATE < @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = endCreatDate?.AddDays(1) });
			}
			if (!string.IsNullOrWhiteSpace(sorterCode))
			{
				sql += " AND A.SORTER_CODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = sorterCode });
			}
			if (!string.IsNullOrWhiteSpace(abnormalCode))
			{
				sql += " AND A.ABNORMAL_CODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = abnormalCode });
			}
			if (!string.IsNullOrWhiteSpace(abnormalType))
			{
				sql += " AND A.ABNORMAL_TYPE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Int) { Value = abnormalType });
			}

			var result = SqlQuery<F060802Data>(sql, param.ToArray());

			return result;
		}
		#endregion
	}
}