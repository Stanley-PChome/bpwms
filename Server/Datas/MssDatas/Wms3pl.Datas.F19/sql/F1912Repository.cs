using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1912Repository : RepositoryBase<F1912, Wms3plDbContext, F1912Repository>
	{
		/// <summary>
		/// 取得儲位清單. 供儲位管制使用.
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="warehouseId"></param>
		/// <param name="areaId"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F1912StatusEx> GetLocListForLocControl(string dcCode, string gupCode, string custCode
				, string warehouseType, string warehouseId, string areaId, string channel, string itemCode, string account)
		{
			var parameters = new List<SqlParameter> {
								new SqlParameter("@p0", dcCode){SqlDbType=SqlDbType.VarChar }
						};

			var sql = @" SELECT A.LOC_CODE,
						         A.AREA_CODE,
						         C.AREA_NAME,
						         A.WAREHOUSE_ID,
						         B.WAREHOUSE_NAME,
						         A.UCC_CODE,
						         D.CAUSE,
						         A.NOW_STATUS_ID,
						         E.LOC_STATUS_NAME,
						         A.DC_CODE,
						         A.GUP_CODE,
						         A.CUST_CODE,
								 (CASE A.GUP_CODE WHEN '0' THEN N'共用' ELSE F.GUP_NAME END ) GUP_NAME,
								 (CASE A.CUST_CODE WHEN '0' THEN N'共用' ELSE G.CUST_NAME END ) CUST_NAME
						    FROM F1912 A
						         JOIN F1980 B
						            ON A.WAREHOUSE_ID = B.WAREHOUSE_ID AND A.DC_CODE = B.DC_CODE
						         JOIN F198001 H ON B.WAREHOUSE_TYPE = H.TYPE_ID
						         JOIN F1919 C ON A.AREA_CODE = C.AREA_CODE AND A.DC_CODE = C.DC_CODE AND A.WAREHOUSE_ID=C.WAREHOUSE_ID
						         LEFT JOIN F1943 E                                             -- 儲位狀態
						                          ON A.NOW_STATUS_ID = E.LOC_STATUS_ID
						         LEFT JOIN F1929 F ON A.GUP_CODE = F.GUP_CODE
						         LEFT JOIN F1909 G
						            ON A.GUP_CODE = G.GUP_CODE AND A.CUST_CODE = G.CUST_CODE
						         LEFT JOIN F1951 D                                               -- 原因
						                          ON A.UCC_CODE = D.UCC_CODE AND D.UCT_ID = 'LB'
						   WHERE     A.DC_CODE = @p0 
						  
";
			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += "    AND A.GUP_CODE     = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode) { SqlDbType = SqlDbType.VarChar });
			}
			else //業主全部要去篩選只有此物流中心業主或業主設為共用
			{
				sql += string.Format(@" AND ((A.GUP_CODE ='0') OR (EXISTS (SELECT 1 
																												             FROM F190101 aa 
																												       INNER JOIN (SELECT * 
																												       				       FROM F192402 
																												       						  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
																												       		  WHERE aa.DC_CODE = A.DC_CODE AND aa.GUP_CODE = A.GUP_CODE))) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account) { SqlDbType = SqlDbType.VarChar });
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += "    AND A.CUST_CODE    = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, custCode) { SqlDbType = SqlDbType.VarChar });
			}
			else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
			{
				sql += string.Format(@" AND ((A.CUST_CODE ='0') OR (EXISTS (SELECT 1 
																																				FROM F190101 cc 
																																	INNER JOIN (SELECT * 
																												       								  FROM F192402 
																												       								 WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
																												       					WHERE cc.DC_CODE = A.DC_CODE AND cc.GUP_CODE = A.GUP_CODE AND cc.CUST_CODE = A.CUST_CODE))) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account) { SqlDbType = SqlDbType.VarChar });
			}
			if (!string.IsNullOrEmpty(warehouseId))
			{
				sql += string.Format("  AND A.WAREHOUSE_ID = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, warehouseId) { SqlDbType = SqlDbType.VarChar });
			}
			if (!string.IsNullOrEmpty(warehouseType))
			{
				sql += string.Format("  AND H.TYPE_ID =  @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, warehouseType) { SqlDbType = SqlDbType.VarChar });
			}
			if (!string.IsNullOrEmpty(areaId))
			{
				sql += string.Format("  AND A.AREA_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, areaId) { SqlDbType = SqlDbType.VarChar });
			}
			if (!string.IsNullOrEmpty(channel))
			{
				sql += string.Format("  AND A.CHANNEL = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, channel) { SqlDbType = SqlDbType.Char });
			}
			sql += @" -- 要加上商品名稱判斷
						ORDER BY A.LOC_CODE ";


			return SqlQuery<F1912StatusEx>(sql, parameters.ToArray()).AsQueryable();
		}
		public IQueryable<F1912WareHouseData> GetCustWarehouseDatas(string dcCode, string gupCode, string custCode)
		{
			var sql = @" 
							SELECT 
									A.WAREHOUSE_ID  , A.DC_CODE , A.GUP_CODE , A.CUST_CODE 
									, B.WAREHOUSE_NAME , B.WAREHOUSE_TYPE, B.DEVICE_TYPE
							FROM F1912 A 
							JOIN F1980 B ON B.DC_CODE = A.DC_CODE AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
							WHERE 
									A.DC_CODE = @p0
									AND (A.GUP_CODE =@p1 OR A.GUP_CODE ='0')
									AND (A.CUST_CODE =@p2 OR A.CUST_CODE ='0')
							GROUP BY A.WAREHOUSE_ID , A.DC_CODE , A.GUP_CODE , A.CUST_CODE , B.WAREHOUSE_NAME , B.WAREHOUSE_TYPE,B.DEVICE_TYPE
							ORDER BY WAREHOUSE_ID
						";
			var param = new object[] { dcCode, gupCode, custCode };

			return SqlQuery<F1912WareHouseData>(sql, param);
		}

		/// <summary>
		/// 取得儲位清單. 供儲位管制使用.
		/// 以Item Code查詢 (F1913為主TABLE)
		/// Memo: 1. 改為F1912StatusEx2, 加上ITEM_CODE, ITEM_NAME, 因為在F1902相同ITEM_CODE可能會有不同ITEM_NAME
		///       2. 加上DISTINCT語法, 因為在F1913相同ITEM/LOC會有不同VALID_DATE, 會取出重複資料
		///       3. 改為讀取F1913的GUP/CUST, 以便在查詢結果識別出不同的GUP/CUST
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F1912StatusEx2> GetLocListForLocControlByItemCode(string dcCode, string gupCode, string custCode
				, string itemCode, string account)
		{
			var parameters = new List<SqlParameter> {
								new SqlParameter("@p0", dcCode),

                //此兩項為選用項目
				//new SqlParameter("@p1", gupCode),
				//new SqlParameter("@p2", custCode),

				new SqlParameter("@p1", itemCode)
						};

			var sql = @"
					SELECT DISTINCT M1.ITEM_CODE, M2.ITEM_NAME, M3.*, M1.GUP_CODE, M1.CUST_CODE
			                    , (SELECT GUP_NAME FROM F1929 F1 WHERE F1.GUP_CODE = M1.GUP_CODE) AS GUP_NAME
			                    , (SELECT CUST_NAME FROM F1909 G1 WHERE G1.CUST_CODE = M1.CUST_CODE) AS CUST_NAME
                    FROM F1913 M1, F1903 M2
			                    , (SELECT A.LOC_CODE, A.AREA_CODE, C.AREA_NAME, A.WAREHOUSE_ID, B.WAREHOUSE_NAME, A.UCC_CODE, D.CAUSE, A.NOW_STATUS_ID, E.LOC_STATUS_NAME
								                    , A.DC_CODE /*, A.GUP_CODE, A.CUST_CODE, F.GUP_NAME, G.CUST_NAME */
					                    FROM  F1912 A
					                    LEFT JOIN F1980 B ON A.WAREHOUSE_ID = B.WAREHOUSE_ID AND A.DC_CODE = B.DC_CODE
					                    LEFT JOIN F1919 C ON 
						                    A.DC_CODE = C.DC_CODE 
						                    AND A.AREA_CODE = C.AREA_CODE 
						                    AND A.WAREHOUSE_ID = C.WAREHOUSE_ID
					                    LEFT JOIN F1943 E ON 
						                    A.NOW_STATUS_ID = E.LOC_STATUS_ID

					                    LEFT JOIN F1929 F ON 
						                    A.GUP_CODE = F.GUP_CODE
					                    LEFT JOIN F1909 G ON 
						                    A.CUST_CODE = G.CUST_CODE 
						                    AND A.GUP_CODE = G.GUP_CODE
					                    LEFT JOIN (SELECT UCC_CODE, CAUSE FROM F1951 WHERE UCT_ID = 'LB') D ON 
						                    A.UCC_CODE = D.UCC_CODE
					                    WHERE 
						                        A.DC_CODE = @p0 ";
			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += $" AND A.GUP_CODE = @p{parameters.Count()} ";
				parameters.Add(new SqlParameter($"@p{parameters.Count()}", gupCode));
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += $" AND A.CUST_CODE = @p{parameters.Count()} ";
				parameters.Add(new SqlParameter($"@p{parameters.Count()}", custCode));
			}
			sql += @"
						                    
						                    AND A.LOC_CODE IN (SELECT LOC_CODE FROM F1913 H
											                    WHERE H.ITEM_CODE = @p1 AND H.DC_CODE = @p0 ";
			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += $" AND H.GUP_CODE = @p{parameters.Count()} ";
				parameters.Add(new SqlParameter($"@p{parameters.Count()}", gupCode));
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += $" AND H.CUST_CODE = @p{parameters.Count()} ";
				parameters.Add(new SqlParameter($"@p{parameters.Count()}", custCode));
			}
			sql += @" )) M3
                    WHERE M1.ITEM_CODE = M2.ITEM_CODE AND M1.GUP_CODE = M2.GUP_CODE AND M1.CUST_CODE = M2.CUST_CODE
	                    AND M1.LOC_CODE = M3.LOC_CODE AND M1.DC_CODE = M3.DC_CODE 
	                    AND M1.ITEM_CODE = @p1 AND M1.DC_CODE = @p0 ";
			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += $" AND M1.GUP_CODE = @p{parameters.Count()} ";
				parameters.Add(new SqlParameter($"@p{parameters.Count()}", gupCode));
			}

			if (string.IsNullOrEmpty(gupCode))
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																		           FROM F190101 aa 
																		     INNER JOIN (SELECT * 
																		     				       FROM F192402 
																		     						  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
																		     		  WHERE aa.DC_CODE = M1.DC_CODE AND aa.GUP_CODE = M1.GUP_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account));
			}
			if (string.IsNullOrEmpty(custCode))
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																								FROM F190101 cc 
																					INNER JOIN (SELECT * 
																		    								  FROM F192402 
																		    								 WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE 
																		    					WHERE cc.DC_CODE = M1.DC_CODE AND cc.GUP_CODE = M1.GUP_CODE AND cc.CUST_CODE = M1.CUST_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account));
			}

			var result = SqlQuery<F1912StatusEx2>(sql, parameters.ToArray()).AsQueryable();
			return result.AsQueryable();
		}


		/// <summary>
		/// 取得儲位統計
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="warehouseId"></param>
		/// <returns></returns>
		public IQueryable<F1912StatisticReport> GetLocStatisticForLocControl(string dcCode, string gupCode, string custCode
				, string warehouseType, string warehouseId, string account)
		{
			string sql = @" 
 SELECT ROW_NUMBER() OVER(ORDER BY ROWTYPE,
						                DC_CODE,
						                GUP_CODE,
						                CUST_CODE,
						                WAREHOUSE_TYPE,
						                WAREHOUSE_TYPE_NAME)[ROW_NUM],
	                A.CUST_CODE,A.CUST_NAME,A.DC_CODE,A.DC_NAME,A.GUP_CODE,A.GUP_NAME,A.LOCCOUNT,A.WAREHOUSE_TYPE,A.WAREHOUSE_TYPE_NAME, 0 AS PERCENTAGE
								                FROM (  /* 1. 查詢條件:物流中心、業主全部、貨主全部、倉別全部。系統顯示資料格式第一列 */
										                /* 這裡取出的LOC_CODE有做DISTINCT, 所以數量可能會較查詢2,3,4的少 */
										                SELECT 
										
											                ROW_NUMBER()OVER(ORDER BY DC_CODE) RO ,

											                '1' ROWTYPE , S1.* FROM (
											                SELECT LOC.DC_CODE,
												                    ( SELECT DC_NAME FROM   F1901 A0  WHERE  A0.DC_CODE = LOC.DC_CODE )   AS DC_NAME,
												                    '' AS  GUP_CODE,
												                    convert(nvarchar,(SELECT COUNT(DISTINCT A1.GUP_CODE) FROM F1912 A1 
																                    WHERE A1.DC_CODE = LOC.DC_CODE 
																		                    AND (A1.NOW_CUST_CODE =@p1 OR A1.CUST_CODE =@p1 OR (@p1='0' AND A1.CUST_CODE <> @p1))
														                    )) AS GUP_NAME,
												                    '' AS   CUST_CODE,
												                    convert(nvarchar,(SELECT COUNT(DISTINCT A2.CUST_CODE)  FROM F1912 A2 
																                    WHERE  A2.DC_CODE = LOC.DC_CODE 
																		                    AND (A2.NOW_CUST_CODE =@p1 OR A2.CUST_CODE =@p1 OR (@p1='0' AND A2.CUST_CODE <> @p1) )
														                    )) AS CUST_NAME,
												                    '' WAREHOUSE_TYPE,
												                    convert(nvarchar,COUNT(DISTINCT WH1.WAREHOUSE_ID)) AS WAREHOUSE_TYPE_NAME,
												                    COUNT(DISTINCT LOC_CODE)  AS LOCCOUNT
											                FROM F1912 LOC
											                JOIN 
												                    (SELECT WH.DC_CODE, WH.WAREHOUSE_ID, WH.WAREHOUSE_TYPE,WHT.TYPE_NAME
													                FROM F1980 WH 
													                JOIN F198001 WHT ON WH.WAREHOUSE_TYPE = WHT.TYPE_ID
											                ) WH1 ON LOC.WAREHOUSE_ID = WH1.WAREHOUSE_ID  AND LOC.DC_CODE = WH1.DC_CODE	
											                WHERE   LOC.DC_CODE = @p0 
													                AND (LOC.NOW_CUST_CODE =@p1 OR LOC.CUST_CODE =@p1 OR (@p1='0' AND LOC.CUST_CODE <> @p1))
											                GROUP BY LOC.DC_CODE
										                ) S1
									                    UNION ALL
										                /* 2. 查詢條件:物流中心、業主=業主編號、貨主全部、倉別全部。系統顯示資料格式第二列 */
										                SELECT 

											                ROW_NUMBER()OVER(ORDER BY DC_CODE,GUP_CODE) RO ,

											                '2' ROWTYPE , S2.* FROM (										
											                SELECT LOC.DC_CODE ,
												                    (SELECT DC_NAME FROM F1901 A0 WHERE A0.DC_CODE = LOC.DC_CODE)  AS DC_NAME,
												                    LOC.GUP_CODE AS GUP_CODE,
												                    convert(nvarchar, (SELECT A1.GUP_NAME  FROM F1929 A1  WHERE A1.GUP_CODE = LOC.GUP_CODE))   AS GUP_NAME,
												                    '' AS CUST_CODE,
												                    convert(nvarchar,
													                    (SELECT COUNT (DISTINCT A2.CUST_CODE)   
													                    FROM F1912 A2  WHERE A2.DC_CODE = LOC.DC_CODE AND A2.GUP_CODE = LOC.GUP_CODE 
																			                AND (A2.NOW_CUST_CODE =@p1 OR A2.CUST_CODE =@p1 OR (@p1='0' AND A2.CUST_CODE <> @p1))
													                    ))
													                    AS CUST_NAME
													                    ,  '' WAREHOUSE_TYPE,
												                    convert(nvarchar,COUNT (DISTINCT WH1.WAREHOUSE_ID))  AS WAREHOUSE_TYPE_NAME,
												                    COUNT(DISTINCT LOC_CODE)  AS LOCCOUNT
											                FROM F1912 LOC        
											                JOIN ( SELECT WH.DC_CODE, WH.WAREHOUSE_ID,  WH.WAREHOUSE_TYPE,  WHT.TYPE_NAME
														                FROM F1980 WH 
														                JOIN F198001 WHT ON WH.WAREHOUSE_TYPE = WHT.TYPE_ID
												                    ) WH1 ON LOC.WAREHOUSE_ID = WH1.WAREHOUSE_ID AND LOC.DC_CODE = WH1.DC_CODE
											                WHERE LOC.DC_CODE = @p0 
													                AND (LOC.NOW_CUST_CODE =@p1  OR LOC.CUST_CODE = @p1 OR (@p1='0' AND LOC.CUST_CODE <> @p1))
											                GROUP BY LOC.DC_CODE, LOC.GUP_CODE 
										                ) S2
									                    UNION ALL
										                /* 3. 查詢條件:物流中心、業主=業主編號、貨主=貨主編號、倉別全部。系統顯示資料格式第三列 */
										                SELECT 
										
											                ROW_NUMBER()OVER(ORDER BY DC_CODE,GUP_CODE,CUST_CODE)RO ,

											                '3' ROWTYPE , S3.* FROM (										
											                SELECT LOC.DC_CODE,
											                    (SELECT DC_NAME FROM F1901 A0  WHERE A0.DC_CODE = LOC.DC_CODE)  AS DC_NAME,
											                    LOC.GUP_CODE AS GUP_CODE,
											                    convert(nvarchar, (SELECT A1.GUP_NAME FROM F1929 A1 WHERE A1.GUP_CODE = LOC.GUP_CODE))  AS GUP_NAME,
											                    LOC.CUST_CODE AS CUST_CODE,
											                    convert(nvarchar, (SELECT A2.CUST_NAME  FROM F1909 A2 WHERE A2.GUP_CODE = LOC.GUP_CODE AND A2.CUST_CODE = LOC.CUST_CODE))  AS CUST_NAME,
											                    '' WAREHOUSE_TYPE,
											                    convert(nvarchar,COUNT (DISTINCT WH1.WAREHOUSE_ID))  AS WAREHOUSE_TYPE_NAME,
											                    COUNT (LOC_CODE) AS LOCCOUNT
											                FROM F1912 LOC
											                JOIN  
												                    (SELECT WH.DC_CODE, WH.WAREHOUSE_ID, WH.WAREHOUSE_TYPE,  WHT.TYPE_NAME
													                    FROM F1980 WH 
													                    JOIN F198001 WHT ON WH.WAREHOUSE_TYPE = WHT.TYPE_ID
												                    ) WH1 ON  LOC.WAREHOUSE_ID = WH1.WAREHOUSE_ID  AND LOC.DC_CODE = WH1.DC_CODE
											                    WHERE  LOC.DC_CODE = @p0
													                AND (LOC.NOW_CUST_CODE =@p1 OR LOC.CUST_CODE =@p1 OR (@p1='0' AND LOC.CUST_CODE <> @p1))
											                GROUP BY LOC.DC_CODE, LOC.GUP_CODE, LOC.CUST_CODE
										                ) S3
									                    UNION ALL
										                /* 4. 查詢條件:物流中心、業主=業主編號、貨主=貨主編號、倉別=良品倉。系統顯示資料格式第四列 */
										                SELECT 
											                ROW_NUMBER()OVER(ORDER BY DC_CODE,GUP_CODE,CUST_CODE,WAREHOUSE_TYPE,WAREHOUSE_TYPE_NAME) RO ,
											                '4' ROWTYPE , S4.* FROM (
											                SELECT LOC.DC_CODE,
                                                                (SELECT DC_NAME FROM F1901 A0 WHERE A0.DC_CODE = LOC.DC_CODE)  AS DC_NAME,
                                                                LOC.GUP_CODE AS GUP_CODE,
                                                                convert(nvarchar,(SELECT A1.GUP_NAME FROM F1929 A1 WHERE A1.GUP_CODE = LOC.GUP_CODE)) AS GUP_NAME,
                                                                LOC.CUST_CODE AS CUST_CODE,
                                                                convert(nvarchar,(SELECT A2.CUST_NAME FROM F1909 A2 WHERE A2.GUP_CODE = LOC.GUP_CODE AND A2.CUST_CODE = LOC.CUST_CODE)) AS CUST_NAME,           
                                                                WH1.WAREHOUSE_TYPE,
                                                                convert(nvarchar,WH1.WAREHOUSE_NAME) AS WAREHOUSE_TYPE_NAME,
                                                                COUNT (LOC_CODE) AS LOCCOUNT
											                FROM F1912 LOC 
												                    JOIN
												                (SELECT WH.DC_CODE, WH.WAREHOUSE_NAME , WH.WAREHOUSE_ID, WH.WAREHOUSE_TYPE,  WHT.TYPE_NAME
												                    FROM F1980 WH 
												                    JOIN  F198001 WHT
												                    ON   WH.WAREHOUSE_TYPE = WHT.TYPE_ID ";
			if (!string.IsNullOrEmpty(warehouseType)) sql += $@" AND WHT.TYPE_ID = @p2 ";
			sql += @"
													                    AND (WH.WAREHOUSE_ID = @p3 OR @p3 = '0')
												                ) WH1 ON LOC.WAREHOUSE_ID = WH1.WAREHOUSE_ID  AND LOC.DC_CODE = WH1.DC_CODE
											                WHERE  LOC.DC_CODE = @p0
												   
												                    AND (LOC.NOW_CUST_CODE =@p1 OR LOC.CUST_CODE = @p1 OR (@p1='0' AND LOC.CUST_CODE <> @p1))
											                GROUP BY LOC.DC_CODE,
													                LOC.GUP_CODE,
													                LOC.CUST_CODE,
													                WH1.WAREHOUSE_TYPE,
													                WH1.WAREHOUSE_NAME,
													                WH1.TYPE_NAME
										                ) S4
							                ) A
                ";
			var param = new[] {
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", custCode),
								new SqlParameter("@p2", warehouseType),
								new SqlParameter("@p3", warehouseId),
						};
			var result = SqlQuery<F1912StatisticReport>(sql, param).AsQueryable();
			return result;
		}

		
		/// <summary>
		/// 傳回儲位屬性維護清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="areaCode"></param>
		/// <param name="locCodeS"></param>
		/// <param name="locCodeE"></param>
		/// <returns></returns>
		public IQueryable<F1912WithF1980> GetLocListForWarehouse(string dcCode, string warehouseId, List<string> gupCodeList
				, List<string> custCodeList, string warehouseType, string areaCode, string channel, string locCodeS, string locCodeE, string account)
		{
			var parameters = new List<object>
								{
										dcCode
								};
			var sql = @"
           SELECT A.DC_CODE,
              H.DC_NAME,
              A.WAREHOUSE_ID,
              A.LOC_CODE,
              B.WAREHOUSE_NAME,
              B.WAREHOUSE_TYPE,
              G.TYPE_NAME WAREHOUSE_TYPE_NAME,
              B.TMPR_TYPE,
              (CASE B.TMPR_TYPE
                  WHEN '01' THEN N'常溫'
                  WHEN '02' THEN N'低溫'
                  WHEN '03' THEN N'冷凍'
               END)
                 TEMP_TYPE_NAME,
              B.CAL_STOCK,
              B.CAL_FEE,
              A.FLOOR,
              A.AREA_CODE,
              F.AREA_NAME,
              A.CHANNEL,
              A.PLAIN,
              A.LOC_LEVEL,
              A.LOC_TYPE,
              A.LOC_TYPE_ID,
              E.LOC_TYPE_NAME,
              E.HANDY,
              A.NOW_STATUS_ID,
              A.PRE_STATUS_ID,
              A.UCC_CODE,
              A.GUP_CODE,
              (CASE A.GUP_CODE WHEN '0' THEN N'共用' ELSE D.GUP_NAME END ) GUP_NAME,
              A.CUST_CODE,
              (CASE A.CUST_CODE WHEN '0' THEN N'共用' ELSE C.CUST_NAME END ) CUST_NAME,
              A.HOR_DISTANCE,
              A.RENT_BEGIN_DATE,
              A.RENT_END_DATE
         FROM F1912 A
              LEFT JOIN F1980 B
                 ON B.DC_CODE = A.DC_CODE AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
              LEFT JOIN F1909 C
                 ON A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE
              LEFT JOIN F1929 D ON A.GUP_CODE = D.GUP_CODE
              LEFT JOIN F1942 E ON A.LOC_TYPE_ID = E.LOC_TYPE_ID
              LEFT JOIN F1919 F
                 ON     A.AREA_CODE = F.AREA_CODE
                    AND A.DC_CODE = F.DC_CODE
                    AND A.WAREHOUSE_ID = F.WAREHOUSE_ID
              LEFT JOIN F198001 G ON B.WAREHOUSE_TYPE = G.TYPE_ID
              LEFT JOIN F1901 H ON A.DC_CODE = H.DC_CODE
        					 WHERE A.DC_CODE=@p0 ";

			if (gupCodeList.Any())
			{
				sql += parameters.CombineSqlInParameters("AND A.GUP_CODE", gupCodeList);
			}
			else if(custCodeList.Any())
			{
				sql += parameters.CombineSqlInParameters("AND A.CUST_CODE", gupCodeList);
			}

			if (!string.IsNullOrEmpty(warehouseId))
			{
				sql += "  AND A.WAREHOUSE_ID = @p" + parameters.Count;
				parameters.Add(warehouseId);
			}

			if (!string.IsNullOrEmpty(warehouseType))
			{
				sql += "  AND B.WAREHOUSE_TYPE = @p" + parameters.Count;
				parameters.Add(warehouseType);
			}
			if (!string.IsNullOrEmpty(areaCode))
			{
				sql += "  AND A.AREA_CODE = @p" + parameters.Count;
				parameters.Add(areaCode);
			}
			if (!string.IsNullOrEmpty(channel))
			{
				sql += "  AND A.CHANNEL = @p" + parameters.Count;
				parameters.Add(channel);
			}
			if (!string.IsNullOrEmpty(locCodeS))
			{
				sql += "  AND A.LOC_CODE >= @p" + parameters.Count;
				parameters.Add(locCodeS);
			}
			if (!string.IsNullOrEmpty(locCodeE))
			{
				sql += "  AND A.LOC_CODE <= @p" + parameters.Count;
				parameters.Add(locCodeE);
			}

			sql += "  ORDER BY A.DC_CODE,A.WAREHOUSE_ID,A.LOC_CODE ";
			var result = SqlQuery<F1912WithF1980>(sql, parameters.ToArray());
			return result;
		}


		/// <summary>
		/// 取得儲位區間資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="floor"></param>
		/// <param name="minChannel"></param>
		/// <param name="maxChannel"></param>
		/// <param name="minPlain"></param>
		/// <param name="maxPlain"></param>
		/// <param name="minLocLevel"></param>
		/// <param name="maxLocLevel"></param>
		/// <param name="minLocType"></param>
		/// <param name="maxLocType"></param>
		/// <returns></returns>
		public IQueryable<F1912> GetF1912DatasByRange(string dcCode, string gupCode, string custCode, string floor, string minChannel, string maxChannel, string minPlain, string maxPlain, string minLocLevel, string maxLocLevel, string minLocType, string maxLocType)
		{
			var channelList = new List<string>();
			int miChannel = ChangeEnglishToNumber(minChannel);
			int maChannel = ChangeEnglishToNumber(maxChannel);
			int miPlain = ChangeEnglishToNumber(minPlain);
			int maPlain = ChangeEnglishToNumber(maxPlain);
			for (var i = miChannel; i <= maChannel; i++)
				channelList.Add(i.ToString().PadLeft(2, '0'));
			var plainList = new List<string>();
			for (var i = miPlain; i <= maPlain; i++)
				plainList.Add(i.ToString().PadLeft(2, '0'));
			var locLevelList = new List<string>();
			for (var i = int.Parse(minLocLevel); i <= int.Parse(maxLocLevel); i++)
				locLevelList.Add(i.ToString().PadLeft(2, '0'));
			var locTypeList = new List<string>();
			for (var i = int.Parse(minLocType); i <= int.Parse(maxLocType); i++)
				locTypeList.Add(i.ToString().PadLeft(2, '0'));

			var f1912Data = this.Filter(o =>
																									o.DC_CODE == dcCode &&
																									o.GUP_CODE == gupCode &&
																									o.CUST_CODE == custCode &&
																									o.FLOOR == floor &&
																									channelList.Contains(o.CHANNEL) &&
																									plainList.Contains(o.PLAIN) &&
																									locLevelList.Contains(o.LOC_LEVEL) &&
																									locTypeList.Contains(o.LOC_TYPE));
			return f1912Data;
		}

		private int ChangeEnglishToNumber(string english)
		{
			var upString = english.ToUpper();
			int result = 0;
			for (int i = 0; i < english.Length; i++)
			{
				int letter = (int)upString[i];
				int a = 1;

				for (int x = 1; x < (english.Length - i); x++)
					a *= 36;
				if (i + 1 != english.Length)
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55) * a;
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48) * a;
				}
				else
				{
					if (letter >= 65 && letter <= 90)
						result += (letter - 55);
					else if (letter >= 48 && letter <= 57)
						result += (letter - 48);
				}
			}

			return result;
		}

		/// <summary>
		/// 儲位列印
		/// </summary>
		/// <param name="locStart"></param>
		/// <param name="locEnd"></param>
		/// <param name="gupCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseCode"></param>
		/// <param name="listItem"></param>
		/// <returns></returns>
		public IQueryable<F1912DataLoc> GetPrintDataLoc(string locStart, string locEnd, string gupCode, string dcCode,
						string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			List<SqlParameter> param = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode)
						};
			string strSQL = string.Empty;
			string condition = string.Empty;

			if (string.IsNullOrWhiteSpace(listItem))
			{
				// 此區代表為搜尋儲位，不須過濾GupCode、CustCode，且需要Join F1913，再根據是否查空儲位篩選

				if (!string.IsNullOrEmpty(warehouseCode) && !warehouseCode.Equals("0"))
				{
					condition += @"AND a.WAREHOUSE_ID = @p3 ";
					param.Add(new SqlParameter("@p3", warehouseCode));
				}

				if (!string.IsNullOrEmpty(locStart) && !string.IsNullOrEmpty(locEnd))
				{
					condition += @"AND a.LOC_CODE >= @p4 AND a.LOC_CODE <= @p5 ";
					param.Add(new SqlParameter("@p4", locStart));
					param.Add(new SqlParameter("@p5", locEnd));
				}

				strSQL = $@"SELECT z.LOC, z.BARCODE FROM (SELECT TOP (100) PERCENT a.LOC_CODE LOC, a.LOC_CODE AS BARCODE , SUM(b.QTY) QTY 
        					FROM F1912 a 
                            LEFT JOIN F1913 b 
        					ON a.LOC_CODE = b.LOC_CODE 
        					AND a.DC_CODE = b.DC_CODE 
                            WHERE a.DC_CODE = @p0 {condition} 
                            GROUP BY a.LOC_CODE
                            ) z {(printEmpty ? "WHERE z.QTY = 0" : string.Empty)} ORDER BY z.LOC ";
			}
			else
			{
				// 此區代表為搜尋品號，必須過濾GupCode、CustCode
				condition += @"AND b.ITEM_CODE in (" + listItem + ") ";

				if (!string.IsNullOrEmpty(gupCode))
				{
					condition += @"AND a.GUP_CODE = @p1 ";
					param.Add(new SqlParameter("@p1", gupCode));
				}

				if (!string.IsNullOrEmpty(custCode) && !custCode.Equals("0"))
				{
					condition += @"AND a.CUST_CODE = @p2 ";
					param.Add(new SqlParameter("@p2", custCode));
				}

				if (!string.IsNullOrEmpty(warehouseCode) && !warehouseCode.Equals("0"))
				{
					condition += @"AND a.WAREHOUSE_ID = @p3 ";
					param.Add(new SqlParameter("@p3", warehouseCode));
				}

				strSQL = $@"SELECT DISTINCT a.LOC_CODE LOC, a.LOC_CODE AS BARCODE 
        								 FROM F1912 a 
                            JOIN F1913 b 
        					ON a.LOC_CODE = b.LOC_CODE 
        					AND a.DC_CODE = b.DC_CODE 
                            WHERE a.DC_CODE = @p0 
                            {condition}
                            ORDER BY a.LOC_CODE";
			}

			var results = SqlQuery<F1912DataLoc>(strSQL, param.ToArray()).AsQueryable();
			return results;
		}

		public IQueryable<F1912> GetUserCanUseLocCode(string dcCode, string locCode, string userId)
		{
			var parameters = new List<SqlParameter>
								{
										new SqlParameter("@p0",  dcCode),
										new SqlParameter("@p1",  locCode),
										new SqlParameter("@p2",  userId),
								};
			var sql = " SELECT A.* " +
													"   FROM F1912 A " +
													"  WHERE A.DC_CODE = @p0 " +
													"    AND A.LOC_CODE = @p1 " +
													"    AND EXISTS ( " + //使用者可處理儲位
													"                 SELECT 1 " +
													"                   FROM F1924 D " +
													"                  INNER JOIN F192403 E ON E.EMP_ID = D.EMP_ID " +
													"                  INNER JOIN F1963 F ON F.WORK_ID = E.WORK_ID AND F.ISDELETED = '0' " +
													"                  INNER JOIN F196301 G ON G.WORK_ID = F.WORK_ID " +
													"                  WHERE G.DC_CODE   =  A.DC_CODE " +
													"										 AND G.LOC_CODE  = A.LOC_CODE " +
													"                    AND E.EMP_ID=@p2 ) ";
			return SqlQuery<F1912>(sql, parameters.ToArray());
		}

		/// <summary>
		/// 取得 從現有 商品庫存中 + 尚未揀貨 + 調撥尚未下架 的總和材積
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="locCodes"></param>
		public IQueryable<LocVolumnData> GetUsedVolumnByLocCodes(string dcCode, IEnumerable<string> locCodes)
		{
			var parameters = new List<SqlParameter> { new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar } };


			int paramStartIndex = parameters.Count;
      var inSql = parameters.CombineSqlInParameters("AND A.LOC_CODE", locCodes, ref paramStartIndex, SqlDbType.VarChar);

      var sql = $@"
