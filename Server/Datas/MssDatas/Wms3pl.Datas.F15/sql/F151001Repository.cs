using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
    public partial class F151001Repository : RepositoryBase<F151001, Wms3plDbContext, F151001Repository>
    {
        #region 調撥單維護
        /// <summary>
		/// 取得調撥單明細，這個會給調撥單維護主畫面的明細與過帳時的明細顯示，故主要要去看過帳的調撥單明細(欄位較多)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
        /// <param name="status"></param>
		/// <returns></returns>
		public IQueryable<F151001DetailDatas> GetF151001DetailDatas(string dcCode, string gupCode, string custCode, string allocationNo, string action, List<string> status = null)
        {
            string condition = string.Empty;
            if (status != null && status.Any())
                condition = $" AND C.STATUS IN ( {string.Join(",", status)} ) ";

            var tarLocSql = string.Empty;
            if (action == "01")// 查詢功能
                tarLocSql = "C.STATUS <> 2";
            else if (action == "02")// 編輯功能
                tarLocSql = "ISNULL(C.TAR_LOC_CODE,'') = ''";
            else if (action == "03")// 過帳功能
                tarLocSql = "B.TAR_WAREHOUSE_ID IS NULL";

            var parameters = new object[] { allocationNo, dcCode, gupCode, custCode };
            var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO) ROWNUM,
							   A.*,
							   D.ITEM_NAME,
							   D.ITEM_SIZE,
							   D.ITEM_SPEC,
							   D.ITEM_COLOR,
							   D.BUNDLE_SERIALLOC,											-- 序號綁儲位
							   D.CUST_ITEM_CODE ,
							   D.EAN_CODE1,
                               F.SOURCE_NAME ITEM_SOURCE_TYPE_NAME
						  FROM (  SELECT B.SRC_DC_CODE,
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,                -- 調撥單明細顯示以畫面上顯示的欄位做 Group By
                                         B.ALLOCATION_DATE,
										 B.STATUS,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,                                            -- 品號
										 C.BOX_CTRL_NO,
										 C.PALLET_CTRL_NO,
                                         CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN '' ELSE C.SRC_LOC_CODE END AS SRC_LOC_CODE,    -- 來源儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.SUG_LOC_CODE END AS SUG_LOC_CODE,   -- 建議上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.TAR_LOC_CODE END AS TAR_LOC_CODE,   -- 實際上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.TAR_QTY) END AS TAR_QTY,         -- 上架數量
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_TAR_QTY) END AS A_TAR_QTY,  -- 實際上架數量
										 CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.SRC_QTY) END AS SRC_QTY,       -- 下架數量
                                         CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_SRC_QTY) END AS A_SRC_QTY,    --實際下架數量
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_SRC_LOC_CODE,  -- 顯示來源儲位
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_SRC_QTY,       -- 顯示下架數量
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_A_SRC_QTY,     -- 顯示實際下架數量
                                         CASE WHEN {tarLocSql} THEN '0' ELSE '1' END AS SHOW_TAR_LOC_CODE,                          -- 顯示實際上架儲位
										 C.SRC_STAFF,                                          -- 下架人員
										 C.SRC_NAME,                                           -- 下架人名
										 C.TAR_STAFF,                                          -- 上架人員
										 C.TAR_NAME,                                           -- 上架人名
										 MAX (C.SRC_DATE) AS SRC_DATE,                    -- 取最後一次下架時間
										 MAX (C.TAR_DATE) AS TAR_DATE,                    -- 取最後一次上架時間
										 CASE WHEN COUNT(E.ALLOCATION_NO) = SUM (C.TAR_QTY) THEN '1' ELSE '0' END AS CHECK_SERIALNO,             -- 序號已刷讀
                                         B.SOURCE_NO,C.VALID_DATE,C.ENTER_DATE,C.MAKE_NO, STRING_AGG(C.ALLOCATION_SEQ, ',') ALLOCATION_SEQ_LIST,
                                         C.BIN_CODE,
										 C.SOURCE_NO ITEM_SOURCE_NO,
										 C.SOURCE_TYPE ITEM_SOURCE_TYPE,
										 C.REFENCE_NO,
										 C.REFENCE_SEQ
									FROM F151001 B
										 JOIN F151002 C
											ON     B.ALLOCATION_NO = C.ALLOCATION_NO
											   AND B.DC_CODE = C.DC_CODE
											   AND B.GUP_CODE = C.GUP_CODE
											   AND B.CUST_CODE = C.CUST_CODE
                         AND (C.SRC_QTY > 0 OR C.TAR_QTY > 0) 
                         {condition}
                     LEFT JOIN F15100101 E  
							        ON     E.ALLOCATION_NO = C.ALLOCATION_NO
							           AND E.ALLOCATION_SEQ = C.ALLOCATION_SEQ
							           AND E.DC_CODE = C.DC_CODE
							           AND E.GUP_CODE = C.GUP_CODE
							           AND E.CUST_CODE = C.CUST_CODE
							           AND E.STATUS = '0'
								GROUP BY B.SRC_DC_CODE,
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,
										 B.ALLOCATION_DATE,
										 B.STATUS,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,
										 C.SRC_LOC_CODE,
										 C.SUG_LOC_CODE,
										 C.TAR_LOC_CODE,
										 C.SRC_STAFF,
										 C.SRC_NAME,
										 C.TAR_STAFF,
										 C.BOX_CTRL_NO,
										 C.PALLET_CTRL_NO,
										 C.TAR_NAME,B.SOURCE_NO,C.VALID_DATE,C.ENTER_DATE,C.MAKE_NO,
                                         C.BIN_CODE,
										 C.SOURCE_NO,
										 C.SOURCE_TYPE,
										 C.REFENCE_NO,
										 C.REFENCE_SEQ,
										 C.STATUS) A
							   LEFT JOIN F1903 D
								  ON     A.GUP_CODE = D.GUP_CODE
									 AND A.CUST_CODE = D.CUST_CODE
									 AND A.ITEM_CODE = D.ITEM_CODE
                               LEFT JOIN F000902 F
								 ON A.ITEM_SOURCE_TYPE = F.SOURCE_TYPE
						 WHERE     A.ALLOCATION_NO = @p0
							   AND A.DC_CODE = @p1
							   AND A.GUP_CODE = @p2
							   AND A.CUST_CODE = @p3";
                 
            

            var result = SqlQuery<F151001DetailDatas>(sql, parameters).ToList();
            return result.AsQueryable();
        }

        /// <summary>
		/// 取得調撥單明細，這個會給調撥單維護主畫面的明細與過帳時的明細顯示，故主要要去看過帳的調撥單明細(欄位較多)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <returns></returns>
		public IQueryable<F151001DetailDatas> GetF151001DetailDatasByIsExpendDate(string dcCode, string gupCode, string custCode, string allocationNo, string action)
        {
            var tarLocSql = string.Empty;
            if (action == "01")// 查詢功能
                tarLocSql = "C.STATUS <> 2";
            else if(action == "02")// 編輯功能
                tarLocSql = "ISNULL(C.TAR_LOC_CODE,'') = ''";
            else if (action == "03")// 過帳功能
                tarLocSql = "B.TAR_WAREHOUSE_ID IS NULL";


            var parameters = new object[] { allocationNo, dcCode, gupCode, custCode };
            var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO) ROWNUM,
							   A.*,
							   E.ITEM_NAME,
							   E.ITEM_SIZE,
							   E.ITEM_SPEC,
							   E.ITEM_COLOR,
							   E.BUNDLE_SERIALLOC,											-- 序號綁儲位
							   E.CUST_ITEM_CODE ,
							   E.EAN_CODE1,
                               F.SOURCE_NAME ITEM_SOURCE_TYPE_NAME
						  FROM (  SELECT B.SRC_DC_CODE,
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,                -- 調撥單明細顯示以畫面上顯示的欄位做 Group By
                                         B.ALLOCATION_DATE,
										 B.STATUS,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,                                            -- 品號
										 C.BOX_CTRL_NO,
										 C.PALLET_CTRL_NO,
                                         CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN '' ELSE C.SRC_LOC_CODE END AS SRC_LOC_CODE,    -- 來源儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.SUG_LOC_CODE END AS SUG_LOC_CODE,   -- 建議上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.TAR_LOC_CODE END AS TAR_LOC_CODE,   -- 實際上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.TAR_QTY) END AS TAR_QTY,         -- 上架數量
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_TAR_QTY) END AS A_TAR_QTY,  -- 實際上架數量
										 CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.SRC_QTY) END AS SRC_QTY,       -- 下架數量
                                         CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_SRC_QTY) END AS A_SRC_QTY,    --實際下架數量
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_SRC_LOC_CODE,  -- 顯示來源儲位
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_SRC_QTY,       -- 顯示下架數量
                                         CASE WHEN ISNULL(B.SRC_WAREHOUSE_ID, '') = '' THEN '0' ELSE '1' END AS SHOW_A_SRC_QTY,     -- 顯示實際下架數量
                                         CASE WHEN {tarLocSql} THEN '0' ELSE '1' END AS SHOW_TAR_LOC_CODE,                          -- 顯示實際上架儲位
										 C.SRC_STAFF,                                          -- 下架人員
										 C.SRC_NAME,                                           -- 下架人名
										 C.TAR_STAFF,                                          -- 上架人員
										 C.TAR_NAME,                                           -- 上架人名
										 MAX (C.SRC_DATE) AS SRC_DATE,                    -- 取最後一次下架時間
										 MAX (C.TAR_DATE) AS TAR_DATE,                    -- 取最後一次上架時間
										 MIN (C.CHECK_SERIALNO) AS CHECK_SERIALNO,            -- 序號已刷讀
                                         B.SOURCE_NO,
                                         C.VALID_DATE,
                                         C.SRC_VALID_DATE,
                                         C.TAR_VALID_DATE,
                                         C.ENTER_DATE,
                                         C.MAKE_NO,
                                         C.SRC_MAKE_NO,
                                         C.TAR_MAKE_NO,
                                         C.BIN_CODE,
										 C.SOURCE_NO ITEM_SOURCE_NO,
										 C.SOURCE_TYPE ITEM_SOURCE_TYPE,
										 C.REFENCE_NO,
										 C.REFENCE_SEQ,
										 STRING_AGG(C.ALLOCATION_SEQ, ',') ALLOCATION_SEQ_LIST,
                     C.SERIAL_NO SerialNo
									FROM F151001 B
										 JOIN F151002 C
											ON     B.ALLOCATION_NO = C.ALLOCATION_NO
											   AND B.DC_CODE = C.DC_CODE
											   AND B.GUP_CODE = C.GUP_CODE
											   AND B.CUST_CODE = C.CUST_CODE
                         AND (C.SRC_QTY > 0 OR C.TAR_QTY > 0)
								GROUP BY B.SRC_DC_CODE,
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,
										 B.ALLOCATION_DATE,
										 B.STATUS,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,
										 C.SRC_LOC_CODE,
										 C.SUG_LOC_CODE,
										 C.TAR_LOC_CODE,
										 C.SRC_STAFF,
										 C.SRC_NAME,
										 C.TAR_STAFF,
										 C.BOX_CTRL_NO,
										 C.PALLET_CTRL_NO,
										 C.TAR_NAME,B.SOURCE_NO,
                     C.VALID_DATE,
                     C.SRC_VALID_DATE,
                     C.TAR_VALID_DATE,
                     C.ENTER_DATE,
                     C.MAKE_NO,
                     C.SRC_MAKE_NO,
                     C.TAR_MAKE_NO,
                     C.BIN_CODE,
					           C.SOURCE_NO,
					           C.SOURCE_TYPE,
					           C.REFENCE_NO,
					           C.REFENCE_SEQ,
										 C.STATUS,
                     C.SERIAL_NO) A
							   LEFT JOIN F1903 E
								  ON     A.GUP_CODE = E.GUP_CODE
									 AND A.CUST_CODE = E.CUST_CODE
									 AND A.ITEM_CODE = E.ITEM_CODE
                               LEFT JOIN F000902 F
								    ON A.ITEM_SOURCE_TYPE = F.SOURCE_TYPE
						 WHERE     A.ALLOCATION_NO = @p0
							   AND A.DC_CODE = @p1
							   AND A.GUP_CODE = @p2
							   AND A.CUST_CODE = @p3";
            
            var result = SqlQuery<F151001DetailDatas>(sql, parameters);
            return result;
        }

        public IQueryable<F151001ReportData> GetF151001ReportData(string dcCode, string gupCode, string custCode, string allocationNo, bool isShowValidDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo)
            };
            var validDateString = string.Empty;
            if (isShowValidDate)
                validDateString = " ,B.VALID_DATE ";
            var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO) ROWNUM,A.*
										FROM (
										Select A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO,A.SRC_WAREHOUSE_ID,A.TAR_WAREHOUSE_ID,
																									B.ITEM_CODE,B.SRC_LOC_CODE,B.SUG_LOC_CODE,SUM(B.SRC_QTY) SRC_QTY,SUM(B.TAR_QTY) TAR_QTY,
																									C.ITEM_NAME, C.ITEM_COLOR , C.ITEM_SIZE , C.ITEM_SPEC,
																									N1.WAREHOUSE_NAME SRC_WAREHOUSE_NAME,N2.WAREHOUSE_NAME TAR_WAREHOUSE_NAME,
																									O.GUP_NAME,N.CUST_NAME,B.SERIAL_NO {validDateString}
																				From F151001 A
																				Join F151002 B On A.DC_CODE=B.DC_CODE And A.GUP_CODE=B.GUP_CODE And A.CUST_CODE=B.CUST_CODE And A.ALLOCATION_NO=B.ALLOCATION_NO
																				Left Join F1903 C On C.GUP_CODE=B.GUP_CODE And C.ITEM_CODE=B.ITEM_CODE And C.CUST_CODE=B.CUST_CODE
																				Left Join F1980 N1 On A.DC_CODE=N1.DC_CODE And A.SRC_WAREHOUSE_ID=N1.WAREHOUSE_ID
																				Left Join F1980 N2 On A.TAR_DC_CODE=N2.DC_CODE And A.TAR_WAREHOUSE_ID=N2.WAREHOUSE_ID
																				Left Join F1929 O On A.GUP_CODE=O.GUP_CODE
																				Left Join F1909 N On A.GUP_CODE=N.GUP_CODE And A.CUST_CODE=N.CUST_CODE
																			 WHERE A.DC_CODE   = @p0
																				 AND A.GUP_CODE  = @p1
																				 AND A.CUST_CODE = @p2
																				 AND A.ALLOCATION_NO = @p3
																			GROUP BY A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO,A.SRC_WAREHOUSE_ID,A.TAR_WAREHOUSE_ID,B.ITEM_CODE,B.SRC_LOC_CODE,B.SUG_LOC_CODE,C.ITEM_NAME, C.ITEM_COLOR , C.ITEM_SIZE , C.ITEM_SPEC,
																							 N1.WAREHOUSE_NAME,N2.WAREHOUSE_NAME,O.GUP_NAME,N.CUST_NAME,B.SERIAL_NO {validDateString}
																							 ) A";

            var result = SqlQuery<F151001ReportData>(sql, parameters.ToArray());
            return result;
        }

        public IQueryable<F151001ReportDataByExpendDate> GetF151001ReportDataByExpendDate(string dcCode, string gupCode, string custCode, string allocationNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo)
            };
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY W.DC_CODE,W.GUP_CODE,W.CUST_CODE,W.ALLOCATION_NO) ROWNUM,
						W.DC_CODE,W.GUP_CODE,W.CUST_CODE,W.ALLOCATION_NO,W.SRC_WAREHOUSE_ID,W.TAR_WAREHOUSE_ID,
															W.ITEM_CODE,W.SRC_LOC_CODE,W.SUG_LOC_CODE,SUM(W.SRC_QTY) SRC_QTY,SUM(W.TAR_QTY) TAR_QTY,
                                                            SUM(W.A_TAR_QTY) A_TAR_QTY,SUM( W.A_SRC_QTY) A_SRC_QTY,
															W.ITEM_NAME, W.ITEM_COLOR , W.ITEM_SIZE , W.ITEM_SPEC,
															W.SRC_WAREHOUSE_NAME,W.TAR_WAREHOUSE_NAME,
															W.GUP_NAME,W.CUST_NAME,W.SERIAL_NO,W.VALID_DATE,W.ENTER_DATE,W.TAR_LOC_CODE FROM (SELECT A.DC_CODE,
															A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO,A.SRC_WAREHOUSE_ID,A.TAR_WAREHOUSE_ID,
															B.ITEM_CODE,B.SRC_LOC_CODE,B.SUG_LOC_CODE,B.SRC_QTY ,B.TAR_QTY ,
                                                            B.A_TAR_QTY ,B.A_SRC_QTY,
															C.ITEM_NAME, C.ITEM_COLOR , C.ITEM_SIZE , C.ITEM_SPEC,
															N1.WAREHOUSE_NAME SRC_WAREHOUSE_NAME,N2.WAREHOUSE_NAME TAR_WAREHOUSE_NAME,
															O.GUP_NAME,N.CUST_NAME,ISNULL(B.SERIAL_NO,'') SERIAL_NO,B.VALID_DATE,B.ENTER_DATE,B.TAR_LOC_CODE
										From F151001 A
										Join F151002 B On A.DC_CODE=B.DC_CODE And A.GUP_CODE=B.GUP_CODE And A.CUST_CODE=B.CUST_CODE And A.ALLOCATION_NO=B.ALLOCATION_NO
										Left Join F1903 C On C.GUP_CODE=B.GUP_CODE And C.ITEM_CODE=B.ITEM_CODE  And C.CUST_CODE=B.CUST_CODE
										Left Join F1980 N1 On A.DC_CODE=N1.DC_CODE And A.SRC_WAREHOUSE_ID=N1.WAREHOUSE_ID
										Left Join F1980 N2 On A.TAR_DC_CODE=N2.DC_CODE And A.TAR_WAREHOUSE_ID=N2.WAREHOUSE_ID
										Left Join F1929 O On A.GUP_CODE=O.GUP_CODE
										Left Join F1909 N On A.GUP_CODE=N.GUP_CODE And A.CUST_CODE=N.CUST_CODE
										 WHERE A.DC_CODE   = @p0
										 AND A.GUP_CODE  = @p1
										 AND A.CUST_CODE = @p2
										 AND A.ALLOCATION_NO = @p3 
									) W  
										  GROUP BY W.DC_CODE,W.GUP_CODE,W.CUST_CODE,W.ALLOCATION_NO,W.SRC_WAREHOUSE_ID,W.TAR_WAREHOUSE_ID,
															W.ITEM_CODE,W.SRC_LOC_CODE,W.SUG_LOC_CODE,
															W.ITEM_NAME, W.ITEM_COLOR , W.ITEM_SIZE , W.ITEM_SPEC,
															W.SRC_WAREHOUSE_NAME,W.TAR_WAREHOUSE_NAME,
															W.GUP_NAME,W.CUST_NAME,W.SERIAL_NO,W.VALID_DATE,W.ENTER_DATE,W.TAR_LOC_CODE";
            var result = SqlQuery<F151001ReportDataByExpendDate>(sql, parameters.ToArray());
            return result;
        }

        public IQueryable<F1510Data> GetF1510Data(string dcCode, string gupCode, string custCode, string allocationNo, string allocationDate, string status, string userId, string makeNo, DateTime enterDate, string srcLocCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", allocationDate),
                new SqlParameter("@p5", status),
                new SqlParameter("@p6", makeNo),
                new SqlParameter("@p7", userId),
                new SqlParameter("@p8", enterDate),
                new SqlParameter("@p9", srcLocCode),
            };
            var sql = @"
						SELECT A.ALLOCATION_DATE,
								 A.ALLOCATION_NO,
								 V.MAKE_NO,
								 V.ITEM_CODE,
								 C.ITEM_NAME,
								 V.SUG_LOC_CODE,
								 V.TAR_LOC_CODE,
								 V.SRC_LOC_CODE,
								 ISNULL (C.BUNDLE_SERIALLOC, '0') AS BUNDLE_SERIALLOC,
								 A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 V.VALID_DATE,
								 SUM (V.SRC_QTY) QTY,
								 ISNULL (D.CHECK_SERIALNO, '1') CHECK_SERIALNO,
								 A.TAR_DC_CODE,
								 A.TAR_WAREHOUSE_ID,
								 V.PALLET_CTRL_NO,
								 V.BOX_CTRL_NO
							FROM F151001 A
								 JOIN F151002 V
									ON     A.GUP_CODE = V.GUP_CODE
									   AND A.CUST_CODE = V.CUST_CODE
									   AND A.DC_CODE = V.DC_CODE
									   AND A.ALLOCATION_NO = V.ALLOCATION_NO
								 INNER JOIN F1903 C
									ON     V.ITEM_CODE = C.ITEM_CODE
									   AND A.GUP_CODE = C.GUP_CODE
									   AND A.CUST_CODE = C.CUST_CODE
								 LEFT JOIN
								 (  --取得只要有一筆是未刷讀 則為未刷讀(0) 否則為已刷讀(1)
									SELECT D.ALLOCATION_DATE,
										   D.ALLOCATION_NO,
										   D.ITEM_CODE,
										   D.SUG_LOC_CODE,
										   D.TAR_LOC_CODE,
										   D.DC_CODE,
										   D.GUP_CODE,
										   D.CUST_CODE,
										   D.VALID_DATE,
										   '0' AS CHECK_SERIALNO,
										   D.PALLET_CTRL_NO,
										   D.BOX_CTRL_NO,
										   D.MAKE_NO
									  FROM F151002 D
									 WHERE     D.CHECK_SERIALNO = 0
										   AND EXISTS                                      -- 使用者可處理儲位
												  (SELECT 1
													 FROM F1924 E
														  INNER JOIN F192403 F ON F.EMP_ID = E.EMP_ID
														  INNER JOIN F1963 G
															 ON     G.WORK_ID = F.WORK_ID
																AND G.ISDELETED = '0'
														  INNER JOIN F196301 H ON H.WORK_ID = G.WORK_ID
														  INNER JOIN F1912 I
															 ON     I.LOC_CODE = H.LOC_CODE
																AND I.DC_CODE = H.DC_CODE
													WHERE     I.DC_CODE = D.DC_CODE
														  AND I.LOC_CODE = D.TAR_LOC_CODE
														  AND E.EMP_ID = @p6)
								  GROUP BY D.ALLOCATION_DATE,
										   D.ALLOCATION_NO,
										   D.ITEM_CODE,
										   D.SUG_LOC_CODE,
										   D.TAR_LOC_CODE,
										   D.DC_CODE,
										   D.GUP_CODE,
										   D.CUST_CODE,
										   D.VALID_DATE,
										   D.PALLET_CTRL_NO,
										   D.BOX_CTRL_NO,
										   D.MAKE_NO) D
									ON     A.ALLOCATION_DATE = D.ALLOCATION_DATE
									   AND A.ALLOCATION_NO = D.ALLOCATION_NO
									   AND V.ITEM_CODE = D.ITEM_CODE
									   AND V.SUG_LOC_CODE = D.SUG_LOC_CODE
									   AND V.TAR_LOC_CODE = D.TAR_LOC_CODE
									   AND A.DC_CODE = D.DC_CODE
									   AND A.GUP_CODE = D.GUP_CODE
									   AND A.CUST_CODE = D.CUST_CODE
									   AND V.VALID_DATE = D.VALID_DATE
									   AND V.PALLET_CTRL_NO = D.PALLET_CTRL_NO
									   AND V.BOX_CTRL_NO = D.BOX_CTRL_NO
									   AND V.MAKE_NO = D.MAKE_NO
						   WHERE     A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND A.ALLOCATION_NO = @p3
								 AND CONVERT (datetime,A.ALLOCATION_DATE,111) = @p4
								 AND A.STATUS = @p5
								 AND A.LOCK_STATUS != '4'
								 AND V.MAKE_NO = @p6
								 AND V.ENTER_DATE = @p8
								 AND V.SRC_LOC_CODE = @p9
								 AND EXISTS                                                -- 使用者可處理儲位
										(SELECT 1
										   FROM F1924 E
												INNER JOIN F192403 F ON F.EMP_ID = E.EMP_ID
												INNER JOIN F1963 G
												   ON G.WORK_ID = F.WORK_ID AND G.ISDELETED = '0'
												INNER JOIN F196301 H ON H.WORK_ID = G.WORK_ID
												INNER JOIN F1912 I
												   ON I.LOC_CODE = H.LOC_CODE AND I.DC_CODE = H.DC_CODE
										  WHERE     I.DC_CODE = A.DC_CODE
												AND I.LOC_CODE = V.TAR_LOC_CODE
												AND E.EMP_ID = @p7)
						GROUP BY A.ALLOCATION_DATE,
								 A.ALLOCATION_NO,
								 V.ITEM_CODE,
								 C.ITEM_NAME,
								 V.SUG_LOC_CODE,
								 V.TAR_LOC_CODE,
								 V.SRC_LOC_CODE,
								 C.BUNDLE_SERIALLOC,
								 A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 V.VALID_DATE,
								 D.CHECK_SERIALNO,
								 A.TAR_DC_CODE,
								 A.TAR_WAREHOUSE_ID,
								 V.MAKE_NO,
								 V.PALLET_CTRL_NO,
								 V.BOX_CTRL_NO
						ORDER BY V.ITEM_CODE, V.TAR_LOC_CODE
						";
            var result = SqlQuery<F1510Data>(sql, parameters.ToArray()).AsQueryable();

            return result;
          
        }
        #endregion

        #region P0203010000 調入上架
        public IQueryable<F151001WithF02020107> GetDatasByTar(string tarDcCode, string gupCode, string custCode, DateTime allocationDate, string[] status)
        {
            var parameter = new List<object> { tarDcCode, gupCode, custCode, allocationDate, Current.Staff };

            var sql = @"
						SELECT A.*, B.RT_NO
						  FROM F151001 A
							   JOIN F02020107 B
								  ON     A.ALLOCATION_NO = B.ALLOCATION_NO
									 AND A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
								LEFT  JOIN F1980 C
							   ON A.TAR_DC_CODE  = C.DC_CODE 
							   AND A.TAR_WAREHOUSE_ID  = C.WAREHOUSE_ID 
						 WHERE     A.TAR_DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND A.ALLOCATION_DATE = @p3
								 AND C.DEVICE_TYPE ='0'
								AND EXISTS (SELECT 1 FROM F020201 --只抓F020201.RT_MODE=0 驗收單資料
								 WHERE B.DC_CODE = DC_CODE 
								 AND B.GUP_CODE=GUP_CODE 
								 AND B.CUST_CODE=CUST_CODE 
								 AND B.RT_NO = RT_NO
								 AND B.PURCHASE_NO = PURCHASE_NO
								 AND RT_MODE = '0')
							   AND NOT EXISTS       -- 必須調撥單明細所有儲位為使用者都有權限(反向思考 只要有一個儲位不是使用者擁有的權限就不能使用此調撥單)
										  (SELECT 1
											 FROM F151002 F
												  INNER JOIN F151001 M
													 ON     F.ALLOCATION_NO = M.ALLOCATION_NO
														AND F.DC_CODE = M.DC_CODE
														AND F.GUP_CODE = M.GUP_CODE
														AND F.CUST_CODE = M.CUST_CODE
												  LEFT JOIN F1912 G
													 ON     G.LOC_CODE = F.SUG_LOC_CODE
														AND G.DC_CODE = M.TAR_DC_CODE
												  LEFT JOIN F196301 H ON H.LOC_CODE = G.LOC_CODE
												  LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID
												  LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID
												  LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID
											WHERE     H.LOC_CODE IS NULL
												  AND K.EMP_ID = @p4
												  AND F.DC_CODE = A.DC_CODE
												  AND F.GUP_CODE = A.GUP_CODE
												  AND F.CUST_CODE = A.CUST_CODE
												  AND F.ALLOCATION_NO = A.ALLOCATION_NO)";
            if (status.Any())
                sql += parameter.CombineSqlInParameters(" AND A.STATUS", status);


            var result = SqlQuery<F151001WithF02020107>(sql, parameter.ToArray());
            return result;
        }

        public IQueryable<F1510Data> GetF1510DatasByTar(string tarDcCode, string gupCode, string custCode, string allocationNo, DateTime allocationDate)
        {
            var sql = @" SELECT distinct L.*
                        ,CASE WHEN M.COUNT_NO > 0 Then '0' ELSE '1' END IS_NEW_ITEM 
                        FROM (
                        SELECT TOP 100 PERCENT ROW_NUMBER()OVER(ORDER by B.ITEM_CODE ,A.ALLOCATION_NO , B.SUG_LOC_CODE) ROWNUM,
                                A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_DATE,A.ALLOCATION_NO,A.TAR_DC_CODE,A.TAR_WAREHOUSE_ID,
					          B.ITEM_CODE,B.SRC_LOC_CODE,B.SUG_LOC_CODE,B.TAR_LOC_CODE,B.QTY,B.VALID_DATE,
								        D.ITEM_NAME,D.BUNDLE_SERIALLOC, 
								        CASE WHEN ISNULL(E.CountCHECK_SERIALNO,0) > 0 THEN '0' ELSE '1' END CHECK_SERIALNO ,B.MAKE_NO , B.PALLET_CTRL_NO, B.BOX_CTRL_NO
					     FROM F151001 A 
					    INNER JOIN ( 
					     SELECT DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO,ITEM_CODE,SRC_LOC_CODE,SUG_LOC_CODE,TAR_LOC_CODE,VALID_DATE,SUM(TAR_QTY) QTY ,MAKE_NO, PALLET_CTRL_NO, BOX_CTRL_NO
					       FROM F151002 
					      WHERE STATUS !='2' 
					      GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO,ALLOCATION_DATE,ITEM_CODE,SRC_LOC_CODE,SUG_LOC_CODE,TAR_LOC_CODE,VALID_DATE,MAKE_NO,PALLET_CTRL_NO, BOX_CTRL_NO ) B 
					       ON B.DC_CODE = A.DC_CODE 
					      AND B.GUP_CODE = A.GUP_CODE 
					      AND B.CUST_CODE = A.CUST_CODE 
					      AND B.ALLOCATION_NO = A.ALLOCATION_NO 
					    INNER JOIN F1903 D 
					       ON D.GUP_CODE = B.GUP_CODE 
					      AND D.CUST_CODE = B.CUST_CODE 
					      AND D.ITEM_CODE = B.ITEM_CODE 
					     LEFT JOIN ( 
					     SELECT DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO,ITEM_CODE,SUG_LOC_CODE,TAR_LOC_CODE,COUNT(CHECK_SERIALNO) CountCHECK_SERIALNO ,MAKE_NO
					        FROM F151002 
					       WHERE CHECK_SERIALNO = 0 
					       GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO,ITEM_CODE,SUG_LOC_CODE,TAR_LOC_CODE,MAKE_NO) E 
					       ON E.DC_CODE = B.DC_CODE 
					      AND E.GUP_CODE = B.GUP_CODE 
					      AND E.CUST_CODE = B.CUST_CODE 
					      AND E.ALLOCATION_NO = B.ALLOCATION_NO 
					      AND E.ITEM_CODE = B.ITEM_CODE 
					      AND E.SUG_LOC_CODE = B.SUG_LOC_CODE 
					      AND E.TAR_LOC_CODE = B.TAR_LOC_CODE 
						  AND E.MAKE_NO = B.MAKE_NO
					    WHERE A.TAR_DC_CODE = @p0 
					      AND A.GUP_CODE = @p1 
					      AND A.CUST_CODE = @p2 
					      AND A.ALLOCATION_NO = @p3 
								    AND A.ALLOCATION_DATE = @p4 
								    AND A.STATUS IN('0','1','3') -- 只抓取Status = 0(待處理) 或 1 (已列印調撥單) 或 3(已下架處理)
								    AND NOT EXISTS (   --必須調撥單明細所有儲位為使用者都有權限(反向思考 只要有一個儲位不是使用者擁有的權限就不能使用此調撥單)
								            SELECT 1  
								              FROM F151002 F 
								              INNER JOIN F151001 M  
								               ON F.ALLOCATION_NO = M.ALLOCATION_NO 
								               AND F.DC_CODE = M.DC_CODE 
								               AND F.GUP_CODE = M.GUP_CODE 
								               AND F.CUST_CODE = M.CUST_CODE 
								              LEFT JOIN F1912 G ON G.LOC_CODE = F.SUG_LOC_CODE AND G.DC_CODE = M.TAR_DC_CODE 
								              LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE
								              LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID 
								              LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID 
								              LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID 
								             WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p5 
								               AND F.DC_CODE = A.DC_CODE 
								               AND F.GUP_CODE =A.GUP_CODE 
								               AND F.CUST_CODE=A.CUST_CODE 
								               AND F.ALLOCATION_NO = A.ALLOCATION_NO 
								        )  ORDER BY B.ITEM_CODE ,A.ALLOCATION_NO , B.SUG_LOC_CODE ) L 
	                    LEFT JOIN (
                            SELECT F1913.DC_CODE
                                ,F1913.LOC_CODE
                                ,F1913.GUP_CODE
                                ,F1913.CUST_CODE
                                ,ITEM_CODE
                                ,COUNT(ITEM_CODE) COUNT_NO 
                            FROM F1913
                            JOIN 
                                F1912 
                            ON F1913.LOC_CODE = F1912.LOC_CODE
                                AND F1913.DC_CODE = F1912.DC_CODE
                                AND F1912.WAREHOUSE_ID<>'I01'
                            Group by F1913.ITEM_CODE,F1913.DC_CODE,F1913.LOC_CODE,F1913.GUP_CODE,F1913.CUST_CODE ) M  
                    ON L.DC_CODE = M.DC_CODE
                        AND L.GUP_CODE = M.GUP_CODE
                        AND L.CUST_CODE = M.CUST_CODE
                        AND L.ITEM_CODE = M.ITEM_CODE 
										LEFT JOIN F1980 N 
                    on L.DC_CODE = N.DC_CODE 
                    and L.TAR_WAREHOUSE_ID = N.WAREHOUSE_ID 
                    where N.DEVICE_TYPE ='0'";
            var parameter = new object[] { tarDcCode, gupCode, custCode, allocationNo, allocationDate, Current.Staff };
            var result = SqlQuery<F1510Data>(sql, parameter);
            return  result;
        }

        public IQueryable<F1510BundleSerialLocData> GetF1510BundleSerialLocDatas(string dcCode, string gupCode, string custCode,
            string allocationNo, string checkSerialNo, string userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", checkSerialNo),
                new SqlParameter("@p5", userId)
            };

            var sql = " SELECT ROW_NUMBER ()OVER(ORDER BY B.ITEM_CODE,B.TAR_LOC_CODE,B.SERIAL_NO) ROWNUM,B.DC_CODE,B.GUP_CODE,B.CUST_CODE, " +
                                "        B.ALLOCATION_DATE,B.ALLOCATION_NO,B.ALLOCATION_SEQ, " +
                        "        B.ITEM_CODE,D.ITEM_NAME,D.ITEM_SPEC,D.ITEM_SIZE,D.ITEM_COLOR, " +
                                "        B.TAR_LOC_CODE,B.SERIAL_NO,B.TAR_QTY  " +
                        "   FROM F151001 A " +
                        "  INNER JOIN F151002 B " +
                        "     ON B.DC_CODE = A.DC_CODE " +
                        "    AND B.GUP_CODE = A.GUP_CODE " +
                        "    AND B.CUST_CODE = A.CUST_CODE " +
                        "    AND B.ALLOCATION_NO = A.ALLOCATION_NO " +
                        "   INNER JOIN F1903 D " +
                        "     ON D.GUP_CODE = B.GUP_CODE " +
                        "    AND D.CUST_CODE = B.CUST_CODE " +
                        "    AND D.ITEM_CODE = B.ITEM_CODE " +
                        "  WHERE A.DC_CODE = @p0 " +
                        "    AND A.GUP_CODE = @p1 " +
                        "    AND A.CUST_CODE = @p2 " +
                        "    AND A.ALLOCATION_NO = @p3 " +
                                "    AND B.CHECK_SERIALNO  =  @p4 " +
                        //"    AND A.STATUS = '1' " + //只抓取Status= 1 (已列印調撥單)
                        "    AND ISNULL(D.BUNDLE_SERIALLOC,0) = '1' " + //序號綁儲位
                        "    AND NOT EXISTS (   " + //必須調撥單明細所有儲位為使用者都有權限(反向思考 只要有一個儲位不是使用者擁有的權限就不能使用此調撥單)
                        "            SELECT 1  " +
                        "              FROM F151002 F " +
                        "              INNER JOIN F151001 M  " +
                        "               ON F.ALLOCATION_NO = M.ALLOCATION_NO " +
                        "               AND F.DC_CODE = M.DC_CODE " +
                        "               AND F.GUP_CODE = M.GUP_CODE " +
                        "               AND F.CUST_CODE = M.CUST_CODE " +
                        "              LEFT JOIN F1912 G ON G.LOC_CODE = F.SUG_LOC_CODE AND G.DC_CODE = M.TAR_DC_CODE " +
                        "              LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE" +
                        "              LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID " +
                        "              LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID " +
                        "              LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID " +
                        "             WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p5 " +
                        "               AND F.DC_CODE = A.DC_CODE " +
                        "               AND F.GUP_CODE =A.GUP_CODE " +
                        "               AND F.CUST_CODE=A.CUST_CODE " +
                        "               AND F.ALLOCATION_NO = A.ALLOCATION_NO " +
                        "        ) " +
                        "  ORDER BY B.ITEM_CODE,B.TAR_LOC_CODE,B.SERIAL_NO ";
            var result = SqlQuery<F1510BundleSerialLocData>(sql, parameters.ToArray());
            return result;
        }

        public IQueryable<F1510ItemLocData> GetF1510ItemLocDatas(string tarDcCode, string gupCode, string custCode,
            string allocationNo, string[] status, string itemCode, DateTime validDate, string srcLocCode, string makeNo)
        {
            var parameter = new List<object> { tarDcCode, gupCode, custCode, allocationNo, itemCode, validDate, srcLocCode, Current.Staff, makeNo };
            var sql = @"
					  SELECT ROW_NUMBER () OVER(ORDER By  B.TAR_LOC_CODE) ROWNUM,
							 'Normal' AS ChangeStatus,
							 A.ALLOCATION_DATE,
							 A.ALLOCATION_NO,
							 B.ITEM_CODE,
							 C.ITEM_NAME,
							 D.WAREHOUSE_ID,
							 E.WAREHOUSE_NAME,
							 B.SUG_LOC_CODE,
							 B.TAR_LOC_CODE,
							 B.QTY AS ORGINAL_QTY,
							 B.VALID_DATE,
							 B.QTY,
							 A.DC_CODE,
							 A.GUP_CODE,
							 A.CUST_CODE
						FROM F151001 A
							 INNER JOIN (  SELECT DC_CODE,
												  GUP_CODE,
												  CUST_CODE,
												  ALLOCATION_NO,
												  ITEM_CODE,
												  SUG_LOC_CODE,
												  TAR_LOC_CODE,
												  SRC_LOC_CODE,
												  VALID_DATE,
												  SUM (TAR_QTY) QTY,
												  MAKE_NO
											 FROM F151002
											WHERE STATUS != '2'
                            AND (SRC_QTY > 0 OR TAR_QTY > 0)
										 GROUP BY DC_CODE,
												  GUP_CODE,
												  CUST_CODE,
												  ALLOCATION_NO,
												  ALLOCATION_DATE,
												  ITEM_CODE,
												  SUG_LOC_CODE,
												  TAR_LOC_CODE,
												  SRC_LOC_CODE,
												  VALID_DATE,
												  MAKE_NO) B
								ON     B.DC_CODE = A.DC_CODE
								   AND B.GUP_CODE = A.GUP_CODE
								   AND B.CUST_CODE = A.CUST_CODE
								   AND B.ALLOCATION_NO = A.ALLOCATION_NO
							 INNER JOIN F1903 C
								ON C.GUP_CODE = B.GUP_CODE AND C.ITEM_CODE = B.ITEM_CODE AND C.CUST_CODE = B.CUST_CODE
							 INNER JOIN F1912 D
								ON D.DC_CODE = A.TAR_DC_CODE AND D.LOC_CODE = B.TAR_LOC_CODE
							 INNER JOIN F1980 E
								ON E.DC_CODE = A.TAR_DC_CODE AND E.WAREHOUSE_ID = D.WAREHOUSE_ID
					   WHERE     A.TAR_DC_CODE = @p0
							 AND A.GUP_CODE = @p1
							 AND A.CUST_CODE = @p2
							 AND A.ALLOCATION_NO = @p3
							 AND B.ITEM_CODE = @p4
							 AND B.VALID_DATE = @p5
							 AND B.SRC_LOC_CODE = @p6
							 AND NOT EXISTS
									(SELECT 1
									   FROM F151002 F
											INNER JOIN F151001 M
											   ON     F.ALLOCATION_NO = M.ALLOCATION_NO
												  AND F.DC_CODE = M.DC_CODE
												  AND F.GUP_CODE = M.GUP_CODE
												  AND F.CUST_CODE = M.CUST_CODE
											LEFT JOIN F1912 G
											   ON     G.LOC_CODE = F.SUG_LOC_CODE
												  AND G.DC_CODE = M.TAR_DC_CODE
											LEFT JOIN F196301 H ON H.LOC_CODE = G.LOC_CODE
											LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID
											LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID
											LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID
									  WHERE     H.LOC_CODE IS NULL
											AND K.EMP_ID = @p7
											AND F.DC_CODE = A.DC_CODE
											AND F.GUP_CODE = A.GUP_CODE
											AND F.CUST_CODE = A.CUST_CODE
											AND F.ALLOCATION_NO = A.ALLOCATION_NO)
							 AND B.MAKE_NO = @p8
					";

            if (status.Any())
                sql += " AND A.STATUS IN (";
            foreach (var s in status)
            {
                sql += "@p" + parameter.Count();
                if (s != status.Last())
                    sql += ",";
                parameter.Add(s);
            }
            if (status.Any())
                sql += ") ";

            sql += "  ORDER BY B.TAR_LOC_CODE ";
            return SqlQuery<F1510ItemLocData>(sql, parameter.ToArray());
        }
        #endregion

        public IQueryable<DcWmsNoOrdPropItem> GetDcWmsNoOrdPropItems(string dcCode, DateTime allocationDate)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.CUST_CODE) ROWNUM, A.* 
                     FROM ( 
									   SELECT A.CUST_CODE,null AS ORD_PROP,
														SUM(CASE WHEN A.STATUS = '5' THEN 1 ELSE 0 END) AS CUST_FINISHCOUNT,
														COUNT(*)  AS CUST_TOTALCOUNT 
											FROM F151001 A
											WHERE A.DC_CODE = @p0
											AND A.ALLOCATION_DATE = @p1
											AND A.STATUS <>'9'
											GROUP BY A.CUST_CODE ) A ";
            var param = new object[] { dcCode, allocationDate.Date };

            var result = SqlQuery<DcWmsNoOrdPropItem>(sql, param);
            return result;
        }

        public IQueryable<AllocationBundleSerialLocCount> GetAllocationBundleSerialLocCount(string dcCode, string gupCode,
            string custCode, string allocationNo, string userId)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.ALLOCATION_NO) ROWNUM,A.*
										 FROM (
										 SELECT A.ALLOCATION_NO,SUM(CASE WHEN A.TAR_WAREHOUSE_ID IS NULL THEN B.SRC_QTY ELSE B.TAR_QTY END) RequiredQty,SUM(CASE WHEN B.CHECK_SERIALNO = '1' THEN CASE WHEN A.TAR_WAREHOUSE_ID IS NULL THEN B.SRC_QTY ELSE B.TAR_QTY END ELSE 0 END) AS CheckSerialNoQty
										   FROM F151001 A
										  INNER JOIN F151002 B
										     ON B.DC_CODE = A.DC_CODE
										    AND B.GUP_CODE = A.GUP_CODE
										    AND B.CUST_CODE = A.CUST_CODE
										    AND B.ALLOCATION_NO = A.ALLOCATION_NO
										  INNER JOIN F1903 C
										     ON C.GUP_CODE = B.GUP_CODE
										    AND C.CUST_CODE= B.CUST_CODE
										    AND C.ITEM_CODE = B.ITEM_CODE
										  WHERE C.BUNDLE_SERIALLOC ='1'
										    AND A.DC_CODE =@p0
										    AND A.GUP_CODE =@p1
										    AND A.CUST_CODE = @p2
										    AND A.ALLOCATION_NO =@p3
										    AND NOT EXISTS (   
														SELECT 1  
															FROM F151002 F 
														 INNER JOIN F151001 M  
															  ON F.ALLOCATION_NO = M.ALLOCATION_NO 
															 AND F.DC_CODE = M.DC_CODE 
															 AND F.GUP_CODE = M.GUP_CODE 
															 AND F.CUST_CODE = M.CUST_CODE 
															LEFT JOIN F1912 G ON G.LOC_CODE = CASE WHEN M.TAR_WAREHOUSE_ID IS NULL THEN  F.SRC_LOC_CODE ELSE F.SUG_LOC_CODE END AND G.DC_CODE = CASE WHEN M.TAR_WAREHOUSE_ID IS NULL THEN M.SRC_DC_CODE ELSE M.TAR_DC_CODE END 
															LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE
															LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID 
															LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID 
															LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID 
														 WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p4 
															 AND F.DC_CODE = A.DC_CODE 
															 AND F.GUP_CODE =A.GUP_CODE 
															 AND F.CUST_CODE=A.CUST_CODE 
															 AND F.ALLOCATION_NO = A.ALLOCATION_NO 
												) 
										GROUP BY A.ALLOCATION_NO ) A ";
            var param = new object[] { dcCode, gupCode, custCode, allocationNo, userId };
            var result = SqlQuery<AllocationBundleSerialLocCount>(sql, param);
            return result;
        }

        public IQueryable<F15100101Data> GetF15100101Data(string dcCode, string gupCode, string custCode, string allocationNo,
            string userId)
        {
            var sql =
                @" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO,B.SERIAL_NO,B.LOC_CODE,B.ITEM_CODE,ISNULL(C.ITEM_NAME,'') ITEM_NAME,ISNULL(C.ITEM_SIZE,'') ITEM_SIZE,C.ITEM_SPEC,ISNULL(C.ITEM_COLOR,'') ITEM_COLOR,B.ISPASS,B.MESSAGE
										FROM F151001 A 
									 INNER JOIN F15100101 B
										  ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.ALLOCATION_NO = A.ALLOCATION_NO
									 LEFT JOIN F1903 C
										  ON C.GUP_CODE = B.GUP_CODE
										 AND C.ITEM_CODE =B.ITEM_CODE
                                         AND C.CUST_CODE =B.CUST_CODE
									 WHERE A.DC_CODE =@p0
										 AND A.GUP_CODE =@p1
										 AND A.CUST_CODE =@p2
										 AND A.ALLOCATION_NO=@p3
										 AND B.STATUS ='0'
										 AND NOT EXISTS (   
														SELECT 1  
															FROM F151002 F 
														 INNER JOIN F151001 M  
															  ON F.ALLOCATION_NO = M.ALLOCATION_NO 
															 AND F.DC_CODE = M.DC_CODE 
															 AND F.GUP_CODE = M.GUP_CODE 
															 AND F.CUST_CODE = M.CUST_CODE 
															LEFT JOIN F1912 G ON G.LOC_CODE = CASE WHEN M.TAR_WAREHOUSE_ID IS NULL THEN  F.SRC_LOC_CODE ELSE F.SUG_LOC_CODE END AND G.DC_CODE = CASE WHEN M.TAR_WAREHOUSE_ID IS NULL THEN M.SRC_DC_CODE ELSE M.TAR_DC_CODE END  
															LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE
															LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID 
															LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID 
															LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID 
														 WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p4
															 AND F.DC_CODE = A.DC_CODE 
															 AND F.GUP_CODE =A.GUP_CODE 
															 AND F.CUST_CODE=A.CUST_CODE 
															 AND F.ALLOCATION_NO = A.ALLOCATION_NO 
												) ";
            var param = new object[] { dcCode, gupCode, custCode, allocationNo, userId };
            return SqlQuery<F15100101Data>(sql, param);
        }

        public void DeleteF151001Datas(string gupCode, string custCode, string dcCode, List<string> allocationNos)
        {
            var parameters = new List<object>
            {
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				Delete From F151001
				 Where  GUP_CODE = @p0
					 And CUST_CODE = @p1
					 And DC_CODE = @p2";
            sql += parameters.CombineSqlInParameters(" AND ALLOCATION_NO ", allocationNos);
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void UpdateDatasForCancel(string gupCode, string custCode, string dcCode, List<string> ordNos)
        {
            var parameters = new List<object>
            {
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				UPDATE  F151001  SET STATUS='9'
				 Where   GUP_CODE = @p0
					 And CUST_CODE = @p1
					 And DC_CODE = @p2";

            sql += parameters.CombineSqlInParameters(" AND ALLOCATION_NO ", ordNos);
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        

       

       

     

        public IQueryable<HealthInsuranceReport> GetHealthInsurancePurchaseData(string dcCode, string gupCode, string custCode, DateTime? startDate, DateTime? endDate, string[] itemCodes)
        {
            var paramList = new List<object>() {
                                                                dcCode,
                                                                gupCode,
                                                                custCode
                                                };

            string sql = @"
SELECT  ROW_NUMBER ()OVER(ORDER BY B.CRT_DATE , B.ITEM_CODE) ROWNUM,'進貨' AS TRANSTYPE ,A.CRT_DATE,E.UNI_FORM, E.BOSS , E.CONTACT , F.SIM_SPEC , B.ITEM_CODE ,
 F.ITEM_NAME , B.MAKE_NO , B.VALID_DATE , D.VNR_NAME ,D.UNI_FORM AS VNR_UNI_FORM,
  B.SRC_QTY AS QTY ,CONCAT(B.SRC_QTY , G.ACC_UNIT_NAME) AS QTY1 , C.VNR_CODE AS VNR_RETIAL_CODE
FROM F151001 A
LEFT JOIN
F151002 B
ON A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND
A.ALLOCATION_NO = B.ALLOCATION_NO
LEFT JOIN F010201 C
ON A.DC_CODE = C.DC_CODE AND A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE AND
A.SOURCE_NO = C.STOCK_NO
LEFT JOIN F1908 D
ON A.GUP_CODE = D.GUP_CODE AND A.CUST_CODE = D.CUST_CODE AND D.VNR_CODE = C.VNR_CODE
LEFT JOIN F1909 E
ON A.GUP_CODE = E.GUP_CODE AND A.CUST_CODE = E.CUST_CODE
LEFT JOIN F1903 F
ON A.GUP_CODE = F.GUP_CODE AND A.CUST_CODE = F.CUST_CODE AND B.ITEM_CODE = F.ITEM_CODE
LEFT JOIN(
SELECT ITEM_TYPE_ID, ACC_UNIT, ACC_UNIT_NAME
FROM F91000302
WHERE  ITEM_TYPE_ID = '001')G
ON F.ITEM_UNIT = G.ACC_UNIT
WHERE A.SOURCE_TYPE = '04' AND A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 
 {0} 
ORDER BY B.CRT_DATE , B.ITEM_CODE";

            string addCondition = string.Empty;
            if (startDate != null && endDate != null)
            {
                addCondition += " AND A.CRT_DATE >= CONVERT(datetime, @p" + paramList.Count + ") ";
                paramList.Add(startDate.Value.ToString("yyyy/MM/dd"));
                addCondition += " AND A.CRT_DATE <= CONVERT(datetime, @p" + paramList.Count + ") ";
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

		/// <summary>
		/// 調撥單據查詢_進倉上架
		/// </summary>
		/// <param name="dcNo">物流中心編號</param>
		/// <param name="custNo">貨主編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="allocDate">調撥日期</param>
		/// <param name="wmsNo">調撥單號/驗收單號</param>
		/// <param name="itemNo">品號</param>
		/// <param name="palletNo">板號</param>
		/// <param name="allocationNos">調撥單號List</param>
		/// <returns></returns>
		public IQueryable<GetAllocRes> GetP810103DataByInbound(string dcNo, string custNo, string gupCode, DateTime? allocDate, string wmsNo, string itemNo, string palletNo,string serialNo, List<string> allocationNos)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", dcNo));
			parm.Add(new SqlParameter("@p1", custNo));
			parm.Add(new SqlParameter("@p2", gupCode));

			#region 條件過濾
			string condition = " AND E.DEVICE_TYPE ='0'"; // 人工倉
			string condition2 = string.Empty;
			string condition3 = string.Empty;
			string condition4 = string.Empty;
			string condition5 = string.Empty;

			// 若 AllocDate is not null, 增加條件 F151001.allocation_date= AllocDate
			if (allocDate != null)
			{
				condition += " AND A.ALLOCATION_DATE = @p3 ";
				parm.Add(new SqlParameter("@p3", allocDate));
			}

			// 若 WmsNo is not null, 增加條件F151001.allocation_no= WmsNo
			if (allocationNos.Count() > 0)
			{
				condition += $" AND A.ALLOCATION_NO in ('{string.Join("','", allocationNos)}') ";
			}

			// 若AllocType in (01, 03)，增加條件 F151001.status in (3,4) & F151002.status =1 (待上架)
			condition += " AND (A.STATUS = '3' OR A.STATUS = '4') ";
			condition2 += " AND X.STATUS = '1' ";
			condition4 += " AND A.TAR_WAREHOUSE_ID  = E.WAREHOUSE_ID "; // 進倉上架、調撥上架是JOIN來源倉別

			// 若ItemNo is not null, 增加條件F151002.item_code = ItemNo
			if (!string.IsNullOrWhiteSpace(itemNo))
			{
				condition += $@" AND (
                                 SELECT COUNT(Z.ITEM_CODE)  
                                 FROM F151002 Z JOIN F1903 X ON Z.GUP_CODE =X.GUP_CODE and Z.CUST_CODE = X.CUST_CODE and Z.ITEM_CODE =X.ITEM_CODE 
                                 WHERE Z.DC_CODE = A.DC_CODE 
                                    AND Z.GUP_CODE = A.GUP_CODE 
                                    AND Z.CUST_CODE = A.CUST_CODE 
                                    AND Z.ALLOCATION_NO = A.ALLOCATION_NO 
                                    AND (X.ITEM_CODE = @p4 OR X.EAN_CODE1 = @p4 OR  X.EAN_CODE2 = @p4 OR X.EAN_CODE3 = @p4)
                                 ) > 0 ";

				parm.Add(new SqlParameter("@p4", itemNo));
			}

			// 若PalletNo is not null, 增加條件F151002.pallet_ctrl_no=PalletNo
			if (!string.IsNullOrWhiteSpace(palletNo))
			{
				condition += $@" AND (
                                 SELECT COUNT(Z.PALLET_CTRL_NO)  
                                 FROM F151002 Z
                                    WHERE Z.DC_CODE = A.DC_CODE 
                                    AND Z.GUP_CODE = A.GUP_CODE 
                                    AND Z.CUST_CODE = A.CUST_CODE 
                                    AND Z.ALLOCATION_NO = A.ALLOCATION_NO 
                                    AND Z.PALLET_CTRL_NO = @p5
                                 ) > 0";
				parm.Add(new SqlParameter("@p5", palletNo));
			}

			// 若SerialNo is not null, 增加條件F2501.SERIAL_NO=SerialNo
			if (!string.IsNullOrWhiteSpace(serialNo))
			{
				
				condition += $@"  AND A.ALLOCATION_NO IN  (
					  SELECT ALLOCATION_NO 
					  FROM F151002 R
					  WHERE (ITEM_CODE IN
					  (SELECT f.ITEM_CODE 
					     FROM F2501 f 
					     join F1903 g
					     on f.GUP_CODE =g.GUP_CODE 
					     AND f.CUST_CODE =g.CUST_CODE 
					     AND f.ITEM_CODE =g.ITEM_CODE 
					     WHERE f.GUP_CODE = R.GUP_CODE 
					     and f.CUST_CODE = R.CUST_CODE 
					     and f.SERIAL_NO  = @p6
					     and f.STATUS = 'A1'
					     AND g.BUNDLE_SERIALNO = '1'
					     AND g.BUNDLE_SERIALLOC ='0'
					   )
					   OR R.SERIAL_NO = @p6) )";
				parm.Add(new SqlParameter("@p6", serialNo));
			}
			#endregion

			string sql = $@"SELECT 
                            A.DC_CODE AS DcNo,
                            A.CUST_CODE AS CustNo,
                            A.ALLOCATION_DATE AS AllocDate,
                            A.ALLOCATION_NO AS AllocNo,
                            '01' AS AllocType,
                            D.RT_NO AS SrcNo,
							F.WAREHOUSE_NAME AS SrcWhName,
							G.WAREHOUSE_NAME AS TarWhName,
                            COUNT(DISTINCT(B.ITEM_CODE)) AS ItemCnt,
                            SUM(B.TAR_QTY) AS ItemQty,
                            ISNULL(COUNT(DISTINCT(B.ITEM_CODE)) - 
                            (SELECT COUNT(DISTINCT X.ITEM_CODE)  FROM F151002 X
                            WHERE X.ALLOCATION_NO = A.ALLOCATION_NO 
                            AND X.DC_CODE = A.DC_CODE 
                            AND X.GUP_CODE = A.GUP_CODE 
                            AND X.CUST_CODE = A.CUST_CODE 
                            {condition2})
                            , 0) AS ActItemCnt,
                            A.STATUS AS Status,
                            C.NAME AS StatusName,
                            ISNULL(A.UPD_STAFF, '') AS AccNo,
                            ISNULL(A.UPD_NAME, '') AS UserName,
                            A.BOX_NO AS BoxNo,
							A.MEMO AS Memo
                            FROM F151001 A 
                            JOIN F151002 B 
                            ON A.ALLOCATION_NO = B.ALLOCATION_NO 
                            AND A.DC_CODE = B.DC_CODE 
                            AND A.GUP_CODE = B.GUP_CODE 
                            AND A.CUST_CODE = B.CUST_CODE
                            JOIN VW_F000904_LANG C 
                            ON  C.VALUE = A.STATUS
                            AND C.TOPIC = 'F151001' 
                            AND C.SUBTOPIC = 'STATUS'
                            AND C.LANG = '{Current.Lang}'
							LEFT JOIN F1980 F
							ON F.DC_CODE = A.DC_CODE
							AND F.WAREHOUSE_ID = A.SRC_WAREHOUSE_ID
							LEFT JOIN F1980 G
							ON G.DC_CODE = A.DC_CODE
							AND G.WAREHOUSE_ID = A.TAR_WAREHOUSE_ID
                            LEFT JOIN F02020107 D
                            ON D.DC_CODE = A.DC_CODE
                            AND D.GUP_CODE = A.GUP_CODE
                            AND D.CUST_CODE =A.CUST_CODE
                            AND D.ALLOCATION_NO = A.ALLOCATION_NO
							LEFT JOIN F1980 E 
                            ON A.DC_CODE = E.DC_CODE 
							{condition4}
                            WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p2
                            AND A.CUST_CODE = @p1
							AND ISNULL(A.SOURCE_TYPE, '17') <> '18'
                            AND ISNULL(A.SOURCE_TYPE, '17') = '04' 
                            {condition}
                            GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ALLOCATION_DATE, A.ALLOCATION_NO, D.RT_NO, A.SRC_WAREHOUSE_ID, A.TAR_WAREHOUSE_ID, A.STATUS, C.NAME, A.UPD_STAFF, A.UPD_NAME, A.BOX_NO, A.MEMO, F.WAREHOUSE_NAME, G.WAREHOUSE_NAME
                            ";

			var result = SqlQuery<GetAllocRes>(sql, parm.ToArray());
			return result;
		}

        /// <summary>
		/// 調撥單據查詢
		/// </summary>
		/// <param name="dcNo">物流中心編號</param>
		/// <param name="custNo">貨主編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="allocType">單據類別 02:調撥下架   03:調撥上架 </param>
		/// <param name="allocDate">調撥日期</param>
		/// <param name="wmsNo">調撥單號/驗收單號</param>
		/// <param name="itemNo">品號</param>
		/// <param name="palletNo">板號</param>
		/// <param name="allocationNos">調撥單號List</param>
		/// <returns></returns>
		public IQueryable<GetAllocRes> GetP810103Data(string dcNo, string custNo, string gupCode, string allocType, DateTime? allocDate, string wmsNo, string itemNo, string palletNo, string serialNo, List<string> allocationNos)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcNo));
            parm.Add(new SqlParameter("@p1", custNo));
            parm.Add(new SqlParameter("@p2", gupCode));
            parm.Add(new SqlParameter("@p3", allocType));

            #region 條件過濾
            string condition = " AND E.DEVICE_TYPE ='0'"; // 人工倉
            string condition2 = string.Empty;
            string condition3 = string.Empty;
            string condition4 = string.Empty;
            string condition5 = string.Empty;

            // 若 AllocDate is not null, 增加條件 F151001.allocation_date= AllocDate
            if (allocDate != null)
            {
                condition += " AND A.ALLOCATION_DATE = @p4 ";
                parm.Add(new SqlParameter("@p4", allocDate));
            }

            // 若 WmsNo is not null, 增加條件F151001.allocation_no= WmsNo
            if (allocationNos.Count() > 0)
            {
                condition += $" AND A.ALLOCATION_NO in ('{string.Join("','", allocationNos)}') ";
            }

            // 若AllocType in (03)，增加條件 F151001.status in (3,4) & F151002.status =1 (待上架)
            // 若AllocType in (02)，增加條件 F151001.status in (0, 2) & F151002.status = 0(待下架)
            if (allocType.Equals("03"))
            {
                condition += @" AND (A.STATUS = '3' OR A.STATUS = '4') 
                                AND NOT EXISTS (SELECT 1 FROM F151003 E 
                                WHERE E.DC_CODE = A.DC_CODE
                                AND E.GUP_CODE = A.GUP_CODE
                                AND E.CUST_CODE = A.CUST_CODE
                                AND E.ALLOCATION_NO = A.ALLOCATION_NO 
                                AND E.STATUS = 0)";

                condition2 += " AND X.STATUS = '1' ";
                condition4 += " AND A.TAR_WAREHOUSE_ID  = E.WAREHOUSE_ID "; // 進倉上架、調撥上架是JOIN來源倉別
            }
            else if (allocType.Equals("02"))
            {
                condition += " AND (A.STATUS = '0' OR A.STATUS = '2') ";
                condition2 += " AND X.STATUS = '0' ";
                condition4 += " AND A.SRC_WAREHOUSE_ID = E.WAREHOUSE_ID "; //調撥下架是JOIN來源倉別
            }

            // 若ItemNo is not null, 增加條件F151002.item_code = ItemNo
            if (!string.IsNullOrWhiteSpace(itemNo))
            {
                condition += $@" AND (
                                 SELECT COUNT(Z.ITEM_CODE)  
                                 FROM F151002 Z JOIN F1903 X ON Z.GUP_CODE =X.GUP_CODE and Z.CUST_CODE = X.CUST_CODE and Z.ITEM_CODE =X.ITEM_CODE 
                                 WHERE Z.DC_CODE = A.DC_CODE 
                                    AND Z.GUP_CODE = A.GUP_CODE 
                                    AND Z.CUST_CODE = A.CUST_CODE 
                                    AND Z.ALLOCATION_NO = A.ALLOCATION_NO 
                                    AND (X.ITEM_CODE = @p6 OR X.EAN_CODE1 = @p6 OR  X.EAN_CODE2 = @p6 OR X.EAN_CODE3 = @p6)
                                 ) > 0 ";

                parm.Add(new SqlParameter("@p6", itemNo));
            }

            // 若PalletNo is not null, 增加條件F151002.pallet_ctrl_no=PalletNo
            if (!string.IsNullOrWhiteSpace(palletNo))
            {
                condition += $@" AND (
                                 SELECT COUNT(Z.PALLET_CTRL_NO)  
                                 FROM F151002 Z
                                    WHERE Z.DC_CODE = A.DC_CODE 
                                    AND Z.GUP_CODE = A.GUP_CODE 
                                    AND Z.CUST_CODE = A.CUST_CODE 
                                    AND Z.ALLOCATION_NO = A.ALLOCATION_NO 
                                    AND Z.PALLET_CTRL_NO = @p7
                                 ) > 0";
                parm.Add(new SqlParameter("@p7", palletNo));
            }

            // 若SerialNo is not null, 增加條件F2501.SERIAL_NO=SerialNo
            if (!string.IsNullOrWhiteSpace(serialNo))
            {

                condition += $@"  AND A.ALLOCATION_NO IN  (
					  SELECT ALLOCATION_NO 
					  FROM F151002 R
					  WHERE (ITEM_CODE IN
					  (SELECT f.ITEM_CODE 
					     FROM F2501 f 
					     join F1903 g
					     on f.GUP_CODE =g.GUP_CODE 
					     AND f.CUST_CODE =g.CUST_CODE 
					     AND f.ITEM_CODE =g.ITEM_CODE 
					     WHERE f.GUP_CODE = R.GUP_CODE 
					     and f.CUST_CODE = R.CUST_CODE 
					     and f.SERIAL_NO  = @p8
					     and f.STATUS = 'A1'
					     AND g.BUNDLE_SERIALNO = '1'
					     AND g.BUNDLE_SERIALLOC ='0'
					   )
					   OR R.SERIAL_NO = @p8) )";
                parm.Add(new SqlParameter("@p8", serialNo));
            }
            #endregion

            string sql = $@"SELECT 
                            A.DC_CODE AS DcNo,
                            A.CUST_CODE AS CustNo,
                            A.ALLOCATION_DATE AS AllocDate,
                            A.ALLOCATION_NO AS AllocNo,
                            @p3 AS AllocType,
							F.WAREHOUSE_NAME AS SrcWhName,
							G.WAREHOUSE_NAME AS TarWhName,
                            COUNT(DISTINCT(B.ITEM_CODE)) AS ItemCnt,
                            ISNULL(SUM(CASE WHEN @p3='03' THEN B.TAR_QTY 
                            WHEN @p3='02' THEN B.SRC_QTY END), 0)  AS ItemQty,
                            ISNULL(COUNT(DISTINCT(B.ITEM_CODE)) - 
                            (SELECT COUNT(DISTINCT X.ITEM_CODE)  FROM F151002 X
                            WHERE X.ALLOCATION_NO = A.ALLOCATION_NO 
                            AND X.DC_CODE = A.DC_CODE 
                            AND X.GUP_CODE = A.GUP_CODE 
                            AND X.CUST_CODE = A.CUST_CODE 
                            {condition2})
                            , 0) AS ActItemCnt,
                            A.STATUS AS Status,
                            C.NAME AS StatusName,
                            ISNULL(A.UPD_STAFF, '') AS AccNo,
                            ISNULL(A.UPD_NAME, '') AS UserName,
                            A.BOX_NO AS BoxNo,
							A.MEMO AS Memo
                            FROM F151001 A 
                            JOIN F151002 B 
                            ON A.ALLOCATION_NO = B.ALLOCATION_NO 
                            AND A.DC_CODE = B.DC_CODE 
                            AND A.GUP_CODE = B.GUP_CODE 
                            AND A.CUST_CODE = B.CUST_CODE
                            JOIN VW_F000904_LANG C 
                            ON  C.VALUE = A.STATUS
                            AND C.TOPIC = 'F151001' 
                            AND C.SUBTOPIC = 'STATUS'
                            AND C.LANG = '{Current.Lang}'
							LEFT JOIN F1980 F
							ON F.DC_CODE = A.DC_CODE
							AND F.WAREHOUSE_ID = A.SRC_WAREHOUSE_ID
							LEFT JOIN F1980 G
							ON G.DC_CODE = A.DC_CODE
							AND G.WAREHOUSE_ID = A.TAR_WAREHOUSE_ID
							LEFT JOIN F1980 E 
                            ON A.DC_CODE = E.DC_CODE 
							{condition4}
                            WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p2
                            AND A.CUST_CODE = @p1
							AND ISNULL(A.SOURCE_TYPE, '17') <> '18'
                            AND ISNULL(A.SOURCE_TYPE, '17') <> '04'
                            {condition}
                            GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ALLOCATION_DATE, A.ALLOCATION_NO, A.SRC_WAREHOUSE_ID, A.TAR_WAREHOUSE_ID, A.STATUS, C.NAME, A.UPD_STAFF, A.UPD_NAME, A.BOX_NO, A.MEMO, F.WAREHOUSE_NAME, G.WAREHOUSE_NAME
                            ";

            var result = SqlQuery<GetAllocRes>(sql, parm.ToArray());
            return result;
        }

        /// <summary>
        /// 調撥單據更新
        /// </summary>
        /// <param name="dcNo">物流中心編號</param>
        /// <param name="custNo">貨主編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="allocNo">調撥單號</param>
        /// <param name="tarMoveStaff">上架人員編號</param>
        /// <param name="tarMoveName">上架人名</param>
        /// <param name="srcMoveStaff">下架人員編號</param>
        /// <param name="srcMoveName">下架人名</param>
        /// <param name="status">調撥單狀態</param>
        /// <param name="lockStatus">調撥Device鎖定狀態</param>
        public void PostAllocUpdate(string dcNo, string custNo, string gupCode, string allocNo, string tarMoveStaff, string tarMoveName, string srcMoveStaff, string srcMoveName, string status, string lockStatus)
        {
            string condition = string.Empty;
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcNo));
            parm.Add(new SqlParameter("@p1", custNo));
            parm.Add(new SqlParameter("@p2", gupCode));
            parm.Add(new SqlParameter("@p3", allocNo));
            parm.Add(new SqlParameter("@p4", status));
            parm.Add(new SqlParameter("@p5", lockStatus));
            parm.Add(new SqlParameter("@p6", Current.Staff));
            parm.Add(new SqlParameter("@p7", Current.StaffName));

            if (!string.IsNullOrWhiteSpace(tarMoveStaff) && !string.IsNullOrWhiteSpace(tarMoveName))
            {
                parm.Add(new SqlParameter("@p8", tarMoveStaff));
                parm.Add(new SqlParameter("@p9", tarMoveName));
                condition += " , TAR_MOVE_STAFF = @p8, TAR_MOVE_NAME = @p9 ";
            }

            if (!string.IsNullOrWhiteSpace(srcMoveStaff) && !string.IsNullOrWhiteSpace(srcMoveName))
            {
                parm.Add(new SqlParameter("@p10", srcMoveStaff));
                parm.Add(new SqlParameter("@p11", srcMoveName));
                condition += " , SRC_MOVE_STAFF = @p10, SRC_MOVE_NAME = @p11 ";
            }

            string sql = $@"
					UPDATE F151001
					   SET LOCK_STATUS = @p5 ,
                           STATUS = @p4,
                           UPD_STAFF = @p6,
						   UPD_DATE = dbo.GetSysDate(),
						   UPD_NAME = @p7
                        {condition}
					 WHERE DC_CODE = @p0 AND CUST_CODE = @p1 AND GUP_CODE = @p2 AND ALLOCATION_NO = @p3
					";

            ExecuteSqlCommand(sql, parm.ToArray());
        }

        public IQueryable<ProcessedData> GetProcessedData(string dcNo, string gupCode, string custNo, List<string> allocationNos)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcNo));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custNo));

            string sql = $@"
					        SELECT Z.BACK_ITEM_TYPE, Z.TAR_WAREHOUSE_ID, SUM(Z.TAR_QTY) TAR_QTY FROM (
                            	SELECT SUBSTRING(A.TAR_WAREHOUSE_ID, 1, 1) TAR_WAREHOUSE_ID, C.BACK_ITEM_TYPE, B.TAR_QTY FROM F151001 A
                            	JOIN F151002 B 
                            	ON A.DC_CODE = B.DC_CODE
                            	AND A.GUP_CODE = B.GUP_CODE
                            	AND A.CUST_CODE = B.CUST_CODE
                            	AND A.ALLOCATION_NO = B.ALLOCATION_NO
                            	JOIN F91020601 C
                            	ON A.DC_CODE = C.DC_CODE
                            	AND A.GUP_CODE = C.GUP_CODE
                            	AND A.CUST_CODE = C.CUST_CODE
                            	AND A.ALLOCATION_NO = C.ALLOCATION_NO
                            	WHERE A.DC_CODE = @p0
                            	AND A.GUP_CODE = @p1
                            	AND A.CUST_CODE = @p2
                            	AND A.ALLOCATION_NO IN ('{string.Join("','", allocationNos)}')
                            ) Z
                            GROUP BY Z.TAR_WAREHOUSE_ID, Z.BACK_ITEM_TYPE
					        ";

            var result = SqlQuery<ProcessedData>(sql, parm.ToArray());
            return result;
        }

        public void UpdateStatus(string dcCode, string gupCode, string custCode, string allocNo, string status)
        {
            var parameters = new List<object>
            {
                status,
                Current.Staff,
                Current.StaffName,
                dcCode,
                gupCode,
                custCode,
                allocNo
            };

            var sql = @"
				UPDATE  F151001  SET STATUS= @p0,
                                     UPD_STAFF = @p1,
						             UPD_DATE = dbo.GetSysDate(),
						             UPD_NAME = @p2
				 Where DC_CODE = @p3
                     And GUP_CODE = @p4
					 And CUST_CODE = @p5
					 And ALLOCATION_NO = @p6 ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

		public IQueryable<P1502010000Data> GetAllocationData( string srcDcCode, string tarDcCode, string gupCode, string custCode,
      DateTime crtDateS, DateTime crtDateE, DateTime? postingDateS, DateTime? postingDateE,
      string allocationNo, string status, string sourceNo, string userName, string containerCode, string allocationType)
    {
      var parm = new List<SqlParameter>()
      {
        new SqlParameter("@p0",srcDcCode),
        new SqlParameter("@p1",tarDcCode),
        new SqlParameter("@p2",gupCode),
        new SqlParameter("@p3",custCode),
        new SqlParameter("@p4",crtDateS),
        new SqlParameter("@p5",crtDateE.AddDays(1).AddSeconds(-1)),
      };
      var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ALLOCATION_NO) ROWNUM,
                                A.*, 
                                B.NAME ALLOCATION_TYPE_NAME,
                                A.PRE_TAR_WAREHOUSE_ID + ' ' + C.WAREHOUSE_NAME PRE_TAR_WAREHOUSE_NAME,
                                F.DC_NAME SRC_DC_NAME,
                                G.DC_NAME TAR_DC_NAME,
                                D.WAREHOUSE_ID + ' ' + D.WAREHOUSE_NAME SRC_WH_NAME,
                                D.DEVICE_TYPE SRC_WH_DEVICE_TYPE,
                                E.DEVICE_TYPE TAR_WH_DEVICE_TYPE,
                                E.WAREHOUSE_ID + ' ' + E.WAREHOUSE_NAME TAR_WH_NAME,
                                Z.NAME STATUS_NAME,
                                H.SOURCE_NAME SOURCE_TYPE_NAME
                        FROM F151001 A 
                        JOIN VW_F000904_LANG Z
                        ON A.STATUS = Z.VALUE
                        AND Z.TOPIC = 'F151001'
                        AND Z.SUBTOPIC = 'STATUS'
                        AND Z.LANG = '{Current.Lang}'
                        LEFT JOIN VW_F000904_LANG B
                        ON A.ALLOCATION_TYPE = B.VALUE
                        AND B.TOPIC = 'F151001'
                        AND B.SUBTOPIC = 'ALLOCATION_TYPE'
                        AND B.LANG = '{Current.Lang}'
                        LEFT JOIN F1980 C
                        ON A.PRE_TAR_WAREHOUSE_ID = C.WAREHOUSE_ID
                        AND A.DC_CODE = C.DC_CODE
                        LEFT JOIN F1980 D
                        ON A.SRC_WAREHOUSE_ID = D.WAREHOUSE_ID
                        AND A.SRC_DC_CODE = D.DC_CODE
                        LEFT JOIN F1980 E
                        ON A.TAR_WAREHOUSE_ID = E.WAREHOUSE_ID
                        AND A.TAR_DC_CODE = E.DC_CODE
                        LEFT JOIN F1901 F
                        ON A.SRC_DC_CODE = F.DC_CODE
                        LEFT JOIN F1901 G
                        ON A.TAR_DC_CODE = G.DC_CODE
                        LEFT JOIN F000902 H
                        ON A.SOURCE_TYPE = H.SOURCE_TYPE
                        WHERE A.SRC_DC_CODE = @p0 
						AND A.TAR_DC_CODE = @p1
						AND A.GUP_CODE = @p2
						AND A.CUST_CODE = @p3
						AND A.CRT_ALLOCATION_DATE >= @p4
						AND A.CRT_ALLOCATION_DATE <= @p5  ";

      if (postingDateS != null)
      {
        sql += " AND A.POSTING_DATE >= @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), postingDateS));

      }

      if (postingDateE != null)
      {
        sql += " AND A.POSTING_DATE <= @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), postingDateE?.AddDays(1).AddSeconds(-1)));

      }

      if (!string.IsNullOrWhiteSpace(allocationNo))
      {
        sql += " AND A.ALLOCATION_NO = @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), allocationNo));

      }

      if (!string.IsNullOrWhiteSpace(status))
      {
        sql += " AND A.STATUS = @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), status));

      }
      else
      {
        sql += " AND A.STATUS <> '9'";
      }

      if (!string.IsNullOrWhiteSpace(sourceNo))
      {
        sql += $" AND (A.SOURCE_NO = @p{parm.Count()} OR EXISTS (SELECT 1 FROM F151002 A1 WHERE A.DC_CODE=A1.DC_CODE AND A.GUP_CODE=A1.GUP_CODE AND A.CUST_CODE=A1.CUST_CODE AND A.ALLOCATION_NO=A1.ALLOCATION_NO AND A1.SOURCE_NO=@p{parm.Count()}))";
        parm.Add(new SqlParameter("@p" + parm.Count(), sourceNo));

      }

      if (!string.IsNullOrWhiteSpace(userName))
      {
        sql += $" AND (A.CRT_STAFF = @p{parm.Count()} OR A.UPD_STAFF = @p{parm.Count()})";
        parm.Add(new SqlParameter("@p" + parm.Count(), userName));
      }

      if (!string.IsNullOrWhiteSpace(containerCode))
      {
        sql += " AND A.CONTAINER_CODE = @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), containerCode));
      }

      if (!string.IsNullOrWhiteSpace(allocationType))
      {
        sql += " AND A.ALLOCATION_TYPE = @p" + parm.Count();
        parm.Add(new SqlParameter("@p" + parm.Count(), allocationType));
      }

      var result = SqlQuery<P1502010000Data>(sql, parm.ToArray());
      return result;
    }

    public IQueryable<P1502010500Data> GetP1502010500Data(string dcCode, string gupCode, string custCode, string allocationNo)
        {
            var para = new List<SqlParameter>() {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=allocationNo},
            };

            var sql = @"
