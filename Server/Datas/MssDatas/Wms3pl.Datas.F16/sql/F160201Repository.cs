using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F160201Repository : RepositoryBase<F160201, Wms3plDbContext, F160201Repository>
	{





        public IQueryable<F160201Data> GetF160201Datas(string dcCode, string gupCode, string custCode, string status,
            DateTime createBeginDateTime, DateTime createEndDateTime, string postingBeginDateTime, string postingEndDateTime,
            string returnNo, string custOrdNo, string vendorCode, string vendorName)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", createBeginDateTime),
                new SqlParameter("@p4", createEndDateTime)
            };
            var sql = $@"SELECT	ROW_NUMBER()OVER(ORDER BY A.RTN_VNR_NO ASC) ROWNUM,A.RTN_VNR_NO,A.CUST_CODE,A.GUP_CODE,A.DC_CODE,A.RTN_VNR_DATE,A.STATUS,	
								A.ORD_PROP,A.RTN_VNR_TYPE_ID,A.RTN_VNR_CAUSE,A.SELF_TAKE,
								A.VNR_CODE,A.COST_CENTER,A.MEMO,A.POSTING_DATE,A.CUST_ORD_NO,
								B.VNR_NAME,A.CRT_STAFF,A.CRT_DATE,A.UPD_STAFF,A.UPD_DATE,
								A.CRT_NAME,A.UPD_NAME,
								(SELECT NAME FROM VW_F000904_LANG  WHERE TOPIC='F160201' AND SUBTOPIC='STATUS' AND VALUE = A.STATUS AND LANG = '{Current.Lang}') AS STATUS_TEXT,
								(SELECT ORD_PROP_NAME FROM F000903 WHERE ORD_PROP = A.ORD_PROP) AS ORD_PROP_TEXT,
								(SELECT RTN_VNR_TYPE_NAME FROM F160203 WHERE RTN_VNR_TYPE_ID = A.RTN_VNR_TYPE_ID) AS RTN_VNR_TYPE_TEXT,
								(SELECT CAUSE FROM F1951 WHERE UCC_CODE = A.RTN_VNR_CAUSE AND UCT_ID='RV') AS RTN_VNR_CAUSE_TEXT,
                                A.ADDRESS,
								A.ITEM_CONTACT,
								A.ITEM_TEL,A.DELIVERY_WAY,A.TYPE_ID 
						FROM F160201 A, 
							 F1908 B
						WHERE A.VNR_CODE = B.VNR_CODE AND
							  B.GUP_CODE = A.GUP_CODE AND
							  B.CUST_CODE = A.CUST_CODE AND
							  A.DC_CODE = @p0 AND
							  A.GUP_CODE = @p1 AND 
			                  A.CUST_CODE = @p2 AND
							  A.CRT_DATE >= @p3 AND
							  A.CRT_DATE < @p4 
								";
            if (!string.IsNullOrEmpty(postingBeginDateTime))
            {
                sql += string.Format(" AND convert(varchar,A.POSTING_DATE,111) >= @p{0} ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, postingBeginDateTime));
            }

            if (!string.IsNullOrEmpty(postingEndDateTime))
            {
                sql += string.Format(" AND convert(varchar,A.POSTING_DATE,111) < @p{0} ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, postingEndDateTime));
            }

            if (!String.IsNullOrEmpty(status))
            {
                if (status != "-1")
                {
                    sql += " AND A.STATUS = @p" + parameters.Count;
                    parameters.Add(new SqlParameter("@p" + parameters.Count, status));
                }
                else
                {
                    sql += " AND A.STATUS != '9'";
                }
            }

            if (!String.IsNullOrEmpty(returnNo))
            {
                sql += " AND A.RTN_VNR_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, returnNo));
            }

            if (!String.IsNullOrEmpty(custOrdNo))
            {
                sql += " AND A.CUST_ORD_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, custOrdNo));
            }

            if (!String.IsNullOrEmpty(vendorCode))
            {
                sql += " AND A.VNR_CODE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, vendorCode));
            }

            if (!string.IsNullOrEmpty(vendorName))
            {
                sql += " AND B.VNR_NAME LIKE '" + vendorName + "%' ";
            }

            sql += @" ORDER BY A.RTN_VNR_NO ASC";

            var result = SqlQuery<F160201Data>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }

        //查詢結果明細
        public IQueryable<F160201DataDetail> GetF160201DataDetails(string dcCode, string gupCode, string custCode,
            string returnNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnNo)
            };

            var sql = @"
  SELECT ROW_NUMBER()OVER(ORDER BY A.RTN_VNR_SEQ) ROWNUM,
         A.RTN_VNR_NO,
         A.RTN_VNR_SEQ,
         A.CUST_CODE,
         F1909.CUST_NAME,
         A.GUP_CODE,
         F1929.GUP_NAME,
         A.DC_CODE,
         F1901.DC_NAME,
         A.ORG_WAREHOUSE_ID,
         A.WAREHOUSE_ID,
         A.LOC_CODE,
         A.ITEM_CODE,
         A.RTN_VNR_QTY,
         A.RTN_WMS_QTY,
         A.MEMO,
         --A.CRT_STAFF,
         --A.CRT_DATE,
         --A.UPD_STAFF,
         --A.UPD_DATE,
         --A.CRT_NAME,
         --A.UPD_NAME,
         B.ITEM_NAME AS ITEM_NAME,
         B.ITEM_SIZE AS ITEM_SIZE,
         B.ITEM_SPEC AS ITEM_SPEC,
         B.ITEM_COLOR AS ITEM_COLOR,
         F91000302.ACC_UNIT_NAME AS ITEM_UNIT,
         (A.RTN_VNR_QTY - A.RTN_WMS_QTY) AS NOT_RTN_WMS_QTY,
         F1980.WAREHOUSE_NAME AS WAREHOUSE_NAME,
         C.RTN_VNR_DATE,
         A.RTN_VNR_CAUSE,
         A.MAKE_NO,
         (SELECT CAUSE FROM F1951 WHERE UCT_ID='RV' AND UCC_CODE = A.RTN_VNR_CAUSE ) RTN_VNR_CAUSE_NAME
    FROM F160202 A
         INNER JOIN F160201 C
            ON     A.RTN_VNR_NO = C.RTN_VNR_NO
               AND A.DC_CODE = C.DC_CODE
               AND A.GUP_CODE = C.GUP_CODE
               AND A.CUST_CODE = C.CUST_CODE
         INNER JOIN F1903 B
            ON A.ITEM_CODE = B.ITEM_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
         LEFT JOIN F1980
            ON F1980.WAREHOUSE_ID = A.WAREHOUSE_ID AND F1980.DC_CODE = A.DC_CODE
         LEFT JOIN F1909 
            ON F1909.GUP_CODE = A.GUP_CODE AND F1909.CUST_CODE = A.CUST_CODE
         LEFT JOIN F1929
            ON F1929.GUP_CODE = A.GUP_CODE
         LEFT JOIN F1901
            ON F1901.DC_CODE = A.DC_CODE
         LEFT JOIN F91000302 
            ON F91000302.ITEM_TYPE_ID = '001' AND F91000302.ACC_UNIT = B.ITEM_UNIT
   WHERE     A.RTN_VNR_NO = @p3
         AND A.DC_CODE = @p0
         AND A.GUP_CODE = @p1
         AND A.CUST_CODE = @p2
						";

            var result = SqlQuery<F160201DataDetail>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }

        //廠退單-編輯-廠退明細
        public IQueryable<F160201ReturnDetail> GetF160201ReturnDetailsForEdit(string dcCode, string gupCode, string custCode,
            string vendorCode, string returnNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", vendorCode),
                new SqlParameter("@p4", returnNo)
            };

            var sql = @"SELECT	ROW_NUMBER()OVER(ORDER BY D.RTN_VNR_SEQ ASC) ROWNUM,D.RTN_VNR_NO,D.RTN_VNR_SEQ,D.WAREHOUSE_ID,
								D.LOC_CODE,D.ITEM_CODE,D.RTN_VNR_QTY,D.RTN_WMS_QTY,D.MEMO,
								C.ITEM_NAME,C.ITEM_SIZE,C.ITEM_SPEC,C.ITEM_COLOR,
								(SELECT WAREHOUSE_NAME FROM F1980 WHERE WAREHOUSE_ID　= D.WAREHOUSE_ID AND DC_CODE = D.DC_CODE) AS WAREHOUSE_NAME,
								ISNULL((SELECT SUM(QTY) FROM F1913 WHERE DC_CODE = D.DC_CODE AND GUP_CODE = D.GUP_CODE AND CUST_CODE = D.CUST_CODE AND 
																	LOC_CODE = D.LOC_CODE AND ITEM_CODE = D.ITEM_CODE AND 
																	VNR_CODE = @p3 ) ,0) AS INVENTORY_QTY,D.RTN_VNR_CAUSE,D.MAKE_NO,
               (SELECT CAUSE  FROM F1951 WHERE UCT_ID='RV' AND UCC_CODE = D.RTN_VNR_CAUSE) RTN_VNR_CAUSE_NAME
						FROM	F160202 D,
								F1903 C
						WHERE	D.DC_CODE = @p0 AND
								D.GUP_CODE = @p1 AND 
								D.CUST_CODE = @p2 AND
								D.RTN_VNR_NO = @p4 AND
                                D.ITEM_CODE = C.ITEM_CODE AND
								D.GUP_CODE = C.GUP_CODE AND
								D.CUST_CODE = C.CUST_CODE
						";
            var result = SqlQuery<F160201ReturnDetail>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }


        //廠退單-新增-廠退明細
        public IQueryable<F160201ReturnDetail> GetF160201ReturnDetails(string dcCode, string gupCode, string custCode,
            string warehouseId, DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin,
            DateTime? validDateEnd, string locBegin, string locEnd, string itemCode, string itemName)
        {
            var paramList = new List<object> { dcCode, gupCode, custCode, warehouseId };

            var sqlFormat = @"SELECT E.*, D.WAREHOUSE_NAME
								  FROM (  SELECT B.WAREHOUSE_ID,
												 B.GUP_CODE,
												 A.DC_CODE,
												 A.ENTER_DATE,
												 A.VALID_DATE,
												 A.LOC_CODE,
												 A.ITEM_CODE,
												 C.ITEM_NAME,
												 C.ITEM_SIZE,
												 C.ITEM_SPEC,
												 C.ITEM_COLOR,
												 SUM (A.QTY) AS INVENTORY_QTY
											FROM F1913 A
												 JOIN F1912 B
													ON     A.LOC_CODE = B.LOC_CODE
													   AND (A.GUP_CODE = B.GUP_CODE OR B.GUP_CODE = '0')
													   AND A.DC_CODE = B.DC_CODE
													   AND (A.CUST_CODE = B.CUST_CODE OR B.CUST_CODE = '0')
												 JOIN F1903 C
													ON A.GUP_CODE = C.GUP_CODE AND A.ITEM_CODE = C.ITEM_CODE AND A.CUST_CODE = C.CUST_CODE
										   WHERE     A.DC_CODE = @p0
												 AND A.GUP_CODE = @p1
												 AND A.CUST_CODE = @p2
												 AND B.WAREHOUSE_ID = @p3
												 {0}
										GROUP BY B.WAREHOUSE_ID,
												 B.GUP_CODE,
												 A.DC_CODE,
												 A.ENTER_DATE,
												 A.VALID_DATE,
												 A.LOC_CODE,
												 A.ITEM_CODE,
												 C.ITEM_NAME,
												 C.ITEM_SIZE,
												 C.ITEM_SPEC,
												 C.ITEM_COLOR
                                        ) E
										
									   JOIN F1980 D
										  ON E.WAREHOUSE_ID = D.WAREHOUSE_ID AND E.DC_CODE = D.DC_CODE
						";

            var conditionSql = paramList.CombineNotNullOrEmpty(" AND A.LOC_CODE >= @p{0}", locBegin);
            conditionSql += paramList.CombineNotNullOrEmpty(" AND A.LOC_CODE <= @p{0}", locEnd);
            conditionSql += paramList.CombineNotNullOrEmpty(" AND A.ITEM_CODE = @p{0}", itemCode);
            conditionSql += paramList.CombineNotNullOrEmpty(" AND C.ITEM_NAME LIKE @p{0} + '%'", itemName);

            if (enterDateBegin.HasValue && enterDateEnd.HasValue)
            {
                conditionSql += paramList.Combine(" AND A.ENTER_DATE BETWEEN @p{0} AND @p{1}", enterDateBegin, enterDateEnd);
            }

            if (validDateBegin.HasValue && validDateEnd.HasValue)
            {
                conditionSql += paramList.Combine(" AND A.VALID_DATE BETWEEN @p{0} AND @p{1}", validDateBegin, validDateEnd);
            }

            var sql = string.Format(sqlFormat, conditionSql);

            return SqlQuery<F160201ReturnDetail>(sql, paramList.ToArray());
        }

        //取得尚未完成出貨扣帳的廠退單資料
        public IQueryable<F160201Data> GetF160201DatasNotFinish(string dcCode, string gupCode, string custCode,
            DateTime createBeginDateTime, DateTime createEndDateTime, DateTime? returnBeginDateTime, DateTime? returnEndDateTime,
            string deliveryWay, string returnNo, string returnType, string vendorCode, string vendorName,string typeId,string custOrdNo )
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", createBeginDateTime),
                new SqlParameter("@p4", createEndDateTime),
								new SqlParameter("@p5", deliveryWay),
								new SqlParameter("@p6", typeId)
						};

            var sql = $@"
                        SELECT	ROW_NUMBER()OVER(ORDER BY A.RTN_VNR_NO ASC) ROWNUM,A.CRT_DATE,A.RTN_VNR_DATE,A.RTN_VNR_NO,A.ORD_PROP,
		                        (SELECT ORD_PROP_NAME FROM F000903 WHERE ORD_PROP = A.ORD_PROP) AS ORD_PROP_TEXT,
		                        A.DELIVERY_WAY,
														(SELECT NAME FROM VW_F000904_LANG  WHERE TOPIC='F160201' AND SUBTOPIC='DELIVERY_WAY' AND VALUE = A.DELIVERY_WAY AND LANG = '{Current.Lang}') DELIVERY_WAY_NAME,
														A.VNR_CODE,B.VNR_NAME,
		                        (SELECT RTN_VNR_TYPE_NAME FROM F160203 WHERE RTN_VNR_TYPE_ID = A.RTN_VNR_TYPE_ID) AS RTN_VNR_TYPE_TEXT,
                            A.TYPE_ID, (SELECT TYPE_NAME FROM F198001 WHERE TYPE_ID = A.TYPE_ID) TYPE_NAME, A.CUST_ORD_NO,A.ADDRESS,A.ITEM_TEL,A.ITEM_CONTACT
                        FROM	F160201 A, 
		                        F1908 B
                        WHERE	A.VNR_CODE = B.VNR_CODE AND
		                        A.GUP_CODE = B.GUP_CODE	AND

                                B.CUST_CODE=
		                        (SELECT 
		                        CASE WHEN
		                        C.ALLOWGUP_VNRSHARE='1'
		                        THEN '0' 
		                        ELSE @p2
		                        END
		                        FROM F1909 C WHERE C.GUP_CODE = @p1 AND C.CUST_CODE = @p2 ) 
								
		                        AND

		                        A.STATUS = '0' AND
		                        A.DC_CODE = @p0 AND
		                        A.GUP_CODE = @p1 AND 
		                        A.CUST_CODE = @p2 AND
		                        A.CRT_DATE >= @p3 AND
		                        A.CRT_DATE <= @p4 AND
		                        A.DELIVERY_WAY = @p5 AND
                            A.TYPE_ID = @p6
