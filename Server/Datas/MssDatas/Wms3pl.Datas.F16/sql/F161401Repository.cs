using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161401Repository : RepositoryBase<F161401, Wms3plDbContext, F161401Repository>
	{
		public IQueryable<DcWmsNoStatusItem> GetReturnProcessOver30MinByDc(string dcCode, DateTime returnDate)
		{
			var sql =
                @" SELECT ROW_NUMBER()OVER(ORDER BY A.RETURN_NO)ROWNUM,A.RETURN_NO AS WMS_NO,C.ORD_PROP_NAME AS MEMO,A.AUDIT_STAFF+A.AUDIT_NAME AS STAFF_NAME,A.CRT_DATE AS START_DATE 
										FROM F161401 A
									 INNER JOIN F161201 B
										  ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.RETURN_NO = A.RETURN_NO
									 INNER JOIN F000903 C 
                      ON C.ORD_PROP = B.ORD_PROP
									 WHERE A.DC_CODE = @p0
										 AND A.RETURN_DATE =@p1
										 AND A.STATUS <>'2'
									   AND DateDiff(MINUTE, dbo.GetSysDate(), A.CRT_DATE) >30 ";
			var param = new object[] { dcCode, returnDate };
			return SqlQuery<DcWmsNoStatusItem>(sql, param);
		}

		/// <summary>
		/// 取得P107_退貨記錄總表報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<P17ReturnAuditReport> GetP17ReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms= new List<SqlParameter>();
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A.RETURN_DATE,
								 B.CUST_ORD_NO,
								 B.RTN_CUST_CODE,
								 B.RTN_CUST_NAME,
								 C.ITEM_CODE,
								 B.CRT_DATE,
								 C.AUDIT_NAME,
								 C.UPD_DATE)ROWNUM,
								 B.RETURN_DATE,
								 B.CUST_ORD_NO,
								 B.RTN_CUST_CODE,
								 B.RTN_CUST_NAME,
								 C.ITEM_CODE,
								 G.ITEM_NAME,
								 C.CRT_DATE,                                               -- (明細)驗收日期
								 C.RTN_QTY,
								 C.AUDIT_QTY,
								 ISNULL (
									(CASE WHEN C.RTN_QTY > C.AUDIT_QTY THEN C.RTN_QTY - C.AUDIT_QTY END),
									0)
									AS Shortfall,                                              -- 短缺數量
								 ISNULL (
									(CASE WHEN C.AUDIT_QTY > C.RTN_QTY THEN C.AUDIT_QTY - C.RTN_QTY END),
									0)
									AS MultiReturn,                                            -- 多退數量
								 D.CAUSE,                                                    -- 中文異常原因
								 C.AUDIT_NAME,
								 C.UPD_DATE
							FROM F161401 A                                                     -- 退貨檢驗
								 JOIN F161201 B                                                -- 退貨商品
									ON     A.DC_CODE = B.DC_CODE
									   AND A.GUP_CODE = B.GUP_CODE
									   AND A.CUST_CODE = B.CUST_CODE
									   AND A.RETURN_NO = B.RETURN_NO
								 JOIN F161402 C                                              -- 退貨檢驗明細
									ON     B.DC_CODE = C.DC_CODE
									   AND B.GUP_CODE = C.GUP_CODE
									   AND B.CUST_CODE = C.CUST_CODE
									   AND B.RETURN_NO = C.RETURN_NO
								 LEFT JOIN F1903 G
									ON C.GUP_CODE = G.GUP_CODE AND C.ITEM_CODE = G.ITEM_CODE AND C.CUST_CODE = G.CUST_CODE
								 LEFT JOIN F1951 D ON C.CAUSE = D.UCC_CODE AND D.UCT_ID = 'RC' 
                                WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            sql += @"     AND C.UPD_DATE IS NOT NULL
						--ORDER BY A.RETURN_DATE,
						--		 B.CUST_ORD_NO,
						--		 B.RTN_CUST_CODE,
						--		 B.RTN_CUST_NAME,
						--		 C.ITEM_CODE,
						--		 B.CRT_DATE,
						--		 C.AUDIT_NAME,
						--		 C.UPD_DATE  ";
            return SqlQuery<P17ReturnAuditReport>(sql, prms.ToArray());
		}

		/// <summary>
		/// 取得RTO17840退貨記錄表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<RTO17840ReturnAuditReport> GetRTO17840ReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms = new List<SqlParameter>();
            var sql = @"SELECT ROW_NUMBER()over(order by CUST_ORD_NO)ROWNUM,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   A.RETURN_NO,
							   B.CUST_ORD_NO,
							   B.RTN_CUST_NAME,
							   A.RETURN_DATE,
							   C.ITEM_CODE,
							   G.ITEM_NAME,
							   C.RTN_QTY,
							   C.AUDIT_QTY,
							   (C.RTN_QTY - C.AUDIT_QTY) AS Difference,                         -- 差異數
							   D.CAUSE,
							   ISNULL( ABS (C.RTN_QTY - C.AUDIT_QTY), 0) AS Abnormal,                       -- 異常數
							   E.CUST_NAME,
							   F.DC_NAME
						  FROM F161401 A                                                       -- 退貨檢驗
							   JOIN F161201 B                                                  -- 退貨商品
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.RETURN_NO = B.RETURN_NO
							   JOIN F161402 C                                                -- 退貨檢驗明細
								  ON     B.DC_CODE = C.DC_CODE
									 AND B.GUP_CODE = C.GUP_CODE
									 AND B.CUST_CODE = C.CUST_CODE
									 AND B.RETURN_NO = C.RETURN_NO
							   LEFT JOIN F1903 G
								  ON C.GUP_CODE = G.GUP_CODE AND C.ITEM_CODE = G.ITEM_CODE AND C.CUST_CODE = G.CUST_CODE 
							   LEFT JOIN F1951 D ON C.CAUSE = D.UCC_CODE AND D.UCT_ID = 'RC'
							   LEFT JOIN F1909 E
								  ON C.GUP_CODE = E.GUP_CODE AND C.CUST_CODE = E.CUST_CODE
							   LEFT JOIN F1901 F ON A.DC_CODE = F.DC_CODE 
                WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            sql += @"";

			return SqlQuery<RTO17840ReturnAuditReport>(sql, prms.ToArray());
        }

		/// <summary>
		/// B2C退貨記錄表(Friday退貨記錄表)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<B2CReturnAuditReport> GetB2CReturnAuditReports(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms = new List<SqlParameter>();
            var sql = @"
