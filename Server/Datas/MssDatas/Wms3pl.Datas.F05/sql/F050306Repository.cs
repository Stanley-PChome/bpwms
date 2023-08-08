using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050306Repository : RepositoryBase<F050306, Wms3plDbContext, F050306Repository>
	{

		public void DeleteByIds(List<Int64> ids)
		{
			if (!ids.Any())
				return;

			var parms = new List<object>();
			var sql = @" DELETE FROM F050306 WHERE 1=1 ";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND ID", ids);
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public IQueryable<F050306> GetDatasBySource(string source, List<string> limitDcList)
		{
			var parms = new List<object> { source };
			var sql = @" SELECT *
                    FROM F050306 
                   WHERE SOURCE = @p0 ";
			if (limitDcList.Any())
				sql += parms.CombineNotNullOrEmptySqlInParameters(" AND DC_CODE", limitDcList);
			return SqlQuery<F050306>(sql, parms.ToArray());
		}

		#region
		/// <summary>
		/// 已配庫但未產生揀貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordType"></param>
		/// <param name="fastDealType"></param>
		/// <param name="custCost"></param>
		/// <param name="ordNo"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="onlyShowMoreThanFourHours"></param>
		/// <returns></returns>
		public IQueryable<NotGeneratedPick> GetNotGeneratedPick(string dcCode, string gupCode, string custCode,
			string ordType, string fastDealType, string custCost, string ordNo, string custOrdNo, bool onlyShowMoreThanFourHours)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
        new SqlParameter("@p3", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };
			var sql = @"SELECT B.ID, C.ORD_DATE ,
							B.WMS_NO ,
							C.CUST_ORD_NO ,
							C.SOURCE_NO ,
							(SELECT TOP(1) NAME  FROM F000904 WHERE TOPIC='F050101' AND SUBTOPIC='CUST_COST' AND VALUE = B.CUST_COST) CUST_COST_NAME ,
							(SELECT TOP(1) NAME  FROM F000904 WHERE TOPIC='F050101' AND SUBTOPIC='FAST_DEAL_TYPE' AND VALUE = B.FAST_DEAL_TYPE) FAST_DEAL_TYPE_NAME ,
							B.MOVE_OUT_TARGET ,
							B.CRT_DATE,
						(CASE WHEN C.CRT_DATE < DateAdd(hour,-4,@p3) THEN '1' ELSE '0' END) MORE_THEN_FOUR_HOURS
						FROM (SELECT * FROM F050306 WHERE ID IN(SELECT MIN(ID) ID FROM (SELECT B.ID, B.DC_CODE, B.GUP_CODE ,B.CUST_CODE ,B.WMS_NO , B.SOURCE_TYPE,B.CRT_DATE FROM f050306 B WHERE 1=1 ) A  
						GROUP BY A.WMS_NO)) B
						JOIN  F050301 C 
						ON B.DC_CODE = C.DC_CODE 
						AND B.GUP_CODE = C.GUP_CODE 
						AND B.CUST_CODE = C.CUST_CODE 
						AND B.WMS_NO = C.ORD_NO 
						WHERE B.SOURCE = '01'
						AND B.DC_CODE = @p0
						AND B.GUP_CODE = @p1
						AND B.CUST_CODE = @p2";

			if (!string.IsNullOrWhiteSpace(ordType))
			{
				sql += $" AND C.ORD_TYPE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = ordType });
			}

			if (!string.IsNullOrWhiteSpace(fastDealType))
			{
				sql += $" AND C.FAST_DEAL_TYPE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = fastDealType });
			}

			if (!string.IsNullOrWhiteSpace(custCost))
			{
				sql += $" AND C.CUST_COST = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.NVarChar) { Value = custCost });
			}

			if (!string.IsNullOrWhiteSpace(ordNo))
			{
				sql += $" AND B.WMS_NO = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = ordNo });
			}

			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sql += $" AND C.CUST_ORD_NO = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = custOrdNo });
			}

			if (onlyShowMoreThanFourHours)
			{
				sql += $" AND B.CRT_DATE < DateAdd(hour,-4,@p{param.Count})";
        param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = DateTime.Now });
      }

      sql += " ORDER BY CRT_DATE";
			
			return SqlQueryWithSqlParameterSetDbType<NotGeneratedPick>(sql, param.ToArray());
		}
		#endregion
	}
}
