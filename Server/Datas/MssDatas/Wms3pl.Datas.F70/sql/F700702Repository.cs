using System;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F70
{
	public partial class F700702Repository : RepositoryBase<F700702, Wms3plDbContext, F700702Repository>
	{
		/// <summary>
		/// 從出貨訂單取得訂單狀況分時統計報表
		/// </summary>
		/// <param name="beginCrtDate"></param>
		/// <returns></returns>
		public IQueryable<F700702ForSchedule> GetOrderCountForHour(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var sql = @"  SELECT A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 CONVERT(datetime , A.CRT_DATE,120) AS CNT_DATE,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '00:00:00'
																				   AND '00:59:59'
									   THEN
										  1
									END)
									AS CNT1,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '01:00:00'
																				   AND '01:59:59'
									   THEN
										  1
									END)
									AS CNT2,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE,108) BETWEEN '02:00:00'
																				   AND '02:59:59'
									   THEN
										  1
									END)
									AS CNT3,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE,108) BETWEEN '03:00:00'
																				   AND '03:59:59'
									   THEN
										  1
									END)
									AS CNT4,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '04:00:00'
																				   AND '04:59:59'
									   THEN
										  1
									END)
									AS CNT5,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '05:00:00'
																				   AND '05:59:59'
									   THEN
										  1
									END)
									AS CNT6,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '06:00:00'
																				   AND '06:59:59'
									   THEN
										  1
									END)
									AS CNT7,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '07:00:00'
																				   AND '07:59:59'
									   THEN
										  1
									END)
									AS CNT8,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '08:00:00'
																				   AND '08:59:59'
									   THEN
										  1
									END)
									AS CNT9,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '09:00:00'
																				   AND '09:59:59'
									   THEN
										  1
									END)
									AS CNT10,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '10:00:00'
																				   AND '10:59:59'
									   THEN
										  1
									END)
									AS CNT11,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '11:00:00'
																				   AND '11:59:59'
									   THEN
										  1
									END)
									AS CNT12,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '12:00:00'
																				   AND '12:59:59'
									   THEN
										  1
									END)
									AS CNT13,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '13:00:00'
																				   AND '13:59:59'
									   THEN
										  1
									END)
									AS CNT14,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '14:00:00'
																				   AND '14:59:59'
									   THEN
										  1
									END)
									AS CNT15,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '15:00:00'
																				   AND '15:59:59'
									   THEN
										  1
									END)
									AS CNT16,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '16:00:00'
																				   AND '16:59:59'
									   THEN
										  1
									END)
									AS CNT17,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '17:00:00'
																				   AND '17:59:59'
									   THEN
										  1
									END)
									AS CNT18,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '18:00:00'
																				   AND '18:59:59'
									   THEN
										  1
									END)
									AS CNT19,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '19:00:00'
																				   AND '19:59:59'
									   THEN
										  1
									END)
									AS CNT20,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '20:00:00'
																				   AND '20:59:59'
									   THEN
										  1
									END)
									AS CNT21,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '21:00:00'
																				   AND '21:59:59'
									   THEN
										  1
									END)
									AS CNT22,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '22:00:00'
																				   AND '22:59:59'
									   THEN
										  1
									END)
									AS CNT23,
								 COUNT (
									CASE
									   WHEN CONVERT(varchar,A.CRT_DATE, 108) BETWEEN '23:00:00'
																				   AND '23:59:59'
									   THEN
										  1
									END)
									AS CNT24
							FROM F050101 A
						   WHERE A.CRT_DATE BETWEEN @p0 AND @p1 AND A.STATUS <> '9'
						GROUP BY A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 CONVERT(datetime,A.CRT_DATE,120)";

			return SqlQuery<F700702ForSchedule>(sql, new object[] { beginCrtDate, endCrtDate });
		}
	}
}