";

            if (!String.IsNullOrEmpty(returnNo))
            {
                sql += " AND A.RTN_VNR_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, returnNo));
            }

            if (!String.IsNullOrEmpty(returnType))
            {
                sql += " AND A.RTN_VNR_TYPE_ID = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, returnType));
            }

            if (!String.IsNullOrEmpty(vendorCode))
            {
                sql += " AND A.VNR_CODE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, vendorCode));
            }

            if (!string.IsNullOrEmpty(vendorName))
            {
                sql += " AND B.VNR_NAME LIKE '%" + vendorName + "%' ";
            }
						if(!string.IsNullOrWhiteSpace(custOrdNo))
						{
							sql += " AND A.CUST_ORD_NO = @p" + parameters.Count;
							parameters.Add(new SqlParameter("@p" + parameters.Count, custOrdNo));
						}
						if (returnBeginDateTime.HasValue)
						{
							sql += " AND A.RTN_VNR_DATE >= @p" + parameters.Count;
							parameters.Add(new SqlParameter("@p" + parameters.Count, returnBeginDateTime.Value));
						}
						if (returnEndDateTime.HasValue)
						{
							sql += " AND A.RTN_VNR_DATE <= @p" + parameters.Count;
							parameters.Add(new SqlParameter("@p" + parameters.Count, returnEndDateTime.Value));
						}
			
            var result = SqlQuery<F160201Data>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }

        /// <summary>
        /// 取得 廠退數量 = 廠退出貨數量 且 關聯的出貨單皆已扣帳 的廠退單主檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="rtnWmsNo"></param>
        /// <param name="filterWmsOrdNos">要過濾的已扣帳判斷的出貨單，因可能是最後一批，避免Transaction問題</param>
        /// <returns></returns>
        public IQueryable<F160201> GetVnrQtyEqWmsQtyF160201(string dcCode, string gupCode, string custCode, string rtnWmsNo, IEnumerable<string> filterWmsOrdNos)
        {
            var sqlFormat = @"SELECT B.*
							  FROM (  SELECT A1.RTN_VNR_NO,
											 A1.DC_CODE,
											 A1.GUP_CODE,
											 A1.CUST_CODE
										FROM F160204 A1                                           -- 廠退出貨單
									   WHERE     A1.RTN_WMS_NO = @p0
											 AND A1.DC_CODE = @p1
											 AND A1.GUP_CODE = @p2
											 AND A1.CUST_CODE = @p3
									GROUP BY A1.RTN_VNR_NO,
											 A1.DC_CODE,
											 A1.GUP_CODE,
											 A1.CUST_CODE) A
								   JOIN F160201 B                                                 -- 廠退單主檔
									  ON     A.RTN_VNR_NO = B.RTN_VNR_NO
										 AND A.DC_CODE = B.DC_CODE
										 AND A.CUST_CODE = B.CUST_CODE
										 AND A.GUP_CODE = B.GUP_CODE
							 WHERE     NOT EXISTS                                -- 確認廠退數量與出貨廠退數都一樣時，才能做結案
											  (SELECT 1
												 FROM F160202 C                                   -- 廠退單明細
												WHERE     B.RTN_VNR_NO = C.RTN_VNR_NO
													  AND B.DC_CODE = C.DC_CODE
													  AND B.CUST_CODE = C.CUST_CODE
													  AND B.GUP_CODE = C.GUP_CODE
													  AND C.RTN_VNR_QTY <> C.RTN_WMS_QTY)
								   AND NOT EXISTS                                -- 確認關聯的所有出貨單都已扣帳時，才能的做結案
											  (SELECT 1
												 FROM F05030101 D
													  JOIN F050801 E
														 ON     D.WMS_ORD_NO = E.WMS_ORD_NO
															AND D.DC_CODE = E.DC_CODE
															AND D.GUP_CODE = E.GUP_CODE
															AND D.CUST_CODE = E.CUST_CODE
													  JOIN F050301 F
														 ON     D.ORD_NO = F.ORD_NO
															AND D.DC_CODE = F.DC_CODE
															AND D.GUP_CODE = F.GUP_CODE
															AND D.CUST_CODE = F.CUST_CODE
												WHERE    F.CUST_ORD_NO = B.RTN_VNR_NO 
													  AND A.DC_CODE = F.DC_CODE
													  AND A.GUP_CODE = F.GUP_CODE
													  AND A.CUST_CODE = F.CUST_CODE
													  AND E.STATUS NOT IN (5, 6)	-- 自取會直接為已裝車(5), 要裝車才會是先已扣帳(6)
													  AND {0} -- 過濾最後一批出貨單，因Transaction的關係需過濾掉
																				  )  AND B.STATUS = '1'";

            var parameterList = new List<object> { rtnWmsNo, dcCode, gupCode, custCode };
            var notInSql = parameterList.CombineSqlNotInParameters("E.WMS_ORD_NO", filterWmsOrdNos);
            var sql = string.Format(sqlFormat, notInSql);
            return SqlQuery<F160201>(sql, parameterList.ToArray());
        }

        public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", settleDate),
                new SqlParameter("@p4", settleDate.AddDays(1))
            };
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY WMS_NO,ITEM_CODE) ROWNUM,TB.* FROM (
						SELECT @p3 CAL_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.RTN_VNR_NO WMS_NO,
									 B.ITEM_CODE,SUM (B.RTN_WMS_QTY) QTY,'01' DELV_ACC_TYPE
							FROM F160201 A
									 JOIN F160202 B
											ON     A.DC_CODE = B.DC_CODE
												 AND A.GUP_CODE = B.GUP_CODE
												 AND A.CUST_CODE = B.CUST_CODE
												 AND A.RTN_VNR_NO = B.RTN_VNR_NO
						 WHERE     (A.DC_CODE = @p0 OR @p0 = '000') AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
									 AND A.POSTING_DATE >= @p3 AND A.POSTING_DATE < @p4 AND A.STATUS = '2'
					GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.RTN_VNR_NO,B.ITEM_CODE) TB";
            return SqlQuery<SettleData>(sql, parameter.ToArray());
        }

        /// <summary>
        /// 取得P017_退貨報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="beginRtnVnrDate"></param>
        /// <param name="endRtnVnrDate"></param>
        /// <param name="rtnVnrNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IQueryable<P160201Report> GetP160201Reports(string dcCode, string gupCode, string custCode, DateTime? beginRtnVnrDate, DateTime? endRtnVnrDate, string rtnVnrNo, string status)
        {
            var sql = $@" SELECT ROW_NUMBER()OVER(
ORDER BY C.RETURN_DATE,
								 C.RETURN_NO,
								 C.GUP_CODE,
								 C.CUST_CODE,
								 C.VNR_CODE,
								 C.ITEM_CODE,
								 C.LOC_CODE) AS ROWNUM,
								 C.RETURN_DATE,
								 C.RETURN_NO,
								 C.CUST_ORD_NO,
								 C.VNR_CODE,
								 C.ITEM_CODE,
								 C.AUDIT_QTY,
								 CASE
									WHEN LEN(C.LOC_CODE) = 9
									THEN
										  SUBSTRING (C.LOC_CODE, 1, 1)
									   + '-'
									   + SUBSTRING (C.LOC_CODE, 2, 2)
									   + '-'
									   + SUBSTRING (C.LOC_CODE, 4, 2)
									   + '-'
									   + SUBSTRING (C.LOC_CODE, 6, 2)
									   + '-'
									   + SUBSTRING (C.LOC_CODE, 8, 2)
									ELSE
									   C.LOC_CODE
								 END
									LOC_CODE,
								 D.GUP_NAME,
								 E.CUST_NAME,
								 C.RTN_CUST_NAME AS RETAIL_NAME,
								 F.VNR_NAME,
								 G.ITEM_NAME,
								 I.AREA_NAME,
								 J.NAME AS TYPE_NAME
							FROM (  SELECT A.RETURN_DATE,
										   A.RETURN_NO,
										   A.DC_CODE,
										   A.GUP_CODE,
										   A.CUST_CODE,
										   A.CUST_ORD_NO,
										   A.RTN_CUST_CODE,
										   A.RTN_CUST_NAME,
										   K.VNR_CODE,
										   B.ITEM_CODE,
										   SUM (B.AUDIT_QTY) AS AUDIT_QTY,
										   B.LOC_CODE
									  FROM F161201 A
										   JOIN F161402 B
											  ON     A.DC_CODE = B.DC_CODE
												 AND A.GUP_CODE = B.GUP_CODE
												 AND A.CUST_CODE = B.CUST_CODE
												 AND A.RETURN_NO = B.RETURN_NO
										   LEFT JOIN
										   (  SELECT K1.GUP_CODE,
													 K1.CUST_CODE,
													 K1.ITEM_CODE,
													 MIN (K1.VNR_CODE) AS VNR_CODE
												FROM F190303 K1
											   WHERE (ISNULL (K1.UPD_DATE, K1.CRT_DATE)) =
														(SELECT MAX (ISNULL (K2.UPD_DATE, K2.CRT_DATE))
														   FROM F190303 K2
														  WHERE     K1.GUP_CODE = K2.GUP_CODE
																AND K1.CUST_CODE = K2.CUST_CODE
																AND K1.ITEM_CODE = K2.ITEM_CODE)
											GROUP BY K1.GUP_CODE, K1.CUST_CODE, K1.ITEM_CODE) K
											  ON     B.GUP_CODE = K.GUP_CODE
												 AND B.CUST_CODE = K.CUST_CODE
												 AND B.ITEM_CODE = K.ITEM_CODE
                                            WHERE 1=1";
            if (!string.IsNullOrEmpty(dcCode))
            {
                sql+=@" AND A.DC_CODE =  @p0 ";
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += @" AND A.GUP_CODE = @p1 ";
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += @" AND A.CUST_CODE =  @p2 ";
            }
            if (beginRtnVnrDate.HasValue)
            {
                sql += @" AND A.RETURN_DATE BETWEEN  @p3 ";
            }
            if (endRtnVnrDate.HasValue)
            {
                sql += @" AND  @p4";
            }
            if (!string.IsNullOrEmpty(rtnVnrNo))
            {
                sql += @" AND A.RETURN_NO =  @p5 ";
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += @" AND A.STATUS = @p6 ";
            }
            sql+=$@"
								  GROUP BY A.RETURN_DATE,
										   A.RETURN_NO,
										   A.DC_CODE,
										   A.GUP_CODE,
										   A.CUST_CODE,
										   A.CUST_ORD_NO,
										   A.RTN_CUST_CODE,
										   A.RTN_CUST_NAME,
										   K.VNR_CODE,
										   B.ITEM_CODE,
										   B.LOC_CODE) C
								 LEFT JOIN F1929 D ON C.GUP_CODE = D.GUP_CODE
								 LEFT JOIN F1909 E
									ON C.GUP_CODE = E.GUP_CODE AND C.CUST_CODE = E.CUST_CODE
								 LEFT JOIN F1908 F
									ON C.GUP_CODE = F.GUP_CODE AND C.VNR_CODE = F.VNR_CODE
								 LEFT JOIN F1903 G
									ON C.GUP_CODE = G.GUP_CODE AND C.ITEM_CODE = G.ITEM_CODE AND C.CUST_CODE=G.CUST_CODE
								 LEFT JOIN F1912 H ON C.DC_CODE = H.DC_CODE AND C.LOC_CODE = H.LOC_CODE
								 LEFT JOIN F1919 I
									ON     H.AREA_CODE = I.AREA_CODE
									   AND H.WAREHOUSE_ID = I.WAREHOUSE_ID
									   AND H.DC_CODE = I.DC_CODE
								 LEFT JOIN VW_F000904_LANG  J
									ON G.TYPE = J.VALUE AND J.TOPIC = 'F1903' AND J.SUBTOPIC = 'TYPE' AND J.LANG = '{Current.Lang}'
						
					 ";

                return SqlQuery<P160201Report>(sql, new object[] { dcCode, gupCode, custCode, beginRtnVnrDate, endRtnVnrDate, rtnVnrNo, status });
        }


        /// <summary>
        /// 確認是否為重覆廠退單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="custOrdNo"></param>
        /// <param name="vnrCode"></param>
        /// <returns></returns>
        public IQueryable<F160201> GetDatasByCustOrdNoAndVnrCodeNotCancel(string dcCode, string gupCode, string custCode, string custOrdNo, string vnrCode)
        {
            var parms = new object[] { dcCode, gupCode, custCode, custOrdNo, vnrCode };
            var sql = @" SELECT * 
                     FROM F160201 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 
                      AND RTN_VNR_NO = @p3
                      AND VNR_CODE = @p4
                      AND STATUS <>'9' ";
            return SqlQuery<F160201>(sql, parms);
        }

        /// <summary>
        /// 取消廠商退貨單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="rtnVnrNo"></param>
        /// <param name="importFlag"></param>
        public void CancelNotProcessVnrReturn(string dcCode, string gupCode, string custCode, string rtnVnrNo, string importFlag)
        {
            string sql = @"
				           update F160201 set STATUS = '9', UPD_DATE = @p7, UPD_STAFF = @p0, UPD_NAME = @p1, IMPORT_FLAG = @p6
                           Where DC_CODE =@p2
				             and GUP_CODE =@p3
				             and CUST_CODE =@p4
                             and RTN_VNR_NO=@p5
                             and STATUS <> '9'
			               ";
            var sqlParams = new SqlParameter[]
            {
                 new SqlParameter("@p0", Current.Staff),
                 new SqlParameter("@p1", Current.StaffName),
                 new SqlParameter("@p2", dcCode),
                 new SqlParameter("@p3", gupCode),
                 new SqlParameter("@p4", custCode),
                 new SqlParameter("@p5", rtnVnrNo),
                 new SqlParameter("@p6", importFlag),
                 new SqlParameter("@p7", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            ExecuteSqlCommand(sql, sqlParams);
        }

    }
}
