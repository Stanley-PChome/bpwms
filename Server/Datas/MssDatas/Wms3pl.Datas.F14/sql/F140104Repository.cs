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

namespace Wms3pl.Datas.F14
{
	public partial class F140104Repository : RepositoryBase<F140104, Wms3plDbContext, F140104Repository>
	{
		/// <summary>
		/// 計算查詢回來的盤點詳細的數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public int CountInventoryDetailItems(string dcCode, string gupCode, string custCode,
				string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, begLocCode, endLocCode };

			var sql = @"
                        SELECT Count(*)
                        FROM   F140104 A
                               INNER JOIN F1903 B
                                       ON B.GUP_CODE = A.GUP_CODE
                                          AND B.ITEM_CODE = A.ITEM_CODE
                                          AND B.CUST_CODE = A.CUST_CODE 
                               INNER JOIN F1980 C
                                       ON C.DC_CODE = A.DC_CODE
                                          AND C.WAREHOUSE_ID = A.WAREHOUSE_ID
                        WHERE  A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND A.INVENTORY_NO = @p3
                               AND A.LOC_CODE >= @p4
                               AND A.LOC_CODE <= @p5 
                        ";
			if (!string.IsNullOrEmpty(itemCode))
			{
				sql += " AND A.ITEM_CODE = @p" + param.Count;
				param.Add(itemCode);
			}

