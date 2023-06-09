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
	public partial class F161402Repository : RepositoryBase<F161402, Wms3plDbContext, F161402Repository>
	{
		public IQueryable<F161402Data> GetF161402ReturnDetails(string dcCode, string gupCode, string custCode, string returnNo, string auditStaff, string auditName)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo),
				new SqlParameter("@p4", auditStaff),
				new SqlParameter("@p5", auditName)
			};
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY a.RETURN_NO,a.ITEM_CODE,c.ITEM_NAME,c.BUNDLE_SERIALNO, t.TOTAL_AUDIT_QTY,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,
																 a.LOC_CODE,t.MOVED_QTY , a.CAUSE, a.MEMO,a.MULTI_FLAG) ROWNUM,a.RETURN_NO,a.ITEM_CODE,c.ITEM_NAME,c.BUNDLE_SERIALNO, t.TOTAL_AUDIT_QTY,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,
																 a.LOC_CODE,a.CAUSE, a.MEMO,a.MULTI_FLAG ,t.MOVED_QTY,SUM(a.RTN_QTY) RTN_QTY,SUM(a.AUDIT_QTY) AUDIT_QTY, (SUM(a.RTN_QTY)-t.TOTAL_AUDIT_QTY) as DIFFERENT_QTY,SUM(CASE WHEN a.RTN_QTY=0 AND (a.ITEM_CODE<>'XYZ00001' OR a.MOVED_QTY =0) THEN 1 ELSE 0 END) HasNotInReturnItem,
                                                                 c.EAN_CODE1 ,c.EAN_CODE2 ,c.EAN_CODE3 
													  FROM F161402 a 
											 LEFT JOIN (SELECT DC_CODE,GUP_CODE,CUST_CODE,RETURN_NO,ITEM_CODE,SUM(MOVED_QTY) as MOVED_QTY ,SUM(AUDIT_QTY) as TOTAL_AUDIT_QTY
																		FROM F161402
																GROUP BY DC_CODE,GUP_CODE,CUST_CODE,RETURN_NO,ITEM_CODE) t 
																			ON a.DC_CODE = t.DC_CODE AND a.GUP_CODE = t.GUP_CODE AND a.CUST_CODE = t.CUST_CODE AND a.RETURN_NO = t.RETURN_NO AND a.ITEM_CODE = t.ITEM_CODE 
											 LEFT JOIN F1903 c ON a.GUP_CODE = c.GUP_CODE AND a.CUST_CODE = c.CUST_CODE AND a.ITEM_CODE = c.ITEM_CODE
													 WHERE a.DC_CODE = @p0
														 AND a.GUP_CODE = @p1
														 AND a.CUST_CODE = @p2
														 AND a.RETURN_NO = @p3
														 AND a.AUDIT_STAFF = @p4
														 AND a.AUDIT_NAME = @p5
											GROUP BY a.RETURN_NO,a.ITEM_CODE,c.ITEM_NAME,c.BUNDLE_SERIALNO, t.TOTAL_AUDIT_QTY,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,
																 a.LOC_CODE,t.MOVED_QTY , a.CAUSE, a.MEMO,a.MULTI_FLAG,c.EAN_CODE1 ,c.EAN_CODE2 ,c.EAN_CODE3 ";

			var result = SqlQuery<F161402Data>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		/// <summary>
		/// 是否超過退貨量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public bool IsOverF161402RtnQty(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, int addAuditQty)
		{
			var sql = @"SELECT TOP(1) C.AUDIT_QTY
						  FROM (  SELECT B.DC_CODE,
										 B.GUP_CODE,
										 B.CUST_CODE,
										 B.RETURN_NO,
										 B.ITEM_CODE,
										 SUM(B.RTN_QTY) AS RTN_QTY,
										 SUM (B.AUDIT_QTY) AS AUDIT_QTY
									FROM F161402 B
								   WHERE     B.DC_CODE = @p0
										 AND B.GUP_CODE = @p1
										 AND B.CUST_CODE = @p2
										 AND B.RETURN_NO = @p3
										 AND B.ITEM_CODE = @p4
								GROUP BY B.DC_CODE,
										 B.GUP_CODE,
										 B.CUST_CODE,
										 B.RETURN_NO,
										 B.ITEM_CODE) C
						 WHERE C.AUDIT_QTY + @p5 > C.RTN_QTY --AND ROWNUM = 1";

			return SqlQuery<int>(sql, new object[] { dcCode, gupCode, custCode, returnNo, itemCode, addAuditQty }).Any();
		}

		/// <summary>
		/// 只取得 ITEM_CODE 跟總合的 MOVED_QTY
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="sourceNo"></param>
		/// <returns></returns>
		public IQueryable<F161402Data> GetItemCodeAndMovedQtys(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var sql = @"  SELECT B.ITEM_CODE, SUM (B.AUDIT_QTY) AS AUDIT_QTY
							FROM F161201 A
								 JOIN F161402 B
									ON     A.DC_CODE = B.DC_CODE
									   AND A.GUP_CODE = B.GUP_CODE
									   AND A.CUST_CODE = B.CUST_CODE
									   AND A.RETURN_NO = B.RETURN_NO
						   WHERE     A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND A.SOURCE_NO = @p3
						GROUP BY B.ITEM_CODE";

			return SqlQuery<F161402Data>(sql, new object[] { dcCode, gupCode, custCode, sourceNo });
		}

		public IQueryable<F161402> GetDatas(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo),
			};
			var sql = @" SELECT *
                     FROM F161402
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND RETURN_NO = @p3 ";
			return SqlQuery<F161402>(sql, parameters.ToArray());
		}
		public IQueryable<F161402> GetDatasByF161202JoinF910101(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo),
				new SqlParameter("@p4", Current.Staff),
				new SqlParameter("@p5", Current.StaffName),
			};
			var sql = @"SELECT  A.DC_CODE,
													A.GUP_CODE,
													A.CUST_CODE,
													A.RETURN_NO,
													ROW_NUMBER()OVER(ORDER BY A.RETURN_NO,A.RETURN_SEQ,A.DC_CODE,A.GUP_CODE,A.CUST_CODE) AS RETURN_AUDIT_SEQ,
													CASE WHEN C.MATERIAL_CODE IS NULL THEN A.ITEM_CODE ELSE C.MATERIAL_CODE END ITEM_CODE,
													D.LOC_CODE,
													0 AS MOVED_QTY,
                          CASE WHEN C.MATERIAL_CODE IS NULL THEN A.RTN_QTY ELSE A.RTN_QTY*C.BOM_QTY END RTN_QTY,
													0 AS AUDIT_QTY,
													@p4 AS AUDIT_STAFF,
													@p5 AS AUDIT_NAME,
													NULL AS MEMO,
													dbo.GetSysDate() AS CRT_DATE,
													@p4 AS CRT_STAFF,
													@p5 AS CRT_NAME,
                          NULL AS UPD_DATE,
                          NULL AS UPD_STAFF,
                          NULL AS UPD_NAME,
                          NULL AS CAUSE,
													CASE WHEN B.ITEM_CODE IS NULL
													THEN NULL 
													ELSE B.ITEM_CODE
													END BOM_ITEM_CODE,
                          C.BOM_QTY,
													ISNULL(E.ITEM_QTY,1) ITEM_QTY,
                          CASE WHEN E.ITEM_QTY IS NULL
                          THEN '0'
                          ELSE '1' END MULTI_FLAG
										 FROM F161202 A
										 LEFT JOIN F910101 B
										   ON A.GUP_CODE = B.GUP_CODE
										  AND A.CUST_CODE = B.CUST_CODE
										  AND A.ITEM_CODE = B.ITEM_CODE
										  AND B.ISPROCESS = '0'
										  AND B.STATUS ='0'
										 LEFT JOIN F910102 C
										  ON C.GUP_CODE = B.GUP_CODE
										 AND C.CUST_CODE = B.CUST_CODE
										 AND C.BOM_NO = B.BOM_NO
										LEFT JOIN 
										(SELECT Min(f.LOC_CODE) LOC_CODE,Min(f.WAREHOUSE_ID) WAREHOUSE_ID,f.DC_CODE  FROM F1912 f LEFT JOIN F1980 g ON f.WAREHOUSE_ID = g.WAREHOUSE_ID AND f.DC_CODE = g.DC_CODE
											WHERE f.DC_CODE = @p0 AND f.GUP_CODE in (@p1,'0') AND f.CUST_CODE in (@p2,'0') AND g.WAREHOUSE_TYPE = 'T' 
													GROUP BY f.DC_CODE) D
										  ON D.DC_CODE = A.DC_CODE
                    LEFT JOIN 
                    (SELECT G.GUP_CODE,G.CUST_CODE,G.ITEM_CODE,SUM(F.BOM_QTY) ITEM_QTY
                                      FROM F910102 F
                                    INNER JOIN F910101 G
                                          ON G.GUP_CODE = F.GUP_CODE
                                        AND G.CUST_CODE = F.CUST_CODE
                                        AND G.BOM_NO = F.BOM_NO
                                        AND G.ISPROCESS = '1'
                                        AND G.STATUS ='0'
                                      GROUP BY G.GUP_CODE,G.CUST_CODE,G.ITEM_CODE
                                    ) E
											ON  E.GUP_CODE = A.GUP_CODE
											AND E.CUST_CODE = A.CUST_CODE             
											AND E.ITEM_CODE = ISNULL(C.MATERIAL_CODE,A.ITEM_CODE)
									 WHERE A.DC_CODE = @p0
										 AND A.GUP_CODE = @p1
										 AND A.CUST_CODE = @p2
										 AND A.RETURN_NO = @p3 ";
			return SqlQuery<F161402>(sql, parameters.ToArray());
		}

		public IQueryable<F161402ToF16140201Data> GetF161402ToF16140201Data(string dcCode,string gupCode,string custCode,string returnNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo),
			};
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.RETURN_NO,
										A.RETURN_AUDIT_SEQ,
										A.ITEM_CODE)ROWNUM
										,A. *
										 FROM (
												--取得實體組合商品有少退的產生組合明細一組
												SELECT  A.DC_CODE,
																A.GUP_CODE,
																A.CUST_CODE,
																A.RETURN_NO,
																A.RETURN_AUDIT_SEQ,
																A.ITEM_CODE,
																ABS((A.AUDIT_QTY-A.RTN_QTY) * C.BOM_QTY) RTN_QTY,
																0 AUDIT_QTY,
																C.MATERIAL_CODE,
																C.BOM_QTY,
																(A.AUDIT_QTY-A.RTN_QTY) * C.BOM_QTY UNAUDIT_QTY,
																1 AS ITEM_QTY,
                               '0' AS MULTI_FLAG
													FROM F161402 A
												 INNER JOIN F910101 B
														ON B.GUP_CODE = A.GUP_CODE
													 AND B.CUST_CODE=  A.CUST_CODE
													 AND B.ITEM_CODE = A.ITEM_CODE
													 AND B.STATUS ='0'
													 AND B.ISPROCESS ='1'
												 INNER JOIN F910102 C
														ON C.GUP_CODE = B.GUP_CODE
													 AND C.CUST_CODE = B.CUST_CODE
													 AND C.BOM_NO = B.BOM_NO
												 WHERE A.MULTI_FLAG ='1'
													 AND A.AUDIT_QTY<A.RTN_QTY
												 UNION ALL
												--將退貨掃描身擋所有資料產生出來
												SELECT A.DC_CODE,
															 A.GUP_CODE,
                               A.CUST_CODE,
                               A.RETURN_NO,
                               A.RETURN_AUDIT_SEQ,
                               A.ITEM_CODE,
                               A.RTN_QTY,
                               A.AUDIT_QTY,
                               NULL MATERIAL_CODE,
                               NULL BOM_QTY,
                               A.AUDIT_QTY-A.RTN_QTY UNAUDIT_QTY,
                               A.ITEM_QTY,
                               A.MULTI_FLAG
												FROM F161402 A
											)A
											WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE =@p2
										AND A.RETURN_NO = @p3 ";
			return SqlQuery<F161402ToF16140201Data>(sql, parameters.ToArray());

		}
        public void UpdateLocAndCauseAndMemo(string dcCode,string gupCode,string custCode,string returnNo,string itemCode,string locCode,string cause,string memo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", locCode),
                new SqlParameter("@p1", string.IsNullOrEmpty(cause)? (object)DBNull.Value:cause),
                new SqlParameter("@p2", string.IsNullOrEmpty(memo)? (object)DBNull.Value:memo),
                new SqlParameter("@p3", Current.Staff),
                new SqlParameter("@p4", Current.StaffName),
                new SqlParameter("@p5", dcCode),
                new SqlParameter("@p6", gupCode),
                new SqlParameter("@p7", custCode),
                new SqlParameter("@p8", returnNo),
                new SqlParameter("@p9", itemCode),
            };
            var sql = @" UPDATE F161402
                             SET LOC_CODE = @p0,
                                 CAUSE = @p1,
                                 MEMO = @p2,
                                 UPD_DATE = dbo.GetSysDate(),
                                 UPD_STAFF = @p3,
                                 UPD_NAME  = @p4
                           WHERE DC_CODE = @p5
                             AND GUP_CODE = @p6
                             AND CUST_CODE = @p7
                             AND RETURN_NO = @p8
                             AND ITEM_CODE = @p9
                             AND AUDIT_STAFF = @p3
                             AND AUDIT_NAME = @p4 ";
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void DeleteNotInReturnDataBomItemAuditQtyIsZero(string dcCode,string gupCode,string custCode,string returnNo)
        {
            var sql = @"  DELETE FROM F161402 
                          WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND RETURN_NO = @p3
                            AND AUDIT_STAFF = @p4
                            AND AUDIT_NAME = @p5
                            AND EXISTS(
                            SELECT 1
                            FROM F161402 B
                            WHERE B.RTN_QTY =0 
                              AND B.BOM_ITEM_CODE IS NOT NULL 
                              AND DC_CODE = B.DC_CODE 
                              AND GUP_CODE = B.GUP_CODE 
                              AND CUST_CODE = B.CUST_CODE 
                              AND RETURN_NO = B.RETURN_NO 
                              AND BOM_ITEM_CODE = B.BOM_ITEM_CODE
                            GROUP BY B.BOM_ITEM_CODE
                            HAVING SUM(B.AUDIT_QTY) = 0) ";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnNo),
                new SqlParameter("@p4", Current.Staff),
                new SqlParameter("@p5", Current.StaffName),
            };
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void DeleteNoInReturnDataAndNotVirturalBomItem(string dcCode,string gupCode,string custCode,string returnNo,string itemCode)
        {
            var sql = @" DELETE FROM F161402
                          WHERE DC_CODE = @p0 
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND RETURN_NO = @p3
                            AND ITEM_CODE =  @p4
                            AND AUDIT_STAFF = @p5
                            AND AUDIT_NAME = @p6
                            AND BOM_ITEM_CODE IS NULL
                            AND RTN_QTY = 0
                            AND MOVED_QTY = 0";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnNo),
                new SqlParameter("@p4", itemCode),
                new SqlParameter("@p5", Current.Staff),
                new SqlParameter("@p6", Current.StaffName),
            };
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

		public IQueryable<F161402RtnData> GetTransReturnData(string dcCode,string gupCode,string custCode)
		{
			var parms = new List<object> { dcCode , gupCode , custCode };
			var sql = @"
					SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.ITEM_CODE,B.RETURN_NO,SUM(B.MOVED_QTY) MOVED_QTY
					  FROM F161201 A
						   INNER JOIN F161402 B
							  ON     A.DC_CODE = B.DC_CODE
								 AND A.GUP_CODE = B.GUP_CODE
								 AND A.CUST_CODE = B.CUST_CODE
								 AND A.RETURN_NO = B.RETURN_NO
					WHERE  A.DC_CODE = @p0
						   AND A.GUP_CODE = @p1
						   AND A.CUST_CODE = @p2
						   AND A.STATUS = '2'
						   AND A.PROC_FLAG = '0'
				GROUP BY B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.RETURN_NO,B.ITEM_CODE
					";

			return SqlQuery<F161402RtnData>(sql, parms.ToArray());
		}

    }
}
