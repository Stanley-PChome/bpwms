using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
  public partial class F010201Repository : RepositoryBase<F010201, Wms3plDbContext, F010201Repository>
  {

    public IQueryable<F010201Data> GetF010201Datas(string dcCode, string gupCode, string custCode, string begStockDate,
    string endStockDate, string stockNo, string vnrCode, string vnrName, string custOrdNo, string sourceNo, string status, string userClosed)
    {
      var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode)
                        };
      var sql = $@"   SELECT A.DC_CODE,E.DC_NAME,A.GUP_CODE,A.CUST_CODE,A.STOCK_NO,A.STOCK_DATE,A.SHOP_DATE,
								            A.DELIVER_DATE,A.SOURCE_TYPE, ISNULL(B.SOURCE_NAME,'') SOURCE_NAME,A.SOURCE_NO,A.VNR_CODE,C.VNR_NAME, 
					                  C.ADDRESS AS VNR_ADDRESS,A.CUST_ORD_NO,A.CUST_COST,A.STATUS,D.NAME AS STATUSNAME,A.MEMO, 
					                  A.CRT_STAFF,A.CRT_DATE,A.CRT_NAME,A.UPD_STAFF,A.UPD_DATE,A.UPD_NAME, A.ORD_PROP, A.SHOP_NO, A.EDI_FLAG,
								A.FAST_PASS_TYPE ,A.BOOKING_IN_PERIOD, A.USER_CLOSED_MEMO
					             FROM F010201 A 
					             LEFT JOIN F000902 B ON B.SOURCE_TYPE = A.SOURCE_TYPE 					  
					            INNER JOIN VW_F000904_LANG D ON D.TOPIC ='F010201' AND D.SUBTOPIC='STATUS' AND D.VALUE = A.STATUS AND D.LANG = '{Current.Lang}'
							        INNER JOIN F1901 E 
                         ON E.DC_CODE = A.DC_CODE 
							        INNER JOIN F1909 F 
                         ON F.GUP_CODE = A.GUP_CODE 
                        AND F.CUST_CODE =A.CUST_CODE				     
			                 LEFT JOIN F1908 C 
                         ON C.VNR_CODE = A.VNR_CODE 
                        AND C.GUP_CODE = A.GUP_CODE  
                        AND C.CUST_CODE = CASE WHEN F.ALLOWGUP_VNRSHARE = '1' THEN '0' ELSE F.CUST_CODE END  
					            WHERE A.DC_CODE = @p0 
					              AND A.GUP_CODE = @p1 
					              AND A.CUST_CODE = @p2 ";

      if (!string.IsNullOrEmpty(begStockDate))
      {
        sql += "     AND A.CRT_DATE >= @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, begStockDate));
      }
      if (!string.IsNullOrEmpty(endStockDate))
      {
        sql += "     AND A.CRT_DATE <= @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, endStockDate));
      }
      if (!string.IsNullOrEmpty(stockNo))
      {
        sql += "     AND A.STOCK_NO = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, stockNo));
      }
      if (!string.IsNullOrEmpty(vnrCode))
      {
        sql += "     AND A.VNR_CODE = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, vnrCode));
      }
      if (!string.IsNullOrEmpty(vnrName))
      {
        sql += $"     AND C.VNR_NAME LIKE '%' + @p" + param.Count + "+'%'";
        param.Add(new SqlParameter("@p" + param.Count, vnrName));
      }
      if (!string.IsNullOrEmpty(custOrdNo))
      {
        sql += "     AND A.CUST_ORD_NO = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, custOrdNo));
      }
      if (!string.IsNullOrEmpty(sourceNo))
      {
        sql += "     AND A.SOURCE_NO = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, sourceNo));
      }
      if (!string.IsNullOrEmpty(status))
      {
        sql += "     AND A.STATUS = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, status));
      }
      else
        sql += " AND A.STATUS !='9' ";
      if (!string.IsNullOrEmpty(userClosed))
      {
        sql += "     AND ISNULL(A.USER_CLOSED, '0') = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, userClosed));
      }
      sql += " ORDER BY A.STOCK_NO";

      var result = SqlQuery<F010201Data>(sql, param.ToArray());

      return result;
    }

    /// <summary>
    /// 取得進倉單是否為內部交易或是互賣訂單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <returns></returns>
    public F010201 FindInHouseF010201(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", stockNo)
                        };


      string sql = @"
