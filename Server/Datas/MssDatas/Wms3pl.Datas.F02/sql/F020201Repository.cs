using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020201Repository : RepositoryBase<F020201, Wms3plDbContext, F020201Repository>
    {
        /// <summary>
		/// 取得商品檢驗驗收單報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public IQueryable<AcceptancePurchaseReport> GetAcceptancePurchaseReport(string dcCode, string gupCode, string custCode
            , string purchaseNo, string rtNo, string commCustCode, bool isDefect)
        {
            var sumRecvQty = "";
            var warehouseId = "";
            var warehouseName = "";
            var existOrNotExistWarehourse = "";  // 切換是否只找不良品倉

            if (!isDefect)
            {
                sumRecvQty = "F020201.RECV_QTY-ISNULL(C.DEFECT_QTY,0) AS SUM_RECV_QTY,";
                warehouseId = "F020201.TARWAREHOUSE_ID AS TARWAREHOUSE_ID,";
                warehouseName = " (SELECT TOP(1) WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = F020201.DC_CODE AND WAREHOUSE_ID = F020201.TARWAREHOUSE_ID) WAREHOUSE_NAME, ";
                existOrNotExistWarehourse = @" NOT ";
            }
            else
            {
                sumRecvQty = "ISNULL(C.DEFECT_QTY, 0) AS SUM_RECV_QTY,";
                warehouseId = "C.WAREHOUSE_ID AS TARWAREHOUSE_ID,";
                warehouseName = " (SELECT TOP(1) WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = C.DC_CODE AND WAREHOUSE_ID = C.WAREHOUSE_ID) WAREHOUSE_NAME, ";

            }
            string sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY CONVERT(decimal, A.RT_SEQ)) AS ROW_NUM, 
					A.*
							  FROM (  SELECT F020201.RT_NO,
											 F020201.RT_SEQ,
											 B.BOSS BUSPER,
											 F010201.VNR_CODE,
											 B.VNR_NAME,
                                             REPLACE(CONVERT(char(5), F020201.IN_DATE, 108), ':', '') AS RECV_TIME,  -- 到貨時間
											 '' AS NAME,
											 '' AS FAX, --, F1908.BUS_FAX AS FAX,
											 F010201.CUST_ORD_NO AS ORG_ORDER_NO,
											 F020201.RT_NO AS ORDER_NO,
											 ISNULL(F1924.EMP_NAME,F020201.CRT_NAME) EMP_NAME,
											 B.ITEM_TEL AS ORD_TEL,
											 CONVERT(varchar,F020201.RECE_DATE,111) AS ORDER_DATE,
											 ISNULL (CONVERT(CHAR,F010202.STOCK_SEQ), '') AS ORDER_SEQ,
											 (SELECT NAME FROM F000904 
											 WHERE F000904.TOPIC = 'F010201' 
											 AND  F000904.SUBTOPIC ='FAST_PASS_TYPE' 
											 AND F000904.VALUE = F010201.FAST_PASS_TYPE) FAST_PASS_TYPE,
											 F010202.ITEM_CODE,
											 F1903.EAN_CODE1,
											 F1903.EAN_CODE2,
											 F1903.EAN_CODE3,
											 F1903.ITEM_NAME AS ITEM_NAME,
											 F1903.ITEM_COLOR AS ITEM_COLOR,
											 F1903.ITEM_SIZE AS ITEM_SIZE,
											 F1903.ITEM_SPEC AS ITEM_SPEC,
											 0 AS PACK_QTY1, 
											 F91000302.ACC_UNIT_NAME AS ORDER_UNIT,
											 1 AS CASE_QTY, 
											 ISNULL (F010202.STOCK_QTY, 0) AS ORDER_QTY,
											 0 AS AMOUNT,
											 F020201.MADE_DATE AS ACE_DATE1,
											 --F020201.VALI_DATE AS ACE_DATE2,
											case when F020201.VALI_DATE = null then ''                                              
                                                  else CONVERT (varchar,F020201.VALI_DATE, 111) 
                                             end ACE_DATE2,
											 F1901.ADDRESS,
											 ISNULL (CONVERT(CHAR,F020201.RECV_QTY), '') AS GUP_CODE     /* 驗收數 */
											 ,
											 F1903.RET_UNIT,
											 TRIM (F1903.CUST_ITEM_CODE) CUST_ITEM_CODE,
											 '' AS VEN_ITEM_CODE,
                                             CASE
											 WHEN F020201.QUICK_CHECK='0'
											 then ''
											 else 'V'
											 end as QUICK_CHECK,
											{warehouseId}
											{warehouseName}
                                             F020201.MAKE_NO,
                                             ISNULL (F020201.RECV_QTY, 0) AS RECV_QTY,
											{sumRecvQty}
                                             F1905.PACK_WEIGHT ,
											F.ALLOCATION_NO
										FROM F010201
										LEFT JOIN F010202
										ON     F010201.DC_CODE = F010202.DC_CODE
											AND F010201.GUP_CODE = F010202.GUP_CODE
											AND F010201.CUST_CODE = F010202.CUST_CODE
											AND F010201.STOCK_NO = F010202.STOCK_NO
										LEFT JOIN F020201
										ON     F010202.DC_CODE = F020201.DC_CODE
											AND F010202.GUP_CODE = F020201.GUP_CODE
											AND F010202.CUST_CODE = F020201.CUST_CODE
											AND F010202.STOCK_NO = F020201.PURCHASE_NO
											AND F010202.STOCK_SEQ = F020201.PURCHASE_SEQ
										LEFT JOIN (select * from F1908 A where A.CUST_CODE = @p5) B
										ON     F010201.GUP_CODE = B.GUP_CODE
											AND F010201.VNR_CODE = B.VNR_CODE
                                        LEFT JOIN F1903
										ON     F010201.GUP_CODE = F1903.GUP_CODE
											AND F010201.CUST_CODE = F1903.CUST_CODE
											AND F010202.ITEM_CODE = F1903.ITEM_CODE
										LEFT JOIN F91000302 ON F91000302.ITEM_TYPE_ID ='001' AND F1903.ITEM_UNIT = F91000302.ACC_UNIT
										LEFT JOIN F1924 ON F020201.CRT_STAFF = F1924.EMP_ID
										LEFT JOIN F1901 ON F010201.DC_CODE = F1901.DC_CODE                    
										LEFT JOIN (select F020201.DC_CODE,F020201.GUP_CODE,F020201.CUST_CODE,F020201.PURCHASE_NO,F020201.RT_NO,F020201.RT_SEQ,SUM(F02020109.DEFECT_QTY) DEFECT_QTY,F02020109.WAREHOUSE_ID from F020201 JOIN F02020109
										ON F020201.DC_CODE = F02020109.DC_CODE 
										AND F020201.GUP_CODE = F02020109.GUP_CODE 
										AND F020201.CUST_CODE =F02020109.CUST_CODE 
										AND F020201.RT_NO = F02020109.RT_NO
										AND F020201.RT_SEQ  = F02020109.RT_SEQ 
										GROUP BY F020201.DC_CODE,F020201.GUP_CODE,F020201.CUST_CODE,F020201.PURCHASE_NO,F020201.RT_NO,F020201.RT_SEQ,F02020109.WAREHOUSE_ID) C
										ON F020201.DC_CODE = C.DC_CODE 
										AND F020201.GUP_CODE = C.GUP_CODE 
										AND F020201.CUST_CODE =C.CUST_CODE 
										AND F020201.RT_NO = C.RT_NO
										AND F020201.RT_SEQ = C.RT_SEQ
										JOIN F1905
										ON F1903.GUP_CODE = F1905.GUP_CODE 
										AND F1903.CUST_CODE  = F1905.CUST_CODE 
										AND F1903.ITEM_CODE =F1905.ITEM_CODE 
										JOIN(SELECT  F02020108.DC_CODE,F02020108.GUP_CODE ,F02020108.CUST_CODE,F02020108.ALLOCATION_NO,F151001.TAR_WAREHOUSE_ID,F02020108.RT_NO,F02020108.RT_SEQ from F02020108 
										JOIN F151001 on 
										F02020108.DC_CODE = F151001.DC_CODE 
										AND F02020108.GUP_CODE = F151001.GUP_CODE 
										AND F02020108.CUST_CODE = F151001.CUST_CODE 
										AND F02020108.ALLOCATION_NO = F151001.ALLOCATION_NO
										WHERE {existOrNotExistWarehourse} EXISTS (SELECT 1 from F02020109
										where F02020109.DC_CODE = F151001.DC_CODE
										and F02020109.GUP_CODE = F151001.GUP_CODE
										and F02020109.CUST_CODE = F151001.CUST_CODE
										AND F02020109.STOCK_NO = F02020108.STOCK_NO
                    AND F02020109.DC_CODE = F151001.DC_CODE
                    AND F02020109.GUP_CODE = F151001.GUP_CODE
                    AND F02020109.CUST_CODE = F151001.CUST_CODE
										AND F02020109.WAREHOUSE_ID = F151001.TAR_WAREHOUSE_ID)
										) F
										ON F020201.DC_CODE = F.DC_CODE
										AND F020201.GUP_CODE = F.GUP_CODE
										AND F020201.CUST_CODE = F.CUST_CODE
										AND F020201.RT_NO = F.RT_NO
										AND F020201.RT_SEQ = F.RT_SEQ
									   WHERE     F010201.DC_CODE = @p0
											 AND F010201.GUP_CODE = @p1
											 AND F010201.CUST_CODE = @p2
											 AND F010201.STOCK_NO = @p3
											 AND F020201.RT_NO = @p4
									) A 
									WHERE A.SUM_RECV_QTY>0
									GROUP BY  A.RT_NO,A.RT_SEQ,A.BUSPER,A.VNR_CODE,A.VNR_NAME,
									RECV_TIME,A.NAME,A.FAX,A.ORG_ORDER_NO,A.ORDER_NO,A.EMP_NAME,
									A.ORD_TEL,A.ORDER_DATE,A.ORDER_SEQ,A.FAST_PASS_TYPE,
									A.ITEM_CODE,A.EAN_CODE1,A.EAN_CODE2,A.EAN_CODE3,A.ITEM_NAME,
									A.ITEM_COLOR,A.ITEM_SIZE,A.ITEM_SPEC,A.CASE_QTY,A.AMOUNT,A.ORDER_UNIT,
									A.PACK_QTY1,A.ORDER_QTY,A.AMOUNT,A.ACE_DATE1,A.ACE_DATE2,A.ADDRESS,A.GUP_CODE,A.RET_UNIT,
									A.CUST_ITEM_CODE,A.VEN_ITEM_CODE,A.QUICK_CHECK,A.TARWAREHOUSE_ID,A.WAREHOUSE_NAME,
									A.MAKE_NO,A.RECV_QTY,A.SUM_RECV_QTY,A.PACK_WEIGHT,A.ALLOCATION_NO";
            var param = new[] {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", purchaseNo),
                new SqlParameter("@p4", rtNo),
                new SqlParameter("@p5",commCustCode)
            };

            return SqlQuery<AcceptancePurchaseReport>(sql, param);
        }

		/// <summary>
		/// 取得商品檢驗與容器明細驗收單報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="commCustCode"></param>
		/// <param name="isDefect"></param>
		/// <returns></returns>
		public IQueryable<AcceptancePurchaseReport> GetAcceptancePurchaseContainerReport(string dcCode, string gupCode, string custCode
			, string purchaseNo, string rtNo, string commCustCode, bool isDefect)
		{
			var sumRecvQty = "";
			var warehouseId = "";
			var warehouseName = "";
			var existOrNotExistWarehourse = "";  // 切換是否只找不良品倉

			if (!isDefect)
			{
				sumRecvQty = "F020201.RECV_QTY-ISNULL(C.DEFECT_QTY,0) AS SUM_RECV_QTY,";
				warehouseId = "F0205.PICK_WARE_ID AS TARWAREHOUSE_ID,";
				warehouseName = " (SELECT TOP(1) WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = F0205.DC_CODE AND WAREHOUSE_ID = F0205.PICK_WARE_ID) WAREHOUSE_NAME, ";
				existOrNotExistWarehourse = @" NOT ";
			}
			else
			{
				sumRecvQty = "ISNULL(C.DEFECT_QTY, 0) AS SUM_RECV_QTY,";
				warehouseId = "C.WAREHOUSE_ID AS TARWAREHOUSE_ID,";
				warehouseName = " (SELECT TOP(1) WAREHOUSE_NAME FROM F1980 WHERE DC_CODE = C.DC_CODE AND WAREHOUSE_ID = C.WAREHOUSE_ID) WAREHOUSE_NAME, ";

			}
			string sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY CONVERT(decimal, A.RT_SEQ)) AS ROW_NUM, 
					A.*
							  FROM (  SELECT F020201.RT_NO,
											 F020201.RT_SEQ,
											 B.BOSS BUSPER,
											 F010201.VNR_CODE,
											 B.VNR_NAME,
                                             REPLACE(CONVERT(char(5), F020201.IN_DATE, 108), ':', '') AS RECV_TIME,  -- 到貨時間
											 '' AS NAME,
											 '' AS FAX, --, F1908.BUS_FAX AS FAX,
											 F010201.CUST_ORD_NO AS ORG_ORDER_NO,
											 F020201.RT_NO AS ORDER_NO,
											 ISNULL(F1924.EMP_NAME,F020201.CRT_NAME) EMP_NAME,
											 B.ITEM_TEL AS ORD_TEL,
											 CONVERT(varchar,F020201.RECE_DATE,111) AS ORDER_DATE,
											 ISNULL (CONVERT(CHAR,F010202.STOCK_SEQ), '') AS ORDER_SEQ,
											 (SELECT NAME FROM F000904 
											 WHERE F000904.TOPIC = 'F010201' 
											 AND  F000904.SUBTOPIC ='FAST_PASS_TYPE' 
											 AND F000904.VALUE = F010201.FAST_PASS_TYPE) FAST_PASS_TYPE,
											 F010202.ITEM_CODE,
											 F1903.EAN_CODE1,
											 F1903.EAN_CODE2,
											 F1903.EAN_CODE3,
											 F1903.ITEM_NAME AS ITEM_NAME,
											 F1903.ITEM_COLOR AS ITEM_COLOR,
											 F1903.ITEM_SIZE AS ITEM_SIZE,
											 F1903.ITEM_SPEC AS ITEM_SPEC,
											 0 AS PACK_QTY1, 
											 F91000302.ACC_UNIT_NAME AS ORDER_UNIT,
											 1 AS CASE_QTY, 
											 ISNULL (F010202.STOCK_QTY, 0) AS ORDER_QTY,
											 0 AS AMOUNT,
											 F020201.MADE_DATE AS ACE_DATE1,
											 --F020201.VALI_DATE AS ACE_DATE2,
											case when F020201.VALI_DATE = null then ''                                              
                                                  else CONVERT (varchar,F020201.VALI_DATE, 111) 
                                             end ACE_DATE2,
											 F1901.ADDRESS,
											 ISNULL (CONVERT(CHAR,F020201.RECV_QTY), '') AS GUP_CODE     /* 驗收數 */
											 ,
											 F1903.RET_UNIT,
											 TRIM (F1903.CUST_ITEM_CODE) CUST_ITEM_CODE,
											 '' AS VEN_ITEM_CODE,
                                             CASE
											 WHEN F020201.QUICK_CHECK='0'
											 then ''
											 else 'V'
											 end as QUICK_CHECK,
											{warehouseId}
											{warehouseName}
                                             F020201.MAKE_NO,
                                             ISNULL (F020201.RECV_QTY, 0) AS RECV_QTY,
											{sumRecvQty}
                                             F1905.PACK_WEIGHT 
										FROM F010201
										LEFT JOIN F010202
										ON     F010201.DC_CODE = F010202.DC_CODE
											AND F010201.GUP_CODE = F010202.GUP_CODE
											AND F010201.CUST_CODE = F010202.CUST_CODE
											AND F010201.STOCK_NO = F010202.STOCK_NO
										LEFT JOIN F020201
										ON     F010202.DC_CODE = F020201.DC_CODE
											AND F010202.GUP_CODE = F020201.GUP_CODE
											AND F010202.CUST_CODE = F020201.CUST_CODE
											AND F010202.STOCK_NO = F020201.PURCHASE_NO
											AND F010202.STOCK_SEQ = F020201.PURCHASE_SEQ
										LEFT JOIN (select * from F1908 A where A.CUST_CODE = @p5) B
										ON     F010201.GUP_CODE = B.GUP_CODE
											AND F010201.VNR_CODE = B.VNR_CODE
                                        LEFT JOIN F1903
										ON     F010201.GUP_CODE = F1903.GUP_CODE
											AND F010201.CUST_CODE = F1903.CUST_CODE
											AND F010202.ITEM_CODE = F1903.ITEM_CODE
										LEFT JOIN F91000302 ON F91000302.ITEM_TYPE_ID ='001' AND F1903.ITEM_UNIT = F91000302.ACC_UNIT
										LEFT JOIN F1924 ON F020201.CRT_STAFF = F1924.EMP_ID
										LEFT JOIN F1901 ON F010201.DC_CODE = F1901.DC_CODE                    
										LEFT JOIN (select F020201.DC_CODE,F020201.GUP_CODE,F020201.CUST_CODE,F020201.PURCHASE_NO,F020201.RT_NO,F020201.RT_SEQ,SUM(F02020109.DEFECT_QTY) DEFECT_QTY,F02020109.WAREHOUSE_ID from F020201 JOIN F02020109
										ON F020201.DC_CODE = F02020109.DC_CODE 
										AND F020201.GUP_CODE = F02020109.GUP_CODE 
										AND F020201.CUST_CODE =F02020109.CUST_CODE 
										AND F020201.RT_NO = F02020109.RT_NO
										AND F020201.RT_SEQ  = F02020109.RT_SEQ 
										GROUP BY F020201.DC_CODE,F020201.GUP_CODE,F020201.CUST_CODE,F020201.PURCHASE_NO,F020201.RT_NO,F020201.RT_SEQ,F02020109.WAREHOUSE_ID) C
										ON F020201.DC_CODE = C.DC_CODE 
										AND F020201.GUP_CODE = C.GUP_CODE 
										AND F020201.CUST_CODE =C.CUST_CODE 
										AND F020201.RT_NO = C.RT_NO
										AND F020201.RT_SEQ = C.RT_SEQ
										JOIN F1905
										ON F1903.GUP_CODE = F1905.GUP_CODE 
										AND F1903.CUST_CODE  = F1905.CUST_CODE 
										AND F1903.ITEM_CODE =F1905.ITEM_CODE 
										JOIN F0205 
										ON F020201.DC_CODE = F0205.DC_CODE 
										AND F020201.GUP_CODE = F0205.GUP_CODE 
										AND F020201.CUST_CODE =F0205.CUST_CODE 
										AND F020201.RT_NO = F0205.RT_NO
										AND F020201.RT_SEQ  = F0205.RT_SEQ 
										AND F020201.PURCHASE_NO = F0205.STOCK_NO
										AND F020201.PURCHASE_SEQ  = F0205.STOCK_SEQ 
										AND F0205.PICK_WARE_ID {existOrNotExistWarehourse} IN (
										SELECT WAREHOUSE_ID from F02020109
										WHERE DC_CODE = F020201.DC_CODE 
										and GUP_CODE = F020201.GUP_CODE 
										AND CUST_CODE = F020201.CUST_CODE 
										AND STOCK_NO = F020201.PURCHASE_NO 
										AND STOCK_SEQ = F020201.PURCHASE_SEQ)
									   WHERE     F010201.DC_CODE = @p0
											 AND F010201.GUP_CODE = @p1
											 AND F010201.CUST_CODE = @p2
											 AND F010201.STOCK_NO = @p3
											 AND F020201.RT_NO = @p4
									) A 
									WHERE A.SUM_RECV_QTY>0
									GROUP BY  A.RT_NO,A.RT_SEQ,A.BUSPER,A.VNR_CODE,A.VNR_NAME,
									RECV_TIME,A.NAME,A.FAX,A.ORG_ORDER_NO,A.ORDER_NO,A.EMP_NAME,
									A.ORD_TEL,A.ORDER_DATE,A.ORDER_SEQ,A.FAST_PASS_TYPE,
									A.ITEM_CODE,A.EAN_CODE1,A.EAN_CODE2,A.EAN_CODE3,A.ITEM_NAME,
									A.ITEM_COLOR,A.ITEM_SIZE,A.ITEM_SPEC,A.CASE_QTY,A.AMOUNT,A.ORDER_UNIT,
									A.PACK_QTY1,A.ORDER_QTY,A.AMOUNT,A.ACE_DATE1,A.ACE_DATE2,A.ADDRESS,A.GUP_CODE,A.RET_UNIT,
									A.CUST_ITEM_CODE,A.VEN_ITEM_CODE,A.QUICK_CHECK,A.TARWAREHOUSE_ID,A.WAREHOUSE_NAME,
									A.MAKE_NO,A.RECV_QTY,A.SUM_RECV_QTY,A.PACK_WEIGHT";
			var param = new[] {
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", purchaseNo),
				new SqlParameter("@p4", rtNo),
				new SqlParameter("@p5",commCustCode)
			};

			return SqlQuery<AcceptancePurchaseReport>(sql, param);
		}

		public IQueryable<F020201Data> GetDatasByWaitOrUpLoc(string dcCode, DateTime receDate)
        {
            var sql = @" SELECT A.RT_NO ,A.RT_SEQ ,A.PURCHASE_NO,A.PURCHASE_SEQ,A.VNR_CODE ,A.ITEM_CODE ,A.RECE_DATE ,A.VALI_DATE ,A.MADE_DATE ,A.ORDER_QTY,A.RECV_QTY,
 A.CHECK_QTY,C.STATUS AS F151001_STATUS,D.STATUS AS F010201_STATUS ,A.CHECK_ITEM,A.CHECK_SERIAL,A.ISPRINT,A.ISUPLOAD,A.DC_CODE,A.GUP_CODE ,A.CUST_CODE ,A.CRT_STAFF ,A.CRT_DATE ,A.UPD_STAFF ,
 A.UPD_DATE,A.CRT_NAME,A.UPD_NAME,A.SPECIAL_DESC,A.SPECIAL_DESC,A.SPECIAL_CODE,A.ISSPECIAL,A.IN_DATE,A.TARWAREHOUSE_ID,A.QUICK_CHECK,A.MAKE_NO
			             FROM F020201 A
			            INNER JOIN F02020107 B 
			               ON B.DC_CODE = A.DC_CODE 
			              AND B.GUP_CODE = A.GUP_CODE 
			              AND B.CUST_CODE = A.CUST_CODE 
			              AND B.RT_NO = A.RT_NO 
			            INNER JOIN F151001 C 
			               ON C.DC_CODE = A.DC_CODE 
			              AND C.GUP_CODE = A.GUP_CODE 
			              AND C.CUST_CODE = A.CUST_CODE 
			              AND C.ALLOCATION_NO = B.ALLOCATION_NO 
                          JOIN F010201  D
			              ON D.DC_CODE = A.DC_CODE 
			              AND D.GUP_CODE = A.GUP_CODE 
			              AND D.CUST_CODE = A.CUST_CODE 
			              AND D.STOCK_NO  = A.PURCHASE_NO 
					WHERE A.DC_CODE = @p0 
                    AND A.RECE_DATE = @p1 ";
            var param = new object[] { dcCode, receDate.Date };
            return SqlQuery<F020201Data>(sql, param);
        }

        public IQueryable<DcWmsNoStatusItem> GetReceUnUpLocOver30MinDatasByDc(string dcCode, DateTime receDate)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.WMS_NO,A.STAFF,A.STAFF_NAME,A.START_DATE) AS[ROWNUM],
		            A.* 
                     FROM ( 
							SELECT A.PURCHASE_NO as WMS_NO,C.CRT_STAFF AS STAFF,C.CRT_STAFF + C.CRT_NAME AS STAFF_NAME,C.CRT_DATE AS START_DATE 
								FROM F020201 A
							INNER JOIN F02020107 B 
									ON B.DC_CODE = A.DC_CODE 
								AND B.GUP_CODE = A.GUP_CODE 
								AND B.CUST_CODE = A.CUST_CODE 
								AND B.RT_NO = A.RT_NO 
							INNER JOIN F151001 C 
									ON C.DC_CODE = A.DC_CODE 
								AND C.GUP_CODE = A.GUP_CODE 
								AND C.CUST_CODE = A.CUST_CODE 
								AND C.ALLOCATION_NO = B.ALLOCATION_NO 
							WHERE A.DC_CODE = @p0 
								AND A.RECE_DATE = @p1
								AND C.STATUS<>'5' 
								AND DateDiff(minute,@p2,C.CRT_DATE) >30 
							GROUP BY A.PURCHASE_NO,C.CRT_STAFF,C.CRT_NAME,C.CRT_DATE 
							) A";
            var param = new object[] { dcCode, receDate.Date, DateTime.Now };
            return SqlQuery<DcWmsNoStatusItem>(sql, param);

        }

        /// <summary>
        /// 取得驗收單查詢的驗收明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="rtNo"></param>
        /// <returns></returns>
        public IQueryable<F020201WithF02020101> GetF020201WithF02020101s(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
        {
            var sql = @"SELECT Q.* FROM 
										(SELECT	
										A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.PURCHASE_NO,
										A.RT_NO,
										A.ITEM_CODE ,
										B.EAN_CODE1,
										B.EAN_CODE2 ,
										B.ITEM_NAME ,
										B.ITEM_SPEC ,
										B.ITEM_COLOR ,
										A.VALI_DATE ,
										C.ALLOCATION_NO ,
										(SELECT WAREHOUSE_NAME FROM F1980 
										WHERE F1980.DC_CODE =A.DC_CODE 
										AND F1980.WAREHOUSE_ID = D.TAR_WAREHOUSE_ID) TAR_WAREHOUSE_ID,
										SUM(ISNULL(C.REC_QTY,0)) AS QTY,
										null AS UCC_CODE,
										null AS CAUSE
										FROM F020201 A
										JOIN F1903 B
										ON A.GUP_CODE =B.GUP_CODE 
										AND A.CUST_CODE =B.CUST_CODE 
										AND A.ITEM_CODE =B.ITEM_CODE 
										JOIN F02020108 C
										ON A.DC_CODE =C.DC_CODE
										AND A.GUP_CODE =C.GUP_CODE
										AND A.CUST_CODE =C.CUST_CODE 
										AND A.RT_NO = C.RT_NO
										AND A.RT_SEQ  = C.RT_SEQ
										JOIN F151001 D 
										ON C.DC_CODE =D.DC_CODE 
										AND C.GUP_CODE =D.GUP_CODE 
										AND C.CUST_CODE =D.CUST_CODE 
										AND C.ALLOCATION_NO =D.ALLOCATION_NO 
										AND (SUBSTRING(D.TAR_WAREHOUSE_ID,1,1) <> 'R' OR D.TAR_WAREHOUSE_ID IS NULL)
										GROUP BY 
									    A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.PURCHASE_NO,
										A.RT_NO,
										A.ITEM_CODE ,
										B.EAN_CODE1,
										B.EAN_CODE2 ,
										B.ITEM_NAME ,
										B.ITEM_SPEC ,
										B.ITEM_COLOR ,
										A.VALI_DATE ,
										C.ALLOCATION_NO ,
										D.TAR_WAREHOUSE_ID
										UNION ALL 
										SELECT DISTINCT 
										A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.PURCHASE_NO,
										A.RT_NO,
										A.ITEM_CODE ,
										B.EAN_CODE1,
										B.EAN_CODE2 ,
										B.ITEM_NAME ,
										B.ITEM_SPEC ,
										B.ITEM_COLOR ,
										A.VALI_DATE ,
										C.ALLOCATION_NO ,
										(SELECT WAREHOUSE_NAME FROM F1980 
										WHERE F1980.DC_CODE =A.DC_CODE 
										AND F1980.WAREHOUSE_ID = D.TAR_WAREHOUSE_ID) TAR_WAREHOUSE_ID ,
										SUM(ISNULL(E.DEFECT_QTY,0)) QTY,
										(SELECT CAUSE FROM F1951 WHERE UCT_ID = 'IC' AND F1951.UCC_CODE  =E.UCC_CODE) UCC_CODE,
										E.CAUSE
										FROM F020201 A
										JOIN F1903 B
										ON A.GUP_CODE =B.GUP_CODE 
										AND A.CUST_CODE =B.CUST_CODE 
										AND A.ITEM_CODE =B.ITEM_CODE 
										JOIN F02020108 C
										ON A.DC_CODE =C.DC_CODE
										AND A.GUP_CODE =C.GUP_CODE
										AND A.CUST_CODE =C.CUST_CODE 
										AND A.RT_NO = C.RT_NO
										AND A.RT_SEQ  = C.RT_SEQ
										JOIN F151001 D 
										ON C.DC_CODE =D.DC_CODE 
										AND C.GUP_CODE =D.GUP_CODE 
										AND C.CUST_CODE =D.CUST_CODE 
										AND C.ALLOCATION_NO =D.ALLOCATION_NO 
										JOIN F02020109 E
										ON A.DC_CODE =E.DC_CODE
										AND A.GUP_CODE =E.GUP_CODE
										AND A.CUST_CODE =E.CUST_CODE 
										AND A.PURCHASE_NO = E.STOCK_NO
										AND A.PURCHASE_SEQ  = E.STOCK_SEQ
										AND A.RT_NO = E.RT_NO
										AND A.RT_SEQ  = E.RT_SEQ
										AND D.TAR_WAREHOUSE_ID = E.WAREHOUSE_ID 
										GROUP BY 
										A.DC_CODE,
										A.GUP_CODE,
										A.CUST_CODE,
										A.PURCHASE_NO,
										A.RT_NO,
										A.ITEM_CODE ,
										B.EAN_CODE1,
										B.EAN_CODE2 ,
										B.ITEM_NAME ,
										B.ITEM_SPEC ,
										B.ITEM_COLOR ,
										A.VALI_DATE ,
										C.ALLOCATION_NO ,
										D.TAR_WAREHOUSE_ID,
										E.UCC_CODE,
										E.CAUSE ) Q
										
										WHERE Q.DC_CODE = @p0
									   AND Q.GUP_CODE = @p1
									   AND Q.CUST_CODE = @p2
									   AND Q.PURCHASE_NO = @p3
									   AND Q.RT_NO = @p4




";
            //        var sql = @"
            //SELECT D.*, F.ITEM_NAME, F.ITEM_SIZE, F.ITEM_SPEC, F.ITEM_COLOR, H.VNR_CODE, I.VNR_NAME, F.BUNDLE_SERIALNO,H.CUST_ORD_NO
            //		FROM (  SELECT C.RECE_DATE,
            //					   C.PURCHASE_NO,
            //					   C.PURCHASE_SEQ,
            //					   C.RT_NO,
            //					   C.ITEM_CODE,
            //					   MIN (C.ORDER_QTY) AS ORDER_QTY,
            //					   MIN (C.RECV_QTY) AS RECV_QTY,
            //                                   MIN (C.CHECK_QTY) AS CHECK_QTY,
            //					   C.DC_CODE,
            //					   C.GUP_CODE,
            //					   C.CUST_CODE,
            //					   MIN (C.STATUS) AS STATUS,
            //                                   C.ACE_DATE2,
            //                                   C.MAKE_NO,
            //                                   C.QUICK_CHECK
            //				  FROM (SELECT A.RECE_DATE,
            //							   A.PURCHASE_NO,
            //							   A.PURCHASE_SEQ,
            //							   A.RT_NO,
            //							   A.ITEM_CODE,
            //							   A.ORDER_QTY,
            //							   A.RECV_QTY,
            //                                           A.CHECK_QTY,
            //							   A.DC_CODE,
            //							   A.GUP_CODE,
            //							   A.CUST_CODE,
            //							   A.STATUS,
            //                                        case when A.VALI_DATE = null then ''                                              
            //                                              else convert (varchar,A.VALI_DATE,111) 
            //                                         end ACE_DATE2,
            //                                           A.MAKE_NO,
            //                                           A.QUICK_CHECK
            //						  FROM F020201 A
            //						UNION ALL
            //						SELECT B.RECE_DATE,
            //							   B.PURCHASE_NO,
            //							   B.PURCHASE_SEQ,
            //							   B.RT_NO,
            //							   B.ITEM_CODE,
            //							   B.ORDER_QTY,
            //							   B.RECV_QTY,
            //                                           B.CHECK_QTY,
            //							   B.DC_CODE,
            //							   B.GUP_CODE,
            //							   B.CUST_CODE,
            //							   B.STATUS,
            //                                          case when B.VALI_DATE = null then ''                                              
            //                                              else convert (varchar,B.VALI_DATE, 111) 
            //                                         end ACE_DATE2,
            //                                           B.MAKE_NO,
            //                                           B.QUICK_CHECK
            //						  FROM F02020101 B) C
            //				 WHERE     C.DC_CODE = @p0
            //					   AND C.GUP_CODE = @p1
            //					   AND C.CUST_CODE = @p2
            //					   AND C.PURCHASE_NO = @p3
            //					   AND C.RT_NO = @p4
            //			  GROUP BY C.RECE_DATE,
            //					   C.PURCHASE_NO,
            //					   C.PURCHASE_SEQ,
            //					   C.RT_NO,
            //					   C.ITEM_CODE,
            //					   C.DC_CODE,
            //					   C.GUP_CODE,
            //					   C.CUST_CODE,
            //                                    C.ACE_DATE2,
            //                                    C.MAKE_NO,
            //                                    C.QUICK_CHECK) D
            //			 JOIN F1903 F
            //				ON     D.ITEM_CODE = F.ITEM_CODE
            //				   AND D.GUP_CODE = F.GUP_CODE
            //				   AND D.CUST_CODE = F.CUST_CODE

            //            JOIN F010201 H ON H.DC_CODE = D.DC_CODE AND H.GUP_CODE = D.GUP_CODE AND H.CUST_CODE = D.CUST_CODE AND H.STOCK_NO = D.PURCHASE_NO
            //                        JOIN F1908 I ON H.GUP_CODE = I.GUP_CODE AND H.CUST_CODE = I.CUST_CODE AND H.VNR_CODE = I.VNR_CODE
            //	ORDER BY D.RECE_DATE,
            //			 D.PURCHASE_NO,
            //			 D.RT_NO,
            //			 D.STATUS	 
            //            ";
            return SqlQuery<F020201WithF02020101>(sql, new object[] { dcCode, gupCode, custCode, purchaseNo, rtNo });
        }

        /// <summary>
        /// 取得調撥單的驗收單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="allocationNo"></param>
        /// <returns></returns>
        public IQueryable<F020201> GetAllocationData(string dcCode, string gupCode, string custCode, string allocationNo)
        {
            var parameter = new List<object> { dcCode, gupCode, custCode, allocationNo };
            var sql = @"
						SELECT B.* 
						  FROM F02020107 A,F020201 B 
						 WHERE A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.RT_NO = B.RT_NO 
						   AND A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND A.ALLOCATION_NO = @p3";
            return SqlQuery<F020201>(sql, parameter.ToArray());
        }

        public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", settleDate)
            };
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY DC_CODE,GUP_CODE,CUST_CODE,WMS_NO,ITEM_CODE)AS ROWNUM,TB.* FROM (
							SELECT @p3 CAL_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,PURCHASE_NO WMS_NO,
										 A.ITEM_CODE,SUM (A.RECV_QTY) QTY,'01' DELV_ACC_TYPE
								FROM F020201 A
							 WHERE (A.DC_CODE = @p0 OR @p0 = '000') AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
								 AND A.RECE_DATE = @p3 AND A.STATUS = '2'
						GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,PURCHASE_NO, A.ITEM_CODE) TB
					  ORDER BY WMS_NO,ITEM_CODE ";
            return SqlQuery<SettleData>(sql, parameter.ToArray());
        }

		public IQueryable<P020206Data> FindEx(string dcCode, string gupCode, string custCode, string purchaseNo, string commCustCode
			, string rtNo = "", string vnrCode = "", string custOrdNo = "", string containerCode = "", string vnrNameConditon = "", string startDt = "", string endDt = "")
		{

			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", Current.Lang),
				new SqlParameter("@p1", dcCode),
				new SqlParameter("@p2", gupCode),
				new SqlParameter("@p3", custCode)
			};

			string sqlRtNo = "";
			if (!string.IsNullOrWhiteSpace(rtNo))
			{
				sqlRtNo = " AND A.RT_NO = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), rtNo));
			}
			string sqlVnrCode = "";
			if (!string.IsNullOrWhiteSpace(vnrCode))
			{
				sqlVnrCode = " AND A.VNR_CODE = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), vnrCode));
			}
			string sqlCustOrdNo = "";
			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sqlCustOrdNo = " AND B.CUST_ORD_NO = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), custOrdNo));
			}
			string sqlContainerCode = "";
			if (!string.IsNullOrWhiteSpace(containerCode))
			{
				sqlContainerCode = $@" AND EXISTS (SELECT 1 from F020502 WHERE DC_CODE = A.DC_CODE 
								AND GUP_CODE = A.GUP_CODE AND  CUST_CODE = A.CUST_CODE AND STOCK_NO  = A.PURCHASE_NO 
                AND A.RT_NO = RT_NO AND  CONTAINER_CODE = @p{ param.Count()})";
				param.Add(new SqlParameter("@p" + param.Count(), containerCode));
			}
			string sqlVnrNameConditon = "";
			if (!string.IsNullOrWhiteSpace(vnrNameConditon))
			{
				sqlVnrNameConditon = " AND C.VNR_NAME LIKE '%' + @p" + param.Count() + " + '%' ";
				param.Add(new SqlParameter("@p" + param.Count(), vnrNameConditon));
			}
			string sqlStartDt = "";
			if (!string.IsNullOrWhiteSpace(startDt))
			{
				sqlStartDt = " AND A.RECE_DATE >= CONVERT(VARCHAR,@p" + param.Count() + ",111)";
				param.Add(new SqlParameter("@p" + param.Count(), startDt));
			}
			string sqlEndDt = "";
			if (!string.IsNullOrWhiteSpace(endDt))
			{
				//endDt = endDt.Value.AddDays(1);
				sqlEndDt = " AND A.RECE_DATE < CONVERT(VARCHAR,@p" + param.Count() + ",111)";
				param.Add(new SqlParameter("@p" + param.Count(), endDt));
			}
			string sqlPurchaseNo = "";
			if (!string.IsNullOrWhiteSpace(purchaseNo))
			{
				sqlPurchaseNo = " AND A.PURCHASE_NO = @p" + param.Count();
				param.Add(new SqlParameter("@p" + param.Count(), purchaseNo));
			}

			string sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A.RECE_DATE, A.RT_NO)  AS ROW_NUM,
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.CRT_DATE AS UPD_DATE,
								A.CRT_NAME AS UPD_NAME,
								A.FAST_PASS_TYPE,
								A.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								A.VNR_NAME ,
								SUM(A.RECV_QTY) RECV_QTY,
								SUM(A.RECV_QTY-ISNULL(A.DEFECT_QTY,0)) SUM_RECV_QTY,
								SUM(ISNULL(A.DEFECT_QTY,0)) DEFECT_QTY,
								SUM(A.ITEM_COUNT) ITEM_COUNT FROM 
								( SELECT
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.CRT_DATE,
								A.CRT_NAME,
								(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F010201' AND SUBTOPIC = 'FAST_PASS_TYPE' AND VALUE = B.FAST_PASS_TYPE AND LANG = @p0) FAST_PASS_TYPE,
								B.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								C.VNR_NAME ,
								SUM(A.RECV_QTY) RECV_QTY,
								SUM(A.RECV_QTY-ISNULL(E.DEFECT_QTY,0)) SUM_RECV_QTY,
								SUM(ISNULL(E.DEFECT_QTY,0)) DEFECT_QTY,
								COUNT(A.RT_NO) ITEM_COUNT
								FROM F020201 A
								JOIN F010201 B
								ON A.DC_CODE =B.DC_CODE 
								AND A.GUP_CODE =B.GUP_CODE 
								AND A.CUST_CODE =B.CUST_CODE 
								AND A.PURCHASE_NO = B.STOCK_NO 
								JOIN F1908 C 
								ON A.GUP_CODE =C.GUP_CODE 
								AND A.CUST_CODE  = C.CUST_CODE 
								AND A.VNR_CODE  = C.VNR_CODE 
								LEFT JOIN (SELECT D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.RT_NO,D.RT_SEQ,SUM(D.DEFECT_QTY) DEFECT_QTY  FROM F02020109 D
								JOIN F020201 F
										ON D.DC_CODE = F.DC_CODE
										AND D.GUP_CODE = F.GUP_CODE
										AND D.CUST_CODE = F.CUST_CODE
										AND D.RT_NO = F.RT_NO
										AND D.RT_SEQ = F.RT_SEQ
										GROUP BY D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.RT_NO,D.RT_SEQ) E 
								ON A.DC_CODE = E.DC_CODE 
								AND A.GUP_CODE = E.GUP_CODE 
								AND A.CUST_CODE =E.CUST_CODE 
								AND A.RT_NO = E.RT_NO
								AND A.RT_SEQ  = E.RT_SEQ 
								 WHERE A.DC_CODE = @p1
								AND A.GUP_CODE = @p2
								AND A.CUST_CODE = @p3
								{0} {1} {2} {3} {4} {5} {6} {7}
								AND A.RT_MODE = '1'
								AND C.VNR_CODE IS NOT NULL
								GROUP BY 
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.CRT_DATE,
								A.CRT_NAME,
								B.FAST_PASS_TYPE,
								B.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								C.VNR_NAME
								) A
								GROUP BY 
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.CRT_DATE,
								A.CRT_NAME,
								A.FAST_PASS_TYPE,
								A.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								A.VNR_NAME
								ORDER BY 1 ASC";
			sql = string.Format(sql, sqlRtNo, sqlVnrCode, sqlStartDt, sqlEndDt, sqlPurchaseNo, sqlCustOrdNo, sqlContainerCode, sqlVnrNameConditon);

			var result = SqlQuery<P020206Data>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<AcceptanceDetail> GetAcceptanceDetail(string dcCode, string gupCode, string custCode, string rtNo)
		{
			var parameter = new List<object> { Current.Lang,dcCode, gupCode, custCode, rtNo };
			var sql = @"SELECT A.RT_NO ,A.ITEM_CODE ,C.EAN_CODE1 ,C.EAN_CODE2 ,C.ITEM_CODE, C.ITEM_NAME,C.ITEM_SPEC ,C.ITEM_COLOR ,A.VALI_DATE ,
						(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F0205' AND SUBTOPIC = 'TYPE_CODE' AND VALUE = B.TYPE_CODE  AND LANG = @p0 ) TYPE_CODE_NAME,
						SUM(B.B_QTY) B_QTY,
						(SELECT WAREHOUSE_NAME FROM F1980 WHERE DC_CODE=B.DC_CODE AND WAREHOUSE_ID = B.PICK_WARE_ID) PICK_WARE_NAME,
						CASE B.NEED_DOUBLE_CHECK  WHEN '1' THEN '是' ELSE '否' END NEED_DOUBLE_CHECK FROM F020201 A
						JOIN F0205 B 
						ON A.DC_CODE =B.DC_CODE 
						AND A.GUP_CODE = B.GUP_CODE 
						AND A.CUST_CODE = B.CUST_CODE 
						AND A.RT_NO = B.RT_NO 
						AND A.RT_SEQ = B.RT_SEQ 
						JOIN F1903 C 
						ON A.GUP_CODE = C.GUP_CODE
						AND A.CUST_CODE = C.CUST_CODE 
						AND A.ITEM_CODE = C.ITEM_CODE 
						AND A.ITEM_CODE = C.ITEM_CODE 
						WHERE A.DC_CODE  = @p1
						AND A.GUP_CODE = @p2
						AND A.CUST_CODE =@p3
						AND A.RT_NO = @p4
						GROUP BY A.RT_NO ,A.ITEM_CODE ,C.EAN_CODE1 ,C.EAN_CODE2 ,C.ITEM_CODE,C.ITEM_NAME ,C.ITEM_SPEC ,C.ITEM_COLOR ,A.VALI_DATE,
						B.TYPE_CODE ,B.DC_CODE ,B.PICK_WARE_ID,B.NEED_DOUBLE_CHECK ";
			var result = SqlQuery<AcceptanceDetail>(sql, parameter.ToArray()).AsQueryable();
			return result;
		}

        public F020201 GetLastOrder(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0", System.Data.SqlDbType.VarChar) { Value = dcCode },
                new SqlParameter("@p1", System.Data.SqlDbType.VarChar) { Value = gupCode },
                new SqlParameter("@p2", System.Data.SqlDbType.VarChar) { Value = custCode },
                new SqlParameter("@p3", System.Data.SqlDbType.VarChar) { Value = purchaseNo },
            };

            var sql = @"
                SELECT TOP 1 * FROM F020201
                WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND PURCHASE_NO=@p3 AND STATUS='3' 
                ORDER BY RT_NO";
            return SqlQuery<F020201>(sql, para.ToArray()).SingleOrDefault();
        }

        public int GetTodayRecvQty(string dcCode, string gupCode, string custCode, string purchaseNo, DateTime receDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", purchaseNo),
                new SqlParameter("@p4", receDate)
            };
            var sql = @"SELECT TOP (1) SUM(RECV_QTY) TodayRecvQty FROM F020201
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
						AND PURCHASE_NO = @p3
						AND RECE_DATE = @p4 
            GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO";
            return SqlQuery<int>(sql, parameter.ToArray()).FirstOrDefault();
        }
		     
		    public IQueryable<F020201> GetDatasByF020501_ID(long f020501Id)
				{
					var parameter = new List<SqlParameter>
					{
							new SqlParameter("@p0", f020501Id){ SqlDbType = System.Data.SqlDbType.BigInt }
					};
					var sql = @"  SELECT DISTINCT B.*
														FROM F020502 A
														JOIN F020201 B
														  ON B.DC_CODE = A.DC_CODE
													 	 AND B.GUP_CODE = A.GUP_CODE
													 	 AND B.CUST_CODE = A.CUST_CODE
														 AND B.RT_NO = A.RT_NO
														 AND B.RT_SEQ = A.RT_SEQ
													 WHERE A.F020501_ID = @p0 ";
						return SqlQuery<F020201>(sql, parameter.ToArray());
				}

    public IQueryable<CloseBoxDetail> GetCloseBoxDetail(long f020501Id)
    {
      var parameter = new List<SqlParameter>
          {
              new SqlParameter("@p0", f020501Id){ SqlDbType = SqlDbType.BigInt }
          };
      var sql = @"SELECT 
	A.DC_CODE,
	A.GUP_CODE,
	A.CUST_CODE,
	A.CUST_CODE, 
	A.ITEM_CODE,
	B.VALI_DATE,
	B.MAKE_NO,
	A.BIN_CODE,
	SUM(A.QTY) QTY
FROM 
  F020502 A
  INNER JOIN F020201 B
  	ON A.DC_CODE = B.DC_CODE 
	  AND A.GUP_CODE = B.GUP_CODE 
	  AND A.CUST_CODE = B.CUST_CODE 
	  AND A.STOCK_NO = B.PURCHASE_NO 
	  AND A.STOCK_SEQ = B.PURCHASE_SEQ 
	  AND A.RT_NO = B.RT_NO 
	  AND A.RT_SEQ = B.RT_SEQ 
WHERE 
	A.F020501_ID = @p0
GROUP BY 
	A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.CUST_CODE, 
	A.ITEM_CODE,B.VALI_DATE,B.MAKE_NO,A.BIN_CODE";
      return SqlQuery<CloseBoxDetail>(sql, parameter.ToArray());
    }

    public IQueryable<F020201> GetDatasByRtNoList(string dcCode, string gupCode, string custCode, List<string> rtNoList)
    {
      var parameter = new List<SqlParameter>
            {
              new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
            };
      var sql = @"SELECT *
FROM F020201
WHERE DC_CODE = @p0
  AND GUP_CODE = @p1
  AND CUST_CODE = @p2 ";
      sql += parameter.CombineSqlInParameters(" AND RT_NO", rtNoList, SqlDbType.VarChar);
      return SqlQuery<F020201>(sql, parameter.ToArray());
    }

    public IQueryable<F020201> GetDatasByF020502(string dcCode, string gupCode, string custCode, List<F020502> f020502s)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
