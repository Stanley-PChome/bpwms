using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Pda.Entitues;
using System.Data;

namespace Wms3pl.Datas.F05
{
	public partial class F050801Repository : RepositoryBase<F050801, Wms3plDbContext, F050801Repository>
	{
		public IQueryable<F050801WithF055001> GetF050801WithF055001Datas(string dcCode, string gupCode, string custCode,
									 DateTime delvDate, string pickTime, string wmsOrdNo, string pastNo, string itemCode, string ordNo)
		{
			var parameters = new List<SqlParameter>
								 {
												new SqlParameter("@p0", dcCode),
												new SqlParameter("@p1", gupCode),
												new SqlParameter("@p2", custCode),
												new SqlParameter("@p3", delvDate),
												new SqlParameter("@p4", pickTime),
												new SqlParameter("@p8", ordNo)
								 };
			var sql = @"Select DISTINCT A.GUP_CODE,A.CUST_CODE,A.DC_CODE,A.WMS_ORD_NO,A.DELV_DATE,
																		 B.PICK_TIME,B.PAST_NO,A.STATUS
															From F050801 A
															Join F055001 B
																		 On A.GUP_CODE=B.GUP_CODE AND A.CUST_CODE=B.CUST_CODE AND A.DC_CODE=B.DC_CODE AND A.WMS_ORD_NO=B.WMS_ORD_NO
															Left Join F050802 C
																		 On A.GUP_CODE=C.GUP_CODE AND A.CUST_CODE=C.CUST_CODE AND A.DC_CODE=C.DC_CODE AND A.WMS_ORD_NO=C.WMS_ORD_NO
														WHERE A.STATUS IN(1,2)	--已包裝扣帳前出貨抽稽可查詢
																		 AND A.DC_CODE = @p0
                                                                         AND A.GUP_CODE = @p1
                                                                         AND A.CUST_CODE = @p2
																		 AND A.DELV_DATE = @p3
																		 AND A.PICK_TIME = @p4 ";

			if (!string.IsNullOrWhiteSpace(wmsOrdNo) && wmsOrdNo != "''")
			{
				sql += "     AND A.WMS_ORD_NO = @p5";
				parameters.Add(new SqlParameter("@p5", wmsOrdNo));
			}
			if (!string.IsNullOrWhiteSpace(pastNo) && pastNo != "''")
			{
				sql += "     AND B.PAST_NO = @p6";
				parameters.Add(new SqlParameter("@p6", pastNo));
			}
			if (!string.IsNullOrWhiteSpace(itemCode) && itemCode != "''")
			{
				sql += "     AND C.ITEM_CODE = @p7";
				parameters.Add(new SqlParameter("@p7", itemCode));
			}


			// 增加訂單編號與該訂單是否有併單
			var sql2 = @"SELECT A1.*, D1.ORD_NO, C.CUST_ORD_NO,
									( 
													-- 是否併單
													SELECT COUNT(*)
													FROM F05030101 D2
													WHERE D2.ORD_NO = D1.ORD_NO
																			AND D2.DC_CODE = D1.DC_CODE
																			AND D2.GUP_CODE = D1.GUP_CODE
																			AND D2.CUST_CODE = D1.CUST_CODE
																			AND EXISTS (
																				SELECT 1
																				FROM F05030101 D3
																				WHERE D3.WMS_ORD_NO = D2.WMS_ORD_NO
																				AND D3.DC_CODE = D2.DC_CODE
																				AND D3.GUP_CODE = D2.GUP_CODE
																				AND D3.CUST_CODE = D2.CUST_CODE
																				AND (
																									SELECT COUNT(*)
																									FROM F05030101 D4
																									WHERE D4.WMS_ORD_NO = D3.WMS_ORD_NO
																									AND D4.DC_CODE = D3.DC_CODE
																									AND D4.GUP_CODE = D3.GUP_CODE
																									AND D4.CUST_CODE = D3.CUST_CODE
																								) > 1
																			) 
													) ISMERGE
									FROM 
									(" + sql + @") A1, F05030101 D1, F050301 C
									WHERE A1.WMS_ORD_NO = D1.WMS_ORD_NO
									AND A1.DC_CODE = D1.DC_CODE
									AND A1.GUP_CODE = D1.GUP_CODE
									AND A1.CUST_CODE = D1.CUST_CODE
									AND C.DC_CODE = D1.DC_CODE
									AND C.GUP_CODE = D1.GUP_CODE
									AND C.CUST_CODE = D1.CUST_CODE
									AND C.ORD_NO = D1.ORD_NO
									AND D1.ORD_NO = CASE WHEN @p8 = '' THEN D1.ORD_NO ELSE @p8 END
									ORDER BY A1.WMS_ORD_NO";

			var result = SqlQuery<F050801WithF055001>(sql2, parameters.ToArray());
			return result;
		}

		/// <summary>
		/// 判斷是否有併單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNoList"></param>
		/// <returns></returns>
		public IQueryable<F05030101> GetIsMerge(string dcCode, string gupCode, string custCode, IEnumerable<string> ordNoList)
		{
			var parameters = new List<object>{
												dcCode,
												gupCode,
												custCode
								 };

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("D.ORD_NO", ordNoList, ref paramStartIndex);

			var sql = @"SELECT DISTINCT D.*
										FROM F05030101 D
										WHERE  D.DC_CODE = @p0
										AND D.GUP_CODE = @p1
										AND D.CUST_CODE = @p2
										AND " + inSql +
				 @" AND ( 
														SELECT COUNT(*)
														FROM F05030101 D2
														WHERE D2.ORD_NO = D.ORD_NO
																				AND D2.DC_CODE = D.DC_CODE
																				AND D2.GUP_CODE = D.GUP_CODE
																				AND D2.CUST_CODE = D.CUST_CODE
																				AND EXISTS (
																					SELECT 1
																					FROM F05030101 D3
																					WHERE D3.WMS_ORD_NO = D2.WMS_ORD_NO
																					AND D3.DC_CODE = D2.DC_CODE
																					AND D3.GUP_CODE = D2.GUP_CODE
																					AND D3.CUST_CODE = D2.CUST_CODE
																					AND (
																										SELECT COUNT(*)
																										FROM F05030101 D4
																										WHERE D4.WMS_ORD_NO = D3.WMS_ORD_NO
																										AND D4.DC_CODE = D3.DC_CODE
																										AND D4.GUP_CODE = D3.GUP_CODE
																										AND D4.CUST_CODE = D3.CUST_CODE
																									) > 1
																				) 
														) > 0";


			var result = SqlQuery<F05030101>(sql, parameters.ToArray());
			return result;
		}

		/// <summary>
		/// 取得相同訂單的出貨單(=>配庫拆單)是否已被扣帳或裝車的出貨單數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNoList"></param>
		/// <returns></returns>
		public List<string> GetSpiltPostingOrdCount(string dcCode, string gupCode, string custCode, IEnumerable<string> wmsOrdNoList)
		{
			var parameters = new List<object>{
												dcCode,
												gupCode,
												custCode,
								 };

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("AND A.WMS_ORD_NO", wmsOrdNoList, ref paramStartIndex);
			var sql = string.Format(@"
					SELECT E.WMS_ORD_NO
						FROM F050801 A
								 JOIN F05030101 B
										ON     A.DC_CODE = B.DC_CODE
											 AND A.GUP_CODE = B.GUP_CODE
											 AND A.CUST_CODE = B.CUST_CODE
											 AND A.WMS_ORD_NO = B.WMS_ORD_NO
								 JOIN F05030101 D
										ON     B.DC_CODE = D.DC_CODE
											 AND B.GUP_CODE = D.GUP_CODE
											 AND B.CUST_CODE = D.CUST_CODE
											 AND B.ORD_NO = D.ORD_NO
								 JOIN F050801 E
										ON     D.DC_CODE = E.DC_CODE
											 AND D.GUP_CODE = E.GUP_CODE
											 AND D.CUST_CODE = E.CUST_CODE
											 AND D.WMS_ORD_NO = E.WMS_ORD_NO
					 WHERE     A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 {0}
								 AND E.STATUS IN (5, 6)", inSql);

			var result = SqlQuery<string>(sql, parameters.ToArray()).ToList();

			return result;
		}

		/// <summary>
		/// 將指定的出貨單號狀態設為取消
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="wmsOdrNoList"></param>
		public void UpdateStatusCancel(string gupCode, string custCode, string dcCode, IEnumerable<string> wmsOdrNoList)
		{
			var parameters = new List<object>
						{
								Current.Staff,
								Current.StaffName,
                DateTime.Now,
                gupCode,
								custCode,
								dcCode
						};

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("WMS_ORD_NO", wmsOdrNoList, ref paramStartIndex);
			var sql = @"Update F050801 Set STATUS = 9, UPD_STAFF = @p0, UPD_NAME = @p1, UPD_DATE = @p2
						Where GUP_CODE = @p3
						And CUST_CODE = @p4
						And DC_CODE = @p5
						And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		#region 未出貨訂單查詢
		public IQueryable<F050801NoShipOrders> GetF050801NoShipOrders(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string status, string ordNo, string custOrdNo)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", delvDate),
						};

			string sqlFormat = @" SELECT * FROM
								(SELECT A.DC_CODE,
                                       A.GUP_CODE,
                                       A.CUST_CODE,
                                       A.DELV_DATE,
                                       A.PICK_TIME,
                                       E.CUST_ORD_NO,
                                       B.ORD_NO,
                                       C.PICK_NAME,
                                       D.PACKAGE_NAME,
                                       ( CASE Max (E.PROC_FLAG)
                                           WHEN '9' THEN '9'
                                           ELSE
                                             CASE
                                               WHEN Min (A.STATUS) <= 2 THEN '0'
                                               ELSE '1'
                                             END
                                         END ) AS STATUS
                                FROM   F050801 A
                                       JOIN F05030101 B
                                         ON A.DC_CODE = B.DC_CODE
                                            AND A.GUP_CODE = B.GUP_CODE
                                            AND A.CUST_CODE = B.CUST_CODE
                                            AND A.WMS_ORD_NO = B.WMS_ORD_NO
                                       JOIN F050301 E
                                         ON B.DC_CODE = E.DC_CODE
                                            AND B.GUP_CODE = E.GUP_CODE
                                            AND B.CUST_CODE = E.CUST_CODE
                                            AND B.ORD_NO = E.ORD_NO
                                       INNER JOIN F051202 F
                                               ON F.WMS_ORD_NO = A.WMS_ORD_NO
                                                  AND F.DC_CODE = A.DC_CODE
                                                  AND F.GUP_CODE = A.GUP_CODE
                                                  AND F.CUST_CODE = A.CUST_CODE
                                       INNER JOIN F051201 C
                                               ON C.DC_CODE = A.DC_CODE
                                                  AND C.GUP_CODE = A.GUP_CODE
                                                  AND C.CUST_CODE = A.CUST_CODE
                                                  AND C.PICK_ORD_NO = F.PICK_ORD_NO
                                       LEFT JOIN F055001 D
                                              ON A.DC_CODE = D.DC_CODE
                                                 AND A.GUP_CODE = D.GUP_CODE
                                                 AND A.CUST_CODE = D.CUST_CODE
                                                 AND A.WMS_ORD_NO = D.WMS_ORD_NO
                                WHERE  A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.DELV_DATE = @p3 
            ";
			if (!string.IsNullOrEmpty(pickTime))
			{
				parameters.Add(new SqlParameter("@p4", pickTime));
				sqlFormat += @" AND A.PICK_TIME = @p4 ";
			}
			if (!string.IsNullOrEmpty(ordNo))
			{
				parameters.Add(new SqlParameter("@p5", ordNo));
				sqlFormat += @" AND B.ORD_NO =  @p5 ";
			}
			sqlFormat += @"
										{0}
							GROUP BY A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.DELV_DATE,
										A.PICK_TIME,
										E.CUST_ORD_NO,
										B.ORD_NO,
										A.STATUS,
										C.PICK_NAME,
										D.PACKAGE_NAME) X
							  ORDER BY X.PICK_TIME";

			//判斷狀態
			string whereStr = "";

			// 全部
			if (string.IsNullOrWhiteSpace(status))
			{
				whereStr = " AND ((A.STATUS = '9' And C.PICK_STATUS=2) or A.STATUS = '0' or A.STATUS = '1' or A.STATUS = '2') ";
			}
			// 未出貨
			else if (status == "0")
			{
				whereStr = " AND  (A.STATUS = '0' or A.STATUS = '1' or A.STATUS = '2') ";
			}
			// 已包裝不出貨
			else if (status == "1")
			{
				whereStr = " AND (A.STATUS = '9' And C.PICK_STATUS=2) ";
			}

			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				whereStr += parameters.CombineNotNullOrEmpty(" AND E.CUST_ORD_NO = @p{0}  ", custOrdNo);
			}

			var sql = string.Format(sqlFormat, whereStr);

