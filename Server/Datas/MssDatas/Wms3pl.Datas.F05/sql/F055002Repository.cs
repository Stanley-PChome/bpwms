using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F055002Repository : RepositoryBase<F055002, Wms3plDbContext, F055002Repository>
	{
        /// <summary>
        /// 取得出貨包裝出貨明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <param name="packageBoxNo">沒填的話，表示沒有箱子，回傳的包裝數就是0</param>
        /// <returns></returns>
        public IQueryable<DeliveryData> GetDeliveryData(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo = 0, string itemCode = "")
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p3",wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p4",packageBoxNo) { SqlDbType = System.Data.SqlDbType.SmallInt },
                new SqlParameter("@p5",itemCode) { SqlDbType = System.Data.SqlDbType.VarChar }
            };

            var sql = @"SELECT F.*, F.OrderQty - F.TotalPackQty AS DiffQty                      -- 差異數
						  FROM (  SELECT TOP 100 PERCENT D.ITEM_CODE ItemCode,
										 ISNULL (E.ITEM_NICKNAME, E.ITEM_NAME) ItemName,
										 E.BUNDLE_SERIALNO,                                    -- 序號商品
										 E.BUNDLE_SERIALLOC,                                  -- 序號綁儲位
										 D.OrderQty,                                       -- 某商品數出貨總量
										 J.CONSIGNEE,
										 K.ACC_UNIT_NAME AS ITEM_UNIT,
										 E.ITEM_SIZE,
										 E.ITEM_SPEC,
										 E.ITEM_COLOR,
										 ISNULL (
											(  SELECT SUM (PACKAGE_QTY)
												 FROM F055002 C
												WHERE     C.WMS_ORD_NO = D.WMS_ORD_NO
													  AND C.DC_CODE = D.DC_CODE
													  AND C.GUP_CODE = D.GUP_CODE
													  AND C.CUST_CODE = D.CUST_CODE
													  AND C.ITEM_CODE = D.ITEM_CODE
													  AND C.PACKAGE_BOX_NO = @p4           -- 目前正在刷的箱子
											 GROUP BY C.ITEM_CODE),
											0)
											PackQty,                                 -- 該出貨單某個箱子的某商品總數
										 ISNULL (
											(  SELECT SUM (PACKAGE_QTY)
												 FROM F055002 C
												WHERE     C.WMS_ORD_NO = D.WMS_ORD_NO
													  AND C.DC_CODE = D.DC_CODE
													  AND C.GUP_CODE = D.GUP_CODE
													  AND C.CUST_CODE = D.CUST_CODE
													  AND C.ITEM_CODE = D.ITEM_CODE
											 GROUP BY C.ITEM_CODE),
											0)
											AS TotalPackQty                          -- 該出貨單所有箱子的某商品總數
									FROM (  SELECT A.ITEM_CODE,
												   A.WMS_ORD_NO,
												   A.DC_CODE,
												   A.GUP_CODE,
												   A.CUST_CODE,
												   SUM (A.A_PICK_QTY) OrderQty             -- 某商品數出貨總量
											  FROM F051202 A
											 WHERE     A.WMS_ORD_NO = @p3
												   AND A.DC_CODE = @p0
												   AND A.GUP_CODE = @p1
												   AND A.CUST_CODE = @p2
                                                   AND A.ITEM_CODE = CASE WHEN @p5 is null or @p5 = '' THEN A.ITEM_CODE ELSE @p5 END 
										  GROUP BY A.ITEM_CODE,
												   A.WMS_ORD_NO,
												   A.DC_CODE,
												   A.GUP_CODE,
												   A.CUST_CODE) D
										 JOIN F1903 E
											ON D.ITEM_CODE = E.ITEM_CODE 
                                           AND D.GUP_CODE = E.GUP_CODE
                                           AND D.CUST_CODE = E.CUST_CODE
										 LEFT JOIN F91000302 K
											ON E.ITEM_UNIT = K.ACC_UNIT AND ITEM_TYPE_ID = '001'
										 LEFT JOIN
										 (  SELECT H.WMS_ORD_NO,
												   I.DC_CODE,
												   I.GUP_CODE,
												   I.CUST_CODE,
												   I.CONSIGNEE
											  FROM F05030101 H
												   JOIN F050101 I
													  ON     H.DC_CODE = I.DC_CODE
														 AND H.GUP_CODE = I.GUP_CODE
														 AND H.CUST_CODE = I.CUST_CODE
														 AND H.ORD_NO = I.ORD_NO
										  GROUP BY H.WMS_ORD_NO,
												   I.DC_CODE,
												   I.GUP_CODE,
												   I.CUST_CODE,
												   I.CONSIGNEE) J
											ON     D.DC_CODE = J.DC_CODE
											   AND D.GUP_CODE = J.GUP_CODE
											   AND D.CUST_CODE = J.CUST_CODE
											   AND D.WMS_ORD_NO = J.WMS_ORD_NO
								ORDER BY D.ITEM_CODE) F";

            var result = SqlQuery<DeliveryData>(sql, parm.ToArray());

            return result;
        }

        /// <summary>
        /// 取得尚未包裝完的商品與差異數
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public IQueryable<DeliveryData> GetQuantityOfDeliveryInfo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string itemCode, short packageBoxNo = 0)
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p3",wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p4",itemCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p5",packageBoxNo) { SqlDbType = System.Data.SqlDbType.SmallInt }
            };

            var sql = $@"SELECT F.*, F.OrderQty - F.TotalPackQty AS DiffQty                      -- 差異數
						  FROM (SELECT D.ITEM_CODE ItemCode,
                        D.ITEM_NAME ItemName,
                     D.ALLOWORDITEM AllowOrdItem,
                         D.IS_EASY_LOSE IsEasyLose,
                         D.IS_PRECIOUS IsPrecious,
                         D.IS_MAGNETIC IsMagnetic,
                         D.IS_PERISHABLE IsPerishable,
                         D.FRAGILE Fragile,
                         D.SPILL Spill,
                         D.NAME TmprTypeName,
                         CASE WHEN SUBSTRING(D.FEATURE, LEN(D.FEATURE) ,1) = '、' THEN REVERSE(stuff(REVERSE(D.FEATURE),1,1,'')) ELSE D.FEATURE END FEATURE,
									   D.OrderQty,                                         -- 某商品數出貨總量
									   ISNULL (
										  (  SELECT SUM (PACKAGE_QTY)
											   FROM F055002 C
											  WHERE     C.WMS_ORD_NO = D.WMS_ORD_NO
													AND C.DC_CODE = D.DC_CODE
													AND C.GUP_CODE = D.GUP_CODE
													AND C.CUST_CODE = D.CUST_CODE
													AND C.ITEM_CODE = D.ITEM_CODE
													AND C.PACKAGE_BOX_NO = @p5             -- 目前正在刷的箱子
										   GROUP BY C.ITEM_CODE),
										  0)
										  PackQty,                                   -- 該出貨單某個箱子的某商品總數
									   ISNULL (
										  (  SELECT SUM (PACKAGE_QTY)
											   FROM F055002 C
											  WHERE     C.WMS_ORD_NO = D.WMS_ORD_NO
													AND C.DC_CODE = D.DC_CODE
													AND C.GUP_CODE = D.GUP_CODE
													AND C.CUST_CODE = D.CUST_CODE
													AND C.ITEM_CODE = D.ITEM_CODE
										   GROUP BY C.ITEM_CODE),
										  0)
										  AS TotalPackQty                            -- 該出貨單所有箱子的某商品總數
								  FROM (  SELECT A.ITEM_CODE,
                                                 B.ITEM_NAME,
                                                 B.ALLOWORDITEM,
                                                 B.IS_EASY_LOSE,
                                                 B.IS_PRECIOUS,
                                                 B.IS_MAGNETIC,
                                                 B.IS_PERISHABLE,
                                                 B.IS_TEMP_CONTROL,
                                                 B.FRAGILE,
                                                 B.SPILL,
                                                 C.NAME,
												 A.WMS_ORD_NO,
												 A.DC_CODE,
												 A.GUP_CODE,
												 A.CUST_CODE,
												 SUM (A.A_PICK_QTY) OrderQty,               -- 某商品數出貨總量
												 ISNULL((
													CASE WHEN ISNULL(B.IS_PRECIOUS,'0') = '1' THEN '貴重品、' ELSE '' END +
													CASE WHEN ISNULL(B.IS_EASY_LOSE,'0') = '1' THEN '易遺失品、' ELSE '' END +
													CASE WHEN ISNULL(B.IS_MAGNETIC,'0') = '1' THEN '強磁標示、' ELSE '' END +
													CASE WHEN ISNULL(B.FRAGILE,'0') = '1' THEN '易碎品、' ELSE '' END +
													CASE WHEN ISNULL(B.SPILL,'0') = '1' THEN '液體、' ELSE '' END +
													CASE WHEN ISNULL(B.IS_PERISHABLE,'0') = '1' THEN '易變質、' ELSE '' END + 
													CASE WHEN ISNULL(B.IS_TEMP_CONTROL,'0') = '1' THEN '需溫控、' ELSE '' END 
												),'') FEATURE
											FROM F051202 A
                                              JOIN F1903 B
                                                ON B.GUP_CODE = A.GUP_CODE
                                               AND B.CUST_CODE = A.CUST_CODE
                                               AND B.ITEM_CODE = A.ITEM_CODE
					                     LEFT JOIN VW_F000904_LANG C
					                            ON C.VALUE = B.TMPR_TYPE
										   WHERE     A.DC_CODE = @p0
												 AND A.GUP_CODE = @p1
												 AND A.CUST_CODE = @p2
												 AND A.WMS_ORD_NO = @p3
												 AND A.ITEM_CODE = ISNULL ( @p4, A.ITEM_CODE)   
                                                 AND C.TOPIC = 'F1903'
												 AND C.SUBTOPIC = 'TMPR_TYPE'
                                                 AND C.LANG = '{Current.Lang}'
										GROUP BY A.ITEM_CODE,
                                                 B.ITEM_NAME,
                                                 B.ALLOWORDITEM,
                                                 B.IS_EASY_LOSE,
                                                 B.IS_PRECIOUS,
                                                 B.IS_MAGNETIC,
                                                 B.IS_PERISHABLE,
                                                 B.IS_TEMP_CONTROL,
                                                 B.FRAGILE,
                                                 B.SPILL,
                                                 C.NAME,
												 A.WMS_ORD_NO,
												 A.DC_CODE,
												 A.GUP_CODE,
												 A.CUST_CODE) D) F  WHERE F.OrderQty <>0";

            var result = SqlQuery<DeliveryData>(sql, parm.ToArray());

            return result;
        }

        public IQueryable<DeliveryReport> GetDeliveryReport(string dcCode, string gupCode, string custCode, string wmsOrdNo, short? packageBoxNo = null)
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p3",wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p4",packageBoxNo) { SqlDbType = System.Data.SqlDbType.SmallInt }
            };
        
            var sql = @"
 SELECT J.ord_no        AS OrdNo,
       D.package_box_no AS PackageBoxNo,
       D.item_code      AS ItemCode,
       D.past_no        AS BoxNo,
       D.packqty,
       CASE
         WHEN E.item_nickname IS NULL
               OR E.item_nickname = '' THEN E.item_name
         ELSE E.item_nickname
       END              AS ItemName,
       J.consignee,
       K.acc_unit_name  AS ITEM_UNIT,
       E.item_size,
       E.item_spec,
       E.item_color,
       E.ean_code1,
       E.cust_item_code,
       ''               AS ORG_ITEM_CODE,
       ''               AS ItemName_MOMO,
       J.retail_code,
       J.short_salesbase_name,
       E.ean_code2,
       M.LOGISTIC_NAME  AS LogisticName