			if (!string.IsNullOrEmpty(wareHouseId))
			{
				sql += " AND A.WAREHOUSE_ID = @p" + param.Count;
				param.Add(wareHouseId);
			}
			return SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();
		}

		/// <summary>
		/// 盤點詳細查詢 只查前500筆
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="checkTool"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode,
				string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode, string checkTool)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, begLocCode, endLocCode };

			var sql = $@"
                SELECT TOP 500 E.*
                FROM   (
                  SELECT     ROW_NUMBER() OVER(ORDER BY C.WAREHOUSE_NAME, A.LOC_CODE, A.ITEM_CODE) AS ROWNUM ,
                             'N'                                                                   AS ChangeStatus,
                             A.LOC_CODE,
                             A.ITEM_CODE,
                             B.ITEM_NAME,
                             B.ITEM_SPEC,
                             B.ITEM_COLOR,
                             B.ITEM_SIZE,
                             A.VALID_DATE,
                             A.ENTER_DATE,
                             A.WAREHOUSE_ID,
                             C.WAREHOUSE_NAME,
                             A.MAKE_NO,
                             A.QTY,
                             A.FIRST_QTY AS FIRST_QTY_ORG,
                             A.FIRST_QTY,
                             A.SECOND_QTY AS SECOND_QTY_ORG,
                             A.SECOND_QTY,
                             A.FLUSHBACK AS FLUSHBACK_ORG,
                             A.FLUSHBACK,
                             CASE
                                        WHEN A.FLUSHBACK = '1' THEN '是'
                                        ELSE '否'
                             END AS FLUSHBACKNAME,
                             A.BOX_CTRL_NO,
                             A.PALLET_CTRL_NO,
							 A.UNMOVE_STOCK_QTY,
							 {(checkTool == "0" ? "NULL" : "A.DEVICE_STOCK_QTY")} DEVICE_STOCK_QTY,
							 B.CUST_ITEM_CODE,
							 B.EAN_CODE1,
							 B.EAN_CODE2,
							 B.EAN_CODE3 
                  FROM       F140104 A
                  INNER JOIN F1903 B
                  ON         B.GUP_CODE = A.GUP_CODE
                  AND        B.ITEM_CODE = A.ITEM_CODE
                  AND        B.CUST_CODE = A.CUST_CODE
                  INNER JOIN F1980 C
                  ON         C.DC_CODE = A.DC_CODE
                  AND        C.WAREHOUSE_ID = A.WAREHOUSE_ID
                  WHERE      A.DC_CODE = @p0
                  AND        A.GUP_CODE = @p1
                  AND        A.CUST_CODE = @p2
                  AND        A.INVENTORY_NO = @p3
                  AND        A.LOC_CODE >= @p4
                  AND        A.LOC_CODE <= @p5
                ";
			if (!string.IsNullOrEmpty(itemCode))
			{
				sql += " AND A.ITEM_CODE = @p" + param.Count;
				param.Add(itemCode);
			}

			if (!string.IsNullOrEmpty(wareHouseId))
			{
				sql += " AND A.WAREHOUSE_ID = @p" + param.Count;
				param.Add(wareHouseId);
			}

			sql += ") E  ";

			return SqlQuery<InventoryDetailItem>(sql, param.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="itemCodes"></param>
		/// <param name="differencesRangeStart">差異數區間 - 開始</param>
		/// <param name="differencesRangeEnd">差異數區間 - 結束</param>
		/// <param name="isRepotTag">是否為報表專用(報表用不用篩選條件)</param>
		/// <param name="isLimitStatusTag">是否要限制狀態只屬於F000904的F140102</param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItemsByIsSecond> GetInventoryDetailItemsByIsSecond(string dcCode, string gupCode, string custCode,
						string inventoryNo, string isSecond, string wareHouseId = "", string itemCodes = "", string differencesRangeStart = "", string differencesRangeEnd = "", string isRepotTag = "1", string isLimitStatusTag = "0", string showCnt = "0")
		{
			var param = new List<object> { DateTime.Now, dcCode, gupCode, custCode, inventoryNo };
			var isFirst = isSecond == "0";//是否為初盤
			var isRepot = isRepotTag == "1";//是否為報表用途
			var isLimitStatus = isLimitStatusTag == "1";//是否要判斷狀態屬於F000904的F140102(因為此處屬於共用)

			var showCntSql = string.Empty;
			if (showCnt == "1")
			{
				showCntSql = $@"
											  CAST(B.QTY AS NVARCHAR) AS QTY,
											  CAST(B.UNMOVE_STOCK_QTY AS NVARCHAR) AS UNMOVE_STOCK_QTY,
											";
			}
			else
			{
				showCntSql = $@"
											   CASE A.SHOW_CNT
											     WHEN '1' THEN CAST(B.QTY AS NVARCHAR)
											     ELSE ''
											   END AS QTY,
											   CASE A.SHOW_CNT
											     WHEN '1' THEN CAST(B.UNMOVE_STOCK_QTY AS NVARCHAR)
											     ELSE ''
											   END AS UNMOVE_STOCK_QTY,
											";
			}

			var sql = $@"
SELECT ROW_NUMBER() OVER(ORDER BY Z.DC_CODE,
                  Z.GUP_CODE,
                  Z.CUST_CODE,
                  Z.WAREHOUSE_ID,
                  Z.LOC_CODE,
                  Z.ITEM_CODE,
                  Z.VALID_DATE,
                  Z.ENTER_DATE) AS ROWNUM, Z.* FROM (
SELECT A.*,
       CASE A.ISSECOND WHEN '1' THEN ISNULL(A.SECOND_QTY, 0) ELSE A.FIRST_QTY END INVENTORY_QTY,																																						--初/複盤點數量
	     CASE A.ISSECOND WHEN '1' THEN A.DIFF_SECOND_QTY ELSE A.DIFF_FIRST_QTY END DIFF_QTY,																																									--初/複盤差數
	     CASE A.ISSECOND WHEN '1' THEN convert(datetime, A.SEC_INVENTORY_DATE, 120) ELSE convert(datetime, A.FST_INVENTORY_DATE, 120) END INVENTORY_DATE,											--初/複盤點日期
	     CASE A.ISSECOND WHEN '1' THEN A.SEC_INVENTORY_NAME ELSE A.FST_INVENTORY_NAME END INVENTORY_NAME,																																			--初/複盤點人員
			 CONVERT(VARCHAR,(CONVERT(int,CASE A.ISSECOND WHEN '1' THEN ISNULL(A.SECOND_QTY, 0) ELSE A.FIRST_QTY END) - A.QTY - ISNULL(UNMOVE_STOCK_QTY, 0) )) AS STOCK_DIFF_QTY	--初/複WMS庫差數
FROM   (SELECT A.DC_CODE,
							 A.GUP_CODE,
							 A.CUST_CODE,
							 A.INVENTORY_NO,
               (SELECT NAME
                FROM   VW_F000904_LANG
                WHERE  TOPIC = 'F140101'
                       AND SUBTOPIC = 'INVENTORY_TYPE'
                       AND VALUE = A.INVENTORY_TYPE
                       AND LANG = '{Current.Lang}')                           AS INVENTORY_TYPE_DESC,
               (SELECT NAME
                FROM   VW_F000904_LANG
                WHERE  TOPIC = 'F140101'
                       AND SUBTOPIC = 'STATUS'
                       AND VALUE = A.STATUS
                       AND LANG = '{Current.Lang}')                           AS STATUS_DESC ,
               (SELECT NAME
                FROM   VW_F000904_LANG
                WHERE  TOPIC = 'F1980'
                       AND SUBTOPIC = 'DEVICE_TYPE'
                       AND VALUE = A.CHECK_TOOL
                       AND LANG = '{Current.Lang}')                           AS CHECK_TOOL_NAME ,
               A.INVENTORY_TYPE,
               C.ITEM_NAME,
               B.ITEM_CODE,
               C.ITEM_SPEC,
               C.ITEM_SIZE,
               C.ITEM_COLOR,
               ISNULL(D.ACC_UNIT_NAME, '')                                  AS ITEM_UNIT,
               B.MAKE_NO,
               B.PALLET_CTRL_NO,
               B.BOX_CTRL_NO,
               CASE
                 WHEN B.PALLET_CTRL_NO = '0' THEN (
                 CASE
                   WHEN B.BOX_CTRL_NO = '0' THEN ''
                   ELSE
                 '(' + ISNULL(B.BOX_CTRL_NO, '') + ')'
                                                    END )
                 ELSE ( CASE
                          WHEN B.BOX_CTRL_NO = '0' THEN B.PALLET_CTRL_NO
                          ELSE ISNULL(B.PALLET_CTRL_NO, '') + '('
                               + ISNULL(B.BOX_CTRL_NO, '') + ')'
                        END )
               END                                                          AS PALLET_BOX_CTRL_NO,
               B.VALID_DATE,
               B.ENTER_DATE,
               B.WAREHOUSE_ID,
               (SELECT WAREHOUSE_NAME
                FROM   F1980
                WHERE  F1980.DC_CODE = B.DC_CODE
                       AND B.WAREHOUSE_ID = F1980.WAREHOUSE_ID)             AS WAREHOUSE_NAME,
               B.LOC_CODE,
                CAST(B.QTY AS NVARCHAR)                                AS QTY_Num,
				CAST(B.FIRST_QTY AS NVARCHAR)                          AS FIRST_QTY_Num,
				CAST(B.SECOND_QTY AS NVARCHAR)                         AS SECOND_QTY_Num,
				CAST(B.UNMOVE_STOCK_QTY AS NVARCHAR)                   AS UNMOVE_STOCK_QTY_Num,
								CAST(ISNULL(B.DEVICE_STOCK_QTY, 0) AS NVARCHAR) AS DEVICE_STOCK_QTY,
								{ showCntSql }
               {(isRepot ? " CAST(B.FIRST_QTY AS NVARCHAR) " : "CAST(ISNULL(B.FIRST_QTY, 0) AS NVARCHAR)")}                    AS FIRST_QTY,
{(isRepot ? "CASE B.FIRST_QTY WHEN NULL THEN '' ELSE CAST((B.FIRST_QTY - B.DEVICE_STOCK_QTY) AS NVARCHAR) END " :
					 " CAST((ISNULL(B.FIRST_QTY,0) - ISNULL(B.DEVICE_STOCK_QTY, 0)) AS NVARCHAR)")}   AS                                  DIFF_FIRST_QTY,
               {((isRepot || isFirst) ? " CAST(B.SECOND_QTY AS NVARCHAR)" : " CAST(ISNULL(B.SECOND_QTY,0) AS NVARCHAR)")} AS SECOND_QTY,
              {((isRepot || isFirst) ? "CASE B.SECOND_QTY WHEN NULL THEN '' ELSE CAST((B.SECOND_QTY - B.DEVICE_STOCK_QTY) AS NVARCHAR) END "
				: " CAST((ISNULL(B.SECOND_QTY,0) - ISNULL(B.DEVICE_STOCK_QTY, 0)) AS NVARCHAR)")} AS DIFF_SECOND_QTY,
               CONVERT(VARCHAR, B.FST_INVENTORY_DATE, 120) AS FST_INVENTORY_DATE,
               B.FST_INVENTORY_NAME,
               CONVERT(VARCHAR, B.SEC_INVENTORY_DATE, 120) AS SEC_INVENTORY_DATE,
               B.SEC_INVENTORY_NAME,
               B.FLUSHBACK,
               (SELECT DC_NAME
                FROM   F1901
                WHERE  DC_CODE = B.DC_CODE)                                 AS DC_NAME,
               ''                                                           AS PRINT_STAFF,
               @p0                                                          AS PRINT_DATE,
               ''                                                           AS PRINT_TITLE,
               CASE
                 WHEN G.UNIT_QTY IS NULL THEN ''
                 ELSE CAST( G.UNIT_QTY AS NVARCHAR)
               END                                                          AS UNIT_QTY,
               A.SHOW_CNT,
				A.ISSECOND,
				C.CUST_ITEM_CODE ,
				C.EAN_CODE1 ,
				C.EAN_CODE2 ,
				C.EAN_CODE3 
        FROM   F140101 A
LEFT JOIN {(isFirst ? "F140104" : "F140105")} B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE AND B.INVENTORY_NO = A.INVENTORY_NO
               LEFT JOIN F1903 C ON C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = A.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
               INNER JOIN F91000302 D ON D.ACC_UNIT = C.ITEM_UNIT AND D.ITEM_TYPE_ID = '001'
			   LEFT JOIN
               (SELECT E.GUP_CODE,
                       E.ITEM_CODE,
                       E.UNIT_ID,
                       E.UNIT_QTY,
                       E.CUST_CODE
                FROM   F190301 E
                       INNER JOIN (SELECT GUP_CODE,
                                          ITEM_CODE,
                                          MAX(UNIT_ID) UNIT_ID,
                                          CUST_CODE
                                   FROM   F190301
                                   GROUP  BY GUP_CODE,
                                             ITEM_CODE,
                                             CUST_CODE) F
                               ON F.GUP_CODE = E.GUP_CODE
                                  AND F.ITEM_CODE = E.ITEM_CODE
                                  AND F.UNIT_ID = E.UNIT_ID
                                  AND F.CUST_CODE = E.CUST_CODE) G 
                                  ON G.GUP_CODE = B.GUP_CODE 
                                  AND G.ITEM_CODE = B.ITEM_CODE
                                  AND G.CUST_CODE = B.CUST_CODE
                WHERE	 A.DC_CODE = @p1
                       AND A.GUP_CODE = @p2
                       AND A.CUST_CODE = @p3
                       AND A.INVENTORY_NO = @p4
                       {(isLimitStatus ? $@"AND (A.STATUS IN (SELECT VALUE
										 FROM VW_F000904_LANG
										 WHERE TOPIC = 'F140102' AND SUBTOPIC = 'STATUS' AND LANG = '{Current.Lang}'))" : string.Empty)}
                
                ";

			var sqlTmp = string.Empty;

			if (isRepot == false)//非報表預覽列印，則使用此判斷
			{
				if (!string.IsNullOrEmpty(wareHouseId))
				{
					sql += " AND B.WAREHOUSE_ID = @p" + param.Count;
					param.Add(wareHouseId);
				}
				if (!string.IsNullOrEmpty(itemCodes))
				{
					var itemCodeList = itemCodes.Contains('^')
							? itemCodes.Split('^').Where(itemCode => !string.IsNullOrWhiteSpace(itemCode)).ToList()
							: new List<string>() { itemCodes };
					var inSql = param.CombineSqlInParameters("B.ITEM_CODE", itemCodeList);
					sql += " AND " + inSql;
				}
				if (!string.IsNullOrWhiteSpace(differencesRangeStart) && !string.IsNullOrWhiteSpace(differencesRangeEnd))
				{
					sqlTmp += $" WHERE (ABS(ISNULL(Z.DIFF_QTY,0)) >= @p{param.Count} ";
					param.Add(Convert.ToInt32(differencesRangeStart));
					sqlTmp += $" AND ABS(ISNULL(Z.DIFF_QTY,0)) <= @p{param.Count} ) ";
					param.Add(Convert.ToInt32(differencesRangeEnd));
					sqlTmp += $" OR (ABS(ISNULL(Z.STOCK_DIFF_QTY,0)) >= @p{param.Count} ";
					param.Add(Convert.ToInt32(differencesRangeStart));
					sqlTmp += $" AND ABS(ISNULL(Z.STOCK_DIFF_QTY,0)) <= @p{param.Count} )";
					param.Add(Convert.ToInt32(differencesRangeEnd));
				}
				else
				{
					sqlTmp += " WHERE (ISNULL(Z.DIFF_QTY, 0) <> 0) OR (ISNULL(Z.STOCK_DIFF_QTY, 0) <> 0 ) ";
				}
			}
			else
			{
				//報表預覽不用篩選條件
			}
			sql = $"{sql} ) A ) Z {(string.IsNullOrWhiteSpace(sqlTmp) ? string.Empty : sqlTmp)}";
			var result = SqlQuery<InventoryDetailItemsByIsSecond>(sql, param.ToArray());
			return result;
		}

		public IQueryable<P140102ReportData> GetP140102ReportData(string dcCode, string gupCode, string custCode, string inventoryNo, string isSecond)
		{
			var param = new List<object> { DateTime.Now, dcCode, gupCode, custCode, inventoryNo };

			string sql = string.Empty;
			if (isSecond == "0")
			{
				sql = $@"
			SELECT CAST(ROW_NUMBER() OVER(ORDER BY Z.DC_CODE,
			                  Z.GUP_CODE,
			                  Z.CUST_CODE,
			                  Z.WAREHOUSE_ID,
			                  Z.LOC_CODE,
			                  Z.ITEM_CODE,
			                  Z.VALID_DATE,
			                  Z.ENTER_DATE) AS NVARCHAR) AS ROWNUM, Z.* FROM (
			SELECT A.*
					FROM   (SELECT 
					A.DC_CODE,
					A.GUP_CODE,
					A.CUST_CODE,
					E.DC_NAME,
					A.INVENTORY_NO,
					CONVERT(VARCHAR, A.INVENTORY_DATE, 111) INVENTORY_DATE,
					D.NAME INVENTORY_TYPE_DESC,
					B.WAREHOUSE_ID,
					REPLACE(CONVERT(VARCHAR, @p0, 120), '-', '/') PRINT_DATE,
					B.ITEM_CODE,
					C.CUST_ITEM_CODE,
					C.EAN_CODE1,
					C.EAN_CODE2,
					C.EAN_CODE3,
					C.ITEM_NAME,
					CONVERT(VARCHAR, B.VALID_DATE, 111) VALID_DATE,
					CONVERT(VARCHAR, B.ENTER_DATE, 111) ENTER_DATE,
					B.MAKE_NO,
					G.WAREHOUSE_NAME,
					B.LOC_CODE,
					CAST(B.DEVICE_STOCK_QTY AS NVARCHAR) FST_DEVICE_STOCK_QTY,
					CAST(B.FIRST_QTY AS NVARCHAR) FST_QTY,
					CASE B.FIRST_QTY WHEN NULL THEN '' ELSE CAST((B.FIRST_QTY - B.DEVICE_STOCK_QTY) AS NVARCHAR) END FST_DIFF_FIRST_QTY,
					CAST(B.QTY AS NVARCHAR) FST_STOCK_QTY,
					CAST(B.UNMOVE_STOCK_QTY AS NVARCHAR) FST_UNMOVE_STOCK_QTY,
					CONVERT(VARCHAR,(CONVERT(int,B.FIRST_QTY) - B.QTY - ISNULL(B.UNMOVE_STOCK_QTY, 0) )) FST_STOCK_DIFF_QTY,
					B.FST_INVENTORY_NAME,
					REPLACE(CONVERT(VARCHAR, B.FST_INVENTORY_DATE, 120), '-', '/') FST_INVENTORY_DATE,
					F.NAME CHECK_TOOL_NAME
			        FROM   F140101 A
					JOIN F1901 E ON A.DC_CODE = E.DC_CODE
					JOIN VW_F000904_LANG D ON D.TOPIC = 'F140101' AND D.SUBTOPIC = 'INVENTORY_TYPE' AND D.VALUE = A.INVENTORY_TYPE AND D.LANG = '{Current.Lang}'
					JOIN VW_F000904_LANG F ON F.TOPIC = 'F1980' AND F.SUBTOPIC = 'DEVICE_TYPE' AND F.VALUE = A.CHECK_TOOL AND F.LANG = '{Current.Lang}'
					JOIN F140104 B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE AND B.INVENTORY_NO = A.INVENTORY_NO
					JOIN F1980 G ON B.DC_CODE = G.DC_CODE AND B.WAREHOUSE_ID = G.WAREHOUSE_ID
					JOIN F1903 C ON C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = A.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
					 WHERE	 A.DC_CODE = @p1
					        AND A.GUP_CODE = @p2
					        AND A.CUST_CODE = @p3
					        AND A.INVENTORY_NO = @p4
					 ) A ) Z
			";
			}
			else
			{
				sql = $@" SELECT CAST(ROW_NUMBER() OVER(ORDER BY Z.DC_CODE,
			                  Z.GUP_CODE,
			                  Z.CUST_CODE,
			                  Z.WAREHOUSE_ID,
			                  Z.LOC_CODE,
			                  Z.ITEM_CODE,
			                  Z.VALID_DATE,
			                  Z.ENTER_DATE) AS NVARCHAR) AS ROWNUM, Z.* FROM (
									SELECT A.*
											FROM   (SELECT 
											A.DC_CODE,
											A.GUP_CODE,
											A.CUST_CODE,
											E.DC_NAME,
											A.INVENTORY_NO,
											CONVERT(VARCHAR, A.INVENTORY_DATE, 111) INVENTORY_DATE,
											D.NAME INVENTORY_TYPE_DESC,
											B.WAREHOUSE_ID,
											REPLACE(CONVERT(VARCHAR, @p0, 120), '-', '/') PRINT_DATE,
											B.ITEM_CODE,
											C.CUST_ITEM_CODE,
											C.EAN_CODE1,
											C.EAN_CODE2,
											C.EAN_CODE3,
											C.ITEM_NAME,
											CONVERT(VARCHAR, B.VALID_DATE, 111) VALID_DATE,
											CONVERT(VARCHAR, B.ENTER_DATE, 111) ENTER_DATE,
											B.MAKE_NO,
											G.WAREHOUSE_NAME,
											B.LOC_CODE,
											CAST(F.DEVICE_STOCK_QTY AS NVARCHAR) FST_DEVICE_STOCK_QTY,
											CAST(B.DEVICE_STOCK_QTY AS NVARCHAR) SEC_DEVICE_STOCK_QTY,
											CAST(F.FIRST_QTY AS NVARCHAR) FST_QTY,
											CAST(B.SECOND_QTY AS NVARCHAR) SEC_QTY,
											CASE F.FIRST_QTY WHEN NULL THEN '' ELSE CAST((F.FIRST_QTY - F.DEVICE_STOCK_QTY) AS NVARCHAR) END FST_DIFF_FIRST_QTY,
											CASE B.SECOND_QTY WHEN NULL THEN '' ELSE CAST((B.SECOND_QTY - B.DEVICE_STOCK_QTY) AS NVARCHAR) END SEC_DIFF_SECOND_QTY,
											CAST(F.QTY AS NVARCHAR) FST_STOCK_QTY,
											CAST(B.QTY AS NVARCHAR) SEC_STOCK_QTY,
											CAST(F.UNMOVE_STOCK_QTY AS NVARCHAR) FST_UNMOVE_STOCK_QTY,
											CAST(B.UNMOVE_STOCK_QTY AS NVARCHAR) SEC_UNMOVE_STOCK_QTY,
											CONVERT(VARCHAR,(CONVERT(int,F.FIRST_QTY) - F.QTY - ISNULL(F.UNMOVE_STOCK_QTY, 0) )) FST_STOCK_DIFF_QTY,
											CONVERT(VARCHAR,(CONVERT(int,B.SECOND_QTY) - B.QTY - ISNULL(B.UNMOVE_STOCK_QTY, 0) )) SEC_STOCK_DIFF_QTY,
											F.FST_INVENTORY_NAME,
											B.SEC_INVENTORY_NAME,
											REPLACE(CONVERT(VARCHAR, F.FST_INVENTORY_DATE, 120), '-', '/') FST_INVENTORY_DATE,
											REPLACE(CONVERT(VARCHAR, B.SEC_INVENTORY_DATE, 120), '-', '/') SEC_INVENTORY_DATE,
											H.NAME CHECK_TOOL_NAME
									        FROM   F140101 A
											JOIN F1901 E ON A.DC_CODE = E.DC_CODE
											JOIN VW_F000904_LANG D ON D.TOPIC = 'F140101' AND D.SUBTOPIC = 'INVENTORY_TYPE' AND D.VALUE = A.INVENTORY_TYPE AND D.LANG = '{Current.Lang}'
											JOIN VW_F000904_LANG H ON H.TOPIC = 'F1980' AND H.SUBTOPIC = 'DEVICE_TYPE' AND H.VALUE = A.CHECK_TOOL AND H.LANG = '{Current.Lang}'
											JOIN F140105 B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE AND B.INVENTORY_NO = A.INVENTORY_NO
											LEFT JOIN F140104 F ON F.DC_CODE = B.DC_CODE AND F.GUP_CODE = B.GUP_CODE AND F.CUST_CODE = B.CUST_CODE AND F.INVENTORY_NO = B.INVENTORY_NO AND F.LOC_CODE = B.LOC_CODE AND F.ITEM_CODE = B.ITEM_CODE AND F.VALID_DATE = B.VALID_DATE AND F.ENTER_DATE = B.ENTER_DATE AND F.BOX_CTRL_NO = B.BOX_CTRL_NO AND F.PALLET_CTRL_NO = B.PALLET_CTRL_NO AND F.MAKE_NO = B.MAKE_NO
											JOIN F1980 G ON B.DC_CODE = G.DC_CODE AND B.WAREHOUSE_ID = G.WAREHOUSE_ID
											JOIN F1903 C ON C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = A.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
											
											 WHERE	 A.DC_CODE = @p0
											        AND A.GUP_CODE = @p1
											        AND A.CUST_CODE = @p2
											        AND A.INVENTORY_NO = @p3
											 ) A ) Z
												";
			}

			var result = SqlQuery<P140102ReportData>(sql, param.ToArray());
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="locCode"></param>
		/// <param name="isSecond"></param>
		/// <returns></returns>
		public InventoryScanLoc GetInventoryLoc(string dcCode, string gupCode, string custCode, string inventoryNo,
				string locCode, string isSecond)
		{

			var sql = $@"
                        SELECT A.*
                        FROM   (SELECT A.LOC_CODE,
                                       C.WAREHOUSE_ID,
                                       C.WAREHOUSE_NAME,
                                       Count(DISTINCT ITEM_CODE) AS TOTAL_CNT,
                                       Sum(QTY)                  AS TOTAL_QTY
                                FROM   {(isSecond == "1" ? "F140105" : "F140104")} A
                                       INNER JOIN F1912 B
                                               ON B.DC_CODE = A.DC_CODE
                                                  AND B.LOC_CODE = A.LOC_CODE
                                       INNER JOIN F1980 C
                                               ON C.DC_CODE = B.DC_CODE
                                                  AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
                                WHERE  A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.INVENTORY_NO = @p3
                                       AND A.LOC_CODE = @p4
                                GROUP  BY A.DC_CODE,
                                          A.GUP_CODE,
                                          A.CUST_CODE,
                                          A.INVENTORY_NO,
                                          A.LOC_CODE,
                                          C.WAREHOUSE_ID,
                                          C.WAREHOUSE_NAME) A 
                        ";
			var param = new object[] { dcCode, gupCode, custCode, inventoryNo, locCode };
			return SqlQuery<InventoryScanLoc>(sql, param).FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public InventoryScanItem GetInventoryScanItem(string dcCode, string gupCode, string custCode, string inventoryNo,
				string isSecond, string locCode, string itemCode)
		{

			var sql = $@"
                        SELECT ROW_NUMBER() OVER(ORDER BY  A.ITEM_CODE,
                                                                   B.ITEM_NAME,
                                                                   B.ITEM_SIZE,
                                                                   B.ITEM_COLOR,
                                                                   B.ITEM_SPEC,
                                                                   D.ACC_UNIT_NAME,
			                                                       ISNULL(B.BUNDLE_SERIALLOC, '0')) AS ROWNUM,
                                       A.ITEM_CODE,
                                       B.ITEM_NAME,
                                       B.ITEM_SIZE,
                                       B.ITEM_COLOR,
                                       B.ITEM_SPEC,
                                       D.ACC_UNIT_NAME                 as ITEM_UNIT,
                                       ISNULL(B.BUNDLE_SERIALLOC, '0') BUNDLE_SERIALLOC,
                                       Sum(A.QTY)                      AS INVENTORY_QTY
                                FROM   {(isSecond == "1" ? "F140105" : "F140104")} A
                                       INNER JOIN F1903 B
                                               ON B.GUP_CODE = A.GUP_CODE
                                                  AND B.ITEM_CODE = A.ITEM_CODE
                                                  AND B.CUST_CODE = A.CUST_CODE
                                       LEFT JOIN F91000302 D
                                              ON D.ITEM_TYPE_ID = '001'
                                                 AND D.ACC_UNIT = B.ITEM_UNIT
                                WHERE  A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.INVENTORY_NO = @p3
                                       AND A.LOC_CODE = @p4
                                       AND A.ITEM_CODE = @p5
                                GROUP  BY A.ITEM_CODE,
                                          B.ITEM_NAME,
                                          B.ITEM_SIZE,
                                          B.ITEM_COLOR,
                                          B.ITEM_SPEC,
                                          D.ACC_UNIT_NAME,
                                          ISNULL(B.BUNDLE_SERIALLOC, '0') 
                        ";

			var param = new object[] { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode };
			return SqlQuery<InventoryScanItem>(sql, param).FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		public IQueryable<InventoryDiffLocItemQty> GetInventoryDiffLocItemQties(string dcCode, string gupCode, string custCode,
string inventoryNo)
		{
			var param = new object[] { dcCode, gupCode, custCode, inventoryNo };
			var sql =
					@"
                    SELECT ROW_NUMBER() OVER(ORDER BY A.INVENTORY_NO) 'ROWNUM',
                           A.DC_CODE,
                           A.GUP_CODE,
                           A.CUST_CODE,
                           A.INVENTORY_NO,
                           A.WAREHOUSE_ID,
                           A.LOC_CODE,
                           A.ITEM_CODE,
                           A.QTY,
                           CASE
                             WHEN A.DIFFQTY >= 0 THEN '0'
                             ELSE '1'
                           END                           AS WORK_TYPE,
                           CASE
                             WHEN A.DIFFQTY > 0 THEN A.DIFFQTY
                             ELSE 0
                           END                           AS ADJ_QTY_IN,
                           CASE
                             WHEN A.DIFFQTY < 0 THEN A.DIFFQTY
                             ELSE 0
                           END                           AS ADJ_QTY_OUT,
                           A.QTY + A.DIFFQTY             AS INVENTORY_QTY,
                           ISNULL(B.BUNDLE_SERIALLOC, 0) AS BUNDLE_SERIALLOC
                    FROM   (SELECT A.DC_CODE,
                                   A.GUP_CODE,
                                   A.CUST_CODE,
                                   A.INVENTORY_NO,
                                   A.WAREHOUSE_ID,
                                   A.LOC_CODE,
                                   A.ITEM_CODE,
                                   Sum(A.QTY)     QTY,
                                   Sum(A.DIFFQTY) AS DIFFQTY
                            FROM   (SELECT A.DC_CODE,
                                           A.GUP_CODE,
                                           A.CUST_CODE,
                                           A.INVENTORY_NO,
                                           A.WAREHOUSE_ID,
                                           A.LOC_CODE,
                                           A.ITEM_CODE,
                                           A.VALID_DATE,
                                           A.ENTER_DATE,
                                           A.QTY,
                                           ISNULL(B.FIRST_QTY, A.FIRST_QTY)
                                                   FIRST_QTY,
                                           ISNULL(B.SECOND_QTY, A.SECOND_QTY)
                                                   AS SECOND_QTY,
                           ISNULL(B.SECOND_QTY, ISNULL(A.SECOND_QTY, ISNULL(A.FIRST_QTY, 0))) -
                           A.QTY AS
                           DIFFQTY
                           FROM   F140104 A
                           LEFT JOIN F140105 B
                                  ON B.DC_CODE = A.DC_CODE
                                     AND B.GUP_CODE = A.GUP_CODE
                                     AND B.CUST_CODE = A.CUST_CODE
                                     AND B.INVENTORY_NO = A.INVENTORY_NO
                                     AND B.LOC_CODE = A.LOC_CODE
                                     AND B.ITEM_CODE = A.ITEM_CODE
                                     AND B.VALID_DATE = A.VALID_DATE
                                     AND B.ENTER_DATE = A.ENTER_DATE) A
                            GROUP  BY A.DC_CODE,
                                      A.GUP_CODE,
                                      A.CUST_CODE,
                                      A.INVENTORY_NO,
                                      A.WAREHOUSE_ID,
                                      A.LOC_CODE,
                                      A.ITEM_CODE
                            HAVING Sum(A.DIFFQTY) <> 0) A
                           INNER JOIN F1903 B
                                   ON B.GUP_CODE = A.GUP_CODE
                                      AND B.CUST_CODE = A.CUST_CODE
                                      AND B.ITEM_CODE = A.ITEM_CODE
                    WHERE  A.DC_CODE = @p0
                           AND A.GUP_CODE = @p1
                           AND A.CUST_CODE = @p2
                           AND A.INVENTORY_NO = @p3
                    ";
			return SqlQuery<InventoryDiffLocItemQty>(sql, param);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <returns></returns>
		public IQueryable<InventoryByLocDetail> GetInventoryByLocDetails(string dcCode, string gupCode, string custCode,
				string inventoryNo, string isSecond)
		{
			var isFirst = isSecond == "0";//是否為初盤
			var param = new List<object> { DateTime.Now, dcCode, gupCode, custCode, inventoryNo };

			var sql = $@"
                        SELECT ROW_NUMBER()
                                 OVER(
                                   ORDER BY B.DC_CODE, B.GUP_CODE, B.CUST_CODE, A.INVENTORY_NO,
                                 A.INVENTORY_DATE, A.INVENTORY_TYPE, B.WAREHOUSE_ID, B.LOC_CODE,
                                 B.ITEM_CODE,
                                 A.SHOW_CNT)                                    AS ROWNUM,
                               A.INVENTORY_NO,
                               A.INVENTORY_DATE,
                               (SELECT NAME
                                FROM   VW_F000904_LANG
                                WHERE  TOPIC = 'F140101'
                                       AND SUBTOPIC = 'INVENTORY_TYPE'
                                       AND VALUE = A.INVENTORY_TYPE
                                       AND LANG = '{Current.Lang}')                      INVENTORY_TYPE_DESC,
                               (SELECT NAME
                                FROM   VW_F000904_LANG
                                WHERE  TOPIC = 'F140101'
                                       AND SUBTOPIC = 'STATUS'
                                       AND VALUE = A.STATUS
                                       AND LANG = '{Current.Lang}')                      STATUS_DESC,
							   (SELECT NAME
							    FROM   VW_F000904_LANG
							    WHERE  TOPIC = 'F1980'
							           AND SUBTOPIC = 'DEVICE_TYPE'
							           AND VALUE = A.CHECK_TOOL
							           AND LANG = '{Current.Lang}')						 CHECK_TOOL_NAME ,
                               A.INVENTORY_TYPE,
                               B.WAREHOUSE_ID,
                               (SELECT WAREHOUSE_NAME
                                FROM   F1980
                                WHERE  F1980.DC_CODE = B.DC_CODE
                                       AND B.WAREHOUSE_ID = F1980.WAREHOUSE_ID) AS WAREHOUSE_NAME,
                               B.LOC_CODE,
                               B.ITEM_CODE,
                               C.ITEM_NAME,
								C.EAN_CODE1,
								C.EAN_CODE2,
								C.EAN_CODE3,
								C.CUST_ITEM_CODE,
                               ISNULL(D.ACC_UNIT_NAME, '')                      AS ITEM_UNIT,
                               CASE
                                 WHEN G.UNIT_QTY IS NULL THEN ''
                                 ELSE Cast(G.UNIT_QTY AS NVARCHAR)
                               END                                              UNIT_QTY,
                               CASE A.SHOW_CNT
                                 WHEN '1' THEN Cast(Sum(B.QTY) AS NVARCHAR)
                                 ELSE ''
                               END                                              AS QTY,
							   CASE A.SHOW_CNT
                                 WHEN '1' THEN Cast(Sum(B.UNMOVE_STOCK_QTY) AS NVARCHAR)
                                 ELSE ''
                               END                                              AS UNMOVE_STOCK_QTY,
                               Sum(B.QTY)                                       QTY_Num,
								Sum(B.FIRST_QTY)								FIRST_QTY_Num,
								Sum(B.SECOND_QTY)								SECOND_QTY_Num,
								Sum(B.UNMOVE_STOCK_QTY)							UNMOVE_STOCK_QTY_Num,
                               Cast(Sum(B.FIRST_QTY) AS NVARCHAR)               AS FIRST_QTY,
                               Cast(Sum(B.SECOND_QTY) AS NVARCHAR)              AS SECOND_QTY,
                               (SELECT DC_NAME
                                FROM   F1901
                                WHERE  DC_CODE = B.DC_CODE)                     AS DC_NAME,
                               ''                                               AS PRINT_STAFF,
                               @p0                                              AS PRINT_DATE,
                               ''                                               AS PRINT_TITLE,
                               A.SHOW_CNT,
                               B.MAKE_NO,
                               CASE
                                 WHEN B.PALLET_CTRL_NO = '0' THEN ( CASE
                                                                      WHEN B.BOX_CTRL_NO = '0' THEN ''
                                                                      ELSE
                                 '(' + ISNULL(B.BOX_CTRL_NO, '') + ')'
                                                                    END )
                                 ELSE ( CASE
                                          WHEN B.BOX_CTRL_NO = '0' THEN B.PALLET_CTRL_NO
                                          ELSE ISNULL(B.PALLET_CTRL_NO, '') + '('
                                               + ISNULL(B.BOX_CTRL_NO, '') + ')'
                                        END )
                               END                                              AS PALLET_BOX_CTRL_NO
                        FROM   F140101 A
                               LEFT JOIN {(isFirst ? "F140104" : "F140105")} B
                                      ON B.DC_CODE = A.DC_CODE
                                         AND B.GUP_CODE = A.GUP_CODE
                                         AND B.CUST_CODE = A.CUST_CODE
                                         AND B.INVENTORY_NO = A.INVENTORY_NO
                               INNER JOIN F1903 C
                                       ON C.ITEM_CODE = B.ITEM_CODE
                                          AND C.GUP_CODE = B.GUP_CODE
                                          AND C.CUST_CODE = A.CUST_CODE
                               INNER JOIN F91000302 D
                                       ON D.ITEM_TYPE_ID = '001'
                                          AND D.ACC_UNIT = C.ITEM_UNIT
                               LEFT JOIN (SELECT E.GUP_CODE,
                                                 E.ITEM_CODE,
                                                 E.UNIT_ID,
                                                 E.UNIT_QTY,
                                                 E.CUST_CODE
                                          FROM   F190301 E
                                                 INNER JOIN (SELECT GUP_CODE,
                                                                    ITEM_CODE,
                                                                    Max(UNIT_ID) UNIT_ID,
                                                                    CUST_CODE
                                                             FROM   F190301
                                                             GROUP  BY GUP_CODE,
                                                                       ITEM_CODE,
                                                                       CUST_CODE) F
                                                         ON F.GUP_CODE = E.GUP_CODE
                                                            AND F.ITEM_CODE = E.ITEM_CODE
                                                            AND F.UNIT_ID = E.UNIT_ID
                                                             AND F.CUST_CODE = E.CUST_CODE) G
                                      ON G.GUP_CODE = B.GUP_CODE
                                         AND G.ITEM_CODE = B.ITEM_CODE
                                           AND G.CUST_CODE = B.CUST_CODE
                        WHERE  A.DC_CODE = @p1
                               AND A.GUP_CODE = @p2
                               AND A.CUST_CODE = @p3
                               AND A.INVENTORY_NO = @p4
                        GROUP  BY B.DC_CODE,
                                  B.GUP_CODE,
                                  B.CUST_CODE,
                                  A.INVENTORY_NO,
                                  A.INVENTORY_DATE,
                                  A.INVENTORY_TYPE,
                                  B.WAREHOUSE_ID,
                                  B.LOC_CODE,
                                  B.ITEM_CODE,
                                  C.ITEM_NAME,
                                  D.ACC_UNIT_NAME,
                                  G.UNIT_QTY,
                                  A.SHOW_CNT,
                                  A.STATUS,
                                  B.MAKE_NO,
                                  B.PALLET_CTRL_NO,
                                  B.BOX_CTRL_NO,
								  C.CUST_ITEM_CODE,
                                  C.EAN_CODE1,
                                  C.EAN_CODE2,
                                  C.EAN_CODE3,
                                  A.CHECK_TOOL 
                        ";
			return SqlQuery<InventoryByLocDetail>(sql, param.ToArray());

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode, string inventoryNo, bool isSecond, List<string> itemCodes)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo };
			var sql = $@"
                        SELECT ROW_NUMBER() OVER(ORDER BY A.INVENTORY_NO) 'ROWNUM',
                                'N'          as ChangeStatus,
                               A.LOC_CODE,
                               A.ITEM_CODE,
                               B.ITEM_NAME,
                               B.ITEM_SPEC,
                               B.ITEM_COLOR,
                               B.ITEM_SIZE,
                               A.MAKE_NO,
                               A.VALID_DATE,
                               A.ENTER_DATE,
                               A.WAREHOUSE_ID,
                               C.WAREHOUSE_NAME,
                               +A.QTY,
                               A.FIRST_QTY  as FIRST_QTY_ORG,
                               A.FIRST_QTY,
                               A.SECOND_QTY AS SECOND_QTY_ORG,
                               A.SECOND_QTY,
                               A.FLUSHBACK  AS FLUSHBACK_ORG,
                               A.FLUSHBACK,
                               CASE
                                 WHEN A.FLUSHBACK = '1' THEN '是'
                                 ELSE '否'
                               END          AS FLUSHBACKNAME,
                               A.BOX_CTRL_NO,
                               A.PALLET_CTRL_NO,
															 A.UNMOVE_STOCK_QTY,
								B.CUST_ITEM_CODE,
								B.EAN_CODE1,
								B.EAN_CODE2,
								B.EAN_CODE3
                        FROM   {(isSecond ? "F140105" : "F140104")} A
                               INNER JOIN F1903 B
                                       ON B.GUP_CODE = A.GUP_CODE
                                       AND B.ITEM_CODE = A.ITEM_CODE
                                       AND B.CUST_CODE = A.CUST_CODE
                               INNER JOIN F1980 C
                                       ON C.DC_CODE = A.DC_CODE
                                          AND C.WAREHOUSE_ID = A.WAREHOUSE_ID
                        WHERE  A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND A.INVENTORY_NO = @p3
       
                        ";

			sql += param.CombineSqlInParameters("AND A.ITEM_CODE ", itemCodes);
			return SqlQuery<InventoryDetailItem>(sql, param.ToArray());
		}

		/// <summary>
		/// 盤點詳細查詢 匯出Excel
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItemsExport(string dcCode, string gupCode, string custCode,
				string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode, string checkTool)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, begLocCode, endLocCode };

			var sql = $@"
                        SELECT 'N'          as ChangeStatus,
	                        ROW_NUMBER()
                                                         OVER(
                                                           ORDER BY C.WAREHOUSE_NAME,
                                          A.LOC_CODE,
                                          A.ITEM_CODE)                                    AS ROWNUM,
                                       A.LOC_CODE,
                                       A.ITEM_CODE,
                                       B.ITEM_NAME,
                                       B.ITEM_SPEC,
                                       B.ITEM_COLOR,
                                       B.ITEM_SIZE,
                                       A.MAKE_NO,
                                       A.VALID_DATE,
                                       A.ENTER_DATE,
                                       A.WAREHOUSE_ID,
                                       C.WAREHOUSE_NAME,
                                       A.QTY,
                                       A.FIRST_QTY  as FIRST_QTY_ORG,
                                       A.FIRST_QTY,
                                       A.SECOND_QTY AS SECOND_QTY_ORG,
                                       A.SECOND_QTY,
                                       A.FLUSHBACK  AS FLUSHBACK_ORG,
                                       A.FLUSHBACK,
                                       CASE
                                         WHEN A.FLUSHBACK = '1' THEN '是'
                                         ELSE '否'
                                       END          AS FLUSHBACKNAME,
                                       A.BOX_CTRL_NO,
                                       A.PALLET_CTRL_NO,
                                       A.UNMOVE_STOCK_QTY,
										{(checkTool == "0" ? "NULL" : "A.DEVICE_STOCK_QTY")} DEVICE_STOCK_QTY,
                                       B.CUST_ITEM_CODE,
                                       B.EAN_CODE1,
                                       B.EAN_CODE2,
                                       B.EAN_CODE3
                                FROM   F140104 A
                                       INNER JOIN F1903 B
                                               ON B.GUP_CODE = A.GUP_CODE
                                                AND B.ITEM_CODE = A.ITEM_CODE
                                                AND B.CUST_CODE = A.CUST_CODE
                                       INNER JOIN F1980 C
                                               ON C.DC_CODE = A.DC_CODE
                                                  AND C.WAREHOUSE_ID = A.WAREHOUSE_ID
                                WHERE  A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.INVENTORY_NO = @p3
                                       AND A.LOC_CODE >= @p4
                                       AND A.LOC_CODE <= @p5
                        ";
			if (!string.IsNullOrEmpty(itemCode))
			{
				sql += " AND A.ITEM_CODE = @p" + param.Count;
				param.Add(itemCode);
			}

			if (!string.IsNullOrEmpty(wareHouseId))
			{
				sql += " AND A.WAREHOUSE_ID = @p" + param.Count;
				param.Add(wareHouseId);
			}

			return SqlQuery<InventoryDetailItem>(sql, param.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="makeNo"></param>
		public void DeleteF140104(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
				string itemCode, DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			List<object> param = new List<object>() { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, validDate, enterDate, boxCtrlNo, palletCtrlNo };

			var sql = @" 
                        DELETE FROM F140104
                        WHERE  DC_CODE = @p0
                               AND GUP_CODE = @p1
                               AND CUST_CODE = @p2
                               AND INVENTORY_NO = @p3
                               AND LOC_CODE = @p4
                               AND ITEM_CODE = @p5
                               AND VALID_DATE = @p6
                               AND ENTER_DATE = @p7
                               AND BOX_CTRL_NO = @p8
                               AND PALLET_CTRL_NO = @p9 
                        ";

			if (!string.IsNullOrWhiteSpace(makeNo))
			{
				param.Add(makeNo);
				sql = string.Format("{0} {1}", sql, " AND MAKE_NO = @p10 ");
			}
			else
			{
				sql = string.Format("{0} {1}", sql, " AND MAKE_NO = '0' ");
			}

			ExecuteSqlCommand(sql, param.ToArray());
		}

    public IQueryable<F140104> GetDatasByWcsInventoryNos(string dcCode, string gupCode, string custCode, List<string> inventoryNos)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"SELECT * FROM F140104 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2";
      sql += para.CombineSqlInParameters(" AND INVENTORY_NO", inventoryNos, SqlDbType.VarChar);
      return SqlQuery<F140104>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      return _db.F140104s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      inventoryNos.Contains(x.INVENTORY_NO));
      */
      #endregion
    }

  }
}
