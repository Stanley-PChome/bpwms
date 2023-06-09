
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F02020101Repository : RepositoryBase<F02020101, Wms3plDbContext, F02020101Repository>
	{
		/// <summary>
		/// 取得驗收單
		/// Memo: 因為一張進倉單一個商品會有多個驗收單, 所以不能共用上面的查詢 (GroupBy不同).
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		public IQueryable<P020203Data> FindEx(string dcCode, string gupCode, string custCode, string purchaseNo, string commCustCode
				, string rtNo = "", string vnrCode = "", string custOrdNo = "", string allocationNo = "", string vnrNameConditon = "", string startDt = "", string endDt = "")
		{

			var param = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode)
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
			string sqlAllocationNo = "";
			if (!string.IsNullOrWhiteSpace(allocationNo))
			{
				sqlAllocationNo = @" AND EXISTS (SELECT 1 FROM F02020108 E
								WHERE E.DC_CODE = A.DC_CODE
								AND E.GUP_CODE = A.GUP_CODE
								AND E.CUST_CODE = A.CUST_CODE
								AND E.RT_NO  = A.RT_NO
								AND E.ALLOCATION_NO = @p" + param.Count() + ")";
				param.Add(new SqlParameter("@p" + param.Count(), allocationNo));
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
								A.UPD_DATE,
								A.UPD_NAME,
								A.FAST_PASS_TYPE,
								A.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								A.VNR_NAME ,
								A.RECV_QTY,
								A.SUM_RECV_QTY,
								SUM(ISNULL(A.DEFECT_QTY,0)) DEFECT_QTY,
								SUM(A.ITEM_COUNT) ITEM_COUNT FROM 
								( SELECT
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.UPD_DATE,
								A.UPD_NAME,
								(SELECT NAME FROM f000904 WHERE f000904.TOPIC = 'F010201' AND SUBTOPIC = 'FAST_PASS_TYPE' AND VALUE = B.FAST_PASS_TYPE) FAST_PASS_TYPE,
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
								 WHERE     A.DC_CODE = @p0
								AND A.GUP_CODE = @p1
								AND A.CUST_CODE = @p2
								{0} {1} {2} {3} {4} {5} {6} {7}
								AND A.RT_MODE = '0'
								AND C.VNR_CODE IS NOT NULL
								GROUP BY 
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.UPD_DATE,
								A.UPD_NAME,
								B.FAST_PASS_TYPE,
								B.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								C.VNR_NAME,
								A.RT_MODE 
								) A
								GROUP BY 
								A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RECE_DATE,
								A.UPD_DATE,
								A.UPD_NAME,
								A.FAST_PASS_TYPE,
								A.CUST_ORD_NO,
								A.PURCHASE_NO,
								A.RT_NO,
								A.VNR_CODE,
								A.VNR_NAME,
								A.VNR_NAME,
								A.RECV_QTY,
								A.SUM_RECV_QTY								
								ORDER BY 1 ASC";
			//        string sql = @"
			//  SELECT ROW_NUMBER()OVER(ORDER BY A.RECE_DATE, A.RT_NO) AS ROW_NUM, A.*
			//		FROM (SELECT DISTINCT B.*
			//						FROM (SELECT DISTINCT PN.PURCHASE_NO,
			//																	PN.VNR_CODE,
			//																	PN.STATUS,
			//																	ISNULL (C.VNR_NAME, F.VNR_NAME) AS VNR_NAME,
			//																	PN.RT_NO,
			//																	PN.RECE_DATE,
			//																	PN.GUP_CODE,
			//																	PN.CUST_CODE,
			//																	PN.DC_CODE,
			//																	E.CUST_ORD_NO,
			//																	D.ALLOCATION_NO,
			//																	ISNULL(C.VNR_CODE,F.VNR_CODE) AS G_VNR_CODE
			//										FROM F02020101 PN
			//												 LEFT JOIN F1908 C
			//														ON     C.VNR_CODE = PN.VNR_CODE
			//															 AND C.GUP_CODE = PN.GUP_CODE
			//															 AND C.CUST_CODE = PN.CUST_CODE
			//												 LEFT JOIN F1908 F
			//														ON     F.VNR_CODE = PN.VNR_CODE
			//															 AND F.GUP_CODE = PN.GUP_CODE
			//															 AND F.CUST_CODE = '0'
			//												 JOIN F02020107 D
			//														ON     D.PURCHASE_NO = PN.PURCHASE_NO
			//															 AND D.DC_CODE = PN.DC_CODE
			//															 AND D.GUP_CODE = PN.GUP_CODE
			//															 AND D.CUST_CODE = PN.CUST_CODE
			//															 AND D.RT_NO = PN.RT_NO
			//												 JOIN F010201 E
			//														ON     E.DC_CODE = PN.DC_CODE
			//															 AND E.GUP_CODE = PN.GUP_CODE
			//															 AND E.CUST_CODE = PN.CUST_CODE
			//															 AND E.STOCK_NO = PN.PURCHASE_NO
			//									 WHERE     PN.DC_CODE = @p0
			//												 AND PN.GUP_CODE = @p1
			//												 AND PN.CUST_CODE = @p2
			//												 {0} {1} {2} {3} {4} {5} {6} {7}
			//									UNION ALL
			//									/* 從F020201找出已上傳檔案的資料 (只計算總量) */
			//									SELECT DISTINCT PN.PURCHASE_NO,
			//																	PN.VNR_CODE,
			//																	PN.STATUS,
			//																	ISNULL (C.VNR_NAME, F.VNR_NAME) AS VNR_NAME,
			//																	PN.RT_NO,
			//																	PN.RECE_DATE,
			//																	PN.GUP_CODE,
			//																	PN.CUST_CODE,
			//																	PN.DC_CODE,
			//																	E.CUST_ORD_NO,
			//																	D.ALLOCATION_NO,
			//																	ISNULL(C.VNR_CODE,F.VNR_CODE) AS G_VNR_CODE
			//										FROM F020201 PN
			//												 LEFT JOIN F1908 C
			//														ON     C.VNR_CODE = PN.VNR_CODE
			//															 AND C.GUP_CODE = PN.GUP_CODE
			//															 AND C.CUST_CODE = PN.CUST_CODE
			//												 LEFT JOIN F1908 F
			//														ON     F.VNR_CODE = PN.VNR_CODE
			//															 AND F.GUP_CODE = PN.GUP_CODE
			//															 AND F.CUST_CODE = '0'
			//												 JOIN F02020107 D
			//														ON     D.PURCHASE_NO = PN.PURCHASE_NO
			//															 AND D.DC_CODE = PN.DC_CODE
			//															 AND D.GUP_CODE = PN.GUP_CODE
			//															 AND D.CUST_CODE = PN.CUST_CODE
			//															 AND D.RT_NO = PN.RT_NO
			//												 JOIN F010201 E
			//														ON     E.DC_CODE = PN.DC_CODE
			//															 AND E.GUP_CODE = PN.GUP_CODE
			//															 AND E.CUST_CODE = PN.CUST_CODE
			//															 AND E.STOCK_NO = PN.PURCHASE_NO
			//									 WHERE     PN.DC_CODE = @p0
			//												 AND PN.GUP_CODE = @p1
			//												 AND PN.CUST_CODE = @p2
			//												 {0} {1} {2} {3} {4} {5} {6} {7}) B
			//					 WHERE B.G_VNR_CODE IS NOT NULL) A
			//ORDER BY 1 ASC";
			sql = string.Format(sql, sqlRtNo, sqlVnrCode, sqlStartDt, sqlEndDt, sqlPurchaseNo, sqlCustOrdNo, sqlAllocationNo, sqlVnrNameConditon);

			var result = SqlQuery<P020203Data>(sql, param.ToArray()).AsQueryable();
			return result;
		}
		public void Delete(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var parameter = new object[] { dcCode, gupCode, custCode, purchaseNo, rtNo };
			var sql = @" DELETE F02020101 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND PURCHASE_NO = @p3 AND RT_NO = @p4 ";
			ExecuteSqlCommand(sql, parameter);
		}

		public IQueryable<DcWmsNoStatusItem> GetReceProcessOver30MinDatasByDc(string dcCode, DateTime receDate)
		{
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY WMS_NO,STAFF,STAFF_NAME,START_DATE ASC)AS ROWNUM ,A.*
                     FROM (
										 SELECT A.PURCHASE_NO as WMS_NO,A.CRT_STAFF AS STAFF,A.CRT_STAFF + A.CRT_NAME AS STAFF_NAME,A.CRT_DATE AS START_DATE
											 FROM F02020101 A
											WHERE  A.DC_CODE = @p0
												AND A.RECE_DATE = @p1
												AND (DATEDIFF(MINUTE,dbo.GetSysDate(),A.CRT_DATE))>30
											GROUP BY A.PURCHASE_NO,A.CRT_STAFF,A.CRT_NAME,A.CRT_DATE ) A 
                                            ";
			var param = new object[] { dcCode, receDate.Date };
			return SqlQuery<DcWmsNoStatusItem>(sql, param);
		}

		/// <summary>
		/// 取得驗收暫存檔的虛擬商品驗收數等於進倉驗收數，且尚未刷讀序號(CHECK_SERIAL)的F02020101
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public IQueryable<F02020101> GetF02020101sByVirtualItem(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			// 故事情節發展日: 2015/08/12
			// 前提摘要:
			// 1.商品檢驗查詢後，會先產生02020101驗收暫存檔(若不存在的話)，等到驗收確認時，才會產生020201驗收檔，當上傳檔案後，就會先020201驗收暫存檔刪除。
			// 2.若要做驗收確認時，目前需要將商品的驗收數等於020302進倉驗收檔狀態為待匯入數量才行。

			// 故事開始:
			// 當從進倉驗收序號畫面匯入序號到F020302後，若是虛擬商品的話，由於不能刷讀序號，也不用做序號收集，
			// 故需要每次在商品檢驗畫面查詢進倉單時，從驗收暫存檔F02020101判斷是否已經在F020302匯入剛好的序號，
			// 接著需要將該虛擬商品的已刷讀序號(CHECK_SERIAL)欄位設為 1。

			var sql = @"SELECT E.*
						  FROM (SELECT A.*,
									   (SELECT COUNT (1)
										  FROM F020302 D
										 WHERE     C.SHOP_NO = D.PO_NO
											   AND C.DC_CODE = D.DC_CODE
											   AND C.GUP_CODE = D.GUP_CODE
											   AND C.CUST_CODE = D.CUST_CODE
											   AND A.ITEM_CODE = D.ITEM_CODE
											   AND D.STATUS = '0')
										  AS SERIAL_QTY
								  FROM F02020101 A
									   JOIN F1903 B
										  ON A.ITEM_CODE = B.ITEM_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
									   JOIN F010201 C
										  ON     A.PURCHASE_NO = C.STOCK_NO
											 AND A.DC_CODE = C.DC_CODE
											 AND A.GUP_CODE = C.GUP_CODE
											 AND A.CUST_CODE = C.CUST_CODE
								 WHERE     A.PURCHASE_NO = @p0
									   AND A.RT_NO = @p1
									   AND A.DC_CODE = @p2
									   AND A.GUP_CODE = @p3
									   AND A.CUST_CODE = @p4
									   AND B.VIRTUAL_TYPE IS NOT NULL
                                       AND B.VIRTUAL_TYPE <> ''
									   AND A.CHECK_SERIAL = '0') E
						 WHERE E.RECV_QTY = E.SERIAL_QTY                             -- 驗收數 = 進倉驗收檔數量時";
			var result = SqlQuery<F02020101>(sql, new object[] { purchaseNo, rtNo, dcCode, gupCode, custCode });
			return result;
		}
		/// <summary>
		/// 是否驗收數(F02020101) != 進倉驗收檔(F020302)數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public bool IsRecvQtyNotEqualsSerialTotal(string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var sql = @"SELECT TOP(1) 1
					  FROM (SELECT A.*,
								   (SELECT COUNT (1)
									  FROM F020302 D
									 WHERE     C.SHOP_NO = D.PO_NO
										   AND C.DC_CODE = D.DC_CODE
										   AND C.GUP_CODE = D.GUP_CODE
										   AND C.CUST_CODE = D.CUST_CODE
										   AND A.ITEM_CODE = D.ITEM_CODE
										   AND D.STATUS = '0')
									  AS SERIAL_QTY
							  FROM F02020101 A
								   JOIN F1903 B
									  ON     A.ITEM_CODE = B.ITEM_CODE
										 AND A.GUP_CODE = B.GUP_CODE
										 AND A.CUST_CODE = B.CUST_CODE
								   JOIN F010201 C
									  ON     A.PURCHASE_NO = C.STOCK_NO
										 AND A.DC_CODE = C.DC_CODE
										 AND A.GUP_CODE = C.GUP_CODE
										 AND A.CUST_CODE = C.CUST_CODE
							 WHERE     A.PURCHASE_NO = @p0
								   AND A.DC_CODE = @p1
								   AND A.GUP_CODE = @p2
								   AND A.CUST_CODE = @p3
								   AND A.ISSPECIAL = '0'						-- 驗收項目不用包含特採的驗收數
								   AND B.BUNDLE_SERIALNO = '1'
								   AND C.SOURCE_TYPE <> '09') E					-- 內部交易不用做驗收數與進倉驗收數檢核
					 WHERE E.RECV_QTY <> E.SERIAL_QTY                           -- 驗收數 != 進倉驗收檔數量時
													 --AND ROWNUM = 1";

			var parameter = new object[] { purchaseNo, dcCode, gupCode, custCode };
			return SqlQuery<int>(sql, parameter).Any();
		}
		public IQueryable<P020203Data> GetP020203Datas(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var parms = new List<SqlParameter> {
				new SqlParameter("@p0",dcCode) {SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode) {SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p2",custCode) {SqlDbType = System.Data.SqlDbType.VarChar} ,
			  new SqlParameter("@p3", purchaseNo) { SqlDbType = System.Data.SqlDbType.VarChar } };
		   string sql;

			if(!string.IsNullOrEmpty(rtNo))
			{
				parms.Add(new SqlParameter("@p4", rtNo) {SqlDbType = System.Data.SqlDbType.VarChar });
				sql = @" 
							SELECT ROW_NUMBER() OVER(ORDER BY A.PURCHASE_SEQ) ROWNUM,
										 A.*,B.SHOP_NO,B.CUST_ORD_NO,ISNULL(H.TOTAL_REC_QTY, 0) SUM_RECV_QTY,
										 ISNULL(I.DEFECT_QTY,0) DEFECT_QTY, C.VNR_NAME,F.CLA_NAME,
									 '0' IsNotNeedCheckScan,ISNULL(J.SERIAL_COUNT, 0) SERIAL_COUNT,
									 G.PACK_HIGHT,G.PACK_LENGTH,G.PACK_WIDTH,G.PACK_WEIGHT,D.IS_QUICK_CHECK,
									 CASE WHEN ISNULL(H.TOTAL_REC_QTY,0)>0 THEN '1' ELSE '0' END HasRecvData,
									 E.ITEM_NAME,E.BUNDLE_SERIALNO,E.ITEM_COLOR,E.ITEM_SIZE,E.ITEM_SPEC,
									 E.VIRTUAL_TYPE,E.ISOEM,E.EAN_CODE1,E.EAN_CODE2,E.EAN_CODE3,E.EAN_CODE4,
									 E.NEED_EXPIRED,E.ALL_SHP,E.ALL_DLN,E.FIRST_IN_DATE,E.SAVE_DAY,E.IS_EASY_LOSE,
									 E.IS_PRECIOUS,E.IS_MAGNETIC,E.FRAGILE,E.SPILL,E.IS_PERISHABLE,E.TMPR_TYPE,
									 E.IS_TEMP_CONTROL 
							FROM 
							(SELECT DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,RT_SEQ,ITEM_CODE,
							ORDER_QTY,RECV_QTY,CHECK_QTY,CHECK_SERIAL,CHECK_ITEM,ISPRINT,ISUPLOAD,STATUS,VNR_CODE,
							ISSPECIAL,SPECIAL_DESC,SPECIAL_CODE,RECE_DATE,VALI_DATE,MAKE_NO,TARWAREHOUSE_ID,
							RT_NO REAL_RT_NO,RT_SEQ REAL_RT_SEQ
							FROM F02020101 
							UNION ALL
							SELECT DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,RT_SEQ,ITEM_CODE,
							ORDER_QTY,RECV_QTY,CHECK_QTY,CHECK_SERIAL,CHECK_ITEM,ISPRINT,ISUPLOAD,STATUS,VNR_CODE,
							ISSPECIAL,SPECIAL_DESC,SPECIAL_CODE,RECE_DATE,VALI_DATE,MAKE_NO,TARWAREHOUSE_ID,
							RT_NO REAL_RT_NO,RT_SEQ REAL_RT_SEQ
							FROM F020201
							WHERE STATUS ='3') A
							JOIN F010201 B
							ON B.DC_CODE = A.DC_CODE
							AND B.GUP_CODE = A.GUP_CODE
							AND B.CUST_CODE = A.CUST_CODE
							AND B.STOCK_NO = A.PURCHASE_NO
							JOIN F1908 C
							ON C.GUP_CODE = A.GUP_CODE
							AND C.CUST_CODE = A.CUST_CODE
							AND C.VNR_CODE = A.VNR_CODE
							JOIN F1909 D
							ON D.GUP_CODE = A.GUP_CODE
							AND D.CUST_CODE = A.CUST_CODE
							JOIN F1903 E
							ON E.GUP_CODE = A.GUP_CODE
							AND E.CUST_CODE = A.CUST_CODE
							AND E.ITEM_CODE = A.ITEM_CODE
							LEFT JOIN F1915 F 
							ON F.GUP_CODE = E.GUP_CODE
							AND F.CUST_CODE = E.CUST_CODE
							AND F.ACODE = E.LTYPE
							LEFT JOIN F1905 G
							ON G.GUP_CODE = A.GUP_CODE
							AND G.CUST_CODE = A.CUST_CODE
							AND G.ITEM_CODE = A.ITEM_CODE
							LEFT JOIN F010204 H
							ON H.DC_CODE = A.DC_CODE
							AND H.GUP_CODE = A.GUP_CODE
							AND H.CUST_CODE = A.CUST_CODE
							AND H.STOCK_NO = A.PURCHASE_NO
							AND H.STOCK_SEQ = A.PURCHASE_SEQ
							LEFT JOIN 
							(SELECT DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,STOCK_SEQ,RT_NO,RT_SEQ,SUM(DEFECT_QTY) DEFECT_QTY
							FROM F02020109 
							GROUP BY DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,STOCK_SEQ,RT_NO,RT_SEQ) I
							ON I.DC_CODE = A.DC_CODE
							AND I.GUP_CODE = A.GUP_CODE
							AND I.CUST_CODE = A.CUST_CODE
							AND I.STOCK_NO = A.PURCHASE_NO
							AND I.STOCK_SEQ = A.PURCHASE_SEQ
							AND ISNULL(I.RT_NO,'0') = CASE WHEN A.STATUS=0 THEN '0' ELSE A.RT_NO END
							AND ISNULL(I.RT_SEQ,'0') = CASE WHEN A.STATUS=0 THEN '0' ELSE A.RT_SEQ END
							LEFT JOIN (
							SELECT DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,COUNT(DISTINCT SERIAL_NO) SERIAL_COUNT
							FROM F02020104
							WHERE ISPASS='1'
							GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO
							) J ON
							J.DC_CODE = A.DC_CODE
							AND J.GUP_CODE = A.GUP_CODE
							AND J.CUST_CODE = A.CUST_CODE
							AND J.PURCHASE_NO = A.PURCHASE_NO
							AND J.PURCHASE_SEQ = A.PURCHASE_SEQ
							AND J.RT_NO = A.RT_NO
							WHERE A.DC_CODE =  @p0
							AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2
							AND A.PURCHASE_NO= @p3
							AND A.RT_NO = @p4
							";
			}
			else
			{
				sql = @" 
							SELECT ROW_NUMBER() OVER(ORDER BY A.PURCHASE_SEQ) ROWNUM,
										 A.*,B.SHOP_NO,B.CUST_ORD_NO,ISNULL(H.TOTAL_REC_QTY, 0) SUM_RECV_QTY,
										 ISNULL(I.DEFECT_QTY,0) DEFECT_QTY, C.VNR_NAME,F.CLA_NAME,
									 '0' IsNotNeedCheckScan,ISNULL(J.SERIAL_COUNT, 0) SERIAL_COUNT,
									 G.PACK_HIGHT,G.PACK_LENGTH,G.PACK_WIDTH,G.PACK_WEIGHT,D.IS_QUICK_CHECK,
									 CASE WHEN ISNULL(H.TOTAL_REC_QTY,0)>0 THEN '1' ELSE '0' END HasRecvData,
									 E.ITEM_NAME,E.BUNDLE_SERIALNO,E.ITEM_COLOR,E.ITEM_SIZE,E.ITEM_SPEC,
									 E.VIRTUAL_TYPE,E.ISOEM,E.EAN_CODE1,E.EAN_CODE2,E.EAN_CODE3,E.EAN_CODE4,
									 E.NEED_EXPIRED,E.ALL_SHP,E.ALL_DLN,E.FIRST_IN_DATE,E.SAVE_DAY,E.IS_EASY_LOSE,
									 E.IS_PRECIOUS,E.IS_MAGNETIC,E.FRAGILE,E.SPILL,E.IS_PERISHABLE,E.TMPR_TYPE,
									 E.IS_TEMP_CONTROL 
							FROM 
							(
							SELECT DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,RT_SEQ,ITEM_CODE,
							ORDER_QTY,RECV_QTY,CHECK_QTY,CHECK_SERIAL,CHECK_ITEM,ISPRINT,ISUPLOAD,STATUS,VNR_CODE,
							ISSPECIAL,SPECIAL_DESC,SPECIAL_CODE,RECE_DATE,VALI_DATE,MAKE_NO,TARWAREHOUSE_ID,
							RT_NO REAL_RT_NO,RT_SEQ REAL_RT_SEQ
							FROM F020201) A
							JOIN F010201 B
							ON B.DC_CODE = A.DC_CODE
							AND B.GUP_CODE = A.GUP_CODE
							AND B.CUST_CODE = A.CUST_CODE
							AND B.STOCK_NO = A.PURCHASE_NO
							JOIN F1908 C
							ON C.GUP_CODE = A.GUP_CODE
							AND C.CUST_CODE = A.CUST_CODE
							AND C.VNR_CODE = A.VNR_CODE
							JOIN F1909 D
							ON D.GUP_CODE = A.GUP_CODE
							AND D.CUST_CODE = A.CUST_CODE
							JOIN F1903 E
							ON E.GUP_CODE = A.GUP_CODE
							AND E.CUST_CODE = A.CUST_CODE
							AND E.ITEM_CODE = A.ITEM_CODE
							LEFT JOIN F1915 F 
							ON F.GUP_CODE = E.GUP_CODE
							AND F.CUST_CODE = E.CUST_CODE
							AND F.ACODE = E.LTYPE
							LEFT JOIN F1905 G
							ON G.GUP_CODE = A.GUP_CODE
							AND G.CUST_CODE = A.CUST_CODE
							AND G.ITEM_CODE = A.ITEM_CODE
							LEFT JOIN F010204 H
							ON H.DC_CODE = A.DC_CODE
							AND H.GUP_CODE = A.GUP_CODE
							AND H.CUST_CODE = A.CUST_CODE
							AND H.STOCK_NO = A.PURCHASE_NO
							AND H.STOCK_SEQ = A.PURCHASE_SEQ
							LEFT JOIN 
							(SELECT DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,STOCK_SEQ,RT_NO,RT_SEQ,SUM(DEFECT_QTY) DEFECT_QTY
							FROM F02020109 
							GROUP BY DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,STOCK_SEQ,RT_NO,RT_SEQ) I
							ON I.DC_CODE = A.DC_CODE
							AND I.GUP_CODE = A.GUP_CODE
							AND I.CUST_CODE = A.CUST_CODE
							AND I.STOCK_NO = A.PURCHASE_NO
							AND I.STOCK_SEQ = A.PURCHASE_SEQ
							AND I.RT_NO =  A.RT_NO
							AND I.RT_SEQ = A.RT_SEQ
							LEFT JOIN (
							SELECT DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,COUNT(DISTINCT SERIAL_NO) SERIAL_COUNT
							FROM F02020104
							WHERE ISPASS='1'
							GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO
							) J ON
							J.DC_CODE = A.DC_CODE
							AND J.GUP_CODE = A.GUP_CODE
							AND J.CUST_CODE = A.CUST_CODE
							AND J.PURCHASE_NO = A.PURCHASE_NO
							AND J.PURCHASE_SEQ = A.PURCHASE_SEQ
							AND J.RT_NO = A.RT_NO
							WHERE A.DC_CODE =  @p0
							AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2
							AND A.PURCHASE_NO= @p3
							";
			}
			var result = SqlQuery<P020203Data>(sql, parms.ToArray());
			return result;
			
		}

		public P020203Data GetDatasByFinishedRecv(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo,string stockSeq)
		{
			var parms = new List<SqlParameter> {
				new SqlParameter("@p0", dcCode){ SqlDbType = System.Data.SqlDbType.VarChar  },
				new SqlParameter("@p1", gupCode){ SqlDbType = System.Data.SqlDbType.VarChar  },
				new SqlParameter("@p2", custCode){ SqlDbType = System.Data.SqlDbType.VarChar  },
				new SqlParameter("@p3", purchaseNo){ SqlDbType = System.Data.SqlDbType.VarChar  },
				new SqlParameter("@p4", stockSeq){ SqlDbType = System.Data.SqlDbType.VarChar  },
				new SqlParameter("@p5", rtNo){ SqlDbType = System.Data.SqlDbType.VarChar  },
			 };
			var sql = @"SELECT TOP 1
    ROW_NUMBER() OVER(
        ORDER BY
            A.PURCHASE_SEQ
    ) ROWNUM,
     A.*
FROM
    (
		SELECT A.*,ISNULL(B.TOTAL_REC_QTY, 0) SUM_RECV_QTY,C.ITEM_NAME,C.BUNDLE_SERIALNO,
		D.VNR_NAME,E.CLA_NAME,
		C.ITEM_COLOR,C.ITEM_SIZE,C.ITEM_SPEC,C.ISOEM,C.EAN_CODE1,C.EAN_CODE2,C.EAN_CODE3,C.EAN_CODE4,
		C.NEED_EXPIRED,C.ALL_SHP,C.ALL_DLN,C.FIRST_IN_DATE,C.SAVE_DAY,C.IS_EASY_LOSE,C.IS_PRECIOUS,
		C.IS_MAGNETIC,C.FRAGILE,C.SPILL,C.IS_PERISHABLE,C.TMPR_TYPE,C.IS_TEMP_CONTROL,C.VIRTUAL_TYPE,
		ISNULL(B.TOTAL_DEFECT_RECV_QTY, 0) DEFECT_QTY,'0' IsNotNeedCheckScan,
		ISNULL(H.SERIAL_COUNT, 0) SERIAL_COUNT,
		I.PACK_HIGHT,I.PACK_LENGTH,I.PACK_WIDTH,I.PACK_WEIGHT,
		CASE  WHEN ISNULL(K.RECVCOUNT, 0) > 0 THEN '1'
										ELSE '0'
								END HasRecvData,
		N.IS_QUICK_CHECK
		FROM 
		(SELECT B.PURCHASE_NO,B.PURCHASE_SEQ,B.ITEM_CODE,B.ORDER_QTY,case when B.ORDER_QTY-SUM(B.RECV_QTY) >=0 then SUM(B.RECV_QTY) else 0 end RECV_QTY,
												B.CHECK_QTY,B.CHECK_SERIAL,B.CHECK_ITEM,B.ISPRINT,B.ISUPLOAD,B.STATUS,B.VNR_CODE,
												B.ISSPECIAL,B.SPECIAL_DESC,B.SPECIAL_CODE,B.RT_NO,B.RT_SEQ,B.RECE_DATE,B.DC_CODE,B.GUP_CODE,
												B.CUST_CODE,B.VALI_DATE,B.MAKE_NO,B.TARWAREHOUSE_ID,C.SHOP_NO,C.CUST_ORD_NO,B.RT_NO REAL_RT_NO,B.RT_SEQ REAL_RT_SEQ
										FROM F020201 B
										JOIN F010201 C
										ON C.DC_CODE = B.DC_CODE
										AND C.GUP_CODE = B.GUP_CODE
										AND C.CUST_CODE = B.CUST_CODE
										AND C.STOCK_NO = B.PURCHASE_NO
										GROUP BY B.PURCHASE_NO,B.PURCHASE_SEQ,B.ITEM_CODE,B.ORDER_QTY,B.RT_NO,B.RT_SEQ,
												B.CHECK_QTY,B.CHECK_SERIAL,B.CHECK_ITEM,B.ISPRINT,B.ISUPLOAD,B.STATUS,B.VNR_CODE,
												B.ISSPECIAL,B.SPECIAL_DESC,B.SPECIAL_CODE,B.RECE_DATE,B.DC_CODE,B.GUP_CODE,
												B.CUST_CODE,B.VALI_DATE,B.MAKE_NO,B.TARWAREHOUSE_ID,C.SHOP_NO,C.CUST_ORD_NO) A
		LEFT JOIN F010204 B
		ON B.DC_CODE = A.DC_CODE 
		AND B.GUP_CODE = A.GUP_CODE
		AND B.CUST_CODE = A.CUST_CODE
		AND B.STOCK_NO = A.PURCHASE_NO
		AND B.STOCK_SEQ = A.PURCHASE_SEQ
		LEFT JOIN F1903 C ON C.GUP_CODE = A.GUP_CODE
		AND C.CUST_CODE = A.CUST_CODE
		AND C.ITEM_CODE = A.ITEM_CODE
		LEFT JOIN F1908 D ON D.GUP_CODE = A.GUP_CODE
		AND D.CUST_CODE = A.CUST_CODE
		AND D.VNR_CODE = A.VNR_CODE
		LEFT JOIN F1915 E ON E.GUP_CODE = C.GUP_CODE
		AND E.CUST_CODE = C.CUST_CODE
		AND E.ACODE = C.LTYPE
		LEFT JOIN (
				SELECT
						DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,PURCHASE_SEQ,RT_NO,COUNT(DISTINCT SERIAL_NO) SERIAL_COUNT
				FROM
						F02020104
				WHERE
						ISPASS = '1'
				GROUP BY
						DC_CODE,
						GUP_CODE,
						CUST_CODE,
						PURCHASE_NO,
						PURCHASE_SEQ,
						RT_NO
		) H ON H.DC_CODE = A.DC_CODE
		AND H.GUP_CODE = A.GUP_CODE
		AND H.CUST_CODE = A.CUST_CODE
		AND H.PURCHASE_NO = A.PURCHASE_NO
		AND H.PURCHASE_SEQ = A.PURCHASE_SEQ
		AND H.RT_NO = A.RT_NO
		LEFT JOIN F1905 I ON I.GUP_CODE = A.GUP_CODE
		AND I.ITEM_CODE = A.ITEM_CODE
		AND I.CUST_CODE = A.CUST_CODE
		LEFT JOIN (
				SELECT
						DC_CODE,GUP_CODE,CUST_CODE,PURCHASE_NO,COUNT(*) RECVCOUNT
				FROM
						F020201
				GROUP BY
						DC_CODE,
						GUP_CODE,
						CUST_CODE,
						PURCHASE_NO
		) K ON K.DC_CODE = A.DC_CODE
		AND K.GUP_CODE = A.GUP_CODE
		AND K.CUST_CODE = A.CUST_CODE
		AND K.PURCHASE_NO = A.PURCHASE_NO
		LEFT JOIN F1909 N ON A.GUP_CODE = N.GUP_CODE
		AND A.CUST_CODE = N.CUST_CODE 
    ) A
   WHERE A.DC_CODE = @p0
		AND A.GUP_CODE = @p1
		AND A.CUST_CODE= @p2
		AND A.PURCHASE_NO = @p3
		AND A.PURCHASE_SEQ = @p4
		AND (A.RT_NO = @p5 OR  A.RT_NO IS NULL)
		";
			return SqlQuery<P020203Data>(sql, parms.ToArray()).FirstOrDefault();
		}
	}
}