FROM   (SELECT K.dc_code,
               K.gup_code,
               K.cust_code,
               K.wms_ord_no,
               K.past_no,
               K.package_box_no,
               L.item_code,
               Sum (L.package_qty) AS PackQty
        FROM   f055001 K
               JOIN f055002 L
                 ON K.dc_code = L.dc_code
                    AND K.gup_code = L.gup_code
                    AND K.cust_code = L.cust_code
                    AND K.wms_ord_no = L.wms_ord_no
                    AND K.package_box_no = L.package_box_no
        WHERE  K.dc_code = @p0
               AND K.gup_code = @p1
               AND K.cust_code = @p2
               AND K.wms_ord_no = @p3
               AND K.package_box_no = Isnull (@p4, K.package_box_no)
               AND L.package_qty > 0
        GROUP  BY K.dc_code,
                  K.gup_code,
                  K.cust_code,
                  K.wms_ord_no,
                  K.past_no,
                  K.package_box_no,
                  L.item_code) D
       LEFT JOIN f1903 E
              ON D.item_code = E.item_code
                 AND D.gup_code = E.gup_code
                 AND D.cust_code = E.cust_code
       LEFT JOIN f91000302 K
              ON E.item_unit = K.acc_unit
                 AND item_type_id = '001'
       LEFT JOIN (SELECT M.ord_no,
                         M.wms_ord_no,
                         M.dc_code,
                         M.gup_code,
                         M.cust_code,
                         M.consignee,
                         N.retail_code,
                         N.short_salesbase_name
                  FROM   (SELECT H.ord_no,
                                 H.wms_ord_no,
                                 H.dc_code,
                                 H.gup_code,
                                 H.cust_code,
                                 I.consignee
                          FROM   f05030101 H
                                 LEFT JOIN f050101 I
                                        ON H.dc_code = I.dc_code
                                           AND H.gup_code = I.gup_code
                                           AND H.cust_code = I.cust_code
                                           AND H.ord_no = I.ord_no
                          GROUP  BY H.ord_no,
                                    H.wms_ord_no,
                                    H.dc_code,
                                    H.gup_code,
                                    H.cust_code,
                                    I.consignee) M
                         LEFT JOIN (SELECT DISTINCT A.wms_ord_no,
                                                    A.cust_code,
                                                    A.gup_code,
                                                    A.dc_code,
                                                    A.retail_code,
                                                    B.short_salesbase_name
                                    FROM   f050801 A,
                                           f1910 B,
                                           f1909 C
                                    WHERE  A.gup_code = B.gup_code
                                           AND A.gup_code = C.gup_code
                                           AND A.cust_code = C.cust_code
                                           AND CASE
                                                 WHEN C.allowgup_retailshare =
                                                      '1' THEN
                                                 '0'
                                                 ELSE C.cust_code
                                               END = B.cust_code
                                           AND A.retail_code = B.retail_code) N
                                ON M.dc_code = N.dc_code
                                   AND M.gup_code = N.gup_code
                                   AND M.cust_code = N.cust_code
                                   AND M.wms_ord_no = N.wms_ord_no) J
              ON D.dc_code = J.dc_code
                 AND D.gup_code = J.gup_code
                 AND D.cust_code = J.cust_code
                 AND D.wms_ord_no = J.wms_ord_no
       LEFT JOIN (SELECT M1.DC_CODE,M1.GUP_CODE,M1.CUST_CODE,M1.WMS_NO,M1.CONSIGN_NO,M2.LOGISTIC_NAME
              FROM F050901 M1
              INNER JOIN F0002 M2
                     ON M1.DC_CODE = M2.DC_CODE
                     AND M1.DELIVID_SEQ_NAME = M2.LOGISTIC_CODE) M
              ON D.dc_code = M.DC_CODE
              AND D.GUP_CODE = M.GUP_CODE
              AND D.CUST_CODE = M.CUST_CODE
              AND D.WMS_ORD_NO = M.WMS_NO
              AND D.PAST_NO = M.CONSIGN_NO
       ";

            var result = SqlQuery<DeliveryReport>(sql, parm.ToArray());

            return result;
        }

        public IQueryable<F055002WithF2501> GetF055002WithF2501s(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p3",wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar }
            };

            var sql = @"SELECT B.GUP_CODE,
						   B.CUST_CODE,
						   B.SERIAL_NO,
						   B.ITEM_CODE,
						   B.PACKAGE_BOX_NO,
						   C.BOX_SERIAL,
						   C.CASE_NO,
						   C.BATCH_NO
					  FROM F055002 B
						   INNER JOIN F2501 C
							  ON     B.GUP_CODE = C.GUP_CODE
								 AND B.CUST_CODE = C.CUST_CODE
								 AND B.SERIAL_NO = C.SERIAL_NO
								 AND (   C.BOX_SERIAL IS NOT NULL
									  OR C.CASE_NO IS NOT NULL
									  OR C.BATCH_NO IS NOT NULL)
					 WHERE     B.DC_CODE = @p0
						   AND B.GUP_CODE = @p1
						   AND B.CUST_CODE = @p2
						   AND B.WMS_ORD_NO = @p3";

            var result = SqlQuery<F055002WithF2501>(sql, parm.ToArray());

            return result;
        }

        public IQueryable<SearchWmsOrderPackingDetailRes> SearchWmsOrderPackingDetail(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
                new SqlParameter("@p3",wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar }
            };

            var sql = $@"
						SELECT
                        ROW_NUMBER ()OVER(ORDER BY Z.ITEM_CODE) RowNum,
                        Z.ALLOWORDITEM IsOriItem,
                        Z.ITEM_CODE ItemCode,
                        Z.ITEM_NAME ItemName,
                        SUM(Z.A_PICK_QTY) ShipQty,
                        ISNULL(SUM(X.PACKAGE_QTY), 0) PackageQty,
                        ISNULL(SUM(X.TOTAL_PACKAGE_QTY), 0) TotalPackageQty,
                        SUM(Z.A_PICK_QTY) - ISNULL(SUM(X.TOTAL_PACKAGE_QTY), 0) DiffQty,
						CASE WHEN SUBSTRING(Z.FEATURE, LEN(Z.FEATURE) ,1) = '、' THEN REVERSE(stuff(REVERSE(Z.FEATURE),1,1,'')) ELSE Z.FEATURE END Feature,
						Z.NAME TmprTypeName
                        FROM
                        (SELECT A.ITEM_CODE, B.ALLOWORDITEM, B.ITEM_NAME, SUM(A.A_PICK_QTY) A_PICK_QTY,C.NAME NAME,
								ISNULL((CASE WHEN ISNULL(B.IS_PRECIOUS,'0') = '1' THEN '貴重品、' ELSE '' END +
										CASE WHEN ISNULL(B.IS_EASY_LOSE,'0') = '1' THEN '易遺失品、' ELSE '' END +
										CASE WHEN ISNULL(B.IS_MAGNETIC,'0') = '1' THEN '強磁標示、' ELSE '' END +
										CASE WHEN ISNULL(B.FRAGILE,'0') = '1' THEN '易碎品、' ELSE '' END +
										CASE WHEN ISNULL(B.SPILL,'0') = '1' THEN '液體、' ELSE '' END +
										CASE WHEN ISNULL(B.IS_PERISHABLE,'0') = '1' THEN '易變質、' ELSE '' END +
										CASE WHEN ISNULL(B.IS_TEMP_CONTROL,'0') = '1' THEN '需溫控、' ELSE '' END
										),'') FEATURE
                        FROM F051202 A
                        JOIN F1903 B
                        ON A.GUP_CODE = B.GUP_CODE
                        AND A.CUST_CODE = B.CUST_CODE
                        AND A.ITEM_CODE = B.ITEM_CODE
                        LEFT JOIN VW_F000904_LANG C
					    ON C.VALUE = B.TMPR_TYPE
                        WHERE C.TOPIC = 'F1903'
						AND C.SUBTOPIC = 'TMPR_TYPE'
                        AND C.LANG = '{Current.Lang}'
                        AND A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.WMS_ORD_NO = @p3
                        GROUP BY A.ITEM_CODE, B.ALLOWORDITEM, B.ITEM_NAME,B.IS_PRECIOUS,B.IS_EASY_LOSE,B.IS_MAGNETIC,B.FRAGILE,B.SPILL,B.IS_PERISHABLE,B.IS_TEMP_CONTROL,C.NAME) Z
                        LEFT JOIN
                        (
                        SELECT 
                        B.ITEM_CODE ITEM_CODE, 
                        SUM(B.PACKAGE_QTY) TOTAL_PACKAGE_QTY,
                        ISNULL(SUM(CASE WHEN A.IS_CLOSED = '0' THEN B.PACKAGE_QTY END), 0) PACKAGE_QTY
                        FROM F055001 A
                        JOIN F055002 B
                        ON A.DC_CODE = B.DC_CODE
                        AND A.GUP_CODE = B.GUP_CODE
                        AND A.CUST_CODE = B.CUST_CODE
                        AND A.WMS_ORD_NO = B.WMS_ORD_NO
                        AND A.PACKAGE_BOX_NO = B.PACKAGE_BOX_NO
                        WHERE A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.WMS_ORD_NO = @p3
                        GROUP BY B.ITEM_CODE
                        ) X
                        ON X.ITEM_CODE = Z.ITEM_CODE
                        GROUP BY Z.ITEM_CODE, Z.ITEM_NAME, Z.ALLOWORDITEM, Z.FEATURE, Z.NAME

                        ";

            var result = SqlQuery<SearchWmsOrderPackingDetailRes>(sql, parm.ToArray());

            return result;
		}

		public IQueryable<F055002> GetDatasByOrdSeqs(string dcCode, string gupCode, string custCode, string ordNo, List<string> ordSeqs)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", ordNo) { SqlDbType = System.Data.SqlDbType.VarChar });

			var sql = $@" SELECT * FROM F055002
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2
							AND ORD_NO = @p3 ";
			sql += sqlParameter.CombineSqlInParameters(" AND ORD_SEQ", ordSeqs, System.Data.SqlDbType.VarChar);

			return SqlQuery<F055002>(sql, sqlParameter.ToArray());
		}

		public IQueryable<F055002> GetDatasByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar });

			var sql = $@" SELECT * FROM F055002
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2 ";
			sql += sqlParameter.CombineSqlInParameters(" AND WMS_ORD_NO", wmsOrdNos, System.Data.SqlDbType.VarChar);

			return SqlQuery<F055002>(sql, sqlParameter.ToArray());
		}
	}
}