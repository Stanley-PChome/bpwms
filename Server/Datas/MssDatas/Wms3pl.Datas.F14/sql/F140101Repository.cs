using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140101Repository : RepositoryBase<F140101, Wms3plDbContext, F140101Repository>
	{
        /// <summary>
		/// 取得盤點單主檔
		/// </summary>
		/// <param name="dcCode">DC_CODE</param>
		/// <param name="gupCode">GUP_CODE</param>
		/// <param name="custCode">CUST_CODE</param>
		/// <param name="inventoryNo">盤點單號</param>
		/// <param name="inventoryType"></param>
		/// <param name="inventorySDate">盤點單日期-起</param>
		/// <param name="inventoryEDate">盤點單日期-訖</param>
		/// <param name="inventoryCycle">盤點週期(0~6)</param>
		/// <param name="inventoryYear">盤點年分</param>
		/// <param name="inventoryMonth">盤點月份</param>
		/// <param name="status">盤點單狀態(0待處理、1初盤、2複盤、3已確認、5過帳、9取消)</param>
		/// <returns></returns>
		/// <summary>
		/// 取得盤點單主檔
		/// </summary>
		/// <param name="dcCode">DC_CODE</param>
		/// <param name="gupCode">GUP_CODE</param>
		/// <param name="custCode">CUST_CODE</param>
		/// <param name="inventoryNo">盤點單號</param>
		/// <param name="inventoryType"></param>
		/// <param name="inventorySDate">盤點單日期-起</param>
		/// <param name="inventoryEDate">盤點單日期-訖</param>
		/// <param name="inventoryCycle">盤點週期(0~6)</param>
		/// <param name="inventoryYear">盤點年分</param>
		/// <param name="inventoryMonth">盤點月份</param>
		/// <param name="status">盤點單狀態(0待處理、1初盤、2複盤、3已確認、5過帳、9取消)</param>
		/// <returns></returns>
		public IQueryable<F140101Expansion> GetDatas(string dcCode, string gupCode, string custCode,
            string inventoryNo, string inventoryType,
            DateTime? inventorySDate, DateTime? inventoryEDate,
            string inventoryCycle, string inventoryYear, string inventoryMonth, string status)
        {
            var param = new List<object> { dcCode, gupCode, custCode };
            var sql = $@" 
SELECT A.*,
       (SELECT NAME
       FROM VW_F000904_LANG
       WHERE TOPIC = 'F1980'
       AND SUBTOPIC = 'DEVICE_TYPE'
       AND VALUE = A.CHECK_TOOL
       AND LANG = '{Current.Lang}') CHECK_TOOL_DESC
FROM F140101 A
WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
";
            if (!string.IsNullOrWhiteSpace(inventoryNo))
            {
                sql += " AND A.INVENTORY_NO = @p" + param.Count;
                param.Add(inventoryNo.Trim());
            }
            if (!string.IsNullOrWhiteSpace(inventoryType))
            {
                sql += " AND A.INVENTORY_TYPE = @p" + param.Count;
                param.Add(inventoryType.Trim());
            }
            if (inventorySDate.HasValue)
            {
                sql += " AND A.INVENTORY_DATE >= @p" + param.Count;
                param.Add(inventorySDate.Value);
            }
            if (inventoryEDate.HasValue)
            {
                sql += " AND A.INVENTORY_DATE <= @p" + param.Count;
                param.Add(inventoryEDate.Value);
            }
            if (!string.IsNullOrEmpty(inventoryCycle))
            {
                sql += " AND A.INVENTORY_CYCLE = @p" + param.Count;
                param.Add(inventoryCycle);
            }
            if (!string.IsNullOrEmpty(inventoryYear))
            {
                sql += " AND A.INVENTORY_YEAR = @p" + param.Count;
                param.Add(inventoryYear);
            }
            if (!string.IsNullOrEmpty(inventoryMonth))
            {
                sql += " AND A.INVENTORY_MONTH = @p" + param.Count;
                param.Add(inventoryMonth);
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND A.STATUS = @p" + param.Count;
                param.Add(status);
            }
            else
                sql += " AND A.STATUS <> '9' ";
            sql += " ORDER BY A.INVENTORY_NO ";
            return SqlQuery<F140101Expansion>(sql, param.ToArray());
        }

        /// <summary>
		/// 取得盤點單主檔 (增加描述欄位)
		/// </summary>
		/// <param name="dcCode">DC_CODE</param>
		/// <param name="gupCode">GUP_CODE</param>
		/// <param name="custCode">CUST_CODE</param>
		/// <param name="inventoryNo">盤點單號</param>
		/// <param name="inventoryType"></param>
		/// <param name="inventorySDate">盤點單日期-起</param>
		/// <param name="inventoryEDate">盤點單日期-訖</param>
		/// <param name="inventoryCycle">盤點週期(0~6)</param>
		/// <param name="inventoryYear">盤點年分</param>
		/// <param name="inventoryMonth">盤點月份</param>
		/// <param name="status">盤點單狀態(0待處理、1初盤、2複盤、3已確認、5過帳、9取消)</param>
		/// <returns></returns>
		public IQueryable<F140101Expansion> GetDatasExpansion(string dcCode, string gupCode, string custCode,
            string inventoryNo, string inventoryType,
            DateTime? inventorySDate, DateTime? inventoryEDate,
            string inventoryCycle, string inventoryYear, string inventoryMonth, string status)
        {
            var param = new List<object> { dcCode, gupCode, custCode };
            var sql = $@" 
SELECT A.*,
       (SELECT NAME
          FROM VW_F000904_LANG
         WHERE TOPIC = 'F140101' AND SUBTOPIC = 'STATUS' AND VALUE = A.STATUS AND LANG = '{Current.Lang}') STATUS_DESC,
       (SELECT NAME
          FROM VW_F000904_LANG
         WHERE     TOPIC = 'F140101'
               AND SUBTOPIC = 'INVENTORY_TYPE'
               AND VALUE = A.INVENTORY_TYPE
			   AND LANG = '{Current.Lang}') INVENTORY_TYPE_DESC,
       (SELECT NAME
          FROM VW_F000904_LANG
         WHERE     TOPIC = 'F140101'
               AND SUBTOPIC = 'INVENTORY_CYCLE'
               AND VALUE = A.INVENTORY_CYCLE
			   AND LANG = '{Current.Lang}') INVENTORY_CYCLE_DESC,
       (SELECT NAME
          FROM VW_F000904_LANG
         WHERE     TOPIC = 'F1980'
               AND SUBTOPIC = 'DEVICE_TYPE'
               AND VALUE = A.CHECK_TOOL
			   AND LANG = '{Current.Lang}') CHECK_TOOL_DESC
FROM F140101 A
WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
";
            if (!string.IsNullOrWhiteSpace(inventoryNo))
            {
                sql += " AND A.INVENTORY_NO = @p" + param.Count;
                param.Add(inventoryNo.Trim());
            }
            if (!string.IsNullOrWhiteSpace(inventoryType))
            {
                sql += " AND A.INVENTORY_TYPE = @p" + param.Count;
                param.Add(inventoryType.Trim());
            }
            if (inventorySDate.HasValue)
            {
                sql += " AND A.INVENTORY_DATE >= @p" + param.Count;
                param.Add(inventorySDate.Value);
            }
            if (inventoryEDate.HasValue)
            {
                sql += " AND A.INVENTORY_DATE <= @p" + param.Count;
                param.Add(inventoryEDate.Value);
            }
            if (!string.IsNullOrEmpty(inventoryCycle))
            {
                sql += " AND A.INVENTORY_CYCLE = @p" + param.Count;
                param.Add(inventoryCycle);
            }
            if (!string.IsNullOrEmpty(inventoryYear))
            {
                sql += " AND A.INVENTORY_YEAR = @p" + param.Count;
                param.Add(inventoryYear);
            }
            if (!string.IsNullOrEmpty(inventoryMonth))
            {
                sql += " AND A.INVENTORY_MON = @p" + param.Count;
                param.Add(inventoryMonth);
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND A.STATUS = @p" + param.Count;
                param.Add(status);
            }
            else
                sql += $@" AND A.STATUS IN (SELECT VALUE
                           FROM VW_F000904_LANG
                          WHERE TOPIC = 'F140102' AND SUBTOPIC = 'STATUS'  AND LANG = '{Current.Lang}') ";
            var sqlResult = SqlQuery<F140101Expansion>(sql, param.ToArray()).ToList();
            for (var i = 0; i < sqlResult.Count(); i++)
            {
                sqlResult[i].ROWNUM = i;
            }
            return sqlResult.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="postingDateBegin"></param>
        /// <param name="postingDateEnd"></param>
        /// <returns></returns>
        public IQueryable<InventoryQueryData> GetInventoryQueryDatas(string dcCode, string gupCode, string custCode, string postingDateBegin, string postingDateEnd)
        {
            var param = new List<object> { dcCode };
            var sql = @"
SELECT O.*,
       CASE O.INVENTORY_TYPE
          WHEN '0' THEN '抽盤'
          WHEN '1' THEN '循環盤'
          WHEN '2' THEN '異動盤'
          WHEN '3' THEN '全盤'
          WHEN '4' THEN '半年盤'
          ELSE ''
       END
          INVENTORY_TYPE_NAME,
        ROW_NUMBER() OVER(ORDER BY O.INVENTORY_NO,
                            O.INVENTORY_DATE,
                            O.POSTING_DATE,
                            O.DC_CODE,
                            O.GUP_CODE,
                            O.CUST_CODE,
                            O.INVENTORY_TYPE ASC) AS ROWNUM 
  FROM (  SELECT A.INVENTORY_NO,
                 A.INVENTORY_DATE,
                 A.POSTING_DATE,
                 A.DC_CODE,
                 A.GUP_CODE,
                 A.CUST_CODE,
                 ISNULL(SUM (B.PROFIT_QTY), 0) PROFIT_QTY,
                 ISNULL(SUM (B.LOSS_QTY), 0) LOSS_QTY,
                 C.DC_NAME,
                 D.GUP_NAME,
                 E.CUST_NAME,
                 A.INVENTORY_TYPE
            FROM   F140101 A
		LEFT JOIN F140107 B ON B.INVENTORY_NO = A.INVENTORY_NO        
		INNER JOIN F1901 C  ON C.DC_CODE = A.DC_CODE
        INNER JOIN F1929 D ON D.GUP_CODE = A.GUP_CODE
        INNER JOIN F1909 E ON E.CUST_CODE = A.CUST_CODE AND E.GUP_CODE = A.GUP_CODE
           WHERE     A.DC_CODE = C.DC_CODE
                 AND A.GUP_CODE = D.GUP_CODE
                 AND A.CUST_CODE = E.CUST_CODE
                 AND A.GUP_CODE = E.GUP_CODE
                 AND A.DC_CODE = B.DC_CODE
                 AND A.GUP_CODE = B.GUP_CODE
                 AND A.CUST_CODE = B.CUST_CODE
                 AND A.INVENTORY_NO = B.INVENTORY_NO
                 AND A.STATUS = '5'
                 AND A.DC_CODE = @p0
";

            if (!string.IsNullOrWhiteSpace(gupCode))
            {
                sql += " AND A.GUP_CODE = @p" + param.Count;
                param.Add(gupCode);
            }

            if (!string.IsNullOrWhiteSpace(custCode))
            {
                sql += " AND A.CUST_CODE = @p" + param.Count;
                param.Add(custCode);
            }

            if (!string.IsNullOrWhiteSpace(postingDateBegin) && !string.IsNullOrWhiteSpace(postingDateEnd))
            {
                sql += " AND A.POSTING_DATE BETWEEN  CONVERT  (DATETIME, @p" + param.Count.ToString() + ") AND CONVERT  (DATETIME, @p" + (param.Count + 1).ToString() + ")";
                param.Add(postingDateBegin + " 00:00:00:000");
                param.Add(postingDateEnd + " 23:59:59:997");
            }

            sql += @"
        GROUP BY A.INVENTORY_NO,
                 A.INVENTORY_DATE,
                 A.POSTING_DATE,
                 A.DC_CODE,
                 A.GUP_CODE,
                 A.CUST_CODE,
                 C.DC_NAME,
                 D.GUP_NAME,
                 E.CUST_NAME,
                 A.INVENTORY_TYPE) O
";
            return SqlQuery<InventoryQueryData>(sql, param.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        /// <param name="sortByCount"></param>
        /// <param name="warehouseId"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        public IQueryable<InventoryQueryDataForDc> GetInventoryQueryDatasForDc(string dcCode, string gupCode, string custCode,
          string inventoryNo, string sortByCount, string warehouseId, string itemCodes)
        {
            var param = new List<object> { dcCode, gupCode, custCode, inventoryNo };

            string filter = "";
            if (!string.IsNullOrWhiteSpace(warehouseId))
            {
                filter += " AND B.WAREHOUSE_ID = @p" + param.Count;
                param.Add(warehouseId);
            }
            if (!string.IsNullOrWhiteSpace(itemCodes))
            {
                filter += string.Format(" AND B.ITEM_CODE IN ('{0}') ",
                  string.Join("','", itemCodes.Split(',')));
            }

            string orderBy = "";
            if (sortByCount == "1")
            {
                orderBy = @"
                            ORDER BY B.LOSS_QTY DESC,  
                                     B.PROFIT_QTY DESC
                        ";
            }
            else
            {
                orderBy = @"
                             ORDER BY B.ITEM_CODE,
                                      B.VALID_DATE,
                                      B.ENTER_DATE,
                                      B.WAREHOUSE_ID,
                                      B.LOC_CODE
                            ";
            }
            #region SQL
            var sql = $@"
                        SELECT         ROW_NUMBER() OVER( {orderBy} ) AS ROWNUM,
                                       A.INVENTORY_NO,
                                       A.INVENTORY_DATE,
                                       A.POSTING_DATE,
                                       A.DC_CODE,
                                       A.GUP_CODE,
                                       A.CUST_CODE,
                                       B.PROFIT_QTY,
                                       B.LOSS_QTY,
                                       B.FLUSHBACK,
                                       B.ITEM_CODE,
                                       B.VALID_DATE,
                                       B.ENTER_DATE,
                                       B.WAREHOUSE_ID,
                                       B.LOC_CODE,
                                       C.DC_NAME,
                                       D.GUP_NAME,
                                       E.SHORT_NAME     CUST_NAME,
                                       F.ITEM_NAME,
                                       F.ITEM_SIZE,
                                       F.ITEM_COLOR,
                                       F.ITEM_SPEC,
                                       I.ACC_UNIT_NAME  AS ITEM_UNIT,
                                       ISNULL(G.QTY, 0) AS QTY,
                                       H.WAREHOUSE_NAME,
                                       A.INVENTORY_TYPE,
                                       CASE A.INVENTORY_TYPE
                                         WHEN '0' THEN '抽盤'
                                         WHEN '1' THEN '循環盤'
                                         WHEN '2' THEN '異動盤'
                                         WHEN '3' THEN '全盤'
                                         WHEN '4' THEN '半年盤'
                                         ELSE ''
                                       END              INVENTORY_TYPE_NAME,
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
                                       END              AS PALLET_BOX_CTRL_NO
                                FROM   F140101 A
                                       LEFT JOIN F140107 B
                                              ON B.INVENTORY_NO = A.INVENTORY_NO
                                                 AND B.DC_CODE = A.DC_CODE
                                                 AND B.GUP_CODE = A.GUP_CODE
                                                 AND B.CUST_CODE = A.CUST_CODE
                                       INNER JOIN F1901 C
                                               ON C.DC_CODE = A.DC_CODE
                                       INNER JOIN F1929 D
                                               ON D.GUP_CODE = A.GUP_CODE
                                       INNER JOIN F1909 E
                                               ON E.CUST_CODE = A.CUST_CODE
                                                  AND E.GUP_CODE = A.GUP_CODE
                                       LEFT JOIN F1903 F
                                              ON F.CUST_CODE = A.CUST_CODE
                                                 AND F.GUP_CODE = B.GUP_CODE
                                                 AND F.ITEM_CODE = B.ITEM_CODE
                                       LEFT JOIN F1913 G
                                              ON G.DC_CODE = B.DC_CODE
                                                 AND G.GUP_CODE = B.GUP_CODE
                                                 AND G.CUST_CODE = B.ITEM_CODE
                                                 AND G.LOC_CODE = B.LOC_CODE
                                                 AND G.VALID_DATE = B.VALID_DATE
                                                 AND G.ENTER_DATE = B.ENTER_DATE
                                                 AND G.MAKE_NO = B.MAKE_NO
                                                 AND G.PALLET_CTRL_NO = B.PALLET_CTRL_NO
                                                 AND G.BOX_CTRL_NO = B.BOX_CTRL_NO
                                       LEFT JOIN F1980 H
                                              ON H.DC_CODE = B.DC_CODE
                                                 AND H.WAREHOUSE_ID = B.WAREHOUSE_ID
                                       INNER JOIN F91000302 I
                                               ON I.ITEM_TYPE_ID = '001'
                                                  AND I.ACC_UNIT = F.ITEM_UNIT
                                WHERE  A.STATUS = '5'
                                       AND ( B.PROFIT_QTY <> 0
                                              OR B.LOSS_QTY <> 0 )
                                       AND A.DC_CODE = @p0
                                       AND A.GUP_CODE = @p1
                                       AND A.CUST_CODE = @p2
                                       AND A.INVENTORY_NO = @p3
                                         {filter}
        ";
            #endregion
            return SqlQuery<InventoryQueryDataForDc>(sql, param.ToArray());
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryDateS"></param>
		/// <param name="inventoryDataE"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="procWmsNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="checkTool"></param>
		/// <returns></returns>
		public IQueryable<F140106QueryData> GetF140106QueryData(string dcCode, string gupCode, string custCode, DateTime? inventoryDateS, DateTime? inventoryDataE, string inventoryNo, string procWmsNo, string itemCode, string checkTool)
		{
			var sqlParamers = new List<SqlParameter>
			{
					new SqlParameter("@p0", dcCode),
					new SqlParameter("@p1", gupCode),
					new SqlParameter("@p2", custCode)
			};

			var condition = string.Empty;
			if (!string.IsNullOrWhiteSpace(inventoryNo))
			{
				condition += $" WHERE X.INVENTORY_NO = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter($"@p{sqlParamers.Count}", inventoryNo));
			}
			if (inventoryDateS.HasValue)
			{
				condition += $" {(string.IsNullOrWhiteSpace(condition) ? " WHERE " : " AND ")} X.INVENTORY_DATE >= @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter($"@p{sqlParamers.Count}", inventoryDateS.Value));
			}
			if (inventoryDataE.HasValue)
			{
				condition += $" {(string.IsNullOrWhiteSpace(condition) ? " WHERE " : " AND ")} X.INVENTORY_DATE <= @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter($"@p{sqlParamers.Count}", inventoryDataE.Value));
			}
			if (!string.IsNullOrWhiteSpace(checkTool))
			{
				condition += $" {(string.IsNullOrWhiteSpace(condition) ? " WHERE " : " AND ")} X.CHECK_TOOL = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter($"@p{sqlParamers.Count}", checkTool));
			}

			var condition2 = string.Empty;
			if (!string.IsNullOrWhiteSpace(procWmsNo))
				condition2 += $" HAVING STRING_AGG(Z.PROC_WMS_NO,',') LIKE '%{procWmsNo}%' ";
			if (!string.IsNullOrWhiteSpace(itemCode))
				condition2 += $" {(string.IsNullOrWhiteSpace(condition2) ? " HAVING " : " AND ")} STRING_AGG(Z.ITEM_CODE,',') LIKE '%{itemCode}%' ";

			var sql = $@"				
				SELECT * FROM (
					SELECT 
							RESULT.DC_CODE ,RESULT.GUP_CODE , RESULT.CUST_CODE 
							,RESULT.INVENTORY_NO ,RESULT.INVENTORY_DATE , RESULT.STATUS , RESULT.STATUS_NAME , RESULT.ITEM_CNT, SUM(ITEMQTY) ITEMQTY, RESULT.QTY, RESULT.MEMO, RESULT.CHECK_TOOL_NAME, RESULT.CHECK_TOOL, CASE WHEN RESULT.CHECK_TOOL <> '0' THEN '1' ELSE '0' END IS_AUTOMATIC, RESULT.INVENTORY_TYPE
					FROM 
					(
						select  
								A.DC_CODE ,A.GUP_CODE,A.CUST_CODE
								,A.INVENTORY_NO ,A.INVENTORY_DATE , A.STATUS ,C.NAME STATUS_NAME ,MAX(A.ITEM_CNT) ITEM_CNT
								,CASE WHEN MAX(B.FIRST_DIFF_QTY) <> 0 OR MAX(B.SECOND_DIFF_QTY) <> 0 THEN 1 ELSE 0 END ITEMQTY
								,MAX(ITEM_QTY) QTY 
								,A.MEMO, D.NAME CHECK_TOOL_NAME
								,A.CHECK_TOOL
                ,A.INVENTORY_TYPE
						from F140101 A
						LEFT JOIN F140106 B ON A.INVENTORY_NO = B.INVENTORY_NO AND A.DC_CODE=B.DC_CODE 
											AND A.GUP_CODE=B.GUP_CODE AND A.CUST_CODE=B.CUST_CODE
						JOIN VW_F000904_LANG C ON C.SUBTOPIC='STATUS' AND C.TOPIC='F140101' AND A.STATUS =C.VALUE AND C.LANG = '{Current.Lang}'
					  JOIN VW_F000904_LANG D ON D.SUBTOPIC='DEVICE_TYPE' AND D.TOPIC='F1980' AND A.CHECK_TOOL =D.VALUE AND D.LANG = '{Current.Lang}'
						WHERE (	A.STATUS ='3' OR A.STATUS = '5')  --已確認或結案
								AND A.DC_CODE = @p0
								AND A.GUP_CODE = @p1
								AND A.CUST_CODE = @p2
						GROUP BY A.DC_CODE ,A.GUP_CODE,A.CUST_CODE , A.INVENTORY_NO 
								,A.INVENTORY_DATE , A.STATUS ,C.NAME , B.ITEM_CODE, A.MEMO, D.NAME , A.CHECK_TOOL, A.INVENTORY_TYPE
					) RESULT
					LEFT JOIN F140106 Z
					ON RESULT.INVENTORY_NO = Z.INVENTORY_NO 
					AND RESULT.DC_CODE=Z.DC_CODE 
					AND RESULT.GUP_CODE=Z.GUP_CODE 
					AND RESULT.CUST_CODE=Z.CUST_CODE 
					GROUP BY RESULT.DC_CODE ,RESULT.GUP_CODE,RESULT.CUST_CODE , RESULT.INVENTORY_NO ,RESULT.INVENTORY_DATE , RESULT.STATUS ,RESULT.STATUS_NAME , RESULT.ITEM_CNT, RESULT.QTY, RESULT.MEMO, RESULT.CHECK_TOOL_NAME, RESULT.CHECK_TOOL, RESULT.INVENTORY_TYPE
							--多包一層GROUP 主要為了 ITEM_CODE 重複 , 計算ITEM_CODE 種類數量更精準
					{condition2}
							) X
					{condition}
					ORDER BY X.INVENTORY_NO
					";

			var result = SqlQuery<F140106QueryData>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		public IQueryable<F1913Data> GetF140106QueryDetailData(string dcCode, string gupCode, string custCode, string inventoryNo)
        {
            var sqlParamers = new List<object>
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            };

            var sql = @"				
					SELECT ROW_NUMBER() OVER( ORDER BY A.INVENTORY_NO
                                                    ,A.DC_CODE
                                                    ,A.GUP_CODE
                                                    ,A.CUST_CODE) 'ROWNUM',
       A.DC_CODE,
       A.GUP_CODE,
       A.CUST_CODE,
       C.WAREHOUSE_ID,
       B.LOC_CODE,
       D.WAREHOUSE_NAME,
       B.ITEM_CODE,
       E.ITEM_NAME,
       ISNULL(E.BUNDLE_SERIALNO, 0)  BUNDLE_SERIALNO,
       ISNULL(E.BUNDLE_SERIALLOC, 0) BUNDLE_SERIALLOC,
       '000000'                      VNR_CODE,
       B.QTY,
       B.VALID_DATE,
       B.ENTER_DATE,
       '999'                         CAUSE,
       '盤盈損調整'             CAUSE_MEMO,
       CASE
         WHEN A.ISSECOND = '1' THEN --複盤
           CASE
             WHEN B.SECOND_DIFF_QTY > 0 THEN '0'
             ELSE '1'
           END
         ELSE --初盤
           CASE
             WHEN B.FIRST_DIFF_QTY > 0 THEN '0'
             ELSE '1'
           END
       END                           WORK_TYPE
       --盤盈 = 0:調入   盤損 1:調出           
       ,
       FLUSHBACK                     IS_FLUSHBACK --是否回沖 
       ,
       CASE
         WHEN A.ISSECOND = '1' THEN --複盤
           CASE
             WHEN B.SECOND_DIFF_QTY > 0 THEN SECOND_DIFF_QTY
             ELSE 0
           END
         ELSE --初盤
           CASE
             WHEN B.FIRST_DIFF_QTY > 0 THEN FIRST_DIFF_QTY
             ELSE 0
           END
       END                           ADJ_QTY_IN -- 盤盈 - 調入
       ,
       CASE
         WHEN A.ISSECOND = '1' THEN --複盤
           CASE
             WHEN B.SECOND_DIFF_QTY < 0 THEN -( SECOND_DIFF_QTY )
             ELSE 0
           END
         ELSE --初盤 
           CASE
             WHEN B.FIRST_DIFF_QTY < 0 THEN -( FIRST_DIFF_QTY )
             ELSE 0
           END
       END                           ADJ_QTY_OUT,-- 盤損 - 調出
       B.BOX_CTRL_NO,
       B.PALLET_CTRL_NO,
       B.MAKE_NO
FROM   F140101 A
       JOIN F140106 B
         ON A.INVENTORY_NO = B.INVENTORY_NO
       JOIN F1912 C
         ON C.DC_CODE = A.DC_CODE
            AND C.LOC_CODE = B.LOC_CODE
       JOIN F1980 D
         ON D.DC_CODE = A.DC_CODE
            AND D.WAREHOUSE_ID = C.WAREHOUSE_ID
       JOIN F1903 E
         ON E.GUP_CODE = A.GUP_CODE
            AND E.CUST_CODE = A.CUST_CODE
            AND E.ITEM_CODE = B.ITEM_CODE
WHERE  A.DC_CODE = @p0
       AND A.GUP_CODE = @p1
       AND A.CUST_CODE = @p2
       AND A.INVENTORY_NO = @p3 
			";

            return SqlQuery<F1913Data>(sql, sqlParamers.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        public void UpdateItemCntAndQty(string dcCode, string gupCode, string custCode, string inventoryNo)
        {
            var param = new List<SqlParameter>
          {
              new SqlParameter("@p0", dcCode),
              new SqlParameter("@p1", gupCode),
              new SqlParameter("@p2", custCode),
              new SqlParameter("@p3", inventoryNo),
              new SqlParameter("@p4", Current.Staff),
              new SqlParameter("@p5", Current.StaffName)
          };
            var sql = @"
						UPDATE F140101
SET    ITEM_CNT = ISNULL((SELECT Count(DISTINCT ITEM_CODE)
                          FROM   F140104
                          WHERE  DC_CODE = @p0
                                 AND GUP_CODE = @p1
                                 AND CUST_CODE = @p2
                                 AND INVENTORY_NO = @p3), 0),
       ITEM_QTY = ISNULL((SELECT Sum(QTY)
                          FROM   F140104
                          WHERE  DC_CODE = @p0
                                 AND GUP_CODE = @p1
                                 AND CUST_CODE = @p2
                                 AND INVENTORY_NO = @p3), 0),
       UPD_STAFF = @p4,
       UPD_NAME = @p5,
       UPD_DATE = dbo.GetSysDate()
WHERE  DC_CODE = @p0
       AND GUP_CODE = @p1
       AND CUST_CODE = @p2
       AND INVENTORY_NO = @p3 
					";
            ExecuteSqlCommand(sql, param.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        /// <returns></returns>
        public IQueryable<F140101> GetDataByUserCanInventory(string dcCode, string gupCode, string custCode, string inventoryNo)
        {
            //有使用者可用的儲位權限
            var userCanUseLocSql = @"AND       EXISTS
                              (
                                         SELECT     1
                                         FROM       {0} B
                                         INNER JOIN F1912 C
                                         ON         C.DC_CODE = B.DC_CODE
                                         AND        C.LOC_CODE = B.LOC_CODE
                                         INNER JOIN F196301 D
                                         ON         D.LOC_CODE = C.LOC_CODE
                                         INNER JOIN F1963 E
                                         ON         E.WORK_ID = D.WORK_ID
                                         INNER JOIN F192403 F
                                         ON         F.WORK_ID = E.WORK_ID
                                         INNER JOIN F1924 G
                                         ON         G.EMP_ID = F.EMP_ID
                                         WHERE      G.EMP_ID = @p3
                                         AND        B.DC_CODE = A.DC_CODE
                                         AND        B.GUP_CODE = A.GUP_CODE
                                         AND        B.CUST_CODE = A.CUST_CODE
                                         AND        B.INVENTORY_NO = A.INVENTORY_NO ) )";

            var sql = $@"SELECT    A.*
FROM      F140101 A
LEFT JOIN
          (
                   SELECT   B.DC_CODE,
                            B.GUP_CODE,
                            B.CUST_CODE,
                            B.INVENTORY_NO,
                            B.ISSECOND,
                            MAX(ISNULL(B.UPD_DATE,B.CRT_DATE)) AS LAST_DATE
                   FROM     F140110 B
                   WHERE    ISNULL(UPD_STAFF,CRT_STAFF) = @p3
                   GROUP BY B.DC_CODE,
                            B.GUP_CODE,
                            B.CUST_CODE,
                            B.INVENTORY_NO,
                            B.ISSECOND ) H
ON        H.DC_CODE = A.DC_CODE
AND       H.GUP_CODE = A.GUP_CODE
AND       H.CUST_CODE = A.CUST_CODE
AND       H.INVENTORY_NO = A.INVENTORY_NO
AND       H.ISSECOND = A.ISSECOND
WHERE     A.DC_CODE = @p0
AND       A.GUP_CODE = @p1
AND       A.CUST_CODE = @p2
AND       A.STATUS IN('0',
                      '1',
                      '2') 
 AND ((A.ISSECOND ='0' {string.Format(userCanUseLocSql, "F140104")} )
  OR(A.ISSECOND = '1' {string.Format(userCanUseLocSql, "F140105")}";
            

            var param = new List<SqlParameter>
          {
              new SqlParameter("@p0", dcCode),
              new SqlParameter("@p1", gupCode),
              new SqlParameter("@p2", custCode),
              new SqlParameter("@p3", Current.Staff)
          };

            if (!string.IsNullOrWhiteSpace(inventoryNo))
            {
                sql += " AND A.INVENTORY_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p4", inventoryNo.Trim()));
            }

            sql += " AND A.STATUS <> '9' ORDER BY (CASE WHEN H.LAST_DATE IS NULL THEN 0 ELSE 1 END), H.LAST_DATE "; //依照使用者已盤點過最後日期盤點單優先

            return SqlQuery<F140101>(sql, param.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="settleDate"></param>
        /// <returns></returns>
        public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", settleDate),
                new SqlParameter("@p4", settleDate.AddDays(1))
            };
            var sql = @"
						SELECT @p4 CAL_DATE,
								 '01' DELV_ACC_TYPE,
								 A.*,
                                A.INVENTORY_NO WMS_NO,
                                A.ITEM_QTY QTY,
                                ROW_NUMBER() OVER(ORDER BY A.INVENTORY_NO ASC) AS ROWNUM 
						FROM F140101 A
					 WHERE     (A.DC_CODE = @p0 OR @p0 = '000')
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND A.INVENTORY_DATE >= @p3
								 AND A.INVENTORY_DATE < @p4
								 AND A.ISCHARGE = '1'
								 AND A.STATUS IN ('3', '5')
					 ORDER BY WMS_NO";
            return SqlQuery<SettleData>(sql, parameter.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        /// <param name="isSec"></param>
        /// <returns></returns>
        public IQueryable<F1913Data> GetInventoryDetailData(string dcCode, string gupCode, string custCode, string inventoryNo, bool isSec)
        {
            var sqlParamers = new List<object>
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            };
            string tableName = isSec ? "F140105" : "F140104";
            var sql = $@"SELECT ROW_NUMBER() OVER( ORDER BY A.INVENTORY_NO
                                                    ,A.DC_CODE
                                                    ,A.GUP_CODE
                                                    ,A.CUST_CODE) 'ROWNUM',
                                A.DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                C.WAREHOUSE_ID,
                                G.LOC_CODE,
                                D.WAREHOUSE_NAME,
                                G.ITEM_CODE,
                                F.ITEM_NAME,
																F.CUST_ITEM_CODE,
																F.EAN_CODE1,
																F.EAN_CODE2,
																F.EAN_CODE3,
                                ISNULL (F.BUNDLE_SERIALNO, 0)  BUNDLE_SERIALNO,
                                ISNULL (F.BUNDLE_SERIALLOC, 0) BUNDLE_SERIALLOC,
                                '000000'                       VNR_CODE,
                                G.QTY,
                                G.VALID_DATE,
                                G.ENTER_DATE,
                                '999'                          CAUSE,
                                '盤盈損調整'              CAUSE_MEMO,
                                CASE
                                  WHEN A.ISSECOND = '1' THEN --複盤
                                    CASE
                                      WHEN B.SECOND_DIFF_QTY > 0 THEN '0'
                                      ELSE '1'
                                    END
                                  ELSE --初盤
                                    CASE
                                      WHEN B.FIRST_DIFF_QTY > 0 THEN '0'
                                      ELSE '1'
                                    END
                                END                            WORK_TYPE,
                                --盤盈 = 0:調入   盤損 1:調出
                                ISNULL (B.FLUSHBACK, 0)        IS_FLUSHBACK,--是否回沖
                                CASE
                                  WHEN A.ISSECOND = '1' THEN --複盤
                                    CASE
                                      WHEN B.SECOND_DIFF_QTY > 0 THEN SECOND_DIFF_QTY
                                      ELSE 0
                                    END
                                  ELSE --初盤
                                    CASE
                                      WHEN B.FIRST_DIFF_QTY > 0 THEN FIRST_DIFF_QTY
                                      ELSE 0
                                    END
                                END                            ADJ_QTY_IN -- 盤盈 - 調入
                                ,
                                CASE
                                  WHEN A.ISSECOND = '1' THEN --複盤
                                    CASE
                                      WHEN B.SECOND_DIFF_QTY < 0 THEN -( SECOND_DIFF_QTY )
                                      ELSE 0
                                    END
                                  ELSE --初盤
                                    CASE
                                      WHEN B.FIRST_DIFF_QTY < 0 THEN -( FIRST_DIFF_QTY )
                                      ELSE 0
                                    END
                                END                            ADJ_QTY_OUT,-- 盤損 - 調出
                                ISNULL (B.BOX_CTRL_NO, '0')    BOX_CTRL_NO,
                                ISNULL (B.PALLET_CTRL_NO, '0') PALLET_CTRL_NO,
                                B.MAKE_NO,
																CASE A.ISSECOND WHEN '1' THEN G.SECOND_QTY ELSE G.FIRST_QTY END INVENTORY_QTY, --實際盤點數    
																B.UNMOVE_STOCK_QTY,	--WMS未搬動數量,
                                B.DEVICE_STOCK_QTY,	--自動倉庫存數量
                                CASE A.ISSECOND WHEN '1' THEN B.SECOND_DIFF_QTY ELSE B.FIRST_DIFF_QTY END DIFF_QTY, --盤點差異數       
                                CASE A.ISSECOND WHEN '1' THEN B.SECOND_STOCK_DIFF_QTY ELSE B.FIRST_STOCK_DIFF_QTY END STOCK_DIFF_QTY, -- 盤墊庫差數
                                B.STATUS, --盤點狀態
																H.NAME WMS_STATUS_NAME,--WMS調整狀態
																I.NAME PERSON_CONFIRM_STATUS_NAME,--人員調整狀態
																B.PROC_WMS_NO,--處理單號(盤盈:調整單號 盤損:調撥單號)
                                CASE A.ISSECOND WHEN '1' THEN B.SEC_INVENTORY_DATE ELSE B.FST_INVENTORY_DATE END INVENTORY_DATE, --盤點時間
                                CASE A.ISSECOND WHEN '1' THEN B.SEC_INVENTORY_NAME ELSE B.FST_INVENTORY_NAME END INVENTORY_NAME --盤點人員
						  FROM F140101 A
							   JOIN {tableName} G
								  ON     A.INVENTORY_NO = G.INVENTORY_NO
									 AND A.DC_CODE = G.DC_CODE
									 AND A.GUP_CODE = G.GUP_CODE
									 AND A.CUST_CODE = G.CUST_CODE
							   INNER JOIN F140106 B
								  ON     A.INVENTORY_NO = B.INVENTORY_NO
									 AND G.ITEM_CODE = B.ITEM_CODE
                                     AND G.LOC_CODE = B.LOC_CODE
                                     AND G.VALID_DATE = B.VALID_DATE
                                     AND G.ENTER_DATE = B.ENTER_DATE
									 AND G.MAKE_NO = B.MAKE_NO
									 AND G.BOX_CTRL_NO = B.BOX_CTRL_NO
									 AND G.PALLET_CTRL_NO = B.PALLET_CTRL_NO
									 AND A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
							   JOIN F1912 C ON C.DC_CODE = A.DC_CODE AND C.LOC_CODE = G.LOC_CODE
							   JOIN F1980 D
								  ON D.DC_CODE = A.DC_CODE AND D.WAREHOUSE_ID = C.WAREHOUSE_ID							   
							   JOIN F1903 F
								  ON     F.GUP_CODE = A.GUP_CODE
									 AND F.CUST_CODE = A.CUST_CODE
									 AND F.ITEM_CODE = G.ITEM_CODE
							   LEFT JOIN VW_F000904_LANG H
								  ON H.TOPIC = 'F140106'
								  AND H.SUBTOPIC = 'WMS_STATUS' 
								  AND H.LANG = '{Current.Lang}'
								  AND H.VALUE = B.WMS_STATUS
							   LEFT JOIN VW_F000904_LANG I
								  ON I.TOPIC = 'F140106'
								  AND I.SUBTOPIC = 'PERSON_CONFIRM_STATUS' 
								  AND I.LANG = '{Current.Lang}'
								  AND I.VALUE = B.PERSON_CONFIRM_STATUS
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND A.INVENTORY_NO = @p3";           
            return SqlQuery<F1913Data>(sql, sqlParamers.ToArray());
        }

        

       

        /// <summary>
        /// 取得盤點資料
        /// </summary>
        /// <param name="dcNo"></param>
        /// <param name="custNo"></param>
        /// <param name="gupCode"></param>
        /// <param name="invNo"></param>
        /// <param name="invDate"></param>
        /// <returns></returns>
        public IQueryable<GetInvRes> GetInventoryList(string dcNo, string custNo, string gupCode, string invNo, string invDate)
        {

            var param = new List<SqlParameter>{
                        new SqlParameter("@p0", dcNo),
                        new SqlParameter("@p1", custNo),
                        new SqlParameter("@p2", gupCode)
            };
            var sql = @"SELECT a.INVENTORY_TYPE InvType,
                        NULL InvTypeName,
                        a.INVENTORY_NO InvNo,
                        a.DC_CODE DcNo,
                        a.CUST_CODE CustNo,
                         CONVERT(varchar,a.INVENTORY_DATE,111) InvDate,
                       COUNT(DISTINCT b.ITEM_CODE) ItemCnt,
                       SUM(b.QTY) ItemQty,
                       a.SHOW_CNT StocktakeFlag,
                        a.STATUS Status,
                        NULL StatusName,
                        '1' FirstCountFlag,
                        COUNT(DISTINCT b.ITEM_CODE)- COUNT(DISTINCT CASE WHEN b.FIRST_QTY IS NULL THEN b.ITEM_CODE ELSE NULL END ) InvItemCnt
                        FROM F140101 a
                        LEFT JOIN F140104 b ON a.INVENTORY_NO  = b.INVENTORY_NO 
	                    AND a.DC_CODE = b.DC_CODE 
	                    AND a.GUP_CODE  = b.GUP_CODE 
	                    AND a.CUST_CODE  = b.CUST_CODE 
                        WHERE ISSECOND  = '0'
                        AND a.STATUS in('0','1','2')
                        AND a.DC_CODE = @p0
                        AND a.CUST_CODE= @p1
                        AND a.GUP_CODE = @p2 
												AND a.CHECK_TOOL = '0' ";

            // 若 InvNo is not null, 增加條件F140101.inventory_no = InvNo
            if (!string.IsNullOrWhiteSpace(invNo))
            {
                sql += " AND a.INVENTORY_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, invNo));
            }

            // 若InvDate is not null, 增加條件F140101.inventory_date = InvDate
            if (!string.IsNullOrWhiteSpace(invDate))
            {

                sql += $" AND a.INVENTORY_DATE = @p{param.Count}";
                param.Add(new SqlParameter("@p" + param.Count, invDate.Replace("/", "-")));
            }

            sql += @"  AND (SELECT count(*) FROM F140104 WHERE DC_CODE=a.DC_CODE AND GUP_CODE = a.GUP_CODE 
                       AND CUST_CODE = a.CUST_CODE AND INVENTORY_NO = a.INVENTORY_NO and FIRST_QTY is null)<>0GROUP BY a.INVENTORY_NO ,a.INVENTORY_TYPE,
                        a.DC_CODE,
                        a.CUST_CODE,
                        a.INVENTORY_DATE,
                        a.SHOW_CNT,
                        a.STATUS
                        UNION
                        SELECT a.INVENTORY_TYPE InvType,
                        NULL InvTypeName,
                        a.INVENTORY_NO InvNo,
                        a.DC_CODE DcNo,
                        a.CUST_CODE CustNo,
                        CONVERT(varchar,a.INVENTORY_DATE,111) InvDate,
                       COUNT(DISTINCT  c.ITEM_CODE) ItemCnt,
                       SUM(c.QTY) ItemQty,
                       a.SHOW_CNT StocktakeFlag,
                        a.STATUS Status,
                        NULL StatusName,
                        '2' FirstCountFlag,
                        COUNT(DISTINCT  c.ITEM_CODE) - COUNT(DISTINCT CASE WHEN c.SECOND_QTY IS NULL THEN c.ITEM_CODE ELSE NULL END ) InvItemCnt
                        FROM F140101 a
                        LEFT JOIN F140105 c ON a.INVENTORY_NO = c.INVENTORY_NO
                        AND a.DC_CODE = c.DC_CODE
                        AND a.GUP_CODE = c.GUP_CODE
                        AND a.CUST_CODE = c.CUST_CODE
                        WHERE ISSECOND = '1' 
                        AND a.STATUS in('0','1','2')
                        AND a.DC_CODE = @p0
                        AND a.CUST_CODE=@p1
                        AND a.GUP_CODE = @p2 
												AND a.CHECK_TOOL = '0' ";

            // 若 InvNo is not null, 增加條件F140101.inventory_no = InvNo
            if (!string.IsNullOrWhiteSpace(invNo))
            {
                sql += " AND a.INVENTORY_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, invNo));
            }

            // 若InvDate is not null, 增加條件F140101.inventory_date = InvDate
            if (!string.IsNullOrWhiteSpace(invDate))
            {
                sql += $" AND a.INVENTORY_DATE = @p{param.Count}";
                param.Add(new SqlParameter("@p" + param.Count, invDate.Replace("/", "-")));
            }

            sql += @" AND (SELECT count(*) FROM F140105 WHERE DC_CODE=c.DC_CODE AND GUP_CODE = c.GUP_CODE 
                    AND CUST_CODE = c.CUST_CODE AND INVENTORY_NO = c.INVENTORY_NO and SECOND_QTY is null)<>0
                    GROUP BY a.INVENTORY_NO ,a.INVENTORY_TYPE,
                    a.DC_CODE,
                    a.CUST_CODE,
                    a.INVENTORY_DATE,
                    a.SHOW_CNT,
                    a.STATUS
                    ORDER BY a.INVENTORY_NO ";

            var result = SqlQuery<GetInvRes>(sql, param.ToArray());
            return result;
        }
    }
}
