using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
    public partial class F140105Repository : RepositoryBase<F140105, Wms3plDbContext, F140105Repository>
    {
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
                            SELECT 'N'                                                     as ChangeStatus,
                                   ROW_NUMBER()
                                     OVER(
                                       ORDER BY C.WAREHOUSE_NAME, A.LOC_CODE, A.ITEM_CODE) AS ROWNUM,
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
                                   A.QTY,
                                   A.FIRST_QTY                                             as FIRST_QTY_ORG,
                                   A.FIRST_QTY,
                                   A.SECOND_QTY                                            AS SECOND_QTY_ORG
                                   ,
                                   A.SECOND_QTY,
                                   A.FLUSHBACK                                             AS FLUSHBACK_ORG,
                                   A.FLUSHBACK,
                                   CASE
                                     WHEN A.FLUSHBACK = '1' THEN '是'
                                     ELSE '否'
                                   END                                                     AS FLUSHBACKNAME,
                                   A.BOX_CTRL_NO,
                                   A.PALLET_CTRL_NO,
                                   A.MAKE_NO,
								   A.UNMOVE_STOCK_QTY,
								   {(checkTool == "0" ? "NULL" : "A.DEVICE_STOCK_QTY")} DEVICE_STOCK_QTY,
								   B.CUST_ITEM_CODE,
								   B.EAN_CODE1,
								   B.EAN_CODE2,
								   B.EAN_CODE3 
                            FROM   F140105 A
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
        /// <param name="enterDate"></param>
        /// <param name="validDate"></param>
        /// <param name="makeNo"></param>
        /// <returns></returns>
        public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode,
            string inventoryNo, string locCode, string itemCode, DateTime enterDate, DateTime validDate, string makeNo)
        {
            var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate };
            var sql = $@"
                            SELECT ROW_NUMBER() OVER(ORDER BY A.DC_CODE,
                                                              A.GUP_CODE, 
                                                              A.CUST_CODE,
                                                              A.INVENTORY_NO ) 'ROWNUM',
                                  'N'          as ChangeStatus,
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
                                   A.QTY,
                                   A.FIRST_QTY  as FIRST_QTY_ORG,
                                   A.FIRST_QTY,
                                   A.SECOND_QTY AS SECOND_QTY_ORG,
                                   A.SECOND_QTY,
                                   A.FLUSHBACK  AS FLUSHBACK_ORG,
                                   A.FLUSHBACK,
                                   A.BOX_CTRL_NO,
                                   A.PALLET_CTRL_NO
                            FROM   F140105 A
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
                                   AND A.LOC_CODE = @p4
                                   AND A.ITEM_CODE = @p5
                                   AND A.ENTER_DATE = @p6
                                   AND A.VALID_DATE = @p7
                            ";
            if (!string.IsNullOrWhiteSpace(makeNo))
            {
                param.Add(makeNo);
                sql = string.Format("{0} {1}", sql, " AND A.MAKE_NO = @p8 ");
            }
            else
            {
                sql = string.Format("{0} {1}", sql, " AND A.MAKE_NO = '' ");
            }
            return SqlQuery<InventoryDetailItem>(sql, param.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventorNo"></param>
        public void Delete(string dcCode, string gupCode, string custCode, string inventoryNo)
        {
            var sql = @"
                        DELETE F140105
                        WHERE     DC_CODE = @p0
                        AND GUP_CODE = @p1
                        AND CUST_CODE = @p2
                        AND INVENTORY_NO = @p3
			          ";
            var param = new[]
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", inventoryNo)
            };
            ExecuteSqlCommand(sql, param);
        }

        /// <summary>
        /// 產生複盤資料By人工倉
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventorNo"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        public void InsertFromF140104ByManual(string dcCode, string gupCode, string custCode, string inventorNo, string userId, string userName)
        {
            #region SQL
            var sql = @"
                        INSERT INTO F140105 
                                    (INVENTORY_NO, 
                                     WAREHOUSE_ID, 
                                     LOC_CODE, 
                                     ITEM_CODE, 
                                     VALID_DATE, 
                                     ENTER_DATE, 
                                     QTY, 
                                     FIRST_QTY, 
                                     SECOND_QTY, 
                                     FLUSHBACK, 
                                     DC_CODE, 
                                     GUP_CODE, 
                                     CUST_CODE, 
                                     FST_INVENTORY_STAFF, 
                                     FST_INVENTORY_NAME, 
                                     FST_INVENTORY_DATE, 
                                     FST_INVENTORY_PC, 
                                     SEC_INVENTORY_STAFF, 
                                     SEC_INVENTORY_NAME, 
                                     SEC_INVENTORY_DATE, 
                                     SEC_INVENTORY_PC, 
                                     BOX_CTRL_NO, 
                                     PALLET_CTRL_NO, 
                                     CRT_STAFF, 
                                     CRT_NAME, 
                                     CRT_DATE, 
                                     UPD_STAFF, 
                                     UPD_NAME, 
                                     UPD_DATE, 
                                     MAKE_NO,
									 DEVICE_STOCK_QTY,
                                    UNMOVE_STOCK_QTY)
                        SELECT A.INVENTORY_NO, 
                               A.WAREHOUSE_ID, 
                               A.LOC_CODE, 
                               A.ITEM_CODE, 
                               A.VALID_DATE, 
                               A.ENTER_DATE, 
                               A.QTY, 
                               ISNULL(A.FIRST_QTY, 0), 
                               A.SECOND_QTY, 
                               A.FLUSHBACK, 
                               A.DC_CODE, 
                               A.GUP_CODE, 
                               A.CUST_CODE, 
                               A.FST_INVENTORY_STAFF, 
                               A.FST_INVENTORY_NAME, 
                               A.FST_INVENTORY_DATE, 
                               A.FST_INVENTORY_PC, 
                               A.SEC_INVENTORY_STAFF, 
                               A.SEC_INVENTORY_NAME, 
                               A.SEC_INVENTORY_DATE, 
                               A.SEC_INVENTORY_PC, 
                               A.BOX_CTRL_NO, 
                               A.PALLET_CTRL_NO, 
                               @p4, 
                               @p5, 
                               dbo.GetSysDate(), 
                               @p4, 
                               @p5, 
                               dbo.GetSysDate(), 
                               A.MAKE_NO ,
															 A.DEVICE_STOCK_QTY,
                               A.UNMOVE_STOCK_QTY
                        FROM   F140104 A 
                        WHERE  A.DC_CODE = @p0 
                           AND A.GUP_CODE = @p1 
                           AND A.CUST_CODE = @p2 
                           AND A.INVENTORY_NO = @p3 
                           AND ((A.FIRST_QTY - ISNULL(A.DEVICE_STOCK_QTY, 0)) <> 0 --初盤差異數
														 OR (A.SECOND_QTY - ISNULL(A.DEVICE_STOCK_QTY, 0)) <> 0    --複盤差異數
														 OR (A.FIRST_QTY - A.QTY - A.UNMOVE_STOCK_QTY) <> 0   --初盤庫差數
														 OR (A.SECOND_QTY - A.QTY - A.UNMOVE_STOCK_QTY) <> 0  --複盤庫差數
														 )
			            ";
            #endregion
            var param = new[]
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", inventorNo),
                new SqlParameter("@p4", userId),
                new SqlParameter("@p5", userName)
            };
            ExecuteSqlCommand(sql, param);
        }

		/// <summary>
		/// 產生複盤資料By自動倉
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventorNo"></param>
		/// <param name="userId"></param>
		/// <param name="userName"></param>
		public void InsertFromF140104ByAuto(string dcCode, string gupCode, string custCode, string inventorNo, string userId, string userName)
		{
			#region SQL
			var sql = @"
                        INSERT INTO F140105 
                                    (INVENTORY_NO, 
                                     WAREHOUSE_ID, 
                                     LOC_CODE, 
                                     ITEM_CODE, 
                                     VALID_DATE, 
                                     ENTER_DATE, 
                                     QTY, 
                                     FIRST_QTY, 
                                     SECOND_QTY, 
                                     FLUSHBACK, 
                                     DC_CODE, 
                                     GUP_CODE, 
                                     CUST_CODE, 
                                     FST_INVENTORY_STAFF, 
                                     FST_INVENTORY_NAME, 
                                     FST_INVENTORY_DATE, 
                                     FST_INVENTORY_PC, 
                                     SEC_INVENTORY_STAFF, 
                                     SEC_INVENTORY_NAME, 
                                     SEC_INVENTORY_DATE, 
                                     SEC_INVENTORY_PC, 
                                     BOX_CTRL_NO, 
                                     PALLET_CTRL_NO, 
                                     CRT_STAFF, 
                                     CRT_NAME, 
                                     CRT_DATE, 
                                     UPD_STAFF, 
                                     UPD_NAME, 
                                     UPD_DATE, 
                                     MAKE_NO,
									 DEVICE_STOCK_QTY,
                                    UNMOVE_STOCK_QTY)
                        SELECT A.INVENTORY_NO, 
                               A.WAREHOUSE_ID, 
                               A.LOC_CODE, 
                               A.ITEM_CODE, 
                               A.VALID_DATE, 
                               A.ENTER_DATE, 
                               A.QTY, 
                               ISNULL(A.FIRST_QTY, 0), 
                               A.SECOND_QTY, 
                               A.FLUSHBACK, 
                               A.DC_CODE, 
                               A.GUP_CODE, 
                               A.CUST_CODE, 
                               A.FST_INVENTORY_STAFF, 
                               A.FST_INVENTORY_NAME, 
                               A.FST_INVENTORY_DATE, 
                               A.FST_INVENTORY_PC, 
                               A.SEC_INVENTORY_STAFF, 
                               A.SEC_INVENTORY_NAME, 
                               A.SEC_INVENTORY_DATE, 
                               A.SEC_INVENTORY_PC, 
                               A.BOX_CTRL_NO, 
                               A.PALLET_CTRL_NO, 
                               @p4, 
                               @p5, 
                               dbo.GetSysDate(), 
                               @p4, 
                               @p5, 
                               dbo.GetSysDate(), 
                               A.MAKE_NO ,
							   0,
                               A.UNMOVE_STOCK_QTY
                        FROM   F140104 A 
                        WHERE  A.DC_CODE = @p0 
                           AND A.GUP_CODE = @p1 
                           AND A.CUST_CODE = @p2 
                           AND A.INVENTORY_NO = @p3 
                           AND EXISTS(SELECT 1 
                                      FROM   F140104 B 
                                      WHERE  B.DC_CODE = A.DC_CODE 
                                         AND B.GUP_CODE = A.GUP_CODE 
                                         AND B.CUST_CODE = A.CUST_CODE 
                                         AND B.INVENTORY_NO = A.INVENTORY_NO 
                                         AND B.LOC_CODE = A.LOC_CODE 
                                         AND B.ITEM_CODE = A.ITEM_CODE 
                                         AND B.MAKE_NO = A.MAKE_NO 
                                         AND B.VALID_DATE = A.VALID_DATE 
                                      GROUP  BY B.DC_CODE, 
                                                B.GUP_CODE, 
                                                B.INVENTORY_NO, 
                                                B.CUST_CODE, 
                                                B.LOC_CODE, 
                                                B.ITEM_CODE, 
                                                B.MAKE_NO, 
                                                B.VALID_DATE 
                                      HAVING (Sum(B.FIRST_QTY) - Sum(ISNULL(B.DEVICE_STOCK_QTY, 0))) <> 0 --初盤差異數
																			OR (Sum(B.SECOND_QTY) - Sum(ISNULL(B.DEVICE_STOCK_QTY, 0))) <> 0    --複盤差異數
																			OR (Sum(B.FIRST_QTY) - Sum(B.QTY) - Sum(B.UNMOVE_STOCK_QTY)) <> 0   --初盤庫差數
																			OR (Sum(B.SECOND_QTY) - Sum(B.QTY) - Sum(B.UNMOVE_STOCK_QTY)) <> 0  --複盤庫差數
																			) 
			            ";
			#endregion
			var param = new[]
			{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", inventorNo),
								new SqlParameter("@p4", userId),
								new SqlParameter("@p5", userName)
						};
			ExecuteSqlCommand(sql, param);
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
                        SELECT 'N'                                                     as ChangeStatus, 
                               ROW_NUMBER() 
                                 OVER( 
                                   ORDER BY C.WAREHOUSE_NAME, A.LOC_CODE, A.ITEM_CODE) AS ROWNUM, 
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
                               A.FIRST_QTY                                             as FIRST_QTY_ORG, 
                               A.FIRST_QTY, 
                               A.SECOND_QTY                                            AS SECOND_QTY_ORG 
                               , 
                               A.SECOND_QTY, 
                               A.FLUSHBACK                                             AS FLUSHBACK_ORG, 
                               A.FLUSHBACK, 
                               CASE 
                                 WHEN A.FLUSHBACK = '1' THEN '是' 
                                 ELSE '否' 
                               END                                                     AS FLUSHBACKNAME, 
                               A.BOX_CTRL_NO, 
                               A.PALLET_CTRL_NO,
                               A.UNMOVE_STOCK_QTY,
                               {(checkTool == "0" ? "NULL" : "A.DEVICE_STOCK_QTY")} DEVICE_STOCK_QTY,
                               B.CUST_ITEM_CODE,
                               B.EAN_CODE1,
                               B.EAN_CODE2,
                               B.EAN_CODE3
                        FROM   F140105 A 
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
        public void DeleteF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
           string itemCode, DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
        {
            List<object> param = new List<object>() { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, validDate, enterDate, boxCtrlNo, palletCtrlNo };

            var sql = @"
                        DELETE FROM F140105
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
            //else
            //{
            //    sql = string.Format("{0} {1}", sql, " AND A.MAKE_NO is Null ");
            //}

            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