SELECT
    a.DC_CODE,
    a.GUP_CODE,
    a.CUST_CODE,
    c.DC_NAME SRC_DC_CODE_NAME,
    d.WAREHOUSE_NAME SRC_WAREHOUSE_NAME,
    a.ALLOCATION_NO,
    b.ALLOCATION_SEQ,
    b.SRC_LOC_CODE,
    b.ITEM_CODE,
    e.ITEM_NAME,
    b.SRC_QTY,
    0 LACK_QTY,
    b.VALID_DATE,
    b.MAKE_NO,
    b.ENTER_DATE,
    b.SERIAL_NO,
    b.VNR_CODE
FROM
    F151001 a
    LEFT JOIN f151002 b ON a.DC_CODE = b.DC_CODE
      AND a.GUP_CODE = b.GUP_CODE
      AND a.CUST_CODE = b.CUST_CODE
      AND a.ALLOCATION_NO = b.ALLOCATION_NO
    LEFT JOIN F1901 c ON a.SRC_DC_CODE = c.DC_CODE
    LEFT JOIN F1980 d ON a.DC_CODE = d.DC_CODE
      AND a.SRC_WAREHOUSE_ID = d.WAREHOUSE_ID
    LEFT JOIN F1903 e ON a.GUP_CODE = e.GUP_CODE
      AND a.CUST_CODE = e.CUST_CODE
      AND b.ITEM_CODE = e.ITEM_CODE