SELECT 
  * 
FROM 
  F020201 
WHERE 
  DC_CODE=@p0
  AND GUP_CODE=@p1
  AND CUST_CODE=@p2
";

      //組出 AND ((RT_NO=@p1 AND RT_SEQ=@p2) OR (RT_NO=@p3 AND RT_SEQ=@p4))
      if (f020502s.Any())
      {
        sql += "AND (";
        var rtNoList = new List<string>();
        foreach (var f020502 in f020502s)
        {
          //組出(RT_NO=@p1 AND RT_SEQ=@p2)
          var tmpSql = $"(RT_NO = @p{para.Count}";
          para.Add(new SqlParameter($"@p{para.Count}", f020502.RT_NO) { SqlDbType = SqlDbType.VarChar });

          tmpSql += $" AND RT_SEQ =  @p{para.Count})";
          para.Add(new SqlParameter($"@p{para.Count}", f020502.RT_SEQ) { SqlDbType = SqlDbType.VarChar });

          rtNoList.Add(tmpSql);
        }
        sql += string.Join(" OR ", rtNoList);
        sql += ")";
      }
      else
        sql += " AND 1=0";

      return SqlQuery<F020201>(sql, para.ToArray());
      //return _db.F020201s.AsNoTracking().Where(x =>
      //x.DC_CODE == dcCode &&
      //x.GUP_CODE == gupCode &&
      //x.CUST_CODE == custCode &&
      //f020502s.Any(z => z.RT_NO == x.RT_NO &&
      //z.RT_SEQ == x.RT_SEQ));
    }

    public IQueryable<RecvNeedBindContainerQueryRes> GetRecvNeedBindContainerQuery(string dcCode, string gupCode, string custCode, string wmsNo, string itemCode)
    {
      var parameter = new List<SqlParameter>
            {
              new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
            };

      var whereSql = "";
      if (!string.IsNullOrWhiteSpace(wmsNo) && string.IsNullOrWhiteSpace(itemCode))
      {
        parameter.Add(new SqlParameter("@p3", wmsNo) { SqlDbType = SqlDbType.VarChar });

        whereSql = @"  
  AND B.STATUS IN ('1','3')
  AND (A.STOCK_NO = @p3 OR A.CUST_ORD_NO = @p3 OR B.RT_NO = @p3)";
      }
      else if (string.IsNullOrWhiteSpace(wmsNo) && !string.IsNullOrWhiteSpace(itemCode))
      {
        parameter.Add(new SqlParameter("@p3", itemCode) { SqlDbType = SqlDbType.VarChar });

        whereSql = @"  
  AND B.STATUS IN ('3')
  AND (B.ITEM_CODE = @p3 OR C.EAN_CODE1 = @p3 OR C.EAN_CODE2 = @p3 OR C.EAN_CODE3 = @p3 OR C.CUST_ITEM_CODE = @p3)";
      }
      else if (!string.IsNullOrWhiteSpace(wmsNo) && !string.IsNullOrWhiteSpace(itemCode))
      {
        parameter.Add(new SqlParameter("@p3", wmsNo) { SqlDbType = SqlDbType.VarChar });
        parameter.Add(new SqlParameter("@p4", itemCode) { SqlDbType = SqlDbType.VarChar });

        whereSql = @"  
  AND B.STATUS IN ('3')
  AND (A.STOCK_NO = @p3 OR A.CUST_ORD_NO = @p3 OR B.RT_NO=@p3)
  AND (B.ITEM_CODE = @p4 OR C.EAN_CODE1 = @p4 OR C.EAN_CODE2 = @p4 OR C.EAN_CODE3 = @p4 OR C.CUST_ITEM_CODE = @p4)";
      }
      else
        whereSql = "1=0";

      var sql = $@"SELECT 
  B.DC_CODE DcNo,
  B.CUST_CODE CustNo,
	B.PURCHASE_NO StockNo,
	B.PURCHASE_SEQ StockSeq,
	A.CUST_ORD_NO CustOrdNo,
	B.RT_NO RtNo,
	B.RT_SEQ RtSeq,
	B.STATUS Status,
	BLngSTATUS.NAME StatusDesc,
	B.VNR_CODE VnrCode,
	D.VNR_NAME VnrName,
	A.FAST_PASS_TYPE FastPassType,
	ALngFastPassType.NAME FastPassTypeDesc,
	B.ITEM_CODE ItemCode,
	C.CUST_ITEM_CODE CustItemCode,
  C.VNR_ITEM_CODE VnrItemCode,
	C.ITEM_NAME ItemName,
	B.RECV_QTY RecvQty
FROM 
	F010201 A
	LEFT JOIN VW_F000904_LANG ALngFastPassType
    ON ALngFastPassType.TOPIC='F010201' 
    AND ALngFastPassType.SUBTOPIC='FAST_PASS_TYPE' 
    AND ALngFastPassType.LANG='{Current.Lang}' 
    AND ALngFastPassType.VALUE=A.FAST_PASS_TYPE
	INNER JOIN F020201 B
		ON A.DC_CODE = B.DC_CODE 
		AND A.GUP_CODE = B.GUP_CODE 
		AND A.CUST_CODE =B.CUST_CODE 
		AND A.STOCK_NO = B.PURCHASE_NO 
    AND B.STATUS IN('1','3')
	LEFT JOIN VW_F000904_LANG BLngSTATUS
    ON BLngSTATUS.TOPIC='F020201' 
    AND BLngSTATUS.SUBTOPIC='STATUS' 
    AND BLngSTATUS.LANG='{Current.Lang}' 
    AND BLngSTATUS.VALUE=B.STATUS
	INNER JOIN F1903 C WITH(NOLOCK)
		ON B.GUP_CODE = C.GUP_CODE 
		AND B.CUST_CODE = C.CUST_CODE 
		AND B.ITEM_CODE = C.ITEM_CODE 
	INNER JOIN F1908 D WITH(NOLOCK)
		ON B.GUP_CODE = D.GUP_CODE 
		AND B.CUST_CODE = D.CUST_CODE 
		AND B.VNR_CODE = D.VNR_CODE
WHERE 
  B.DC_CODE=@p0
  AND B.GUP_CODE=@p1
  AND B.CUST_CODE=@p2
  {whereSql}";
      return SqlQuery<RecvNeedBindContainerQueryRes>(sql, parameter.ToArray());
    }

    /// <summary>
    /// 檢查該筆驗收單是否已綁定完成
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo"></param>
    /// <param name="rtSeq"></param>
    /// <returns></returns>
    public Boolean IsAllOrdBindComplete(string dcCode, string gupCode, string custCode, string rtNo, string rtSeq)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", rtNo)     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", rtSeq)    { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"SELECT TOP 1 1 FROM F020201 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND RT_NO=@p3 AND RT_SEQ=@p4 AND STATUS='2'";

      return !SqlQuery<int>(sql, para.ToArray()).Any();
    }

    /// <summary>
    /// 檢查驗收單是否都綁定完成
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo"></param>
    /// <param name="rtSeq"></param>
    /// <returns></returns>
    public Boolean IsAcceptenceIsComplete(string dcCode, string gupCode, string custCode, string rtNo, string rtSeq)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", rtNo)     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", rtSeq)    { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"SELECT TOP 1 1 FROM F020201 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND RT_NO=@p3 AND RT_SEQ!=@p4 AND STATUS='3'";

      return !SqlQuery<int>(sql, para.ToArray()).Any();
    }

    public IQueryable<RecvRecords> GetF020209RecvRecord(string dcCode, string gupCode, string custCode, DateTime RecvDateBegin, DateTime RecvDateEnd, string PurchaseNo,
      string CustOrdNo, string PrintMode, string PalletLocation, string ItemCode, string RecvStaff)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar }
      };

      var sql = @"
                SELECT
	                A.PALLET_LOCATION,
	                A.PURCHASE_NO,
	                A.CUST_ORD_NO,
	                A.RT_NO, 
	                A.RT_SEQ,
	                A.ITEM_CODE,
	                A.RECV_QTY,
	                A.VALI_DATE,
	                A.MAKE_NO,
                  A.CRT_STAFF,
	                A.CRT_NAME,
	                CASE WHEN A.IS_PRINT_ITEM_ID = '1' THEN '是' ELSE '否' END IS_PRINT_ITEM_ID,
                  A.IS_PRINT_ITEM_ID IS_PRINT_ITEM_ID_RAW,
	                CASE WHEN A.IS_PRINT_RECVNOTE = '1' THEN '是' ELSE '否' END IS_PRINT_RECVNOTE,
                  A.IS_PRINT_RECVNOTE IS_PRINT_RECVNOTE_RAW,
	                A.CRT_DATE,
	                A.PRINT_NAME,
	                A.PRINT_TIME,
					        B.NAME PRINT_MODE,
                  A.PRINT_MODE IsEnabledString
                FROM 
	                F020201 A
				        LEFT JOIN
					        VW_F000904_LANG B ON B.TOPIC='F020201' AND B.SUBTOPIC='PRINT_MODE' AND A.PRINT_MODE=B.VALUE
                WHERE
                  A.DC_CODE = @p0
                  AND A.GUP_CODE = @p1
                  AND A.CUST_CODE = @p2
                  AND A.PRINT_MODE <> '0'
                ";

      var sql2 = "";

      if (!string.IsNullOrWhiteSpace(RecvDateBegin.ToString()))
      {
        sql2 += $" AND CRT_DATE > @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", RecvDateBegin) { SqlDbType = SqlDbType.DateTime2 });
      }

      if (!string.IsNullOrWhiteSpace(RecvDateEnd.ToString()))
      {
        sql2 += $" AND CRT_DATE < @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", RecvDateEnd.AddDays(1)) { SqlDbType = SqlDbType.DateTime2 });
      }

      if (!string.IsNullOrWhiteSpace(PurchaseNo))
      {
        sql2 += $" AND PURCHASE_NO = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", PurchaseNo) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(CustOrdNo))
      {
        sql2 += $" AND CUST_ORD_NO = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", CustOrdNo) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(PrintMode))
      {
        sql2 += $" AND PRINT_MODE = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", PrintMode) { SqlDbType = SqlDbType.Char });
      }

      if (!string.IsNullOrWhiteSpace(PalletLocation))
      {
        sql2 += $" AND PALLET_LOCATION = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", PalletLocation) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(ItemCode))
      {
        sql2 += $" AND ITEM_CODE = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", ItemCode) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(RecvStaff))
      {
        sql2 += $" AND CRT_STAFF = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", RecvStaff) { SqlDbType = SqlDbType.VarChar });
      }

      sql += sql2;

      var result = SqlQuery<RecvRecords>(sql, param.ToArray());
      return result;
    }

    public IQueryable<ItemLabelData> GetP020209ItemLabelData(string dcCode, string gupCode, string custCode, List<string> rtNos)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar }
      };

      var sql = @"
                SELECT 
                B.CUST_ITEM_CODE,
                B.ITEM_NAME,
                A.CUST_ORD_NO,
                A.RECV_QTY,
                A.VALI_DATE,
                CASE WHEN LEFT(A.CUST_ORD_NO, 2) = 'BR' THEN 'B' ELSE 'N' END ORDER_TYPE
                FROM F020201 A 
	                JOIN F1903 B 
		                ON A.GUP_CODE=B.GUP_CODE 
		                AND A.CUST_CODE=B.CUST_CODE 
		                AND A.ITEM_CODE=B.ITEM_CODE
                WHERE
	                A.DC_CODE = @p0
	                AND A.GUP_CODE = @p1
	                AND A.CUST_CODE = @p2
                  AND RT_NO + RT_SEQ IN({0})
                ";

      StringBuilder sqlIn = new StringBuilder();

      foreach (var rtNo in rtNos)
      {
        sqlIn.Append($"'{rtNo}',");
      }

      sqlIn.Remove(sqlIn.Length - 1, 1);
      sql = string.Format(sql, sqlIn.ToString());

      //加上ROW_NUM
      sql = $@"
            SELECT 
              ROW_NUMBER()OVER(ORDER BY aa.CUST_ITEM_CODE) AS ROW_NUM,
              aa.*
            FROM
              ({sql}) aa";

      var result = SqlQuery<ItemLabelData>(sql, param.ToArray());

      foreach (var item in result)
      {
        if (item.VALI_DATE.ToString("yyyy-MM-dd") != "9999-12-31")
        {
          var month = "0" + item.VALI_DATE.Month;
          item.VALI_DATE_MONTH = month.Substring(month.Length - 2);
          item.VALI_DATE_YEAR = (item.VALI_DATE.Year - 1911).ToString();
        }
      }

      return result;
    }
  }
}
