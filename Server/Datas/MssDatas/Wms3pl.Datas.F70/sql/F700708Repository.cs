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
	public partial class F700708Repository : RepositoryBase<F700708, Wms3plDbContext, F700708Repository>
	{
		/// <summary>
		/// 從進貨到出貨的資料表，取得物流中心整體作業績效統計
		/// </summary>
		/// <param name="importDate"></param>
		/// <returns></returns>
		public IQueryable<F700708ForSchedule> GetDcPerformanceStatistics(DateTime beginImportDate, DateTime endImportDate)
		{
			var sql = @"SELECT L.*,
							   ROUND (
								  (  L.TOTAL_QTY
								   / (CASE L.WORK_HOUR WHEN 0 THEN 1 ELSE L.WORK_HOUR END)),
								  2)
								  AS WORK_AVG
						  FROM (SELECT K.*,
									   (  K.STOCK_QTY
										+ K.ALLOCATION_QTY
										+ K.PICK_QTY
										+ K.PACKAGE_QTY
										+ K.AUDIT_QTY
										+ K.INCAR_QTY)
										  AS TOTAL_QTY
								  FROM (  SELECT A.GRP_ID,
												 A.IMPORT_DATE AS CNT_DATE,
												 (DATEPART (DAY,A.IMPORT_DATE) - 1) AS CNT_DAY,
												 A.PERSON_NUMBER,
												 A.WORK_HOUR,
												 B.DC_CODE,
												 B.GUP_CODE,
												 B.CUST_CODE,
												 ISNULL (SUM (E.RECV_QTY), 0) AS STOCK_QTY,
												 ISNULL (SUM (F.ALLOCATION_QTY), 0) AS ALLOCATION_QTY,
												 ISNULL (SUM (G.PICK_QTY), 0) AS PICK_QTY,
												 ISNULL (SUM (H.PACKAGE_QTY), 0) AS PACKAGE_QTY,
												 ISNULL (SUM (I.AUDIT_QTY), 0) AS AUDIT_QTY,
												 ISNULL (SUM (J.INCAR_QTY), 0) AS INCAR_QTY
											FROM F700701 A
												 JOIN F190101 B ON A.DC_CODE = B.DC_CODE
												 JOIN F192401 C ON A.GRP_ID = C.GRP_ID
												 JOIN F1924 D ON C.EMP_ID = D.EMP_ID
												 LEFT JOIN (  SELECT E1.DC_CODE,
																	 E1.GUP_CODE,
																	 E1.CUST_CODE,
																	 E1.CRT_STAFF,
																	 E1.CRT_NAME,
																	 E1.RECE_DATE,
																	 SUM (E1.RECV_QTY) AS RECV_QTY
																FROM F020201 E1
															   WHERE E1.RECE_DATE = @p13
															GROUP BY E1.DC_CODE,
																	 E1.GUP_CODE,
																	 E1.CUST_CODE,
																	 E1.CRT_STAFF,
																	 E1.CRT_NAME,
																	 E1.RECE_DATE) E          -- 進倉驗收檔
													ON     B.DC_CODE = E.DC_CODE
													   AND B.GUP_CODE = E.GUP_CODE
													   AND B.CUST_CODE = E.CUST_CODE
													   AND D.EMP_ID = E.CRT_STAFF
													   AND D.EMP_NAME = E.CRT_NAME
													   AND A.IMPORT_DATE = E.RECE_DATE
												 LEFT JOIN
												 (  SELECT F3.DC_CODE,
														   F3.GUP_CODE,
														   F3.CUST_CODE,
														   F3.STAFF,
														   F3.STAFF_NAME,
														   F3.ALLOCATION_DATE,
														   SUM (F3.ALLOCATION_QTY) AS ALLOCATION_QTY
													  FROM (  SELECT F1.DC_CODE,
																	 F1.GUP_CODE,
																	 F1.CUST_CODE,
																	 F2.SRC_STAFF AS STAFF,
																	 F2.SRC_NAME AS STAFF_NAME,
																	 F1.ALLOCATION_DATE,
																	 SUM (F2.A_SRC_QTY) AS ALLOCATION_QTY
																FROM F151001 F1
																	 JOIN F151002 F2
																		ON     F1.DC_CODE = F2.DC_CODE
																		   AND F1.GUP_CODE = F2.GUP_CODE
																		   AND F1.CUST_CODE = F2.CUST_CODE
																		   AND F1.ALLOCATION_NO =
																				  F2.ALLOCATION_NO
															   WHERE     F1.ALLOCATION_DATE = @p13
																	 AND F1.STATUS = '5'
																	 AND F2.TAR_STAFF IS NULL -- 只有已下架
															GROUP BY F1.DC_CODE,
																	 F1.GUP_CODE,
																	 F1.CUST_CODE,
																	 F2.SRC_STAFF,
																	 F2.SRC_NAME,
																	 F1.ALLOCATION_DATE
															UNION ALL
															  SELECT F1.DC_CODE,
																	 F1.GUP_CODE,
																	 F1.CUST_CODE,
																	 F2.TAR_STAFF AS STAFF,
																	 F2.TAR_NAME AS STAFF_NAME,
																	 F1.ALLOCATION_DATE,
																	 SUM (F2.A_TAR_QTY) AS ALLOCATION_QTY
																FROM F151001 F1
																	 JOIN F151002 F2
																		ON     F1.DC_CODE = F2.DC_CODE
																		   AND F1.GUP_CODE = F2.GUP_CODE
																		   AND F1.CUST_CODE = F2.CUST_CODE
																		   AND F1.ALLOCATION_NO =
																				  F2.ALLOCATION_NO
															   WHERE     F1.ALLOCATION_DATE = @p13
																	 AND F1.STATUS = '5'
																	 AND F2.TAR_STAFF IS NOT NULL -- 已上架
															GROUP BY F1.DC_CODE,
																	 F1.GUP_CODE,
																	 F1.CUST_CODE,
																	 F2.TAR_STAFF,
																	 F2.TAR_NAME,
																	 F1.ALLOCATION_DATE) F3
												  GROUP BY F3.DC_CODE,
														   F3.GUP_CODE,
														   F3.CUST_CODE,
														   F3.STAFF,
														   F3.STAFF_NAME,
														   F3.ALLOCATION_DATE) F                -- 調撥單
													ON     B.DC_CODE = F.DC_CODE
													   AND B.GUP_CODE = F.GUP_CODE
													   AND B.CUST_CODE = F.CUST_CODE
													   AND D.EMP_ID = F.STAFF
													   AND D.EMP_NAME = F.STAFF_NAME
													   AND A.IMPORT_DATE = F.ALLOCATION_DATE
												 LEFT JOIN
												 (  SELECT G1.DC_CODE,
														   G1.GUP_CODE,
														   G1.CUST_CODE,
														   G1.PICK_STAFF,
														   G1.PICK_NAME,
														   CONVERT (varchar,G1.CRT_DATE,111) AS CRT_DATE,
														   SUM (G2.A_PICK_QTY) AS PICK_QTY
													  FROM F051201 G1
														   JOIN F051202 G2
															  ON     G1.DC_CODE = G2.DC_CODE
																 AND G1.GUP_CODE = G2.GUP_CODE
																 AND G1.CUST_CODE = G2.CUST_CODE
																 AND G1.PICK_ORD_NO = G2.PICK_ORD_NO
													 WHERE     G1.CRT_DATE BETWEEN @p13 AND @p14
														   AND G1.PICK_STATUS = 2              -- 揀貨完成
												  GROUP BY G1.DC_CODE,
														   G1.GUP_CODE,
														   G1.CUST_CODE,
														   G1.PICK_STAFF,
														   G1.PICK_NAME,
														   CONVERT (varchar,G1.CRT_DATE,111)) G               -- 揀貨單
													ON     B.DC_CODE = G.DC_CODE
													   AND B.GUP_CODE = G.GUP_CODE
													   AND B.CUST_CODE = G.CUST_CODE
													   AND D.EMP_ID = G.PICK_STAFF
													   AND D.EMP_NAME = G.PICK_NAME
													   AND A.IMPORT_DATE = G.CRT_DATE
												 LEFT JOIN (  SELECT H1.DC_CODE,
																	 H1.GUP_CODE,
																	 H1.CUST_CODE,
																	 H1.PACKAGE_STAFF,
																	 H1.PACKAGE_NAME,
																	 CONVERT (varchar,H1.PRINT_DATE,111) AS PRINT_DATE,
																	 COUNT (H1.BOX_NUM) AS PACKAGE_QTY
																FROM F055001 H1
															   WHERE H1.PRINT_DATE BETWEEN @p13 AND @p14 -- 列印包裝完成的時間
															GROUP BY H1.DC_CODE,
																	 H1.GUP_CODE,
																	 H1.CUST_CODE,
																	 H1.PACKAGE_STAFF,
																	 H1.PACKAGE_NAME,
																	 CONVERT (varchar,H1.PRINT_DATE,111)) H
													ON     B.DC_CODE = H.DC_CODE
													   AND B.GUP_CODE = H.GUP_CODE
													   AND B.CUST_CODE = H.CUST_CODE
													   AND D.EMP_ID = H.PACKAGE_STAFF
													   AND D.EMP_NAME = H.PACKAGE_NAME
													   AND A.IMPORT_DATE = H.PRINT_DATE
												 LEFT JOIN
												 (  SELECT I1.DC_CODE,
														   I1.GUP_CODE,
														   I1.CUST_CODE,
														   I1.AUDIT_STAFF,
														   I1.AUDIT_NAME,
														   CONVERT (varchar,I1.AUDIT_DATE,111) AS AUDIT_DATE,
														   COUNT (I1.BOX_NUM) AS AUDIT_QTY
													  FROM F055001 I1
													 WHERE     I1.AUDIT_DATE BETWEEN @p13 AND @p14
														   AND I1.STATUS = '1'                  -- 已稽核
												  GROUP BY I1.DC_CODE,
														   I1.GUP_CODE,
														   I1.CUST_CODE,
														   I1.AUDIT_STAFF,
														   I1.AUDIT_NAME,
														   CONVERT (varchar,I1.AUDIT_DATE,111)) I
													ON     B.DC_CODE = I.DC_CODE
													   AND B.GUP_CODE = I.GUP_CODE
													   AND B.CUST_CODE = I.CUST_CODE
													   AND D.EMP_ID = I.AUDIT_STAFF
													   AND D.EMP_NAME = I.AUDIT_NAME
													   AND A.IMPORT_DATE = I.AUDIT_DATE
												 LEFT JOIN
												 (  SELECT J1.DC_CODE,
														   J1.GUP_CODE,
														   J1.CUST_CODE,
														   J1.INCAR_STAFF,
														   J1.INCAR_NAME,
														   CONVERT (varchar,J1.INCAR_DATE,111) AS INCAR_DATE,
														   COUNT (J2.BOX_NUM) AS INCAR_QTY
													  FROM F050801 J1
														   JOIN F055001 J2
															  ON     J1.DC_CODE = J2.DC_CODE
																 AND J1.GUP_CODE = J2.GUP_CODE
																 AND J1.CUST_CODE = J2.CUST_CODE
																 AND J1.WMS_ORD_NO = J2.WMS_ORD_NO
													 WHERE J1.INCAR_DATE BETWEEN @p13 AND @p14 -- 裝車時間
												  GROUP BY J1.DC_CODE,
														   J1.GUP_CODE,
														   J1.CUST_CODE,
														   J1.INCAR_STAFF,
														   J1.INCAR_NAME,
														   CONVERT (varchar,J1.INCAR_DATE,111)) J
													ON     B.DC_CODE = J.DC_CODE
													   AND B.GUP_CODE = J.GUP_CODE
													   AND B.CUST_CODE = J.CUST_CODE
													   AND D.EMP_ID = J.INCAR_STAFF
													   AND D.EMP_NAME = J.INCAR_NAME
													   AND A.IMPORT_DATE = J.INCAR_DATE
										   WHERE A.IMPORT_DATE = @p13
										GROUP BY A.GRP_ID,
												 A.IMPORT_DATE,
												 (DATEPART (DAY,A.IMPORT_DATE) - 1),
												 A.PERSON_NUMBER,
												 A.WORK_HOUR,
												 B.DC_CODE,
												 B.GUP_CODE,
												 B.CUST_CODE) K) L";

			return SqlQuery<F700708ForSchedule>(sql, 
												new SqlParameter[] 
												{	
													new SqlParameter("@p13", beginImportDate) ,
													new SqlParameter("@p14", endImportDate) 
												});
		}
	}
}