WHERE
    a.DC_CODE = @p0
    AND a.GUP_CODE = @p1
    AND a.CUST_CODE = @p2
    AND a.ALLOCATION_NO = @p3
    AND b.STATUS='0'
";
            var result=SqlQuery<P1502010500Data>(sql,para.ToArray());
            return result;
        }
    
        public ContainerSingleByAlloc GetContainerSingleByAlloc(string dcCode, string gupCode, string custCode, string allocNo)
        {
            var parm = new List<SqlParameter>
            {
                new SqlParameter("@p0", Current.Lang),
                new SqlParameter("@p1", dcCode),
                new SqlParameter("@p2", gupCode),
                new SqlParameter("@p3", custCode),
                new SqlParameter("@p4", allocNo)
            };
            var sql = $@" SELECT 
                          TAR_WAREHOUSE_ID,
                          L.NAME STATUS_NAME 
                          FROM F151001 T 
                          LEFT JOIN VW_F000904_LANG L
                          ON L.TOPIC='F151001'
                            AND L.SUBTOPIC='STATUS'
                            AND L.LANG = @p0
                            AND L.VALUE = T.STATUS
                          WHERE T.DC_CODE = @p1
                            AND T.GUP_CODE = @p2
                            AND T.CUST_CODE = @p3
                            AND T.ALLOCATION_NO = @p4
                         ";

            return SqlQuery<ContainerSingleByAlloc>(sql, parm.ToArray()).FirstOrDefault();
        }
		     
		    public F151001 GetF151001(string dcCode,string gupCode,string custCode,string allocationNo)
				{
					var parms = new List<SqlParameter>
								{
										new SqlParameter("@p0", SqlDbType.VarChar){ Value = dcCode },
										new SqlParameter("@p1", SqlDbType.VarChar){Value = gupCode },
										new SqlParameter("@p2", SqlDbType.VarChar){ Value = custCode },
										new SqlParameter("@p3", SqlDbType.VarChar){ Value = allocationNo },
								};
					var sql = @" SELECT *
															FROM F151001 
														 WHERE DC_CODE = @p0
															 AND GUP_CODE = @p1
															 AND CUST_CODE = @p2
															 AND ALLOCATION_NO = @p3 ";
					return SqlQuery<F151001>(sql, parms.ToArray()).FirstOrDefault();
				}
    }
}