SELECT 
  A.DC_CODE, 
  A.LOC_CODE, 
  SUM(
    CASE WHEN B.VOLUMN IS NULL THEN 0 ELSE B.VOLUMN END
  ) VOLUMN 
FROM 
  F1912 A 
  LEFT JOIN (
    SELECT 
      A.DC_CODE, 
      A.LOC_CODE, 
      (
        B.PACK_LENGTH * B.PACK_WIDTH * B.PACK_HIGHT * SUM(A.QTY)
      ) VOLUMN 
    FROM 
      (
        select 
          DC_CODE, 
          GUP_CODE, 
          CUST_CODE, 
          LOC_CODE, 
          ITEM_CODE, 
          SUM(QTY) QTY 
        from 
          F1913 
        GROUP BY 
          DC_CODE, 
          GUP_CODE, 
          CUST_CODE, 
          LOC_CODE, 
          ITEM_CODE 
        UNION ALL 
        SELECT 
          A.DC_CODE, 
          A.GUP_CODE, 
          A.CUST_CODE, 
          A.PICK_LOC LOC_CODE, 
          A.ITEM_CODE, 
          SUM(A.B_PICK_QTY - A.A_PICK_QTY) QTY 
        FROM 
          F051202 A 
          JOIN VW_VirtualStock B ON B.DC_CODE = A.DC_CODE 
          AND B.GUP_CODE = A.GUP_CODE 
          AND B.CUST_CODE = A.CUST_CODE 
          AND B.ORDER_NO = A.PICK_ORD_NO 
          AND B.ORDER_SEQ = A.PICK_ORD_SEQ 
          AND B.STATUS = '0' 
          JOIN F050801 C ON C.DC_CODE = A.DC_CODE 
          AND C.GUP_CODE = A.GUP_CODE 
          AND C.CUST_CODE = A.CUST_CODE 
          AND C.WMS_ORD_NO = A.WMS_ORD_NO 
        WHERE 
          (
            C.STATUS = '0' 
            OR B.STATUS = '0'
          ) 
          AND A.PICK_STATUS = '0' --出貨單還在準備揀貨的狀態，且尚未取消訂單或者尚未揀貨
        GROUP BY 
          A.DC_CODE, 
          A.GUP_CODE, 
          A.CUST_CODE, 
          A.PICK_LOC, 
          A.ITEM_CODE 
        UNION ALL 
        SELECT 
          B.DC_CODE, 
          B.GUP_CODE, 
          B.CUST_CODE, 
          B.SRC_LOC_CODE LOC_CODE, 
          B.ITEM_CODE, 
          SUM(B.SRC_QTY - B.A_SRC_QTY) QTY 
        FROM 
          F151001 A 
          JOIN F151002 B ON B.DC_CODE = A.DC_CODE 
          AND B.GUP_CODE = A.GUP_CODE 
          AND B.CUST_CODE = A.CUST_CODE 
          AND B.ALLOCATION_NO = A.ALLOCATION_NO 
        WHERE 
          A.STATUS IN('0', '1') -- 剛建立調撥單或列印，還沒實際下架
        GROUP BY 
          B.DC_CODE, 
          B.GUP_CODE, 
          B.CUST_CODE, 
          B.SRC_LOC_CODE, 
          B.ITEM_CODE
      ) A 
      JOIN F1905 B ON B.GUP_CODE = A.GUP_CODE 
      AND B.ITEM_CODE = A.ITEM_CODE 
      AND B.CUST_CODE = A.CUST_CODE 
    GROUP BY 
      A.DC_CODE, 
      A.LOC_CODE, 
      B.PACK_LENGTH, 
      B.PACK_WIDTH, 
      B.PACK_HIGHT
  ) B ON B.DC_CODE = A.DC_CODE 
  AND B.LOC_CODE = A.LOC_CODE 
