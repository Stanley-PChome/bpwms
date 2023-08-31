
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051206Repository : RepositoryBase<F051206, Wms3plDbContext, F051206Repository>
	{
		/// <summary>
		/// 取得缺貨處理的主查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="status"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public IQueryable<F051206Pick> GetGetF051206PicksByQuery(string dcCode, string gupCode, string custCode, DateTime delvDateStart, DateTime delvDateEnd, string status, string pickOrdNo, string wmsOrdNo, string containerCode,string crtOrUpdOpertor)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDateStart, delvDateEnd };

			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A1.PICK_ORD_NO, B.WMS_ORD_NO ASC) ROWNUM,
							   A1.*,
							   B.DELV_DATE,
							   B.PICK_TIME
						  FROM (  SELECT A.DC_CODE,
										 A.GUP_CODE,
										 A.CUST_CODE,
										 A.PICK_ORD_NO,
										 A.WMS_ORD_NO,
										 A.STATUS,
										 A.CRT_STAFF,
										 A.CRT_DATE,
										 A.UPD_STAFF,
										 A.UPD_DATE,
										 A.CRT_NAME,
										 A.UPD_NAME
									FROM F051206 A
								   WHERE     A.ISDELETED = '0'
										 AND A.DC_CODE = @p0
										 AND A.GUP_CODE = @p1
										 AND A.CUST_CODE = @p2
								GROUP BY A.DC_CODE,
										 A.GUP_CODE,
										 A.CUST_CODE,
										 A.PICK_ORD_NO,
										 A.WMS_ORD_NO,
										 A.STATUS,
										 A.CRT_STAFF,
										 A.CRT_DATE,
										 A.UPD_STAFF,
										 A.UPD_DATE,
                     A.CRT_NAME,
                     A.UPD_NAME) A1
							   JOIN F050801 B
								  ON   A1.WMS_ORD_NO = B.WMS_ORD_NO
									 AND A1.DC_CODE = B.DC_CODE
									 AND A1.GUP_CODE = B.GUP_CODE
									 AND A1.CUST_CODE = B.CUST_CODE
						 WHERE     B.DELV_DATE >= @p3
							   AND B.DELV_DATE <= @p4";

			sql += paramList.CombineNotNullOrEmpty(" AND A1.STATUS = @p{0} ", status);
			sql += paramList.CombineNotNullOrEmpty(" AND A1.PICK_ORD_NO = @p{0} ", pickOrdNo);
			sql += paramList.CombineNotNullOrEmpty(" AND A1.WMS_ORD_NO = @p{0} ", wmsOrdNo);
			sql += paramList.CombineNotNullOrEmpty(" AND (A1.CRT_STAFF = @p{0} OR A1.UPD_STAFF =@p{0})", crtOrUpdOpertor);
			sql += paramList.CombineNotNullOrEmpty(@" AND Exists 
					(Select C.ID From F070101 C
					   Join F0701 D On C.F0701_ID=D.ID
					  Where C.DC_CODE=A1.DC_CODE And C.GUP_CODE=A1.GUP_CODE And C.CUST_CODE=A1.CUST_CODE
					    And (C.WMS_NO=A1.WMS_ORD_NO Or C.WMS_NO=A1.PICK_ORD_NO)
					    And D.CONTAINER_CODE=@p{0}) ", containerCode);
			sql += " ORDER BY A1.PICK_ORD_NO, B.WMS_ORD_NO";

			var result = SqlQuery<F051206Pick>(sql, paramList.ToArray());

			return result;
		}

		/// <summary>
		/// 取得要新增缺貨處理的主查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public IQueryable<F051206Pick> GetGetF051206PicksByAdd(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string pickOrdNo)
		{
			var paramList = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime };

			var sql = @"
					  SELECT  ROW_NUMBER()OVER(ORDER BY B.PICK_ORD_NO, A.WMS_ORD_NO ASC) ROWNUM,
                    A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.WMS_ORD_NO,
                    B.PICK_ORD_NO,MIN(B.PICK_STATUS) STATUS,
                    B.PICK_LOC
				    FROM F050801 A
           INNER JOIN F051202 B 
              ON B.DC_CODE = A.DC_CODE
             AND B.GUP_CODE = A.GUP_CODE
             AND B.CUST_CODE = A.CUST_CODE
             AND B.WMS_ORD_NO = A.WMS_ORD_NO
					 WHERE A.DC_CODE = @p0
						 AND A.GUP_CODE = @p1
						 AND A.CUST_CODE = @p2
						 AND A.DELV_DATE = @p3
						 AND A.PICK_TIME = @p4
             AND A.STATUS ='0' --只顯示出貨單未包裝資料才可設定揀缺
             AND NOT EXISTS(  --排除訂單已取消
                  SELECT 1 
                    FROM F05030101 C
                    INNER JOIN F050301 D
                      ON D.DC_CODE = C.DC_CODE
                      AND D.GUP_CODE = C.GUP_CODE
                      AND D.CUST_CODE = C.CUST_CODE
                      AND D.ORD_NO = C.ORD_NO
                    WHERE C.DC_CODE = A.DC_CODE
                      AND C.GUP_CODE = A.GUP_CODE
                      AND C.CUST_CODE = A.CUST_CODE
                      AND C.WMS_ORD_NO = A.WMS_ORD_NO
                      AND D.PROC_FLAG ='9')
							";
			sql += paramList.CombineNotNullOrEmpty(" AND A.PICK_ORD_NO = @p{0} ", pickOrdNo);
			sql += " GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,A.WMS_ORD_NO,B.PICK_ORD_NO, B.PICK_LOC ";
			sql += " HAVING MIN(B.PICK_STATUS) = 1 "; //出貨單全部揀貨完成
			sql += " ORDER BY B.PICK_ORD_NO, A.WMS_ORD_NO";

			var result = SqlQuery<F051206Pick>(sql, paramList.ToArray());

			return result;
		}

		public IQueryable<F051206AllocationList> GetF051206AllocationLists(string dcCode, string gupCode, string custCode, string editType, string status, string allocation_no)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode),
								new SqlParameter("@p2", dcCode),
						};
			StringBuilder strCmd = new StringBuilder();
			if (editType == "ADD")
			{
				strCmd.AppendLine("SELECT ROW_NUMBER()OVER(ORDER BY ALLOCATION_NO ASC) ROWNUM,TAR_DC_CODE,ALLOCATION_NO,STATUS  ");
				strCmd.AppendLine("FROM F151001 ");
				strCmd.AppendLine("WHERE  GUP_CODE = @p0 ");
				strCmd.AppendLine("AND CUST_CODE = @p1 ");
				strCmd.AppendLine("AND DC_CODE = @p2 ");
				strCmd.AppendLine("AND  STATUS IN ('0','1') ");
				if (!string.IsNullOrWhiteSpace(allocation_no))
				{
					strCmd.AppendLine("AND ALLOCATION_NO = @p4 ");
					parameters.Add(new SqlParameter("@p4", allocation_no));
				}
				strCmd.AppendLine("ORDER BY ALLOCATION_NO DESC");
			}
			else
			{
				strCmd.AppendLine("SELECT DISTINCT B.TAR_DC_CODE,A.ALLOCATION_NO,A.STATUS  ");
				strCmd.AppendLine("FROM F151003 A JOIN F151001 B ON A.ALLOCATION_NO=B.ALLOCATION_NO ");
				strCmd.AppendLine("WHERE  A.GUP_CODE = @p0 ");
				strCmd.AppendLine("AND A.CUST_CODE = @p1 ");
				strCmd.AppendLine("AND A.DC_CODE = @p2 ");
				strCmd.AppendLine("AND A.STATUS !='9' ");
				if (!string.IsNullOrWhiteSpace(status))
				{
					strCmd.AppendLine("	       AND A.STATUS =@p3  ");
					parameters.Add(new SqlParameter("@p3", status));
				}
				if (!string.IsNullOrWhiteSpace(allocation_no))
				{
					strCmd.AppendLine("AND A.ALLOCATION_NO = @p4 ");
					parameters.Add(new SqlParameter("@p4", allocation_no));
				}
				strCmd.AppendLine("ORDER BY A.ALLOCATION_NO DESC");

			}

			var result = SqlQuery<F051206AllocationList>(strCmd.ToString(), parameters.ToArray());

			return result;
			#region 舊
			//StringBuilder strCmd = new StringBuilder();
			//strCmd.AppendLine("	SELECT ROWNUM,	 ");
			//strCmd.AppendLine("	       P.DC_CODE,	 ");
			//strCmd.AppendLine("	       P.AllocationNO,	 ");
			//strCmd.AppendLine("	       L.STATUS	 ");
			//strCmd.AppendLine("	  FROM F051201 P	 ");
			//strCmd.AppendLine("	       JOIN F050801 W	 ");
			//strCmd.AppendLine("	          ON     W.PICK_ORD_NO = P.PICK_ORD_NO	 ");
			//strCmd.AppendLine("	             AND W.GUP_CODE = P.GUP_CODE	 ");
			//strCmd.AppendLine("	             AND W.CUST_CODE = P.CUST_CODE	 ");
			//strCmd.AppendLine("	             AND W.DC_CODE = P.DC_CODE	 ");
			//strCmd.AppendLine("	             AND W.DELV_DATE = P.DELV_DATE	 ");
			//strCmd.AppendLine("	             AND W.PICK_TIME = P.PICK_TIME	 ");
			//strCmd.AppendLine("	       LEFT JOIN (  SELECT PICK_ORD_NO,	 ");
			//strCmd.AppendLine("	                           GUP_CODE,	 ");
			//strCmd.AppendLine("	                           CUST_CODE,	 ");
			//strCmd.AppendLine("	                           DC_CODE,	 ");
			//strCmd.AppendLine("	                           STATUS	 ");
			//strCmd.AppendLine("	                      FROM F051206	 ");
			//strCmd.AppendLine("	                  GROUP BY PICK_ORD_NO,	 ");
			//strCmd.AppendLine("	                           GUP_CODE,	 ");
			//strCmd.AppendLine("	                           CUST_CODE,	 ");
			//strCmd.AppendLine("	                           DC_CODE,	 ");
			//strCmd.AppendLine("	                           STATUS) L	 ");
			//strCmd.AppendLine("	          ON     L.PICK_ORD_NO = P.PICK_ORD_NO	 ");
			//strCmd.AppendLine("	             AND L.GUP_CODE = P.GUP_CODE	 ");
			//strCmd.AppendLine("	             AND L.CUST_CODE = P.CUST_CODE	 ");
			//strCmd.AppendLine("	             AND L.DC_CODE = P.DC_CODE	 ");
			//strCmd.AppendLine("	 WHERE     P.GUP_CODE = :p0	 ");
			//strCmd.AppendLine("	       AND P.CUST_CODE = :p1	 ");
			//strCmd.AppendLine("	       AND P.DC_CODE = :p2	 ");
			//strCmd.AppendLine("	       AND P.DELV_DATE = TO_DATE(:p3, 'yyyy/mm/dd')	 ");
			//strCmd.AppendLine("	       AND P.PICK_TIME = :p4	 ");
			//if (editType == "ADD")
			//	strCmd.AppendLine("	       AND L.STATUS IS NULL	 ");
			//else
			//	strCmd.AppendLine("	       AND L.STATUS IS NOT NULL	 ");

			//return SqlQuery<F051206AllocationList>(strCmd.ToString(), parameters.ToArray());
			#endregion
		}

		public IQueryable<F051206LackList> GetF051206LackLists(string dcCode, string gupCode, string custCode, string pickOrdNo, string wmsOrdNo, string editType)
		{
      var parameters = new List<SqlParameter>
            {
              new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p4", wmsOrdNo) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p5", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
						};

			var sql = string.Empty;
			//strCmd.AppendLine("	SELECT ROWNUM,0 AS IsUpdate, I.ITEM_NAME, L.*	 ");
			if (editType == "ADD")
			{
				sql = @" SELECT ROW_NUMBER()OVER(ORDER BY L.WMS_ORD_NO, L.CUST_CODE, L.GUP_CODE, L.DC_CODE ASC) ROWNUM, 0 IsUpdate,D.ITEM_NAME,L.*
									FROM(
									SELECT 0 AS LACK_SEQ,A.WMS_ORD_NO,B.PICK_ORD_NO,B.PICK_ORD_SEQ,B.ITEM_CODE,B.B_PICK_QTY,0 AS LACK_QTY,'999' AS REASON,
												 '' AS MEMO,'0' AS STATUS,'' AS RETURN_FLAG,A.CUST_CODE,A.GUP_CODE,A.DC_CODE,
												 '' AS CRT_STAFF,'' AS CRT_NAME,@p5 AS CRT_DATE,'' AS UPD_STAFF,@p5 AS UPD_DATE,'' AS UPD_NAME,'0' AS ISDELETED,C.CUST_ORD_NO ,C.ORD_NO,B.PICK_LOC LOC_CODE,B.SERIAL_NO
									FROM F050801 A
									JOIN F051202 B
									ON B.DC_CODE = A.DC_CODE
									AND B.GUP_CODE = A.GUP_CODE
									AND B.CUST_CODE = A.CUST_CODE
									AND B.WMS_ORD_NO = A.WMS_ORD_NO
									JOIN(
									SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,STRING_AGG(C.CUST_ORD_NO,',') WITHIN GROUP(ORDER BY C.ORD_NO) AS CUST_ORD_NO,STRING_AGG(C.ORD_NO,',') WITHIN GROUP(ORDER BY C.ORD_NO) AS ORD_NO
									FROM F050801 A
									JOIN F05030101 B
									ON B.DC_CODE = A.DC_CODE
									AND B.GUP_CODE = A.GUP_CODE
									AND B.CUST_CODE = A.CUST_CODE
									AND B.WMS_ORD_NO = A.WMS_ORD_NO
									JOIN F050301 C
									ON C.DC_CODE = B.DC_CODE
									AND C.GUP_CODE = B.GUP_CODE
									AND C.CUST_CODE= B.CUST_CODE
									AND C.ORD_NO = B.ORD_NO
									GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO) C
									ON C.DC_CODE = A.DC_CODE
									AND C.GUP_CODE = A.GUP_CODE
									AND C.CUST_CODE = A.CUST_CODE
									AND C.WMS_ORD_NO = A.WMS_ORD_NO
									) L
									JOIN F1903 D
									ON D.GUP_CODE = L.GUP_CODE
									AND D.CUST_CODE = L.CUST_CODE
									AND D.ITEM_CODE = L.ITEM_CODE
									WHERE L.DC_CODE = @p0
										AND L.GUP_CODE = @p1
										AND L.CUST_CODE = @p2
										AND L.PICK_ORD_NO = @p3
										AND L.WMS_ORD_NO = @p4";

			}
			else
			{
				sql = @" SELECT ROW_NUMBER()OVER(ORDER BY L.LACK_SEQ ASC) ROWNUM, 0 IsUpdate,D.ITEM_NAME,L.*
									FROM(
									SELECT A.LACK_SEQ,A.WMS_ORD_NO,B.PICK_ORD_NO,B.PICK_ORD_SEQ,A.ITEM_CODE,B.B_PICK_QTY,A.LACK_QTY,A.REASON,
												 A.MEMO,A.STATUS,A.RETURN_FLAG,A.CUST_CODE,A.GUP_CODE,A.DC_CODE,
												 A.CRT_STAFF,A.CRT_NAME,A.CRT_DATE,A.UPD_STAFF,A.UPD_DATE,A.UPD_NAME,A.ISDELETED,C.CUST_ORD_NO ,C.ORD_NO,A.LOC_CODE,A.SERIAL_NO
									FROM F051206 A
									JOIN F051202 B
									ON B.DC_CODE = A.DC_CODE
									AND B.GUP_CODE = A.GUP_CODE
									AND B.CUST_CODE = A.CUST_CODE
									AND B.PICK_ORD_NO = A.PICK_ORD_NO
									AND B.PICK_ORD_SEQ = A.PICK_ORD_SEQ
									JOIN(
									SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,STRING_AGG(C.CUST_ORD_NO,',') WITHIN GROUP(ORDER BY C.ORD_NO) AS CUST_ORD_NO,STRING_AGG(C.ORD_NO,',') WITHIN GROUP(ORDER BY C.ORD_NO) AS ORD_NO
									FROM F050801 A
									JOIN F05030101 B
									ON B.DC_CODE = A.DC_CODE
									AND B.GUP_CODE = A.GUP_CODE
									AND B.CUST_CODE = A.CUST_CODE
									AND B.WMS_ORD_NO = A.WMS_ORD_NO
									JOIN F050301 C
									ON C.DC_CODE = B.DC_CODE
									AND C.GUP_CODE = B.GUP_CODE
									AND C.CUST_CODE= B.CUST_CODE
									AND C.ORD_NO = B.ORD_NO
									GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO) C
									ON C.DC_CODE = A.DC_CODE
									AND C.GUP_CODE = A.GUP_CODE
									AND C.CUST_CODE = A.CUST_CODE
									AND C.WMS_ORD_NO = A.WMS_ORD_NO
									) L
									JOIN F1903 D
									ON D.GUP_CODE = L.GUP_CODE
									AND D.CUST_CODE = L.CUST_CODE
									AND D.ITEM_CODE = L.ITEM_CODE
									WHERE L.ISDELETED <>'1'
										AND L.DC_CODE = @p0
										AND L.GUP_CODE = @p1
										AND L.CUST_CODE = @p2
										AND L.PICK_ORD_NO = @p3
										AND L.WMS_ORD_NO = @p4 ";
			}

			var result = SqlQuery<F051206LackList>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F051206LackList_Allot> GetF051206LackLists_Allot(string dcCode, string gupCode, string custCode, string allocationNo, string editType, string status)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode),
								new SqlParameter("@p2", dcCode),
								new SqlParameter("@p3", allocationNo),
								new SqlParameter("@p4", status)
						};
			StringBuilder strCmd = new StringBuilder();

			string commandStr = @"      JOIN	                                      
                                           F1903 I	                              
                                        ON P.ITEM_CODE = I.ITEM_CODE 	          
                                        AND P.GUP_CODE = I.GUP_CODE	              
                                        AND P.CUST_CODE = I.CUST_CODE	          
                                  WHERE P.GUP_CODE = @p0 	                      
                                              AND P.CUST_CODE = @p1 	              
                                              AND P.DC_CODE = @p2 	              
                                              AND P.ALLOCATION_NO = @p3 	          
                                              AND P.STATUS !='9' 	              
                                              AND P.STATUS = @p4";

			if (editType == "ADD")
			{
				strCmd.AppendLine("	SELECT ROW_NUMBER()OVER(ORDER BY P.ALLOCATION_SEQ ASC) ROWNUM, P.ALLOCATION_NO,P.ALLOCATION_SEQ,I.ITEM_NAME, ");
				strCmd.AppendLine("	       P.ITEM_CODE,      0 AS LACK_QTY,       P.SRC_QTY,	 ");
				strCmd.AppendLine("	      '999' AS REASON,	 ");
				strCmd.AppendLine("	       '' AS MEMO,       '0' AS STATUS,  ");
				strCmd.AppendLine("	       P.CUST_CODE,       P.GUP_CODE,       P.DC_CODE	 ,'0' AS ISDELETED, ");
				strCmd.AppendLine("	       '調撥下架' LACK_TYPE, 	 ");
				strCmd.AppendLine("	       P.SRC_LOC_CODE, P.TAR_LOC_CODE, P.SERIAL_NO, P.VALID_DATE, P.MAKE_NO, P.ENTER_DATE, P.VNR_CODE, P.PALLET_CTRL_NO, P.BOX_CTRL_NO 	 ");
				strCmd.AppendLine("	  FROM F151002 P	 ");
				strCmd.AppendLine("	  {0}	 ");
			}
			else
			{
				strCmd.AppendLine("	SELECT ROW_NUMBER()OVER(ORDER BY P.LACK_SEQ ASC) ROWNUM,P.LACK_SEQ,I.ITEM_NAME, ");
				strCmd.AppendLine("	       P.ITEM_CODE,      P.LACK_QTY,       P.MOVE_QTY AS SRC_QTY,	 ");
				strCmd.AppendLine("	       P.REASON,	 P.MEMO,       P.STATUS,  ");
				strCmd.AppendLine("	       P.CUST_CODE,       P.GUP_CODE,       P.DC_CODE,	 ");
				strCmd.AppendLine("	       CASE WHEN P.LACK_TYPE IS NULL OR P.LACK_TYPE = '' THEN '調撥下架' ELSE  	 ");
				strCmd.AppendLine("	       CASE WHEN P.LACK_TYPE = '0' THEN '調撥下架' ELSE '調撥上架' END 	 ");
				strCmd.AppendLine("	       END LACK_TYPE, 	 ");
				strCmd.AppendLine("	       A.SRC_LOC_CODE, A.TAR_LOC_CODE, A.SERIAL_NO, A.VALID_DATE, A.MAKE_NO, A.ENTER_DATE, A.VNR_CODE, A.PALLET_CTRL_NO, A.BOX_CTRL_NO 	 ");
				strCmd.AppendLine("	  , P.CRT_DATE, P.CRT_NAME, P.UPD_DATE, P.UPD_NAME ");
				strCmd.AppendLine("	  FROM F151002 A ");
				strCmd.AppendLine("	  JOIN F151003 P ");
				strCmd.AppendLine("	  ON P.DC_CODE = A.DC_CODE ");
				strCmd.AppendLine("	  AND P.GUP_CODE = A.GUP_CODE ");
				strCmd.AppendLine("	  AND P.CUST_CODE = A.CUST_CODE ");
				strCmd.AppendLine("	  AND P.ALLOCATION_NO = A.ALLOCATION_NO ");
				strCmd.AppendLine("	  AND P.ALLOCATION_SEQ = A.ALLOCATION_SEQ  ");
				strCmd.AppendLine("	  {0}	 ");
			}

			var QList = SqlQuery<F051206LackList_Allot>(string.Format(strCmd.ToString(), commandStr), parameters.ToArray());
			return QList;
		}

		public IQueryable<P060202Data> GetP060202Datas(string dcCode, string gupCode, string custCode, DateTime pickSDate, DateTime pickEDate, string warehouseId, string itemCode)
		{
			var param = new List<object> { dcCode, gupCode, custCode, pickSDate, pickEDate };
			var filterSql = string.Empty;
			if (string.IsNullOrWhiteSpace(warehouseId))
			{
				filterSql += @" AND EXISTS (
												SELECT 1
												FROM F196301 H
												JOIN F1963 I
												ON I.WORK_ID = H.WORK_ID
												JOIN F192403 J
												ON J.WORK_ID = I.WORK_ID
												JOIN F1924 K ON K.EMP_ID = J.EMP_ID
												WHERE H.DC_CODE = A.DC_CODE
												AND H.LOC_CODE = A.LOC_CODE
												AND K.EMP_ID = @p" + param.Count + ") ";
				param.Add(Current.Staff);
			}
			else
			{
				filterSql += " AND D.WAREHOUSE_ID = @p" + param.Count;
				param.Add(warehouseId);
			}

			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				filterSql += " AND A.ITEM_CODE = @p" + param.Count;
				param.Add(itemCode);
			}

			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.WAREHOUSE_ID,A.PICK_LOC,A.ITEM_CODE ASC) ROWNUM,A.*
                      FROM (
										SELECT TOP 100 PERCENT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,C.DELV_DATE,E.WAREHOUSE_ID,E.WAREHOUSE_NAME,B.PICK_LOC,B.ITEM_CODE,F.ITEM_NAME,ISNULL(G.QTY,0) PICK_STOCK_QTY,SUM(A.LACK_QTY) LACK_QTY
											FROM F051206 A
											JOIN F051202 B
											ON B.DC_CODE = A.DC_CODE
											AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.PICK_ORD_NO = A.PICK_ORD_NO
											AND B.PICK_ORD_SEQ = A.PICK_ORD_SEQ
											JOIN F051201 C
											ON C.DC_CODE = B.DC_CODE
											AND C.GUP_CODE = B.GUP_CODE
											AND C.CUST_CODE = B.CUST_CODE
											AND C.PICK_ORD_NO = B.PICK_ORD_NO
											JOIN F1912 D
											ON D.DC_CODE = B.DC_CODE
											AND D.LOC_CODE = B.PICK_LOC
											JOIN F1980 E
											ON E.DC_CODE = D.DC_CODE
											AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
											LEFT JOIN F1903 F
											ON F.GUP_CODE = B.GUP_CODE
											AND F.CUST_CODE = B.CUST_CODE
											AND F.ITEM_CODE = B.ITEM_CODE
											LEFT JOIN (
												SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,SUM(A.QTY) QTY
													FROM F1913 A
													JOIN F1912 B 
														ON B.DC_CODE = A.DC_CODE
													 AND B.LOC_CODE = A.LOC_CODE
													JOIN F1919 C
														ON C.DC_CODE = B.DC_CODE
													 AND C.AREA_CODE = B.AREA_CODE
												 WHERE C.ATYPE_CODE = 'A'
												 GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE
											)G
											ON G.DC_CODE = B.DC_CODE
											AND G.GUP_CODE = B.GUP_CODE
											AND G.CUST_CODE = B.CUST_CODE
											AND G.LOC_CODE = B.PICK_LOC
											AND G.ITEM_CODE = B.ITEM_CODE
											WHERE A.DC_CODE = @p0
											  AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND C.DELV_DATE >= @p3 
                        AND C.DELV_DATE <= @p4
                        AND A.STATUS IN('0','1')
                        AND ISNULL(A.TRANS_FLAG,'0') = '0'
											{filterSql}
											GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,C.DELV_DATE,E.WAREHOUSE_ID,E.WAREHOUSE_NAME,B.PICK_LOC,B.ITEM_CODE,F.ITEM_NAME,G.QTY 
                      ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,C.DELV_DATE,E.WAREHOUSE_ID,B.PICK_LOC,B.ITEM_CODE) A ";

			var result = SqlQuery<P060202Data>(sql, param.ToArray());

			return result;
		}

		public IQueryable<P060202TransferData> GetP060202TransferDatas(string dcCode, string gupCode, string custCode, List<DateTime> delvDates, List<string> locCodes, List<string> itemCodes)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,C.DELV_DATE,D.WAREHOUSE_ID,B.PICK_LOC,B.ITEM_CODE,
													B.VALID_DATE,B.ENTER_DATE,B.MAKE_NO,B.VNR_CODE,B.BOX_CTRL_NO,B.PALLET_CTRL_NO,B.SERIAL_NO,A.LACK_QTY,A.LACK_SEQ
										FROM F051206 A
										JOIN F051202 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.PICK_ORD_NO = A.PICK_ORD_NO
										AND B.PICK_ORD_SEQ = A.PICK_ORD_SEQ
										JOIN F051201 C
										ON C.DC_CODE = B.DC_CODE
										AND C.GUP_CODE = B.GUP_CODE
										AND C.CUST_CODE = B.CUST_CODE
										AND C.PICK_ORD_NO = B.PICK_ORD_NO
										JOIN F1912 D
										ON D.DC_CODE = B.DC_CODE
										AND D.LOC_CODE = B.PICK_LOC
										WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2
                    AND A.STATUS IN('0','1')
                    AND ISNULL(A.TRANS_FLAG,'0') = '0'";
			sql += parms.CombineSqlInParameters(" AND C.DELV_DATE ", delvDates);
			sql += parms.CombineSqlInParameters(" AND B.PICK_LOC ", locCodes);
			sql += parms.CombineSqlInParameters(" AND B.ITEM_CODE ", itemCodes);

			var result = SqlQuery<P060202TransferData>(sql, parms.ToArray());

			return result;
		}

		public void UpdateTransferByLackSeq(string allocationNo, int allocationSeq, List<decimal> LackSeqs)
		{
			if (LackSeqs.Any())
			{
				var parms = new List<object> { allocationNo, allocationSeq, DateTime.Now, Current.Staff, Current.StaffName, DateTime.Now, Current.Staff, Current.StaffName };

				var sql = @" UPDATE F051206 
                      SET ALLOCATION_NO = @p0, ALLOCATION_SEQ = @p1,UPD_DATE = @p2,UPD_STAFF = @p3,UPD_NAME =@p4,
													TRANS_FLAG ='1',TRANS_DATE = @p5,TRANS_STAFF = @p6,TRANS_NAME = @p7,
                          RETURN_FLAG ='4',STATUS ='2' 
                   WHERE 1 = 1 ";

				sql += parms.CombineSqlInParameters(" AND LACK_SEQ ", LackSeqs);

				ExecuteSqlCommand(sql, parms.ToArray());
			}
		}

		public IQueryable<F051206> GetCollectionOutboundDatas()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F051206 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_ORD_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									) AND A.STATUS <> '2' -- 已確認
									";

			var result = SqlQuery<F051206>(sql, parms.ToArray());
			return result;
		}

		/// <summary>
		/// 找到商品
		/// </summary>
		/// <returns></returns>
		public IQueryable<F051206> GetCollectionOutboundDatasByFindItem()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F051206 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_ORD_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									) AND A.STATUS = '2' -- 已確認
										AND A.RETURN_FLAG = '3' -- 找到商品
									";

			var result = SqlQuery<F051206>(sql, parms.ToArray());
			return result;
		}

		public IQueryable<F051206> GetNotDeleteDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @" SELECT *
                     FROM F051206
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND WMS_ORD_NO = @p3
                      AND ISDELETED ='0'";
			return SqlQuery<F051206>(sql, parms.ToArray());
		}

		public IQueryable<F051206> GetDatasByOrderCancel(string dcCode, string gupCode, string custCode, List<string> wmsOrdNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT *
                     FROM F051206
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
											AND (STATUS = '0' OR STATUS = '1') --0(揀缺待確認)或1(貨主待確認)
                      AND ISDELETED ='0' --未刪除
									";
			sql += parms.CombineSqlInParameters(" AND WMS_ORD_NO ", wmsOrdNo);
			return SqlQuery<F051206>(sql, parms.ToArray());
		}

    public IQueryable<F051206> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
      var sql = @" SELECT *
                     FROM F051206
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND WMS_ORD_NO = @p3
                      AND ISDELETED ='0' 
                      AND RETURN_FLAG NOT IN ('3') ";
      return SqlQuery<F051206>(sql, parms.ToArray());
    }

    public IQueryable<F051206> GetDatasByPickOrdNo(string dcCode, string gupCode, string custCode, string pickNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", pickNo) { SqlDbType = System.Data.SqlDbType.VarChar },
      };

      var sql = @"SELECT * FROM F051206 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND PICK_ORD_NO=@p3 AND ISDELETED='0'";
      var result = SqlQuery<F051206>(sql, param.ToArray());
      return result;
    }

    public IQueryable<F051206> GetDatasByLackSeq( List<int> LackSeqs)
    {
      var param = new List<SqlParameter>();

      if (LackSeqs == null || !LackSeqs.Any())
        return null;

      var sql = $@"SELECT * FROM F051206 WHERE {param.CombineSqlInParameters("LACK_SEQ", LackSeqs, SqlDbType.Int)}";
      var result = SqlQuery<F051206>(sql, param.ToArray());
      return result;
		}

		public IQueryable<F051206LackData> GetF051206LackDatas(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)   { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode)  { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
			};
			var sql = @" SELECT A.*, B.WMS_ORD_SEQ
										FROM F051206 A
										JOIN F051202 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.PICK_ORD_NO = A.PICK_ORD_NO
										AND B.PICK_ORD_SEQ = A.PICK_ORD_SEQ
										WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2
                    AND A.STATUS = '2'
                    AND A.RETURN_FLAG = '1' ";
			sql += param.CombineSqlInParameters("AND A.WMS_ORD_NO", wmsOrdNos, SqlDbType.VarChar);

			var result = SqlQuery<F051206LackData>(sql, param.ToArray());

			return result;
		}
	}
}
