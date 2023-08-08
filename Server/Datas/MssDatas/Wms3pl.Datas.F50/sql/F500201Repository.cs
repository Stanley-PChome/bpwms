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

namespace Wms3pl.Datas.F50
{
	public partial class F500201Repository : RepositoryBase<F500201, Wms3plDbContext, F500201Repository>
	{
		public IQueryable<F500201ClearingData> GetF500201ClearingData(string gupCode, string custCode, string outSourceId, string clearingYearMonth)
		{
			var sqlParamers = new List<object>
			{
				gupCode
			};

			var sql = @"						
                    SELECT ROW_NUMBER()OVER(order by CNT_DATE,CONTRACT_NO,QUOTE_NO,ITEM_TYPE_ID)ROWNUM,
                            CNT_DATE,
                            CONTRACT_NO,
                            QUOTE_NO,
                            ITEM_TYPE_ID,
	                        (SELECT ITEM_TYPE FROM F910003 WHERE F910003.ITEM_TYPE_ID = F500201.ITEM_TYPE_ID) AS ITEM_TYPE,
                            ACC_ITEM_NAME,      
                            COST,
                            AMOUNT,
                            IS_TAX,
                            STATUS
                    FROM F500201 WHERE GUP_CODE = @p0
			";
			//貨主
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += " AND CUST_CODE = @p" + sqlParamers.Count;
				sqlParamers.Add(custCode);
			}
			//委外商
			if (!string.IsNullOrEmpty(outSourceId))
			{
				sql += " AND OUTSOURCE_ID = @p" + sqlParamers.Count;
				sqlParamers.Add(outSourceId);
			}

			//結算期間 -起 迄
			var selectDate = DateTimeHelper.ConversionDate(clearingYearMonth);
			sql += String.Format(@" AND CNT_DATE BETWEEN convert (varchar, @p{0},111)", sqlParamers.Count);
			sqlParamers.Add(DateTimeHelper.DateMonthFirstDate(selectDate).ToString("yyyy/MM/dd"));
			sql += String.Format(@" AND convert(varchar,@p{0},111)", sqlParamers.Count);
			sqlParamers.Add(DateTimeHelper.DateMonthLastDate(selectDate).ToString("yyyy/MM/dd"));

			sql += " ORDER BY CNT_DATE ";