SELECT ROW_NUMBER()over(order by A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
								A.RETURN_NO
								)ROWNUM,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   B.WMS_ORD_NO,
							   E.CUST_NAME,
							   B.CUST_ORD_NO,
							   A.RETURN_NO,
							   B.RTN_CUST_NAME,
							   B.TEL,
							   D.CAUSE,                                                  -- 退貨單主檔的退貨原因
							   A.RETURN_DATE,
							   C.ITEM_CODE,
							   G.ITEM_NAME,
							   C.RTN_QTY,
							   C.AUDIT_QTY,
							   (C.RTN_QTY - C.AUDIT_QTY) AS Difference,                         -- 差異數
							   C.MEMO,
							   '' AS Description                                               -- 內容描述
						  FROM F161401 A                                                       -- 退貨檢驗
							   JOIN F161201 B                                                  -- 退貨商品
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.RETURN_NO = B.RETURN_NO
							   JOIN F161402 C                                                -- 退貨檢驗明細
								  ON     B.DC_CODE = C.DC_CODE
									 AND B.GUP_CODE = C.GUP_CODE
									 AND B.CUST_CODE = C.CUST_CODE
									 AND B.RETURN_NO = C.RETURN_NO
							   LEFT JOIN F1903 G
								  ON C.GUP_CODE = G.GUP_CODE AND C.ITEM_CODE = G.ITEM_CODE AND C.CUST_CODE = G.CUST_CODE
							   LEFT JOIN F1951 D ON B.RTN_CAUSE = D.UCC_CODE AND D.UCT_ID = 'RT'
							   LEFT JOIN F1909 E
								  ON C.GUP_CODE = E.GUP_CODE AND C.CUST_CODE = E.CUST_CODE 
                         WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            sql +=@" ";

			return SqlQuery<B2CReturnAuditReport>(sql, prms.ToArray());
        }

		/// <summary>
		/// 取得P106_退貨未上架明細表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<P106ReturnNotMoveDetail> GetP106ReturnNotMoveDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms = new List<SqlParameter>();
            var sql = @"SELECT ROW_NUMBER()over(order by C.CRT_DATE)ROWNUM,
							   C.CRT_DATE AS AUDIT_DATE,
							   B.POSTING_DATE,
							   B.RTN_CUST_CODE,
							   B.RTN_CUST_NAME,
							   B.CUST_ORD_NO,
							   A.RETURN_NO,
							   C.ITEM_CODE,
							   G.ITEM_NAME,
							   C.RTN_QTY,
							   C.AUDIT_QTY,
							   0 AS ALLOW_QTY,                                                  -- 允收量
							   0 AS NOTALLOW_QTY,                                              -- 不允收量
							   (C.AUDIT_QTY - C.MOVED_QTY) AS NOTMOVED_QTY                    -- 未上架數量
						  FROM F161401 A                                                       -- 退貨檢驗
							   JOIN F161201 B                                                  -- 退貨商品
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.RETURN_NO = B.RETURN_NO
							   JOIN F161402 C                                                -- 退貨檢驗明細
								  ON     B.DC_CODE = C.DC_CODE
									 AND B.GUP_CODE = C.GUP_CODE
									 AND B.CUST_CODE = C.CUST_CODE
									 AND B.RETURN_NO = C.RETURN_NO
							   LEFT JOIN F1903 G
								  ON C.GUP_CODE = G.GUP_CODE AND C.ITEM_CODE = G.ITEM_CODE AND C.CUST_CODE = G.CUST_CODE
                 WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            sql += @" ";

            return SqlQuery<P106ReturnNotMoveDetail>(sql, prms.ToArray());
		}

		/// <summary>
		/// 61-6 Txt 格式的退貨詳細資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<TxtFormatReturnDetail> GetTxtFormatReturnDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms = new List<SqlParameter>();
            var sql = @"  
                        SELECT 
        ROW_NUMBER()OVER(ORDER BY TB.SERIAL_NO, TB.CELL_NUM,TB.WMS_ORD_NO,TB.SECOND_ITEM_CODE)AS[ROWNUM]
        ,* 
    FROM(
		SELECT DISTINCT
						D.SERIAL_NO, -- DISTINCT 是因為 F161402 同退貨單同品項可能會有不同刷驗人員，資料會重複
						E.CELL_NUM,
						B.WMS_ORD_NO,
						'' AS SECOND_ITEM_CODE,                               -- 子料號
						C.ITEM_CODE,
						B.RTN_CUST_CODE,
						B.RTN_CUST_NAME,
						F.DELV_DATE,
						A.RETURN_NO	-- 加上 RETURN_NO 避免 DISTINCT 將不同天但相同序號過濾掉
		FROM F161401 A                                                     -- 退貨檢驗
				JOIN F161201 B                                                -- 退貨商品
				ON     A.DC_CODE = B.DC_CODE
					AND A.GUP_CODE = B.GUP_CODE
					AND A.CUST_CODE = B.CUST_CODE
					AND A.RETURN_NO = B.RETURN_NO
				JOIN F161402 C                                              -- 退貨檢驗明細
				ON     B.DC_CODE = C.DC_CODE
					AND B.GUP_CODE = C.GUP_CODE
					AND B.CUST_CODE = C.CUST_CODE
					AND B.RETURN_NO = C.RETURN_NO
				LEFT JOIN F16140101 D
				ON     C.DC_CODE = D.DC_CODE
					AND C.GUP_CODE = D.GUP_CODE
					AND C.CUST_CODE = D.CUST_CODE
					AND C.RETURN_NO = D.RETURN_NO
					AND C.ITEM_CODE = D.ITEM_CODE
					AND D.ISPASS = '1'
				LEFT JOIN F2501 E ON D.SERIAL_NO = E.SERIAL_NO
				LEFT JOIN F050801 F
				ON     B.DC_CODE = F.DC_CODE
					AND B.GUP_CODE = F.GUP_CODE
					AND B.CUST_CODE = F.CUST_CODE
					AND B.WMS_ORD_NO = F.WMS_ORD_NO
                 WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            sql += @" ) TB";

			return SqlQuery<TxtFormatReturnDetail>(sql, prms.ToArray());
		}

		/// <summary>
		/// 61-7	 Txt 格式的退貨資料以 F1903.TYPE in (types) 的資料為主
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public IQueryable<ReturnSerailNoByType> GetReturnSerailNosByType(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status, string[] types)
		{
            var sql = @"SELECT DISTINCT ROW_NUMBER()OVER(ORDER BY A.RETURN_NO,A.DC_CODE,A.GUP_CODE,A.CUST_CODE) ROWNUM,
										E.SERIAL_NO,
										E.CELL_NUM,
										E.STATUS
						  FROM F161401 A                                                       -- 退貨檢驗
							   JOIN F161402 C                                                -- 退貨檢驗明細
								  ON     A.DC_CODE = C.DC_CODE
									 AND A.GUP_CODE = C.GUP_CODE
									 AND A.CUST_CODE = C.CUST_CODE
									 AND A.RETURN_NO = C.RETURN_NO
							   JOIN F16140101 D
								  ON     C.DC_CODE = D.DC_CODE
									 AND C.GUP_CODE = D.GUP_CODE
									 AND C.CUST_CODE = D.CUST_CODE
									 AND C.RETURN_NO = D.RETURN_NO
									 AND C.ITEM_CODE = D.ITEM_CODE
									 AND D.ISPASS = '1'
							   JOIN F2501 E ON D.SERIAL_NO = E.SERIAL_NO
							   JOIN F1903 F
								  ON     C.ITEM_CODE = F.ITEM_CODE
									 AND C.GUP_CODE = F.GUP_CODE
                                     AND C.CUST_CODE = F.CUST_CODE 
                         WHERE A.DC_CODE = CASE  WHEN @p0 ='' THEN A.DC_CODE ELSE @p0 END 
							   AND A.GUP_CODE = CASE  WHEN @p1 ='' THEN A.GUP_CODE ELSE @p1 END 
							   AND A.CUST_CODE = CASE  WHEN @p2 ='' THEN A.CUST_CODE ELSE @p2 END 
							   AND A.RETURN_DATE BETWEEN CASE  WHEN @p3 =''  THEN A.RETURN_DATE ELSE  @p3 END 
													 AND  CASE  WHEN @p4 =''  THEN A.RETURN_DATE ELSE @p4 END 
							   AND A.RETURN_NO = CASE  WHEN @p5 ='' THEN A.RETURN_NO ELSE @p5 END 
							   AND A.STATUS =  CASE  WHEN @p6 ='' THEN A.STATUS ELSE @p6 END  ";


            var paramList = new List<object> { dcCode, dcCode, gupCode, gupCode, custCode, custCode, beginReturnDate, beginReturnDate, endReturnDate, endReturnDate, returnNo, returnNo, status, status };
            sql += paramList.CombineSqlInParameters("AND F.TYPE", types);

            return SqlQuery<ReturnSerailNoByType>(sql, paramList.ToArray());
        }

		/// <summary>
		/// 取得P015_預計退貨明細表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginReturnDate"></param>
		/// <param name="endReturnDate"></param>
		/// <param name="returnNo"></param>
		/// <param name="status"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public IQueryable<P015ForecastReturnDetail> GetP015ForecastReturnDetails(string dcCode, string gupCode, string custCode, DateTime? beginReturnDate, DateTime? endReturnDate, string returnNo, string status)
		{
            List<SqlParameter> prms = new List<SqlParameter>();
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY E.CUST_NAME,
								 A.RETURN_NO,
								 A.CUST_ORD_NO,
								 A.RTN_CUST_CODE,
								 A.RTN_CUST_NAME,
								 B.ITEM_CODE)ROWNUM,
								 E.CUST_NAME,
								 A.RETURN_NO,
								 A.CUST_ORD_NO,
								 A.RTN_CUST_CODE,
								 A.RTN_CUST_NAME,
								 B.ITEM_CODE,
								 G.ITEM_NAME,
								 B.RTN_QTY
							FROM F161201 A
								 JOIN F161202 B
									ON     A.DC_CODE = B.DC_CODE
									   AND A.GUP_CODE = B.GUP_CODE
									   AND A.CUST_CODE = B.CUST_CODE
									   AND A.RETURN_NO = B.RETURN_NO
								 LEFT JOIN F1909 E
									ON B.GUP_CODE = E.GUP_CODE AND B.CUST_CODE = E.CUST_CODE
								 LEFT JOIN F1903 G
									ON B.GUP_CODE = G.GUP_CODE AND B.ITEM_CODE = G.ITEM_CODE AND B.CUST_CODE = G.CUST_CODE
						    WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql += $@" AND A.DC_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = dcCode });
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += $@" AND A.GUP_CODE = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = gupCode });
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += $@" AND A.CUST_CODE =  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = custCode });
            }
            if (beginReturnDate.HasValue)
            {
                sql += $@" AND A.RETURN_DATE BETWEEN  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = beginReturnDate.Value });
            }
            if (endReturnDate.HasValue)
            {
                sql += $@" AND  @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = endReturnDate.Value });
            }
            if (!string.IsNullOrEmpty(returnNo))
            {
                sql += $@" AND A.RETURN_NO = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = returnNo });
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += $@" AND A.STATUS = @p{prms.Count()} ";
                prms.Add(new SqlParameter() { ParameterName = $"@p{prms.Count()}", Value = status });
            }
            return SqlQuery<P015ForecastReturnDetail>(sql, prms.ToArray());
		}
	}
}
