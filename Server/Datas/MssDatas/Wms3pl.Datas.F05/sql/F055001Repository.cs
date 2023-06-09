using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F055001Repository : RepositoryBase<F055001, Wms3plDbContext, F055001Repository>
    {
        public IQueryable<F055001Data> GetConsignData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string packageBoxNo)
        {
#if DEBUG
            var sql2 = " AND L.ISTEST = '1' ";
            var sql3 = " AND Q.ISTEST = '1' ";
#else
			var sql2 = " AND L.ISTEST = '0' ";
			var sql3 = " AND Q.ISTEST = '0' ";
#endif
            var sql = $@"
                        SELECT ROW_NUMBER()OVER(ORDER BY A.WMS_ORD_NO, A.PACKAGE_BOX_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE ASC) ROWNUM,
                        	   A.DC_CODE,
                               A.GUP_CODE,
                               A.CUST_CODE,
                               CASE WHEN C.ESHOP = SUBSTRING(A.PAST_NO,1,3)  THEN
                                SUBSTRING(A.PAST_NO,4, 1) 
                               ELSE 
                                A.PAST_NO
                               END PAST_NO,
                               CONVERT(varchar, B.DELV_DATE, 112) AS DELV_DATE,
                               CONVERT(varchar, C.ARRIVAL_DATE, 111) AS ARRIVAL_DATE,
                               C.CUST_ORD_NO,
                               C.COLLECT,
                               C.CONSIGNEE,
                               C.ADDRESS,
                               C.TEL,
                               C.MEMO,
                               CASE
                                  WHEN     A.PACKAGE_BOX_NO = 1               
                                  THEN
                                     ISNULL (B.COLLECT_AMT, 0)
                                  ELSE
                                     0
                               END
                                  AS COLLECT_AMT,
                               ISNULL (B.SA_QTY, 0) AS SA_QTY,
                               E.ERST_NO,
                               F.SHORT_NAME,
                               H.TEL AS CUST_TEL,
                               H.ADDRESS AS CUST_ADDRESS,
                               1 AS TOTAL_AMOUNT,
                               E.ROUTE,
                               '001' AS FIXED_CODE,
                               G.ADDRESS_TYPE,
                               C.RETAIL_CODE,
                               C.RETAIL_NAME,
                               C.CUST_NAME,
                               C.CHANNEL,
                               CONVERT(varchar, dbo.GetSysDate(), 120) AS PRINT_TIME,
                               I.CONSIGN_ID,I.MEMO AS CONSIGN_MEMO,I.CONSIGN_NAME,
                               C.RETAIL_DELV_DATE,
                               C.RETAIL_RETURN_DATE,
                               C.ALL_ID,
                               C.ORD_NO,
                               CONVERT(varchar, B.DELV_DATE, 111) AS TCAT_DELV_DATE,
                               CONVERT(varchar, ISNULL(N.EXP_DELV_DATE,C.ARRIVAL_DATE), 111) AS TCAT_ARRIVAL_DATE,
                               C.MEMO TCAT_MEMO,
                               '' TCAT_PLACE,
                               CASE WHEN K.DEFAULT_BOXSIZE IS NULL
                               THEN CASE WHEN M.DEFAULT_BOXSIZE IS NULL
                                    THEN CASE WHEN O.DEFAULT_BOXSIZE IS NULL
                                        THEN CASE WHEN P.DEFAULT_BOXSIZE IS NULL
                                            THEN ''
                                            ELSE CONVERT(varchar,P.DEFAULT_BOXSIZE) END
                                        ELSE CONVERT(varchar,O.DEFAULT_BOXSIZE) END
                                    ELSE CONVERT(varchar,M.DEFAULT_BOXSIZE) END
                               ELSE CONVERT(varchar,K.DEFAULT_BOXSIZE) END TCAT_SIZE,
                               J.NAME TCAT_TIME,
                               ISNULL(K.CHANNEL_NAME,ISNULL(M.CHANNEL_NAME,ISNULL(O.CHANNEL_NAME,ISNULL(P.CHANNEL_NAME, H.DC_NAME)))) CHANNEL_NAME,
                               ISNULL(K.CHANNEL_ADDRESS,ISNULL(M.CHANNEL_ADDRESS,ISNULL(O.CHANNEL_ADDRESS,ISNULL(P.CHANNEL_ADDRESS,H.ADDRESS)))) CHANNEL_ADDRESS,
                               ISNULL(K.CHANNEL_TEL,ISNULL(M.CHANNEL_TEL,ISNULL(O.CHANNEL_TEL,ISNULL(P.CHANNEL_TEL,H.TEL)))) CHANNEL_TEL,
                               '' PAST_NOByCode128,
                               L.CUSTOMER_ID,
                               C.ESERVICE,
                        			 C.ESERVICE_NAME,
                        			 C.ESHOP,
                        			 C.ESHOP_ID,
                        			 C.PLATFORM_NAME,
                        			 C.VNR_NAME,
                        			 C.CUST_INFO,
                        			 C.NOTE1,
                        			 C.NOTE2,
                        			 C.NOTE3,
                               C.BARCODE_TYPE,
                               C.ISPRINTSTAR,
                               C.LAB_VNR_NAME,
                               C.LAB_NOTE1,
                               C.LAB_NOTE2,
                               C.LAB_NOTE3,
                               C.LAB_CUST_INFO,
                               CASE
                                  WHEN A.PACKAGE_BOX_NO = 1 AND  ISNULL (B.COLLECT_AMT, 0) > 0               
                                  THEN
                                     C.UNPAID_NOTE
                                  ELSE
                                     C.PAID_NOTE
                               END
                                  AS SHOW_ISPAID_NOTE,
                                  N.INVOICE,
                                  CONVERT(varchar,N.INVOICE_DATE,111) INVOICE_DATE,
                                  N.IDENTIFIER,
                        		  N.CONCENTRATED_NO,
                        		  N.CONCENTRATED,
                        		  N.SHIPPING_AREA_NO,
                        		  N.SHIPPINGCITY,
                        		  N.SELLER_NAME,
                        		  CASE WHEN N.SHIPPING_FLAG = '0' THEN N'虚出' ELSE N'实出' END SHIPPING_FLAG,
                        		  N.PACK_WEIGHT,
                        		  N.PACK_INSURANCE,
                              ISNULL(Q.LOGCENTER_ID,R.LOGCENTER_ID) LOGCENTER_ID,
                              A.PACKAGE_BOX_NO
                              FROM F055001 A
                               LEFT JOIN F050801 B
                                  ON     A.DC_CODE = B.DC_CODE
                                     AND A.GUP_CODE = B.GUP_CODE
                                     AND A.CUST_CODE = B.CUST_CODE
                                     AND A.WMS_ORD_NO = B.WMS_ORD_NO
                               LEFT JOIN
                               (SELECT AA.ORD_NO,
                                       AA.DC_CODE,
                                       AA.GUP_CODE,
                                       AA.CUST_CODE,
                                       AA.ARRIVAL_DATE,
                                       AA.CUST_ORD_NO,
                                       AA.COLLECT,
                                       AA.CONSIGNEE,
                                       AA.ADDRESS,
                                       AA.TEL_1 TEL,
                                       AA.MEMO,
                                       ISNULL(DD.DELV_RETAILCODE,AA.RETAIL_CODE) RETAIL_CODE,
                                       ISNULL(REPLACE(REPLACE(DD.DELV_RETAILNAME, char(9), ''),'{"\""}',''),CC.RETAIL_NAME) RETAIL_NAME,
                                       AA.CUST_NAME,
                                       AA.CHANNEL,
                                       AA.SUBCHANNEL,
                                       BB.WMS_ORD_NO,
                                       CONVERT(varchar,DD.DELV_DATE,111) RETAIL_DELV_DATE,
                                       CONVERT(varchar,DD.RETURN_DATE,111) RETAIL_RETURN_DATE,
                                       AA.ALL_ID,
                                       DD.ESERVICE,
                                       EE.ESERVICE_NAME,
                                       EE.ESHOP,
                                       EE.ESHOP_ID,
                                       EE.PLATFORM_NAME,
                                       EE.VNR_NAME,
                                       EE.CUST_INFO,
                                       EE.NOTE1,
                                       EE.NOTE2,
                                       EE.NOTE3,
                                       EE.PAID_NOTE,
                                       EE.UNPAID_NOTE,
                                       EE.BARCODE_TYPE,
                                       EE.ISPRINTSTAR,
                                       EE.LAB_VNR_NAME,
                                       EE.LAB_NOTE1,
                                       EE.LAB_NOTE2,
                                       EE.LAB_NOTE3,
                                       EE.LAB_CUST_INFO
                                  FROM F050301 AA
                                       LEFT JOIN F05030101 BB
                                          ON     AA.DC_CODE = BB.DC_CODE
                                             AND AA.GUP_CODE = BB.GUP_CODE
                                             AND AA.CUST_CODE = BB.CUST_CODE
                                             AND AA.ORD_NO = BB.ORD_NO
                                        LEFT JOIN F1909 CU
                                          ON CU.GUP_CODE = AA.GUP_CODE
                                         AND CU.CUST_CODE = AA.CUST_CODE
                                        LEFT JOIN F1910 CC
                                          ON CC.GUP_CODE = AA.GUP_CODE 
                                         AND CC.RETAIL_CODE = AA.RETAIL_CODE
                                         AND CC.CUST_CODE = CASE WHEN CU.ALLOWGUP_RETAILSHARE ='1' THEN '0' ELSE AA.CUST_CODE END
                                        LEFT JOIN F050304 DD
                                          ON DD.DC_CODE = AA.DC_CODE 
                                         AND DD.GUP_CODE = AA.GUP_CODE
                                         AND DD.CUST_CODE = AA.CUST_CODE
                                         AND DD.ORD_NO = AA.ORD_NO
                                        LEFT JOIN F194713 EE
                                          ON EE.DC_CODE = DD.DC_CODE
                                         AND EE.GUP_CODE = DD.GUP_CODE
                                         AND EE.CUST_CODE = DD.CUST_CODE
                                         AND EE.ESERVICE = DD.ESERVICE
                                         AND EE.ALL_ID = DD.ALL_ID) C
                                  ON     A.DC_CODE = C.DC_CODE
                                     AND A.GUP_CODE = C.GUP_CODE
                                     AND A.CUST_CODE = C.CUST_CODE
                                     AND A.WMS_ORD_NO = C.WMS_ORD_NO       
                               LEFT JOIN F050901 E
                                  ON     A.DC_CODE = E.DC_CODE
                                     AND A.GUP_CODE = E.GUP_CODE
                                     AND A.CUST_CODE = E.CUST_CODE
                                     AND A.PAST_NO = E.CONSIGN_NO
                                     AND A.WMS_ORD_NO = E.WMS_NO
                               LEFT JOIN F1909 F
                                  ON A.GUP_CODE = F.GUP_CODE AND A.CUST_CODE = F.CUST_CODE
                               LEFT JOIN F194706 G ON C.CHANNEL = G.CHANNEL
                               LEFT JOIN F1901 H ON H.DC_CODE = A.DC_CODE
                               LEFT JOIN F194711 I ON I.DC_CODE = A.DC_CODE AND I.GUP_CODE = A.GUP_CODE AND I.CUST_CODE = A.CUST_CODE AND I.ALL_ID = B.ALL_ID
                               LEFT JOIN VW_F000904_LANG J ON J.TOPIC='F050301' AND J.SUBTOPIC='DELV_PERIOD' AND J.VALUE = B.DELV_PERIOD AND J.LANG = '{Current.Lang}'
                               LEFT JOIN F190905 K ON K.DC_CODE = C.DC_CODE AND K.GUP_CODE = C.GUP_CODE AND K.CUST_CODE = C.CUST_CODE AND K.ALL_ID = C.ALL_ID AND K.CHANNEL = C.CHANNEL AND K.SUBCHANNEL = C.SUBCHANNEL
                               LEFT JOIN F19471201 L ON L.DC_CODE = C.DC_CODE AND L.GUP_CODE = C.GUP_CODE AND L.CUST_CODE = C.CUST_CODE AND (L.CHANNEL = C.CHANNEL OR L.CHANNEL='00') AND L.ALL_ID = C.ALL_ID AND L.CONSIGN_NO = A.PAST_NO {sql2}
                               LEFT JOIN F190905 M ON M.DC_CODE = C.DC_CODE AND M.GUP_CODE = C.GUP_CODE AND M.CUST_CODE = C.CUST_CODE AND M.ALL_ID = C.ALL_ID AND M.CHANNEL = '00' AND M.SUBCHANNEL = C.SUBCHANNEL
                        	   LEFT JOIN F050103 N ON N.DC_CODE = C.DC_CODE AND N.GUP_CODE = C.GUP_CODE AND N.CUST_CODE = C.CUST_CODE AND N.ORD_NO = C.ORD_NO AND N.CUST_ORD_NO = C.CUST_ORD_NO
                        	   LEFT JOIN F190905 O ON O.DC_CODE = C.DC_CODE AND O.GUP_CODE = C.GUP_CODE AND O.CUST_CODE = C.CUST_CODE AND O.ALL_ID = C.ALL_ID AND O.CHANNEL = C.CHANNEL AND O.SUBCHANNEL = '00'
                        	   LEFT JOIN F190905 P ON P.DC_CODE = C.DC_CODE AND P.GUP_CODE = C.GUP_CODE AND P.CUST_CODE = C.CUST_CODE AND P.ALL_ID = C.ALL_ID AND P.CHANNEL = '00' AND P.SUBCHANNEL = '00'
                             LEFT JOIN F194710 Q ON Q.DC_CODE = C.DC_CODE AND Q.ALL_ID= C.ALL_ID AND Q.LOG_ID = E.DELIVID_SEQ_NAME AND Q.GUP_CODE = C.GUP_CODE AND Q.CUST_CODE = C.CUST_CODE {sql3}
                             LEFT JOIN F194710 R ON R.DC_CODE = C.DC_CODE AND R.ALL_ID= C.ALL_ID AND R.LOG_ID = E.DELIVID_SEQ_NAME AND R.GUP_CODE = '00' AND R.CUST_CODE='00' {sql3}
                         WHERE     A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND A.WMS_ORD_NO = @p3 ";

            var parameters = new List<object>
            {
              dcCode,
              gupCode,
              custCode,
              wmsOrdNo
            };

            if (!string.IsNullOrEmpty(packageBoxNo))
            {
                sql += "AND A.PACKAGE_BOX_NO = @p4 ";
                parameters.Add(packageBoxNo);
            }

            var result = SqlQuery<F055001Data>(sql, parameters.ToArray());

            return result;
        }

        /// <summary>
        /// 託運單商品明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <param name="pastNo"></param>
        /// <returns></returns>
        public IQueryable<F055002Data> GetConsignItemData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string pastNo)
        {
            var parameters = new List<object>
            {
              dcCode,
              gupCode,
              custCode,
              wmsOrdNo
            };

            string sql = @"
			SELECT ROW_NUMBER()OVER(ORDER BY A.WMS_ORD_NO, A.PACKAGE_BOX_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE, B.ITEM_CODE ASC) ROWNUM, 
				   A.WMS_ORD_NO,
				   A.PAST_NO,
				   B.ITEM_CODE,
				   C.ITEM_NAME,
				   B.PACKAGE_QTY,
				   D.ACC_UNIT_NAME ITEM_UNIT
			  FROM F055001 A
				   INNER JOIN F055002 B
					  ON     A.WMS_ORD_NO = B.WMS_ORD_NO
						 AND A.DC_CODE = B.DC_CODE
						 AND A.GUP_CODE = B.GUP_CODE
						 AND A.CUST_CODE = B.CUST_CODE
				   INNER JOIN F1903 C
					  ON     B.ITEM_CODE = C.ITEM_CODE
						 AND B.GUP_CODE = C.GUP_CODE
						 AND B.CUST_CODE = C.CUST_CODE
					LEFT JOIN F91000302 D
					  ON    D.ITEM_TYPE_ID = '001'
						 AND C.ITEM_UNIT = D.ACC_UNIT
			 WHERE A.DC_CODE = @p0 AND 
				   A.GUP_CODE = @p1 AND 
				   A.CUST_CODE = @p2 AND 
				   A.WMS_ORD_NO = @p3
			";

            if (!string.IsNullOrEmpty(pastNo))
            {
                sql += "AND A.PAST_NO = @p4 ";
                parameters.Add(pastNo);
            }

            var result = SqlQuery<F055002Data>(sql, parameters.ToArray());

            return result;
        }

        #region  新增<<重新取得託運單號>>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wmsOrdCode">更新--出貨單編號</param>
        /// <param name="orgOrdCode">條件--出貨單編號</param>
        /// <returns></returns>
        public void UpdateData(string dcCode, string gupCode, string custCode, string consignNo, string orgOrdCode)
        {

            var parameters = new List<object>
            {
                Current.Staff,
                Current.StaffName,
                consignNo,
                dcCode,
                gupCode,
                custCode,
                orgOrdCode
            };
            var sql = $@" UPDATE F055001
                       SET 
									UPD_DATE =dbo.GetSysDate(),
									UPD_STAFF = @p0, 
									UPD_NAME = @p1,
									PAST_NO = @p2
							WHERE   	DC_CODE =@p3
									AND GUP_CODE =@p4
									AND CUST_CODE =@p5
									AND WMS_ORD_NO = @p6
						
              ";
            ExecuteSqlCommand(sql, parameters.ToArray());

        }
        #endregion
        

		public HomeDeliveryOrderNumberData GetHomeDeliveryOrderNumberData(string dcCode,string gupCode,string custCode,string pastNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p3",pastNo){SqlDbType = System.Data.SqlDbType.VarChar},
			};
			var sql = @" SELECT TOP (1) A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,A.PACKAGE_BOX_NO,A.STATUS,A.PAST_NO,A.WORKSTATION_CODE,A.BOX_NUM,C.LOGISTIC_CODE,C.LOGISTIC_NAME
										FROM F055001 A
										JOIN F050901 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.WMS_NO = A.WMS_ORD_NO
										AND B.CONSIGN_NO = A.PAST_NO
										JOIN F0002 C
										ON C.DC_CODE = B.DC_CODE
										AND C.LOGISTIC_CODE = B.DELIVID_SEQ_NAME
										WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2
										AND A.PAST_NO = @p3
										ORDER BY A.CRT_DATE DESC
                 ";
			return SqlQuery<HomeDeliveryOrderNumberData>(sql, parms.ToArray()).FirstOrDefault();
		}

		public void UpdateToAudit(string dcCode,string gupCode,string custCode,string wmsOrdNo,short packageBoxNo,string status,DateTime auditTime,string auditStaff,string auditName,string boxNum =null)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",status){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p1",auditTime){SqlDbType = System.Data.SqlDbType.DateTime2},
				new SqlParameter("@p2",auditStaff){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p3",auditName){SqlDbType = System.Data.SqlDbType.NVarChar},
				new SqlParameter("@p4",dcCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p5",gupCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p6",custCode){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p7",wmsOrdNo){SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p8",packageBoxNo){SqlDbType = System.Data.SqlDbType.SmallInt},
			};
			var boxNumSql = string.Empty;
			if (!string.IsNullOrEmpty(boxNum))
			{
				boxNumSql = ",BOX_NUM = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, boxNum) { SqlDbType = System.Data.SqlDbType.VarChar });
			}

			var sql = $@" UPDATE F055001 
                    SET STATUS = @p0,AUDIT_DATE = @p1,AUDIT_STAFF = @p2, AUDIT_NAME = @p3 {boxNumSql} 
                    WHERE DC_CODE = @p4
                      AND GUP_CODE = @p5
                      AND CUST_CODE = @p6
                      AND WMS_ORD_NO = @p7
                      AND PACKAGE_BOX_NO = @p8 ";
			ExecuteSqlCommand(sql, parms.ToArray());
		}
	}
}