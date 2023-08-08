using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700101Repository : RepositoryBase<F700101, Wms3plDbContext, F700101Repository>
	{

		public void DeleteF700101(string distrCarNo, string dcCode)
		{
			var sqlParamers = new List<object>();
			sqlParamers.Add(distrCarNo);
			sqlParamers.Add(dcCode);

			string sql = @"
				delete from F700101 Where DISTR_CAR_NO=@p0 and DC_CODE =@p1
			";

			ExecuteSqlCommand(sql, sqlParamers.ToArray());
		}



		public IQueryable<F700101> GetDatas(string dcCode, string gupCode, string custCode, DateTime delvDate,
			string pickTime, string allId, string takeTime)
		{
			var sql = "SELECT DISTINCT A.* " +
						"  FROM F700101 A " +
						" INNER JOIN F700102 B " +
						"    ON B.DISTR_CAR_NO = A.DISTR_CAR_NO " +
						"   AND B.DC_CODE = A.DC_CODE " +
						" INNER JOIN F050801 C " +
						"    ON C.DC_CODE = B.DC_CODE " +
						"   AND C.GUP_CODE = B.GUP_CODE " +
						"   AND C.CUST_CODE = B.CUST_CODE " +
						"   AND C.WMS_ORD_NO = B.WMS_NO " +
						" WHERE C.DC_CODE = @p0 " +
						"   AND C.GUP_CODE = @p1 " +
						"   AND C.CUST_CODE =@p2 " +
						"   AND C.DELV_DATE =@p3 " +
						"   AND C.PICK_TIME =@p4 " +
						"   AND A.ALL_ID = @p5 " +
						"   AND B.TAKE_TIME = @p6 ";
			var param = new object[] { dcCode, gupCode, custCode, delvDate, pickTime, allId, takeTime };
			return SqlQuery<F700101>(sql, param);
		}
		

		public IQueryable<F700101DistributionRate> GetDistributionRateDatas(string dcCode, string gupCode, string custCode, DateTime? take_SDate, DateTime? take_EDate, string allId)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)
			};

			var sql = @"
SELECT ROW_NUMBER()OVER(ORDER BY TMP2.ZIP_CODE, TMP2.ZIP_NAME)ROWNUM,
	TMP2.*
	FROM (  
						  SELECT		 TMP.ZIP_CODE,
										 TMP.ZIP_NAME
										 ,
										 ROUND (ISNULL (AVG (TMP.DELV_TIME), 0), 2) AS AVG_PAST_DATE,
										 ISNULL (MAX (TMP.DELV_TIME), 0) AS MAX_PAST_DATE,
										 ISNULL (MIN (TMP.DELV_TIME), 0) AS MIN_PAST_DATE,
										 1 AS DELIVERYTIMES,
										 ROUND (
											(SUM (TMP.FourArrival) / COUNT (ISNULL (TMP.CONSIGN_ID, 0))),
											6)
											AS FOUR_ARRIVALRATE,
										 ROUND (
											(SUM (TMP.EightArrival) / COUNT (ISNULL (TMP.CONSIGN_ID, 0))),
											6)
											AS EIGHT_ARRIVALRATE,
										 ROUND (
											(SUM (TMP.TowFourArrival) / COUNT (ISNULL (TMP.CONSIGN_ID, 0))),
											6)
											AS TOWFOUR_ARRIVALRATE,
										 ROUND (
											(  SUM (TMP.OverTowFourArrival)
											 / COUNT (ISNULL (TMP.CONSIGN_ID, 0))),
											6)
											AS OVER_TOWFOUR_ARRIVALRATE
										FROM (
													SELECT b.ZIP_CODE,
															 c.ZIP_NAME,
															 DATEDIFF(MINUTE,(CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME))),d.PAST_DATE)
													   AS DELV_TIME,
															 CASE
																WHEN DATEDIFF (HOUR,(CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME))),d.PAST_DATE) <= 4
																THEN
																   1
																ELSE
																   0
															 END
																FourArrival,
															 CASE
																WHEN     DATEDIFF (HOUR,(CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME))),d.PAST_DATE) > 4
																	 AND DATEDIFF (HOUR,
																			  (CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME)))
																			 ,d.PAST_DATE) <= 8
																THEN
																   1
																ELSE
																   0
															 END
																EightArrival,
															 CASE
																WHEN     DATEDIFF (HOUR,
																			  (CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME))),d.PAST_DATE) > 8
																	 AND DATEDIFF (HOUR,
																			  (CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME)))
																			, d.PAST_DATE) <= 24
																THEN
																   1
																ELSE
																   0
															 END
																TowFourArrival,
															 CASE
																WHEN DATEDIFF (HOUR,
																		  (CONVERT(smalldatetime,ltrim(convert(date,a.TAKE_DATE))+' '+ltrim(b.TAKE_TIME)))
																		,d.PAST_DATE) > 24
																THEN
																   1
																ELSE
																   0
															 END
																OverTowFourArrival,
															 d.CONSIGN_ID,
															 d.PAST_DATE
														 FROM F700101 a
															 LEFT JOIN F700102 b
																ON     a.DC_CODE = b.DC_CODE
																   AND a.DISTR_CAR_NO = b.DISTR_CAR_NO
															 LEFT JOIN F1934 c ON b.ZIP_CODE = c.ZIP_CODE
															 LEFT JOIN F050901 d
																ON     b.DC_CODE = d.DC_CODE
																   AND b.GUP_CODE = d.GUP_CODE
																   AND b.CUST_CODE = d.CUST_CODE
																   AND b.WMS_NO = d.WMS_NO
WHERE     a.DC_CODE = @p0
															 AND ISNULL (b.ZIP_CODE, ' ') <> ' '
															 AND d.PAST_DATE IS NOT NULL 

				