WHERE 
  A.DC_CODE = @p0

                        {inSql}
                    GROUP BY A.DC_CODE,A.LOC_CODE ";
			return SqlQuery<LocVolumnData>(sql, parameters.ToArray());
		}

    public IQueryable<string> GetUpdateLocVolumnByCalvolumnTime(string dcCode,int CountMaxLocVolumn)
    {
      var para = new List<SqlParameter> { new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar } };
      var sql = $@"
SELECT TOP {CountMaxLocVolumn} 
	A.LOC_CODE 
FROM F1912 A 
INNER JOIN 
(
	SELECT DC_CODE,LOC_CODE,ISNULL(UPD_DATE,CRT_DATE) AS LAST_TIME FROM F1913 GROUP BY DC_CODE,LOC_CODE,ISNULL(UPD_DATE,CRT_DATE)
) B
  ON A.DC_CODE = B.DC_CODE AND A.LOC_CODE = B.LOC_CODE 
INNER JOIN F198001 C
  ON SUBSTRING(A.WAREHOUSE_ID,1,1) = C.TYPE_ID AND C.CALVOLUMN ='1'
WHERE 
  A.DC_CODE = @p0
  AND B.LAST_TIME > CASE WHEN A.LAST_CALVOLUMN_TIME IS NULL THEN '0001-01-01' ELSE A.LAST_CALVOLUMN_TIME END