SELECT
    A.*
FROM
    F010201 A
    INNER JOIN F050301 E ON E.DC_CODE = A.DC_CODE
    AND E.GUP_CODE = A.GUP_CODE
    AND E.CUST_CODE = A.CUST_CODE
    AND E.ORD_NO = A.SOURCE_NO
    INNER JOIN F1901 F ON F.DC_CODE = E.DC_CODE
    AND F.ADDRESS = E.ADDRESS
WHERE
    A.SOURCE_TYPE = '01'
    AND A.DC_CODE = @p0
    AND A.GUP_CODE = @p1
    AND A.CUST_CODE = @p2
    AND A.STOCK_NO = @p3";

      #region 舊的SQL語法，已無內部交易類
      //string sql = " SELECT * " +
      //												 "   FROM ( " +
      //												 "     SELECT B.* " +
      //												 "       FROM F010201 B " +
      //												 "      INNER JOIN F540101 C " +
      //												 "         ON C.TRANSACTION_NO = B.SOURCE_NO AND C.SELL_DC_CODE = C.BUY_DC_CODE " +
      //												 "      WHERE B.SOURCE_TYPE='09' " + //內部交易 
      //												 "      UNION ALL " +
      //												 "     SELECT D.* " +
      //												 "       FROM F010201 D " +
      //												 "      INNER JOIN F050301 E " +
      //												 "         ON E.DC_CODE = D.DC_CODE AND E.GUP_CODE = D.GUP_CODE AND E.CUST_CODE = D.CUST_CODE AND E.ORD_NO = D.SOURCE_NO " +
      //												 "      INNER JOIN F1901 F ON F.DC_CODE = E.DC_CODE AND F.ADDRESS = E.ADDRESS " +
      //												 "      WHERE D.SOURCE_TYPE='01' " + //互賣訂單
      //												 "   ) A " +
      //												 "   WHERE A.DC_CODE = @p0 " +
      //												 "     AND A.GUP_CODE = @p1 " +
      //												 "     AND A.CUST_CODE = @p2 " +
      //												 "     AND A.STOCK_NO = @p3 ";
      #endregion

      var result = SqlQuery<F010201>(sql, param.ToArray()).FirstOrDefault();

      return result;
    }

    public IEnumerable<P020201ReportData> GetInWarehouseReport(string dcCode, string gupCode, string custCode, string purchaseNo)
    {
      var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", purchaseNo),
                                new SqlParameter("@p1", dcCode),
                                new SqlParameter("@p2", gupCode),
                                new SqlParameter("@p3", custCode)
                        };

      string sql = @"
				SELECT 
						A.STOCK_NO ORDER_NO , B.STOCK_SEQ ORDER_SEQ , B.ITEM_CODE , B.STOCK_QTY ORDER_QTY , A.VNR_CODE
						,D.ACC_UNIT_NAME ORDER_UNIT , C.ITEM_NAME , E.VNR_NAME , A.CUST_ORD_NO , A.SHOP_NO , A.DELIVER_DATE
						, C.ITEM_SIZE , C.ITEM_SPEC , C.ITEM_COLOR  ,A.SOURCE_NO, '' AS VOLUME_UNIT --由程式計算
				FROM F010201 A 
				JOIN F010202 B ON A.STOCK_NO = B.STOCK_NO AND A.DC_CODE =B.DC_CODE  AND A.GUP_CODE  =B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
				JOIN F1903 C ON C.ITEM_CODE = B.ITEM_CODE AND C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = B.CUST_CODE
				LEFT JOIN F91000302 D ON D.ITEM_TYPE_ID ='001' AND C.ITEM_UNIT = D.ACC_UNIT
				JOIN F1908 E ON E.GUP_CODE = A.GUP_CODE AND E.VNR_CODE = A.VNR_CODE  
				WHERE A.STOCK_NO = @p0 AND A.DC_CODE = @p1 AND A.GUP_CODE = @p2 AND A.CUST_CODE = @p3
				ORDER BY ORDER_SEQ 
			";

      var result = SqlQuery<P020201ReportData>(sql, param.ToArray());

      return result;
    }

    public IQueryable<DcWmsNoOrdPropItem> GetDcWmsNoOrdPropItems(string dcCode, DateTime stockDate)
    {
      var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", stockDate)
                        };

      string sql = @"
				SELECT ROW_NUMBER()OVER(ORDER BY A.CUST_CODE ASC) ROWNUM  ,A.* 
                      FROM (
                SELECT A.CUST_CODE,A.ORD_PROP,
                			SUM(CASE WHEN A.STATUS = '2' THEN 1 ELSE 0 END) AS CUST_FINISHCOUNT,
                			COUNT(*)  AS CUST_TOTALCOUNT 
                 FROM F010201 A
                WHERE A.DC_CODE = @p0
                	AND A.STOCK_DATE = @p1
                	AND A.STATUS <>'9'
                GROUP BY A.CUST_CODE,A.ORD_PROP ) A 
			";

      var result = SqlQuery<DcWmsNoOrdPropItem>(sql, param.ToArray());

      return result;
    }

    public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItems(string dcCode, string gupCode, string custCode,
    DateTime begStockDate, DateTime endStockDate)
    {
      var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", begStockDate),
                                new SqlParameter("@p4", endStockDate)
                        };

      string sql = @"
				SELECT ROW_NUMBER()OVER(ORDER BY A.WmsDate ASC) ROWNUM, A.* 
					 FROM (
					 SELECT A.STOCK_DATE AS WmsDate,Count(*) AS WmsCount 
					   FROM F010201 A
					  WHERE A.DC_CODE = @p0
					 	  AND A.GUP_CODE = @p1
						  AND A.CUST_CODE = @p2
					      AND A.STOCK_DATE BETWEEN @p3 AND @p4
                          AND A.STATUS <>'9'
					  GROUP BY A.STOCK_DATE ) A
			";

      var result = SqlQuery<DcWmsNoDateItem>(sql, param.ToArray());

      return result;
    }

    #region Schedule Check-物流單(進倉、退貨、出貨、調撥、盤點)

    public IQueryable<OrderIsProblem> GetOrderIsProblem(DateTime selectDate)
    {
      var parameters = new List<SqlParameter>();
      parameters.Add(new SqlParameter("@p0", selectDate));

      string sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY TYPE, NO ASC) ROWNUM,
                         T.*, F1901.DC_NAME, F1909.CUST_NAME, F1929.GUP_NAME  FROM (
                         SELECT '進倉' TYPE, A.STOCK_NO AS NO, ISNULL (A.UPD_DATE, A.CRT_DATE) AS CREATE_DATE, A.DC_CODE, A.CUST_CODE, A.GUP_CODE, (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F010201' AND SUBTOPIC = 'STATUS' AND VALUE =  A.STATUS AND LANG = '{Current.Lang}') STATUS
                           FROM F010201 A
                          WHERE     STATUS = '0' 
                                AND DATEDIFF(minute, ISNULL (A.UPD_DATE, A.CRT_DATE), @p0) > 1
                                
                         UNION
                         SELECT '退貨' TYPE, B.RETURN_NO AS NO, ISNULL (B.UPD_DATE, B.CRT_DATE) AS CREATE_DATE, B.DC_CODE, B.CUST_CODE, B.GUP_CODE, (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F161201' AND SUBTOPIC = 'STATUS' AND VALUE = B.STATUS AND LANG = '{Current.Lang}') STATUS
                           FROM F161201 B
                          WHERE     STATUS = '0' 
                                AND DATEDIFF(minute, ISNULL (B.UPD_DATE, B.CRT_DATE), @p0) > 1
                                
                         UNION
                         SELECT '出貨' TYPE, C.ORD_NO AS NO, ISNULL (C.UPD_DATE, C.CRT_DATE) AS CREATE_DATE, C.DC_CODE, C.CUST_CODE, C.GUP_CODE, (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F050301' AND SUBTOPIC = 'STATUS' AND VALUE = C.PROC_FLAG AND LANG = '{Current.Lang}') STATUS
                           FROM F050301 C
                          WHERE     PROC_FLAG = '0'
                                AND DATEDIFF(minute, ISNULL (C.UPD_DATE, C.CRT_DATE), @p0) > 1
                                
                         UNION
                         SELECT '調撥' TYPE, D.ALLOCATION_NO AS NO, ISNULL (D.UPD_DATE, D.CRT_DATE) AS CREATE_DATE, D.DC_CODE, D.CUST_CODE, D.GUP_CODE, (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F151001' AND SUBTOPIC = 'STATUS' AND VALUE = D.STATUS AND LANG = '{Current.Lang}') STATUS
                           FROM F151001 D
                          WHERE     STATUS = '0'
                                AND DATEDIFF(minute, ISNULL (D.UPD_DATE, D.CRT_DATE), @p0)  > 1
                                
                         UNION
                         SELECT '盤點' TYPE, E.INVENTORY_NO AS NO, ISNULL (E.UPD_DATE, E.CRT_DATE) AS CREATE_DATE, E.DC_CODE, E.CUST_CODE, E.GUP_CODE, (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC = 'F140101' AND SUBTOPIC = 'STATUS' AND VALUE = E.STATUS AND LANG = '{Current.Lang}') STATUS
                           FROM F140101 E
                          WHERE     STATUS = '0'
                                AND (DATEDIFF(minute, ISNULL (E.UPD_DATE, E.CRT_DATE), @p0)) > 1) T
                         LEFT JOIN F1901 ON T.DC_CODE = F1901.DC_CODE
                         LEFT JOIN F1909 ON T.GUP_CODE = F1909.GUP_CODE AND T.CUST_CODE = F1909.CUST_CODE 
                         LEFT JOIN F1929 ON T.GUP_CODE = F1929.GUP_CODE 
                         ORDER BY TYPE, NO
						";

      var data = SqlQuery<OrderIsProblem>(sql, parameters.ToArray());

      return data;
    }

    #endregion

    public IQueryable<P010201PalletData> GetPalletDatas(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", stockNo),
                new SqlParameter("@p4", DateTime.Now)
            };

      string sql = @"SELECT ROW_NUMBER()OVER(ORDER BY B.STICKER_NO ASC) ROWNUM,
                                     D.GUP_NAME,
                        			 A.STOCK_NO,
                        			 C.VNR_NAME,
                        			 B.ITEM_CODE,
                        			 E.ITEM_NAME,
                                     CONVERT(varchar, B.PALLET_LEVEL_CASEQTY) + '*' + CONVERT(varchar, B.PALLET_LEVEL_CNT) PALLET_LEVEL,
                        			 CONVERT(varchar,B.ITEM_CASE_QTY) + CASE WHEN H.ITEM_CODE IS NOT NULL AND I.ITEM_CODE IS NOT NULL THEN N'' ELSE G.ACC_UNIT_NAME END ITEM_CASE_QTY,
                        			 CONVERT(varchar,B.ITEM_PACKAGE_QTY) ITEM_PACKAGE_QTY,
                                     CONVERT(varchar,(CONVERT(INT, B.PALLET_NO) - CONVERT(INT, F.MIN_PALLET_NO) +1)) + '-' + CONVERT(varchar,(CONVERT(INT, F.MAX_PALLET_NO) - CONVERT(INT, F.MIN_PALLET_NO) +1)) PALLET_SEQ,
                                     E.EAN_CODE1,E.EAN_CODE3,
                                     CONVERT(varchar, B.ORDER_CASE_QTY) + '*' + CONVERT(varchar, B.ITEM_CASE_QTY) + CASE WHEN B.ORDER_OTHER_QTY >0 THEN '+' + CONVERT(varchar, B.ORDER_OTHER_QTY) ELSE '' END + G.ACC_UNIT_NAME ORDER_QTY_DESC,
                        			 CASE WHEN B.ENTER_DATE IS NULL THEN '' ELSE CONVERT(varchar, B.ENTER_DATE, 111) END ENTER_DATE,
                                     CASE WHEN B.VALID_DATE IS NULL THEN '' ELSE CONVERT(varchar, B.VALID_DATE, 111) END VALID_DATE,
                                     B.STICKER_NO,CONVERT(varchar, @p4, 111) PRINT_DATE
                        									 FROM F010201 A
                        									 JOIN F010203 B
                        									   ON B.DC_CODE = A.DC_CODE
                        									  AND B.GUP_CODE = A.GUP_CODE
                        									  AND B.CUST_CODE = A.CUST_CODE
                        									  AND B.STOCK_NO = A.STOCK_NO
                        									 JOIN F1908 C
                        									   ON C.GUP_CODE = A.GUP_CODE
                        									  AND C.CUST_CODE = A.CUST_CODE
                        									  AND C.VNR_CODE = A.VNR_CODE
                        									 JOIN F1929 D
                        									   ON D.GUP_CODE = A.GUP_CODE
                        									 JOIN F1903 E
                        									   ON E.GUP_CODE = B.GUP_CODE
                        									  AND E.ITEM_CODE = B.ITEM_CODE
                                                              AND E.CUST_CODE = B.CUST_CODE
                        									 JOIN (SELECT DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,ITEM_CODE,MIN(PALLET_NO) MIN_PALLET_NO, MAX(PALLET_NO) MAX_PALLET_NO
                        													 FROM F010203 
                        												  GROUP BY DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,ITEM_CODE) F
                        									   ON F.DC_CODE = A.DC_CODE
                        									  AND F.GUP_CODE = A.GUP_CODE
                        									  AND F.CUST_CODE = A.CUST_CODE
                        									  AND F.STOCK_NO = A.STOCK_NO
                        										AND F.ITEM_CODE = B.ITEM_CODE
                        									 JOIN F91000302 G
                        									   ON G.ITEM_TYPE_ID ='001' AND G.ACC_UNIT	= E.ITEM_UNIT
                        									 LEFT JOIN (
                        		SELECT A1.GUP_CODE,
                                       A1.ITEM_CODE,
                                       A2.ACC_UNIT_NAME UNIT_NAME
                                  FROM F190301 A1
                                  JOIN F91000302 A2 ON A1.UNIT_ID = A2.ACC_UNIT AND A2.ITEM_TYPE_ID = '001' AND A2.ACC_UNIT_NAME='箱'
                        									 ) H
                        									 ON H.GUP_CODE = B.GUP_CODE
                        									 AND H.ITEM_CODE = B.ITEM_CODE
                        									 LEFT JOIN F190305 I
                        									 ON I.GUP_CODE = B.GUP_CODE
                        									 AND I.CUST_CODE = B.CUST_CODE
                        									 AND I.ITEM_CODE = B.ITEM_CODE
                        									WHERE A.DC_CODE = @p0
                        									  AND A.GUP_CODE = @p1
                        									  AND A.CUST_CODE = @p2
                        									  AND A.STOCK_NO = @p3
                        									ORDER BY B.STICKER_NO
						";

      var data = SqlQuery<P010201PalletData>(sql, parameters.ToArray());

      return data;
    }

    public void DeleteF010201(string stockNo, string dcCode, string gupCode, string custCode)
    {
      string sql = @"
				delete from  F010201 Where STOCK_NO=@p0
									   and DC_CODE =@p1
									   and GUP_CODE =@p2
									   and CUST_CODE =@p3
		
			";
      var sqlParams = new SqlParameter[]
      {
                                new SqlParameter("@p0", stockNo),
                                new SqlParameter("@p1", dcCode),
                                new SqlParameter("@p2", gupCode),
                                new SqlParameter("@p3", custCode)
      };

      ExecuteSqlCommand(sql, sqlParams);
    }

    /// <summary>
    /// 取消進倉單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <param name="importFlag"></param>
    public void CancelNotProcessWarehouseIn(string dcCode, string gupCode, string custCode, string stockNo, string importFlag)
    {
      string sql = @"
				           update F010201 set STATUS = '9', UPD_DATE = @p7, UPD_STAFF = @p0, UPD_NAME = @p1, IMPORT_FLAG=@p6
                           Where DC_CODE =@p2
				             and GUP_CODE =@p3
				             and CUST_CODE =@p4
                             and STOCK_NO=@p5
                             and STATUS <> '9'
			               ";
      var sqlParams = new SqlParameter[]
      {
                 new SqlParameter("@p0", Current.Staff),
                 new SqlParameter("@p1", Current.StaffName),
                 new SqlParameter("@p2", dcCode),
                 new SqlParameter("@p3", gupCode),
                 new SqlParameter("@p4", custCode),
                 new SqlParameter("@p5", stockNo),
                 new SqlParameter("@p6", importFlag),
                 new SqlParameter("@p7", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }

      };

      ExecuteSqlCommand(sql, sqlParams);
    }

    /// <summary>
    /// 取得進倉單By進貨單號/貨主單號
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <returns></returns>
    public F010201 FindDataByStockNoOrCustOrdNo(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var param = new List<SqlParameter>
      {
              new SqlParameter("@p0", dcCode),
              new SqlParameter("@p1", gupCode),
              new SqlParameter("@p2", custCode),
              new SqlParameter("@p3", stockNo),
              new SqlParameter("@p4", stockNo)
      };

      string sql = $@" SELECT * FROM F010201 A
											 WHERE A.DC_CODE = @p0
											 AND A.GUP_CODE = @p1
											 AND A.CUST_CODE = @p2
											 AND (A.STOCK_NO = @p3 OR A.CUST_ORD_NO = @p4 ) ";

      var result = SqlQuery<F010201>(sql, param.ToArray()).FirstOrDefault();

      return result;
    }

    /// <summary>
    /// PDA API取得進貨檢驗驗收單項目清單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <returns></returns>
    public IQueryable<GetStockReceivedDataRes> GetStockReceivedDataRes(string dcCode, string gupCode, string custCode, string stockNo, string itemCode)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
      };

      var sql2 = "";
      if (!string.IsNullOrWhiteSpace(stockNo))
      {
        sql2 += $@"AND(A.STOCK_NO = @p{param.Count} OR A.CUST_ORD_NO = @p{param.Count})";
        param.Add(new SqlParameter($"@p{param.Count}", SqlDbType.VarChar) { Value = stockNo });
      }

      if (!string.IsNullOrWhiteSpace(itemCode))
      {
        sql2 += $@" AND A.CUST_COST = 'In' AND (C.ITEM_CODE = @p{param.Count} OR C.EAN_CODE1 = @p{param.Count} OR C.EAN_CODE2 = @p{param.Count} OR C.EAN_CODE3 = @p{param.Count} OR C.CUST_ITEM_CODE = @p{param.Count}) AND A.STATUS IN('1', '3')";
        param.Add(new SqlParameter($"@p{param.Count}", SqlDbType.VarChar) { Value = itemCode });
      }

      string sql = $@"
SELECT A.STOCK_NO StockNo,
  A.CUST_ORD_NO CustOrdNo,
  A.STATUS Status,
  (
    SELECT TOP 1 NAME
    FROM VW_F000904_LANG X
    WHERE X.TOPIC = 'F010201'
      AND SUBTOPIC = 'STATUS'
      AND LANG = '{Current.Lang}'
      AND VALUE = A.STATUS
  ) StatusDesc,
  A.VNR_CODE VnrCode,
  D.VNR_NAME VnrName,
  A.FAST_PASS_TYPE FastPassType,
  (
    SELECT TOP 1 NAME
    FROM VW_F000904_LANG X
    WHERE X.TOPIC = 'F010201'
      AND SUBTOPIC = 'FAST_PASS_TYPE'
      AND LANG = '{Current.Lang}'
      AND VALUE = A.FAST_PASS_TYPE
  ) FastPassTypeDesc,
  B.ITEM_CODE ItemCode,
  C.CUST_ITEM_CODE CustItemCode,
  C.ITEM_NAME ItemName,
  B.STOCK_QTY Qty,
  B.STOCK_SEQ StockSeq,
  A.SHOP_NO PoNo,
  CAST(CASE WHEN (ISNULL(TRIM(VIRTUAL_TYPE), '')) = '' THEN 0 ELSE 1 END AS BIT) IsVirtualItem,
  CAST(CASE WHEN A.STATUS IN ('0','2','9') OR (E.ID IS NOT NULL AND E.STOCK_QTY <= E.TOTAL_REC_QTY)
            THEN 0 ELSE 1 END AS BIT) CanOperator,
  A.CUST_COST CustCost
FROM F010201 A
  INNER JOIN F010202 B ON A.DC_CODE = B.DC_CODE
  AND A.GUP_CODE = B.GUP_CODE
  AND A.CUST_CODE = B.CUST_CODE
  AND A.STOCK_NO = B.STOCK_NO
  LEFT JOIN F1903 C WITH(nolock) ON B.GUP_CODE = C.GUP_CODE
  AND B.CUST_CODE = C.CUST_CODE
  AND B.ITEM_CODE = C.ITEM_CODE
  LEFT JOIN F1908 D WITH(nolock) ON A.GUP_CODE = D.GUP_CODE
  AND A.CUST_CODE = D.CUST_CODE
  AND A.VNR_CODE = D.VNR_CODE
  LEFT JOIN F010204 E ON B.DC_CODE = E.DC_CODE
  AND B.GUP_CODE = E.GUP_CODE
  AND B.CUST_CODE = E.CUST_CODE
  AND B.STOCK_NO = E.STOCK_NO
  AND B.STOCK_SEQ = E.STOCK_SEQ
WHERE A.DC_CODE = @p0
  AND A.GUP_CODE = @p1
  AND A.CUST_CODE = @p2 " + sql2;

      return SqlQuery<GetStockReceivedDataRes>(sql, param.ToArray());
    }

    public F010201 GetDataByStockNoOrCustOrdNo(string dcCode, string gupCode, string custCode, string custInCode)
    {
      var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", custInCode),
                new SqlParameter("@p4", custInCode),
            };

      string sql = @"
				SELECT * FROM F010201 
                WHERE DC_CODE = @p0
                AND GUP_CODE = @p1
                AND CUST_CODE = @p2
                AND (STOCK_NO = @p3 OR CUST_ORD_NO = @p4)
			";

      var result = SqlQuery<F010201>(sql, param.ToArray()).FirstOrDefault();

      return result;
    }

    public IQueryable<F010201> GetDatasByPoNo(string poNo)
    {
      var paramList = new List<object> { poNo };
      var sql = @"SELECT * FROM F010201 WHERE STATUS <> 9 AND SHOP_NO = @p0";

      return SqlQuery<F010201>(sql, paramList.ToArray());
    }

    /// <summary>
    /// 是否存在F010201尚未填採購單號且該採購單有序號商品。
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseNo"></param>
    /// <returns></returns>
    public bool ExistsEmptyShopNoByBundleSerialNo(string dcCode, string gupCode, string custCode, string purchaseNo)
    {
      var paramList = new List<object> { dcCode, gupCode, custCode, purchaseNo };
      var sql = @"SELECT 
                        A.STOCK_NO
                        FROM F010201 A
                        JOIN F010202 B
                        ON A.DC_CODE = B.DC_CODE
                        AND A.GUP_CODE = B.GUP_CODE
                        AND A.CUST_CODE = B.CUST_CODE
                        AND A.STOCK_NO = B.STOCK_NO
                        JOIN F1903 C
                        ON B.ITEM_CODE = C.ITEM_CODE
                        AND B.GUP_CODE = C.GUP_CODE
                        AND B.CUST_CODE = C.CUST_CODE
                        WHERE A.SHOP_NO IS NULL
                        AND A.SOURCE_TYPE <> '09'
                        AND C.BUNDLE_SERIALNO = '1'
                        AND A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.STOCK_NO = @p3
                        ";

      return SqlQuery<string>(sql, paramList.ToArray()).Any();
    }

    /// <summary>
    /// 修改F010201單具狀態危待處理
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    public void UpdateF010201(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
        new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
        new SqlParameter("@p3",SqlDbType.VarChar){ Value = stockNo},
        new SqlParameter("@p4",SqlDbType.NVarChar){ Value = Current.StaffName},
        new SqlParameter("@p5",SqlDbType.VarChar){ Value = Current.Staff},
        new SqlParameter("@p6",SqlDbType.DateTime2) { Value = DateTime.Now}
      };

      var sql = $@"UPDATE F010201
							SET STATUS = '0' ,CRT_DATE = @p6, UPD_NAME = @p4, UPD_STAFF = @p5
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2
							AND STOCK_NO = @p3";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, param.ToArray());
    }

    public void UpdateFastPassType(string dcCode, string gupCode, string custCode, string custOrdNo, string FastPassType)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode},
        new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode},
        new SqlParameter("@p3",SqlDbType.VarChar) { Value = custOrdNo},
        new SqlParameter("@p4",SqlDbType.VarChar) { Value = FastPassType},
        new SqlParameter("@p5",SqlDbType.NVarChar) { Value = Current.StaffName},
        new SqlParameter("@p6",SqlDbType.VarChar) { Value = Current.Staff},
        new SqlParameter("@p7",SqlDbType.DateTime2) { Value = DateTime.Now }
      };

      var sql = $@"UPDATE F010201
							SET FAST_PASS_TYPE = @p4 ,UPD_DATE = @p7, UPD_NAME = @p5, UPD_STAFF = @p6
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2
							AND CUST_ORD_NO = @p3";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, param.ToArray());

    }

    public IQueryable<F010201> GetDatasByStockNos(string dcCode, string gupCode, string custCode, List<string> stockNos)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode},
        new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode},
      };

      var sql = $@"SELECT * FROM F010201
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2";

      if (stockNos.Any())
        sql += param.CombineSqlInParameters(" AND STOCK_NO", stockNos, SqlDbType.VarChar);

      return SqlQuery<F010201>(sql, param.ToArray());
    }

    public IQueryable<string> CheckAndGetCustOrdNoStartWithTr(string dcCode, string gupCode, string custCode, List<string> stockNos)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode},
        new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode},
      };

      var sql = @"
                SELECT 
                  STOCK_NO 
                FROM 
                  F010201 
                WHERE 
                  DC_CODE = @p0 
                  AND GUP_CODE = @p1 
                  AND CUST_CODE = @p2 
                  AND CUST_ORD_NO LIKE 'TR%'
                ";

      if (stockNos.Any())
        sql += param.CombineSqlInParameters(" AND STOCK_NO", stockNos, SqlDbType.VarChar);
      else
        return null;

      return SqlQuery<string>(sql, param.ToArray());
    }
		/// <summary>
		/// 修改F010201單據狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		public void UpdateStatusByStockNo(string dcCode, string gupCode, string custCode, string stockNo, string status)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode },
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
				new SqlParameter("@p3",SqlDbType.VarChar){ Value = stockNo},
				new SqlParameter("@p4",SqlDbType.NVarChar){ Value = Current.StaffName},
				new SqlParameter("@p5",SqlDbType.VarChar){ Value = Current.Staff},
				new SqlParameter("@p6",SqlDbType.DateTime2) { Value = DateTime.Now},
				new SqlParameter("@p7",SqlDbType.Char) { Value = status},
			};

			var sql = $@"UPDATE F010201
							SET STATUS = @p7, UPD_DATE = @p6, UPD_NAME = @p4, UPD_STAFF = @p5
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2
							AND STOCK_NO = @p3";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public F010201 GetDatasByStockNo(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p3",SqlDbType.VarChar) { Value = stockNo },
			};

			var sql = $@"SELECT TOP(1) * FROM F010201
							      WHERE DC_CODE = @p0
							        AND GUP_CODE = @p1
							        AND CUST_CODE = @p2
							        AND STOCK_NO = @p3 ";

			return SqlQuery<F010201>(sql, param.ToArray()).SingleOrDefault();
		}

		public F010201 GetEnabledStockData(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p3",SqlDbType.VarChar) { Value = stockNo },
			};

			var sql = $@"SELECT TOP(1) * FROM F010201
							      WHERE DC_CODE = @p0
							        AND GUP_CODE = @p1
							        AND CUST_CODE = @p2
							        AND (STOCK_NO = @p3 OR CUST_ORD_NO = @p3 OR CHECK_CODE = @p3)
							        AND STATUS <> '9'
							      ORDER BY CRT_DATE DESC ";

			return SqlQuery<F010201>(sql, param.ToArray()).SingleOrDefault();
			//var result = _db.F010201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			//                                     x.GUP_CODE == gupCode &&
			//                                     x.CUST_CODE == custCode &&
			//                                     (x.STOCK_NO == stockNo || x.CUST_ORD_NO == stockNo || x.CHECK_CODE == stockNo) &&
			//                                     x.STATUS != "9").OrderByDescending(x => x.CRT_DATE);

			//return result.FirstOrDefault();
		}
	}
}
