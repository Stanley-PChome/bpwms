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
	public partial class F0513Repository : RepositoryBase<F0513, Wms3plDbContext, F0513Repository>
	{
		public IQueryable<F0513WithF1909> GetF0513WithF1909Datas(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string delvTime, string status)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", delvDate)
			};
			var sql = @"SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,C.ORD_PIER_CODE,
                             C.ALL_ID,D.ALL_COMP,B.TAKE_TIME,C.PIER_CODE, 
                             CASE WHEN ISNULL(C.ORD_PIER_CODE,' ') =ISNULL(C.PIER_CODE,' ') THEN '0' ELSE '1' END AS STATUS 
                        FROM F050801 A 
                       INNER JOIN F700102 B 
                          ON B.DC_CODE = A.DC_CODE 
                         AND B.GUP_CODE = A.GUP_CODE 
                         AND B.CUST_CODE = A.CUST_CODE 
                         AND B.WMS_NO = A.WMS_ORD_NO 
                       INNER JOIN F700101 C 
                          ON C.DISTR_CAR_NO = B.DISTR_CAR_NO AND C.DC_CODE = B.DC_CODE 
                       INNER JOIN F1947 D 
                          ON D.DC_CODE = C.DC_CODE 
                         AND D.ALL_ID = C.ALL_ID 
                       WHERE A.DC_CODE = @p0 
                         AND A.GUP_CODE = @p1 
                         AND A.CUST_CODE =@p2 
                         AND A.DELV_DATE =@p3 ";

			if (!string.IsNullOrEmpty(delvTime))
			{
				sql += "   AND A.PICK_TIME = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, delvTime));
			}
			if (!string.IsNullOrEmpty(status))
			{
				sql += "   AND (CASE WHEN ISNULL(C.ORD_PIER_CODE,' ') =ISNULL(C.PIER_CODE,' ') THEN '0' ELSE '1' END) = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, status));
			}
			sql += @" GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,
								A.PICK_TIME,C.ORD_PIER_CODE, C.ALL_ID,D.ALL_COMP,B.TAKE_TIME,C.PIER_CODE, 
								CASE WHEN ISNULL(C.ORD_PIER_CODE,' ') =ISNULL(C.PIER_CODE,' ') THEN '0' ELSE '1' END ";

			var result = SqlQuery<F0513WithF1909>(sql, parameters.ToArray());

			return result;
		}

		#region 扣帳作業-批次
		public IQueryable<F0513WithF050801Batch> GetBatchDebitDatas(string dcCode, string gupCode, string custCode, bool notOrder, bool isB2c)
		{
			var param = new List<SqlParameter>() {
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode)
						};

			string sql = $@"   
					SELECT *
					  FROM (SELECT W.DC_CODE,
								   W.GUP_CODE,
								   W.CUST_CODE,
								   W.DELV_DATE,
								   W.PICK_TIME,
								   W.SOURCE_TYPE,
								   W.SOURCE_NAME,
								   W.ORD_TYPE,
								   W.ORD_TYPE_DESC,
								   ISNULL (X.WMS_ORD_COUNT, 0) WMS_ORD_COUNT,			-- 總出貨單數
								   ISNULL (Y.AUDIT_COUNT, 0) AUDIT_COUNT,				-- 已稽核單數
								   ISNULL (Z.NOSHIP_COUNT, 0) NOSHIP_COUNT,			-- 不出貨數
								   ISNULL (U.DEBIT_COUNT, 0) DEBIT_COUNT				-- 已扣帳單數
							  FROM (SELECT A.DC_CODE,
										   A.GUP_CODE,
										   A.CUST_CODE,
										   A.DELV_DATE,
										   A.PICK_TIME,
										   A.SOURCE_TYPE,
										   B.SOURCE_NAME,
										   C.VALUE ORD_TYPE,
										   C.NAME ORD_TYPE_DESC,
										   0 WMS_ORD_COUNT,
										   0 AUDIT_COUNT,
										   0 NOSHIP_COUNT,
										   0 DEBIT_COUNT
									  FROM F0513 A
										   LEFT JOIN F000902 B
											  ON B.SOURCE_TYPE = ISNULL (A.SOURCE_TYPE, '01')
										   LEFT JOIN VW_F000904_LANG C
										   ON C.TOPIC = 'F050001' AND C.SUBTOPIC = 'ORD_TYPE' AND C.LANG = '{Current.Lang}'
									 WHERE     A.DC_CODE = @p0
										   AND A.GUP_CODE = @p1
										   AND A.CUST_CODE = @p2) W
								   LEFT JOIN
								   (  SELECT A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE,
											 COUNT (C.DC_CODE) WMS_ORD_COUNT
										FROM F0513 A
											 INNER JOIN F050801 C
												ON     C.DC_CODE = A.DC_CODE
												   AND C.GUP_CODE = A.GUP_CODE
												   AND C.CUST_CODE = A.CUST_CODE
												   AND C.DELV_DATE = A.DELV_DATE
												   AND C.PICK_TIME = A.PICK_TIME
												   AND NOT EXISTS
														  (SELECT 1
															 FROM F05030101 D
																  INNER JOIN F050301 E
																	 ON     E.DC_CODE =
																			   D.DC_CODE
																		AND E.GUP_CODE =
																			   D.GUP_CODE
																		AND E.CUST_CODE =
																			   D.CUST_CODE
																		AND E.ORD_NO =
																			   D.ORD_NO
															WHERE     E.PROC_FLAG = '9'
																  AND C.DC_CODE =
																		 D.DC_CODE
																  AND C.GUP_CODE =
																		 D.GUP_CODE
																  AND C.CUST_CODE =
																		 D.CUST_CODE
																  AND C.WMS_ORD_NO =
																		 D.WMS_ORD_NO)
									   WHERE     A.DC_CODE = @p0
											 AND A.GUP_CODE = @p1
											 AND A.CUST_CODE = @p2
									GROUP BY A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE) X
									  ON     W.DC_CODE = X.DC_CODE
										 AND W.GUP_CODE = X.GUP_CODE
										 AND W.CUST_CODE = X.CUST_CODE
										 AND W.DELV_DATE = X.DELV_DATE
										 AND W.PICK_TIME = X.PICK_TIME
										 AND W.ORD_TYPE = X.ORD_TYPE
								   LEFT JOIN
								   (  SELECT A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE,
											 COUNT (C.DC_CODE) AUDIT_COUNT
										FROM F0513 A
											 INNER JOIN F050801 C
												ON     C.DC_CODE = A.DC_CODE
												   AND C.GUP_CODE = A.GUP_CODE
												   AND C.CUST_CODE = A.CUST_CODE
												   AND C.DELV_DATE = A.DELV_DATE
												   AND C.PICK_TIME = A.PICK_TIME
												   AND C.STATUS IN (2, 5, 6)
									GROUP BY A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE) Y
									  ON     W.DC_CODE = Y.DC_CODE
										 AND W.GUP_CODE = Y.GUP_CODE
										 AND W.CUST_CODE = Y.CUST_CODE
										 AND W.DELV_DATE = Y.DELV_DATE
										 AND W.PICK_TIME = Y.PICK_TIME
										 AND W.ORD_TYPE = X.ORD_TYPE
								   LEFT JOIN (  SELECT A.DC_CODE,
													   A.GUP_CODE,
													   A.CUST_CODE,
													   A.DELV_DATE,
													   A.PICK_TIME,
													   A.SOURCE_TYPE,
													   C.ORD_TYPE,
													   COUNT (C.DC_CODE) NOSHIP_COUNT
												  FROM F0513 A
													   INNER JOIN F050801 C
														  ON     C.DC_CODE = A.DC_CODE
															 AND C.GUP_CODE = A.GUP_CODE
															 AND C.CUST_CODE = A.CUST_CODE
															 AND C.DELV_DATE = A.DELV_DATE
															 AND C.PICK_TIME = A.PICK_TIME
															 AND C.STATUS = '9'
															 AND NOT EXISTS
																	(SELECT 1
																	   FROM F05030101 D
																			INNER JOIN F050301 E
																			   ON     E.DC_CODE =
																						 D.DC_CODE
																				  AND E.GUP_CODE =
																						 D.GUP_CODE
																				  AND E.CUST_CODE =
																						 D.CUST_CODE
																				  AND E.ORD_NO =
																						 D.ORD_NO
																	  WHERE     E.PROC_FLAG = '9'
																			AND C.DC_CODE =
																				   D.DC_CODE
																			AND C.GUP_CODE =
																				   D.GUP_CODE
																			AND C.CUST_CODE =
																				   D.CUST_CODE
																			AND C.WMS_ORD_NO =
																				   D.WMS_ORD_NO)
											  GROUP BY A.DC_CODE,
													   A.GUP_CODE,
													   A.CUST_CODE,
													   A.DELV_DATE,
													   A.PICK_TIME,
													   A.SOURCE_TYPE,
													   C.ORD_TYPE) Z
									  ON     W.DC_CODE = Z.DC_CODE
										 AND W.GUP_CODE = Z.GUP_CODE
										 AND W.CUST_CODE = Z.CUST_CODE
										 AND W.DELV_DATE = Z.DELV_DATE
										 AND W.PICK_TIME = Z.PICK_TIME
										 AND W.ORD_TYPE = X.ORD_TYPE
								   LEFT JOIN
								   (  SELECT A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE,
											 COUNT (C.DC_CODE) DEBIT_COUNT
										FROM F0513 A
											 INNER JOIN F050801 C
												ON     C.DC_CODE = A.DC_CODE
												   AND C.GUP_CODE = A.GUP_CODE
												   AND C.CUST_CODE = A.CUST_CODE
												   AND C.DELV_DATE = A.DELV_DATE
												   AND C.PICK_TIME = A.PICK_TIME
												   AND C.STATUS IN (5, 6)
									GROUP BY A.DC_CODE,
											 A.GUP_CODE,
											 A.CUST_CODE,
											 A.DELV_DATE,
											 A.PICK_TIME,
											 A.SOURCE_TYPE,
											 C.ORD_TYPE) U
									  ON     W.DC_CODE = U.DC_CODE
										 AND W.GUP_CODE = U.GUP_CODE
										 AND W.CUST_CODE = U.CUST_CODE
										 AND W.DELV_DATE = U.DELV_DATE
										 AND W.PICK_TIME = U.PICK_TIME
										 AND W.ORD_TYPE = X.ORD_TYPE) tb
					 WHERE     tb.AUDIT_COUNT + tb.NOSHIP_COUNT > 0
						   AND ( (tb.AUDIT_COUNT + tb.NOSHIP_COUNT) <> tb.DEBIT_COUNT)
						";
			if (notOrder)
				sql += " AND tb.SOURCE_TYPE <> '01'";
			if (isB2c)
				sql += " AND tb.ORD_TYPE = '1' ";
			sql += "					ORDER BY tb.DELV_DATE, tb.PICK_TIME";

			var result = SqlQuery<F0513WithF050801Batch>(sql, param.ToArray());

			return result;
		}
		#endregion

		public IQueryable<F0513Data> GetF0513Datas(string dcCode, string gupCode, string custCode, string delvDate)
		{
			var parameter = new List<SqlParameter>() {
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", delvDate),
						};

			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.ALL_ID,A.PICK_TIME ASC) ROWNUM,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,B.ALL_ID,B.ALL_COMP 
                         FROM F0513 A 
                        INNER JOIN (
                           SELECT F1947.*
                           FROM F1947
                           LEFT JOIN F194704
                               ON F1947.DC_CODE = F194704.DC_CODE
                               AND F1947.ALL_ID = F194704.ALL_ID
                           WHERE F1947.DC_CODE = @p0
                           AND (F194704.ALL_ID IS NULL OR F194704.GUP_CODE = @p1 AND F194704.CUST_CODE = @p2)
                           ) B 
                           ON B.DC_CODE = A.DC_CODE 
                          AND A.ALL_ID = B.ALL_ID 
                        WHERE A.DC_CODE = @p0 
                          AND A.GUP_CODE = @p1 
                          AND A.CUST_CODE = @p2 
                          AND A.DELV_DATE = @p3
                          AND A.PROC_FLAG IN ('0','1','2') 
                        ORDER BY A.ALL_ID,A.PICK_TIME ";

			var result = SqlQuery<F0513Data>(sql, parameter.ToArray());

			return result;

		}

    public IQueryable<BatchPickNoList> GetBatchPickNoList(string dcCode, string gupCode, string custCode, string sourceType, string custCost, DateTime? DelvDate = null, string ISPRINTED = "0")
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", ISPRINTED) {SqlDbType = System.Data.SqlDbType.VarChar }
      };


			var sql = $@"SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.ORD_TYPE,E.NAME ORD_TYPE_NAME,A.CUST_COST,C.NAME CUST_COST_NAME,A.FAST_DEAL_TYPE,D.NAME FAST_DEAL_TYPE_NAME,A.SOURCE_TYPE,B.SOURCE_NAME,ATFL_N_PICK_CNT,ATFL_B_PICK_CNT+ATFL_S_PICK_CNT AS ATFL_B_PICK_CNT,ATFL_NP_PICK_CNT,ATFL_BP_PICK_CNT+ATFL_SP_PICK_CNT AS ATFL_BP_PICK_CNT,A.PDA_PICK_PERCENT * 100 AS PDA_PICK_PERCENT,F.CROSS_NAME,A.REPICK_CNT, A.ORDER_PROC_TYPE,
       CASE WHEN A.ORDER_PROC_TYPE ='1' THEN '是' ELSE NULL END ORDER_PROC_TYPE_NAME
										FROM F0513 A
										JOIN F000902 B
										ON B.SOURCE_TYPE = ISNULL(A.SOURCE_TYPE,'01')
										LEFT JOIN VW_F000904_LANG C
										ON C.TOPIC='F050101' 
										AND C.SUBTOPIC='CUST_COST'
										AND C.VALUE = A.CUST_COST
										AND C.LANG ='{Current.Lang}'
										LEFT JOIN VW_F000904_LANG D
										ON D.TOPIC='F050101' 
										AND D.SUBTOPIC='FAST_DEAL_TYPE'
										AND D.VALUE = A.FAST_DEAL_TYPE
										AND D.LANG ='{Current.Lang}'
                    LEFT JOIN VW_F000904_LANG E
										 ON E.TOPIC='F050001' 
										AND E.SUBTOPIC='ORD_TYPE'
										AND E.VALUE = A.ORD_TYPE
										AND E.LANG ='{Current.Lang}'
                    LEFT JOIN F0001 F
                                        ON A.MOVE_OUT_TARGET=F.CROSS_CODE
                                            AND F.CROSS_TYPE='01'
										WHERE A.ISPRINTED = @p3
										  AND (A.ATFL_N_PICK_CNT>0 OR 
											    A.ATFL_B_PICK_CNT>0 OR 
											    A.ATFL_S_PICK_CNT>0 OR 
											    A.ATFL_NP_PICK_CNT>0 OR 
            							A.ATFL_BP_PICK_CNT>0 OR 
											    A.ATFL_SP_PICK_CNT>0)
										  AND A.DC_CODE = @p0
										  AND A.GUP_CODE = @p1
										  AND A.CUST_CODE = @p2
                      AND A.PROC_FLAG <>'9' ";

			if(!string.IsNullOrEmpty(sourceType))
			{
				sql += "AND (CASE WHEN A.SOURCE_TYPE='' OR A.SOURCE_TYPE IS NULL THEN '01' ELSE A.SOURCE_TYPE END) = @p" + parms.Count;
        parms.Add(new SqlParameter("@p" + parms.Count, sourceType) { SqlDbType = System.Data.SqlDbType.VarChar });
			}

			if (!string.IsNullOrWhiteSpace(custCost))
			{
				sql += " AND A.CUST_COST = @p"+parms.Count;
        parms.Add(new SqlParameter("@p" + parms.Count, custCost) { SqlDbType = System.Data.SqlDbType.NVarChar });
			}

      if(DelvDate.HasValue)
      {
        sql += " AND A.DELV_DATE = @p" + parms.Count;
        parms.Add(new SqlParameter("@p" + parms.Count, DelvDate.Value) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }

      var result = SqlQueryWithSqlParameterSetDbType<BatchPickNoList>(sql, parms.ToArray());
      return result;
		}

    public IQueryable<SinglePickingReportData> GetSinglePickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvTime, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var parm = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) {SqlDbType = System.Data.SqlDbType.VarChar }
      };

      var sql2 = string.Empty;

			if(delvTime.HasValue)
			{
        sql2 += " AND A.DELV_DATE = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", delvTime.Value) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
      if (!string.IsNullOrEmpty(pickTime))
			{
				sql2 += " AND A.PICK_TIME = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickTime) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      if (!string.IsNullOrEmpty(pickOrdNo))
			{
				sql2 += " AND A.PICK_ORD_NO = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      //是否排除補揀單 揀貨單列印-補印批量揀貨單用
      if (IsCheckNotRePick)
        sql2 += " AND A.PICK_TYPE != '5'";

      var sql = $@"SELECT I.GUP_NAME,
								J.CUST_NAME,
								A.DELV_DATE,
								A.PICK_TIME,
								A.PICK_ORD_NO,
								F.WAREHOUSE_NAME,
								F.PICK_FLOOR FLOOR,
			            		CASE F.TMPR_TYPE
			            			WHEN '01' THEN N'常溫'
			            			WHEN '02' THEN N'低溫'
			            			WHEN '03' THEN N'冷凍'
			            		END TMPR_TYPE_NAME,
			            		E.AREA_NAME,
			            		CASE WHEN LEN (B.PICK_LOC) = 9
			            			THEN SUBSTRING (B.PICK_LOC, 1, 1)
			            					+ '-'
			            					+ SUBSTRING (B.PICK_LOC, 2, 2)
			            					+ '-'
			            					+ SUBSTRING (B.PICK_LOC, 4, 2)
			            					+ '-'
			            					+ SUBSTRING (B.PICK_LOC, 6, 2)
			            					+ '-'
			            					+ SUBSTRING (B.PICK_LOC, 8, 2)
			            			ELSE B.PICK_LOC END PICK_LOC,
			            		B.ITEM_CODE,
                                G.ITEM_NAME,
                                L.CUST_ORD_NO,
			            		SUM(B.B_PICK_QTY) B_PICK_QTY,
			            		L.MEMO,
			            		G.EAN_CODE1,G.EAN_CODE2,G.EAN_CODE3,
			            		J.SHORT_NAME ,
			            		(SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='CUST_COST' AND VALUE = H.CUST_COST  AND LANG='{Current.Lang}') CUST_COST,
			            		(SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='FAST_DEAL_TYPE' AND VALUE = H.FAST_DEAL_TYPE  AND LANG='{Current.Lang}') FAST_DEAL_TYPE,
			            		--CASE WHEN A.SPLIT_TYPE = '01'then  A.SPLIT_CODE + ISNULL((select TOP(1) WAREHOUSE_NAME from F1980 where DC_CODE = A.DC_CODE AND WAREHOUSE_ID=A.SPLIT_CODE),'')
			            		--WHEN A.SPLIT_TYPE = '02' THEN A.SPLIT_CODE + ISNULL((SELECT PK_NAME FROM F191206 WHERE DC_CODE = A.DC_CODE AND PK_AREA=A.SPLIT_CODE),'')  
                      --WHEN A.SPLIT_TYPE = '03' THEN '跨PK區' END AS SPLIT_CODE,
								CASE WHEN ISNULL(A.PK_AREA,'') ='' THEN '跨PK區' ELSE A.PK_AREA+' '+A.PK_AREA_NAME END SPLIT_CODE,
			          CASE WHEN A.NEXT_STEP = '2' THEN  (select B1.COLLECTION_NAME FROM F051301 A1 JOIN F1945 B1 ON B1.DC_CODE = A1.DC_CODE AND B1.COLLECTION_CODE = A1.COLLECTION_CODE AND B1.CELL_TYPE = A1.CELL_TYPE WHERE A1.DC_CODE = A.DC_CODE AND A1.WMS_NO= A.SPLIT_CODE) 
                ELSE  (select TOP(1) NAME FROM VW_F000904_LANG WHERE TOPIC = 'F051201' AND SUBTOPIC='NEXT_STEP' AND VALUE= A.NEXT_STEP AND LANG='{Current.Lang}') END NEXT_STEP,
			          CASE WHEN A.ORD_TYPE = '0' THEN N'B2B揀貨單' ELSE (SELECT TOP(1) NAME FROM VW_F000904_LANG WHERE TOPIC='F051201' AND SUBTOPIC='PICK_TYPE' AND VALUE=A.PICK_TYPE AND LANG='{Current.Lang}') END   ORD_TYPE,
			          CASE WHEN A.SPLIT_TYPE = '03' then A.SPLIT_CODE else 'NA' END WMS_NO,M.CROSS_NAME,
								CASE WHEN H.SOURCE_TYPE='13' THEN A.RTN_VNR_CODE ELSE '' END RTN_VNR_CODE,
			          CASE WHEN H.SOURCE_TYPE='13' THEN (SELECT TOP(1) VNR_NAME FROM F1908 WHERE VNR_CODE = A.RTN_VNR_CODE) ELSE '' END RTN_VNR_NAME,
                CASE 
	                WHEN A.ORDER_PROC_TYPE = '1' AND A.IS_NORTH_ORDER  = '1' THEN SUBSTRING(A.ORDER_ZIP_CODE,1,3) + ' 北北基'
                	WHEN A.ORDER_PROC_TYPE = '1' AND A.IS_NORTH_ORDER != '1' THEN SUBSTRING(A.ORDER_ZIP_CODE,1,3) + ' 非北北基'
                	ELSE NULL
                END ORDER_PROC_TYPE_NAME,
                B.SERIAL_NO
FROM F051201 A
JOIN F051202 B
ON B.DC_CODE = A.DC_CODE
AND B.GUP_CODE = A.GUP_CODE
AND B.CUST_CODE = A.CUST_CODE
AND B.PICK_ORD_NO = A.PICK_ORD_NO
JOIN F050801 C
ON C.DC_CODE = A.DC_CODE
AND C.GUP_CODE = A.GUP_CODE
AND C.CUST_CODE = A.CUST_CODE
AND C.WMS_ORD_NO = A.SPLIT_CODE
JOIN F1912 D
ON D.DC_CODE = B.DC_CODE
AND D.LOC_CODE = B.PICK_LOC
LEFT JOIN F1919 E
ON E.DC_CODE = D.DC_CODE
AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
AND E.AREA_CODE = D.AREA_CODE 
JOIN F1980 F
ON F.DC_CODE = D.DC_CODE
AND F.WAREHOUSE_ID = D.WAREHOUSE_ID
LEFT JOIN F1903 G WITH (NOLOCK)
ON G.GUP_CODE = B.GUP_CODE
AND G.CUST_CODE = B.CUST_CODE
AND G.ITEM_CODE = B.ITEM_CODE
JOIN F0513 H
ON H.DC_CODE = A.DC_CODE
AND H.GUP_CODE=  A.GUP_CODE
AND H.CUST_CODE = A.CUST_CODE
AND H.DELV_DATE = A.DELV_DATE
AND H.PICK_TIME = A.PICK_TIME
JOIN F1929 I
ON I.GUP_CODE = A.GUP_CODE
JOIN F1909 J
ON J.GUP_CODE = A.GUP_CODE
AND J.CUST_CODE = A.CUST_CODE
JOIN F05030101 K
ON K.DC_CODE = A.DC_CODE
AND K.GUP_CODE = A.GUP_CODE
AND K.CUST_CODE = A.CUST_CODE
AND K.WMS_ORD_NO = A.SPLIT_CODE
JOIN F050301 L
ON L.DC_CODE = K.DC_CODE
AND L.GUP_CODE = K.GUP_CODE
AND L.CUST_CODE = K.CUST_CODE
AND L.ORD_NO = K.ORD_NO
LEFT JOIN F0001 M 
ON A.MOVE_OUT_TARGET=M.CROSS_CODE 
AND M.CROSS_TYPE='01'
WHERE 
A.PICK_STATUS <>'9'     --排除取消揀貨單
AND A.SPLIT_TYPE ='03'  --單一揀貨
AND A.DISP_SYSTEM='0'   --人工倉
AND A.PICK_TOOL ='1'    --紙本揀貨
AND B.PICK_STATUS <>'9' --排除揀貨明細取消
AND ISNULL(G.VIRTUAL_TYPE,'') ='' --排除虛擬商品 
AND A.DC_CODE = @p0
AND A.GUP_CODE = @p1
AND A.CUST_CODE = @p2
{sql2}
GROUP BY  
  I.GUP_NAME,
  J.CUST_NAME,
  A.DELV_DATE,
  A.PICK_TIME,
  A.PICK_ORD_NO,
  F.WAREHOUSE_NAME,
  D.FLOOR,
  F.TMPR_TYPE,
  E.AREA_NAME,
  B.PICK_LOC,
  B.ITEM_CODE,
  G.ITEM_NAME,
  L.CUST_ORD_NO,
  L.MEMO,
  G.EAN_CODE1,G.EAN_CODE2,G.EAN_CODE3,
  J.SHORT_NAME ,
  H.CUST_COST,
  H.FAST_DEAL_TYPE,
  A.SPLIT_TYPE,A.SPLIT_CODE,A.NEXT_STEP,A.ORD_TYPE,F.PICK_FLOOR,A.PICK_TYPE,A.DC_CODE,M.CROSS_NAME,B.ROUTE_SEQ,H.SOURCE_TYPE,A.RTN_VNR_CODE ,A.PK_AREA,A.PK_AREA_NAME,
  A.ORDER_PROC_TYPE,A.ORDER_ZIP_CODE,A.IS_NORTH_ORDER,B.SERIAL_NO
ORDER BY A.IS_NORTH_ORDER DESC,A.PICK_ORD_NO,B.ROUTE_SEQ,B.ITEM_CODE";


			var result = SqlQuery<SinglePickingReportData>(sql, parm.ToArray());

			return result;
		}

    public IQueryable<BatchPickingReportData> GetBatchPickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvTime, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var parm = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) {SqlDbType = System.Data.SqlDbType.VarChar }
      };

      var sql2 = string.Empty;

      if (delvTime.HasValue)
      {
        sql2 += " AND A.DELV_DATE = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", delvTime.Value) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
      if (!string.IsNullOrEmpty(pickTime))
      {
        sql2 += " AND A.PICK_TIME = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickTime) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      if (!string.IsNullOrEmpty(pickOrdNo))
      {
        sql2 += " AND A.PICK_ORD_NO = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      //是否排除補揀單 揀貨單列印-補印批量揀貨單用
      if (IsCheckNotRePick)
        sql2 += " AND A.PICK_TYPE != '5'";
      var sql = $@"SELECT DISTINCT Q.DC_CODE ,Q.GUP_CODE,Q.CUST_CODE,Q.PICK_ORD_NO,S.CUST_ORD_NO INTO #KK  FROM F051202 Q
                    JOIN F05030101 R 
                    ON Q.DC_CODE = R.DC_CODE 
                    AND Q.GUP_CODE = R.GUP_CODE 
                    AND Q.CUST_CODE = R.CUST_CODE 
                    AND Q.WMS_ORD_NO = R.WMS_ORD_NO 
                    JOIN F050301 S 
                    ON R.DC_CODE = S.DC_CODE 
                    AND R.GUP_CODE = S.GUP_CODE 
                    AND R.CUST_CODE = S.CUST_CODE 
                    AND R.ORD_NO = S.ORD_NO
     JOIN F051201 A
     ON A.DC_CODE = Q.DC_CODE 
                    AND A.GUP_CODE = Q.GUP_CODE 
                    AND A.CUST_CODE = Q.CUST_CODE 
                    AND A.PICK_ORD_NO = Q.PICK_ORD_NO
     WHERE A.DC_CODE = @p0
	   AND A.GUP_CODE  = @p1
	   AND A.CUST_CODE = @p2
	   {sql2};
 
SELECT 
I.GUP_NAME,
                               J.CUST_NAME,
                               A.DELV_DATE,
                               A.PICK_TIME,
                               A.PICK_ORD_NO,
                               F.WAREHOUSE_NAME,
                               F.PICK_FLOOR FLOOR,
                   CASE F.TMPR_TYPE
                  WHEN '01' THEN N'常溫'
                  WHEN '02' THEN N'低溫'
                  WHEN '03' THEN N'冷凍'
                   END TMPR_TYPE_NAME,
                   E.AREA_NAME,
                   CASE
                  WHEN LEN (B.PICK_LOC) = 9
                  THEN
                     SUBSTRING (B.PICK_LOC, 1, 1)
                    + '-'
                    + SUBSTRING (B.PICK_LOC, 2, 2)
                    + '-'
                    + SUBSTRING (B.PICK_LOC, 4, 2)
                    + '-'
                    + SUBSTRING (B.PICK_LOC, 6, 2)
                    + '-'
                    + SUBSTRING (B.PICK_LOC, 8, 2)
                  ELSE B.PICK_LOC END PICK_LOC,
                 B.ITEM_CODE,
                                G.ITEM_NAME,
                    SUM(B.B_PICK_QTY) B_PICK_QTY,
                    G.EAN_CODE1,G.EAN_CODE2,G.EAN_CODE3,
                    J.SHORT_NAME ,
                    (SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='CUST_COST' AND VALUE = H.CUST_COST  AND LANG='{Current.Lang}') CUST_COST,
                 (SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='FAST_DEAL_TYPE' AND VALUE = H.FAST_DEAL_TYPE  AND LANG='{Current.Lang}') FAST_DEAL_TYPE,
                 --CASE WHEN A.SPLIT_TYPE = '01'then  A.SPLIT_CODE + '  ' + ISNULL((select TOP(1) WAREHOUSE_NAME from F1980 where DC_CODE = A.DC_CODE AND WAREHOUSE_ID=A.SPLIT_CODE),'')
                 --WHEN A.SPLIT_TYPE = '02' THEN A.SPLIT_CODE + '  ' + ISNULL((SELECT PK_NAME FROM F191206 WHERE DC_CODE = A.DC_CODE AND PK_AREA=A.SPLIT_CODE),'')  
                     -- WHEN A.SPLIT_TYPE = '03' THEN '跨PK區' END AS SPLIT_CODE,
        CASE WHEN ISNULL(A.PK_AREA,'') ='' THEN '跨PK區' ELSE A.PK_AREA+' '+A.PK_AREA_NAME END SPLIT_CODE,
                     (select TOP(1) NAME FROM VW_F000904_LANG WHERE TOPIC = 'F051201' AND SUBTOPIC='NEXT_STEP' AND VALUE= A.NEXT_STEP AND LANG='{Current.Lang}') NEXT_STEP,
                    CASE WHEN A.ORD_TYPE = '0' THEN N'B2B揀貨單' ELSE (SELECT TOP(1) NAME FROM VW_F000904_LANG WHERE TOPIC='F051201' AND SUBTOPIC='PICK_TYPE' AND VALUE=A.PICK_TYPE AND LANG='{Current.Lang}') END   ORD_TYPE,
                    CASE WHEN A.SPLIT_TYPE = '03' then A.SPLIT_CODE else 'NA' END WMS_NO, K.CROSS_NAME,U.DateList AS CUST_ORD_NO
                ,B.SERIAL_NO
FROM F051201 A
JOIN F051203 B
ON B.DC_CODE = A.DC_CODE
AND B.GUP_CODE = A.GUP_CODE
AND B.CUST_CODE = A.CUST_CODE
AND B.PICK_ORD_NO = A.PICK_ORD_NO
JOIN F1912 D
ON D.DC_CODE = B.DC_CODE
AND D.LOC_CODE = B.PICK_LOC
LEFT JOIN F1919 E
ON E.DC_CODE = D.DC_CODE
AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
AND E.AREA_CODE = D.AREA_CODE 
JOIN F1980 F
ON F.DC_CODE = D.DC_CODE
AND F.WAREHOUSE_ID = D.WAREHOUSE_ID
LEFT JOIN F1903 G WITH (NOLOCK)
ON G.GUP_CODE = B.GUP_CODE
AND G.CUST_CODE = B.CUST_CODE
AND G.ITEM_CODE = B.ITEM_CODE
JOIN F0513 H
ON H.DC_CODE = A.DC_CODE
AND H.GUP_CODE=  A.GUP_CODE
AND H.CUST_CODE = A.CUST_CODE
AND H.DELV_DATE = A.DELV_DATE
AND H.PICK_TIME = A.PICK_TIME
JOIN F1929 I
ON I.GUP_CODE = A.GUP_CODE
JOIN F1909 J
ON J.GUP_CODE = A.GUP_CODE
AND J.CUST_CODE = A.CUST_CODE
LEFT JOIN F0001 K
ON A.MOVE_OUT_TARGET = K.CROSS_CODE
 AND K.CROSS_TYPE='01'
JOIN (SELECT DISTINCT  T.DC_CODE,T.GUP_CODE,T.CUST_CODE,T.PICK_ORD_NO,STUFF((
 SELECT ', '+P.CUST_ORD_NO 
    from #KK P   
 where P.PICK_ORD_NO=T.PICK_ORD_NO --把name一樣的加起來
 FOR XML PATH('')),1,1,''
) AS DateList FROM #KK T) U
                    ON A.DC_CODE =U.DC_CODE
                    AND A.GUP_CODE = U.GUP_CODE
                    AND A.CUST_CODE = U.CUST_CODE
                    AND A.PICK_ORD_NO = U.PICK_ORD_NO
WHERE 
A.PICK_STATUS <>'9'     --排除取消揀貨單
AND A.SPLIT_TYPE <>'03'  --批量揀貨
AND A.DISP_SYSTEM='0'   --人工倉
AND A.PICK_TOOL ='1'    --紙本揀貨
AND B.PICK_STATUS <>'9' --排除揀貨明細取消
AND ISNULL(G.VIRTUAL_TYPE,'') ='' --排除虛擬商品 
AND A.DC_CODE = @p0
AND A.GUP_CODE = @p1
AND A.CUST_CODE = @p2
{sql2}
GROUP BY
  I.GUP_NAME,
  J.CUST_NAME,
  A.DELV_DATE,
  A.PICK_TIME,
  A.PICK_ORD_NO,
  F.WAREHOUSE_NAME,
  D.FLOOR,
  F.TMPR_TYPE,
  E.AREA_NAME,
  B.PICK_LOC,
  B.ITEM_CODE,
  G.ITEM_NAME,        
  G.EAN_CODE1,G.EAN_CODE2,G.EAN_CODE3,
  J.SHORT_NAME ,
  H.CUST_COST,
  H.FAST_DEAL_TYPE,
  A.SPLIT_TYPE,A.SPLIT_CODE,A.NEXT_STEP,A.ORD_TYPE,F.PICK_FLOOR,A.PICK_TYPE,A.DC_CODE,K.CROSS_NAME,B.ROUTE_SEQ,
  H.SOURCE_TYPE,A.RTN_VNR_CODE,A.PK_AREA,A.PK_AREA_NAME,U.DateList,B.SERIAL_NO
ORDER BY A.PICK_ORD_NO,B.ROUTE_SEQ,B.ITEM_CODE

DROP TABLE #KK";

      var result = SqlQuery<BatchPickingReportData>(sql, parm.ToArray());

      return result;
    }


    public IQueryable<SinglePickingTickerData> GetSinglePickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvTime, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var parm = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) {SqlDbType = System.Data.SqlDbType.VarChar }
      };


      var sql2 = string.Empty;

			if (delvTime.HasValue)
			{
				sql2 += " AND A.DELV_DATE = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", delvTime.Value) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
      if (!string.IsNullOrEmpty(pickTime))
			{
				sql2 += " AND A.PICK_TIME = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickTime) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      if (!string.IsNullOrEmpty(pickOrdNo))
			{
				sql2 += " AND B.PICK_ORD_NO = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      //是否排除補揀單 揀貨單列印-補印批量揀貨單用
      if (IsCheckNotRePick)
        sql2 += " AND B.PICK_TYPE != '5'";

      var sql = $@"select B.PICK_ORD_NO ,B.SPLIT_CODE, CASE WHEN C.CROSS_NAME = '' THEN '' ELSE '跨庫目的地:' + C.CROSS_NAME END CROSS_NAME,
CASE WHEN A.SOURCE_TYPE='13' THEN B.RTN_VNR_CODE ELSE '' END RTN_VNR_CODE from F0513 A
							join F051201 B
							on A.DC_CODE = B.DC_CODE
							and A.GUP_CODE =B.GUP_CODE
							and A.CUST_CODE =B.CUST_CODE
							and A.DELV_DATE =B.DELV_DATE
							and A.PICK_TIME =B.PICK_TIME
							 AND NOT EXISTS                                     -- 排除取消的訂單
							(SELECT 1
							FROM F05030101 Q
							INNER JOIN F050301 R
							ON     Q.DC_CODE = R.DC_CODE
								AND Q.GUP_CODE = R.GUP_CODE
								AND Q.CUST_CODE = R.CUST_CODE
								AND Q.ORD_NO = R.ORD_NO
							WHERE Q.DC_CODE = B.DC_CODE
							AND Q.GUP_CODE = B.GUP_CODE
							AND Q.CUST_CODE = B.CUST_CODE
							AND Q.WMS_ORD_NO = B.SPLIT_CODE 
							AND R.PROC_FLAG = '9')
                            LEFT JOIN F0001 C 
                                ON B.MOVE_OUT_TARGET=C.CROSS_CODE AND C.CROSS_TYPE ='01'
							WHERE B.PICK_TOOL = '2'
							AND B.DISP_SYSTEM = '0'
							AND B.SPLIT_TYPE = '03'
							AND B.PICK_STATUS<>9
							AND A.DC_CODE = @p0
							 AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2
							{sql2}";

			var result = SqlQuery<SinglePickingTickerData>(sql, parm.ToArray());

			return result;
		}

    public IQueryable<BatchPickingTickerData> GetBatchPickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvTime, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var parm = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) {SqlDbType = System.Data.SqlDbType.VarChar }
      };


      var sql2 = string.Empty;

			if (delvTime.HasValue)
			{
				sql2 += " AND A.DELV_DATE = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", delvTime.Value) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
      if (!string.IsNullOrEmpty(pickTime))
			{ 
				sql2 += " AND A.PICK_TIME = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickTime) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      if (!string.IsNullOrEmpty(pickOrdNo))
			{
				sql2 += " AND B.PICK_ORD_NO = @p" + parm.Count;
        parm.Add(new SqlParameter($"@p{parm.Count}", pickOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      //是否排除補揀單 揀貨單列印-補印批量揀貨單用
      if (IsCheckNotRePick)
        sql2 += " AND B.PICK_TYPE != '5'";


      var sql = $@"select B.PICK_ORD_NO, CASE WHEN C.CROSS_NAME = '' THEN '' ELSE '跨庫目的地:' + C.CROSS_NAME END CROSS_NAME from F0513 A
							join F051201 B
							on A.DC_CODE = B.DC_CODE
							and A.GUP_CODE =B.GUP_CODE
							and A.CUST_CODE =B.CUST_CODE
							and A.DELV_DATE =B.DELV_DATE
							and A.PICK_TIME =B.PICK_TIME
							 AND NOT EXISTS                                     -- 排除取消的訂單
							(SELECT 1
							FROM F05030101 Q
							INNER JOIN F050301 R
							ON     Q.DC_CODE = R.DC_CODE
								AND Q.GUP_CODE = R.GUP_CODE
								AND Q.CUST_CODE = R.CUST_CODE
								AND Q.ORD_NO = R.ORD_NO
							WHERE Q.DC_CODE = B.DC_CODE
							AND Q.GUP_CODE = B.GUP_CODE
							AND Q.CUST_CODE = B.CUST_CODE
							AND Q.WMS_ORD_NO = B.SPLIT_CODE 
							AND R.PROC_FLAG = '9')
                            LEFT JOIN F0001 C 
                                ON B.MOVE_OUT_TARGET = C.CROSS_CODE
                                AND C.CROSS_TYPE = '01'
							WHERE B.PICK_TOOL = '2'
							AND B.DISP_SYSTEM = '0'
							AND B.SPLIT_TYPE <> '03'
							AND B.PICK_STATUS<>9
							AND A.DC_CODE = @p0
							 AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2
							{sql2}";

			var result = SqlQuery<BatchPickingTickerData>(sql, parm.ToArray());

			return result;
		}

		public IQueryable<BatchPickData> GetNotCancelBatchPickData(string dcCode,string gupCode,string custCode, DateTime? delvDate, string pickTime)
		{
			var parms = new List<object> { dcCode,gupCode,custCode, delvDate, pickTime };
			var sql = $@" SELECT A.PICK_ORD_NO,
						 A.DELV_DATE,
						A.PICK_TIME,
						(SELECT CROSS_NAME FROM F0001 where CROSS_CODE = A.MOVE_OUT_TARGET AND CROSS_TYPE = '01') CROSS_NAME,
						(SELECT NAME FROM VW_F000904_LANG where TOPIC = 'F051201' and SUBTOPIC = 'PICK_STATUS' AND VALUE = A.PICK_STATUS AND LANG = '{Current.Lang}') PICK_STATUS,
           (SELECT NAME FROM VW_F000904_LANG where TOPIC = 'F051201' and SUBTOPIC = 'PICK_TOOL' AND VALUE = A.PICK_TOOL AND LANG ='{Current.Lang}') PICK_TOOL ,
						(CASE A.SPLIT_TYPE WHEN '01' THEN (SELECT WAREHOUSE_NAME from F1980 where  WAREHOUSE_ID = A.SPLIT_CODE and DC_CODE = A.DC_CODE)
						WHEN '02' THEN (SELECT PK_NAME from F191206 where PK_AREA = A.SPLIT_CODE) END)  SPLIT_CODE
						FROM F051201 A
						JOIN F0513 B 
						ON A.DC_CODE = B.DC_CODE 
						AND A.GUP_CODE = B.GUP_CODE 
						AND A.CUST_CODE  = B.CUST_CODE 
						AND A.DELV_DATE = B.DELV_DATE 
						AND A.PICK_TIME =B.PICK_TIME 
						WHERE B.CUST_COST  = 'MoveOut'
            AND A.DC_CODE = @p0
            AND A.GUP_CODE = @p1
            AND A.CUST_CODE = @p2
						AND A.DELV_DATE = @p3
						AND A.PICK_TIME = @p4
            AND A.PICK_STATUS <>'9' ";
			return SqlQuery<BatchPickData>(sql, parms.ToArray());
		}
		public IQueryable<F0513> GetDatasByPickOrdNos(string dcCode,string gupCode,string custCode,List<string> pickOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql2 = parms.CombineNotNullOrEmptySqlInParameters(" AND B.PICK_ORD_NO", pickOrdNos);
			var sql = $@" SELECT *
                     FROM F0513 A
                    WHERE EXISTS (
                    SELECT 1 
                      FROM F051201 B
                     WHERE B.DC_CODE = @p0
                       AND B.GUP_CODE = @p1
                       AND B.CUST_CODE = @p2
                       {sql2} 
                       AND A.DC_CODE = B.DC_CODE
                       AND A.GUP_CODE = B.GUP_CODE
                       AND A.CUST_CODE = B.CUST_CODE
                       AND A.DELV_DATE = B.DELV_DATE
                       AND A.PICK_TIME = B.PICK_TIME) ";
			return SqlQuery<F0513>(sql, parms.ToArray());
		}

		public F0513 GetF0513(string dcCode,string gupCode,string custCode,DateTime? delvDate,string pickTime)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",delvDate.Value){SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p4",pickTime){SqlDbType = SqlDbType.VarChar }
			};
			var sql = @" SELECT TOP (1) *
                     FROM F0513 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4 ";
			return SqlQuery<F0513>(sql, parms.ToArray()).FirstOrDefault();
		}

    public string GetSourceType(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3",delvDate.Value){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p4",pickTime){SqlDbType = SqlDbType.VarChar }
      };

      var sql = @" SELECT TOP (1) SOURCE_TYPE
                     FROM F0513 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4 ";

      return SqlQuery<string>(sql, param.ToArray()).FirstOrDefault();
    }

    public void UpdatePrinted(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",DateTime.Now){ SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p1",Current.Staff){ SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2",Current.StaffName){ SqlDbType = SqlDbType.NVarChar},
        new SqlParameter("@p3",dcCode){ SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p4",gupCode){ SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p5",custCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p6",delvDate.Value){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p7",pickTime){SqlDbType = SqlDbType.VarChar }
      };

      var sql = @"
                UPDATE 
                  F0513 
                SET 
                  ISPRINTED = '1',
                  UPD_DATE = @p0,
                  UPD_STAFF = @p1,
                  UPD_NAME = @p2
                WHERE 
                  DC_CODE = @p3
                  AND GUP_CODE = @p4
                  AND CUST_CODE = @p5
                  AND DELV_DATE = @p6
                  AND PICK_TIME = @p7
                ";

      ExecuteSqlCommand(sql, param.ToArray());
    }
  }
}