GROUP BY
	A.LOC_CODE,A.LAST_CALVOLUMN_TIME
ORDER BY A.LAST_CALVOLUMN_TIME";

      return SqlQuery<string>(sql, para.ToArray());
    }

    /// <summary>
    /// 更新儲位容積
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="locCode"></param>
    /// <param name="UsedVolumn"></param>
    public void UpdateUsedVolumn(string dcCode, string locCode, decimal UsedVolumn)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", locCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", UsedVolumn) { SqlDbType = SqlDbType.Decimal},
        new SqlParameter("@p3", Current.Staff) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p4", Current.StaffName) {SqlDbType = SqlDbType.NVarChar},
        new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };
      var sqlUsedVolumn = "";
      if (UsedVolumn == 0)
        sqlUsedVolumn = ",NOW_CUST_CODE = '0'";

      var sql = $"UPDATE F1912 SET USED_VOLUMN = @p2, LAST_CALVOLUMN_TIME = @p5, UPD_DATE = @p5, UPD_STAFF = @p3, UPD_NAME = @p4 {sqlUsedVolumn} WHERE DC_CODE = @p0 AND LOC_CODE = @p1";

      ExecuteSqlCommand(sql, para.ToArray());
    }

    /// <summary>
    /// 取得混品儲位
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="warehouseType"></param>
    /// <param name="aTypeCode"></param>
    /// <param name="excludeNotMixItem">是否不包含不可混品商品</param>
    /// <param name="excludeLocCodes"></param>
    /// <param name="targetWarehouseId"></param>
    /// <param name="locCode">比較距離的參考儲位</param>
    /// <returns></returns>
    public IQueryable<F1912> GetMixItemLoc(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string aTypeCode, bool excludeNotMixItem, string targetWarehouseId = "", decimal? volume = null, string wareHouseTmpr = null, bool isForIn = true)
		{
			var parameters = new List<object> {
										gupCode,
										custCode,
										dcCode,
										aTypeCode,
										itemCode,
										custCode
								};

			var sqlFilter = string.Empty;

			if (isForIn)
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'03'";

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);


			}

			//同時判斷 TMPR_TYPE
			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				sqlFilter += parameters.CombineSqlInParameters(" And C.TMPR_TYPE ", wareHouseTmpr.Split(','));
			}


			if (volume.HasValue)
			{
				sqlFilter += string.Format(" And (ISNULL(A.USEFUL_VOLUMN,0) - ISNULL(A.USED_VOLUMN,0))>=@p{0}", parameters.Count);
				parameters.Add(volume);
			}

			//if (excludeLocCodes != null && excludeLocCodes.Any())
			//{
			//	int paramStartIndex = parameters.Count;
			//	sqlFilter += " And " + parameters.CombineSqlNotInParameters("A.LOC_CODE", excludeLocCodes, ref paramStartIndex);
			//}


			var sqlJoin = string.Empty;
			//不包含不可混品的商品
			if (excludeNotMixItem)
			{
				sqlFilter += @"
            			           And Not Exists (Select E.LOC_CODE  --過濾掉含有商品不可混品的儲位
            			                             From F1913 E, F1903 F
            			                            Where E.GUP_CODE=F.GUP_CODE
            			                              And E.CUST_CODE=F.CUST_CODE
            			                              And E.ITEM_CODE=F.ITEM_CODE
            			                              And F.LOC_MIX_ITEM<>'1'
            			                              And E.GUP_CODE=B.GUP_CODE
            			                              And E.CUST_CODE=B.CUST_CODE
            			                              And E.DC_CODE=B.DC_CODE
            			                              And E.LOC_CODE=B.LOC_CODE)";
			}


			//已排除凍結儲位
			string sql = @"
            			Select S.*
            			  From (Select Distinct A.*
            			          From F1912 A, F1913 B, F1980 C, F1919 D,F198001 E
            			         Where A.LOC_CODE=B.LOC_CODE And A.DC_CODE=B.DC_CODE
            			           And A.WAREHOUSE_ID=C.WAREHOUSE_ID And A.DC_CODE=C.DC_CODE
            			           And A.WAREHOUSE_ID=D.WAREHOUSE_ID And A.DC_CODE=D.DC_CODE And A.AREA_CODE=D.AREA_CODE
                              And C.WAREHOUSE_TYPE = E.TYPE_ID
            			           And A.NOW_STATUS_ID<>'04'
            			           And B.GUP_CODE=@p0
            			           And B.CUST_CODE=@p1
            			           And A.DC_CODE=@p2
            			           And D.ATYPE_CODE=@p3
            			           And Not Exists (Select E.LOC_CODE --過濾掉含有指定的ITEM_CODE的儲位
            			                             From F1913 E
            			                            Where E.GUP_CODE=B.GUP_CODE
            			                              And E.CUST_CODE=B.CUST_CODE
            			                              And E.DC_CODE=B.DC_CODE
            			                              And E.LOC_CODE=B.LOC_CODE
            			                              And E.ITEM_CODE=@p4)
                              AND (E.LOC_MUSTSAME_NOWCUSTCODE = '0' OR A.NOW_CUST_CODE ='0' OR A.NOW_CUST_CODE = @p5)"
											+ sqlFilter + @"
            			         ) S
                                Order By (ISNULL(S.USEFUL_VOLUMN,0) - ISNULL(S.USED_VOLUMN,0)) Desc, S.HOR_DISTANCE
            			 ";

			return SqlQuery<F1912>(sql, parameters.ToArray());
		}

    /// <summary>
    /// 取得混品儲位
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="warehouseType"></param>
    /// <param name="aTypeCode"></param>
    /// <param name="excludeNotMixItem"></param>
    /// <param name="targetWarehouseId"></param>
    /// <param name="volume"></param>
    /// <param name="wareHouseTmpr"></param>
    /// <param name="isForIn"></param>
    /// <param name="topRecord"></param>
    /// <returns></returns>
    public IQueryable<MixLocPriorityInfo> GetNewMixItemLoc(string dcCode,string gupCode, string custCode, string itemCode, string warehouseType, string aTypeCode, bool excludeNotMixItem, string targetWarehouseId = "", decimal? volume = null, string wareHouseTmpr = null, List<string> excludeLocCodes = null, bool isForIn = true, long? topRecord = null)
    {
      var parameters = new List<object> {
										dcCode,
										aTypeCode,
										itemCode,
										custCode,
                    gupCode,
										custCode
                };

			var sqlFilter = string.Empty;

			if (isForIn)
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'03'";

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);


			}

			//同時判斷 TMPR_TYPE
			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				sqlFilter += parameters.CombineSqlInParameters(" And C.TMPR_TYPE ", wareHouseTmpr.Split(','));
			}


			if (volume.HasValue)
			{
				sqlFilter += string.Format(" And (ISNULL(A.USEFUL_VOLUMN,0) - ISNULL(A.USED_VOLUMN,0))>=@p{0}", parameters.Count);
				parameters.Add(volume);
			}

			if (excludeLocCodes != null && excludeLocCodes.Any())
			{
				int paramStartIndex = parameters.Count;
				sqlFilter += " And " + parameters.CombineSqlNotInParameters("A.LOC_CODE", excludeLocCodes, ref paramStartIndex);
			}


			var sqlJoin = string.Empty;
			//不包含不可混品的商品
			if (excludeNotMixItem)
			{
				sqlFilter += @"
            			           And Not Exists (Select DISTINCT E.LOC_CODE  --過濾掉含有商品不可混品的儲位
            			                             From F1913 E, F1903 F
            			                            Where E.GUP_CODE=F.GUP_CODE
            			                              And E.CUST_CODE=F.CUST_CODE
            			                              And E.ITEM_CODE=F.ITEM_CODE
            			                              And F.LOC_MIX_ITEM<>'1'
            			                              And E.DC_CODE=A.DC_CODE
            			                              And E.LOC_CODE=A.LOC_CODE)";
			}

			var topSql = string.Empty;
			if (topRecord.HasValue)
				topSql += " TOP (" + topRecord.Value + ") ";


			//已排除凍結儲位
			string sql = $@"
            		Select  {topSql} A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.LOC_CODE, A.WAREHOUSE_ID, A.HOR_DISTANCE, A.FLOOR,A .USEFUL_VOLUMN, A.USED_VOLUMN,
                   D.ATYPE_CODE, C.WAREHOUSE_TYPE, C.TMPR_TYPE,A.AREA_CODE
            			          From F1912 A,  F1980 C, F1919 D,F198001 E
            			         Where  A.WAREHOUSE_ID=C.WAREHOUSE_ID And A.DC_CODE=C.DC_CODE
            			           And A.WAREHOUSE_ID=D.WAREHOUSE_ID And A.DC_CODE=D.DC_CODE And A.AREA_CODE=D.AREA_CODE
                             And C.WAREHOUSE_TYPE = E.TYPE_ID
            			           And A.NOW_STATUS_ID<>'04'
            			           And A.DC_CODE=@p0
            			           And D.ATYPE_CODE=@p1
            			           And Not Exists (Select DISTINCT E.LOC_CODE --過濾掉含有指定的ITEM_CODE的儲位
            			                             From F1913 E
            			                            Where E.DC_CODE=A.DC_CODE
            			                              And E.LOC_CODE=A.LOC_CODE
            			                              And E.ITEM_CODE=@p2)
                              AND (E.LOC_MUSTSAME_NOWCUSTCODE = '0' OR A.NOW_CUST_CODE ='0' OR A.NOW_CUST_CODE = @p3)
                              AND A.GUP_CODE IN(@p4,'0')
                              AND A.CUST_CODE IN(@p5,'0') {sqlFilter}
                              Order By (ISNULL(A.USEFUL_VOLUMN,0) - ISNULL(A.USED_VOLUMN,0)) Desc, A.HOR_DISTANCE
            			 ";

			return SqlQuery<MixLocPriorityInfo>(sql, parameters.ToArray());
		}



		public F1912 GetFirstLoc(string dcCode, string gupCode, string custCode, string warehouseType, string aTypeCode = "", string targetWarehouseId = "", bool isForIn = true, bool isShareGup = false, bool isShareCust = false)
		{
			var parameters = new List<object> {
										dcCode
								};

			var sqlFilter = "";

			if (isForIn)
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'03'";

			// 儲區
			if (!string.IsNullOrEmpty(aTypeCode))
			{
				sqlFilter += string.Format(" And C.ATYPE_CODE=@p{0} ", parameters.Count);
				parameters.Add(aTypeCode);
			}


			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And B.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);
			}
			else
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}

			if (isShareGup)
			{
				sqlFilter += string.Format(" AND A.GUP_CODE = '0'");
			}
			else
			{
				sqlFilter += string.Format(" AND A.GUP_CODE = @p{0}", parameters.Count);
				parameters.Add(gupCode);
			}

			if (isShareCust)
			{
				sqlFilter += string.Format(" AND A.CUST_CODE = '0'");
			}
			else
			{
				sqlFilter += string.Format(" AND A.CUST_CODE = @p{0}", parameters.Count);
				parameters.Add(custCode);
			}

			sqlFilter += string.Format(" And (A.NOW_CUST_CODE = @p{0} OR A.NOW_CUST_CODE ='0')", parameters.Count);
			parameters.Add(custCode);

			string sql = @"
        			SELECT TOP(1)ROW_NUMBER()OVER(Order By E.HOR_DISTANCE , E.GUP_CODE DESC , E.CUST_CODE DESC) ROWNUM,*
        			  FROM (Select A.*
        			          From F1912 A, F1980 B, F1919 C
        			         Where A.WAREHOUSE_ID=B.WAREHOUSE_ID And A.DC_CODE=B.DC_CODE
        			           And A.WAREHOUSE_ID=C.WAREHOUSE_ID And A.DC_CODE=C.DC_CODE And A.AREA_CODE=C.AREA_CODE
        			           And A.NOW_STATUS_ID<>'04'
        			           And A.DC_CODE=@p0  " + sqlFilter + @"
        			         ) E
        			 ORDER BY 1 ASC
        		";

			return SqlQuery<F1912>(sql, parameters.ToArray()).FirstOrDefault();
		}
		/// <summary>
		/// 更新儲位可用容積(cm)
		/// </summary>
		/// <param name="locTypeId"></param>
		/// <param name="usefulVolumn"></param>
		public void UpdateUsefulVolumn(string locTypeId, decimal usefulVolumn)
		{
			var sql = @"UPDATE F1912
        					SET USEFUL_VOLUMN = @p0
        						,UPD_DATE = @p1  
        						,UPD_STAFF = @p2  
        						,UPD_NAME = @p3     
                               WHERE LOC_TYPE_ID = @p4";
			var parameters = new object[]
			{
										usefulVolumn,
                    DateTime.Now,
                    Current.Staff,
										Current.StaffName,
										locTypeId
			};

			ExecuteSqlCommand(sql, parameters);
		}
		/// <summary>
		/// 儲位列印
		/// </summary>
		/// <param name="locStart"></param>
		/// <param name="locEnd"></param>
		/// <param name="gupCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseCode"></param>
		/// <param name="listItem"></param>
		/// <returns></returns>
		public IQueryable<F1912DataLocByLocType> GetPrintDataLocByNewReport(string locStart, string locEnd, string gupCode, string dcCode,
						string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			string gupCodeCondition = (!string.IsNullOrEmpty(gupCode)) ? @"AND A.GUP_CODE = @p1 " : string.Empty;
			string custCodeCondition = (!string.IsNullOrEmpty(custCode) && !custCode.Equals("0")) ? @"AND A.CUST_CODE = @p2 " : string.Empty;
			string warehouseCondition = (!string.IsNullOrEmpty(warehouseCode) && !warehouseCode.Equals("0")) ? @"AND A.WAREHOUSE_ID = @p3 " : string.Empty;
			string locCodeCondition = (!string.IsNullOrEmpty(locStart) && !string.IsNullOrEmpty(locEnd)) ? @"AND A.LOC_CODE >= @p4 AND A.LOC_CODE <= @p5 " : string.Empty;
			string itemListCondition = (!string.IsNullOrEmpty(listItem)) ? @"AND B.ITEM_CODE in (" + listItem + ") " : string.Empty;
			string printEmptyCondition = printEmpty ? @" AND A.USED_VOLUMN = 0 " : string.Empty;
			string strSQL = string.Empty;
			strSQL = @"
                    SELECT DISTINCT
                            A.LOC_CODE LOC,
                            A.LOC_CODE AS BARCODE,
                            A.AREA_CODE,
                            C.AREA_NAME,
                            A.FLOOR + '-' + A.CHANNEL + '-' + A.PLAIN + '-' + A.LOC_LEVEL + '-' + A.LOC_TYPE AS LOC_CODE_TYPE,
                            D.WAREHOUSE_NAME
                       FROM F1912 A LEFT JOIN F1913 B  ON A.LOC_CODE = B.LOC_CODE 
                                    LEFT JOIN F1919 C  ON A.AREA_CODE = C.AREA_CODE AND A.DC_CODE = C.DC_CODE
        			             LEFT JOIN F1980 D  ON A.WAREHOUSE_ID = D.WAREHOUSE_ID AND A.DC_CODE = D.DC_CODE AND C.WAREHOUSE_ID = D.WAREHOUSE_ID AND C.DC_CODE = D.DC_CODE
        			             WHERE A.DC_CODE = @p0
            ";
			strSQL += gupCodeCondition;
			strSQL += custCodeCondition;
			strSQL += warehouseCondition;
			strSQL += locCodeCondition;
			strSQL += itemListCondition;
			strSQL += printEmptyCondition;
			strSQL += " ORDER BY A.LOC_CODE";

			var param = new[] {
										new SqlParameter("@p0", dcCode),
										new SqlParameter("@p1", gupCode),
										new SqlParameter("@p2", custCode),
										new SqlParameter("@p3", warehouseCode),
										new SqlParameter("@p4", locStart),
										new SqlParameter("@p5", locEnd)
								};
			var results = SqlQuery<F1912DataLocByLocType>(strSQL, param).AsQueryable();
			return results;
		}

		public IQueryable<P710101DetailData> GetLocDetailData(string dcCode, List<string> locCodes)
		{
			//LOC_CODE CHANNEL PLAIN LOC_LEVEL   WAREHOUSE_ID STATUS  IsEdit AREA_CODE
			//10A010101   0A  01  01  G01 1   0   A02
			//10A010102   0A  01  01  G01 1   0   A02
			//10A010103   0A  01  01  G01 1   0   A02
			//10A010104   0A  01  01  G01 1   0   A02
			//10A010105   0A  01  01  G01 1   0   A02
			var sql = @" 
            SELECT A.LOC_CODE,
							   A.CHANNEL,
							   A.PLAIN,
							   A.LOC_LEVEL,
							   A.WAREHOUSE_ID,
							   '1' AS STATUS,
							   0 AS IsEdit,
							   A.AREA_CODE
						  FROM F1912 A
						 WHERE     EXISTS
									  (SELECT A.LOC_CODE
										 FROM F1913 B
										WHERE     (A.CUST_CODE = B.CUST_CODE OR A.CUST_CODE = '0')
											  AND A.LOC_CODE = B.LOC_CODE)
							   AND A.DC_CODE = @p0 AND {0}
						UNION ALL
						SELECT A.LOC_CODE,
							   A.CHANNEL,
							   A.PLAIN,
							   A.LOC_LEVEL,
							   A.WAREHOUSE_ID,
							   '0' AS STATUS,
							   1 AS IsEdit,
							   A.AREA_CODE
						  FROM F1912 A
						 WHERE     NOT EXISTS
									  (SELECT A.LOC_CODE
										 FROM F1913 B
										WHERE     (A.CUST_CODE = B.CUST_CODE OR A.CUST_CODE = '0')
											  AND A.LOC_CODE = B.LOC_CODE)
							   AND DC_CODE = @p0 AND {0}";
			var param0 = new List<SqlParameter>() { new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode } };

			var sql2 = new StringBuilder("  A.LOC_CODE IN(");
			locCodes.ForEach(x =>
			{
				sql2.Append($"@p{param0.Count},");
				param0.Add(new SqlParameter($"@p{param0.Count}", SqlDbType.VarChar) { Value = x });
			});
			sql2.Remove(sql2.Length - 1, 1);
			sql2.Append(")");
			var result = SqlQuery<P710101DetailData>(string.Format(sql, sql2), param0.ToArray());

			//var param = new List<object> { dcCode };
			//sql = string.Format(sql, param.CombineSqlInParameters(" A.LOC_CODE ", locCodes));
			//sw.Restart();
			//var result = SqlQuery<P710101DetailData>(sql, param.ToArray());
			//sw.Stop();
			return result;
		}

		/// <summary>
		/// 取得空儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType">倉別代號</param>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區，C:補貨區</param>
		/// <param name="excludeLocCodes">排除的儲位編號</param>
		/// <returns></returns>
		public IQueryable<LocPriorityInfo> GetEmptyLocPriorityInfo(string dcCode, string gupCode, string custCode, string warehouseType, string aTypeCode, string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			var parameters = new List<SqlParameter> {
								 new SqlParameter($"@p0", SqlDbType.VarChar) { Value = dcCode }
								 };

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(new SqlParameter($"@p{parameters.Count}", SqlDbType.VarChar) { Value = targetWarehouseId });
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(new SqlParameter($"@p{parameters.Count}", SqlDbType.VarChar) { Value = warehouseType });
			}

			if (!string.IsNullOrEmpty(gupCode))
			{
				sqlFilter += string.Format(" And B.GUP_CODE=@p{0} ", parameters.Count);
				parameters.Add(new SqlParameter($"@p{parameters.Count}", SqlDbType.VarChar) { Value = gupCode });
			}
			else
				sqlFilter += " And B.GUP_CODE='0' ";

			if (!string.IsNullOrEmpty(custCode))
			{
				sqlFilter += string.Format(" And B.CUST_CODE=@p{0} ", parameters.Count);
				parameters.Add(new SqlParameter($"@p{parameters.Count}", SqlDbType.VarChar) { Value = custCode });
			}
			else
				sqlFilter += " And B.CUST_CODE='0' ";

			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				sqlFilter += $"  AND C.TMPR_TYPE IN ( '{string.Join("','", wareHouseTmpr.Split(',')) }' )";
			}

			if (!string.IsNullOrEmpty(aTypeCode))
			{
				sqlFilter += @" AND D.ATYPE_CODE = @p" + parameters.Count;
				parameters.Add(new SqlParameter($"@p{parameters.Count}", SqlDbType.VarChar) { Value = aTypeCode });
			}

			//已排除凍結儲位
			string sql = @"
            Select B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.LOC_CODE, B.WAREHOUSE_ID, B.HOR_DISTANCE, B.FLOOR, B.USEFUL_VOLUMN, B.USED_VOLUMN,
                   D.ATYPE_CODE, E.HANDY, C.WAREHOUSE_TYPE, C.TMPR_TYPE,B.AREA_CODE
                    --, ROW_NUMBER()OVER(ORDER BY B.DC_CODE,B.LOC_CODE) ROWNUM
              From F1912 B, F1980 C, F1919 D, F1942 E
             Where B.WAREHOUSE_ID=C.WAREHOUSE_ID And B.DC_CODE=C.DC_CODE
               And B.WAREHOUSE_ID=D.WAREHOUSE_ID And B.DC_CODE=D.DC_CODE And B.AREA_CODE=D.AREA_CODE
               And B.LOC_TYPE_ID=E.LOC_TYPE_ID
               And (B.NOW_STATUS_ID='01' Or B.NOW_STATUS_ID='03')
               And B.NOW_CUST_CODE='0' --0代表空儲位
               And B.DC_CODE=@p0 " + sqlFilter;

			return SqlQuery<LocPriorityInfo>(sql, parameters.ToArray());
		}

		/// <summary>
		/// 取得空儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType">倉別代號</param>
		/// <param name="aTypeCode">儲區型態代號，A:一般揀貨區，B:黃金揀貨區，C:補貨區</param>
		/// <param name="excludeLocCodes">排除的儲位編號</param>
		/// <returns></returns>
		public IQueryable<LocPriorityInfo> GetEmptyLocPriorityInfo(string dcCode, string gupCode, string custCode, string warehouseType, string aTypeCode, string targetWarehouseId = "", string wareHouseTmpr = "", List<string> excludeLocCodes=null, long? topRecord = null)
		{
			var parameters = new List<object> {
								 dcCode
								 };

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);
			}

			if (!string.IsNullOrEmpty(gupCode))
			{
				sqlFilter += string.Format(" And B.GUP_CODE=@p{0} ", parameters.Count);
				parameters.Add(gupCode);
			}
			else
				sqlFilter += " And B.GUP_CODE='0' ";

			if (!string.IsNullOrEmpty(custCode))
			{
				sqlFilter += string.Format(" And B.CUST_CODE=@p{0} ", parameters.Count);
				parameters.Add(custCode);
			}
			else
				sqlFilter += " And B.CUST_CODE='0' ";

			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				sqlFilter += $"  AND C.TMPR_TYPE IN ( '{string.Join("','", wareHouseTmpr.Split(',')) }' )";
			}

			if (!string.IsNullOrEmpty(aTypeCode))
			{
				sqlFilter += @" AND D.ATYPE_CODE = @p" + parameters.Count;
				parameters.Add(aTypeCode);
			}
			if (excludeLocCodes != null && excludeLocCodes.Any())
			{
				int paramStartIndex = parameters.Count;
				sqlFilter += " AND " + parameters.CombineSqlNotInParameters("B.LOC_CODE", excludeLocCodes, ref paramStartIndex);
			}

			var topSql = string.Empty;
			if (topRecord.HasValue)
				topSql = " TOP (" + topRecord.Value + ")";

			//已排除凍結儲位
			string sql = $@"
            Select {topSql} B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.LOC_CODE, B.WAREHOUSE_ID, B.HOR_DISTANCE, B.FLOOR, B.USEFUL_VOLUMN, B.USED_VOLUMN,
                   D.ATYPE_CODE, E.HANDY, C.WAREHOUSE_TYPE, C.TMPR_TYPE,B.AREA_CODE
                    --, ROW_NUMBER()OVER(ORDER BY B.DC_CODE,B.LOC_CODE) ROWNUM
              From F1912 B, F1980 C, F1919 D, F1942 E
             Where B.WAREHOUSE_ID=C.WAREHOUSE_ID And B.DC_CODE=C.DC_CODE
               And B.WAREHOUSE_ID=D.WAREHOUSE_ID And B.DC_CODE=D.DC_CODE And B.AREA_CODE=D.AREA_CODE
               And B.LOC_TYPE_ID=E.LOC_TYPE_ID
               And (B.NOW_STATUS_ID='01' Or B.NOW_STATUS_ID='03')
               And B.NOW_CUST_CODE='0' --0代表空儲位
               And B.DC_CODE=@p0 " + sqlFilter;

			return SqlQuery<LocPriorityInfo>(sql, parameters.ToArray());
		}

		public IQueryable<F1912> GetNearestLoc(string dcCode, string gupCode, string custCode, string warehouseType, string aTypeCode, List<string> excludeLocCodes, string wareHouseTmpr, string targetWarehouseId = "", bool isForIn = true)
		{
			var parameters = new List<object> {
										dcCode,
										aTypeCode,
								};

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);
			}
			else
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}

			if (!string.IsNullOrEmpty(gupCode))
			{
				sqlFilter += string.Format(" And A.GUP_CODE=@p{0} ", parameters.Count);
				parameters.Add(gupCode);
			}
			else
				sqlFilter += " And A.GUP_CODE='0' ";

			if (!string.IsNullOrEmpty(custCode))
			{
				sqlFilter += string.Format(" And A.CUST_CODE=@p{0} ", parameters.Count);
				parameters.Add(custCode);
			}
			else
				sqlFilter += " And A.CUST_CODE='0' ";

			if (isForIn)
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'03'";

			//if (excludeLocCodes != null && excludeLocCodes.Any())
			//{
			//    int paramStartIndex = parameters.Count;
			//    sqlFilter += "AND " + parameters.CombineSqlNotInParameters("A.LOC_CODE", excludeLocCodes, ref paramStartIndex);
			//}

			if (!string.IsNullOrWhiteSpace(wareHouseTmpr))
			{
				sqlFilter += parameters.CombineSqlInParameters(" AND C.TMPR_TYPE ", wareHouseTmpr.Split(','));
			}
			string sql = $@"
        			Select  A.*
        			  From F1912 A, F1980 C, F1919 D
        			 Where A.WAREHOUSE_ID=C.WAREHOUSE_ID And A.DC_CODE=C.DC_CODE
        			   And A.WAREHOUSE_ID=D.WAREHOUSE_ID And A.DC_CODE=D.DC_CODE And A.AREA_CODE=D.AREA_CODE
        			   And A.NOW_CUST_CODE='0' --0代表空儲位
        			   And A.NOW_STATUS_ID<>'04'
        			   And A.DC_CODE=@p0
        			   And D.ATYPE_CODE=@p1 " + sqlFilter + @"
        			 Order By  A.HOR_DISTANCE
        		";

			return SqlQuery<F1912>(sql, parameters.ToArray()).AsQueryable();
		}

		public IQueryable<NearestLocPriorityInfo> GetNewNearestLoc(string dcCode, string gupCode, string custCode, string warehouseType, string aTypeCode, List<string> excludeLocCodes, string wareHouseTmpr, string targetWarehouseId = "", long? topRecord = null, bool isForIn = true)
		{
			var parameters = new List<object> {
										dcCode,
										aTypeCode,
								};

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (string.IsNullOrEmpty(targetWarehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);
			}
			else
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}

			if (!string.IsNullOrEmpty(gupCode))
			{
				sqlFilter += string.Format(" And A.GUP_CODE=@p{0} ", parameters.Count);
				parameters.Add(gupCode);
			}
			else
				sqlFilter += " And A.GUP_CODE='0' ";

			if (!string.IsNullOrEmpty(custCode))
			{
				sqlFilter += string.Format(" And A.CUST_CODE=@p{0} ", parameters.Count);
				parameters.Add(custCode);
			}
			else
				sqlFilter += " And A.CUST_CODE='0' ";

			if (isForIn)
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
        			   And A.NOW_STATUS_ID<>'03'";

			if (excludeLocCodes != null && excludeLocCodes.Any())
			{
				int paramStartIndex = parameters.Count;
				sqlFilter += "AND " + parameters.CombineSqlNotInParameters("A.LOC_CODE", excludeLocCodes, ref paramStartIndex);
			}

			if (!string.IsNullOrWhiteSpace(wareHouseTmpr))
			{
				sqlFilter += parameters.CombineSqlInParameters(" AND C.TMPR_TYPE ", wareHouseTmpr.Split(','));
			}
			var topStr = string.Empty;
			if (topRecord.HasValue)
				topStr = "TOP(" + topRecord.Value + ")";
			string sql = $@"
        			Select  {topStr } A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.LOC_CODE, A.WAREHOUSE_ID, A.HOR_DISTANCE, A.FLOOR,A .USEFUL_VOLUMN, A.USED_VOLUMN,A.AREA_CODE
        			  From F1912 A, F1980 C, F1919 D
        			 Where A.WAREHOUSE_ID=C.WAREHOUSE_ID And A.DC_CODE=C.DC_CODE
        			   And A.WAREHOUSE_ID=D.WAREHOUSE_ID And A.DC_CODE=D.DC_CODE And A.AREA_CODE=D.AREA_CODE
        			   And A.NOW_CUST_CODE='0' --0代表空儲位
        			   And A.NOW_STATUS_ID<>'04'
        			   And A.DC_CODE=@p0
        			   And D.ATYPE_CODE=@p1 " + sqlFilter + @"
        			 Order By  A.HOR_DISTANCE
        		";

			return SqlQuery<NearestLocPriorityInfo>(sql, parameters.ToArray()).AsQueryable();
		}

		public IQueryable<P08130101MoveLoc> GetP08130101MoveLocs(string dcCode, string locCode)
		{
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.LOC_CODE, A.DC_CODE) ROWNUM,A.LOC_CODE,A.WAREHOUSE_ID,B.WAREHOUSE_NAME,A.NOW_STATUS_ID,C.LOC_STATUS_NAME
        		FROM F1912 A
        		JOIN F1980 B
        		ON B.DC_CODE = A.DC_CODE
        		AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
        		JOIN F1943 C
        		ON C.LOC_STATUS_ID = A.NOW_STATUS_ID 
                    WHERE A.DC_CODE = @p0
                      AND A.LOC_CODE = @p1 ";
			var parms = new object[] { dcCode, locCode };
			return SqlQuery<P08130101MoveLoc>(sql, parms);
		}

		public IQueryable<F1912> GetDatas(string dcCode, string warehouseType, string gupCode = "0", string custCode = "0")
    {
      var parameter = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", warehouseType) { SqlDbType = SqlDbType.VarChar },
      };

      //dcCode, gupCode, custCode, warehouseType };
      string sql = @"SELECT A.* 
											FROM F1912 A 
											INNER JOIN F1980 B 
													ON A.DC_CODE = B.DC_CODE 
												AND A.WAREHOUSE_ID = B.WAREHOUSE_ID 
												AND A.DC_CODE = @p0 
												AND A.GUP_CODE = @p1 
												AND A.CUST_CODE = @p2 
												AND B.WAREHOUSE_TYPE = @p3
											ORDER BY A.DC_CODE,A.LOC_CODE ";
      return SqlQuery<F1912>(sql, parameter.ToArray());
    }

    public void DeleteLocByLocCode(string dcCode, string gupCode, string custCode, string warehouseId, List<string> locCodes)
		{
			var splitLocCode = SplitList(locCodes);
			foreach (var item in splitLocCode)
			{
				var param = new List<SqlParameter> {
										new SqlParameter("@p0",SqlDbType.VarChar){Value= dcCode },
										new SqlParameter("@p1",SqlDbType.VarChar){Value= gupCode },
										new SqlParameter("@p2",SqlDbType.VarChar){Value= custCode },
										new SqlParameter("@p3",SqlDbType.VarChar){Value= warehouseId }
								};
				var sbSQLIn = new StringBuilder();
				item.ForEach(x =>
				{
					sbSQLIn.Append($"@p{param.Count},");
					param.Add(new SqlParameter($"@p{param.Count}", SqlDbType.VarChar) { Value = x });
				});
				sbSQLIn.Remove(sbSQLIn.Length - 1, 1);
				string sql = @"
				DELETE F1912 WHERE DC_CODE = @p0 AND 
								   GUP_CODE = @p1 AND 
								   CUST_CODE = @p2 AND
								   WAREHOUSE_ID = @p3 AND
                                   LOC_CODE IN ({0})";
				ExecuteSqlCommand(string.Format(sql, sbSQLIn.ToString()), param.ToArray());
				//sql += param.CombineSqlInParameters(" AND LOC_CODE ", item);
				//ExecuteSqlCommand(sql, param.ToArray());
			}
		}

		public IQueryable<DcWmsNoLocTypeItem> GetDcWmsNoLocTypeItems(string dcCode, string gupCode, string custCode)
		{
			var sql = @" SELECT ROW_NUMBER()OVER(order by A.LOC_TYPE_ID)[ROWNUM],A.*,A.TOTALLOCCOUNT - A.USEDLOCCOUNT AS UNUSEDLOCCOUNT
										 FROM (
										 SELECT DISTINCT A.LOC_TYPE_ID,COUNT(DISTINCT C.LOC_CODE) AS USEDLOCCOUNT,COUNT(*) AS TOTALLOCCOUNT
										   FROM F1912 A
										  INNER JOIN F1980 B
										     ON B.DC_CODE = A.DC_CODE 
									  	  AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
										   LEFT JOIN F1913 C
										     ON C.DC_CODE = A.DC_CODE
										    AND C.GUP_CODE = A.GUP_CODE
										    AND C.CUST_CODE = A.CUST_CODE
										    AND C.LOC_CODE = A.LOC_CODE
										  WHERE B.WAREHOUSE_TYPE ='G'
										    AND A.DC_CODE = @p0
										    AND A.GUP_CODE =@p1
										    AND A.CUST_CODE =@p2
										  GROUP BY A.LOC_TYPE_ID ) A";
			var param = new object[] { dcCode, gupCode, custCode };
			return SqlQuery<DcWmsNoLocTypeItem>(sql, param);
		}

		public IQueryable<string> GEtF1912LocCodeList(string dcCode, string BeginLocCode, string EndLocCode)
		{
			var sql = @"
SELECT distinct SUBSTRING([LOC_CODE],1,5)
FROM F1912
WHERE dc_code= @p0
AND (SUBSTRING([LOC_CODE],1,5) BETWEEN @p1 AND @p2
	OR SUBSTRING([LOC_CODE],1,5) BETWEEN @p3 AND @p4)
ORDER BY SUBSTRING([LOC_CODE],1,5);";

			var param = new object[] { dcCode, BeginLocCode, EndLocCode, EndLocCode, BeginLocCode };

			//var param = new List<SqlParameter>()
			//{
			//    new SqlParameter("@p0",System.Data.SqlDbType.VarChar){Value=dcCode},
			//    new SqlParameter("@p1",System.Data.SqlDbType.VarChar){Value=BeginLocCode},
			//    new SqlParameter("@p2",System.Data.SqlDbType.VarChar){Value=EndLocCode}
			//};
			return SqlQuery<string>(sql, param);

		}

		public IQueryable<F1912> GetF1912DataSQL(string dcCode, List<string> locCodes)
		{
			List<F1912> result = new List<F1912>();
			string sql = @"SELECT * FROM F1912 WHERE DC_CODE=@p0 AND LOC_CODE IN({0})";

			var splitLocCodes = SplitList(locCodes);
			foreach (var item in splitLocCodes)
			{
				StringBuilder sbSQLIN = new StringBuilder();
				var para = new List<SqlParameter>();
				para.Add(new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode });

				foreach (var locitem in item)
				{
					sbSQLIN.Append($"@p{para.Count},");
					para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = locitem });
				}
				sbSQLIN.Remove(sbSQLIN.Length - 1, 1);
				result.AddRange(SqlQuery<F1912>(string.Format(sql, sbSQLIN.ToString()), para.ToArray()));

			}

			return result.AsQueryable();
		}

		public List<F1912> GetLocCodeDataSQL(string dcCode, string gupCode, string custCode, List<string> locCodes)
		{
			List<F1912> result = new List<F1912>();
			string sql = @"SELECT * FROM F1912 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND LOC_CODE IN({0})";

			var splitLocCodes = SplitList(locCodes, 1000);
			foreach (var item in splitLocCodes)
			{
				StringBuilder sbSQLIN = new StringBuilder();
				var para = new List<SqlParameter>();
				para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = dcCode });
				para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = gupCode });
				para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = custCode });

				foreach (var locitem in item)
				{
					sbSQLIN.Append($"@p{para.Count},");
					para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = locitem });
				}
				sbSQLIN.Remove(sbSQLIN.Length - 1, 1);
				result.AddRange(SqlQuery<F1912>(string.Format(sql, sbSQLIN.ToString()), para.ToArray()));

			}

			return result;
		}


		private List<List<string>> SplitList(List<string> source, int chunkSize = 2000)
		{
			var result = new List<List<string>>();
			var sourceCount = source.Count;
			for (var i = 0; i < sourceCount; i += chunkSize)
			{
				result.Add(source.GetRange(i, Math.Min(chunkSize, sourceCount - i)));
			}
			return result;
		}

		public IQueryable<NameValueList> GetDcWarehouseChannelList(string dcCode, string warehouseId, string areaCode)
		{
			var parms = new List<SqlParameter>
					{
						new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
						new SqlParameter("@p1",warehouseId){ SqlDbType = SqlDbType.VarChar},

					};
			var sql = @" SELECT DISTINCT CHANNEL Name,CHANNEL Value FROM F1912 
                          WHERE DC_CODE =  @p0
                            AND WAREHOUSE_ID = @p1 ";
			if (!string.IsNullOrWhiteSpace(areaCode))
			{
				sql += " AND AREA_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, areaCode) { SqlDbType = SqlDbType.VarChar });
			}
			sql += " ORDER BY CHANNEL ";
			return SqlQuery<NameValueList>(sql, parms.ToArray());
		}

		public F1912 GetFirstLocByWarehouseId(string dcCode,string warehouseId)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",warehouseId){ SqlDbType = SqlDbType.VarChar},

			};
			var sql = @" SELECT  TOP 1 * 
                     FROM F1912 
                    WHERE DC_CODE = @p0
                      AND WAREHOUSE_ID = @p1";
			return SqlQuery<F1912>(sql, parms.ToArray()).FirstOrDefault();
		}

    public IQueryable<F1912> GetDatasByLocCodes(string dcCode, string gupCode, string custCode, List<string> locCodes)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar},
      };
      var sql = @"SELECT * FROM F1912 WHERE DC_CODE=@p0 AND GUP_CODE IN(@p1,'0') AND CUST_CODE IN(@p2,'0')";
      sql += para.CombineSqlInParameters(" AND LOC_CODE", locCodes, SqlDbType.VarChar);
      return SqlQuery<F1912>(sql, para.ToArray());

      #region 原始Linq語法
      /*
      return _db.F1912s
                  .Where(x => x.DC_CODE == dcCode)
                  .Where(x => x.GUP_CODE == gupCode || x.GUP_CODE == "0")
                  .Where(x => x.CUST_CODE == custCode || x.CUST_CODE == "0")
                  .Where(x => locCodes.Contains((x.LOC_CODE)))
                  .Select(x => x);
      */
      #endregion
    }

    public IQueryable<F1912> GetDatasByLocCodes(string dcCode, List<string> locCodes)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar},
      };

      var sql = @"SELECT * FROM F1912 WHERE DC_CODE=@p0";
      sql += para.CombineSqlInParameters(" AND LOC_CODE", locCodes, SqlDbType.VarChar);
      return SqlQuery<F1912>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      return _db.F1912s
          .Where(x => x.DC_CODE == dcCode)
          .Where(x => locCodes.Contains((x.LOC_CODE)))
          .Select(x => x);
      */
      #endregion

    }

    public IQueryable<GetStockRes> GetInventoryInfo(string custNo, string dcNo, string gupCode, List<string> itemCode, string mkNo,
    string sn, string whNo, string begLoc, string endLoc, string begPalletNo, string endPalletNo, DateTime? begEnterDate,
    DateTime? endEnterDate, DateTime? begValidDate, DateTime? endValidDate, string BUNDLE_SERIALLOC, string serialItemCode)
    {
      var f1913Filter = "";
      var f1912Filter = "";

      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcNo },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custNo },
        new SqlParameter("@p3", SqlDbType.DateTime2) { Value = DateTime.Today },
      };


      if (itemCode.Any())
        f1913Filter += para.CombineSqlInParameters(" AND B.ITEM_CODE", itemCode, SqlDbType.VarChar);

      if (!string.IsNullOrWhiteSpace(mkNo))
      {
        f1913Filter += $" AND B.MAKE_NO=@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = mkNo });
      }

      if (!string.IsNullOrWhiteSpace(sn))
      {
        if (BUNDLE_SERIALLOC == "1")
        {
          f1913Filter += $" AND B.SERIAL_NO=@p{para.Count}";
          para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = sn });
        }
        else
        {
          f1913Filter += $" AND B.ITEM_CODE=@p{para.Count}";
          para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = serialItemCode });
        }
      }

      if (whNo != "ALL" && !string.IsNullOrWhiteSpace(whNo))
      {
        f1912Filter += $" AND A.WAREHOUSE_ID=@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = whNo });
      }

      if (!string.IsNullOrWhiteSpace(begLoc))
      {
        f1913Filter += $" AND B.LOC_CODE BETWEEN @p{para.Count} AND @p{para.Count + 1}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = begLoc });
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = endLoc });
      }

      if (!string.IsNullOrWhiteSpace(begPalletNo))
      {
        f1913Filter += $" AND B.PALLET_CTRL_NO BETWEEN @p{para.Count} AND @p{para.Count + 1}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar)  { Value = begPalletNo });
        para.Add(new SqlParameter($"@p{para.Count }", SqlDbType.VarChar) { Value = endPalletNo });
      }

      if (begEnterDate != null)
      {
        f1913Filter += $" AND (B.ENTER_DATE >= @p{para.Count} AND B.ENTER_DATE < @p{para.Count + 1})";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.DateTime2) { Value = begEnterDate });
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.DateTime2) { Value = endEnterDate.Value.AddDays(1) });
      }

      if (begValidDate != null)
      {
        f1913Filter += $" AND (B.VALID_DATE >= @p{para.Count} AND VALID_DATE < @p{para.Count + 1})";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.DateTime2) { Value = begValidDate });
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.DateTime2) { Value = endValidDate.Value.AddDays(1) });
      }

      var sql = $@"SELECT 
	A.DC_CODE AS DcNo,
	B.CUST_CODE AS CustNo,
	C.WAREHOUSE_NAME AS WhName,
	B.LOC_CODE AS Loc,
	B.ITEM_CODE AS ItemNo,
	B.VALID_DATE AS ValidDate,
	B.ENTER_DATE AS EnterDate,
	D1.ACC_UNIT_NAME AS Unit,
	B.MAKE_NO AS MkNo,
	B.SERIAL_NO AS Sn,
	B.QTY AS StockQty,
	B.BOX_CTRL_NO AS BoxNo,
	B.PALLET_CTRL_NO AS PalletNo,
	A1.LOC_STATUS_NAME AS LocStatus,
	DATEDIFF(DAY,@p3,B.VALID_DATE) AS DiffVDate,
	D.ITEM_NAME AS ItemName,
	D.ITEM_SIZE AS ItemSize,
	D.ITEM_COLOR AS ItemColor,
	D.ITEM_SPEC AS ItemSpec,
	(SELECT 
		SUM(E.B_PICK_QTY) 
	FROM F1511 E 
	WHERE E.DC_CODE=A.DC_CODE 
		AND E.GUP_CODE=B.GUP_CODE 
		AND E.CUST_CODE=B.CUST_CODE 
		AND E.LOC_CODE=B.LOC_CODE 
		AND E.ITEM_CODE=B.ITEM_CODE 
		AND E.VALID_DATE=B.VALID_DATE 
		AND E.ENTER_DATE=B.ENTER_DATE 
		AND E.MAKE_NO=B.MAKE_NO
    AND E.STATUS='0'
    AND E.B_PICK_QTY>0) AS BPickQty,
	D.EAN_CODE1 AS EANCode1,
	D.BUNDLE_SERIALNO AS BundleSerialNo
FROM F1912 A
INNER JOIN F1943 A1
	ON A.NOW_STATUS_ID=A1.LOC_STATUS_ID
INNER JOIN F1913 B
	ON A.DC_CODE=B.DC_CODE AND A.LOC_CODE =B.LOC_CODE 
INNER JOIN F1980 C
	ON A.DC_CODE=C.DC_CODE AND A.WAREHOUSE_ID=C.WAREHOUSE_ID
INNER JOIN F1903 D
	ON B.GUP_CODE=D.GUP_CODE AND B.CUST_CODE =D.CUST_CODE AND B.ITEM_CODE=D.ITEM_CODE
INNER JOIN F91000302 D1
	ON D1.ITEM_TYPE_ID ='001' AND D.ITEM_UNIT=D1.ACC_UNIT 
WHERE A.DC_CODE=@p0
AND B.GUP_CODE=@p1
AND B.CUST_CODE=@p2
AND B.QTY>0
{f1912Filter}
{f1913Filter}";

      return SqlQuery<GetStockRes>(sql, para.ToArray());
    }


  }

}
