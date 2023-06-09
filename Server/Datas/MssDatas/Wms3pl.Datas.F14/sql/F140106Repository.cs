using System.Data.SqlClient;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140106Repository : RepositoryBase<F140106, Wms3plDbContext, F140106Repository>
	{
		public void InsertFromF140104AndF140105(bool isSecond, string dcCode, string gupCode, string custCode, string inventorNo, string userId, string userName, string checkTool)
		{

			var sql = $@"
                        INSERT INTO F140106 
                                    (INVENTORY_NO, 
                                     WAREHOUSE_ID, 
                                     LOC_CODE, 
                                     ITEM_CODE, 
                                     VALID_DATE, 
                                     ENTER_DATE, 
                                     QTY, 
                                     FIRST_QTY, 
                                     SECOND_QTY, 
                                     FIRST_DIFF_QTY, 
                                     SECOND_DIFF_QTY, 
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
																		 FIRST_STOCK_DIFF_QTY,
																		 SECOND_STOCK_DIFF_QTY,
																		 UNMOVE_STOCK_QTY,
																		 DEVICE_STOCK_QTY,
																		 STATUS) 
											SELECT 
											Z.INVENTORY_NO, 
											Z.WAREHOUSE_ID, 
											Z.LOC_CODE, 
											Z.ITEM_CODE, 
											Z.VALID_DATE, 
											Z.ENTER_DATE, 
											Z.QTY, 
											Z.FIRST_QTY, 
											Z.SECOND_QTY, 
											Z.FIRST_DIFF_QTY, 
											Z.SECOND_DIFF_QTY, 
											Z.FLUSHBACK, 
											Z.DC_CODE, 
											Z.GUP_CODE, 
											Z.CUST_CODE, 
											Z.FST_INVENTORY_STAFF, 
											Z.FST_INVENTORY_NAME, 
											Z.FST_INVENTORY_DATE, 
											Z.FST_INVENTORY_PC, 
											Z.SEC_INVENTORY_STAFF, 
											Z.SEC_INVENTORY_NAME, 
											Z.SEC_INVENTORY_DATE, 
											Z.SEC_INVENTORY_PC, 
											Z.BOX_CTRL_NO, 
											Z.PALLET_CTRL_NO, 
											Z.CRT_STAFF, 
											Z.CRT_NAME, 
											Z.CRT_DATE, 
											Z.UPD_STAFF, 
											Z.UPD_NAME, 
											Z.UPD_DATE, 
											Z.MAKE_NO,
											Z.FIRST_STOCK_DIFF_QTY,
											Z.SECOND_STOCK_DIFF_QTY,
											Z.UNMOVE_STOCK_QTY,
											Z.DEVICE_STOCK_QTY,
											{(checkTool == "0" ? "1" : " (CASE WHEN Z.FIRST_DIFF_QTY <> 0 OR Z.SECOND_DIFF_QTY <> 0 THEN 0 WHEN Z.FIRST_DIFF_QTY = 0 OR Z.SECOND_DIFF_QTY = 0 THEN 1 END) ")} STATUS
											FROM (
                        SELECT INVENTORY_NO, 
                               WAREHOUSE_ID, 
                               LOC_CODE, 
                               ITEM_CODE, 
                               VALID_DATE, 
                               ENTER_DATE, 
                               QTY, 
                               ISNULL(FIRST_QTY, 0) FIRST_QTY, 
                               {(isSecond ? "ISNULL(SECOND_QTY,0)" : "SECOND_QTY")} SECOND_QTY, 
                               (ISNULL(FIRST_QTY, 0) - DEVICE_STOCK_QTY) FIRST_DIFF_QTY, 
                               {(isSecond ? "(ISNULL(SECOND_QTY,0) - DEVICE_STOCK_QTY)" : "CASE WHEN SECOND_QTY IS NULL THEN SECOND_QTY ELSE SECOND_QTY - DEVICE_STOCK_QTY END")} SECOND_DIFF_QTY, 
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
                               @p4 CRT_STAFF, 
                               @p5 CRT_NAME, 
                               dbo.GetSysDate() CRT_DATE, 
                               @p4 UPD_STAFF, 
                               @p5 UPD_NAME, 
                               dbo.GetSysDate() UPD_DATE, 
                               MAKE_NO,
															 {(isSecond ? "NULL" : "(ISNULL(FIRST_QTY, 0) - QTY - ISNULL(UNMOVE_STOCK_QTY, 0)) ")} FIRST_STOCK_DIFF_QTY,
															 {(isSecond ? "(ISNULL(SECOND_QTY, 0) - QTY - ISNULL(UNMOVE_STOCK_QTY, 0)) " : "NULL")} SECOND_STOCK_DIFF_QTY,
															 UNMOVE_STOCK_QTY,
															 DEVICE_STOCK_QTY
                        FROM   {(isSecond ? "F140105" : "F140104")} 
                        WHERE  DC_CODE = @p0 
                           AND GUP_CODE = @p1 
                           AND CUST_CODE = @p2 
                           AND INVENTORY_NO = @p3 
                           AND QTY <> {(isSecond ? "ISNULL(SECOND_QTY,0)" : "ISNULL(FIRST_QTY,0)")} 
                        ) Z
												";
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
	}
}