			var result = SqlQuery<F050801NoShipOrders>(sql, parameters.ToArray());
			return result;
		}
		#endregion



		public IQueryable<F050801> GetF050801ByOrderNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var parameter = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", dcCode),
								new SqlParameter("@p4", gupCode),
								new SqlParameter("@p5", custCode),
								new SqlParameter("@p6", ordNo),
								new SqlParameter("@p7", Current.Staff)
						};
			var sql = @"  SELECT A.*
							FROM F050801 A
								 INNER JOIN F05030101 B
									ON     B.DC_CODE = A.DC_CODE
									   AND B.GUP_CODE = A.GUP_CODE
									   AND B.CUST_CODE = A.CUST_CODE
									   AND B.WMS_ORD_NO = A.WMS_ORD_NO
								 INNER JOIN F050301 C
									ON     C.DC_CODE = B.DC_CODE
									   AND C.GUP_CODE = B.GUP_CODE
									   AND C.CUST_CODE = B.CUST_CODE
									   AND C.ORD_NO = B.ORD_NO
						   WHERE     A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND C.ORD_NO IN ( 
												  SELECT DISTINCT
															A1.ORD_NO
															ORD_NO
													FROM F050101 A1
												   WHERE     A1.DC_CODE = @p3
                                                         AND A1.GUP_CODE = @p4
                                                         AND A1.CUST_CODE = @p5
                                                         AND A1.ORD_NO = @p6)
								 AND EXISTS
										(SELECT 1
										   FROM F190101 cc
												INNER JOIN (SELECT *
															  FROM F192402
															 WHERE EMP_ID = @p7) dd
												   ON     cc.DC_CODE = dd.DC_CODE
													  AND cc.GUP_CODE = dd.GUP_CODE
													  AND cc.CUST_CODE = dd.CUST_CODE
										  WHERE     cc.DC_CODE = A.DC_CODE
												AND cc.GUP_CODE = A.GUP_CODE
												AND cc.CUST_CODE = A.CUST_CODE)
						ORDER BY A.WMS_ORD_NO";

			var result = SqlQuery<F050801>(sql, parameter.ToArray());
			return result;
		}

		public IQueryable<F050801WmsOrdNo> GetF050801ListBySourceNo(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var parameter = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", sourceNo),
						};

			var sql = @"
					SELECT ROW_NUMBER()OVER(ORDER BY A.WMS_ORD_NO, A.CUST_CODE, A.GUP_CODE, A.DC_CODE ASC) ROWNUM, A.WMS_ORD_NO,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.STATUS,A.DELV_DATE,A.PICK_TIME
					FROM F050801 A
							 INNER JOIN F05030101 B
									ON     B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.WMS_ORD_NO = A.WMS_ORD_NO
							 INNER JOIN F050301 C
									ON     C.DC_CODE = B.DC_CODE
										 AND C.GUP_CODE = B.GUP_CODE
										 AND C.CUST_CODE = B.CUST_CODE
										 AND C.ORD_NO = B.ORD_NO
				 WHERE     A.DC_CODE = @p0
							 AND A.GUP_CODE = @p1
							 AND A.CUST_CODE = @p2
							 AND C.SOURCE_NO = @p3";

			var data = SqlQuery<F050801WmsOrdNo>(sql, parameter.ToArray());

			return data;
		}

		/// <summary>
		/// 取得出貨資訊 Master 部分所有資料
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public IQueryable<P05030201BasicData> GetP05030201BasicData(string gupCode, string custCode, string dcCode, string wmsOrdNo, string ordNo)
		{
			var paramList = new List<SqlParameter>
						{
								new SqlParameter("@p0", SqlDbType.VarChar){ Value = gupCode},
								new SqlParameter("@p1", SqlDbType.VarChar){ Value = custCode},
								new SqlParameter("@p2", SqlDbType.VarChar){ Value = dcCode},
								new SqlParameter("@p3", SqlDbType.VarChar){ Value = wmsOrdNo}
						};

			var sql = @"
									SELECT TOP(1) A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,
									CASE WHEN C.PROC_FLAG = '9' THEN 29
											 WHEN A.STATUS ='0' AND E.WMS_ORD_NO IS NULL THEN 10 
										   WHEN A.STATUS ='0' AND E.WMS_ORD_NO IS NOT NULL THEN 12 
										   WHEN A.STATUS = '1' OR A.STATUS='2' THEN 2 
										   ELSE A.STATUS END AS STATUS,
										   C.CUST_ORD_NO,
										   C.ORD_NO,
										   C.SOURCE_TYPE,
										   C.SOURCE_NO,
										   C.ARRIVAL_DATE,
										   C.RETAIL_CODE,
										   C.CUST_NAME,
										   C.ORD_DATE,
										   C.CONSIGNEE,
										   C.TEL,
										   C.TEL_1,
										   C.TEL_2,
										   A.SA_QTY,
										   CASE WHEN C.COLLECT ='1' THEN '是' ELSE '否' END COLLECT ,
										   C.MEMO,
										   C.ADDRESS,
										   C.ADDRESS AS DISTR_CAR_ADDRESS,
										   A.DELV_DATE,
										   A.PICK_TIME,
										   A.APPROVE_DATE,
										   NULL PICK_ORD_NO,
										   A.INCAR_DATE,
										   NULL ALL_ID,
										   F.RETURN_FLAG AS LACK_DO_STATUS,
										   NULL AS DISTR_CAR_NO,
										   NULL AS TAKE_DATE,
										   NULL AS TAKE_TIME,
										   NULL AS DISTR_CAR_STATUS,
										   (SELECT TOP(1) CROSS_NAME FROM F0001 WHERE CROSS_CODE = A.MOVE_OUT_TARGET) AS CROSS_NAME,
										   A.SUG_BOX_NO,
										   A.ISPACKCHECK,
										  (SELECT TOP(1) NAME FROM F000904 WHERE TOPIC ='F050101' AND SUBTOPIC ='FAST_DEAL_TYPE' AND VALUE = A.FAST_DEAL_TYPE) FAST_DEAL_TYPE_NAME,
                                          (SELECT TOP(1) NAME FROM F000904 WHERE TOPIC ='F050801' AND SUBTOPIC ='CUST_COST' AND VALUE = A.CUST_COST) CUST_COST,
                                          (SELECT TOP(1) NAME FROM F000904 WHERE TOPIC ='F050101' AND SUBTOPIC ='PACKING_TYPE' AND VALUE = A.PACKING_TYPE) PACKING_TYPE,
                                          (SELECT TOP(1) NAME FROM F000904 WHERE TOPIC ='F050801' AND SUBTOPIC ='SHIP_MODE' AND VALUE = A.SHIP_MODE) SHIP_MODE,
                       (SELECT TOP(1) LOGISTIC_NAME FROM F0002 WHERE DC_CODE = @p2 AND LOGISTIC_CODE = A.SUG_LOGISTIC_CODE) SUG_LOGISTIC_CODE,
                      A.NP_FLAG,
                      A.UPD_DATE
									FROM F050801 A
									JOIN F05030101 B
									ON B.DC_CODE = A.DC_CODE
									AND B.GUP_CODE = A.GUP_CODE
									AND B.CUST_CODE = A.CUST_CODE
									AND B.WMS_ORD_NO = A.WMS_ORD_NO
									JOIN F050301 C
									ON C.DC_CODE = B.DC_CODE
									AND C.GUP_CODE = B.GUP_CODE
									AND C.CUST_CODE = B.CUST_CODE
									AND C.ORD_NO = B.ORD_NO
									LEFT JOIN 
									(SELECT  D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO
										 FROM F051201 M
										 JOIN F051202 D
										 ON M.DC_CODE = D.DC_CODE
										 AND M.GUP_CODE = D.GUP_CODE
										 AND M.CUST_CODE = D.CUST_CODE
										 AND M.PICK_ORD_NO = D.PICK_ORD_NO
										 GROUP BY D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO
										 HAVING COUNT(D.PICK_STATUS) - SUM(CASE WHEN D.PICK_STATUS IN('1','9') THEN 1 ELSE 0 END)  = 0) E
										 ON E.DC_CODE = A.DC_CODE
										 AND E.GUP_CODE = A.GUP_CODE
										 AND E.CUST_CODE = A.CUST_CODE
										 AND E.WMS_ORD_NO = A.WMS_ORD_NO
									 LEFT JOIN ( 
									 SELECT D1.DC_CODE,
											D1.GUP_CODE,
											D1.CUST_CODE,
											D1.WMS_ORD_NO,
											MAX (D1.RETURN_FLAG) AS RETURN_FLAG
										 FROM F051206 D1
										GROUP BY D1.DC_CODE,
												 D1.GUP_CODE,
												 D1.CUST_CODE,
												 D1.WMS_ORD_NO) F
									ON F.DC_CODE = A.DC_CODE
									AND F.GUP_CODE = A.GUP_CODE
									AND F.CUST_CODE = A.CUST_CODE
									AND F.WMS_ORD_NO = A.WMS_ORD_NO
						 WHERE     A.GUP_CODE = @p0
							   AND A.CUST_CODE = @p1
							   AND A.DC_CODE = @p2
							   AND A.WMS_ORD_NO = @p3";

			if (!string.IsNullOrEmpty(ordNo))
			{
				sql += string.Format(" AND C.ORD_NO = @p{0}", paramList.Count);
				paramList.Add(new SqlParameter("@p" + paramList.Count(), SqlDbType.VarChar) { Value = ordNo });
			}

			var result = SqlQuery<P05030201BasicData>(sql, paramList.ToArray());

			return result;
		}

		/// <summary>
		/// 取得該出貨單所有來源單號
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public IQueryable<string> GetSourceNosByWmsOrdNo(string gupCode, string custCode, string dcCode, string wmsOrdNo)
		{
			var sql = @"SELECT A.SOURCE_NO
						  FROM F050801 C                                                        -- 出貨單
							   JOIN F05030101 B                                            -- 出貨單與訂單關聯
								  ON     C.DC_CODE = B.DC_CODE
									 AND C.GUP_CODE = B.GUP_CODE
									 AND C.CUST_CODE = B.CUST_CODE
									 AND C.WMS_ORD_NO = B.WMS_ORD_NO
							   JOIN F050301 A                                          -- 關聯大單或小單的訂單編號
								  ON     B.DC_CODE = A.DC_CODE
									 AND B.GUP_CODE = A.GUP_CODE
									 AND B.CUST_CODE = A.CUST_CODE
									 AND B.ORD_NO = A.ORD_NO
						 WHERE     C.GUP_CODE = @p0
							   AND C.CUST_CODE = @p1
							   AND C.DC_CODE = @p2
							   AND C.WMS_ORD_NO = @p3";

			var result = SqlQuery<string>(sql, new object[] { gupCode, custCode, dcCode, wmsOrdNo });
			return result;
		}

		/// <summary>
		/// 取得該批次日期與批次時段的所有已稽核出貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="wmsOrdNos"></param>
		/// <returns></returns>
		public IQueryable<F050801WmsOrdNo> GetF050801ByDelvPickTime(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string custOrdNo)
		{
			var paramList = new List<object>
						{
								dcCode,
								gupCode,
								custCode,
								delvDate,
								pickTime,
						};

			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY D.WMS_ORD_NO, D.CUST_CODE, D.GUP_CODE, D.DC_CODE ASC) ROWNUM, D.* FROM (SELECT DISTINCT
                        A.WMS_ORD_NO,
                        A.DC_CODE,
                        A.GUP_CODE,
                        A.CUST_CODE,
                        A.STATUS,
                        A.DELV_DATE,
                        A.PICK_TIME,
                        C.CUST_ORD_NO
                   FROM F050801 A
                        INNER JOIN F05030101 B
                           ON     B.DC_CODE = A.DC_CODE
                              AND B.GUP_CODE = A.GUP_CODE
                              AND B.CUST_CODE = A.CUST_CODE
                              AND B.WMS_ORD_NO = A.WMS_ORD_NO
                        INNER JOIN F050301 C
                           ON     C.DC_CODE = B.DC_CODE
                              AND C.GUP_CODE = B.GUP_CODE
                              AND C.CUST_CODE = B.CUST_CODE
                              AND C.ORD_NO = B.ORD_NO
                        INNER JOIN F1909 D
                           ON D.GUP_CODE = A.GUP_CODE
                          AND D.CUST_CODE=  A.CUST_CODE
                  WHERE  A.STATUS IN (2,9) AND C.PROC_FLAG <>'9'
                        AND (A.SELF_TAKE='0' OR  D.SELFTAKE_CHECKCODE ='0' OR   A.SELFTAKE_CHECKCODE='1')
                        AND A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.DELV_DATE = @p3
                        AND A.PICK_TIME = @p4) D";

			if (!string.IsNullOrEmpty(custOrdNo))
			{
				sql += string.Format(" WHERE D.CUST_ORD_NO = @p{0}", paramList.Count);
				paramList.Add(custOrdNo);
			}

			var data = SqlQuery<F050801WmsOrdNo>(sql, paramList.ToArray());

			return data;
		}

		#region F710802 作業異動查詢
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="changeDateBegin"></param>
		/// <param name="changeDateEnd"></param>
		/// <param name="itemCode"></param>
		/// <param name="itemName"></param>
		/// <param name="receiptType">單據類別</param>
		/// <param name="makeNo">批號</param>
		/// <returns></returns>
		public IQueryable<P710802SearchResult> GetF710802Type1(string gupCode, string custCode, string dcCode,
	DateTime changeDateBegin, DateTime changeDateEnd, string itemCode, string itemName, string receiptType, string makeNo)
		{
			var parameterList = new List<SqlParameter>()
			{
		new SqlParameter("@p0",SqlDbType.VarChar) {Value = custCode},
		new SqlParameter("@p1",SqlDbType.VarChar) {Value = gupCode},
		new SqlParameter("@p2",SqlDbType.VarChar) {Value = dcCode},
		new SqlParameter("@p3",SqlDbType.DateTime2) {Value = changeDateBegin.Date.ToString("yyyy/MM/dd HH:mm:ss")},
		new SqlParameter("@p4",SqlDbType.DateTime2) {Value = changeDateEnd.Date.AddDays(1).AddSeconds(-1).ToString("yyyy/MM/dd HH:mm:ss")},
	  };
			//var parameterList = new List<object> { custCode, gupCode, dcCode, changeDateBegin.ToString("yyyy/MM/dd"), changeDateEnd.ToString("yyyy/MM/dd") };

			// 商品編號條件
			//var itemCodeSql = parameterList.CombineNotNullOrEmpty(" AND C.ITEM_CODE = @p{0} ", itemCode);
			// 下面這行與上面意思相同，但下面支援多個參數 {0} {1} ... {n} 使用
			//var itemCodeSql = (!string.IsNullOrEmpty(itemCode)) ? parameterList.Combine(" AND C.ITEM_CODE = @p{0} ", itemCode) : string.Empty;
			var itemCodeSql = string.Empty;
			if (!string.IsNullOrEmpty(itemCode))
			{
				itemCodeSql = $" AND C.ITEM_CODE = @p{parameterList.Count}";
				parameterList.Add(new SqlParameter($"@p{parameterList.Count}", SqlDbType.VarChar) { Value = itemCode });
			}

			var makeNoSQL = string.Empty;
			if (!string.IsNullOrEmpty(makeNo))
			{
				makeNoSQL = $" AND C.MAKE_NO = @p{parameterList.Count}";
				parameterList.Add(new SqlParameter($"@p{parameterList.Count}", SqlDbType.VarChar) { Value = makeNo });
			}

			// 單據類別 == 出貨(00) 
			// var receiptTypeSql = (receiptType == "00")
			//? " AND A.SOURCE_TYPE IS NULL "
			//: parameterList.CombineNotNullOrEmpty(" AND A.SOURCE_TYPE = @p{0} ", receiptType);
			var receiptTypeSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(receiptType))
			{
				if (receiptType == "00")
					receiptTypeSql = $"  AND A.SOURCE_TYPE IS NULL";
				else
				{
					receiptTypeSql = $" AND A.SOURCE_TYPE = @p{parameterList.Count}";
					parameterList.Add(new SqlParameter($"@p{parameterList.Count}", SqlDbType.VarChar) { Value = receiptType });
				}
			}

			// 商品名稱條件
			//var itemNameSql = parameterList.CombineNotNullOrEmpty(" AND E.ITEM_NAME LIKE @p{0} + '%'", itemName);
			var itemNameSql = string.Empty;
			if (!string.IsNullOrEmpty(itemName))
			{
				itemNameSql = $" AND E.ITEM_NAME LIKE @p{parameterList.Count}";
				parameterList.Add(new SqlParameter($"@p{parameterList.Count}", SqlDbType.NVarChar) { Value = itemName + "%" });
			}


			var sql = @"SELECT F.*,
							 E.ITEM_NAME,
							 E.ITEM_SIZE,
							 E.ITEM_SPEC,
							 E.ITEM_COLOR,
                             G.GUP_NAME,
                             H.CUST_NAME
					    FROM (  
	                    SELECT A.DC_CODE,
                               A.GUP_CODE,
                               A.CUST_CODE,
                               A.DELV_DATE AS ChangeDate,                             -- 批次日期 = 異動日期
                               ISNULL (A.SOURCE_NO, A.WMS_ORD_NO) AS ReceiptNo,                 -- 單據號碼,
                               A.SOURCE_TYPE,
                               CASE
                                  WHEN A.SOURCE_TYPE IS NULL
                                  THEN N'出貨' WHEN A.SOURCE_TYPE = '-1'
                                  THEN N'配庫' ELSE
                                     (SELECT SOURCE_NAME
                                      FROM F000902
                                      WHERE SOURCE_TYPE = A.SOURCE_TYPE)
                               END AS ReceiptType,
                               C.ITEM_CODE,                                                  -- 商品編號
                               CASE WHEN A.SOURCE_TYPE IS NULL
                               THEN '888888888' 
                               ELSE C.PICK_LOC 
                               END AS SOURCE_LOC,                                     -- 來源儲位
                               CASE WHEN A.SOURCE_TYPE = '-1' THEN '888888888' ELSE '' END AS TARGET_LOC,
                               CASE WHEN A.SOURCE_TYPE ='-1' THEN SUM(C.B_PICK_QTY) ELSE SUM (C.A_PICK_QTY) END AS ChangeNumber,                             -- 異動數
                               CASE WHEN C.MAKE_NO = '0' THEN '' ELSE C.MAKE_NO END MAKE_NO,
                               C.VALID_DATE,
                               CASE 
                                   WHEN A.SOURCE_TYPE IS NULL THEN A.BCRT_NAME
                                   WHEN A.SOURCE_TYPE = '-1' THEN A.BCRT_NAME
                                   ELSE A.ACRT_NAME
                               END CRT_NAME,
                               CASE 
                                   WHEN A.SOURCE_TYPE IS NULL THEN A.BUPD_NAME
                                   WHEN A.SOURCE_TYPE = '-1' THEN A.BUPD_NAME
                                   ELSE A.AUPD_NAME
                               END UPD_NAME,
                               CASE WHEN A.UPD_DATE IS NULL THEN A.CRT_DATE ELSE A.UPD_DATE END PROC_TIME
                       FROM (
                       SELECT *
                       FROM 
                       (SELECT DISTINCT A.DC_CODE,
                                        A.GUP_CODE,
                                        A.CUST_CODE,
                                        A.DELV_DATE,
                                        B.SOURCE_NO,
                                        A.WMS_ORD_NO,
                                        B.SOURCE_TYPE,
                                        A.CRT_NAME ACRT_NAME,
                                        A.UPD_NAME AUPD_NAME,
                                        B.CRT_NAME BCRT_NAME,
                                        B.UPD_NAME BUPD_NAME,
                                        A.CRT_DATE,
                                        A.UPD_DATE
                               FROM F050801 A
                                    JOIN F05030101 D
                                       ON     A.WMS_ORD_NO = D.WMS_ORD_NO
                                          AND A.CUST_CODE = D.CUST_CODE
                                          AND A.GUP_CODE = D.GUP_CODE
                                          AND A.DC_CODE = D.DC_CODE
                                    JOIN F050301 B
                                       ON     D.ORD_NO = B.ORD_NO
                                          AND D.CUST_CODE = B.CUST_CODE
                                          AND D.GUP_CODE = B.GUP_CODE
                                          AND D.DC_CODE = B.DC_CODE
                                   WHERE A.STATUS <>'9' --取消的訂單不計入作業異動中
                          UNION ALL
                          SELECT DISTINCT A.DC_CODE,
                                          A.GUP_CODE,
                                          A.CUST_CODE,
                                          A.DELV_DATE,
                                          B.SOURCE_NO,
                                          A.WMS_ORD_NO,
                                          '-1' SOURCE_TYPE,
                                          A.CRT_NAME ACRT_NAME,
                                          A.UPD_NAME AUPD_NAME,
                                          B.CRT_NAME BCRT_NAME,
                                          B.UPD_NAME BUPD_NAME,
                                          A.CRT_DATE,
                                          A.UPD_DATE
                               FROM F050801 A
                                    JOIN F05030101 D
                                       ON     A.WMS_ORD_NO = D.WMS_ORD_NO
                                          AND A.CUST_CODE = D.CUST_CODE
                                          AND A.GUP_CODE = D.GUP_CODE
                                          AND A.DC_CODE = D.DC_CODE
                                    JOIN F050301 B
                                       ON     D.ORD_NO = B.ORD_NO
                                          AND D.CUST_CODE = B.CUST_CODE
                                          AND D.GUP_CODE = B.GUP_CODE
                                          AND D.DC_CODE = B.DC_CODE 
                                 WHERE A.SOURCE_TYPE IS NULL
                                   AND A.STATUS <>'9' --取消的訂單不計入作業異動中
                              ) G
                              WHERE     G.CUST_CODE = @p0
	                   			    AND G.GUP_CODE = @p1
	                   			    AND G.DC_CODE = @p2
	                   			    AND G.DELV_DATE >= @p3
                                    AND G.DELV_DATE <= @p4
	                   			) A
                            JOIN F051202 C
                               ON     A.WMS_ORD_NO = C.WMS_ORD_NO
                                  AND A.CUST_CODE = C.CUST_CODE
                                  AND A.GUP_CODE = C.GUP_CODE
                                  AND A.DC_CODE = C.DC_CODE
	                   			WHERE  1=1 "
															+ itemCodeSql
															+ makeNoSQL
															+ receiptTypeSql
															+ @" GROUP BY A.DC_CODE,
                                                  A.GUP_CODE,
                                                  A.CUST_CODE,
                                                  A.DELV_DATE,
                                                  ISNULL (A.SOURCE_NO, A.WMS_ORD_NO),
                                                  A.SOURCE_TYPE,
                                                  C.ITEM_CODE,
                                                  C.PICK_LOC,
                                                  C.MAKE_NO,
                                                  C.VALID_DATE,
                                                  A.ACRT_NAME,
                                                  A.AUPD_NAME,
                                                  A.BCRT_NAME,
                                                  A.BUPD_NAME,

                                    A.CRT_DATE,
                                    A.UPD_DATE
                                   ) F
	                   			  JOIN F1903 E ON F.ITEM_CODE = E.ITEM_CODE AND F.GUP_CODE = E.GUP_CODE AND F.CUST_CODE = E.CUST_CODE  -- 取得商品主檔資訊
                                  JOIN F1929 G ON G.GUP_CODE = F.GUP_CODE
                                  JOIN F1909 H ON H.GUP_CODE = F.GUP_CODE AND H.CUST_CODE = F.CUST_CODE
	                   						 WHERE 1=1 "
																													 + itemNameSql
											 + " ORDER BY F.ReceiptNo,F.ITEM_CODE,F.VALID_DATE,F.MAKE_NO,F.SOURCE_TYPE,F.SOURCE_LOC ";

			var result = SqlQueryWithSqlParameterSetDbType<P710802SearchResult>(sql, parameterList.ToArray());
			return result;
		}


		public IQueryable<P710802SearchResult> GetF710802Type2(string gupCode, string custCode, string dcCode,
			DateTime changeDateBegin, DateTime changeDateEnd, string itemCode, string itemName, string receiptType, string makeNo)
		{
			var sqlParamers = new List<SqlParameter>
	  {
		new SqlParameter("@p0",SqlDbType.VarChar) {Value = custCode},
		new SqlParameter("@p1",SqlDbType.VarChar) {Value = gupCode},
		new SqlParameter("@p2",SqlDbType.VarChar) {Value = dcCode},
		new SqlParameter("@p3",SqlDbType.DateTime2) {Value = changeDateBegin.Date.ToString("yyyy/MM/dd HH:mm:ss")},
		new SqlParameter("@p4",SqlDbType.DateTime2) {Value = changeDateEnd.Date.AddDays(1).AddSeconds(-1).ToString("yyyy/MM/dd HH:mm:ss")},
	  };

			string sql = @"	SELECT	A.DC_CODE,A.GUP_CODE,A.CUST_CODE,F.GUP_NAME,G.CUST_NAME,A.ALLOCATION_DATE AS ChangeDate, A.SOURCE_TYPE,
									ISNULL(ISNULL(B.SOURCE_NO,A.SOURCE_NO),A.ALLOCATION_NO) AS ReceiptNo, B.ITEM_CODE,
									E.ITEM_NAME, E.ITEM_SIZE, E.ITEM_SPEC, E.ITEM_COLOR,
									CASE WHEN A.SRC_WAREHOUSE_ID IS NULL THEN '' ELSE B.SRC_LOC_CODE END AS SOURCE_LOC, 
									CASE WHEN A.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE B.TAR_LOC_CODE END AS TARGET_LOC,
									SUM(B.A_TAR_QTY) AS ChangeNumber ,
                  CASE WHEN A.SOURCE_TYPE IS NULL THEN N'調撥' ELSE 
                       (SELECT SOURCE_NAME FROM F000902 WHERE 
                          SOURCE_TYPE = 
                          CASE 
                            WHEN A.SOURCE_TYPE='30' THEN '30' 
                            ELSE ISNULL(B.SOURCE_TYPE,A.SOURCE_TYPE)
                          END) 
                  END AS ReceiptType,
                  CASE WHEN A.TAR_WAREHOUSE_ID IS NULL THEN 
                       CASE WHEN ISNULL(B.SRC_MAKE_NO,B.MAKE_NO) = '0' THEN '' ELSE ISNULL(B.SRC_MAKE_NO,B.MAKE_NO) END
                  ELSE 
                       CASE WHEN ISNULL(B.TAR_MAKE_NO,ISNULL(B.SRC_MAKE_NO,B.MAKE_NO)) = '0' THEN '' ELSE  ISNULL(B.TAR_MAKE_NO,ISNULL(B.SRC_MAKE_NO,B.MAKE_NO)) END
                  END MAKE_NO,
                  CASE WHEN A.TAR_WAREHOUSE_ID IS NULL THEN 
                       ISNULL(B.SRC_VALID_DATE,B.VALID_DATE)
                  ELSE 
                       ISNULL(B.TAR_VALID_DATE,ISNULL(B.SRC_VALID_DATE,B.VALID_DATE))
                  END VALID_DATE,
                  A.CRT_NAME,A.UPD_NAME,
                  CASE WHEN A.UPD_DATE IS NULL THEN A.CRT_DATE ELSE A.UPD_DATE END PROC_TIME,
                  CASE WHEN ISNULL(ISNULL(B.SOURCE_NO,A.SOURCE_NO),'') != '' THEN
                      A.ALLOCATION_NO
                    ELSE
                      ''
                  END AS ALLOCATION_NO
							FROM	F151001 A,F151002 B,F1903 E,F1929 F,F1909 G
							WHERE	A.CUST_CODE = @p0					AND
									A.GUP_CODE = @p1					AND
									A.DC_CODE = @p2						AND									
									B.CUST_CODE = A.CUST_CODE			AND
									B.GUP_CODE = A.GUP_CODE				AND
									B.DC_CODE = A.DC_CODE				AND
									B.ITEM_CODE = E.ITEM_CODE			AND
									B.GUP_CODE = E.GUP_CODE				AND
                  B.CUST_CODE = E.CUST_CODE				AND
									A.ALLOCATION_NO = B.ALLOCATION_NO	AND
									A.GUP_CODE = F.GUP_CODE  AND
									A.GUP_CODE = G.GUP_CODE AND  
									A.CUST_CODE = G.CUST_CODE AND
									A.ALLOCATION_DATE >= @p3	AND 
									A.ALLOCATION_DATE <= @p4 AND
                  (A.STATUS!='9' OR B.STATUS!='9')";

			if (!String.IsNullOrEmpty(itemCode))
			{
				sql += " AND B.ITEM_CODE = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.VarChar) { Value = itemCode });
			}
			if (!String.IsNullOrEmpty(makeNo))
			{
				sql += " AND B.MAKE_NO = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.VarChar) { Value = makeNo });
			}

			if (!String.IsNullOrEmpty(receiptType))
			{
				if (receiptType == "17") //調撥除了SOURCE_TYPE=17之外 還要抓SOURCE_TYPE IS NULL
				{
					sql += " AND (A.SOURCE_TYPE IS NULL OR A.SOURCE_TYPE='17' AND A.ALLOCATION_TYPE != '4') ";
				}
				else
				{
					if (receiptType == "04")// 
											//sql += $" AND (A.SOURCE_TYPE = @p{sqlParamers.Count} OR A.ALLOCATION_TYPE = '4' )";
						sql += $" AND (A.SOURCE_TYPE = @p{sqlParamers.Count} OR (A.SOURCE_TYPE !='30' AND A.ALLOCATION_TYPE = '4' ))";
					else
						sql += " AND A.SOURCE_TYPE = @p" + sqlParamers.Count;

					sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.VarChar) { Value = receiptType });
				}
			}

			if (!String.IsNullOrEmpty(itemName))
			{
				sql += " AND E.ITEM_NAME LIKE @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.NVarChar) { Value = itemName + "%" });
			}

			sql += @" GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,F.GUP_NAME,G.CUST_NAME, A.ALLOCATION_DATE, A.SOURCE_TYPE,
								ISNULL(ISNULL(B.SOURCE_NO,A.SOURCE_NO),A.ALLOCATION_NO), B.ITEM_CODE,
								E.ITEM_NAME, E.ITEM_SIZE, E.ITEM_SPEC, E.ITEM_COLOR,
								B.SRC_LOC_CODE, B.TAR_LOC_CODE,B.MAKE_NO,B.SRC_MAKE_NO,
                B.TAR_MAKE_NO,B.VALID_DATE,B.SRC_VALID_DATE,B.TAR_VALID_DATE,
                A.SRC_WAREHOUSE_ID,A.TAR_WAREHOUSE_ID,A.CRT_NAME,A.UPD_NAME,
                A.ALLOCATION_NO,A.SOURCE_NO,A.CRT_DATE,A.UPD_DATE,B.SOURCE_TYPE,
                B.SOURCE_NO
					  ORDER BY ISNULL(A.SOURCE_NO,A.ALLOCATION_NO),B.ITEM_CODE  ";

			var result = SqlQueryWithSqlParameterSetDbType<P710802SearchResult>(sql, sqlParamers.ToArray());
			return result;
		}

		public IQueryable<P710802SearchResult> GetF710802Type3(string gupCode, string custCode, string dcCode,
		 DateTime changeDateBegin, DateTime changeDateEnd, string itemCode, string itemName, string makeNo)
		{
			var sqlParamers = new List<SqlParameter>
								{
																		new SqlParameter("@p0",SqlDbType.VarChar){Value=custCode},
																		new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
																		new SqlParameter("@p2",SqlDbType.VarChar){Value=dcCode},
																		new SqlParameter("@p3",SqlDbType.DateTime2){Value=changeDateBegin.Date.ToString("yyyy/MM/dd HH:mm:ss")},
																		new SqlParameter("@p4",SqlDbType.DateTime2){Value=changeDateEnd.Date.AddDays(1).AddSeconds(-1).ToString("yyyy/MM/dd HH:mm:ss")},
																};

			string sql = @"	SELECT	a.DC_CODE,a.GUP_CODE,a.CUST_CODE,F.GUP_NAME,G.CUST_NAME,a.CRT_DATE AS ChangeDate, '19' AS SOURCE_TYPE,
									a.ADJUST_NO  AS ReceiptNo, a.ITEM_CODE,
									E.ITEM_NAME, E.ITEM_SIZE, E.ITEM_SPEC, E.ITEM_COLOR,
									(CASE WHEN a.WORK_TYPE =1 THEN a.LOC_CODE ELSE '' END) AS SOURCE_LOC,
									(CASE WHEN a.WORK_TYPE =0 THEN a.LOC_CODE ELSE '' END) AS TARGET_LOC,
									SUM(a.ADJ_QTY) AS ChangeNumber ,
									N'調整' AS ReceiptType,
                  CASE WHEN a.MAKE_NO = '0' THEN '' ELSE a.MAKE_NO END MAKE_NO, 
                  a.VALID_DATE,CASE WHEN A.UPD_DATE IS NULL THEN A.CRT_DATE ELSE A.UPD_DATE END PROC_TIME,
                  a.CRT_NAME,a.UPD_NAME
							FROM	F200103 a, F1903 E,F1929 F,F1909 G
							WHERE	a.CUST_CODE = @p0			AND
									a.GUP_CODE = @p1			AND
									a.DC_CODE = @p2				AND																		
									a.ITEM_CODE = E.ITEM_CODE	AND
									a.GUP_CODE = E.GUP_CODE		AND
                                    a.CUST_CODE = E.CUST_CODE		AND
									a.GUP_CODE = F.GUP_CODE  AND
                                    a.GUP_CODE = G.GUP_CODE AND  
                                    a.CUST_CODE = G.CUST_CODE AND
									a.CRT_DATE >= @p3   AND 
									a.CRT_DATE <=  @p4 ";

			if (!String.IsNullOrEmpty(itemCode))
			{
				sql += " AND a.ITEM_CODE = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.VarChar) { Value = itemCode });
			}
			if (!String.IsNullOrEmpty(makeNo))
			{
				sql += " AND a.MAKE_NO = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.VarChar) { Value = makeNo });
			}

			if (!String.IsNullOrEmpty(itemName))
			{
				sql += " AND E.ITEM_NAME LIKE @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, SqlDbType.NVarChar) { Value = itemName + "%" });
			}

			sql += @" GROUP BY	a.DC_CODE,a.GUP_CODE,a.CUST_CODE,F.GUP_NAME,G.CUST_NAME,a.CRT_DATE, a.ADJUST_NO, a.ITEM_CODE,
								E.ITEM_NAME, E.ITEM_SIZE, E.ITEM_SPEC, E.ITEM_COLOR,
								(CASE WHEN a.WORK_TYPE =1 THEN a.LOC_CODE ELSE '' END),
								(CASE WHEN a.WORK_TYPE =0 THEN a.LOC_CODE ELSE '' END),
								a.MAKE_NO,a.VALID_DATE,a.CRT_NAME,a.UPD_NAME,A.CRT_DATE,A.UPD_DATE
					  ORDER BY a.ADJUST_NO,a.ITEM_CODE";

			var result = SqlQueryWithSqlParameterSetDbType<P710802SearchResult>(sql, sqlParamers.ToArray());
			return result;
		}
		#endregion




		#region 取F050801 相關資訊 FOR F1101用
		public IQueryable<F050801DataForF1101> GetF050801DataForF1101(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{

			var paramList = new List<object>
						{
								wmsOrdNo , dcCode , gupCode , custCode
						};

			var sql = @"select A.WMS_ORD_NO ,C.ORD_NO , C.SOURCE_NO ,C.SOURCE_TYPE 
					    	,E.PICK_LOC , E.ITEM_CODE ,E.VALID_DATE ,E.ENTER_DATE
					    	,E.VNR_CODE , E.SERIAL_NO ,E.A_PICK_QTY , '0' STATUS
					    	,E.DC_CODE ,E.GUP_CODE ,E.CUST_CODE ,E.PALLET_CTRL_NO ,E.BOX_CTRL_NO ,E.MAKE_NO
					    from F050801 A
					    join F05030101 B on A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND  A.WMS_ORD_NO =B.WMS_ORD_NO
					    join F050301 C on C.DC_CODE = B.DC_CODE AND C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = B.CUST_CODE AND  C.ORD_NO =B.ORD_NO and C.SOURCE_TYPE in ('05','06')
					    join F051202 E on E.DC_CODE = A.DC_CODE AND E.GUP_CODE = A.GUP_CODE AND E.CUST_CODE = A.CUST_CODE AND  A.WMS_ORD_NO = E.WMS_ORD_NO 
					    where A.WMS_ORD_NO =@p0 
					    		AND  A.DC_CODE = @p1
					    		AND A.GUP_CODE = @p2
					    		AND A.CUST_CODE = @p3
				      ";


			var result = SqlQuery<F050801DataForF1101>(sql, paramList.ToArray());
			return result;
		}
		#endregion

		/// <summary>
		/// 修改配送商代號
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="wmsOdrNo"></param>
		/// <param name="allId">配送商代號</param>
		public void UpdateAllId(string gupCode, string custCode, string dcCode, List<string> wmsOdrNoList, string allId)
		{
			var parameters = new List<object>
						{
								allId,
								Current.Staff,
								Current.StaffName,
                DateTime.Now,
                gupCode,
								custCode,
								dcCode
						};

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("WMS_ORD_NO", wmsOdrNoList, ref paramStartIndex);

			var sql = @"Update F050801 Set ALL_ID = @p0, 
                                           UPD_STAFF = @p1, 
                                           UPD_NAME = @p2, 
                                           UPD_DATE = @p3
				         Where GUP_CODE = @p4
				           And CUST_CODE = @p5
				           And DC_CODE = @p6
				           And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<SchReplenishStock> GetReplensihStockData(string dcCode, string gupCode, string custCode, int baseDay = 0, List<decimal> ids = null)
		{
			var parameters = new List<SqlParameter> {
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", DateTime.Today) {SqlDbType = SqlDbType.DateTime2}
                        };
			string insSql = string.Empty;
			string column = string.Empty;
			if (baseDay > 0)
			{
				parameters.Add(new SqlParameter("@p4", baseDay));
        parameters.Add(new SqlParameter("@p5", DateTime.Today.AddDays(-baseDay)));

        insSql = @"     --以下為 GROUP BY ITEM_CODE / 出貨量 (前 1-90 天) 的平均量
                                SELECT 
										A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE , NULL SERIAL_NO
										,CAST(CEILING((CAST(SUM(A.A_DELV_QTY) AS float) / A.START_DATE)  * 7 )as int) AVG_QTY, A.PICK_SAVE_QTY, A.PICK_SAVE_ORD, 0 OUTSTOCK_QTY
								FROM 
								(
									--明細
									SELECT 
											A.DC_CODE ,A.GUP_CODE,A.CUST_CODE, A.WMS_ORD_NO ,A.DELV_DATE
											,@p4 START_DATE
											,B.ITEM_CODE ,B.A_DELV_QTY, C.PICK_SAVE_QTY, C.PICK_SAVE_ORD
									FROM F050801 A
									JOIN F050802 B ON A.WMS_ORD_NO =B.WMS_ORD_NO AND A.DC_CODE=B.DC_CODE AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
									JOIN F1903 C ON C.GUP_CODE =B.GUP_CODE AND C.ITEM_CODE = B.ITEM_CODE AND C.CUST_CODE = B.CUST_CODE 
									WHERE
									A.DELV_DATE <= @p3  
									AND A.DELV_DATE >= @p5 
									AND A.STATUS in (5,6)
								) A 
								GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE , A.START_DATE, A.PICK_SAVE_QTY, A.PICK_SAVE_ORD";
			}
			else if (ids != null && ids.Any())
			{
				insSql = $@"SELECT A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE , A.SERIAL_NO
												,A.TTL_OUTSTOCK_QTY AVG_QTY, B.PICK_SAVE_QTY, B.PICK_SAVE_ORD, ISNULL(A.TTL_OUTSTOCK_QTY, 0) OUTSTOCK_QTY, A.MAKE_NO
										FROM F050805 A
										JOIN F1903 B ON A.GUP_CODE =B.GUP_CODE AND A.ITEM_CODE = B.ITEM_CODE AND A.CUST_CODE = B.CUST_CODE 
										WHERE A.ID IN ({ string.Join(",", ids) })";

				column = " A.MAKE_NO, ";
			}

			var sql = $@"SELECT 
								A.DC_CODE ,
								A.GUP_CODE ,
								A.CUST_CODE , 
								A.ITEM_CODE , 
								A.SERIAL_NO , 
                                {column}
								ISNULL(A.QTY, 0) QTY  , -- 所有 (黃金揀貨區/一般揀貨區)(良品倉) 數量
                                A.OUTSTOCK_QTY OUTSTOCK_QTY, --缺貨數(只有手動挑單才有)
								A.RESULT_QTY , -- 每週平均出貨量。
								A.PICK_SAVE_QTY, -- 補貨安全庫存量
                                A.PICK_SAVE_ORD -- 補貨最小單位數
						FROM 
						(
							
							SELECT 
									A.* , B.QTY
									, A.AVG_QTY RESULT_QTY 
							FROM 
							(
								{insSql}
							) A						
							LEFT JOIN (					
								-- 以下為 BROUP BY ITEM_CODE 所有 (黃金揀貨區/一般揀貨區)(良品倉) 數量 ; 箱包數量		
								SELECT 
									A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE , SUM(A.QTY) QTY
								FROM 
								(
									--明細
									SELECT 
											A.DC_CODE ,
											A.GUP_CODE ,
											A.CUST_CODE, 
											A.ITEM_CODE ,
											A.LOC_CODE , 
											A.VALID_DATE ,
											A.ENTER_DATE ,
											A.SERIAL_NO ,
											A.QTY, 
											B.AREA_CODE , 
											C.AREA_NAME , 
											--C.AREA_CODE , 
											C.ATYPE_CODE      
									FROM F1913 A
									JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE
									JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('A','B')
									JOIN F1980 E ON E.DC_CODE= A.DC_CODE AND E.WAREHOUSE_ID = B.WAREHOUSE_ID AND E.WAREHOUSE_TYPE ='G'
									WHERE @p3 >= A.ENTER_DATE AND @p3 <= A.VALID_DATE AND B.NOW_STATUS_ID IN ('01', '02')
								) A GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE 
							) B ON A.DC_CODE =B.DC_CODE AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE AND A.ITEM_CODE = B.ITEM_CODE
						) A WHERE 
								A.RESULT_QTY > 0 AND 
								A.DC_CODE = @p0 AND 
								A.GUP_CODE = @p1 AND 
								A.CUST_CODE = @p2
						";

			var result = SqlQuery<SchReplenishStock>(sql, parameters.ToArray());
			return result;
		}

		public IQueryable<SchReplenishStock> GetReplensihStock(string dcCode, string gupCode, string custCode, int baseDay = 0, List<decimal> ids = null)
		{
			var parameters = new List<SqlParameter> {
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
                new SqlParameter("@p4", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };
			string insSql = string.Empty;
			if (baseDay > 0)
			{
				parameters.Add(new SqlParameter("@p3", baseDay));
				insSql = @"     --以下為 GROUP BY ITEM_CODE / 出貨量 (前 1-90 天) 的平均量
                                SELECT 
										A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE 
										,CAST(CEILING((CAST(SUM(A.A_DELV_QTY) AS float) / A.START_DATE)  * 7 )as int) AVG_QTY, A.PICK_SAVE_QTY, A.PICK_SAVE_ORD, 0 OUTSTOCK_QTY
								FROM 
								(
									--明細
									SELECT 
											A.DC_CODE ,A.GUP_CODE,A.CUST_CODE, A.WMS_ORD_NO ,A.DELV_DATE
											,@p3 START_DATE
											,B.ITEM_CODE ,B.A_DELV_QTY, C.PICK_SAVE_QTY, C.PICK_SAVE_ORD
									FROM F050801 A
									JOIN F050802 B ON A.WMS_ORD_NO =B.WMS_ORD_NO AND A.DC_CODE=B.DC_CODE AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
									JOIN F1903 C ON C.GUP_CODE =B.GUP_CODE AND C.ITEM_CODE = B.ITEM_CODE AND C.CUST_CODE = B.CUST_CODE 
									WHERE
									A.DELV_DATE <= CONVERT(datetime, CONVERT(varchar, dbo.GetSysDate() , 111))  
									AND A.DELV_DATE >= CONVERT(datetime, CONVERT(varchar, dbo.GetSysDate() - @p3, 111)) 
									AND A.STATUS in (5,6)
								) A 
								GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE , A.START_DATE, A.PICK_SAVE_QTY, A.PICK_SAVE_ORD";
			}
			else if (ids != null && ids.Any())
			{
				insSql = $@"SELECT A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE 
												,A.TTL_ORD_QTY AVG_QTY, B.PICK_SAVE_QTY, B.PICK_SAVE_ORD, ISNULL(A.TTL_OUTSTOCK_QTY, 0) OUTSTOCK_QTY
										FROM F050805 A
										JOIN F1903 B ON A.GUP_CODE =B.GUP_CODE AND A.ITEM_CODE = B.ITEM_CODE AND A.CUST_CODE = B.CUST_CODE 
										WHERE A.ID IN ({ string.Join(",", ids) })";
			}

			var sql = $@"SELECT 
								A.DC_CODE ,
								A.GUP_CODE ,
								A.CUST_CODE , 
								A.ITEM_CODE , 
								ISNULL(A.QTY, 0) QTY  , -- 所有 (黃金揀貨區/一般揀貨區)(良品倉) 數量
                                A.OUTSTOCK_QTY OUTSTOCK_QTY, --缺貨數(只有手動挑單才有)
								A.RESULT_QTY , -- 每週平均出貨量。
								A.PICK_SAVE_QTY, -- 補貨安全庫存量
								A.REPLENSIH_QTY, -- 補貨區庫存 (良品倉)
                                A.PICK_SAVE_ORD -- 補貨最小單位數
						FROM 
						(
							
							SELECT 
									A.* , B.QTY
									, A.AVG_QTY RESULT_QTY 
									, ISNULL(C.REPLENSIH_QTY,0) REPLENSIH_QTY 
							FROM 
							(
								{insSql}
							) A						
							LEFT JOIN (					
								-- 以下為 BROUP BY ITEM_CODE 所有 (黃金揀貨區/一般揀貨區)(良品倉) 數量 ; 箱包數量		
								SELECT 
									A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE , SUM(A.QTY) QTY
								FROM 
								(
									--明細
									SELECT 
											A.DC_CODE ,
											A.GUP_CODE ,
											A.CUST_CODE, 
											A.ITEM_CODE ,
											A.LOC_CODE , 
											A.VALID_DATE ,
											A.ENTER_DATE ,
											A.SERIAL_NO ,
											A.QTY, 
											B.AREA_CODE , 
											C.AREA_NAME , 
											--C.AREA_CODE , 
											C.ATYPE_CODE      
									FROM F1913 A
									JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE
									JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('A','B')
									JOIN F1980 E ON E.DC_CODE= A.DC_CODE AND E.WAREHOUSE_ID = B.WAREHOUSE_ID AND E.WAREHOUSE_TYPE ='G'
									WHERE @p4 >= A.ENTER_DATE AND @p4 <= A.VALID_DATE AND B.NOW_STATUS_ID IN ('01', '02')
								) A GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE 
							) B ON A.DC_CODE =B.DC_CODE AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE AND A.ITEM_CODE = B.ITEM_CODE
							LEFT JOIN 
								(   
								-- BROUP BY ITEM_CODE 所有補貨區庫存 (良品倉)
								SELECT 
									A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE , SUM(A.QTY) REPLENSIH_QTY 
								FROM 
								(
									SELECT 
										  A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE
										, A.ITEM_CODE ,A.LOC_CODE , A.VALID_DATE ,A.ENTER_DATE ,A.SERIAL_NO ,A.QTY                
									FROM F1913 A
									JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE
									JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('C')        
									JOIN F1980 E ON E.DC_CODE= A.DC_CODE AND E.WAREHOUSE_ID = B.WAREHOUSE_ID AND E.WAREHOUSE_TYPE ='G'
									WHERE @p4 >= A.ENTER_DATE AND @p4 <= A.VALID_DATE AND B.NOW_STATUS_ID IN ('01', '02')
								) A GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE
							) C  ON A.DC_CODE =C.DC_CODE AND A.GUP_CODE =C.GUP_CODE AND A.CUST_CODE =C.CUST_CODE AND A.ITEM_CODE = C.ITEM_CODE

						) A WHERE 
								A.RESULT_QTY > 0 AND 
								A.REPLENSIH_QTY > 0 AND 
								A.DC_CODE = @p0 AND 
								A.GUP_CODE = @p1 AND 
								A.CUST_CODE = @p2
						";

			var result = SqlQuery<SchReplenishStock>(sql, parameters.ToArray());
			return result;
		}

		/// <summary>
		/// 取得退貨單(換貨單)維護新增時，左側查詢歷史出貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="beginDelvDate"></param>
		/// <param name="endDelvDate"></param>
		/// <param name="retailCode"></param>
		/// <param name="custName"></param>
		/// <returns></returns>
		public IQueryable<CustomerData> GetCustomerDatas(string dcCode, string gupCode, string custCode, DateTime beginDelvDate, DateTime endDelvDate, string retailCode, string custName, string wmsOrdNo, string custOrdNo)
		{
			var sql = @"SELECT C.DELV_DATE,
							   C.WMS_ORD_NO,
							   C.DC_CODE,
							   C.GUP_CODE,
							   C.CUST_CODE,
							   A.RETAIL_CODE,
							   A.CUST_NAME,
							   A.CONTACT,
							   A.CONTACT_TEL,
							   A.ADDRESS
						  FROM F050301 A
							   JOIN F05030101 B
								  ON     A.ORD_NO = B.ORD_NO
									 AND A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
							   JOIN F050801 C
								  ON     B.WMS_ORD_NO = C.WMS_ORD_NO
									 AND B.DC_CODE = C.DC_CODE
									 AND B.GUP_CODE = C.GUP_CODE
									 AND B.CUST_CODE = C.CUST_CODE
						 WHERE     A.PROC_FLAG <> '9'
							   AND C.STATUS in (5,6)
							   AND C.DC_CODE = @p0
							   AND C.GUP_CODE = @p1
							   AND C.CUST_CODE = @p2
							   AND C.DELV_DATE BETWEEN @p3 AND @p4";

			beginDelvDate = beginDelvDate.Date;
			endDelvDate = endDelvDate.Date;

			var param = new List<SqlParameter> {
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
								new SqlParameter("@p3",beginDelvDate),
								new SqlParameter("@p4",endDelvDate)
						};
			if (!string.IsNullOrWhiteSpace(retailCode))
			{
				sql += string.Format(" AND A.RETAIL_CODE = @p{0} ", param.Count);
				param.Add(new SqlParameter(string.Format("@p{0}", param.Count), retailCode));
			}

			if (!string.IsNullOrWhiteSpace(custName))
			{
				sql += string.Format(" AND A.CUST_NAME LIKE @p{0}", param.Count);
				param.Add(new SqlParameter(string.Format("@p{0}", param.Count), string.Format("{0}%", custName)));
			}
			if (!string.IsNullOrWhiteSpace(wmsOrdNo))
			{
				sql += string.Format(" AND C.WMS_ORD_NO = @p{0} ", param.Count);
				param.Add(new SqlParameter(string.Format("@p{0}", param.Count), wmsOrdNo));
			}
			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sql += string.Format(" AND A.CUST_ORD_NO = @p{0} ", param.Count);
				param.Add(new SqlParameter(string.Format("@p{0}", param.Count), custOrdNo));
			}
			//var paramList = new List<object> { dcCode, gupCode, custCode, beginDelvDate, endDelvDate };
			//sql += paramList.CombineNotNullOrEmpty(" AND A.RETAIL_CODE = @p{0} ", retailCode);
			//sql += paramList.CombineNotNullOrEmpty(" AND A.CUST_NAME LIKE @p{0} || '%' ", custName);
			sql += " ORDER BY C.WMS_ORD_NO";
			var result = SqlQuery<CustomerData>(sql, param.ToArray());

			return result;
		}

		/// <summary>
		/// 取得出貨處理費結算用的出貨資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="settleDateS"></param>
		/// <param name="settleDateE"></param>
		/// <returns></returns>
		public IQueryable<SettleData> GetDeliveryDatas(string dcCode, string gupCode, string custCode, DateTime settleDateS, DateTime settleDateE)
		{
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY TB.DELV_DATE,WMS_NO,TB.ITEM_CODE,TB.PAST_NO ASC) ROWNUM, TB.* FROM (
                        SELECT DISTINCT @p3 CAL_DATE,
                               A.*,CASE WHEN A.RETAIL_CODE IS NULL THEN '02' ELSE '04' END DELV_ACC_TYPE,
                               (SELECT COUNT (1)
                                  FROM F050301 G
                                 WHERE     F.DC_CODE = G.DC_CODE
                                       AND F.GUP_CODE = G.GUP_CODE
                                       AND F.CUST_CODE = G.CUST_CODE
                                       AND F.ORD_NO = G.ORD_NO
                                       AND G.PRINT_RECEIPT = '1')
                                  INVOICE_CNT
                          FROM    
                          (  SELECT A.DC_CODE,
                                         A.GUP_CODE,
                                         A.CUST_CODE,
                                         A.DELV_DATE,
                                         A.WMS_ORD_NO WMS_NO,
                                         A.RETAIL_CODE,
                                         C.ITEM_CODE,                 
                                         E.PAST_NO,
                                         MAX(E.PACKAGE_BOX_NO) PACKAGE_BOX_NO,
                                         0 AMT,
                                         MIN(A.SA_QTY) SA_QTY,                                  
                                         SUM(C.PACKAGE_QTY) QTY                 
                                    FROM F050801 A
                                         JOIN F050802 B
                                            ON     A.DC_CODE = B.DC_CODE
                                               AND A.GUP_CODE = B.GUP_CODE
                                               AND A.CUST_CODE = B.CUST_CODE
                                               AND A.WMS_ORD_NO = B.WMS_ORD_NO                 
                                         JOIN F055002 C
                                            ON     B.DC_CODE = C.DC_CODE
                                               AND B.GUP_CODE = C.GUP_CODE
                                               AND B.CUST_CODE = C.CUST_CODE
                                               AND B.WMS_ORD_NO = C.WMS_ORD_NO                       
                                               AND B.ITEM_CODE = C.ITEM_CODE
                                          JOIN F055001 E
                                            ON     C.DC_CODE = E.DC_CODE
                                               AND C.GUP_CODE = E.GUP_CODE
                                               AND C.CUST_CODE = E.CUST_CODE
                                               AND C.WMS_ORD_NO = E.WMS_ORD_NO
                                               AND C.PACKAGE_BOX_NO = E.PACKAGE_BOX_NO
                                   WHERE     (A.DC_CODE = @p0 OR @p0 = '000')
                                         AND A.GUP_CODE = @p1
                                         AND A.CUST_CODE = @p2
                                         AND A.STATUS in (5,6)
                                         AND A.APPROVE_DATE >= @p3
                                         AND A.APPROVE_DATE < @p4                       
                                GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.WMS_ORD_NO
                                                        ,E.PAST_NO
                                                        ,A.RETAIL_CODE,C.ITEM_CODE) A
                               JOIN F05030101 F
                                  ON     A.DC_CODE = F.DC_CODE
                                     AND A.GUP_CODE = F.GUP_CODE
                                     AND A.CUST_CODE = F.CUST_CODE
                                     AND A.WMS_NO = F.WMS_ORD_NO ) TB
                        		ORDER BY DELV_DATE,WMS_NO,ITEM_CODE,PAST_NO ";

			var param = new SqlParameter[]
			{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", settleDateS),
								new SqlParameter("@p4", settleDateE),
			};

			var result = SqlQuery<SettleData>(sql, param);

			return result;
		}

		public IQueryable<P050303QueryItem> GetP050303SearchData(string gupCode, string custCode, string dcCode,
			DateTime? delvDateBegin, DateTime? delvDateEnd, string ordNo, string custOrdNo,
			string wmsOrdNo, string status, string consignNo, string itemCode)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode)
						};

			var sql = @"
						SELECT ROW_NUMBER()OVER(ORDER BY  A.WMS_ORD_NO ASC) ROWNUM,A.*
						 FROM (
							SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,B.ORD_NO,C.CUST_ORD_NO,A.WMS_ORD_NO,
                     (SELECT LOGISTIC_NAME FROM F0002 WHERE DC_CODE = @p0 AND LOGISTIC_CODE = A.SUG_LOGISTIC_CODE) SUG_LOGISTIC_CODE,
										 CASE WHEN C.PROC_FLAG = '9' THEN '7'                       -- 取消訂單
										 WHEN A.STATUS = '9' AND E.WMS_ORD_NO IS NOT NULL THEN '5'  -- 出貨單狀態為取消 = 已包裝不出貨 
										 WHEN A.STATUS ='5' THEN '4'                                -- 出貨單已出貨 = 已裝車
									 	 WHEN A.STATUS = 6 THEN '6'                                 -- 出貨單狀態為已扣帳
										 WHEN A.STATUS = 1 OR A.STATUS = 2 THEN '3'                 -- 出貨單已包裝或已稽核待出貨 = 已包裝
										 WHEN E.WMS_ORD_NO IS NOT NULL THEN '2'                     -- 出貨單所有明細已揀貨 = 已揀貨
										 WHEN C.PROC_FLAG = '1' THEN '1'                            --產生批次
										 ELSE '0' 
										 END  AS DELV_STATUS,
										 CASE WHEN E.WMS_ORD_NO IS NULL THEN '0' ELSE '1' END
										 PICK_STATUS                                                -- 0:未揀貨或揀貨中  1:揀貨完成
							FROM F050801 A
							JOIN F05030101 B
							ON B.DC_CODE = A.DC_CODE
							AND B.GUP_CODE = A.GUP_CODE
							AND B.CUST_CODE = A.CUST_CODE
							AND B.WMS_ORD_NO = A.WMS_ORD_NO
							JOIN F050301 C
							ON C.DC_CODE = B.DC_CODE
							AND C.GUP_CODE = B.GUP_CODE
							AND C.CUST_CODE = B.CUST_CODE
							AND C.ORD_NO = B.ORD_NO
							LEFT JOIN 
							(SELECT  D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO
									FROM F051201 M
									JOIN F051202 D
									ON M.DC_CODE = D.DC_CODE
									AND M.GUP_CODE = D.GUP_CODE
									AND M.CUST_CODE = D.CUST_CODE
									AND M.PICK_ORD_NO = D.PICK_ORD_NO
									GROUP BY D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO
									HAVING COUNT(D.PICK_STATUS) - SUM(CASE WHEN D.PICK_STATUS IN('1','9') THEN 1 ELSE 0 END)  = 0) E
									ON E.DC_CODE = A.DC_CODE
									AND E.GUP_CODE = A.GUP_CODE
									AND E.CUST_CODE = A.CUST_CODE
									AND E.WMS_ORD_NO = A.WMS_ORD_NO) A
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
";
			//批次日期
			if (delvDateBegin.HasValue)
				sql += parameters.Combine(" AND A.DELV_DATE >= @p{0}", delvDateBegin);
			if (delvDateEnd.HasValue)
				sql += parameters.Combine(" AND A.DELV_DATE <= @p{0}", delvDateEnd);

			//訂單編號
			sql += parameters.CombineNotNullOrEmpty(" AND A.ORD_NO = @p{0}", ordNo);

			//出貨單號
			sql += parameters.CombineNotNullOrEmpty(" AND A.WMS_ORD_NO = @p{0}", wmsOrdNo);

			//貨主單號
			sql += parameters.CombineNotNullOrEmpty(" AND A.CUST_ORD_NO = @p{0}", custOrdNo);

			//出貨狀態
			if (string.IsNullOrWhiteSpace(status))
				sql += " AND A.DELV_STATUS <> '0' AND A.DELV_STATUS <> '7'";
			else
				sql += parameters.CombineNotNullOrEmpty(" AND A.DELV_STATUS = @p{0}", status);

			sql += parameters.CombineNotNullOrEmpty(@" AND EXISTS
													  (SELECT 1
														 FROM F050901 E
														WHERE     A.WMS_ORD_NO = E.WMS_NO
															  AND A.GUP_CODE = E.GUP_CODE
															  AND A.CUST_CODE = E.CUST_CODE
															  AND A.DC_CODE = E.DC_CODE
															  AND E.CONSIGN_NO = @p{0})", consignNo);

			sql += parameters.CombineNotNullOrEmpty(@" AND EXISTS
													  (SELECT 1
														 FROM  F050802 F
									                              WHERE F.WMS_ORD_NO = A.WMS_ORD_NO
									                              AND F.CUST_CODE = A.CUST_CODE
									                              AND F.GUP_CODE = A.GUP_CODE
									                              AND F.DC_CODE = A.DC_CODE
															  AND F.ITEM_CODE = @p{0})", itemCode);

			sql += " ORDER BY A.DELV_DATE, A.ORD_NO, A.CUST_ORD_NO, A.WMS_ORD_NO";

			var result = SqlQuery<P050303QueryItem>(sql, parameters.ToArray());

			return result;
		}

		#region 檢查出貨單是否有拆單
		public IQueryable<F050801WithBill> GetF050801SeparateBillData(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var sql = @"
						SELECT  
								A.DC_CODE , A.GUP_CODE , A.CUST_CODE ,
								A.WMS_ORD_NO , B.ORD_NO ,  COUNT(B.ORD_NO) 
						FROM F050801 A
						JOIN F05030101 B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE
										AND B.WMS_ORD_NO = A.WMS_ORD_NO
						JOIN F05030101 C ON C.DC_CODE = A.DC_CODE AND C.GUP_CODE = A.GUP_CODE AND C.CUST_CODE = A.CUST_CODE
										AND C.ORD_NO = B.ORD_NO
						WHERE  
							 A.DC_CODE = @p0 AND A.GUP_CODE =@p1 AND A.CUST_CODE =@p2
							AND A.WMS_ORD_NO =@p3
						GROUP  BY  A.WMS_ORD_NO , B.ORD_NO , A.DC_CODE , A.GUP_CODE , A.CUST_CODE 
						HAVING COUNT(B.ORD_NO)  > 1  --超過一張代表拆單
						";

			var paramList = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
			var result = SqlQuery<F050801WithBill>(sql, paramList.ToArray());
			return result;
		}
		#endregion

		public IQueryable<F700101CarData> GetF700101Data(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime
																						, string sourceTye, string ordType)
		{
			var sql = $@"SELECT 
                                B.TAKE_DATE , 
                                B.TAKE_TIME , 
                                D.ALL_COMP , 
                                E.NAME EFFIC_NAME
							FROM F050801 A 
							JOIN
                ( SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO,B.TAKE_TIME,C.TAKE_DATE,C.ALL_ID,B.DELV_EFFIC
                    FROM F700102 B
                   INNER JOIN F700101 C
                      ON C.DISTR_CAR_NO = B.DISTR_CAR_NO AND C.DC_CODE = B.DC_CODE
                   WHERE C.STATUS <>'9' ) B
                ON B.WMS_NO = A.WMS_ORD_NO   AND B.DC_CODE = A.DC_CODE  AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE 
							JOIN F1947 D ON D.DC_CODE = A.DC_CODE AND D.ALL_ID = B.ALL_ID
							JOIN VW_F000904_LANG E ON E.TOPIC ='F190102' AND E.SUBTOPIC ='DELV_EFFIC' AND E.VALUE = B.DELV_EFFIC AND E.LANG = '{Current.Lang}'
							WHERE 
									A.DC_CODE = @p0 
                                    AND A.GUP_CODE = @p1
                                    AND A.CUST_CODE = @p2
									AND A.DELV_DATE = @p3  
                                    AND A.PICK_TIME = ISNULL ( @p4, A.PICK_TIME)
									AND A.ORD_TYPE = @p5
						";

			var param = new List<object>() { dcCode, gupCode, custCode, delvDate, pickTime, ordType };
			if (!string.IsNullOrEmpty(sourceTye))
			{
				sql += " AND A.SOURCE_TYPE = @p" + param.Count;
				param.Add(sourceTye);
			}

			sql += " GROUP BY D.ALL_COMP , B.TAKE_DATE , B.TAKE_TIME , E.NAME ";

			var result = SqlQuery<F700101CarData>(sql, param.ToArray());

			return result;
		}

		public void UpdateStatus(string dcCode, string gupCode, string custCode, string wmsOrdNo, int status)
		{
			var parameters = new List<object>
						{
								status,
								Current.Staff,
								Current.StaffName,
                DateTime.Now,
                gupCode,
								custCode,
								dcCode,
								wmsOrdNo
						};

			var sql = @"
				Update F050801 Set STATUS = @p0, UPD_STAFF = @p1, UPD_NAME = @p2, UPD_DATE = @p3
				 Where GUP_CODE = @p4
				   And CUST_CODE = @p5
				   And DC_CODE = @p6
				   And WMS_ORD_NO = @p7";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<DelvdtlInfo> GetDelvdtlInfo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var param = new object[] { dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY D.WMS_ORD_NO, D.CUST_CODE, D.GUP_CODE, D.DC_CODE ASC) ROWNUM, ISNULL (
                                  E.DELVDTL_FORMAT,
                                  ISNULL (F.DELVDTL_FORMAT,
                                  ISNULL (G.DELVDTL_FORMAT, ISNULL (H.DELVDTL_FORMAT, 'default'))))
                                  DELVDTL_FORMAT,
                                  CASE WHEN E.ALL_ID IS NOT NULL THEN  E.AUTO_PRINT_DELVDTL 
                                       WHEN F.ALL_ID IS NOT NULL THEN  F.AUTO_PRINT_DELVDTL 
                                       WHEN G.ALL_ID IS NOT NULL THEN  G.AUTO_PRINT_DELVDTL
                                       WHEN H.ALL_ID IS NOT NULL THEN  H.AUTO_PRINT_DELVDTL
                                       WHEN D.SELF_TAKE ='1' THEN '1' 
                                       ELSE '0' END
                                  AUTO_PRINT_DELVDTL,D.CUST_ORD_NO ORD_NO,D.CVS_TAKE,D.ALL_ID,D.SOURCE_TYPE
                          FROM (SELECT A.WMS_ORD_NO, A.CUST_CODE, A.GUP_CODE, A.DC_CODE, C.CHANNEL, C.SUBCHANNEL, C.ALL_ID,B.ORD_NO,C.CVS_TAKE,C.CUST_ORD_NO,A.SELF_TAKE,A.SOURCE_TYPE
                                  FROM F050801 A
                                       INNER JOIN F05030101 B ON A.WMS_ORD_NO = B.WMS_ORD_NO
                                       INNER JOIN F050301 C ON B.ORD_NO = C.ORD_NO
                                 WHERE     A.DC_CODE = B.DC_CODE
                                       AND A.GUP_CODE = B.GUP_CODE
                                       AND A.CUST_CODE = B.CUST_CODE
                                       AND A.DC_CODE = C.DC_CODE
                                       AND A.GUP_CODE = C.GUP_CODE
                                       AND A.CUST_CODE = C.CUST_CODE
                        			   AND A.DC_CODE = @p0
                        			   AND A.GUP_CODE = @p1
                        			   AND A.CUST_CODE = @p2
                                       AND A.WMS_ORD_NO = @p3) D
                               LEFT JOIN F190905 E
                                  ON     E.ALL_ID = D.ALL_ID
                                     AND E.CHANNEL = D.CHANNEL
                                     AND E.SUBCHANNEL = D.SUBCHANNEL
                               LEFT JOIN F190905 F
                                  ON     F.ALL_ID = D.ALL_ID
                                     AND F.CHANNEL = D.CHANNEL
                                     AND F.SUBCHANNEL = '00'
                               LEFT JOIN F190905 G
                                  ON     G.ALL_ID = D.ALL_ID
                                     AND G.CHANNEL = '00'
                                     AND G.SUBCHANNEL = D.SUBCHANNEL
                               LEFT JOIN F190905 H
                                  ON F.ALL_ID = D.ALL_ID AND H.CHANNEL = '00' AND H.SUBCHANNEL = '00' 
                        ORDER BY D.WMS_ORD_NO, D.CUST_CODE, D.GUP_CODE, D.DC_CODE";

			var result = SqlQuery<DelvdtlInfo>(sql, param);

			return result;
		}

		public IQueryable<DelvNoItem> GetDelvNoItems(DateTime calDate)
		{
			var sql = @" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.DELV_NO,B.EXTRA_FEE,C.ITEM_CODE,
													D.PACK_LENGTH,D.PACK_WIDTH,D.PACK_HIGHT,D.PACK_WEIGHT,SUM(C.B_DELV_QTY) QTY,E.REGION_FEE,E.OIL_FEE,E.OVERTIME_FEE
										 FROM F050801 A
										 JOIN F050803 B
										 	 ON B.DC_CODE = A.DC_CODE
										  AND B.GUP_CODE = A.GUP_CODE
										  AND B.CUST_CODE = A.CUST_CODE
										  AND B.WMS_ORD_NO = A.WMS_ORD_NO
										 JOIN F050802 C
											 ON C.DC_CODE = A.DC_CODE
										  AND C.GUP_CODE = A.GUP_CODE
										  AND C.CUST_CODE = A.CUST_CODE
										  AND C.WMS_ORD_NO = A.WMS_ORD_NO
										 JOIN F1905 D
											 ON D.GUP_CODE = C.GUP_CODE
										  AND D.ITEM_CODE = C.ITEM_CODE
                                            AND D.CUST_CODE = C.CUST_CODE 
                     LEFT JOIN F194716 E
                       ON B.DC_CODE = E.DC_CODE
										  AND B.GUP_CODE = E.GUP_CODE
										  AND B.CUST_CODE = E.CUST_CODE
										  AND B.DELV_NO = E.DELV_NO
                    WHERE A.STATUS <>'9' AND A.INCAR_DATE >= @p0 AND A.INCAR_DATE < @p1
									  GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.DELV_NO,B.EXTRA_FEE,C.ITEM_CODE,D.PACK_LENGTH,D.PACK_WIDTH,D.PACK_HIGHT,D.PACK_WEIGHT,E.REGION_FEE,E.OIL_FEE,E.OVERTIME_FEE ";

			var parms = new object[] { calDate.Date, calDate.Date.AddDays(1) };

			var result = SqlQuery<DelvNoItem>(sql, parms);

			return result;

		}







		public IQueryable<HealthInsuranceReport> GetHealthInsuranceSalesData(string dcCode, string gupCode, string custCode, DateTime? startDate, DateTime? endDate, string[] itemCodes)
		{
			var paramList = new List<object>() {
																dcCode,
																gupCode,
																custCode
												};

			string sql = @"SELECT ROW_NUMBER()OVER(ORDER BY Z.CRT_DATE , Z.ITEM_CODE ASC) ROWNUM,Z.*
                            FROM(
                            SELECT TOP 100 PERCENT '銷貨' AS TRANSTYPE , A.CRT_DATE , E.UNI_FORM  , E.BOSS , E.CONTACT , F.SIM_SPEC , B.ITEM_CODE , 
                            F.ITEM_NAME , B.MAKE_NO , B.VALID_DATE , G.RETAIL_NAME AS VNR_NAME , G.UNIFIED_BUSINESS_NO AS VNR_UNI_FORM 
                            , B.A_PICK_QTY AS QTY , (CONVERT(varchar, B.A_PICK_QTY)+H.ACC_UNIT_NAME) AS QTY1 , A.RETAIL_CODE AS VNR_RETIAL_CODE
                            FROM F050801 A 
                            LEFT JOIN F051202 B
                            ON A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.WMS_ORD_NO = B.WMS_ORD_NO 
                            LEFT JOIN F1909 E
                            ON A.GUP_CODE = E.GUP_CODE AND A.CUST_CODE = E.CUST_CODE 
                            LEFT JOIN F1903 F
                            ON A.GUP_CODE = F.GUP_CODE AND A.CUST_CODE = F.CUST_CODE AND B.ITEM_CODE = F.ITEM_CODE
                            LEFT JOIN F1910 G
                            ON A.GUP_CODE = G.GUP_CODE AND A.CUST_CODE = G.CUST_CODE AND A.RETAIL_CODE = G.RETAIL_CODE
                            LEFT JOIN (
                            SELECT ITEM_TYPE_ID,ACC_UNIT,ACC_UNIT_NAME 
                            FROM F91000302
                            WHERE  ITEM_TYPE_ID = '001')H
                            ON F.ITEM_UNIT = H.ACC_UNIT
                            WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
                            {0}
                            ORDER BY B.CRT_DATE , B.ITEM_CODE
                            ) Z";

			string addCondition = string.Empty;
			if (startDate != null && endDate != null)
			{
				addCondition += " AND A.CRT_DATE >= @p" + paramList.Count;
				paramList.Add(startDate.Value.ToString("yyyy/MM/dd"));

				addCondition += " AND A.CRT_DATE <= @p" + paramList.Count;
				paramList.Add(endDate.Value.AddDays(1).ToString("yyyy/MM/dd"));

				if (itemCodes.Any())
				{
					addCondition += paramList.CombineSqlInParameters(" AND B.ITEM_CODE", itemCodes);
				}
			}

			sql = string.Format(sql, addCondition);

			var result = SqlQuery<HealthInsuranceReport>(sql, paramList.ToArray());

			return result;
		}

		public IQueryable<GetPickRes> GetP810106Datas(string mode, string dcCode, string gupCode, string custCode, string wmsNo, DateTime? shipDate, string empId)
		{
			var parms = new List<SqlParameter> {
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
								new SqlParameter("@p5",Current.Lang),
								new SqlParameter("@p6",mode)
						};

			string conditionA = string.Empty;

			if (!string.IsNullOrWhiteSpace(wmsNo))
			{
				// 傳入的出貨單號(單號為O開頭)
				if (wmsNo.StartsWith("O"))
				{
					conditionA += @" AND A.WMS_ORD_NO = @p3 ";
				}

				// 傳入的揀貨單號(單號為P開頭)
				if (wmsNo.StartsWith("P"))
				{
					conditionA += @" AND B.PICK_ORD_NO = @p3 ";
				}
				parms.Add(new SqlParameter("@p3", wmsNo));
			}

			if (shipDate != null)
			{
				conditionA += " AND A.DELV_DATE = @p4 ";
				parms.Add(new SqlParameter("@p4", shipDate));
			}

			var sql = $@"SELECT 
                       	@p6 AS OTNo,
                       	C.DC_CODE AS DcNo, 
                       	C.CUST_CODE AS CustNo,
                       	C.WMS_ORD_NO AS WmsNo,
                       	C.DELV_DATE AS WmsDate,
                       	F.AREA_NAME AS AreaName,
                       	C.ITEM_CODE_CNT AS ItemCnt,
                       	C.ITEM_QTY AS ItemQty,
                       	C.PICK_ITEM_CNT AS PickItemCnt,
                       	V.NAME AS StatusName,
                       	C.ACC_NO AS AccNo,
                       	C.USER_NAME AS UserName,
                        C.STATUS As STATUS
                       FROM
                       	(SELECT 
                       	A.DC_CODE,
                       	A.GUP_CODE,
                       	A.CUST_CODE,
                       	A.PICK_ORD_NO,
                       	A.DELV_DATE,
                       	A.WMS_ORD_NO ,
                       	COUNT(DISTINCT(C.ITEM_CODE)) ITEM_CODE_CNT,
                       	SUM(C.B_PICK_QTY) ITEM_QTY,
                       	COUNT(DISTINCT(C.ITEM_CODE)) - (SELECT COUNT(DISTINCT(Z.ITEM_CODE)) FROM F051202 Z 
                       			WHERE Z.PICK_ORD_NO = A.PICK_ORD_NO 
                       			AND Z.CUST_CODE = A.CUST_CODE 
                       			AND Z.GUP_CODE = A.GUP_CODE 
                       			AND Z.DC_CODE = A.DC_CODE 
                                AND Z.WMS_ORD_NO = A.WMS_ORD_NO 
                       			AND Z.PICK_STATUS = '0') PICK_ITEM_CNT,
                       	(SELECT TOP (1) UPD_STAFF FROM F051202 Z
                        WHERE Z.DC_CODE = A.DC_CODE 
	           			   AND Z.GUP_CODE = A.GUP_CODE  
	           			   AND Z.CUST_CODE = A.CUST_CODE 
	           			   AND Z.PICK_ORD_NO = A.PICK_ORD_NO 
	           			   ORDER BY Z.UPD_DATE DESC) ACC_NO,
                       	(SELECT  TOP (1) UPD_NAME FROM F051202 Z
                       	WHERE Z.DC_CODE = A.DC_CODE 
               			   AND Z.GUP_CODE = A.GUP_CODE  
               			   AND Z.CUST_CODE = A.CUST_CODE 
               			   AND Z.PICK_ORD_NO = A.PICK_ORD_NO 
               			   ORDER BY Z.UPD_DATE DESC) USER_NAME,
                         (SELECT CASE WHEN ItemCnt = NoPickItemCnt THEN '0' ELSE CASE WHEN NoPickItemCnt = 0 THEN '2' ELSE '1' END END 
                         FROM (SELECT COUNT(DISTINCT ITEM_CODE) ItemCnt
                         FROM F051202 Z 
                         WHERE Z.DC_CODE = A.DC_CODE
                         AND Z.GUP_CODE = A.GUP_CODE
                         AND Z.CUST_CODE = A.CUST_CODE
                         AND Z.WMS_ORD_NO = A.WMS_ORD_NO) Cnt,
                         (SELECT COUNT(DISTINCT CASE WHEN PICK_STATUS = 0 THEN ITEM_CODE ELSE NULL END) NoPickItemCnt
                         FROM F051202 Z
                          WHERE Z.DC_CODE = A.DC_CODE
                         AND Z.GUP_CODE = A.GUP_CODE
                         AND Z.CUST_CODE = A.CUST_CODE
                         AND Z.WMS_ORD_NO = A.WMS_ORD_NO) NoPickCnt) STATUS
                       	--B.PICK_STATUS ,
                       	--V.NAME STATUS_NAME
                       FROM F050801 A
                       JOIN F051202 C 
                       	ON A.DC_CODE = C.DC_CODE 
                       	AND A.GUP_CODE = C.GUP_CODE 
                       	AND A.CUST_CODE = C.CUST_CODE 
                       	AND A.WMS_ORD_NO = C.WMS_ORD_NO
												JOIN F051201 B 
                       	ON C.DC_CODE = B.DC_CODE 
                       	AND C.GUP_CODE = B.GUP_CODE 
                       	AND C.CUST_CODE = B.CUST_CODE 
                       	AND C.PICK_ORD_NO = B.PICK_ORD_NO 
                        JOIN　F0513 E
						ON A.DELV_DATE = E.DELV_DATE 
						AND A.CUST_CODE =A.CUST_CODE 
						AND A.PICK_TIME =E.PICK_TIME 
						AND A.GUP_CODE = E.GUP_CODE 
						AND A.DC_CODE  = E.DC_CODE 
                       WHERE A.DC_CODE = @p0
                         AND A.GUP_CODE = @p1
                         AND A.CUST_CODE = @p2
                         AND B.PICK_STATUS IN (0, 1) 
                         AND B.DISP_SYSTEM = '0' --派發系統為WMS
                         AND B.PICK_TYPE IN('0','5') --揀貨單類型為單一揀貨或快速補揀單
                         AND B.PICK_TOOL = '2' --PDA揀貨
                        AND A.STATUS <> '9'
                         {conditionA}
                       GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.PICK_ORD_NO, A.DELV_DATE, A.WMS_ORD_NO, B.PICK_STATUS) C
                        JOIN VW_F000904_LANG V
                       	ON V.TOPIC = 'F051201'
                       	AND V.SUBTOPIC = 'PICK_STATUS'
                       	AND V.LANG = @p5
                       	AND V.VALUE = C.STATUS
                       LEFT JOIN 
                       	(SELECT 
                       		D.*, 
                       		E.AREA_NAME 
                       	FROM F05120101 D 
                       	JOIN F1919 E ON D.AREA_CODE = E.AREA_CODE 
                       				AND D.WAREHOUSE_ID = E.WAREHOUSE_ID 
                       				AND D.DC_CODE = E.DC_CODE) F
                       	ON C.DC_CODE = F.DC_CODE 
                       	AND C.GUP_CODE = F.GUP_CODE 
                       	AND C.CUST_CODE = F.CUST_CODE 
                       	AND C.PICK_ORD_NO = F.PICK_ORD_NO
                        WHERE C.STATUS !='2' --已揀貨完成不顯示";

			var result = SqlQuery<GetPickRes>(sql, parms.ToArray());
			return result;
		}

		public void UpdateStartTime(string dcCode, string gupCode, string custCode, List<string> wmsOdrNoList, DateTime startTime, string empId, string empName)
		{
			//避免傳入空的單號資料，導致全部的記錄都被更新
			if (!wmsOdrNoList.Any())
				return;

      var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",startTime){SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p1",empId){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",empName){ SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p3",DateTime.Now){ SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p4",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p5",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p6",dcCode){ SqlDbType = SqlDbType.VarChar},
			};

      int range = 100;
      int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(wmsOdrNoList.Count()) / range));

      for (int i = 0; i < index; i++)
      {
        var currNos = wmsOdrNoList.Skip(i * range).Take(range).ToList();

			  var sql = $@"
                  UPDATE 
                    F050801 
                  SET 
                    START_TIME = @p0, 
                    UPD_STAFF = @p1, 
                    UPD_NAME = @p2, 
                    UPD_DATE = @p3
                  WHERE GUP_CODE = @p4
                    AND CUST_CODE = @p5
                    AND DC_CODE = @p6
                  ";

			  sql += parms.CombineSqlInParameters("AND WMS_ORD_NO", currNos, SqlDbType.VarChar);
			  ExecuteSqlCommand(sql, parms.ToArray());
      }
		}

		public void UpdateCompleteTime(string dcCode, string gupCode, string custCode, List<string> wmsOdrNoList, DateTime completeDate, string empId, string empName)
		{
			//避免傳入空的單號資料，導致全部的記錄都被更新
			if (!wmsOdrNoList.Any())
				return;

			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",completeDate){SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p1",empId){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",empName){ SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p3",DateTime.Now){ SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p4",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p5",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p6",dcCode){ SqlDbType = SqlDbType.VarChar},
			};

			var sql = $@"Update F050801 Set COMPLETE_TIME = @p0, UPD_STAFF = @p1, UPD_NAME = @p2, UPD_DATE = @p3
			    		Where GUP_CODE = @p4
			    		And CUST_CODE = @p5
			    		And DC_CODE = @p6 ";
			sql += parms.CombineSqlInParameters("AND WMS_ORD_NO", wmsOdrNoList, SqlDbType.VarChar);
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public IQueryable<F050801> GetCollectionOutboundDatas()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F050801 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_ORD_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									) 
									";

			var result = SqlQuery<F050801>(sql, parms.ToArray());
			return result;
		}

		public IQueryable<string> GetOrderIsCancelByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT WMS_ORD_NO 
                     FROM F050801
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND STATUS = '9' ";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_ORD_NO", wmsOrdNos);
			return SqlQuery<string>(sql, parms.ToArray());
		}

		public void UpdatePickFinished(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			if (!wmsOrdNos.Any())
				return;

			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql2 = parms.CombineNotNullOrEmptySqlInParameters("AND B.WMS_ORD_NO", wmsOrdNos);

			var sql = $@" update F050801 
											SET F050801.COMPLETE_TIME = B.COMPLETE_TIME
											FROM F050801 A
											JOIN 
											(SELECT DC_CODE,GUP_CODE,CUST_CODE,WMS_ORD_NO,MAX(UPD_DATE) COMPLETE_TIME
											 FROM F051202 B
											 WHERE B.DC_CODE = @p0
											 AND B.GUP_CODE = @p1
											 AND B.CUST_CODE = @p2
											 {sql2}
											 GROUP BY DC_CODE,GUP_CODE,CUST_CODE,WMS_ORD_NO
											 HAVING  SUM(CASE WHEN B.PICK_STATUS = '0' OR B.PICK_STATUS='9' THEN 1 ELSE 0 END) =0) B
											 ON B.DC_CODE = A.DC_CODE
											 AND B.GUP_CODE = A.GUP_CODE
											 AND B.CUST_CODE = A.CUST_CODE
											 AND B.WMS_ORD_NO = A.WMS_ORD_NO
							";
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public void Update(string dcCode, string gupCode, string custCode, string wmsOrdNo, int status, string printFlag)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = status},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = printFlag},
				new SqlParameter("@p2",SqlDbType .VarChar){ Value = Current.Staff},
				new SqlParameter("@p3",SqlDbType.NVarChar){ Value = Current.StaffName},
				new SqlParameter("@p4",SqlDbType.DateTime2){ Value = DateTime.Now},
				new SqlParameter("@p5",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p6",SqlDbType.VarChar){ Value = custCode},
				new SqlParameter("@p7",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p8",SqlDbType.VarChar){ Value = wmsOrdNo},
			};

			var sql = @"
				Update F050801 Set STATUS = @p0, PRINT_FLAG = @p1, UPD_STAFF = @p2, UPD_NAME = @p3, UPD_DATE = @p4, PACK_FINISH_TIME = @p4
				 Where GUP_CODE = @p5
				   And CUST_CODE = @p6
				   And DC_CODE = @p7
				   And WMS_ORD_NO = @p8 ";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, param.ToArray());
		}

		public void UpdateIsPackCheck(string dcCode, string gupCode, string custCode, string wmsOrdNo, string isPackCheck)
		{
			var param = new object[] { isPackCheck, DateTime.Now, Current.DefaultStaffName, dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @"UPDATE F050801 SET ISPACKCHECK = @p0, UPD_DATE = @p1,UPD_NAME = @p2
						WHERE DC_CODE  = @p3
						AND GUP_CODE = @p4
						AND CUST_CODE  = @p5
						AND WMS_ORD_NO =@p6";
			ExecuteSqlCommand(sql, param);
		}

		public F050801 GetF050801ByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType .VarChar){ Value = custCode},
				new SqlParameter("@p3",SqlDbType.VarChar){ Value = wmsOrdNo}
			};

			var sql = @"SELECT * FROM F050801 WHERE DC_CODE = @p0
									AND GUP_CODE = @p1
									AND CUST_CODE = @p2
									AND WMS_ORD_NO = @p3";

			return SqlQuery<F050801>(sql, param.ToArray()).FirstOrDefault();
		}

		public string LockF050801()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F050801';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

    public void UpdateStatusWithNoCancelAndHasPrintFlag(string dcCode, string gupCode, string custCode, string wmsOrdNo, int status, DateTime startTime, DateTime completeTime)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", status) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar },
        new SqlParameter("@p3", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p4", startTime) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p5", completeTime) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p6", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p7", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p8", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p9", wmsOrdNo) { SqlDbType = SqlDbType.VarChar },
      };

			var sql = @"
				Update F050801 Set STATUS = @p0, UPD_STAFF = @p1, UPD_NAME = @p2, UPD_DATE = @p3, PRINT_FLAG = '1', PACK_START_TIME = @p4, PACK_FINISH_TIME = @p5
          Where GUP_CODE = @p6
            And CUST_CODE = @p7
            And DC_CODE = @p8
            And WMS_ORD_NO = @p9
            And STATUS <>'9'";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<CanDebitShipOrder> GetCanDebitShipOrders()
		{
			var sql = @" SELECT  TOP 500 A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO
										FROM F050801 A
										JOIN F055001 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.WMS_ORD_NO = A.WMS_ORD_NO
										WHERE A.STATUS='6'
										GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO
										HAVING MIN(B.STATUS) ='1' ";
			return SqlQuery<CanDebitShipOrder>(sql);
		}

		public IQueryable<F050801> GetNotDebitAndNotCancelShipOrdersByBatch(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",delvDate){SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p4",pickTime){SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT DISTINCT A.*
                     FROM F050801 A
                     JOIN F05030101 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.WMS_ORD_NO = A.WMS_ORD_NO
                     JOIN F050301 C
                       ON C.DC_CODE = B.DC_CODE
                      AND C.GUP_CODE = B.GUP_CODE
                      AND C.CUST_CODE = B.CUST_CODE
                      AND C.ORD_NO = B.ORD_NO
                    WHERE A.DC_CODE  =  @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND A.DELV_DATE = @p3
                      AND A.PICK_TIME = @p4
                      AND A.STATUS NOT IN('5','6','9')
                      AND C.PROC_FLAG <>'9' ";
			return SqlQuery<F050801>(sql, parms.ToArray());
		}

		public F050801 GetF050801(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType .VarChar){ Value = custCode},
				new SqlParameter("@p3",SqlDbType.VarChar){ Value = wmsOrdNo}
			};
			var sql = @" SELECT TOP(1) *
                    FROM F050801 
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE= @p2
                     AND WMS_ORD_NO = @p3";

			return SqlQuery<F050801>(sql, param.ToArray()).FirstOrDefault();
		}
		public void UpdateShipMode(string dcCode, string gupCode, string custCode, string wmsOrdNo, string shipMode)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = shipMode},
				new SqlParameter("@p1",SqlDbType.DateTime2){ Value = DateTime.Now},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = Current.Staff},
				new SqlParameter("@p3",SqlDbType.VarChar){ Value = Current.StaffName},
				new SqlParameter("@p4",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p5",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p6",SqlDbType .VarChar){ Value = custCode},
				new SqlParameter("@p7",SqlDbType.VarChar){ Value = wmsOrdNo}
			};
			var sql = @" UPDATE F050801 
                   SET SHIP_MODE = @p0,UPD_DATE = @p1,UPD_STAFF=@p2,UPD_NAME=@p3
                   WHERE DC_CODE = @p4
                     AND GUP_CODE = @p5
                     AND CUST_CODE= @p6
                     AND WMS_ORD_NO = @p7 ";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public bool IsCancelOrdByPickOrdNo(List<string> pickOrdNos)
		{
			var param = new List<SqlParameter>();
			var ordNoSql = param.CombineSqlInParameters(" AND F051202.PICK_ORD_NO ", pickOrdNos, SqlDbType.VarChar);

			var sql = $@" 
						SELECT TOP(1) 1
						  FROM F050801
						  JOIN F051202 ON F051202.DC_CODE = F050801.DC_CODE
									  AND F051202.GUP_CODE = F050801.GUP_CODE
									  AND F051202.CUST_CODE = F050801.CUST_CODE
									  AND F051202.WMS_ORD_NO = F050801.WMS_ORD_NO
						 WHERE F050801.STATUS = 9
							   { ordNoSql }
						";

			var result = SqlQuery<int>(sql, param.ToArray());

			if (result != null && result.Any())
				return true;
			return false;
		}

		public IQueryable<F050801> GetDatasForWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
			};
			var sql = @" SELECT *
                    FROM F050801 
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     ";
			sql += param.CombineSqlInParameters(" AND WMS_ORD_NO ", wmsOrdNos, SqlDbType.VarChar);

			return SqlQuery<F050801>(sql, param.ToArray());
		}

		public F050801 GetDataByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p3", SqlDbType.VarChar) { Value = wmsOrdNo },
			};
			var sql = @" SELECT TOP(1) *
                    FROM F050801 
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND WMS_ORD_NO = @p3
                     ";

			return SqlQuery<F050801>(sql, param.ToArray()).FirstOrDefault();
    }

    public void UpdatePackFinishTime(string dcCode, string gupCode, string custCode, List<string> canShipWmsNos, DateTime PackFinishTime)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3",PackFinishTime){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p4",DateTime.Now){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p5",Current.Staff){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p6",Current.StaffName){SqlDbType = SqlDbType.NVarChar}
      };

      var sql = @"UPDATE F050801 
