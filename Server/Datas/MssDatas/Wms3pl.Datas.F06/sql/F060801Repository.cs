using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.F06
{
	public partial class F060801Repository
	{
		public IQueryable<F060801Data> GetF060801Datas(string dcCode, string gupCode, string custCode, DateTime? beginCrtDate,DateTime? endCrtDate,
			string warehouseId, string abnormaltype, string shelfcode, string ordercode, string bincode, string skucode)
		{
			var sql = @"SELECT 
							ID,
							(SELECT TOP(1) WAREHOUSE_ID +' '+ WAREHOUSE_NAME FROM F1980 WHERE WAREHOUSE_ID = A.WAREHOUSE_ID) WAREHOUSE_ID,
							(SELECT TOP(1) NAME  FROM F000904 WHERE TOPIC = 'F060801' AND SUBTOPIC = 'abnormaltype' AND VALUE = A.ABNORMALTYPE)ABNORMALTYPE_NAME,
							SHELFCODE,
							BINCODE,
							ORDERCODE,
							SKUCODE,
							SKUQTY,
							OPERATOR
						FROM F060801 A WHERE A.DC_CODE = @p0
							AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2";
			List<SqlParameter> param = new List<SqlParameter> {
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode}
			};

			if (beginCrtDate.HasValue)
			{
				sql += " AND A.CRT_DATE >= @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = beginCrtDate });
			}
			if (endCrtDate.HasValue)
			{
				sql += " AND A.CRT_DATE < @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = endCrtDate?.AddDays(1) });
			}
			if (!string.IsNullOrWhiteSpace(warehouseId))
			{
				sql += " AND A.WAREHOUSE_ID = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = warehouseId });
			}
			if (!string.IsNullOrWhiteSpace(abnormaltype))
			{
				sql += " AND A.ABNORMALTYPE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Int) { Value = abnormaltype });
			}
			if (!string.IsNullOrWhiteSpace(shelfcode))
			{
				sql += " AND A.SHELFCODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = shelfcode });
			}
			if (!string.IsNullOrWhiteSpace(ordercode))
			{
				sql += " AND A.ORDERCODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = ordercode });
			}
			if (!string.IsNullOrWhiteSpace(bincode))
			{
				sql += " AND A.BINCODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = bincode });
			}
			if (!string.IsNullOrWhiteSpace(skucode))
			{
				sql += " AND A.SKUCODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = skucode });
			}

			var result = SqlQuery<F060801Data>(sql, param.ToArray());

			return result;
		}

	}
}
