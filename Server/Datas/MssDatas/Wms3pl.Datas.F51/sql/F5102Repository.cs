using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F5102Repository : RepositoryBase<F5102, Wms3plDbContext, F5102Repository>
	{
		public IQueryable<F5102> GetSettleLocQty(string dcCode,string gupCode, string custCode, DateTime settleDate,string contractNo)
		{
			var sql = @"
					SELECT @p3 CAL_DATE, B.DC_CODE,@p1 GUP_CODE,B.CUST_CODE, B.LOC_TYPE_ID ,D.TMPR_TYPE,
								 COUNT(1) LOC_QTY, 0 LOC_AMT,@p4 CRT_STAFF,@p5 CRT_NAME,dbo.GetSysDate() CRT_DATE,'' UPD_STAFF,''UPD_NAME,NULL UPD_DATE,
								 '01' ACC_ITEM_KIND_ID,'01' DELV_ACC_TYPE,@p6 CONTRACT_NO,'' QUOTE_NO
						FROM F1912 B
						JOIN F1980 D ON B.DC_CODE = D.DC_CODE AND B.WAREHOUSE_ID = D.WAREHOUSE_ID
					 WHERE (D.DC_CODE = @p0 OR @p0 = '000') AND  B.CUST_CODE = @p2 AND D.CAL_FEE = '1' 
						 AND (B.RENT_BEGIN_DATE < @p3 OR B.RENT_BEGIN_DATE IS NULL) 
						 AND (B.RENT_END_DATE > @p3 OR B.RENT_END_DATE IS NULL)
					 GROUP BY B.DC_CODE,B.CUST_CODE, B.LOC_TYPE_ID ,D.TMPR_TYPE ";
			var param = new SqlParameter[]
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", settleDate),
				new SqlParameter("@p4", Current.Staff),
				new SqlParameter("@p5", Current.StaffName),
				new SqlParameter("@p6", contractNo),
			};
			return SqlQuery<F5102>(sql, param);
		}

		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5102 WHERE CAL_DATE = @p0 ";
			var param = new SqlParameter[]
			{
				new SqlParameter("@p0", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}

		public IQueryable<F51ComplexReportData> GetF51ComplexReportData(string dcCode, DateTime? calSDate, DateTime? calEDate,
			string gupCode, string custCode, string allId)
		{
			var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY A.CAL_DATE,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   A.CHECK_ACCOUNT_TYPE)ROWNUM,
							   A.CAL_DATE,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   A.CHECK_ACCOUNT_TYPE,
							   A.ACC_ITEM_KIND_ID,
							   A.DELV_ACC_TYPE,
							   A.ITEM_TYPE_ID,
							   A.ACC_ITEM_NAME,
							   A.WMS_NO,
							   A.ITEM_CODE,
							   A.QTY,
							   A.AMT,
							   A.LOC_TYPE_ID,
							   A.TMPR_TYPE,
							   A.DELV_DATE,
							   A.PAST_NO,
							   A.INVOICE_CNT,
							   A.SA_QTY,
							   (CASE
								   WHEN A.PACKAGE_BOX_NO IS NOT NULL
								   THEN
									  A.PACKAGE_BOX_NO
								   ELSE
									  (SELECT LTRIM (COUNT (*))
										 FROM F055001 P
										WHERE     A.DC_CODE = P.DC_CODE
											  AND A.GUP_CODE = P.GUP_CODE
											  AND A.CUST_CODE = P.CUST_CODE
											  AND A.WMS_NO = P.WMS_ORD_NO
											  AND P.PAST_NO = ISNULL (A.PAST_NO, P.PAST_NO))
								END)
								  AS PACKAGE_BOX_NO,
							   A.ITEM_CODE_BOM,
							   A.PROCESS_ID,
							   ISNULL (A.TAKE_TIME, N.TAKE_TIME) AS TAKE_TIME,
							   ISNULL (A.DISTR_CAR_NO, N.DISTR_CAR_NO) AS DISTR_CAR_NO,
							   ISNULL (A.VOLUMN, N.VOLUMN) AS VOLUMN,
							   ISNULL (A.WEIGHT, N.WEIGHT) AS WEIGHT,
							   ISNULL (A.ZIP_CODE, N.ZIP_CODE) AS ZIP_CODE,
							   ISNULL (A.DELV_TMPR, N.DELV_TMPR) AS DELV_TMPR,
							   ISNULL (A.CAN_FAST, N.CAN_FAST) AS CAN_FAST,
							   ISNULL (A.DISTR_USE, N.DISTR_USE) AS DISTR_USE,
							   ISNULL (A.SP_CAR, N.SP_CAR) AS SP_CAR,
							   ISNULL (A.ALL_ID, N.ALL_ID) AS ALL_ID,
							   B.NAME AS CHECK_ACCOUNT_TYPE_NAME,
							   C.DC_NAME,
							   D.GUP_NAME,
							   E.CUST_NAME,
							   F.LOC_TYPE_NAME,
							   G.NAME AS TMPR_TYPE_NAME,
							   I.NAME AS DELV_ACC_TYPE_NAME,
							   J.ITEM_NAME,
							   K.ITEM_NAME AS ITEM_NAME_BOM,
							   ISNULL (L.ALL_COMP, N.ALL_COMP) AS ALL_COMP,
							   ISNULL (M.NAME, N.DELV_TMPR_NAME) AS DELV_TMPR_NAME,
							   ISNULL (
								  CASE A.DISTR_USE
									 WHEN '01' THEN '送件'
									 WHEN '02' THEN '取件'
								  END,
								  N.DISTR_USE_NAME)
								  AS DISTR_USE_NAME
						  FROM (SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'A' CHECK_ACCOUNT_TYPE,
									   ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '002' ITEM_TYPE_ID,
									   (SELECT TOP(1) H.ACC_ITEM_NAME
										  FROM F500101 H
										 WHERE     H.DC_CODE = F5102.DC_CODE
											   AND H.GUP_CODE = F5102.GUP_CODE
											   AND H.CUST_CODE = F5102.CUST_CODE
											   AND H.QUOTE_NO = F5102.QUOTE_NO
											   --AND ROWNUM = 1
											   )
										  ACC_ITEM_NAME,
									   CAST ('' AS VARCHAR(20)) WMS_NO,
									   CAST ('' AS VARCHAR(20)) ITEM_CODE,
									   LTRIM(LOC_QTY) QTY,
									   LTRIM(LOC_AMT) AMT,
									   LOC_TYPE_ID,
									   TMPR_TYPE,
									   LTRIM ('') AS DELV_DATE,
									   CAST ('' AS VARCHAR(20)) PAST_NO,
									   LTRIM('') INVOICE_CNT,
									   LTRIM('') SA_QTY,
									   LTRIM('') PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR(20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR(20)) PROCESS_ID,
									   CAST ('' AS VARCHAR(5)) TAKE_TIME,
									   CAST ('' AS VARCHAR(20)) DISTR_CAR_NO,
									   LTRIM('') VOLUMN,
									   LTRIM('') WEIGHT,
									   CAST ('' AS VARCHAR(5)) ZIP_CODE,
									   CAST ('' AS VARCHAR(5)) DELV_TMPR,
									   CAST ('' AS VARCHAR(1)) CAN_FAST,
									   CAST ('' AS VARCHAR(2)) DISTR_USE,
									   CAST ('' AS VARCHAR(1)) SP_CAR,
									   CAST ('' AS VARCHAR(10)) ALL_ID
								  FROM F5102
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'B' CHECK_ACCOUNT_TYPE,
									   ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '004' ITEM_TYPE_ID,
									   (SELECT TOP(1)H.ACC_ITEM_NAME
										  FROM F500103 H
										 WHERE     H.DC_CODE = F5103.DC_CODE
											   AND H.GUP_CODE = F5103.GUP_CODE
											   AND H.CUST_CODE = F5103.CUST_CODE
											   AND H.QUOTE_NO = F5103.QUOTE_NO
											   --AND ROWNUM = 1
											   )
										  ACC_ITEM_NAME,
									   WMS_NO,
									   ITEM_CODE,
									   LTRIM(QTY) QTY,
									   LTRIM(AMT) AMT,
									   CAST ('' AS VARCHAR(4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR(2)) TMPR_TYPE,
									   CONVERT(varchar,DELV_DATE,111) AS DELV_DATE,
									   PAST_NO,
									   LTRIM(INVOICE_CNT) INVOICE_CNT,
									   LTRIM(SA_QTY) SA_QTY,
									   LTRIM('') PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR(20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR(20)) PROCESS_ID,
									   CAST ('' AS VARCHAR(5)) TAKE_TIME,
									   CAST ('' AS VARCHAR(20)) DISTR_CAR_NO,
									   LTRIM('') VOLUMN,
									   LTRIM('') WEIGHT,
									   CAST ('' AS VARCHAR(5)) ZIP_CODE,
									   CAST ('' AS VARCHAR(5)) DELV_TMPR,
									   CAST ('' AS VARCHAR(1)) CAN_FAST,
									   CAST ('' AS VARCHAR(2)) DISTR_USE,
									   CAST ('' AS VARCHAR(1)) SP_CAR,
									   CAST ('' AS VARCHAR(10)) ALL_ID
								  FROM F5103
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'C' CHECK_ACCOUNT_TYPE,
									   ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '003' ITEM_TYPE_ID,
									   (SELECT TOP(1)H.ACC_ITEM_NAME
										  FROM F500104 H
										 WHERE     H.DC_CODE = F5104.DC_CODE
											   AND H.GUP_CODE = F5104.GUP_CODE
											   AND H.CUST_CODE = F5104.CUST_CODE
											   AND H.QUOTE_NO = F5104.QUOTE_NO
											   --AND ROWNUM = 1
											   )
										  ACC_ITEM_NAME,
									   WMS_NO,
									   ITEM_CODE,
									   LTRIM(QTY) QTY,
									   LTRIM(AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   LTRIM ('') AS DELV_DATE,
									   CAST ('' AS VARCHAR (20)) PAST_NO,
									   LTRIM ('') INVOICE_CNT,
									   LTRIM ('') SA_QTY,
									   LTRIM ('') PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR (20)) PROCESS_ID,
									   CAST ('' AS VARCHAR (5)) TAKE_TIME,
									   CAST ('' AS VARCHAR (20)) DISTR_CAR_NO,
									   LTRIM ('') VOLUMN,
									   LTRIM ('') WEIGHT,
									   CAST ('' AS VARCHAR (5)) ZIP_CODE,
									   CAST ('' AS VARCHAR (5)) DELV_TMPR,
									   CAST ('' AS VARCHAR (1)) CAN_FAST,
									   CAST ('' AS VARCHAR (2)) DISTR_USE,
									   CAST ('' AS VARCHAR (1)) SP_CAR,
									   CAST ('' AS VARCHAR (10)) ALL_ID
								  FROM F5104
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'D' CHECK_ACCOUNT_TYPE,
									   CAST ('' AS VARCHAR (2)) ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '' ITEM_TYPE_ID,
									   ACC_ITEM_NAME,
									   WMS_NO,
									   ITEM_CODE,
									   LTRIM(QTY) QTY,
									   LTRIM(AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   CONVERT (varchar,DELV_DATE, 111) AS DELV_DATE,
									   PAST_NO,
									   LTRIM('') INVOICE_CNT,
									   LTRIM('') SA_QTY,
									   LTRIM(PACKAGE_BOX_NO) PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR (20)) PROCESS_ID,
									   CAST ('' AS VARCHAR (5)) TAKE_TIME,
									   CAST ('' AS VARCHAR (20)) DISTR_CAR_NO,
									   LTRIM ('') VOLUMN,
									   LTRIM ('') WEIGHT,
									   CAST ('' AS VARCHAR (5)) ZIP_CODE,
									   CAST ('' AS VARCHAR (5)) DELV_TMPR,
									   CAST ('' AS VARCHAR (1)) CAN_FAST,
									   CAST ('' AS VARCHAR (2)) DISTR_USE,
									   CAST ('' AS VARCHAR (1)) SP_CAR,
									   CAST ('' AS VARCHAR (10)) ALL_ID
								  FROM F5105
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'E' CHECK_ACCOUNT_TYPE,
									   CAST ('' AS VARCHAR (2)) ACC_ITEM_KIND_ID,
									   '' ITEM_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) DELV_ACC_TYPE,
									   (SELECT TOP(1)H.QUOTE_NAME
										  FROM F910401 H
										 WHERE     (H.DC_CODE = F5106.DC_CODE OR H.DC_CODE = '000')
											   AND H.GUP_CODE = F5106.GUP_CODE
											   AND H.CUST_CODE = F5106.CUST_CODE
											   AND H.QUOTE_NO = F5106.QUOTE_NO
											   --AND ROWNUM = 1
											   )
										  ACC_ITEM_NAME,
									   WMS_NO,
									   ITEM_CODE,
									   LTRIM(QTY) QTY,
									   LTRIM(AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   LTRIM ('') AS DELV_DATE,
									   CAST ('' AS VARCHAR (20)) PAST_NO,
									   LTRIM ('') INVOICE_CNT,
									   LTRIM ('') SA_QTY,
									   LTRIM ('') PACKAGE_BOX_NO,
									   ITEM_CODE_BOM,
									   PROCESS_ID,
									   CAST ('' AS VARCHAR (5)) TAKE_TIME,
									   CAST ('' AS VARCHAR (20)) DISTR_CAR_NO,
									   LTRIM ('') VOLUMN,
									   LTRIM ('') WEIGHT,
									   CAST ('' AS VARCHAR (5)) ZIP_CODE,
									   CAST ('' AS VARCHAR (5)) DELV_TMPR,
									   CAST ('' AS VARCHAR (1)) CAN_FAST,
									   CAST ('' AS VARCHAR (2)) DISTR_USE,
									   CAST ('' AS VARCHAR (1)) SP_CAR,
									   CAST ('' AS VARCHAR (10)) ALL_ID
								  FROM F5106
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'F' CHECK_ACCOUNT_TYPE,
									   ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '005' ITEM_TYPE_ID,
									   (SELECT TOP(1)H.ACC_ITEM_NAME
										  FROM F500102 H
										 WHERE     H.DC_CODE = F5107.DC_CODE
											   AND H.GUP_CODE = F5107.GUP_CODE
											   AND H.CUST_CODE = F5107.CUST_CODE
											   AND H.QUOTE_NO = F5107.QUOTE_NO
											   --AND ROWNUM = 1
											   )
										  ACC_ITEM_NAME,
									   WMS_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE,
									   LTRIM (QTY) QTY,
									   LTRIM (AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   CONVERT (varchar,DELV_DATE,111) AS DELV_DATE,
									   PAST_NO,
									   LTRIM ('') INVOICE_CNT,
									   LTRIM ('') SA_QTY,
									   LTRIM (PACKAGE_BOX_NO) PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR (20)) PROCESS_ID,
									   TAKE_TIME,
									   DISTR_CAR_NO,
									   LTRIM(VOLUMN) VOLUMN,
									   LTRIM(WEIGHT) WEIGHT,
									   ZIP_CODE,
									   DELV_TMPR,
									   CAN_FAST,
									   DISTR_USE,
									   SP_CAR,
									   ALL_ID
								  FROM F5107
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'G' CHECK_ACCOUNT_TYPE,
									   CAST ('' AS VARCHAR (2)) ACC_ITEM_KIND_ID,
									   '' ITEM_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) DELV_ACC_TYPE,
									   ACC_ITEM_NAME,
									   CAST ('' AS VARCHAR (20)) WMS_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE,
									   LTRIM ('') QTY,
									   LTRIM (AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   LTRIM ('') AS DELV_DATE,
									   CAST ('' AS VARCHAR (20)) PAST_NO,
									   LTRIM ('') INVOICE_CNT,
									   LTRIM ('') SA_QTY,
									   LTRIM ('') PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR (20)) PROCESS_ID,
									   CAST ('' AS VARCHAR (5)) TAKE_TIME,
									   CAST ('' AS VARCHAR (20)) DISTR_CAR_NO,
									   LTRIM ('') VOLUMN,
									   LTRIM ('') WEIGHT,
									   CAST ('' AS VARCHAR (5)) ZIP_CODE,
									   CAST ('' AS VARCHAR (5)) DELV_TMPR,
									   CAST ('' AS VARCHAR (1)) CAN_FAST,
									   CAST ('' AS VARCHAR (2)) DISTR_USE,
									   CAST ('' AS VARCHAR (1)) SP_CAR,
									   CAST ('' AS VARCHAR (10)) ALL_ID
								  FROM F5108
								UNION ALL
								SELECT CAL_DATE,
									   DC_CODE,
									   GUP_CODE,
									   CUST_CODE,
									   'H' CHECK_ACCOUNT_TYPE,
									   CAST ('' AS VARCHAR (2)) ACC_ITEM_KIND_ID,
									   DELV_ACC_TYPE,
									   '' ITEM_TYPE_ID,
									   CAST ('' AS NVARCHAR (50)) ACC_ITEM_NAME,
									   WMS_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE,
									   LTRIM (QTY) QTY,
									   LTRIM (AMT) AMT,
									   CAST ('' AS VARCHAR (4)) LOC_TYPE_ID,
									   CAST ('' AS VARCHAR (2)) TMPR_TYPE,
									   CONVERT (varchar,DELV_DATE,111) AS DELV_DATE,
									   PAST_NO,
									   LTRIM ('') INVOICE_CNT,
									   LTRIM ('') SA_QTY,
									   LTRIM (PACKAGE_BOX_NO) PACKAGE_BOX_NO,
									   CAST ('' AS VARCHAR (20)) ITEM_CODE_BOM,
									   CAST ('' AS VARCHAR (20)) PROCESS_ID,
									   TAKE_TIME,
									   DISTR_CAR_NO,
									   LTRIM (VOLUMN) VOLUMN,
									   LTRIM (WEIGHT) WEIGHT,
									   ZIP_CODE,
									   DELV_TMPR,
									   CAN_FAST,
									   DISTR_USE,
									   SP_CAR,
									   ALL_ID
								  FROM F5109) A
							   INNER JOIN VW_F000904_LANG B
								  ON     B.TOPIC = 'P710702'
									 AND B.SUBTOPIC = 'CheckAccountType'
									 AND VALUE = A.CHECK_ACCOUNT_TYPE
									 AND B.LANG = '{Current.Lang}'
							   INNER JOIN F1901 C ON C.DC_CODE = A.DC_CODE
							   INNER JOIN F1929 D ON D.GUP_CODE = A.GUP_CODE
							   INNER JOIN F1909 E
								  ON E.GUP_CODE = A.GUP_CODE AND E.CUST_CODE = A.CUST_CODE
							   LEFT JOIN F1942 F ON F.LOC_TYPE_ID = A.LOC_TYPE_ID
							   LEFT JOIN VW_F000904_LANG G
								  ON     G.TOPIC = 'F1980'
									 AND G.SUBTOPIC = 'TMPR_TYPE'
									 AND G.VALUE = A.TMPR_TYPE
									 AND G.LANG = '{Current.Lang}'
							   LEFT JOIN VW_F000904_LANG I
								  ON     I.TOPIC = 'F91000301'
									 AND I.SUBTOPIC = 'DELV_ACC_TYPE'
									 AND I.VALUE = A.DELV_ACC_TYPE
									 AND I.LANG = '{Current.Lang}'
							   LEFT JOIN F1903 J
								  ON J.GUP_CODE = A.GUP_CODE AND J.ITEM_CODE = A.ITEM_CODE AND J.CUST_CODE = A.CUST_CODE
							   LEFT JOIN F1903 K
								  ON K.GUP_CODE = A.GUP_CODE AND K.ITEM_CODE = A.ITEM_CODE_BOM AND K.CUST_CODE = A.CUST_CODE
							   LEFT JOIN F1947 L ON L.DC_CODE = A.DC_CODE AND L.ALL_ID = A.ALL_ID
							   LEFT JOIN VW_F000904_LANG M
								  ON     M.TOPIC = 'F194701'
									 AND M.SUBTOPIC = 'DELV_TMPR'
									 AND M.VALUE = A.DELV_TMPR
									 AND M.LANG = '{Current.Lang}'
							   LEFT JOIN                            -- 因系統單號若是出貨單號的話，派車相關的資料就會沒有，需額外取得
							   (SELECT B.DC_CODE,
									   B.GUP_CODE,
									   B.CUST_CODE,
									   B.DISTR_CAR_NO,
									   B.WMS_NO,
									   B.TAKE_TIME,
									   B.VOLUMN,
									   F.WEIGHT,
									   B.ZIP_CODE,
									   B.DELV_TMPR,
									   C.NAME AS DELV_TMPR_NAME,
									   B.CAN_FAST,
									   B.DISTR_USE,
									   D.NAME AS DISTR_USE_NAME,
									   A.SP_CAR,
									   A.ALL_ID,
									   E.ALL_COMP
								  FROM F700101 A
									   JOIN F700102 B
										  ON     A.DC_CODE = B.DC_CODE
											 AND A.DISTR_CAR_NO = B.DISTR_CAR_NO
									   LEFT JOIN VW_F000904_LANG C
										  ON     C.TOPIC = 'F194701'
											 AND C.SUBTOPIC = 'DELV_TMPR'
											 AND C.VALUE = B.DELV_TMPR
											 AND C.LANG = '{Current.Lang}'
									   LEFT JOIN VW_F000904_LANG D
										  ON     D.TOPIC = 'F700102'
											 AND D.SUBTOPIC = 'DISTR_USE'
											 AND D.VALUE = B.DISTR_USE
											 AND D.LANG = '{Current.Lang}'
									   LEFT JOIN F1947 E
										  ON E.DC_CODE = A.DC_CODE AND E.ALL_ID = A.ALL_ID
									   LEFT JOIN F050801 F
										  ON     B.DC_CODE = F.DC_CODE
											 AND B.GUP_CODE = F.GUP_CODE
											 AND B.CUST_CODE = F.CUST_CODE
											 AND B.WMS_NO = F.WMS_ORD_NO
								 WHERE A.STATUS IN ('1', '2')) N
								  ON     A.DC_CODE = N.DC_CODE
									 AND A.GUP_CODE = N.GUP_CODE
									 AND A.CUST_CODE = N.CUST_CODE
									 AND A.WMS_NO = N.WMS_NO
						 WHERE A.DC_CODE = @p0 ";
			var param = new List<object> {dcCode};
			if (calSDate.HasValue)
			{
				sql += " AND A.CAL_DATE >=@p" + param.Count();
				param.Add(calSDate);
			}
			if (calEDate.HasValue)
			{
				sql += " AND A.CAL_DATE <=@p" + param.Count();
				param.Add(calEDate);
			}
			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += " AND A.GUP_CODE =@p" + param.Count();
				param.Add(gupCode);
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += " AND A.CUST_CODE =@p" + param.Count();
				param.Add(custCode);
			}
			if (!string.IsNullOrEmpty(allId))
			{
				sql += " AND A.ALL_ID =@p" + param.Count();
				param.Add(allId);
			}
			return SqlQuery<F51ComplexReportData>(sql, param.ToArray());
		}

		public IQueryable<SettleMonFeeData> GetLocSettleMonFee(DateTime calDate, string quoteNo)
		{			
			var parameters = new List<object>
			{
				calDate.AddMonths(-1),
				calDate,
				quoteNo
			};
			var sql = @"
                SELECT ROW_NUMBER()OVER(ORDER BY tb.ACC_ITEM_NAME,tb.UNIT_FEE,tb.IN_TAX) AS ROWNUM,
	                tb.* FROM (
	                            SELECT B.ACC_ITEM_NAME,
                                        1 ACC_NUM,
                                        B.APPROV_UNIT_FEE UNIT_FEE,
				                                0 BASIC_FEE,
				                                0 OVER_FEE,
                                        SUM (A.LOC_QTY) PRICE_CNT,
                                        SUM (A.LOC_AMT) COST,
                                        SUM (A.LOC_AMT) AMOUNT,
                                        B.IN_TAX,
				                                'A' ACC_KIND
                                FROM F5102 A JOIN F500101 B ON A.QUOTE_NO = B.QUOTE_NO
                                WHERE A.CAL_DATE >= @p0 AND A.CAL_DATE < @p1 AND A.QUOTE_NO = @p2
                            GROUP BY B.ACC_ITEM_NAME, B.APPROV_UNIT_FEE, B.IN_TAX ) tb";
			return SqlQuery<SettleMonFeeData>(sql, parameters.ToArray()).AsQueryable();
		}
	}

}