			var result = SqlQuery<F500201ClearingData>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<RP7105100001> GetRp7105100001Data(string gupCode, string custCode, string outSourceId, DateTime cntDate)
		{			
			var firstDate = DateTimeHelper.DateMonthFirstDate(cntDate);
			var lastDate = DateTimeHelper.DateMonthLastDate(cntDate);

			var sqlParamers = new List<object> { gupCode, firstDate, lastDate };
			var sql = @"						
SELECT ROW_NUMBER()OVER(ORDER BY TB.GUP_NAME, TB.GUP_CODE,TB.CUST_NAME,TB.CUST_CODE)ROWNUM,TB.* FROM (
SELECT 
       (SELECT GUP_NAME
          FROM F1929
         WHERE F1929.GUP_CODE = A.GUP_CODE)
          GUP_NAME,
       GUP_CODE,
       (SELECT CUST_NAME
          FROM F1909
         WHERE F1909.CUST_CODE = A.CUST_CODE AND F1909.GUP_CODE = A.GUP_CODE)
          CUST_NAME,
       CUST_CODE,
			 A.ITEM_TYPE_ID,
       A.QUOTE_NO +'-' +A.ACC_ITEM_NAME AS QUOTE_NO,
       A.PRICE_DETAIL DETAIL,
       CASE A.IS_TAX WHEN '0' THEN A.COST ELSE -1 END NOTAX,          --小計[未稅]
       CASE A.IS_TAX WHEN '1' THEN A.COST ELSE -1 END TAX,            --小計[含稅]
       A.AMOUNT,                                                      --費用[含稅]
          convert (varchar,A.CNT_DATE_S,111)
       +'~'
       +convert (varchar,A.CNT_DATE, 111)
          CNT_DATE_RANGE,
       (CASE A.ITEM_TYPE_ID WHEN '007' THEN '二、其他運費' ELSE '一、物流費用' END)
          GROUPNAME
  FROM F500201 A
 WHERE		 A.GUP_CODE = @p0
			 AND A.CNT_DATE BETWEEN @p1 AND @p2
			";
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += string.Format(" AND A.CUST_CODE = @p{0}", sqlParamers.Count);
				sqlParamers.Add(custCode);
			}
			if (!string.IsNullOrEmpty(outSourceId))
			{
				sql += string.Format(" AND A.OUTSOURCE_ID = @p{0}", sqlParamers.Count);
				sqlParamers.Add(outSourceId);
			}
            sql += " )TB ";

			var result = SqlQuery<RP7105100001>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<RP7105100002> GetRp7105100002Data(DateTime cntDate, string contractNo)
		{
			var firstDate = DateTimeHelper.DateMonthFirstDate(cntDate);
			var lastDate = DateTimeHelper.DateMonthLastDate(cntDate);
			var sqlParamers = new List<object> { cntDate.AddMonths(-1), cntDate.AddDays(1), contractNo, firstDate, lastDate };

			var sql = $@"	

SELECT ROW_NUMBER()OVER(
		ORDER BY 
				 TB2.COST_CENTER,
                 TB2.DELV_DATE,
				 TB2.WMS_ORD_NO,
                 TB2.CUST_ORD_NO,
                 TB2.PAST_NO,
                 TB2.ITEM_CODE
		) ROWNUM,
       TB2.*,
	   (SELECT NAME
          FROM VW_F000904_LANG
         WHERE     VW_F000904_LANG.TOPIC = 'F50020104'
               AND VW_F000904_LANG.SUBTOPIC = 'TRANS_TYPE'
			   AND VW_F000904_LANG.LANG = '{Current.Lang}'
               AND VALUE = TRANS_TYPE)
          TRANS_TYPE_DESC
  FROM (  SELECT    CONVERT (VARCHAR,A.CNT_DATE_S, 111)
                 + '~'
                 +  CONVERT (VARCHAR,A.CNT_DATE, 111)
                    CNT_DATE_RANGE,
                 D.CUST_COST COST_CENTER,
                  CONVERT (VARCHAR,B.DELV_DATE,111) DELV_DATE,
                 D.CUST_NAME,
                 D.CUST_ORD_NO,
                 B.PAST_NO,
                 B.WMS_NO WMS_ORD_NO,
                 CASE
                    WHEN ISNULL (D.COLLECT_AMT, 0) > 0 THEN '現金'
                    ELSE '非現金'
                 END
                    COLLECT_DESC,                                     --付款方式
                 ISNULL(D.COLLECT_AMT,0) COLLECT_AMT,
                 0 SERVICE_CHARGE,                                     --手續費
                 E.ITEM_CODE,
                 X.ITEM_NAME,
                 E.ORD_QTY A_DELV_QTY,
                 D.ADDRESS,
                 CASE
                    WHEN D.SELF_TAKE = '1' THEN '1'
                    WHEN D.SPECIAL_BUS = '1' THEN '2'
                    WHEN F.DELV_EFFIC = '01' THEN '0'
                    WHEN F.DELV_EFFIC = '02' THEN '3'
                    WHEN F.DELV_EFFIC = '03' THEN '4'
                    WHEN F.DELV_EFFIC = '04' THEN '5'
                 END
                    TRANS_TYPE,
                 0 UNIONFEE,                                           --聯運費
                 0 SPECIALFEE,                                        --特殊運費
                 D.SA,
                 ISNULL(D.SA_QTY,0) SA_QTY,
                 B.AMT FEE,
                 B.AMT TOTAL_AMOUNT,                                  --單據金額
                 '' REMARK                                              --備註
            FROM (SELECT CAL_DATE,
                         DELV_DATE,
                         WMS_NO,
                         ITEM_CODE,
                         PAST_NO,
                         CASE WHEN rn > 1 THEN 0 ELSE SumAmt END AMT,
                         CUST_CODE,
                         GUP_CODE,
                         DC_CODE,
                         DELV_ACC_TYPE,
                         CONTRACT_NO,
                         QUOTE_NO
                    FROM (SELECT CAL_DATE,
                                 DELV_DATE,
                                 WMS_NO,
                                 ITEM_CODE,
                                 PAST_NO,
                                 SumAmt,
                                 ROW_NUMBER ()
                                 OVER (PARTITION BY WMS_NO
                                       ORDER BY SumAmt DESC)
                                    rn,
                                 CUST_CODE,
                                 GUP_CODE,
                                 DC_CODE,
                                 DELV_ACC_TYPE,
                                 CONTRACT_NO,
                                 QUOTE_NO
                            FROM (SELECT CAL_DATE,
                                         DELV_DATE,
                                         WMS_NO,
                                         ITEM_CODE,
                                         PAST_NO,
                                         SUM (
                                            AMT)
                                         OVER (PARTITION BY WMS_NO
                                               ORDER BY ITEM_CODE DESC)
                                            SumAmt,
                                         CUST_CODE,
                                         GUP_CODE,
                                         DC_CODE,
                                         DELV_ACC_TYPE,
                                         CONTRACT_NO,
                                         QUOTE_NO
                                    FROM F5103 B
                                   WHERE     B.CAL_DATE >= @p0
                                         AND B.CAL_DATE < @p1
                                         AND B.CONTRACT_NO = @p2) T1 ) tb
                   WHERE ITEM_CODE IS NOT NULL) B
                 JOIN F05030101 C
                    ON     B.DC_CODE = C.DC_CODE
                       AND B.GUP_CODE = C.GUP_CODE
                       AND B.CUST_CODE = C.CUST_CODE
                       AND B.WMS_NO = C.WMS_ORD_NO
                 JOIN F050301 D
                    ON     C.DC_CODE = D.DC_CODE
                       AND C.GUP_CODE = D.GUP_CODE
                       AND C.CUST_CODE = D.CUST_CODE
                       AND C.ORD_NO = D.ORD_NO
                 JOIN F050302 E
                    ON     D.DC_CODE = E.DC_CODE
                       AND D.GUP_CODE = E.GUP_CODE
                       AND D.CUST_CODE = E.CUST_CODE
                       AND D.ORD_NO = E.ORD_NO
                       AND B.ITEM_CODE = E.ITEM_CODE
                 LEFT JOIN F700102 F
                    ON     B.DC_CODE = F.DC_CODE
                       AND B.GUP_CODE = F.GUP_CODE
                       AND B.CUST_CODE = F.CUST_CODE
                       AND B.WMS_NO = F.WMS_NO
                 LEFT JOIN F1903 X
                    ON B.GUP_CODE = X.GUP_CODE AND B.ITEM_CODE = X.ITEM_CODE AND B.CUST_CODE = X.CUST_CODE 
                 LEFT JOIN F500201 A
                    ON     A.CONTRACT_NO = B.CONTRACT_NO
                       AND A.QUOTE_NO = B.QUOTE_NO
                       AND (A.DC_CODE = B.DC_CODE OR A.DC_CODE = '000')
                       AND A.GUP_CODE = B.GUP_CODE
                       AND A.CUST_CODE = B.CUST_CODE
											 AND A.CNT_DATE BETWEEN @p3 AND @p4                       
        --ORDER BY CUST_COST,
        --         DELV_DATE,
								-- WMS_ORD_NO,
        --         CUST_ORD_NO,
        --         PAST_NO,
        --         ITEM_CODE
				 ) TB2
				 
        ";
			var result = SqlQuery<RP7105100002>(sql, sqlParamers.ToArray()).AsQueryable();			
			return result;
		}

		public IQueryable<RP7105100003> GetRp7105100003Data(DateTime cntDate, string contractNo)
		{
			var firstDate = DateTimeHelper.DateMonthFirstDate(cntDate);
			var lastDate = DateTimeHelper.DateMonthLastDate(cntDate);
			var sqlParamers = new List<object> { cntDate.AddMonths(-1), cntDate.AddDays(1), contractNo, firstDate, lastDate };

			var sql = @"		
            
SELECT 
	   ROW_NUMBER()OVER(ORDER BY C.COST_CENTER,C.RETURN_DATE,C.CUST_ORD_NO,C.RETURN_NO,C.RTN_CUST_NAME)
	   ROWNUM,
       C.COST_CENTER,
       CONVERT(VARCHAR,C.RETURN_DATE,111) PROCESS_DATE,              --退貨日期
       C.CUST_ORD_NO,                                                --退貨/取件單號(貨主單號)
	   C.RETURN_NO,																									 --退貨單號
       C.RTN_CUST_NAME CUST_NAME,                                       --客戶名稱
       F.ITEM_CODE,                                                     --商品編號
       E.ITEM_NAME,                                                     --商品名稱
       ISNULL (B.QTY, -1) PROCESS_QTY,                                       --數量
       -1 PRICE,                                                          --單價
       ISNULL (B.AMT, -1) FEE,                                               --運費
       D.CAUSE,                                                           --備註
          CONVERT(VARCHAR,A.CNT_DATE_S, 111)
       + '~'
       + CONVERT(VARCHAR,A.CNT_DATE, 111)
          CNT_DATE_RANGE
  FROM 
	F500201 A
	INNER JOIN F5107 B ON A.CONTRACT_NO = B.CONTRACT_NO AND A.QUOTE_NO = B.QUOTE_NO AND (A.DC_CODE = B.DC_CODE OR A.DC_CODE = '000') AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
	INNER JOIN F161201 C ON B.DC_CODE = C.DC_CODE AND B.GUP_CODE = C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE AND B.WMS_NO = C.RETURN_NO
  	INNER JOIN F161202 F ON C.DC_CODE = F.DC_CODE AND C.GUP_CODE = F.GUP_CODE AND C.CUST_CODE = F.CUST_CODE AND C.RETURN_NO = F.RETURN_NO
  	LEFT JOIN F1951 D ON C.RTN_CAUSE = D.UCC_CODE
	LEFT JOIN F1903 E ON F.GUP_CODE =  E.GUP_CODE AND F.ITEM_CODE = E.ITEM_CODE AND F.CUST_CODE = E.CUST_CODE
 WHERE     D.UCT_ID = 'RT'
       AND B.CAL_DATE >= @p0 AND B.CAL_DATE < @p1 
       AND A.CONTRACT_NO = @p2
			 AND A.CNT_DATE BETWEEN @p3 AND @p4
		 ORDER BY C.RETURN_DATE,C.RETURN_NO,C.CUST_ORD_NO,F.ITEM_CODE
";
			var result = SqlQuery<RP7105100003>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<RP7105100004> GetRp7105100004Data(DateTime cntDate, string contractNo)
		{
			var firstDate = DateTimeHelper.DateMonthFirstDate(cntDate);
			var lastDate = DateTimeHelper.DateMonthLastDate(cntDate);
			var sqlParamers = new List<object> { cntDate.AddMonths(-1), cntDate.AddDays(1), contractNo, firstDate, lastDate };

			var sql = $@"						                        
  						SELECT ROW_NUMBER()OVER(ORDER BY C.RETURN_NO, C.CUST_ORD_NO, D.ITEM_CODE )ROWNUM,
         C.COST_CENTER,
         convert(varchar,C.RETURN_DATE, 111) CRT_WMS_DATE,             --通知日
         ISNULL (C.CUST_ORD_NO, B.WMS_NO) CUST_ORD_NO,                     --入帳單號
         B.WMS_NO,                                                      --退貨單號
         C.RTN_CUST_NAME CUST_NAME,                                     --客戶名稱
         D.ITEM_CODE,                                                   --商品編號
         E.ITEM_NAME,                                                   --商品名稱
         (SELECT '退貨' + X.NAME + '入帳'
            FROM VW_F000904_LANG X
           WHERE X.TOPIC = 'F1903' AND X.SUBTOPIC = 'ACC_TYPE' AND X.VALUE = E.TYPE AND X.LANG = '{Current.Lang}')
            CAUSE,                                                      --計價類別
         ISNULL (B.QTY, -1) PROCESS_QTY,                                     --數量
         ISNULL (F.FEE, -1) PRICE,                                           --單價
         ISNULL (B.AMT, -1) TOTAL,                                           --總計
            convert(varchar,A.CNT_DATE_S, 111)
         + '~'
         + convert(varchar,A.CNT_DATE, 111)
            CNT_DATE_RANGE
    FROM F500201 A,
         F5104 B,
         F161201 C,
         F161202 D,
         F1903 E,
         F500104 F
   WHERE     A.CONTRACT_NO = B.CONTRACT_NO
         AND A.QUOTE_NO = B.QUOTE_NO
         AND (A.DC_CODE = B.DC_CODE OR A.DC_CODE = '000')
         AND A.GUP_CODE = B.GUP_CODE
         AND A.CUST_CODE = B.CUST_CODE
         AND B.DC_CODE = C.DC_CODE
         AND B.GUP_CODE = C.GUP_CODE
         AND B.CUST_CODE = C.CUST_CODE
         AND B.ITEM_CODE = D.ITEM_CODE
         AND B.WMS_NO = C.RETURN_NO
         AND C.DC_CODE = D.DC_CODE
         AND C.GUP_CODE = D.GUP_CODE
         AND C.CUST_CODE = D.CUST_CODE
         AND C.RETURN_NO = D.RETURN_NO
         AND D.GUP_CODE = E.GUP_CODE
         AND D.ITEM_CODE = E.ITEM_CODE
         AND D.CUST_CODE = E.CUST_CODE
         AND (B.DC_CODE = F.DC_CODE OR F.DC_CODE = '000')
         AND B.GUP_CODE = F.GUP_CODE
         AND B.CUST_CODE = F.CUST_CODE
         AND B.QUOTE_NO = F.QUOTE_NO
         AND B.CAL_DATE >= @p0
         AND B.CAL_DATE < @p1
         AND A.CONTRACT_NO = @p2
				 AND A.CNT_DATE BETWEEN @p3 AND @p4 ";
			var result = SqlQuery<RP7105100004>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<RP7105100005> GetRp7105100005Data(DateTime cntDate, string contractNo)
		{
			var firstDate = DateTimeHelper.DateMonthFirstDate(cntDate);
			var lastDate = DateTimeHelper.DateMonthLastDate(cntDate);
			var sqlParamers = new List<object> { cntDate.AddMonths(-1), cntDate.AddDays(1), contractNo, firstDate, lastDate };

			var sql = @"						
SELECT ROW_NUMBER()OVER(ORDER BY TB.COST_CENTER,TB.TAKE_DATE,TB.DISTR_DATE,TB.PAST_NO)ROWNUM,* FROM(
SELECT --
       '' COST_CENTER,
       convert(varchar,C.TAKE_DATE,  111) TAKE_DATE,   --派車日
       convert(varchar,C.TAKE_DATE , 111) DISTR_DATE, --通知日
       '' PAST_NO,                                      --查貨號碼
       D.CUST_NAME,                                     --客戶名稱
       '' CAUSE,                                         --寄送原因
       ISNULL(B.QTY ,0) ACC_NUM,                        --件數
       ISNULL(B.AMT ,0) FEE,                                 --運費
             convert(varchar,A.CNT_DATE_S,111) + '~' + convert(varchar,A.CNT_DATE, 111) CNT_DATE_RANGE
  FROM F500201 A, F5107 B,F700101 C,F700102 D
 WHERE     A.CONTRACT_NO = B.CONTRACT_NO
       AND A.QUOTE_NO = B.QUOTE_NO 
			 AND (A.DC_CODE = B.DC_CODE OR A.DC_CODE = '000')
       AND B.DC_CODE = C.DC_CODE
       AND B.DISTR_CAR_NO = C.DISTR_CAR_NO   
       AND C.DC_CODE = D.DC_CODE
       AND C.DISTR_CAR_NO = D.DISTR_CAR_NO   
       AND B.CAL_DATE >= @p0 AND B.CAL_DATE < @p1 
       AND A.CONTRACT_NO = @p2
			 AND A.CNT_DATE BETWEEN @p3 AND @p4
			 AND D.ORD_TYPE IS NULL
			 )TB ";
			var result = SqlQuery<RP7105100005>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		//列印結算總表
		public void SettlementPrint(string updStaff, string updStaffName, DateTime cntDate, string gupCode, string custCode)
		{
			var sql = @"
						UPDATE F500201
						SET PRINT_DATE = @p5,							   
							   UPD_DATE = @p5,
							   UPD_STAFF = @p0,
							   UPD_NAME = @p1
						 WHERE CNT_DATE = @p2 AND GUP_CODE = @p3 AND CUST_CODE = @p4 AND STATUS = '0'
			";

			var param = new[] {
				new SqlParameter("@p0", updStaff),
				new SqlParameter("@p1", updStaffName),
				new SqlParameter("@p2", cntDate),
        new SqlParameter("@p3", gupCode),
				new SqlParameter("@p4", custCode),
        new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			ExecuteSqlCommand(sql, param);
		}

		/// <summary>
		/// 結算關帳
		/// </summary>
		public void SettlementClosing(string updStaff, string updStaffName, DateTime cntDate, string gupCode,string custCode)
		{
			var sql = @"
						UPDATE F500201
						SET STATUS = '1',
							   CLOSE_DATE = @p5,
							   UPD_DATE = @p5,
							   UPD_STAFF = @p0,
							   UPD_NAME = @p1
						 WHERE CNT_DATE = @p2 AND GUP_CODE = @p3 AND CUST_CODE = @p4 AND STATUS = '0'
			";

			var param = new[] {
				new SqlParameter("@p0", updStaff),
				new SqlParameter("@p1", updStaffName),
				new SqlParameter("@p2", cntDate),
        new SqlParameter("@p3", gupCode),
				new SqlParameter("@p4", custCode),
        new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			ExecuteSqlCommand(sql, param);
		}

		public IQueryable<BaseDay> GetBaseDay()
		{
			var sqlParamers = new List<object> { DateTime.Now };

			var sql = @"						
                        select  
								A.DC_CODE 
								,A.GUP_CODE 
								,A.CUST_CODE 
							    , MIN(A.CNT_DATE) CNT_DATE
							    ,DATEDIFF(DAY,MIN(A.CNT_DATE),@p0) BASE_DAY
						from F500201 A 
						group by A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE  
					";

			var result = SqlQuery<BaseDay>(sql, sqlParamers.ToArray()).AsQueryable();

			return result;
		}

		public void DeleteByDate(DateTime cntDate)
		{
			var sql = @"
						DELETE F500201						
						WHERE CNT_DATE = @p0 AND STATUS = '0' ";
			var param = new[]
			{
				new SqlParameter("@p0", cntDate)
			};
			ExecuteSqlCommand(sql, param);
		}

	}
	
}
