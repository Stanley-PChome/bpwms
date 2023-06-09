
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051201Repository : RepositoryBase<F051201, Wms3plDbContext, F051201Repository>
	{
		public IQueryable<F051201Data> GetF051201Datas(string dcCode, string gupCode, string custCode, DateTime delvDate,
				string isPrinted, string ordType)
		{
			var parameters = new List<object>
						{
								dcCode,gupCode,custCode,delvDate.Date,ordType
						};
			var filterSql = string.Empty;
			if (!string.IsNullOrEmpty(isPrinted))
			{
				filterSql = "   AND ISNULL(A.ISPRINTED,'0') = @p" + parameters.Count;
				parameters.Add(isPrinted);
			}

			var sql = $@" 
					SELECT A.*,B.ITEMCOUNT,B.TOTALPICK_QTY,X.PICK_TYPE
						FROM(
						SELECT A.DC_CODE,
                               A.GUP_CODE,
                               A.CUST_CODE,
                               A.DELV_DATE,
                               A.PICK_TIME,
                               A.ORD_TYPE,
                               B.SOURCE_TYPE,
                               E.SOURCE_NAME,
                               COUNT(DISTINCT A.PICK_ORD_NO) PICKCOUNT,
                               COUNT(DISTINCT C.ORD_NO) ORDCOUNT,
                               ISNULL(F.ISPRINTED,'0') ISPRINTED
						FROM F051201 A
            JOIN F051202 G
            ON G.DC_CODE = A.DC_CODE
            AND G.GUP_CODE = A.GUP_CODE
            AND G.CUST_CODE = A.CUST_CODE
            AND G.PICK_ORD_NO = A.PICK_ORD_NO
						JOIN F050801 B
						ON B.DC_CODE = G.DC_CODE
						AND B.GUP_CODE = G.GUP_CODE
						AND B.CUST_CODE = G.CUST_CODE
						AND B.WMS_ORD_NO  = G.WMS_ORD_NO
						JOIN F05030101 C
						ON C.DC_CODE = B.DC_CODE
						AND C.GUP_CODE = B.GUP_CODE
						AND C.CUST_CODE = B.CUST_CODE
						AND C.WMS_ORD_NO  = B.WMS_ORD_NO
						JOIN F050301 D
						ON D.DC_CODE = C.DC_CODE
						AND D.GUP_CODE = C.GUP_CODE
						AND D.CUST_CODE = C.CUST_CODE
						AND D.ORD_NO = C.ORD_NO
						AND D.PROC_FLAG <>'9'
						LEFT JOIN F000902 E
						ON E.SOURCE_TYPE = B.SOURCE_TYPE	
						LEFT JOIN (
						SELECT DC_CODE,
                               GUP_CODE,
                               CUST_CODE,
                               DELV_DATE,
                               PICK_TIME,
                               ORD_TYPE,'1' ISPRINTED
						FROM F051201 
						WHERE (ORD_TYPE = 0 AND (ISPRINTED = '1' OR DEVICE_PRINT ='1')) OR (ORD_TYPE = 1 AND (ISPRINTED ='1' OR ISDEVICE='1'))
						GROUP BY DC_CODE,GUP_CODE,CUST_CODE,DELV_DATE,PICK_TIME,ORD_TYPE
						) F
						ON F.DC_CODE = A.DC_CODE
						AND F.GUP_CODE = A.GUP_CODE
						AND F.CUST_CODE = A.CUST_CODE
						AND F.DELV_DATE = A.DELV_DATE
						AND F.PICK_TIME = A.PICK_TIME
						AND F.ORD_TYPE = A.ORD_TYPE
						WHERE A.DC_CODE = @p0
						AND A.GUP_CODE= @p1
						AND A.CUST_CODE = @p2
						AND A.DELV_DATE = @p3
						AND A.ORD_TYPE = @p4
                        {filterSql}
						GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.ORD_TYPE,B.SOURCE_TYPE,E.SOURCE_NAME,F.ISPRINTED) A
						JOIN (
						SELECT A.DC_CODE,
                               A.GUP_CODE,
                               A.CUST_CODE,
                               A.DELV_DATE,
                               A.PICK_TIME,
                               A.ORD_TYPE,
                               COUNT(DISTINCT B.ITEM_CODE) ITEMCOUNT,
                               SUM(B.B_PICK_QTY) TOTALPICK_QTY
							FROM F051201 A
							JOIN F051202 B
								ON B.DC_CODE = A.DC_CODE
							 AND B.GUP_CODE = A.GUP_CODE
							 AND B.CUST_CODE = A.CUST_CODE
							 AND B.PICK_ORD_NO = A.PICK_ORD_NO
							 AND EXISTS
							 (SELECT 1
								 FROM F051201 C
                JOIN F051202 G
                ON G.DC_CODE = C.DC_CODE
                AND G.GUP_CODE = C.GUP_CODE
                AND G.CUST_CODE = C.CUST_CODE
                AND G.PICK_ORD_NO = C.PICK_ORD_NO
								JOIN F050801 D
								ON D.DC_CODE = G.DC_CODE
								AND D.GUP_CODE = G.GUP_CODE
								AND D.CUST_CODE = G.CUST_CODE
								AND D.WMS_ORD_NO  = G.WMS_ORD_NO
								JOIN F05030101 E
								ON E.DC_CODE = D.DC_CODE
								AND E.GUP_CODE = D.GUP_CODE
								AND E.CUST_CODE = D.CUST_CODE
								AND E.WMS_ORD_NO  = D.WMS_ORD_NO
								JOIN F050301 F
								ON F.DC_CODE = E.DC_CODE
								AND F.GUP_CODE = E.GUP_CODE
								AND F.CUST_CODE = E.CUST_CODE
								AND F.ORD_NO = E.ORD_NO
								AND F.PROC_FLAG <> '9'
								WHERE C.DC_CODE = A.DC_CODE
									AND C.GUP_CODE = A.GUP_CODE
									AND C.CUST_CODE = A.CUST_CODE
									AND C.PICK_ORD_NO = A.PICK_ORD_NO)
							GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.ORD_TYPE 
						) B
						ON B.DC_CODE = A.DC_CODE
						AND B.GUP_CODE = A.GUP_CODE
						AND B.CUST_CODE = A.CUST_CODE
						AND B.DELV_DATE = A.DELV_DATE
						AND B.PICK_TIME = A.PICK_TIME
						AND B.ORD_TYPE = A.ORD_TYPE
                        JOIN F0513 X
						on A.DELV_DATE = X.DELV_DATE 
						AND A.PICK_TIME = X.PICK_TIME 
						AND A.DC_CODE = X.DC_CODE 
						AND A.GUP_CODE = X.GUP_CODE 
						AND A.CUST_CODE = X.CUST_CODE 
						ORDER BY A.DELV_DATE,A.PICK_TIME ";

			var result = SqlQuery<F051201Data>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F051201SelectedData> GetF051201SelectedDatas(string dcCode, string gupCode, string custCode,
				string delvDate, string pickTime, string ordType, string isPrinted, string isDevicePrint = null)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", delvDate),
								new SqlParameter("@p4", pickTime),
								new SqlParameter("@p5", ordType),
								new SqlParameter("@p6",isPrinted)
						};
			var sql = @" SELECT A.PICK_ORD_NO,A.PICK_TIME 
					  FROM F051201 A 
					 WHERE A.DC_CODE = @p0 
					   AND A.GUP_CODE = @p1 
					   AND A.CUST_CODE = @p2 
					   AND A.DELV_DATE = @p3
					   AND A.PICK_TIME = @p4 
					   AND A.ORD_TYPE = @p5 
					   AND EXISTS (
					      SELECT 1 
					        FROM F051202 E 
					       INNER JOIN F050801 F 
					          ON F.DC_CODE = E.DC_CODE 
					         AND F.GUP_CODE = E.GUP_CODE 
					         AND F.CUST_CODE = E.CUST_CODE 
					         AND F.WMS_ORD_NO =E.WMS_ORD_NO 
					       INNER JOIN F05030101 G 
					          ON G.DC_CODE = F.DC_CODE 
					         AND G.GUP_CODE = F.GUP_CODE 
					         AND G.CUST_CODE = F.CUST_CODE 
					         AND G.WMS_ORD_NO = F.WMS_ORD_NO 
					       INNER JOIN F050301 H 
					          ON H.DC_CODE = G.DC_CODE 
					         AND H.GUP_CODE = G.GUP_CODE 
					         AND H.CUST_CODE = G.CUST_CODE 
					         AND H.ORD_NO = G.ORD_NO 
					        WHERE F.VIRTUAL_ITEM <>'1'  --排除虛擬商品出貨單
					          AND H.PROC_FLAG <>'9'   --排除取消的訂單
					         AND E.DC_CODE = A.DC_CODE 
					            AND E.GUP_CODE = A.GUP_CODE
					            AND E.CUST_CODE = A.CUST_CODE
					            AND E.PICK_ORD_NO = A.PICK_ORD_NO) 
					      AND (A.ISPRINTED = @p6 ";

			if (ordType == "0")
			{
				if (string.IsNullOrEmpty(isDevicePrint))
					sql += "     OR A.DEVICE_PRINT ='1') ";
				else
				{
					sql += "     )  AND A.DEVICE_PRINT ='" + isDevicePrint + "'";
				}

				if (isPrinted == "0")
				{
					sql += @" AND A.PICK_STATUS ='0' 
					          AND NOT EXISTS ( 
								    SELECT 1 
                      FROM F051202 B
                    WHERE B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
                      AND B.PICK_STATUS = '2'
					        )  ";
				}
			}
			else
				sql += "     ) ";

			var result = SqlQuery<F051201SelectedData>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(string dcCode, string gupCode, string custCode,
				DateTime delvDate, string pickTime, string pickOrdNo, bool showValidDate, string ordType = null)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime, ordType };
			var filterSql = string.Empty;
			if (!string.IsNullOrEmpty(pickOrdNo))
			{
				filterSql = paramList.CombineSqlInParameters(" AND A.PICK_ORD_NO", pickOrdNo.Split(','));
			}
			var validDateString = string.Empty;
			if (showValidDate)
				validDateString = " ,F.VALID_DATE ";

			var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.TAKE_DATE, 
                               A.TAKE_TIME,
                               A.PICK_LOC ASC) ROWNUM,
                               A.*
			            FROM(
			            SELECT O.GUP_NAME,
                               M.CUST_NAME,
                               A.DELV_DATE,
                               A.PICK_TIME,
                               A.PICK_ORD_NO,
                               B.WMS_ORD_NO,
                               C.TAKE_TIME,
                               CONVERT(varchar, D.TAKE_DATE,111) TAKE_DATE,
                               E.ALL_COMP,
                               H.WAREHOUSE_NAME,
                               G.FLOOR,
			            	   CASE H.TMPR_TYPE
			            			WHEN '01' THEN N'常溫'
			            			WHEN '02' THEN N'低溫'
			            			WHEN '03' THEN N'冷凍'
			            	   END TMPR_TYPE_NAME,
			            	   I.AREA_NAME,
			            	   CASE
			            			WHEN LEN (F.PICK_LOC) = 9
			            			THEN
			            						SUBSTRING (F.PICK_LOC, 1, 1)
			            					+ '-'
			            					+ SUBSTRING (F.PICK_LOC, 2, 2)
			            					+ '-'
			            					+ SUBSTRING (F.PICK_LOC, 4, 2)
			            					+ '-'
			            					+ SUBSTRING (F.PICK_LOC, 6, 2)
			            					+ '-'
			            					+ SUBSTRING (F.PICK_LOC, 8, 2)
			            			ELSE F.PICK_LOC END PICK_LOC,
			            		F.ITEM_CODE,
                                J.ITEM_NAME,
                                J.ITEM_SIZE,
                                J.ITEM_COLOR,
                                J.ITEM_SPEC,
                                K.ACC_UNIT_NAME,
                                F.SERIAL_NO,
                                L.CUST_ORD_NO,
                                L.CHANNEL,
                                N.NAME,
			            				CASE WHEN M.ISPICKSHOWCUSTNAME = '1' THEN L.CONSIGNEE ELSE NULL END
			            					ORDER_CUST_NAME{validDateString},SUM(F.B_PICK_QTY) B_PICK_QTY, L.MEMO, J.EAN_CODE1,M.SHORT_NAME 
			            FROM F051201 A
			            JOIN F051202 F
			            ON F.DC_CODE = A.DC_CODE
			            AND F.GUP_CODE = A.GUP_CODE
			            AND F.CUST_CODE = A.CUST_CODE
			            AND F.PICK_ORD_NO = A.PICK_ORD_NO
			            JOIN F050801 B
			            ON B.DC_CODE = F.DC_CODE
			            AND B.GUP_CODE = F.GUP_CODE
			            AND B.CUST_CODE = F.CUST_CODE
			            AND B.WMS_ORD_NO = F.WMS_ORD_NO
			            LEFT JOIN F700102 C
			            ON C.DC_CODE = B.DC_CODE
			            AND C.GUP_CODE = B.GUP_CODE
			            AND C.CUST_CODE = B.CUST_CODE
			            AND C.WMS_NO = B.WMS_ORD_NO
			            LEFT JOIN F700101 D
			            ON D.DC_CODE = C.DC_CODE
			            AND D.DISTR_CAR_NO = C.DISTR_CAR_NO 
			            LEFT JOIN F1947 E
			            ON E.DC_CODE = B.DC_CODE
			            AND E.ALL_ID = ISNULL(D.ALL_ID,B.ALL_ID)
			            JOIN F1912 G
			            ON G.DC_CODE = F.DC_CODE
			            AND G.LOC_CODE = F.PICK_LOC
			            JOIN F1980 H
			            ON H.DC_CODE = G.DC_CODE
			            AND H.WAREHOUSE_ID = G.WAREHOUSE_ID
			            LEFT JOIN F1919 I
			            ON I.DC_CODE = G.DC_CODE
			            AND I.WAREHOUSE_ID = G.WAREHOUSE_ID
			            AND I.AREA_CODE = G.AREA_CODE
			            LEFT JOIN F1903 J
			            ON J.GUP_CODE = F.GUP_CODE
			            AND J.ITEM_CODE = F.ITEM_CODE
                        AND J.CUST_CODE = F.CUST_CODE
			            LEFT JOIN F91000302 K
			            ON K.ACC_UNIT = J.ITEM_UNIT AND K.ITEM_TYPE_ID = '001'
			            JOIN (
			            	SELECT A.DC_CODE,
                                   A.GUP_CODE,
                                   A.CUST_CODE,
                                   A.WMS_ORD_NO,
                                   B.CHANNEL,
                                   B.CONSIGNEE,
			            	       STRING_AGG (B.CUST_ORD_NO, ',')
			            		   WITHIN GROUP (ORDER BY B.CUST_ORD_NO)
			            		   AS CUST_ORD_NO,
                                   B.MEMO
					    FROM F05030101 A
					    JOIN F050301 B
					    ON B.DC_CODE = A.DC_CODE
					    AND B.GUP_CODE = A.GUP_CODE
					    AND B.CUST_CODE= A.CUST_CODE
					    AND B.ORD_NO = A.ORD_NO
					    GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,B.CHANNEL,B.CONSIGNEE,B.MEMO
			            ) L
			            ON L.DC_CODE = B.DC_CODE
			            AND L.GUP_CODE = B.GUP_CODE
			            AND L.CUST_CODE = B.CUST_CODE
			            AND L.WMS_ORD_NO = B.WMS_ORD_NO
			            JOIN F1909 M
			            ON M.GUP_CODE = A.GUP_CODE
			            AND M.CUST_CODE = A.CUST_CODE
			            LEFT JOIN VW_F000904_LANG N
			            	ON N.VALUE = L.CHANNEL
			            AND N.TOPIC = 'F050101'
			            AND N.SUBTOPIC = 'CHANNEL'
			            AND N.LANG = '{Current.Lang}'
			            LEFT JOIN F1929 O
			            ON O.GUP_CODE = A.GUP_CODE
			            WHERE A.DC_CODE = @p0
			            AND A.GUP_CODE = @p1
			            AND A.CUST_CODE = @p2
			            AND A.DELV_DATE = @p3
			            AND A.PICK_TIME = @p4
                  AND A.DISP_SYSTEM ='0' --派發系統為WMS
                  AND A.PICK_TYPE IN('0','5') --單一揀貨/快速補揀單
                  AND A.PICK_TOOL = '1' --紙本揀貨單
			            AND A.ORD_TYPE = ISNULL ( @p5, A.ORD_TYPE)
			            AND B.VIRTUAL_ITEM <> '1'                        -- 排除虛擬商品出貨單
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
			            AND Q.WMS_ORD_NO = B.WMS_ORD_NO
			            AND R.PROC_FLAG = '9')
                        {filterSql}
			            GROUP BY O.GUP_NAME,M.CUST_NAME,A.DELV_DATE,A.PICK_TIME,A.PICK_ORD_NO,B.WMS_ORD_NO,C.TAKE_TIME,D.TAKE_DATE,E.ALL_COMP,H.WAREHOUSE_NAME,G.FLOOR,H.TMPR_TYPE,I.AREA_NAME,F.PICK_LOC,
			            F.ITEM_CODE,J.ITEM_NAME,J.ITEM_SIZE,J.ITEM_COLOR,J.ITEM_SPEC,K.ACC_UNIT_NAME,F.SERIAL_NO,L.CUST_ORD_NO,L.CHANNEL,N.NAME,M.ISPICKSHOWCUSTNAME,L.CONSIGNEE,L.MEMO,J.EAN_CODE1,M.SHORT_NAME  {validDateString}
			            ) A
	                    ORDER BY A.TAKE_DATE, A.TAKE_TIME,A.PICK_LOC
                        ";

			var result = SqlQuery<F051201ReportDataA>(sql, paramList.ToArray());

			return result;
		}

		public IQueryable<F051201ReportDataB> GetF051201ReportDataBs(string dcCode, string gupCode, string custCode,
string delvDate, string pickTime, string pickOrdNo, string ordType)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", delvDate),
								new SqlParameter("@p4", pickTime),
								new SqlParameter("@p5", ordType),
						};
			var filtersql = string.Empty;
			if (!string.IsNullOrEmpty(pickOrdNo))
			{
				var pickOrdNoList = pickOrdNo.Split(',').ToList();
				var insql = new List<string>();
				foreach (var item in pickOrdNoList)
				{
					insql.Add("@p" + parameters.Count);
					parameters.Add(new SqlParameter("@p" + parameters.Count, item));
				}
				filtersql += string.Format(" AND A.PICK_ORD_NO IN({0}) ", string.Join(",", insql.ToArray()));
			}
			var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.PICK_ORD_NO, A.CUST_CODE, A.GUP_CODE, A.DC_CODE ASC) ROWNUM,A.*
                           FROM (
                               SELECT DISTINCT 
                               A.DC_CODE,
                               A.GUP_CODE,
                               A.CUST_CODE,
                               A.PICK_ORD_NO,
                               A.DELV_DATE,
                               A.PICK_TIME,
                               B.WMS_ORD_NO,
                               ISNULL (C.B_DELV_QTY, 0) B_DELV_QTY,
                               C.ITEM_CODE,
                               D.ITEM_NAME,
                               E.GUP_NAME,
                               F.SHORT_NAME CUST_NAME,
                               G.ACC_UNIT_NAME ITEM_UNIT,
                               D.ITEM_SIZE,
                               D.ITEM_SPEC,
                               D.ITEM_COLOR
                          FROM F051201 A
                               INNER JOIN F051202 H
                               ON H.DC_CODE = A.DC_CODE
                               AND H.GUP_CODE = A.GUP_CODE
                               AND H.CUST_CODE = A.CUST_CODE
                               AND H.PICK_ORD_NO = A.PICK_ORD_NO
                               INNER JOIN F050801 B
                                  ON     B.WMS_ORD_NO = H.WMS_ORD_NO
                                     AND B.DC_CODE = H.DC_CODE
                                     AND B.GUP_CODE = H.GUP_CODE
                                     AND B.CUST_CODE = H.CUST_CODE
                               INNER JOIN F050802 C
                                  ON     C.WMS_ORD_NO = B.WMS_ORD_NO
                                     AND C.DC_CODE = B.DC_CODE
                                     AND C.GUP_CODE = B.GUP_CODE
                                     AND C.CUST_CODE = B.CUST_CODE
                               INNER JOIN F1903 D
                                  ON D.GUP_CODE = C.GUP_CODE AND D.ITEM_CODE = C.ITEM_CODE AND D.CUST_CODE = C.CUST_CODE
                               INNER JOIN F1929 E ON E.GUP_CODE = A.GUP_CODE
                               INNER JOIN F1909 F
                                  ON F.GUP_CODE = E.GUP_CODE AND F.CUST_CODE = A.CUST_CODE
                               LEFT JOIN (SELECT * FROM F91000302 WHERE F91000302.ITEM_TYPE_ID = '001') G
                                  ON D.ITEM_UNIT = G.ACC_UNIT
                         WHERE     A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND A.DELV_DATE = @p3
                               AND A.PICK_TIME = @p4
                               AND A.ORD_TYPE = @p5
                               AND B.VIRTUAL_ITEM <> '1' --排除虛擬商品出貨單
                               AND NOT EXISTS --排除取消的訂單
                                          (SELECT 1 
                                             FROM F05030101 G
                                                  INNER JOIN F050301 H
                                                     ON     H.DC_CODE = G.DC_CODE
                                                        AND H.GUP_CODE = G.GUP_CODE
                                                        AND H.CUST_CODE = G.CUST_CODE
                                                        AND H.ORD_NO = G.ORD_NO
                                            WHERE     G.DC_CODE = B.DC_CODE
                                                  AND G.GUP_CODE = B.GUP_CODE
                                                  AND G.CUST_CODE = B.CUST_CODE
                                                  AND G.WMS_ORD_NO = B.WMS_ORD_NO
                                                  AND H.PROC_FLAG = '9')
                               {filtersql}) A";


			var result = SqlQuery<F051201ReportDataB>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F051201Progress> GetOrderProcessProgress(string dcCode, string gupCode, string custCode, string pickTime, DateTime? delvDate)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", pickTime),
								new SqlParameter("@p4", delvDate)
						};


			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY a.PICK_ORD_NO ASC) ROWNUM, 
                                 a.DC_CODE,
                                 a.GUP_CODE,
                                 a.CUST_CODE,
                                 a.PICK_ORD_NO,
                                 a.DELV_DATE,
                                 a.PICK_TIME,
                                 b.PICK_PERIOD,
                                 c.PACKAGE_PERIOD,
                                 d.RETURN_DATE
						FROM F051201 a 
						LEFT JOIN (SELECT DC_CODE,
                                          GUP_CODE,
                                          CUST_CODE,
                                          PICK_ORD_NO,
                                          CONVERT(varchar(5),MIN(CRT_DATE),108) + '~' + CONVERT(varchar(5),MAX(UPD_DATE),108) as PICK_PERIOD 
									FROM F051202 
									GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO ) b 
									ON a.DC_CODE = b.DC_CODE 
                                   AND a.GUP_CODE = b.GUP_CODE 
                                   AND a.CUST_CODE = b.CUST_CODE 
                                   AND a.PICK_ORD_NO = b.PICK_ORD_NO
						LEFT JOIN (SELECT DC_CODE,
                                          GUP_CODE,
                                          CUST_CODE,
                                          DELV_DATE,
                                          PICK_TIME,
                                          CONVERT(varchar(5),MIN(CRT_DATE),108) + '~' + CONVERT(varchar(5), MAX(UPD_DATE),108) as PACKAGE_PERIOD 
								   FROM F055001 
								   GROUP BY DC_CODE,GUP_CODE,CUST_CODE,DELV_DATE,PICK_TIME) c 
								   ON a.DC_CODE = c.DC_CODE 
                                  AND a.GUP_CODE = c.GUP_CODE 
                                  AND a.CUST_CODE = c.CUST_CODE 
                                  AND a.DELV_DATE = c.DELV_DATE 
                                  AND a.PICK_TIME = c.PICK_TIME
						LEFT JOIN F0513 d 
								   ON a.DC_CODE = d.DC_CODE 
                                  AND a.GUP_CODE = d.GUP_CODE 
                                  AND a.CUST_CODE = d.CUST_CODE 
                                  AND a.DELV_DATE = d.DELV_DATE 
                                  AND a.PICK_TIME = d.PICK_TIME
						WHERE a.DC_CODE = @p0
						  AND a.GUP_CODE = @p1
						  AND a.CUST_CODE = @p2
						  AND a.PICK_TIME = @p3
						  AND a.DELV_DATE = @p4 ";

			var result = SqlQuery<F051201Progress>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F051201> GetDatasByOrdNo(string dcCode, string gupCode, string custCode, string[] ordNo)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode };

			var sql = @"SELECT DISTINCT A.* 
                        FROM F051201 A 
                       INNER JOIN F051202 E
                          ON E.DC_CODE = A.DC_CODE
                         AND E.GUP_CODE = A.GUP_CODE
                         AND E.CUST_CODE = A.CUST_CODE
                         AND E.PICK_ORD_NO = A.PICK_ORD_NO
                       INNER JOIN F050801 B 
                          ON B.DC_CODE = E.DC_CODE 
                         AND B.GUP_CODE = E.GUP_CODE 
                         AND B.CUST_CODE = E.CUST_CODE
                         AND B.WMS_ORD_NO = E.WMS_ORD_NO 
                       INNER JOIN F05030101 C 
                          ON C.DC_CODE = B.DC_CODE 
                         AND C.GUP_CODE = B.GUP_CODE 
                         AND C.CUST_CODE = B.CUST_CODE 
                         AND C.WMS_ORD_NO = B.WMS_ORD_NO 
                       INNER JOIN F050301 D 
                          ON D.DC_CODE = C.DC_CODE 
                         AND D.GUP_CODE = C.GUP_CODE 
                         AND D.CUST_CODE = C.CUST_CODE 
                         AND D.ORD_NO = C.ORD_NO 
                       WHERE A.DC_CODE = @p0 
                         AND A.GUP_CODE = @p1 
                         AND A.CUST_CODE = @p2 ";

			sql += parameter.CombineSqlInParameters(" AND D.ORD_NO ", ordNo.ToList());

			var result = SqlQuery<F051201>(sql, parameter.ToArray());

			return result;
		}

		#region Schedule Check - 撿貨時間過長
		public IQueryable<ExceedPickFinishTime> GetExceedPickFinishTimeDatas(DateTime selectDate)
		{
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY T.PICK_ORD_NO ASC) ROWNUM, T.*, F1901.DC_NAME, F1909.CUST_NAME, F1929.GUP_NAME FROM ( 
                        SELECT TOP 100 PERCENT F051201.PICK_ORD_NO, F051201.DC_CODE, F051201.GUP_CODE, F051201.CUST_CODE, ISNULL (F051201.UPD_DATE, F051201.CRT_DATE) CREATE_DATE, F0003.SYS_PATH PickFinishTime
                          FROM (SELECT *
                                  FROM F051201
                                 WHERE F051201.PICK_STATUS <> '2') F051201
                               -- 撿貨單狀態=未撿貨完成
                               JOIN (SELECT *
                                       FROM F050801
                                      WHERE F050801.STATUS <> '9') F050801
                                  --出貨單狀態=未取消
                                  ON F050801.PICK_ORD_NO = F051201.PICK_ORD_NO
                               JOIN (SELECT *
                                       FROM F0003
                                      WHERE AP_NAME = 'PickFinishTime') F0003
                                  ON     F0003.DC_CODE = F051201.DC_CODE
                                     AND F0003.GUP_CODE = F051201.GUP_CODE
                                     AND F0003.CUST_CODE = F051201.CUST_CODE
                         WHERE DATEDIFF(MINUTE, ISNULL (F051201.UPD_DATE, F051201.CRT_DATE), @p0) > F0003.SYS_PATH
                         ORDER BY F051201.PICK_ORD_NO ) T
                         LEFT JOIN F1901 ON T.DC_CODE = F1901.DC_CODE
                         LEFT JOIN F1909 ON T.GUP_CODE = F1909.GUP_CODE AND T.CUST_CODE = F1909.CUST_CODE 
                         LEFT JOIN F1929 ON T.GUP_CODE = F1929.GUP_CODE 
                         --F051201的撿貨單建立日期若大於F0003的設定上限，需發送郵件通知
                        ";
			var parameters = new List<object>
						{
								selectDate.ToString("yyyy/MM/dd HH:mm:ss")
						};

			var result = SqlQuery<ExceedPickFinishTime>(sql, parameters.ToArray());

			return result;
		}
		#endregion

		public IQueryable<F051201> GetDatasByNoVirturlItem(string dcCode, string gupCode, string custCode, DateTime delvDate,
				string pickTime)
		{
			var sql = @"SELECT *
						FROM F051201 A
						WHERE A.DC_CODE=@p0 
						 AND  A.GUP_CODE= @p1
						 AND A.CUST_CODE =@p2
						 AND A.DELV_DATE =@p3
						 AND A.PICK_TIME =@p4
						 AND A.ORD_TYPE='1'
						 AND NOT EXISTS
						 (SELECT 1 
							 FROM F051202 B
							 INNER JOIN F050801 C
							 ON C.DC_CODE = B.DC_CODE
							 AND C.GUP_CODE = B.GUP_CODE
							 AND C.CUST_CODE = B.CUST_CODE
							 AND C.WMS_ORD_NO = B.WMS_ORD_NO
							 WHERE B.DC_CODE = A.DC_CODE
							 AND B.GUP_CODE = A.GUP_CODE
							 AND B.CUST_CODE = A.CUST_CODE
							 AND B.PICK_ORD_NO = A.PICK_ORD_NO 
							 AND C.VIRTUAL_ITEM ='1'
							 )";

			var param = new object[] { dcCode, gupCode, custCode, delvDate, pickTime };

			var result = SqlQuery<F051201>(sql, param);

			return result;
		}

		public IQueryable<P050103PickTime> GetPickTimes(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate)
		{
			var sql = @" SELECT DISTINCT A.PICK_TIME
										 FROM F051201 A
										 JOIN F050801 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
                      AND B.STATUS <>'9'
									  WHERE A.DC_CODE = @p0
											AND A.GUP_CODE = @p1
											AND A.CUST_CODE = @p2
											AND A.DELV_DATE = @p3";

			var parms = new List<object> { dcCode, gupCode, custCode, delvDate };

			if (!string.IsNullOrEmpty(ordType))
			{
				sql += string.Format(" {0} ", "AND A.ORD_TYPE = @p4");
				parms.Add(ordType);
			}

			sql += @" AND A.PICK_STATUS <>'9'
					ORDER BY A.PICK_TIME ";

			var result = SqlQuery<P050103PickTime>(sql, parms.ToArray());

			return result;
		}

		public IQueryable<P050103PickOrdNo> GetPickOrderNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickTime)
		{
			var sql = @" SELECT DISTINCT A.PICK_ORD_NO
										 FROM F051201 A
                        JOIN F051202 C
                          ON C.DC_CODE = A.DC_CODE
                         AND C.GUP_CODE = A.GUP_CODE
                         AND C.CUST_CODE = A.CUST_CODE
                         AND C.PICK_ORD_NO = A.PICK_ORD_NO
                        JOIN F050801 B
                          ON B.DC_CODE = C.DC_CODE
                         AND B.GUP_CODE = C.GUP_CODE
                         AND B.CUST_CODE = C.CUST_CODE
                         AND B.WMS_ORD_NO = C.WMS_ORD_NO
                         AND B.STATUS <>'9'
					    					WHERE A.DC_CODE = @p0
					    					  AND A.GUP_CODE = @p1
					    					  AND A.CUST_CODE = @p2
					    						AND A.DELV_DATE = @p3
					    						AND A.ORD_TYPE= @p4
					    						AND A.PICK_STATUS <>'9'
                         AND A.PICK_TIME = CASE WHEN @p5 = '' THEN A.PICK_TIME ELSE @p6 END
											ORDER BY A.PICK_ORD_NO ";

			var parms = new object[] { dcCode, gupCode, custCode, delvDate, ordType, pickTime, pickTime };

			var result = SqlQuery<P050103PickOrdNo>(sql, parms);

			return result;
		}

		public IQueryable<P050103WmsOrdNo> GetWmsOrderNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickTime)
		{
			var sql = @" SELECT DISTINCT A.PICK_ORD_NO,B.WMS_ORD_NO
										 FROM F051201 A
                     JOIN F051202 C
                       ON C.DC_CODE = A.DC_CODE
                      AND C.GUP_CODE = A.GUP_CODE
                      AND C.CUST_CODE = A.CUST_CODE
                      AND C.PICK_ORD_NO = A.PICK_ORD_NO
                     JOIN F050801 B
                       ON B.DC_CODE = C.DC_CODE
                      AND B.GUP_CODE = C.GUP_CODE
                      AND B.CUST_CODE = C.CUST_CODE
                      AND B.WMS_ORD_NO = C.WMS_ORD_NO
                      AND B.STATUS <>'9'
										WHERE A.DC_CODE = @p0
										  AND A.GUP_CODE = @p1
										  AND A.CUST_CODE = @p2
											AND A.DELV_DATE = @p3
											AND A.ORD_TYPE= @p4
											AND A.PICK_STATUS <>'9'
                      AND A.PICK_TIME = CASE WHEN @p5 = '' THEN A.PICK_TIME ELSE @p6 END
											ORDER BY A.PICK_ORD_NO ";

			var parms = new object[] { dcCode, gupCode, custCode, delvDate, ordType, pickTime, pickTime };

			var result = SqlQuery<P050103WmsOrdNo>(sql, parms);

			return result;
		}

		public IQueryable<P050103ReportData> GetSummaryReport(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickOrdNo, string wmsOrdNo)
		{
			var sqlParamers = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", delvDate),
								new SqlParameter("@p4", ordType)
						};

			var sql = @" SELECT A.DELV_DATE,
                         SUM (C.B_PICK_QTY) as PICK_QTY_SUM,
                         C.SERIAL_NO,
                         C.PICK_LOC,
                         E.WAREHOUSE_NAME,
                         CASE E.TMPR_TYPE
                          WHEN '01' THEN N'常溫'
											    WHEN '02' THEN N'低溫'
                          WHEN '03' THEN N'冷凍'
                         END TMPR_TYPE,
												 F.ITEM_SIZE,
												 F.ITEM_SPEC,
												 F.ITEM_COLOR,
												 C.ITEM_CODE,
												 F.ITEM_NAME,
												 G.AREA_NAME,
												 H.ACC_UNIT_NAME
										FROM F051201 A
										JOIN F051202 C
										  ON C.DC_CODE = A.DC_CODE
										 AND C.GUP_CODE = A.GUP_CODE
										 AND C.CUST_CODE = A.CUST_CODE
										 AND C.PICK_ORD_NO = A.PICK_ORD_NO
                    JOIN F050801 B
										  ON B.DC_CODE = C.DC_CODE
										 AND B.GUP_CODE = C.GUP_CODE
										 AND B.CUST_CODE = C.CUST_CODE
										 AND B.WMS_ORD_NO = C.WMS_ORD_NO
										JOIN F1912 D 
										  ON D.DC_CODE = C.DC_CODE
										 AND (D.GUP_CODE = C.GUP_CODE OR D.GUP_CODE = '0')
										 AND (D.CUST_CODE = C.CUST_CODE OR D.CUST_CODE = '0')
										 AND  D.LOC_CODE = C.PICK_LOC
									  JOIN F1980 E
										  ON E.DC_CODE = D.DC_CODE
										 AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
									  JOIN F1903 F
										  ON F.GUP_CODE = C.GUP_CODE
										 AND F.ITEM_CODE = C.ITEM_CODE
                                         AND F.CUST_CODE = C.CUST_CODE
										JOIN F1919 G
										  ON G.DC_CODE = D.DC_CODE AND G.WAREHOUSE_ID = D.WAREHOUSE_ID AND G.AREA_CODE = D.AREA_CODE 
										JOIN F91000302 H
										  ON  H.ACC_UNIT = F.ITEM_UNIT AND H.ITEM_TYPE_ID = '001'
									 WHERE  B.STATUS <>'9'  
										 AND A.PICK_STATUS<>'9'
										 AND A.DC_CODE = @p0
										 AND A.GUP_CODE = @p1
										 AND A.CUST_CODE = @p2
										 AND A.DELV_DATE = @p3
										 AND A.ORD_TYPE = @p4
       ";
			if (!string.IsNullOrWhiteSpace(pickOrdNo))
			{
				var codes = string.Join("','", pickOrdNo.Split(','));
				codes = string.Format("'{0}'", codes);
				sql += string.Format(" AND A.PICK_ORD_NO IN ({0}) ", codes);
			}
			if (!string.IsNullOrWhiteSpace(wmsOrdNo))
			{
				var codes = string.Join("','", wmsOrdNo.Split(','));
				codes = string.Format("'{0}'", codes);
				sql += string.Format(" AND B.WMS_ORD_NO IN ({0}) ", codes);
			}

			sql += @"  GROUP BY A.DELV_DATE,C.SERIAL_NO,C.PICK_LOC,
												  E.WAREHOUSE_NAME,
													E.TMPR_TYPE,
													F.ITEM_SIZE,
													F.ITEM_SPEC,
													F.ITEM_COLOR,
													F.ITEM_NAME,
													C.ITEM_CODE,
													G.AREA_NAME,
													H.ACC_UNIT_NAME		";
			sql += " ORDER BY C.PICK_LOC";

			var result = SqlQuery<P050103ReportData>(sql, sqlParamers.ToArray()); ;

			return result;
		}
		public void UpdateF051201ByResendTask(string dcCode, string gupCode, string custCode, string wbCode, string pickOrdNo)
		{
			var sql = @" UPDATE F051201 SET UPD_NAME = @p0 , PICK_STATUS = 1 
						  WHERE DC_CODE = @p1 AND GUP_CODE = @p2 AND CUST_CODE = @p3 AND PICK_ORD_NO = @p4  ";

			var parms = new object[] { wbCode, dcCode, gupCode, custCode, pickOrdNo };

			ExecuteSqlCommand(sql, parms);
		}



		public IQueryable<P050112Pick> GetP050112PickDatas(string dcCode, string gupCode, string custCode, DateTime delvDateS, DateTime delvDateE, string pickTool, string areaCode)
		{
			var parms = new List<object> { Current.Lang, Current.Lang, dcCode, gupCode, custCode, delvDateS, delvDateE, pickTool };
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.PICK_ORD_NO, A.CUST_CODE, A.GUP_CODE, A.DC_CODE ASC) ROWNUM,
                                        A.DELV_DATE,A.PICK_TIME, A.PICK_ORD_NO,B.AREA_CODE,C.AREA_NAME,A.PICK_STATUS,D.NAME PICK_STATUS_NAME,B.PICK_TOOL,E.NAME PICK_TOOL_NAME,F.ITEM_CNT,F.TOTAL_QTY
										FROM F051201 A
										JOIN F05120101 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.PICK_ORD_NO = A.PICK_ORD_NO
										JOIN F1919 C
										ON C.DC_CODE = B.DC_CODE
										AND C.AREA_CODE = B.AREA_CODE
										JOIN VW_F000904_LANG D
										ON D.TOPIC='F051201'
										AND D.SUBTOPIC='PICK_STATUS'
										AND D.VALUE = A.PICK_STATUS
										AND D.LANG = @p0
										JOIN VW_F000904_LANG E
										ON E.TOPIC='F191902'
										AND E.SUBTOPIC='PICK_TOOL'
										AND E.VALUE = B.PICK_TOOL
										AND E.LANG =@p1
										JOIN (
										SELECT DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO,COUNT(DISTINCT ITEM_CODE) ITEM_CNT,SUM(B_PICK_QTY) TOTAL_QTY
										FROM F051202
										GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO) F
										ON F.DC_CODE = A.DC_CODE
										AND F.GUP_CODE = A.GUP_CODE
										AND F.CUST_CODE = A.CUST_CODE
										AND F.PICK_ORD_NO = A.PICK_ORD_NO
                    JOIN (
                      SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.PICK_ORD_NO,A.ORD_TYPE
                        FROM F050801 A
                        JOIN F051202 B
                        ON B.DC_CODE = A.DC_CODE
                        AND B.GUP_CODE = A.GUP_CODE
                        AND B.CUST_CODE = A.CUST_CODE
                        AND B.WMS_ORD_NO = A.WMS_ORD_NO
                       GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.PICK_ORD_NO,A.ORD_TYPE
                    ) G
                    ON G.DC_CODE = A.DC_CODE
                    AND G.GUP_CODE = A.GUP_CODE
                    AND G.CUST_CODE = A.CUST_CODE
                    AND G.PICK_ORD_NO = A.PICK_ORD_NO
										WHERE A.PICK_STATUS='0'
                    AND G.ORD_TYPE = '0' --B2B單
										AND A.DC_CODE = @p2
										AND A.GUP_CODE = @p3
										AND A.CUST_CODE = @p4
										AND A.DELV_DATE >= @p5 
										and A.DELV_DATE <= @p6
										AND B.PICK_TOOL = @p7
										";
			if (!string.IsNullOrWhiteSpace(areaCode))
			{
				sql += " AND B.AREA_CODE = @p" + parms.Count;
				parms.Add(areaCode);
			}

			var result = SqlQuery<P050112Pick>(sql, parms.ToArray());

			return result;
		}

		public IQueryable<P050112PickSummary> GetP050112PickSummaries(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var filterSql = string.Empty;
			if (pickOrdNos.Any())
			{
				filterSql = parms.CombineSqlInParameters("AND A.PICK_ORD_NO", pickOrdNos);
			}

			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.CUST_CODE, A.GUP_CODE, A.DC_CODE ASC) ROWNUM, A.*
                      FROM (
											SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,COUNT(DISTINCT B.ITEM_CODE) ITEM_CNT,SUM(B.B_PICK_QTY) TOTAL_QTY,COUNT(DISTINCT C.RETAIL_CODE) RETAIL_CNT
											FROM F051201 A
											JOIN F051202 B
											ON B.DC_CODE = A.DC_CODE
											AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.PICK_ORD_NO = A.PICK_ORD_NO
											JOIN F050801 C
											ON C.DC_CODE = B.DC_CODE
											AND C.GUP_CODE = B.GUP_CODE
											AND C.CUST_CODE = B.CUST_CODE
											AND C.WMS_ORD_NO = B.WMS_ORD_NO
											WHERE A.PICK_STATUS= '0'
											AND A.DC_CODE = @p0
											AND A.GUP_CODE = @p1
											AND A.CUST_CODE = @p2
											{filterSql}
											GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE) A ";

			var result = SqlQuery<P050112PickSummary>(sql, parms.ToArray());

			return result;
		}

		public IQueryable<P050112PickSummaryDetail> GetP050112PickSummaryDetails(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var filterSql = string.Empty;
			if (pickOrdNos.Any())
			{
				filterSql = parms.CombineSqlInParameters("AND A.PICK_ORD_NO", pickOrdNos);
			}
			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.CUST_CODE, A.GUP_CODE, A.DC_CODE ASC) ROWNUM, A.*
                      FROM (
					    SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.AREA_CODE,SUBSTRING(B.PICK_LOC,0,5) SHELF_NO,B.PICK_LOC LOC_CODE,B.ITEM_CODE,SUM(B.B_PICK_QTY) B_PICK_QTY
						    FROM F051201 A
						    JOIN F051202 B
						    ON B.DC_CODE = A.DC_CODE
						    AND B.GUP_CODE = A.GUP_CODE
						    AND B.CUST_CODE = A.CUST_CODE
						    AND B.PICK_ORD_NO = A.PICK_ORD_NO
                        JOIN F1912 D
                          ON D.DC_CODE = B.DC_CODE
                         AND D.LOC_CODE = B.PICK_LOC
												WHERE A.DC_CODE = @p0
                          AND A.GUP_CODE = @p1
                          AND A.CUST_CODE = @p2
                          {filterSql}
						GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.AREA_CODE,C.SHELF_NO,B.PICK_LOC,B.ITEM_CODE ) A ";

			var result = SqlQuery<P050112PickSummaryDetail>(sql, parms.ToArray());

			return result;
		}

		public IQueryable<P050112PickSummaryRetail> GetP050112PickSummaryRetails(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var filterSql = string.Empty;
			if (pickOrdNos.Any())
			{
				filterSql = parms.CombineSqlInParameters("AND A.PICK_ORD_NO", pickOrdNos);
			}
			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE, A.RETAIL_CODE ASC) ROWNUM,A.*
											FROM (
											SELECT TOP 100 PERCENT A.DC_CODE,A.GUP_CODE,A.CUST_CODE, C.RETAIL_CODE,E.RETAIL_NAME
												FROM F051201 A
												JOIN F051202 B
												ON B.DC_CODE = A.DC_CODE
												AND B.GUP_CODE = A.GUP_CODE
												AND B.CUST_CODE = A.CUST_CODE
												AND B.PICK_ORD_NO = A.PICK_ORD_NO
												JOIN F050801 C
												ON C.DC_CODE = B.DC_CODE
												AND C.GUP_CODE = B.GUP_CODE
												AND C.CUST_CODE = B.CUST_CODE
												AND C.WMS_ORD_NO = B.WMS_ORD_NO
												JOIN F1909 D
												ON D.GUP_CODE = C.GUP_CODE
												AND D.CUST_CODE = D.CUST_CODE
												JOIN F1910 E
												ON  E.GUP_CODE = C.GUP_CODE
												AND E.CUST_CODE = CASE WHEN D.ALLOWGUP_RETAILSHARE = '1' THEN '0' ELSE  C.CUST_CODE END
												AND E.RETAIL_CODE = C.RETAIL_CODE
												WHERE A.DC_CODE = @p0
													AND A.GUP_CODE = @p1
													AND A.CUST_CODE = @p2
                          {filterSql}
												GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE, C.RETAIL_CODE,E.RETAIL_NAME
                        ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE, C.RETAIL_CODE) A ";

			var result = SqlQuery<P050112PickSummaryRetail>(sql, parms.ToArray());

			return result;
		}

		public void UpdateF051201PickStatus(string dcCode, string gupCode, string custCode, List<string> pickOrdNos, string pickStatus)
		{
			var sql = @" UPDATE F051201 
                      SET PICK_STATUS = @p0,UPD_DATE = dbo.GetSysDate(),UPD_STAFF= @p1,UPD_NAME =@p2
                    WHERE DC_CODE = @p3
                      AND GUP_CODE =@p4
                      AND CUST_CODE = @p5 ";
			var parms = new List<object> { pickStatus, Current.Staff, Current.StaffName, dcCode, gupCode, custCode };
			if (pickOrdNos.Any())
			{
				sql += parms.CombineSqlInParameters("AND PICK_ORD_NO", pickOrdNos);
			}
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		/// <summary>
		/// 取得PDA批量揀貨單
		/// </summary>
		/// <param name="mode">作業模式</param>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="wmsNo">彙總/揀貨單號</param>
		/// <param name="shipDate">彙總/揀貨日期</param>
		/// <param name="pickType">揀貨方式</param>
		/// <returns></returns>
		public IQueryable<GetPickRes> GetPdaBatchPick(string mode, string dcCode, string gupCode, string custCode, string wmsNo, DateTime? shipDate, string empId)
		{
			var parms = new List<SqlParameter> {
																new SqlParameter("@p0",mode),
																new SqlParameter("@p1",Current.Lang),
																new SqlParameter("@p2",dcCode),
																new SqlParameter("@p3",gupCode),
																new SqlParameter("@p4",custCode),
						};

			string conditionA = string.Empty;

			if (!string.IsNullOrWhiteSpace(wmsNo))
			{
				conditionA += @" AND A.PICK_ORD_NO = @p"+parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, wmsNo));
			}

			if (shipDate != null)
			{
				conditionA += " AND A.DELV_DATE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, shipDate));
			}

			var sql = $@" SELECT @p0 AS OTNo,
                                 A.DC_CODE DcNo,
                                 A.CUST_CODE CustNo,
                                 A.PICK_ORD_NO WmsNo,
                                 A.DELV_DATE WmsDate,
                                 CONVERT(CHAR (1),CASE when ISNULL(A.PICK_STAFF,'')='' THEN '0' ELSE A.PICK_STATUS END) Status,
                                 E.NAME StatusName,
                                 A.PICK_STAFF AccNo,
                                 A.PICK_NAME UserName,
                                 CASE WHEN A.SPLIT_TYPE ='01' THEN A.SPLIT_CODE + ISNULL((SELECT WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = A.DC_CODE AND WAREHOUSE_ID=A.SPLIT_CODE),'') 
                                      WHEN A.SPLIT_TYPE ='02' THEN A.SPLIT_CODE + ISNULL((SELECT PK_NAME FROM F191206 WHERE DC_CODE = A.DC_CODE AND PK_AREA = A.SPLIT_CODE),'')
                                      WHEN A.SPLIT_TYPE ='03' THEN  N'跨PK區'
                                 ELSE A.SPLIT_CODE END AreaName,
                                 COUNT(DISTINCT B.ITEM_CODE) ItemCnt,
                                 SUM(B.B_PICK_QTY) ItemQty,
                                 COUNT(DISTINCT B.ITEM_CODE) - COUNT(DISTINCT CASE WHEN B.PICK_STATUS='0' THEN B.ITEM_CODE ELSE NULL END) PickItemCnt,
																 CASE WHEN A.NEXT_STEP = '6' THEN ISNULL(F.CROSS_NAME, NULL) ELSE NULL END MoveOutTarget
													  FROM F051201 A 
													  JOIN F051203 B
													    ON B.DC_CODE = A.DC_CODE
													   AND B.GUP_CODE = A.GUP_CODE
													   AND B.CUST_CODE = A.CUST_CODE
													   AND B.PICK_ORD_NO = A.PICK_ORD_NO
													  JOIN F1912 C 
                              ON C.DC_CODE = B.DC_CODE
                             AND C.LOC_CODE = B.PICK_LOC		
                            LEFT JOIN VW_F000904_LANG E
                              ON E.TOPIC='F051201'
                             AND E.SUBTOPIC='PICK_STATUS'
                             AND E.VALUE = CONVERT(CHAR,CASE when ISNULL(A.PICK_STAFF,'')='' THEN '0' ELSE A.PICK_STATUS END)
                             AND E.LANG = @p1
														 LEFT JOIN F0001 F
														 ON A.MOVE_OUT_TARGET = F.CROSS_CODE
														 AND CROSS_TYPE = '01'													 
														 WHERE A.DC_CODE =@p2
														 AND A.GUP_CODE = @p3
														 AND A.CUST_CODE = @p4
                             AND A.PICK_STATUS  IN ('0','1')
                             AND A.DISP_SYSTEM = '0' --派發系統為WMS
                             AND A.SPLIT_TYPE <> '03' --批量揀貨
                             AND A.PICK_TOOL = '2' --PDA揀貨
														 AND A.ISPRINTED = '1' --已列印
                             AND B.PICK_STATUS <> '9' --排除揀貨明細取消
														 {conditionA}
													 GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,A.DELV_DATE,A.PICK_STATUS,A.PICK_STAFF,A.PICK_NAME,A.SPLIT_TYPE,A.SPLIT_CODE,E.NAME, A.NEXT_STEP, F.CROSS_NAME
                        ";

			var result = SqlQuery<GetPickRes>(sql, parms.ToArray());
			return result;
		}

		/// <summary>
		/// 取得PDA單一揀貨單
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="shipDate"></param>
		/// <param name="empId"></param>
		/// <returns></returns>
		public IQueryable<GetPickRes> GetPdaSinglePick(string mode, string dcCode, string gupCode, string custCode, string wmsNo, DateTime? shipDate, string empId)
		{
			var parms = new List<SqlParameter> {
																new SqlParameter("@p0",mode),
																new SqlParameter("@p1",Current.Lang),
																new SqlParameter("@p2",dcCode),
																new SqlParameter("@p3",gupCode),
																new SqlParameter("@p4",custCode),
						};

			string conditionA = string.Empty;

			if (!string.IsNullOrWhiteSpace(wmsNo))
			{
				conditionA += @" AND A.PICK_ORD_NO = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, wmsNo));
			}

			if (shipDate != null)
			{
				conditionA += " AND A.DELV_DATE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, shipDate));
			}
			var sql = $@" SELECT @p0 AS OTNo,
                                 A.DC_CODE DcNo,
                                 A.CUST_CODE CustNo,
                                 A.PICK_ORD_NO WmsNo,
                                 A.DELV_DATE WmsDate,
                                 CONVERT(CHAR (1),CASE when ISNULL(A.PICK_STAFF,'')='' THEN '0' ELSE A.PICK_STATUS END) Status,
                                 E.NAME StatusName,
                                 A.PICK_STAFF AccNo,
                                 A.PICK_NAME UserName,
                                 CASE WHEN A.SPLIT_TYPE ='01' THEN A.SPLIT_CODE + ISNULL((SELECT WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = A.DC_CODE AND WAREHOUSE_ID=A.SPLIT_CODE),'') 
                                      WHEN A.SPLIT_TYPE ='02' THEN A.SPLIT_CODE + ISNULL((SELECT PK_NAME FROM F191206 WHERE DC_CODE = A.DC_CODE AND PK_AREA = A.SPLIT_CODE),'')
                                      WHEN A.SPLIT_TYPE ='03' THEN  N'跨PK區'
                                 ELSE A.SPLIT_CODE END AreaName,
                                 COUNT(DISTINCT B.ITEM_CODE) ItemCnt,
                                 SUM(B.B_PICK_QTY) ItemQty,
                                 COUNT(DISTINCT B.ITEM_CODE) - COUNT(DISTINCT CASE WHEN B.PICK_STATUS='0' THEN B.ITEM_CODE ELSE NULL END) PickItemCnt
													  FROM F051201 A 
													  JOIN F051202 B
													    ON B.DC_CODE = A.DC_CODE
													   AND B.GUP_CODE = A.GUP_CODE
													   AND B.CUST_CODE = A.CUST_CODE
													   AND B.PICK_ORD_NO = A.PICK_ORD_NO
													  JOIN F1912 C 
                              ON C.DC_CODE = B.DC_CODE
                             AND C.LOC_CODE = B.PICK_LOC
                            LEFT JOIN VW_F000904_LANG E
                              ON E.TOPIC='F051201'
                             AND E.SUBTOPIC='PICK_STATUS'
                             AND E.VALUE = CONVERT(CHAR,CASE when ISNULL(A.PICK_STAFF,'')='' THEN '0' ELSE A.PICK_STATUS END)
                             AND E.LANG = @p1
													 WHERE A.DC_CODE =@p2
														 AND A.GUP_CODE = @p3
														 AND A.CUST_CODE = @p4
                             AND A.PICK_STATUS  IN ('0','1')
                             AND A.DISP_SYSTEM = '0' --派發系統為WMS
                             AND A.SPLIT_TYPE = '03' --單一揀貨
                             AND A.PICK_TOOL = '2' --PDA揀貨
														 AND A.ISPRINTED = '1' --已列印
                             AND B.PICK_STATUS <> '9' --排除揀貨明細取消
														 {conditionA}
													 GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,A.DELV_DATE,A.PICK_STATUS,A.PICK_STAFF,A.PICK_NAME,A.SPLIT_TYPE,A.SPLIT_CODE,E.NAME
                        ";

			var result = SqlQuery<GetPickRes>(sql, parms.ToArray());
			return result;
		}


		// 單一揀貨標籤貼紙
		public IQueryable<RP0501010004Model> GetF051201SingleStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
				DateTime delvDate, string pickTime, string pickOrdNo, string ordType = null)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime, ordType };
			var filterSql = string.Empty;
			if (!string.IsNullOrEmpty(pickOrdNo))
			{
				filterSql = paramList.CombineSqlInParameters(" AND A.PICK_ORD_NO", pickOrdNo.Split(','));
			}
			var sql = $@"SELECT ROW_NUMBER()OVER(ORDER by A.WMS_ORD_NO) ROWNUM,A.WMS_ORD_NO from (select B.WMS_ORD_NO from F051201 A
                            join F051202 B on A.PICK_ORD_NO =B.PICK_ORD_NO 
                            and A.DC_CODE =B.DC_CODE 
                            and A.GUP_CODE =B.GUP_CODE 
                            and A.CUST_CODE =B.CUST_CODE  
                            WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND A.DELV_DATE = @p3
                            AND A.PICK_TIME = @p4 
                            AND A.ORD_TYPE = @p5
                            AND A.DISP_SYSTEM ='0' --派發系統為WMS
                            AND A.PICK_TYPE IN('0','5') --單一揀貨/快速補揀單
                            AND A.PICK_TOOL = '2' --PDA揀貨單
                             {filterSql}
                           GROUP BY B.PICK_ORD_NO ,B.WMS_ORD_NO) A";

			var result = SqlQuery<RP0501010004Model>(sql, paramList.ToArray());
			return result;
		}

		// 批次揀貨揀貨單
		public IQueryable<P050103ReportData> GetF051201BatchReportDataAsForB2C(string dcCode, string gupCode, string custCode,
			 DateTime delvDate, string pickTime, string pickOrdNo, string ordType = null)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime, ordType };
			var filterSql = string.Empty;
			if (!string.IsNullOrEmpty(pickOrdNo))
			{
				filterSql = paramList.CombineSqlInParameters(" AND A.PICK_ORD_NO", pickOrdNo.Split(','));
			}

			var sql = $@"SELECT A.PICK_ORD_NO,A.DELV_DATE,A.PICK_TIME,
                         SUM (C.B_PICK_QTY) as PICK_QTY_SUM,
                         C.SERIAL_NO,
                         C.PICK_LOC,
                         E.WAREHOUSE_NAME,
                         CASE E.TMPR_TYPE
                          WHEN '01' THEN N'常溫'
				          WHEN '02' THEN N'低溫'
                          WHEN '03' THEN N'冷凍'
                         END TMPR_TYPE,
												 F.ITEM_SIZE,
												 F.ITEM_SPEC,
												 F.ITEM_COLOR,
												 C.ITEM_CODE,
												 F.ITEM_NAME,
												 G.AREA_NAME,
												 H.ACC_UNIT_NAME,
                         I.GUP_NAME,
                         J.SHORT_NAME CUST_NAME,
                         C.VALID_DATE
										FROM F051201 A
										JOIN F051202 C
										  ON C.DC_CODE = A.DC_CODE
										 AND C.GUP_CODE = A.GUP_CODE
										 AND C.CUST_CODE = A.CUST_CODE
										 AND C.PICK_ORD_NO = A.PICK_ORD_NO
                    					JOIN F050801 B
										  ON B.DC_CODE = C.DC_CODE
										 AND B.GUP_CODE = C.GUP_CODE
										 AND B.CUST_CODE = C.CUST_CODE
										 AND B.WMS_ORD_NO = C.WMS_ORD_NO
										JOIN F1912 D 
										  ON D.DC_CODE = C.DC_CODE
										 AND (D.GUP_CODE = C.GUP_CODE OR D.GUP_CODE = '0')
										 AND (D.CUST_CODE = C.CUST_CODE OR D.CUST_CODE = '0')
										 AND  D.LOC_CODE = C.PICK_LOC
									  JOIN F1980 E
										  ON E.DC_CODE = D.DC_CODE
										 AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
									  JOIN F1903 F
										  ON F.GUP_CODE = C.GUP_CODE
										 AND F.ITEM_CODE = C.ITEM_CODE
                                         AND F.CUST_CODE = C.CUST_CODE
										JOIN F1919 G
										  ON G.DC_CODE = D.DC_CODE AND G.WAREHOUSE_ID = D.WAREHOUSE_ID AND G.AREA_CODE = D.AREA_CODE 
										JOIN F91000302 H
										  ON  H.ACC_UNIT = F.ITEM_UNIT AND H.ITEM_TYPE_ID = '001'
                    JOIN F1929 I
                      ON I.GUP_CODE = A.GUP_CODE
                    JOIN F1909 J
                      ON J.GUP_CODE = A.GUP_CODE
                     AND J.CUST_CODE = A.CUST_CODE
									 WHERE  B.STATUS <>'9'  
										 AND A.PICK_STATUS<>'9'
										 AND A.DC_CODE = @p0
										 AND A.GUP_CODE = @p1
										 AND A.CUST_CODE = @p2
										 AND A.DELV_DATE = @p3
                                        AND A.PICK_TIME = @p4
										 AND A.ORD_TYPE = @p5
                     AND A.DISP_SYSTEM = '0' -- 派發系統為WMS
                     AND A.PICK_TYPE IN('1','3','4') --揀貨單為批量揀貨單或特殊結構訂單
                     AND A.PICK_TOOL = '1' --紙本
                                          {filterSql}
        									GROUP BY A.PICK_ORD_NO,A.DELV_DATE,A.PICK_TIME,C.SERIAL_NO,C.PICK_LOC,
												  E.WAREHOUSE_NAME,
													E.TMPR_TYPE,
													F.ITEM_SIZE,
													F.ITEM_SPEC,
													F.ITEM_COLOR,
													F.ITEM_NAME,
													C.ITEM_CODE,
													G.AREA_NAME,
													H.ACC_UNIT_NAME,I.GUP_NAME,J.SHORT_NAME,
                                                    C.VALID_DATE
													ORDER BY C.PICK_LOC";
			var result = SqlQuery<P050103ReportData>(sql, paramList.ToArray());
			return result;
		}

		// 批次揀貨標簽貼紙
		public IQueryable<RP0501010005Model> GetF051201BatchStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
				DateTime delvDate, string pickTime, string pickOrdNo, string ordType = null)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime, ordType };
			var filterSql = string.Empty;
			if (!string.IsNullOrEmpty(pickOrdNo))
			{
				filterSql = paramList.CombineSqlInParameters(" AND A.PICK_ORD_NO", pickOrdNo.Split(','));
			}
			var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.PICK_ORD_NO),A.PICK_ORD_NO FROM (
                            SELECT 
                            A.PICK_ORD_NO from F051201 A
                            join F051202 B on A.PICK_ORD_NO =B.PICK_ORD_NO 
                            and A.DC_CODE =B.DC_CODE 
                            and A.GUP_CODE =B.GUP_CODE 
                            and A.CUST_CODE =B.CUST_CODE 
                            WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND A.DELV_DATE = @p3
                            AND A.PICK_TIME = @p4 
                            AND A.ORD_TYPE = @p5
                            AND A.DISP_SYSTEM = '0' -- 派發系統為WMS
														AND A.PICK_TYPE IN('1','3','4') --揀貨單為批量揀貨單或特殊結構訂單
														AND A.PICK_TOOL = '2' --紙本
                            {filterSql}
                           GROUP BY A.PICK_ORD_NO) A";
			var result = SqlQuery<RP0501010005Model>(sql, paramList.ToArray());
			return result;
		}

		public void UpdateStatus(string dcCode, string gupCode, string custCode, string pickOrdNo, string status)
		{
			var parameters = new List<object>
						{
								status,
								Current.Staff,
								Current.StaffName,
								dcCode,
								gupCode,
								custCode,
								pickOrdNo
						};

			var sql = @"
				UPDATE  F051201  SET PICK_STATUS= @p0,
                                     UPD_STAFF = @p1,
						             UPD_DATE = dbo.GetSysDate(),
						             UPD_NAME = @p2
				 Where DC_CODE = @p3
                     And GUP_CODE = @p4
					 And CUST_CODE = @p5
					 And PICK_ORD_NO = @p6 ";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<F051201> GetDatasByWmsOrdNos(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT DISTINCT A.*
                     FROM F051201 A
                     JOIN F051202 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
                    WHERE B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND B.WMS_ORD_NO", wmsOrdNos);
			return SqlQuery<F051201>(sql, parms.ToArray());
		}

		public IQueryable<RePickNoList> GetRePickNoList(string dcCode,string gupCode,string custCode,string sourceType,string custCost)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };

			var sql = $@"SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.CRT_DATE,A.PICK_ORD_NO,
                  (SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='CUST_COST' AND VALUE = B.CUST_COST  AND LANG='{Current.Lang}') CUST_COST,
			            		(SELECT TOP(1) NAME FROM  VW_F000904_LANG WHERE TOPIC='F050101' AND SUBTOPIC='FAST_DEAL_TYPE' AND VALUE = B.FAST_DEAL_TYPE  AND LANG='{Current.Lang}') FAST_DEAL_TYPE,        
                  D.SOURCE_NAME AS SOURCE_TYPE,A.PICK_TOOL,
                  (SELECT TOP(1) COLLECTION_NAME FROM F1945 WHERE DC_CODE = C.DC_CODE AND COLLECTION_CODE = C.COLLECTION_CODE) AS COLLECTION_CODE, E.CROSS_NAME,
                  CASE WHEN B.ORDER_PROC_TYPE ='1' THEN '是' ELSE NULL END ORDER_PROC_TYPE_NAME
            FROM F051201 A
						JOIN F0513 B 
						ON A.DC_CODE = B.DC_CODE 
						AND A.GUP_CODE  = B.GUP_CODE 
						AND A.CUST_CODE = B.CUST_CODE 
						AND A.DELV_DATE = B.DELV_DATE 
						AND A.PICK_TIME  = B.PICK_TIME 
						JOIN F051301 C 
						ON B.DC_CODE =C.DC_CODE 
						AND B.GUP_CODE =C.GUP_CODE 
						AND B.CUST_CODE =C.CUST_CODE 
						AND B.DELV_DATE  = C.DELV_DATE
						AND B.PICK_TIME = C.PICK_TIME 
						AND A.SPLIT_CODE = C.WMS_NO 
            JOIN F000902 D
						 ON D.SOURCE_TYPE = ISNULL(B.SOURCE_TYPE,'01')
            LEFT JOIN F0001 E
                        ON A.MOVE_OUT_TARGET = E.CROSS_CODE
                            AND E.CROSS_TYPE ='01'
						WHERE A.DC_CODE =@p0
						AND A.GUP_CODE =@p1
						AND A.CUST_CODE  = @p2
						
						AND A.ISPRINTED ='0'
						AND A.DISP_SYSTEM ='0'
            AND A.PICK_TYPE ='5'";

			if(!string.IsNullOrWhiteSpace(sourceType))
			{
				sql += "AND (CASE WHEN B.SOURCE_TYPE='' OR B.SOURCE_TYPE IS NULL THEN '01' ELSE B.SOURCE_TYPE END) = @p" + parms.Count;
				parms.Add(sourceType);
			}
			if (!string.IsNullOrWhiteSpace(custCost))
			{
				sql += " AND B.CUST_COST = @p"+parms.Count;
				parms.Add(custCost);
			}
			var result = SqlQuery<RePickNoList>(sql, parms.ToArray());

			return result;
		}

		public IQueryable<RePrintPickNoList> GetRePrintPickNoList(string dcCode,string gupCode,string custCode,string pickOrdNo,string wmsOrdNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql2 = string.Empty;
			if(!string.IsNullOrEmpty(pickOrdNo))
			{
				sql2 += " AND A.PICK_ORD_NO=@p" + parms.Count;
				parms.Add(pickOrdNo);
			}

			if (!string.IsNullOrEmpty(wmsOrdNo))
			{
				sql2 += " AND B.WMS_ORD_NO=@p" + parms.Count;
				parms.Add(wmsOrdNo);
			}

			var sql = $@"SELECT DISTINCT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.PICK_ORD_NO ,B.WMS_ORD_NO,A.PICK_TOOL,
            (SELECT NAME FROM VW_F000904_LANG where TOPIC = 'F051201' and SUBTOPIC = 'PICK_TOOL' AND VALUE = A.PICK_TOOL AND LANG ='{Current.Lang}')
            PICK_TOOL_NAME ,
            C.CROSS_NAME
						FROM F051201 A
						JOIN F051202 B
						ON A.DC_CODE = B.DC_CODE 
						AND A.GUP_CODE =B.GUP_CODE 
						AND A.CUST_CODE =B.CUST_CODE 
						AND A.PICK_ORD_NO = B.PICK_ORD_NO 
           LEFT JOIN F0001 C
             ON A.MOVE_OUT_TARGET =C.CROSS_CODE
             AND C.CROSS_TYPE ='01'
					 WHERE A.ISPRINTED ='1'
						AND A.DC_CODE =@p0
						AND A.GUP_CODE =@p1
						AND A.CUST_CODE  = @p2
            AND B.PICK_STATUS <> '9'
            {sql2} ";
			var result = SqlQuery<RePrintPickNoList>(sql, parms.ToArray());

			return result;
		}
		public IQueryable<string> GetNotFinishOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" 
                SELECT DISTINCT  C.ORD_NO
                     FROM F051201 A
                     JOIN F051202 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
										 JOIN F05030101 C
											 ON C.DC_CODE = B.DC_CODE
											AND C.GUP_CODE = B.GUP_CODE
											AND C.CUST_CODE = B.CUST_CODE
											AND C.WMS_ORD_NO = B.WMS_ORD_NO
                    WHERE B.PICK_STATUS = '0'
                      AND B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2";
			if (wmsOrdNos.Any())
				sql += parms.CombineNotNullOrEmptySqlInParameters("AND B.WMS_ORD_NO", wmsOrdNos);
			return SqlQuery<string>(sql, parms.ToArray());
			
		}

		public bool CheckIsBatchPickNotFinished(string dcCode,string gupCode,string custCode,DateTime delvDate,string pickTIme)
		{
			var parms = new object[] { dcCode, gupCode, custCode, delvDate, pickTIme };
			var sql = @" SELECT *
                     FROM F051201
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4
                      AND PICK_STATUS NOT IN ('2','9')";
			return SqlQuery<F051201>(sql, parms).Any();
		}

        public WcsOutboundPickOrdData GetWcsOutboundPickOrdData(string dcCode, string gupCode, string custCode, string pickOrdNo)
        {
					var parms = new List<SqlParameter>
								{
										new SqlParameter("@p0", SqlDbType.VarChar){Value = dcCode },
										new SqlParameter("@p1", SqlDbType.VarChar){Value = gupCode },
										new SqlParameter("@p2", SqlDbType.VarChar){ Value = custCode },
										new SqlParameter("@p3", SqlDbType.VarChar){ Value = pickOrdNo },
								};
            var sql = $@" SELECT 
                            A.PICK_ORD_NO PickOrdNo,
                            B.SOURCE_TYPE SourceType,
                            B.CUST_COST CustCost,
                            B.FAST_DEAL_TYPE FastDealType,
                            A.PICK_TYPE PickType,
                            C.NAME NextStepName,
                            A.NEXT_STEP NextStep,
                            A.MOVE_OUT_TARGET MoveOutTarget,
						                A.CONTAINER_TYPE ContainerType,
						                A.PACKING_TYPE PackingType,
                            A.SPLIT_CODE SplitCode,
                            A.PRIORITY_VALUE PriorityValue,
                            A.PICK_STATUS PickStatus,
                            A.RTN_VNR_CODE RtnVnrCode,
                            A.NP_FLAG NpFlag
                         FROM 
                            F051201 A,F0513 B,VW_F000904_LANG C
                         WHERE A.DC_CODE = B.DC_CODE
                            AND A.DC_CODE = B.DC_CODE
                            AND A.GUP_CODE = B.GUP_CODE
                            AND A.DELV_DATE = B.DELV_DATE
                            AND A.PICK_TIME = B.PICK_TIME
                            AND C.TOPIC = 'F051201'
                            AND C.SUBTOPIC = 'NEXT_STEP'
                            AND C.LANG = '{Current.Lang}'
                            AND C.VALUE = A.NEXT_STEP
                            AND A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND A.PICK_ORD_NO = @p3
                         ";
            return SqlQuery<WcsOutboundPickOrdData>(sql, parms.ToArray()).FirstOrDefault();
        }

        public ContainerSingleByPick GetContainerSingleByPick(string dcCode, string gupCode, string custCode, string pickOrdNo)
        {
            var parm = new List<SqlParameter>
            {
                new SqlParameter("@p0", Current.Lang),
                new SqlParameter("@p1", dcCode),
                new SqlParameter("@p2", gupCode),
                new SqlParameter("@p3", custCode),
                new SqlParameter("@p4", pickOrdNo)
            };
            var sql = $@" SELECT
                            P.DC_CODE,
                            P.GUP_CODE,
                            P.CUST_CODE,
                            P.PICK_TYPE,
                            P.MERGE_NO,
                            P.PICK_ORD_NO,
                            P.NEXT_STEP,
                            L.NAME NEXT_STEP_NAME,
                            M.NAME PICK_STATUS_NAME
                            FROM F051201 P
                            LEFT JOIN VW_F000904_LANG L
                            ON L.TOPIC='F051201'
                            AND L.SUBTOPIC='NEXT_STEP'
                            AND L.LANG=@p0
                            AND L.VALUE = P.NEXT_STEP
                            LEFT JOIN VW_F000904_LANG M
                            ON M.TOPIC='F051201' 
                            AND M.SUBTOPIC = 'PICK_STATUS'
                            AND M.LANG=@p0 
                            AND M.VALUE = P.PICK_STATUS
                            WHERE P.DC_CODE = @p1
                            AND P.GUP_CODE = @p2  
                            AND P.CUST_CODE = @p3  
                            AND P.PICK_ORD_NO = @p4
                         ";

            return SqlQuery<ContainerSingleByPick>(sql, parm.ToArray()).FirstOrDefault();
        }
    

		// 揀貨單資料
		public IQueryable<F051201WithF051202> GetF051201WithF051202s(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var param = new object[] { dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @"SELECT  DISTINCT A.PICK_ORD_NO ,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC ='PICK_STATUS' AND VALUE = A.PICK_STATUS) PICK_STATUS,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC='PICK_TYPE' AND VALUE=A.PICK_TYPE) PICK_TYPE,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC='PICK_TOOL' AND VALUE=A.PICK_TOOL) PICK_TOOL,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC='NEXT_STEP' AND VALUE=A.NEXT_STEP) NEXT_STEP,
						A.PICK_START_TIME,
						A.PICK_FINISH_DATE,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC='CONTAINER_TYPE' AND VALUE=A.CONTAINER_TYPE) CONTAINER_TYPE_NAME,
						(SELECT NAME FROM F000904 WHERE TOPIC = 'F051201' AND SUBTOPIC = 'DISP_SYSTEM' AND VALUE = A.DISP_SYSTEM) DISP_SYSTEM,
						A.PICK_NAME,
            A.PRIORITY_VALUE
						FROM F051201 A 
						JOIN F051202 B 
						ON A.DC_CODE =B.DC_CODE 
						AND A.GUP_CODE =B.GUP_CODE 
						AND A.CUST_CODE = B.CUST_CODE 
						AND A.PICK_ORD_NO = B.PICK_ORD_NO 
						WHERE A.DC_CODE = @p0
						AND A.GUP_CODE = @p1
						AND A.CUST_CODE = @p2
						AND B.WMS_ORD_NO =@p3";

			return SqlQuery<F051201WithF051202>(sql, param); 
		}

		public F051201 GetData(string dcCode,string gupCode,string custCode,string pickOrdNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",pickOrdNo){ SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT *
                     FROM F051201 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO = @p3 ";
			return SqlQuery<F051201>(sql, parms.ToArray()).FirstOrDefault();
		}
	}
}
