using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05030201Repository : RepositoryBase<F05030201, Wms3plDbContext, F05030201Repository>
	{
        public IQueryable<F050802Report> GetShippingReportByBomItem(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", dcCode));
            param.Add(new SqlParameter("@p1", gupCode));
            param.Add(new SqlParameter("@p2", custCode));
            var sql = @" SELECT DISTINCT D.DELV_DATE,D.PICK_TIME,A.ORD_NO,D.WMS_ORD_NO,C.CUST_ORD_NO,A.BOM_ITEM_CODE ITEM_CODE,E.ITEM_NAME,
                                   (A.ORD_QTY/A.BOM_QTY) B_DELV_QTY,F.ALL_COMP,D.RETAIL_CODE,D.CUST_NAME,E.ITEM_SIZE,E.ITEM_SPEC,
                                   E.ITEM_COLOR,G.ACC_UNIT_NAME AS ITEM_UNIT
										 FROM F05030201 A
										 JOIN F05030101 B
										  ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE =A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.ORD_NO = A.ORD_NO
									  JOIN F050301 C
										  ON C.DC_CODE = B.DC_CODE
										 AND C.GUP_CODE = B.GUP_CODE
										 AND C.CUST_CODE = B.CUST_CODE
										 AND C.ORD_NO = B.ORD_NO
									  JOIN F050801 D
										  ON D.DC_CODE = B.DC_CODE
										 AND D.GUP_CODE = B.GUP_CODE
										 AND D.CUST_CODE = B.CUST_CODE
										 AND D.WMS_ORD_NO = B.WMS_ORD_NO
									  JOIN F1903 E
										  ON E.GUP_CODE = A.GUP_CODE
										 AND E.ITEM_CODE = A.BOM_ITEM_CODE
                                         AND E.CUST_CODE = A.CUST_CODE
									  JOIN F1947 F
										  ON F.DC_CODE = D.DC_CODE
										 AND F.ALL_ID = D.ALL_ID
									  LEFT JOIN F91000302 G 
										  ON G.ITEM_TYPE_ID ='001' 
                     AND G.ACC_UNIT = E.ITEM_UNIT
										WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2 ";
            var insql = new List<string>();
            foreach (var item in wmsOrdNos)
            {
                insql.Add("@p" + param.Count);
                param.Add(new SqlParameter("@p" + param.Count, item));
            }
            sql += string.Format(" AND D.WMS_ORD_NO IN ({0}) ", string.Join(",", insql.ToArray()));
            var result = SqlQuery<F050802Report>(sql, param.ToArray());

            return result;
        }

        public IQueryable<DeliveryReport> GetDeliveryReportByBomItem(string dcCode, string gupCode, string custCode, string wmsOrdNo, short? packageBoxNo = null)
        {
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", dcCode));
            param.Add(new SqlParameter("@p1", gupCode));
            param.Add(new SqlParameter("@p2", custCode));
            param.Add(new SqlParameter("@p3", wmsOrdNo));
            param.Add(new SqlParameter("@p4", packageBoxNo == null ? (object)DBNull.Value : packageBoxNo));

            var sql = @" SELECT D.PACKAGE_BOX_NO AS PackageBoxNo,
                          D.ITEM_CODE ItemCode,
                          D.PAST_NO AS BoxNo,
                          D.PackQty,
                          ISNULL (G.ITEM_NICKNAME, G.ITEM_NAME) ItemName,
                          J.CONSIGNEE,
                          K.ACC_UNIT_NAME AS ITEM_UNIT,
                          G.ITEM_SIZE,
                          G.ITEM_SPEC,
                          G.ITEM_COLOR
                     FROM (
										 SELECT DC_CODE,GUP_CODE,CUST_CODE,WMS_ORD_NO,PAST_NO,PACKAGE_BOX_NO,ITEM_CODE,SUM(PackQty) PackQty
											 FROM(
											SELECT DISTINCT K.DC_CODE,
                                      K.GUP_CODE,
                                      K.CUST_CODE,
                                      K.WMS_ORD_NO,
                                      K.PAST_NO,
                                      K.PACKAGE_BOX_NO,
                                      E.BOM_ITEM_CODE ITEM_CODE,
                                      E.ORD_QTY/E.BOM_QTY  AS PackQty
                        FROM F055001 K
                        JOIN F055002 L
                          ON K.DC_CODE = L.DC_CODE
                         AND K.GUP_CODE = L.GUP_CODE
                         AND K.CUST_CODE = L.CUST_CODE
                         AND K.WMS_ORD_NO = L.WMS_ORD_NO
                         AND K.PACKAGE_BOX_NO = L.PACKAGE_BOX_NO
                        JOIN F05030101 D
                          ON D.DC_CODE = K.DC_CODE
                         AND D.GUP_CODE = K.GUP_CODE
                         AND D.CUST_CODE = K.CUST_CODE
                         AND D.WMS_ORD_NO = K.WMS_ORD_NO 
                        JOIN F05030201 E
                          ON E.DC_CODE = D.DC_CODE
                         AND E.GUP_CODE = D.GUP_CODE
                         AND E.CUST_CODE = D.CUST_CODE
                         AND E.ORD_NO = D.ORD_NO
                       WHERE K.DC_CODE = @p0
                         AND K.GUP_CODE = @p1
                         AND K.CUST_CODE = @p2
                         AND K.WMS_ORD_NO = @p3
                         AND K.PACKAGE_BOX_NO = CASE WHEN @p4 = '' OR @p4 IS NULL THEN K.PACKAGE_BOX_NO ELSE @p4 END 
                         AND K.PACKAGE_BOX_NO = ISNULL ( @p4, K.PACKAGE_BOX_NO)
                         AND L.PACKAGE_QTY > 0)A
                       GROUP BY A.DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                A.WMS_ORD_NO,
                                A.PAST_NO,
                                A.PACKAGE_BOX_NO,
                                A.ITEM_CODE) D
                   LEFT JOIN F1903 G
                     ON D.ITEM_CODE = G.ITEM_CODE
                    AND D.GUP_CODE = G.GUP_CODE
                    AND D.CUST_CODE = G.CUST_CODE
                  LEFT JOIN F91000302 K
                    ON G.ITEM_UNIT = K.ACC_UNIT AND ITEM_TYPE_ID = '001'
                  LEFT JOIN
                  (
                                SELECT M.WMS_ORD_NO,
										   M.DC_CODE,
										   M.GUP_CODE,
										   M.CUST_CODE,
										   M.CONSIGNEE,
                                           N.RETAIL_CODE,
                                           N.SHORT_SALESBASE_NAME
									  FROM (
                                            SELECT H.WMS_ORD_NO,
                                                  H.DC_CODE,
                                                  H.GUP_CODE,
                                                  H.CUST_CODE,
                                                  I.CONSIGNEE
                                             FROM F05030101 H
                                             LEFT JOIN F050101 I
                                               ON H.DC_CODE = I.DC_CODE
                                              AND H.GUP_CODE = I.GUP_CODE
                                              AND H.CUST_CODE = I.CUST_CODE
                                              AND H.ORD_NO = I.ORD_NO
                                            GROUP BY H.WMS_ORD_NO,
                                                     H.DC_CODE,
                                                     H.GUP_CODE,
                                                     H.CUST_CODE,
                                                     I.CONSIGNEE
                                            ) M 
                                         LEFT JOIN (
                                            SELECT distinct A.WMS_ORD_NO,A.CUST_CODE,A.GUP_CODE,A.DC_CODE,A.RETAIL_CODE,B.SHORT_SALESBASE_NAME  FROM F050801 A , F1910 B , F1909 C
                                                            WHERE A.GUP_CODE=B.GUP_CODE
                                                              AND A.GUP_CODE=C.GUP_CODE
                                                              AND A.CUST_CODE=C.CUST_CODE
                                                              AND CASE WHEN C.ALLOWGUP_RETAILSHARE = '1' THEN '0' ELSE C.CUST_CODE END = B.CUST_CODE
                                                              AND A.RETAIL_CODE = B.RETAIL_CODE
                                            ) N 
                                           ON M.DC_CODE = N.DC_CODE
                                          AND M.GUP_CODE = N.GUP_CODE
                                          AND M.CUST_CODE = N.CUST_CODE
                                          AND M.WMS_ORD_NO = N.WMS_ORD_NO
                  ) J
											ON D.DC_CODE = J.DC_CODE
										 AND D.GUP_CODE = J.GUP_CODE
										 AND D.CUST_CODE = J.CUST_CODE
										 AND D.WMS_ORD_NO = J.WMS_ORD_NO
                   ORDER BY D.PACKAGE_BOX_NO, D.ITEM_CODE ";

            var result = SqlQuery<DeliveryReport>(sql, param.ToArray());
            return result;
        }
    }
}