";

			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += string.Format("AND b.GUP_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += string.Format("AND b.CUST_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
			}
			if (take_SDate != null)
			{
				sql += string.Format(" AND a.TAKE_DATE >= @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, take_SDate.Value.Date));
			}
			if (take_EDate != null)
			{
				sql += string.Format(" AND a.TAKE_DATE <= @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, take_EDate.Value.Date));
			}
			if (!string.IsNullOrEmpty(allId))
			{
				sql += string.Format("AND a.ALL_ID = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, allId));
			}

			sql += @" )TMP
												 GROUP BY TMP.ZIP_CODE, TMP.ZIP_NAME
            ) TMP2";

			return SqlQuery<F700101DistributionRate>(sql, parameters.ToArray()).AsQueryable();
		}

		public IQueryable<F700101DistrCarData> GetDistrCarDatas(string dcCode, string gupCode, string custCode, DateTime? take_SDate, DateTime? take_EDate, string allId)
		{
			var sql = @"
                    SELECT 
	                    ROW_NUMBER()OVER(ORDER BY I.DISTR_CAR_NO, I.TAKE_TIME) ROWNUM, 
	                    I.*
                    FROM (  SELECT E.*,
				            F.CUST_NAME,
				            G.CAR_KIND_NAME,
				            H.ALL_COMP,
				            (CASE E.SP_CAR WHEN '0' THEN '否' ELSE '是' END)
				            AS IS_SP_CAR
		            FROM (  SELECT A.DC_CODE,
						            A.DISTR_CAR_NO,
						            A.TAKE_DATE,
						            B.TAKE_TIME,
						            A.CRT_STAFF,
						            A.CRT_NAME,
						            (CASE A.SP_CAR
							            WHEN '0' THEN B.CUST_CODE
							            ELSE A.CHARGE_CUST_CODE
						            END)
							            AS CHARGE_CUST_CODE,
						            A.ALL_ID,
						            A.CAR_KIND_ID,
						            C.ROUTE,
						            ISNULL (SUM (B.ITEM_QTY), 0) AS ITEM_QTY,
						            ISNULL (SUM (B.VOLUMN), 0) AS VOLUMN,
						            B.MEMO,
						            ISNULL (
							            (CASE A.SP_CAR WHEN '0' THEN D.AMT ELSE A.FEE END),
							            0)
							            AS FEE,          -- 若是非專車，計費來源是 F5107，若是專車，計費來源是
						            A.SP_CAR,
						            B.GUP_CODE,
						            B.CUST_CODE,
						            B.WMS_NO
					            FROM F700101 A
						            LEFT JOIN F700102 B
							            ON     A.DC_CODE = B.DC_CODE
								            AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
						            LEFT JOIN (  SELECT DC_CODE,
											            GUP_CODE,
											            CUST_CODE,
											            WMS_NO,
											            MIN (ROUTE) AS ROUTE
										            FROM F050901
									            GROUP BY DC_CODE,
											            GUP_CODE,
											            CUST_CODE,
											            WMS_NO) C
							            ON     B.DC_CODE = C.DC_CODE
								            AND B.GUP_CODE = C.GUP_CODE
								            AND B.CUST_CODE = C.CUST_CODE
								            AND B.WMS_NO = C.WMS_NO
						            LEFT JOIN
						            (  SELECT DISTR_CAR_NO, WMS_NO, SUM (AMT) AS AMT
							            FROM F5107
						            GROUP BY DISTR_CAR_NO, WMS_NO) D
							            ON     B.DISTR_CAR_NO = D.DISTR_CAR_NO
								            AND B.WMS_NO = D.WMS_NO
					            WHERE     A.DC_CODE = @p0
						            AND B.GUP_CODE = ISNULL ( @p1, B.GUP_CODE)
						            AND B.CUST_CODE = ISNULL ( @p2, B.CUST_CODE)
						            AND A.TAKE_DATE BETWEEN ISNULL ( @p13, A.TAKE_DATE)
											            AND ISNULL ( @p14, A.TAKE_DATE)
						            AND A.ALL_ID = ISNULL ( @p5, A.ALL_ID)
				            GROUP BY A.DC_CODE,
						            A.DISTR_CAR_NO,
						            A.TAKE_DATE,
						            B.TAKE_TIME,
						            A.CRT_STAFF,
						            A.CRT_NAME,
						            A.CHARGE_CUST_CODE,
						            A.ALL_ID,
						            A.CAR_KIND_ID,
						            C.ROUTE,
						            B.MEMO,
						            ISNULL (
							            (CASE A.SP_CAR WHEN '0' THEN D.AMT ELSE A.FEE END),
							            0),
						            A.SP_CAR,
						            B.GUP_CODE,
						            B.CUST_CODE,
						            B.WMS_NO) E
				            LEFT JOIN F1909 F
				            ON E.GUP_CODE = F.GUP_CODE AND E.CUST_CODE = F.CUST_CODE
				            LEFT JOIN F194702 G ON E.CAR_KIND_ID = G.CAR_KIND_ID
				            LEFT JOIN F1947 H
				            ON E.DC_CODE = H.DC_CODE AND E.ALL_ID = H.ALL_ID
	                        --ORDER BY E.DISTR_CAR_NO, E.TAKE_TIME) I	
	                        ) I
                    ";

			return SqlQuery<F700101DistrCarData>(sql, new object[] { dcCode, gupCode, custCode, take_SDate, take_EDate, allId });
		}






		public void UpdateF700101StatusToCancel(string dcCode, List<string> wmsNo)
		{
			var parameters = new List<object> {
        DateTime.Now,
        Current.Staff,
				Current.StaffName,
				dcCode
			};


			var inSql1 = parameters.CombineSqlInParameters(" AND F700102.WMS_NO", wmsNo);

			string sql = @"
							UPDATE F700101 SET
									  F700101.STATUS ='9'
									, F700101.UPD_DATE = @p0
									, F700101.UPD_STAFF = @p1
									, F700101.UPD_NAME = @p2
								WHERE F700101.DC_CODE =@p3
                AND Exists (
											SELECT 0 FROM F700102
											 WHERE F700101.DISTR_CAR_NO = F700102.DISTR_CAR_NO 
                         AND F700101.DC_CODE = F700102.DC_CODE
												 AND F700101.STATUS ='0' 
												" + inSql1 + @"
											)
						";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<SettleData> GetSettleDatas(string dcCode, string gupCode, string custCode, DateTime settleDate, DateTime settleDateS, DateTime settleDateE)
		{
			var sql = string.Format(@"
SELECT ROW_NUMBER()OVER(ORDER BY DISTR_CAR_NO,TAKE_TIME,WMS_NO,PAST_NO,ITEM_CODE), tb.*
  FROM (   SELECT @p0 CAL_DATE,
                 CASE WHEN B.ORD_TYPE NOT IN ('O','R','S') THEN '01' WHEN B.RETAIL_CODE IS NULL THEN '02' ELSE '04' END DELV_ACC_TYPE,
                 A.DC_CODE,
                 A.DISTR_CAR_NO,
                 A.TAKE_DATE DELV_DATE,
                 A.ALL_ID,
                 A.SP_CAR,
                 A.FEE AMT,
                 B.GUP_CODE,
                 B.CUST_CODE,
                 B.RETAIL_CODE,
                 B.CUST_NAME,
                 B.TAKE_TIME,                 
                 F.ITEM_CODE,
                 SUM(F.PACKAGE_QTY) QTY,
                 B.DISTR_USE,
                 B.ORD_TYPE,
                 B.WMS_NO,
                 B.DELV_EFFIC,
                 B.CAN_FAST,
                 B.DELV_TMPR,
                 B.ZIP_CODE,
                 B.VOLUMN,
                 E.WEIGHT,                 
                 ISNULL(E.PACKAGE_BOX_NO,1) PACKAGE_BOX_NO,
                 E.PAST_NO,
                 CASE WHEN H.SCHEDULE_DATE IS NULL THEN 'A' ELSE 'B' END ACC_TYPE
            FROM F700101 A
                 JOIN F700102 B
                    ON     A.DC_CODE = B.DC_CODE
                       AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
                 LEFT JOIN F050901 C
                    ON B.DC_CODE = C.DC_CODE AND B.WMS_NO = C.WMS_NO
                 JOIN F050801 D
                    ON     B.DC_CODE = D.DC_CODE
                       AND B.GUP_CODE = D.GUP_CODE
                       AND B.CUST_CODE = D.CUST_CODE
                       AND B.WMS_NO = D.WMS_ORD_NO
                 JOIN F055001 E
                    ON     D.DC_CODE = E.DC_CODE
                       AND D.GUP_CODE = E.GUP_CODE
                       AND D.CUST_CODE = E.CUST_CODE
                       AND D.WMS_ORD_NO = E.WMS_ORD_NO
                 JOIN F055002 F
                    ON E.DC_CODE = F.DC_CODE
                    AND E.GUP_CODE = F.GUP_CODE
                    AND E.CUST_CODE = F.CUST_CODE
                    AND E.WMS_ORD_NO = F.WMS_ORD_NO
                    AND E.PACKAGE_BOX_NO = F.PACKAGE_BOX_NO
                 JOIN F1905 G 
                    ON     F.GUP_CODE = G.GUP_CODE
                    AND F.ITEM_CODE = G.ITEM_CODE
                    AND F.CUST_CODE = G.CUST_CODE
						LEFT JOIN F700501 H
                    ON     A.DC_CODE = H.DC_CODE
                       AND A.TAKE_DATE = H.SCHEDULE_DATE
                       AND H.SCHEDULE_TYPE = 'H'
           WHERE     A.STATUS <> '9'
                 AND ((A.SP_CAR = '1' AND A.TAKE_DATE >= @p1 AND A.TAKE_DATE < @p2)
                      OR (   A.SP_CAR = '0' AND D.INCAR_DATE >= @p3 AND D.INCAR_DATE < @p4 ))
								 AND (A.DC_CODE = @p5 OR @p6 = '000')                 
								 {0}
        GROUP BY A.DC_CODE,
                 A.DISTR_CAR_NO,
                 A.TAKE_DATE,
                 A.ALL_ID,
                 A.SP_CAR,
                 A.FEE,
                 B.GUP_CODE,
                 B.CUST_CODE,
                 B.RETAIL_CODE,
                 B.CUST_NAME,
                 B.TAKE_TIME,
                 F.ITEM_CODE,
                 B.DISTR_USE,
                 B.ORD_TYPE,
                 B.WMS_NO,
                 B.DELV_EFFIC,
                 B.CAN_FAST,
                 B.DELV_TMPR,
                 B.ZIP_CODE,
                 B.VOLUMN,
                 E.WEIGHT,                 
                 E.PACKAGE_BOX_NO,
                 E.PAST_NO,
                 H.SCHEDULE_DATE) tb 
--ORDER BY DISTR_CAR_NO,TAKE_TIME,WMS_NO,PAST_NO,ITEM_CODE ",
				string.IsNullOrWhiteSpace(gupCode)
					? ""
					: @" AND ((A.SP_CAR = '0')
												OR (A.SP_CAR = '1' AND A.CHARGE_GUP_CODE = @p7 AND A.CHARGE_CUST_CODE = @p8 AND A.CHARGE_CUST = '1')) ");
			var param = new List<object>
			{
				settleDate,settleDateS,settleDateE,settleDateS,settleDateE,dcCode,dcCode
			};
			if (!string.IsNullOrWhiteSpace(gupCode))
			{
				param.AddRange(new List<object>
				{
					gupCode,
					custCode
				});
			}
			return SqlQuery<SettleData>(sql, param.ToArray());
		}
		public void BulkUpdatStatus<T>(List<T> datas)
		{
			foreach (var d in datas)
			{
				var parms = new object[5];
        parms[0] = DateTime.Now;
        parms[1] = Current.Staff;
				parms[2] = Current.StaffName;

				foreach (var property in typeof(T).GetProperties())
				{
					if (property.Name == "DC_CODE")
						parms[3] = property.GetValue(d);
					if (property.Name == "WMS_ORD_NO")
						parms[4] = property.GetValue(d);
				}

				var sql = $@" UPDATE F700101 
                      SET F700101.STATUS = '2',
                          F700101.UPD_DATE = @p0,
                          F700101.UPD_STAFF = @p1,
                          F700101.UPD_NAME = @p2
                    WHERE EXISTS(
                          SELECT 0 
                            FROM F700102 B
                           WHERE B.DC_CODE = F700101.DC_CODE
                             AND B.DISTR_CAR_NO = F700101.DISTR_CAR_NO
                             AND F700101.DC_CODE = @p3
                             AND F700101.STATUS = '0'
                             AND B.WMS_NO = @p4
                   )";

				ExecuteSqlCommand(sql, parms);
			}
		}

		public IQueryable<F700101Data> GetF700101Datas(string dcCode, string allId, string delvTmpr, DateTime? takeDateFrom, DateTime? takeDateTo, string distrUse, string[] consignNos, string[] detailNos)
		{
			var conditionSql = string.Empty;
			var sqlParams = new List<SqlParameter> { new SqlParameter("@p0", dcCode) };
			if (!string.IsNullOrEmpty(allId))
			{
				conditionSql += " AND A.ALL_ID = @p" + sqlParams.Count;
				sqlParams.Add(new SqlParameter("@p" + sqlParams.Count, allId));
			}
			if (!string.IsNullOrEmpty(delvTmpr))
			{
				conditionSql += " AND B.DELV_TMPR = @p" + sqlParams.Count;
				sqlParams.Add(new SqlParameter("@p" + sqlParams.Count, delvTmpr));
			}
			if (takeDateFrom.HasValue)
			{
				conditionSql += " AND A.TAKE_DATE >= TO_DATE(@p" + sqlParams.Count + ", 'yyyy/mm/dd')";
				sqlParams.Add(new SqlParameter("@p" + sqlParams.Count, takeDateFrom.Value.ToString("yyyy/MM/dd")));
			}
			if (takeDateTo.HasValue)
			{
				conditionSql += " AND A.TAKE_DATE <= TO_DATE(@p" + sqlParams.Count + ", 'yyyy/mm/dd')";
				sqlParams.Add(new SqlParameter("@p" + sqlParams.Count, takeDateTo.Value.ToString("yyyy/MM/dd")));
			}
			if (!string.IsNullOrEmpty(distrUse))
			{
				conditionSql += " AND B.DISTR_USE = @p" + sqlParams.Count;
				sqlParams.Add(new SqlParameter("@p" + sqlParams.Count, distrUse));
			}
			if (consignNos.Any())
			{
				var count = sqlParams.Count;
				var consignNoOrSql = string.Join(" OR ", consignNos.Select((s, index) => string.Format(" C.CONSIGN_NO = @p{0}", count + index)));
				conditionSql += string.Format(" AND ({0}) ", consignNoOrSql);
				foreach (var item in consignNos.Select((consignNo, index) => new { consignNo, index }))
				{
					sqlParams.Add(new SqlParameter("@p" + (count + item.index), item.consignNo));
				}
			}
			if (detailNos.Any())
			{
				var count = sqlParams.Count;
				var detailNoOrSql = string.Join(" OR ", detailNos.Select((s, index) => string.Format(" D.DETAIL_NO = @p{0}", count + index)));
				conditionSql += string.Format(" AND ({0}) ", detailNoOrSql);
				foreach (var item in detailNos.Select((detailNo, index) => new { detailNo, index }))
				{
					sqlParams.Add(new SqlParameter("@p" + (count + item.index), item.detailNo));
				}
			}

			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.DISTR_CAR_NO,A.DC_CODE) ROWNUM,
	                    A.*
										FROM (
										SELECT DISTINCT A.DC_CODE,A.TAKE_DATE,B.DELV_TMPR,A.ALL_ID,A.CHARGE_CUST,A.CHARGE_DC,A.CHARGE_GUP_CODE,A.CHARGE_CUST_CODE,A.DISTR_CAR_NO,B.DELV_DATE,B.DISTR_USE,A.STATUS,B.CONTACT,B.CONTACT_TEL,B.ZIP_CODE,B.ADDRESS,B.DELV_PERIOD,B.VOLUMN,B.MEMO,C.STATUS CONSIGN_STATUS,A.CRT_DATE,A.CRT_NAME,A.UPD_DATE,A.UPD_NAME,C.CONSIGN_NO
											FROM F700101 A
										 INNER JOIN F700102 B
												ON A.DC_CODE = B.DC_CODE
											 AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
											LEFT JOIN F050901 C
												ON C.DC_CODE = B.DC_CODE
											 AND C.GUP_CODE = B.GUP_CODE
											 AND C.CUST_CODE = B.CUST_CODE
											 AND C.WMS_NO = B.DISTR_CAR_NO
											LEFT JOIN F70010201 D
												ON D.DC_CODE = A.DC_CODE
											 AND D.DISTR_CAR_NO = A.DISTR_CAR_NO
										 WHERE A.HAVE_WMS_NO = '0' --無單派車
											 AND A.DC_CODE = @p0
											 {conditionSql}
											)A ";
			return SqlQuery<F700101Data>(sql, sqlParams.ToArray());
		}

		public IQueryable<F055001Data> GetConsignData(string dcCode, string distrCarNo)
		{
#if DEBUG
			var sql2 = " AND G.ISTEST = '1' ";
#else
			var sql2 = " AND G.ISTEST='0' ";
#endif

			var sql = $@" SELECT 
        ROW_NUMBER()OVER(ORDER BY A.DC_CODE,B.GUP_CODE,C.CUST_CODE,C.CONSIGN_NO)[ROWNUM],
        A.DC_CODE,B.GUP_CODE,C.CUST_CODE,C.CONSIGN_NO PAST_NO,CONVERT (VARCHAR,A.TAKE_DATE,112) AS DELV_DATE,CONVERT(VARCHAR,B.DELV_DATE,111) AS ARRIVAL_DATE,A.DISTR_CAR_NO AS CUST_ORD_NO,'不可代收' COLLECT,
													CASE WHEN B.DISTR_USE = '01' THEN B.CONTACT ELSE D.DC_NAME END CONSIGNEE,
													CASE WHEN B.DISTR_USE = '01' THEN B.ZIP_CODE + ' '+ B.ADDRESS ELSE D.ZIP_CODE + ' ' + D.ADDRESS END ADDRESS,
													CASE WHEN B.DISTR_USE = '01' THEN B.CONTACT_TEL ELSE D.TEL END TEL,
													B.MEMO,
													0 COLLECT_AMT,
													0 SA_QTY,
													C.ERST_NO,
													'' SHORT_NAME,
													CASE WHEN B.DISTR_USE = '01' THEN B.CONTACT_TEL ELSE D.TEL END CUST_TEL,
													CASE WHEN B.DISTR_USE = '01' THEN B.ZIP_CODE + ' ' + B.ADDRESS ELSE D.ZIP_CODE + ' ' + D.ADDRESS END CUST_ADDRESS,
													1 AS TOTAL_AMOUNT,
													C.ROUTE,
													 '001' AS FIXED_CODE,
													 '' ADDRESS_TYPE,
													 B.RETAIL_CODE,
													 B.CUST_NAME RETAIL_NAME,
													 B.CUST_NAME,
													 '' CHANNEL,
													 CONVERT (VARCHAR,@p0,111)+ ' ' + CONVERT (VARCHAR,@p0,108) AS PRINT_TIME,
													 E.CONSIGN_ID,E.MEMO AS CONSIGN_MEMO,E.CONSIGN_NAME,
													 NULL RETAIL_DELV_DATE,
													 NULL RETAIL_RETURN_DATE,
													 A.ALL_ID,
													 '' ORD_NO,
													 CONVERT(VARCHAR,A.TAKE_DATE, 111) AS TCAT_DELV_DATE,
													 CONVERT(VARCHAR,ISNULL(B.DELV_DATE,A.TAKE_DATE), 111) AS TCAT_ARRIVAL_DATE,
													 B.MEMO  TCAT_MEMO,
													 '' TCAT_PLACE,
													 CONVERT(VARCHAR,B.VOLUMN) TCAT_SIZE,
													 F.NAME TCAT_TIME,
													 CASE WHEN B.DISTR_USE = '01' THEN D.DC_NAME ELSE B.CONTACT END CHANNEL_NAME,
													CASE WHEN B.DISTR_USE = '01' THEN D.ZIP_CODE + ' ' + D.ADDRESS  ELSE B.ZIP_CODE + ' ' + B.ADDRESS END CHANNEL_ADDRESS,
													CASE WHEN B.DISTR_USE = '01' THEN D.TEL ELSE B.CONTACT_TEL  END CHANNEL_TEL,
													'' PAST_NOByCode128,
													G.CUSTOMER_ID,
													'' ESERVICE,
													'' ESERVICE_NAME,
													'' ESHOP,
													'' ESHOP_ID,
													'' PLATFORM_NAME,
													'' VNR_NAME,
													'' CUST_INFO,
													'' NOTE1,
													'' NOTE2,
													'' NOTE3,
													'' SHOW_ISPAID_NOTE,
													'' INVOICE,
													NULL INVOICE_DATE,
													'' IDENTIFIER   
										FROM F700101 A
									 INNER JOIN F700102 B
									    ON A.DC_CODE = B.DC_CODE
									   AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
									  LEFT JOIN F050901 C
									    ON C.DC_CODE = B.DC_CODE
									   AND C.GUP_CODE = B.GUP_CODE
									   AND C.CUST_CODE = B.CUST_CODE
									   AND C.WMS_NO = B.DISTR_CAR_NO
									  LEFT JOIN F1901 D
									    ON D.DC_CODE = A.DC_CODE
									  LEFT JOIN F194711 E 
									    ON E.DC_CODE = B.DC_CODE AND E.GUP_CODE = B.GUP_CODE AND E.CUST_CODE = B.CUST_CODE AND E.ALL_ID = A.ALL_ID
									  LEFT JOIN VW_F000904_LANG  F ON F.TOPIC='F050301' AND F.SUBTOPIC='DELV_PERIOD' AND F.VALUE = B.DELV_PERIOD AND F.LANG = '{Current.Lang}'
									  LEFT JOIN F19471201 G ON G.DC_CODE = B.DC_CODE AND G.GUP_CODE = B.GUP_CODE AND G.CUST_CODE = B.CUST_CODE AND G.CHANNEL='00' AND G.ALL_ID = A.ALL_ID AND G.CONSIGN_NO = C.CONSIGN_NO {sql2}
									WHERE A.HAVE_WMS_NO = '0' --無單派車
										AND A.DC_CODE = @p1
										AND A.DISTR_CAR_NO = @p2 ";

			var parameters = new List<SqlParameter>
						{
              new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2},
              new SqlParameter("@p1", dcCode) {SqlDbType = SqlDbType.VarChar},
              new SqlParameter("@p2", distrCarNo) {SqlDbType = SqlDbType.VarChar}
						};

			return SqlQuery<F055001Data>(sql, parameters.ToArray());
		}
		/// <summary>
		/// 有單派車 逆物流
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public IQueryable<EgsReturnConsign2> GetEgsReturnConsignsByHaveWmsNoBack(EgsReturnConsignParam param)
		{
			var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			var sql = @" 
                    SELECT DISTINCT 
													A.DC_CODE, --物流中心
													B.GUP_CODE, --業主
													B.CUST_CODE,--貨主
                          A.ALL_ID, --配送商
													B.WMS_NO WMS_ORD_NO, --單號
													'3'  CONSIGN_TYPE, --托單類別(1:客戶自行列印託運單,2:速達協助列印 (由速達系統分配託運單號),3:已有單號，由速達列印(A4二模) –逆物流收退貨)
													C.CONSIGN_NO, --託運單號
													B.WMS_NO CUST_ORD_NO, --訂單編號
													E.CUSTOMER_ID, --契客代號
													'00' + CASE WHEN B.DELV_TMPR ='A' THEN '01' ELSE '02' END  TEMPERATURE, --溫層(0001:常溫  0002:冷藏 0003:冷凍)
													'' DISTANCE, --距離(00:同縣市 01:外縣市 02:離島)
                          '0001'  SPEC,   --規格(0001: 60cm   0002: 90cm   0003: 120cm  0004: 150cm)
												  'N'  ISCOLLECT, --是否代收(	N:否  Y:是)
												  0 COLLECT_AMT, -- 代收金額
												  'N' ARRIVEDPAY, --是否到付(無作用，請固定填N)
												  '01' PAYCASH, --是否付現(00:付現 01:月結)
												  F.DC_NAME  RECEIVER_NAME, -- 收件者姓名
												  '***' RECEIVER_MOBILE, --收件者手機號碼
												  F.TEL RECEIVER_PHONE, --收件者電話
												  ISNULL(G.ZIP_CODE,'') RECEIVER_SUDA5,--收件人地址的速達五碼郵遞區號(必填)
												  F.ADDRESS RECEIVER_ADDRESS, --收件者地址(必填)
												  B.CONTACT  SENDER_NAME, --寄件者姓名(必填)
												  B.CONTACT_TEL SENDER_TEL,--寄件者電話
												  '' SENDER_MOBILE, --寄件者手機
												  '' SENDER_SUDA5, --寄件者地址速達五碼郵遞區號(必填)
												  B.ADDRESS SENDER_ADDRESS, --寄件者地址(必填)
												  convert(varchar,B.DELV_DATE,112)+REPLACE(convert(varchar,B.DELV_DATE,108),':','') SHIP_DATE, --契客出貨日期(系統日YYYYMMDDhhmmss共14碼) 正物流為系統日 逆物流為預定配達日/出貨日的時間
												  '4' PICKUP_TIMEZONE, --預定取件時段(1: 9~12    2: 12~17    3: 17~20   4: 不限時(固定4不限時))
												  B.DELV_PERIOD  DELV_TIMEZONE, --預定配達時段(1: 9~12    2: 12~17   3: 17~20   4: 不限時  5:20~21(需限定區域))
                          '' MEMBER_ID, --會員編號
												  '' ITEM_NAME,--物品名稱
												  'N' ISFRAGILE,--易碎物品(固定填N)
												  'N' ISPRECISON_INSTRUMENT,--精密儀器(固定填N)
												  B.MEMO, --備註
												  '' SD_ROUTE, --SD路線代碼 
                          B.DISTR_USE   
										 FROM F700101 A
										 JOIN F700102 B
											 ON B.DC_CODE = A.DC_CODE
										  AND B.DISTR_CAR_NO = A.DISTR_CAR_NO
										 JOIN F050901 C
											 ON C.DC_CODE = B.DC_CODE
										  AND C.GUP_CODE =B.GUP_CODE
										  AND C.CUST_CODE = B.CUST_CODE
										  AND C.WMS_NO = B.WMS_NO
										 JOIN F19471201 D
											 ON D.DC_CODE = C.DC_CODE
										  AND D.GUP_CODE = C.GUP_CODE
                      AND D.CUST_CODE = C.CUST_CODE
										  AND D.ALL_ID = A.ALL_ID
										  AND D.CONSIGN_NO = C.CONSIGN_NO
										  AND D.ISUSED='1'
										 JOIN F194712 E
										 	 ON E.DC_CODE = D.DC_CODE
										  AND E.GUP_CODE = D.GUP_CODE
                      AND E.CUST_CODE = D.CUST_CODE
                      AND E.CHANNEL = D.CHANNEL
										  AND E.ALL_ID = D.ALL_ID
										  AND E.CUSTOMER_ID = D.CUSTOMER_ID
								 		  AND E.CONSIGN_TYPE = D.CONSIGN_TYPE
										  AND E.ISTEST = D.ISTEST
										 LEFT JOIN F1901 F
											 ON F.DC_CODE = A.DC_CODE
										 LEFT JOIN F194704 G
											 ON G.DC_CODE = B.DC_CODE
										  AND G.GUP_CODE = B.GUP_CODE
										  AND G.CUST_CODE = B.CUST_CODE
										  AND G.ALL_ID = A.ALL_ID
									  WHERE C.DISTR_EDI_STATUS ='0' --取得配送商未回檔資料
										  AND A.STATUS <> '9' -- 非取消派車單
										  AND A.HAVE_WMS_NO = '1' --有單派車
										  AND B.DISTR_USE = '02' 
										  AND CONVERT(DATE,@p0) =  CONVERT(DATE,DATEADD(DAY,-1,B.DELV_DATE)) --逆物流 且系統日=預計配達日/收貨日-1
                   ";
			//客戶代號
			if (!string.IsNullOrWhiteSpace(param.CustomerId))
			{
				sql += " AND D.CUSTOMER_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.CustomerId));
			}
			//物流中心
			if (!string.IsNullOrWhiteSpace(param.DcCode))
			{
				sql += " AND A.DC_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DcCode));
			}
			//業主
			if (!string.IsNullOrWhiteSpace(param.GupCode))
			{
				sql += " AND B.GUP_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.GupCode));
			}
			//貨主
			if (!string.IsNullOrWhiteSpace(param.CustCode))
			{
				sql += " AND B.CUST_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.CustCode));
			}
			//通路
			if (!string.IsNullOrWhiteSpace(param.Channel))
			{
				sql += " AND D.CHANNEL = ISNULL((SELECT CHANNEL FROM F194712 WHERE CHANNEL = @p" + parms.Count + "),'00') ";
				parms.Add(new SqlParameter("@p" + parms.Count, param.Channel));
			}
			else
			{
				sql += " AND D.CHANNEL = '00' ";
			}
			//配送商
			if (!string.IsNullOrWhiteSpace(param.AllId))
			{
				sql += " AND A.ALL_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.AllId));
			}
			//配次
			if (!string.IsNullOrWhiteSpace(param.DelvTimes))
			{
				sql += " AND B.DELV_TIMES = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DelvTimes));
			}



#if DEBUG
			sql += " AND D.ISTEST = '1' ";
#else
			sql += " AND D.ISTEST = '0' ";
#endif

			return SqlQuery<EgsReturnConsign2>(sql, parms.ToArray());
		}

		/// <summary>
		/// 無單派車 正/逆物流
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public IQueryable<EgsReturnConsign2> GetEgsReturnConsignsByNoWmsNo(EgsReturnConsignParam param)
		{
			var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			var sql = @" 
SELECT DISTINCT 
													A.DC_CODE, --物流中心
													B.GUP_CODE, --業主
													B.CUST_CODE,--貨主
                          A.ALL_ID, --配送商
													B.WMS_NO WMS_ORD_NO, --單號
													CASE WHEN B.DISTR_USE ='01' THEN '1' ELSE '3' END CONSIGN_TYPE, --托單類別(1:客戶自行列印託運單,2:速達協助列印 (由速達系統分配託運單號),3:已有單號，由速達列印(A4二模) –逆物流收退貨)
													C.CONSIGN_NO, --託運單號
													A.DISTR_CAR_NO CUST_ORD_NO, --訂單編號
													E.CUSTOMER_ID, --契客代號
													'00' + CASE WHEN B.DELV_TMPR ='A' THEN '01' ELSE '02' END  TEMPERATURE, --溫層(0001:常溫  0002:冷藏 0003:冷凍)
													'' DISTANCE, --距離(00:同縣市 01:外縣市 02:離島)
													CASE WHEN B.VOLUMN >=0 AND B.VOLUMN <=60 
                          THEN '0001' 
                          ELSE
                              CASE WHEN B.VOLUMN >60 AND B.VOLUMN <=90 
                              THEN '0002'
                              ELSE
                                  CASE WHEN B.VOLUMN >90 AND B.VOLUMN <=120 
                                  THEN '0003' 
                                  ELSE '0004' 
                                  END
                              END
                          END SPEC,   --規格(0001: 60cm   0002: 90cm   0003: 120cm  0004: 150cm)
												'N'  ISCOLLECT, --是否代收(	N:否  Y:是)
												 0 COLLECT_AMT, -- 代收金額
												'N' ARRIVEDPAY, --是否到付(無作用，請固定填N)
												'01' PAYCASH, --是否付現(00:付現 01:月結)
												CASE WHEN B.DISTR_USE ='01' THEN B.CONTACT ELSE F.DC_NAME END RECEIVER_NAME, -- 收件者姓名
												CASE WHEN B.DISTR_USE ='01' THEN '***' ELSE '***'  END  RECEIVER_MOBILE, --收件者手機號碼
												CASE WHEN B.DISTR_USE ='01' THEN '***' ELSE F.TEL END RECEIVER_PHONE, --收件者電話
												CASE WHEN B.DISTR_USE ='01' THEN '' ELSE ISNULL(G.ZIP_CODE,'') END RECEIVER_SUDA5,--收件人地址的速達五碼郵遞區號(必填)
												CASE WHEN B.DISTR_USE ='01' THEN B.ADDRESS ELSE F.ADDRESS END RECEIVER_ADDRESS, --收件者地址(必填)
												CASE WHEN B.DISTR_USE ='01' THEN F.DC_NAME ELSE B.CONTACT END SENDER_NAME, --寄件者姓名(必填)
												CASE WHEN B.DISTR_USE ='01' THEN F.TEL ELSE B.CONTACT_TEL END SENDER_TEL,--寄件者電話
												CASE WHEN B.DISTR_USE ='01' THEN '' ELSE '' END SENDER_MOBILE, --寄件者手機
												CASE WHEN B.DISTR_USE ='01' THEN ISNULL(G.ZIP_CODE,'') ELSE '' END SENDER_SUDA5, --寄件者地址速達五碼郵遞區號(必填)
												CASE WHEN B.DISTR_USE ='01' THEN F.ADDRESS ELSE B.ADDRESS END  SENDER_ADDRESS, --寄件者地址(必填)
												CASE WHEN B.DISTR_USE ='01' THEN CONVERT(VARCHAR,@p0,112)+REPLACE(CONVERT(VARCHAR,@p0,108),':','') ELSE CONVERT(VARCHAR,B.DELV_DATE,112)+REPLACE(CONVERT(VARCHAR,B.DELV_DATE,108),':','') END SHIP_DATE, --契客出貨日期(系統日YYYYMMDDhhmmss共14碼) 正物流為系統日 逆物流為預定配達日/出貨日的時間
												'4' PICKUP_TIMEZONE, --預定取件時段(1: 9~12    2: 12~17    3: 17~20   4: 不限時(固定4不限時))
												B.DELV_PERIOD  DELV_TIMEZONE, --預定配達時段(1: 9~12    2: 12~17   3: 17~20   4: 不限時  5:20~21(需限定區域))
                        '' MEMBER_ID, --會員編號
												'' ITEM_NAME,--物品名稱
												'N' ISFRAGILE,--易碎物品(固定填N)
												'N' ISPRECISON_INSTRUMENT,--精密儀器(固定填N)
												B.MEMO, --備註
												'' SD_ROUTE, --SD路線代碼 
                        B.DISTR_USE   
									FROM F700101 A
									JOIN F700102 B
									  ON B.DC_CODE = A.DC_CODE
									 AND B.DISTR_CAR_NO = A.DISTR_CAR_NO
									JOIN F050901 C
									  ON C.DC_CODE = B.DC_CODE
									 AND C.GUP_CODE =B.GUP_CODE
									 AND C.CUST_CODE = B.CUST_CODE
									 AND C.WMS_NO = B.DISTR_CAR_NO
									JOIN F19471201 D
									  ON D.DC_CODE = C.DC_CODE
									 AND D.GUP_CODE = C.GUP_CODE
                   AND D.CUST_CODE=  C.CUST_CODE
                   AND D.ALL_ID = A.ALL_ID
									 AND D.CONSIGN_NO = C.CONSIGN_NO
                   AND D.CHANNEL = '00' --無單通路都抓00
									 AND D.ISUSED='1'
									JOIN F194712 E
									  ON E.DC_CODE = D.DC_CODE
									 AND E.GUP_CODE = D.GUP_CODE
                   AND E.CUST_CODE = D.CUST_CODE
                   AND E.CHANNEL = D.CHANNEL
									 AND E.ALL_ID = D.ALL_ID
									 AND E.CUSTOMER_ID = D.CUSTOMER_ID
								 	 AND E.CONSIGN_TYPE = D.CONSIGN_TYPE
                   AND E.ISTEST = D.ISTEST
									LEFT JOIN F1901 F
									  ON F.DC_CODE = A.DC_CODE
                  LEFT JOIN F194704 G
                    ON G.DC_CODE = B.DC_CODE
                   AND G.GUP_CODE = B.GUP_CODE
                   AND G.CUST_CODE = B.CUST_CODE
                   AND G.ALL_ID = A.ALL_ID
								 WHERE C.DISTR_EDI_STATUS ='0' --取得配送商未回檔資料
									 AND A.STATUS <> '9' -- 非取消派車單
                   AND A.HAVE_WMS_NO = '0' --無單派車
                   AND B.GUP_CODE= '00' --無單派車固定00
                   AND B.CUST_CODE = '0' --無單派車固定0
                   AND (B.DISTR_USE = '01' OR ( B.DISTR_USE = '02'  AND CONVERT(date,@p0) =  convert(date,dateadd(DAY,-1,B.DELV_DATE)) ))
                   ";
			//客戶代號
			if (!string.IsNullOrWhiteSpace(param.CustomerId))
			{
				sql += " AND E.CUSTOMER_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.CustomerId));
			}
			//物流中心
			if (!string.IsNullOrWhiteSpace(param.DcCode))
			{
				sql += " AND A.DC_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DcCode));
			}
			//配送商
			if (!string.IsNullOrWhiteSpace(param.AllId))
			{
				sql += " AND A.ALL_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.AllId));
			}
			//物流方向
			if (!string.IsNullOrWhiteSpace(param.DISTR_USE))
			{
				sql += " AND B.DISTR_USE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DISTR_USE));
			}


#if DEBUG
			sql += " AND E.ISTEST = '1' ";
#else
			sql += " AND E.ISTEST = '0' ";
#endif

			return SqlQuery<EgsReturnConsign2>(sql, parms.ToArray());
		}

		/// <summary>
		/// 取得已開始包裝後的出貨單號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="distrCarNo"></param>
		/// <returns></returns>
		public IQueryable<string> GetPackagedWmsOrdNos(string dcCode, IEnumerable<string> distrCarNos)
		{
			var sql = @"SELECT DISTINCT C.WMS_ORD_NO
						  FROM F700101 A
							   JOIN F700102 B
								  ON A.DC_CODE = B.DC_CODE AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
							   JOIN F050801 C
								  ON     B.DC_CODE = C.DC_CODE
									 AND B.GUP_CODE = C.GUP_CODE
									 AND B.CUST_CODE = C.CUST_CODE
									 AND B.WMS_NO = C.WMS_ORD_NO
							   LEFT JOIN F055001 D
								  ON     B.DC_CODE = D.DC_CODE
									 AND B.GUP_CODE = D.GUP_CODE
									 AND B.CUST_CODE = D.CUST_CODE
									 AND B.WMS_NO = D.WMS_ORD_NO
						 WHERE     (D.WMS_ORD_NO IS NOT NULL OR C.STATUS BETWEEN 1 AND 6) -- 包裝中，或者已包裝到結案
							   AND ISNULL (C.NO_DELV, '0') = '0'                                 -- 且為可出貨
							   AND A.DC_CODE = @p0";

			var paramList = new List<object> { dcCode };
			sql += paramList.CombineSqlInParameters("AND A.DISTR_CAR_NO", distrCarNos);

			return SqlQuery<string>(sql, paramList.ToArray());
		}
	}
}