SET PACK_START_TIME = @p3, UPD_DATE=@p4, UPD_STAFF=@p5, UPD_NAME=@p6 
WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2";
      sql += parms.CombineSqlInParameters(" AND WMS_ORD_NO", canShipWmsNos, SqlDbType.VarChar);

      ExecuteSqlCommand(sql, parms.ToArray());
    }

    public void UpdatePackStart(string dcCode, string gupCode, string custCode, string wmsOrdNo, DateTime? packStartTime, string noSpecReport, string closeByBoxNo)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3",wmsOrdNo){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p4",packStartTime.HasValue ? packStartTime : DateTime.Now){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p5",noSpecReport){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p6",closeByBoxNo){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p7",DateTime.Now){SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p8",Current.Staff){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p9",Current.StaffName){SqlDbType = SqlDbType.NVarChar}
      };

      var sql = @"
                UPDATE 
                  F050801 
                SET 
                  PACK_START_TIME = @p4, 
                  NO_SPEC_REPROTS = @p5, 
                  CLOSE_BY_BOXNO = @p6, 
                  UPD_DATE = @p7, 
                  UPD_STAFF = @p8, 
                  UPD_NAME = @p9
                WHERE
                  DC_CODE = @p0
                  AND GUP_CODE = @p1
                  AND CUST_CODE = @p2
                  AND WMS_ORD_NO = @p3
                ";

      ExecuteSqlCommand(sql, parms.ToArray());
    }

    public void UpdateOrderUnpacked(string dcCode, string gupCode, string custCode, string wmsOrdNo, int? isPackCheck)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3",wmsOrdNo){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p4",DateTime.Now){SqlDbType = SqlDbType.DateTime2}
      };

      var sql = @"
                UPDATE 
                  F050801 
                SET 
                  STATUS = 0, 
                  PRINT_FLAG = '0', 
                  PACK_CANCEL_TIME = @p4, 
                  PACK_START_TIME = NULL, 
                  PACK_FINISH_TIME = NULL 
                  {0} 
                WHERE 
                  DC_CODE = @p0 
                  AND GUP_CODE = @p1 
                  AND CUST_CODE = @p2 
                  AND WMS_ORD_NO = @p3
                ";

      var sql2 = "";

      if (isPackCheck.HasValue)
      {
        sql2 += $", ISPACKCHECK = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", isPackCheck) { SqlDbType = SqlDbType.Int });
      }

      sql = string.Format(sql, sql2);

      ExecuteSqlCommand(sql, param.ToArray());
    }

    public bool IsOrderCanceled(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = wmsNo }
      };

      var sql = @"
                SELECT TOP 1 
                  1 
                FROM F050801 
                WHERE 
                  DC_CODE = @p0 
                  AND GUP_CODE = @p1 
                  AND CUST_CODE = @p2 
                  AND WMS_ORD_NO = @p3
                  AND STATUS = 9
                ";

      return SqlQuery<int>(sql, param.ToArray()).Any();
    }
  }
}
