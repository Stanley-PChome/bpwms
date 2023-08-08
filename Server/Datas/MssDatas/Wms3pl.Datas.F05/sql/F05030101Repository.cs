using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05030101Repository : RepositoryBase<F05030101, Wms3plDbContext, F05030101Repository>
    {
        /// <summary>
		/// 取得虛擬儲位查詢結果
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="dcCode">物流中心</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="custOrdNo">貨主單號</param>
		/// <param name="ordNo">訂單編號</param>
		/// <returns></returns>
		public IQueryable<F05030101Ex> GetP060103Data(string gupCode, string custCode, string dcCode, DateTime delvDate, string pickTime, string custOrdNo, string ordNo, string itemCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", gupCode),
                new SqlParameter("@p1", custCode),
                new SqlParameter("@p2", dcCode),
                new SqlParameter("@p3", delvDate),
                new SqlParameter("@p4", string.IsNullOrEmpty(pickTime) ? (object)DBNull.Value:pickTime),
                new SqlParameter("@p5", string.IsNullOrEmpty( custOrdNo)? (object)DBNull.Value:custOrdNo),
                new SqlParameter("@p6", string.IsNullOrEmpty( ordNo)? (object)DBNull.Value:ordNo),
                new SqlParameter("@p7", string.IsNullOrEmpty( itemCode)? (object)DBNull.Value:itemCode)
            };

            var sql = @"-- 2015/05/16 Walter 虛擬儲位回復查詢
                        -- F05030101 訂單與出貨單的關聯表 A
                        -- F050301 訂單主檔 B
                        -- F050801 出貨主檔 D
                        -- F051202 揀貨明細 F
                        -- F1511 虛擬儲位 G
                        -- 虛擬儲位查詢畫面需要( SELECT )業主、貨主、物流中心、批次日期、批次時段、貨主單號、訂單編號
                        -- 查詢邏輯：只要訂單處置狀態為取消，或者出貨單處置狀態為取消，且虛擬儲位尚未取消，且揀貨單狀態已開始揀貨的，就列出該訂單於畫面上供使用者選擇回復虛擬儲位.

                        SELECT A.DC_CODE, A.GUP_CODE, A.CUST_CODE, D.DELV_DATE, D.PICK_TIME, B.CUST_ORD_NO, A.ORD_NO
                        FROM F05030101 A
                        JOIN F050301 B ON A.ORD_NO = B.ORD_NO
                            AND A.CUST_CODE = B.CUST_CODE 
                            AND A.GUP_CODE = B.GUP_CODE 
                            AND A.DC_CODE = B.DC_CODE
                        JOIN F050801 D ON A.WMS_ORD_NO = D.WMS_ORD_NO
                            AND A.CUST_CODE = D.CUST_CODE 
                            AND A.GUP_CODE = D.GUP_CODE 
                            AND A.DC_CODE = D.DC_CODE
                        JOIN F051202 E ON D.WMS_ORD_NO = E.WMS_ORD_NO
                            AND D.CUST_CODE = E.CUST_CODE 
                            AND D.GUP_CODE = E.GUP_CODE 
                            AND D.DC_CODE = E.DC_CODE
                        JOIN F051201 F ON E.CUST_CODE = F.CUST_CODE 
                            AND E.GUP_CODE = F.GUP_CODE 
                            AND E.DC_CODE = F.DC_CODE
                            AND E.PICK_ORD_NO = F.PICK_ORD_NO
                        JOIN F1511 G ON E.PICK_ORD_NO = G.ORDER_NO
                            AND E.PICK_ORD_SEQ = G.ORDER_SEQ
                            AND E.CUST_CODE = G.CUST_CODE 
                            AND E.GUP_CODE = G.GUP_CODE
                            AND E.DC_CODE = G.DC_CODE
                        WHERE ( B.PROC_FLAG = '9' Or D.STATUS = 9 )     -- PROC_FLAG 處置狀態 9 = 取消, STATUS 9 = 不出貨
                        AND G.STATUS <> '9'                             -- 狀態 9 = 取消
                        --AND F.PICK_STATUS <> '0'						-- 5/16 揀貨單狀態必須開始揀貨才會建立調撥單
						AND A.GUP_CODE = @p0                            -- 以下為查詢過濾條件
						AND A.CUST_CODE = @p1
						AND A.DC_CODE = @p2
						AND D.DELV_DATE = @p3
						AND D.PICK_TIME = CASE WHEN @p4 IS NULL THEN D.PICK_TIME ELSE @p4 END
						AND ISNULL(B.CUST_ORD_NO,'') = CASE WHEN @p5 IS NULL THEN ISNULL(B.CUST_ORD_NO,'') ELSE  @p5 END
						AND A.ORD_NO = CASE WHEN @p6 IS NULL THEN A.ORD_NO ELSE @p6 END
						AND E.ITEM_CODE = CASE WHEN @p7 IS NULL THEN E.ITEM_CODE ELSE @p7 END
						GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, D.DELV_DATE, D.PICK_TIME, B.CUST_ORD_NO, A.ORD_NO
						ORDER BY D.DELV_DATE, D.PICK_TIME, B.CUST_ORD_NO, A.ORD_NO";

            var query = SqlQuery<F05030101Ex>(sql, parameters.ToArray());
            return query;
        }

    /// <summary>
    /// 將選擇訂單關聯的出貨單下的所有訂單與揀貨資料撈出來
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="dcCode"></param>
    /// <param name="ordNoList"></param>
    /// <returns></returns>
    public IQueryable<F05030101WithF051202> GetF05030101WithF051202Data(string gupCode, string custCode, string dcCode, List<string> ordNoList)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", dcCode)   { SqlDbType = SqlDbType.VarChar },
      };

      int paramStartIndex = parameters.Count;
      var inSql = parameters.CombineSqlInParameters("A1.ORD_NO", ordNoList, ref paramStartIndex, SqlDbType.VarChar);
      var sql = @"-- 2015/5/19 Walter 要回復虛擬儲位的訂單條件查詢
                        -- F05030101 訂單與出貨單的關聯表
                        -- F050301 訂單主檔
                        -- F050801 出貨主檔
                        -- F051202 揀貨明細
                        -- F1511 虛擬儲位
                        -- 1912 倉別
                        -- F151001 調撥單主檔
                        -- F151002 調撥單明細
                        SELECT A.ORD_NO, A.WMS_ORD_NO, G.ORDER_NO, 
							   G.ORDER_SEQ, F.ITEM_CODE, F.SERIAL_NO, 
							   F.VALID_DATE, F.VNR_CODE, F.PICK_LOC, 
							   H.WAREHOUSE_ID, G.A_PICK_QTY, A.GUP_CODE, 
							   A.CUST_CODE, A.DC_CODE, B.PROC_FLAG, 
							   D.STATUS AS F581_STATUS,F.ENTER_DATE,
							   G.BOX_CTRL_NO,G.PALLET_CTRL_NO,B.FOREIGN_WMSNO,F.MAKE_NO, G.STATUS F1511_STATUS,
                 B.CUST_COST AS F050301_CUST_COST
                        FROM F05030101 A
                        JOIN F050301 B ON A.ORD_NO = B.ORD_NO
                            AND A.CUST_CODE = B.CUST_CODE 
                            AND A.GUP_CODE = B.GUP_CODE 
                            AND A.DC_CODE = B.DC_CODE
                        JOIN F050801 D ON A.WMS_ORD_NO = D.WMS_ORD_NO
                            AND A.CUST_CODE = D.CUST_CODE 
                            AND A.GUP_CODE = D.GUP_CODE 
                            AND A.DC_CODE = D.DC_CODE
                        JOIN F051202 F ON D.WMS_ORD_NO = F.WMS_ORD_NO
                            AND D.CUST_CODE = F.CUST_CODE 
                            AND D.GUP_CODE = F.GUP_CODE 
                            AND D.DC_CODE = F.DC_CODE
                         JOIN F051201 E ON F.CUST_CODE = E.CUST_CODE 
                            AND F.GUP_CODE = E.GUP_CODE 
                            AND F.DC_CODE = E.DC_CODE
                            AND F.PICK_ORD_NO = E.PICK_ORD_NO
                        JOIN F1511 G ON F.PICK_ORD_NO = G.ORDER_NO
                            AND F.PICK_ORD_SEQ = G.ORDER_SEQ
                            AND A.CUST_CODE = G.CUST_CODE 
                            AND A.GUP_CODE = G.GUP_CODE
                            AND A.DC_CODE = G.DC_CODE
                        JOIN F1912 H ON F.PICK_LOC = H.LOC_CODE
						--20170704 因為F1912 加上CUST_CODE ='0' 或 GUP_CODE ='0' 共用狀態, 所以增加下方H.CUST_CODE= '0'與  H.GUP_CODE = '0'的判斷
                            AND (A.CUST_CODE = H.CUST_CODE  OR H.CUST_CODE = '0')
                            AND (A.GUP_CODE = H.GUP_CODE OR H.GUP_CODE = '0')
                            AND A.DC_CODE = H.DC_CODE
                        WHERE G.STATUS <> '9'                                        -- 狀態 9 = 取消
                        --AND E.PICK_STATUS > 0                                       -- 5/16 揀貨單狀態必須開始揀貨才會建立調撥單
                        --AND G.A_PICK_QTY > 0                                         -- 5/16 沒有實揀數的不用放到調撥單
                        AND A.GUP_CODE = @p0                                         -- 以下為查詢過濾條件
                        AND A.CUST_CODE = @p1
                        AND A.DC_CODE = @p2
                        AND A.WMS_ORD_NO IN (SELECT A1.WMS_ORD_NO                     -- 將該訂單關聯的出貨單下的所有訂單撈出來
                                             FROM F05030101 A1 
                                             WHERE A.GUP_CODE = A1.GUP_CODE
                                             AND A.CUST_CODE = A1.CUST_CODE
                                             AND A.DC_CODE = A1.DC_CODE
                                             AND " + inSql + ")";

      var query = SqlQuery<F05030101WithF051202>(sql, parameters.ToArray());
      return query;
    }

        public IQueryable<GetPickNosRes> GetPickNos(List<string> ordNos)
        {
            var sql = $@"
                        SELECT DISTINCT B.WMS_ORD_NO WmsOrdNo, B.DC_CODE DcCode, B.GUP_CODE GupCode, B.CUST_CODE CustCode, B.PICK_ORD_NO PickOrdNo, F.WAREHOUSE_ID WarehouseId
                        FROM F05030101 A 
                        JOIN F050801 B
                        ON B.DC_CODE = A.DC_CODE
                        AND B.GUP_CODE = A.GUP_CODE
                        AND B.CUST_CODE = A.CUST_CODE
                        AND B.WMS_ORD_NO = A.WMS_ORD_NO
                        JOIN F050802 C
                        ON C.DC_CODE = B.DC_CODE
                        AND C.GUP_CODE = B.GUP_CODE
                        AND C.CUST_CODE = B.CUST_CODE
                        AND C.WMS_ORD_NO = B.WMS_ORD_NO
                        JOIN F051202 D
                        ON D.DC_CODE = C.DC_CODE
                        AND D.GUP_CODE = C.GUP_CODE
                        AND D.CUST_CODE = C.CUST_CODE
                        AND D.WMS_ORD_NO = C.WMS_ORD_NO
                        JOIN F1912 E
                        ON E.DC_CODE = D.DC_CODE
                        AND E.LOC_CODE = D.PICK_LOC
                        JOIN F1980 F
                        ON F.DC_CODE = E.DC_CODE
                        AND F.WAREHOUSE_ID = E.WAREHOUSE_ID
                        WHERE A.ORD_NO IN ('{string.Join("','", ordNos)}')
                        AND ISNULL(F.DEVICE_TYPE,0) <> 0 
                        ";

            var result = SqlQuery<GetPickNosRes>(sql);
            return result;
        }

		public IQueryable<F05030101> GetOrdNoDatas(string dcCode,string gupCode,string custCode,List<string> ordNos)
		{
			var parameters = new List<object>
			{
				dcCode,
				 gupCode,
				 custCode,
				 
			};

			var sql = @"SELECT * FROM F05030101 A
						WHERE A.DC_CODE = @p0
						AND A.GUP_CODE = @p1
						AND A.CUST_CODE = @p2";

			sql += parameters.CombineSqlInParameters(" AND A.ORD_NO ", ordNos);
			var result = SqlQuery<F05030101>(sql, parameters.ToArray());
			return result;
		}

		public IQueryable<F050305> GetOrderRtnInsertDatas(string dcCode, string gupCode, string custCode, string status, List<string> ordNos)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0",status){SqlDbType = SqlDbType.Char},
				new SqlParameter("@p1",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",custCode){SqlDbType = SqlDbType.VarChar},
			};

			var sql = @"SELECT Distinct A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,B.SOURCE_NO,B.SOURCE_TYPE,@p0 As STATUS,'0' As PROC_FLAG,A.WMS_ORD_NO
										FROM F05030101 A
										JOIN F050301 B On A.DC_CODE=B.DC_CODE And A.GUP_CODE=B.GUP_CODE And A.CUST_CODE=B.CUST_CODE And A.ORD_NO=B.ORD_NO
                    JOIN F050801 C ON C.DC_CODE = A.DC_CODE AND C.GUP_CODE = A.GUP_CODE AND C.CUST_CODE = A.CUST_CODE AND C.WMS_ORD_NO = A.WMS_ORD_NO
									 WHERE Not Exists(Select C.ORD_NO From F050305 C
									                   Where A.DC_CODE=C.DC_CODE
									                     And A.GUP_CODE=C.GUP_CODE
									                     And A.CUST_CODE=C.CUST_CODE
									                     And A.ORD_NO=C.ORD_NO
									                     And C.STATUS=@p0)
                     AND C.STATUS <> '9' 
									   And A.DC_CODE = @p1
									   AND A.GUP_CODE = @p2
									   AND A.CUST_CODE = @p3";

			sql += parameters.CombineSqlInParameters(" AND A.WMS_ORD_NO ", ordNos, SqlDbType.VarChar);
			var result = SqlQuery<F050305>(sql, parameters.ToArray());
			return result;
		}

		public string GetFirstWmsOrdNoBySourceNo(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var parameters = new List<object>
			{
				dcCode,
				gupCode,
				custCode,
				sourceNo
			};

			var sql = @"SELECT A.WMS_ORD_NO 
									FROM F05030101 A
									JOIN F050301 B
									ON A.DC_CODE = B.DC_CODE
									AND A.GUP_CODE = B.GUP_CODE
									AND A.CUST_CODE = B.CUST_CODE
									AND A.ORD_NO = B.ORD_NO
									WHERE B.DC_CODE = @p0
									AND B.GUP_CODE = @p1
									AND B.CUST_CODE = @p2
									AND B.SOURCE_NO = @p3 ";

			return SqlQuery<string>(sql, parameters.ToArray()).FirstOrDefault();
		}

		public BoxHeaderData GetBoxHeaderData(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode){SqlDbType = System.Data.SqlDbType.VarChar },
								new SqlParameter("@p1", gupCode){SqlDbType = System.Data.SqlDbType.VarChar },
								new SqlParameter("@p2", custCode){SqlDbType = System.Data.SqlDbType.VarChar },
								new SqlParameter("@p3", wmsOrdNo){SqlDbType = System.Data.SqlDbType.VarChar },
						};
			var sql = @" SELECT TOP 1 C.ORDER_PROC_TYPE,C.ORDER_ZIP_CODE,C.IS_NORTH_ORDER,D.PRINT_MEMO,D.PRINT_CUST_ORD_NO,C.CUST_ORD_NO, C.CRT_DATE
										 FROM F05030101 A
										 JOIN F050301 B
										   ON B.DC_CODE = A.DC_CODE
										  AND B.GUP_CODE=A.GUP_CODE
										  AND B.CUST_CODE = A.CUST_CODE
										  AND B.ORD_NO = A.ORD_NO
										 LEFT JOIN F050101 C
										   ON C.DC_CODE = A.DC_CODE
										  AND C.GUP_CODE = A.GUP_CODE
										  AND C.CUST_CODE = A.CUST_CODE
										  AND C.ORD_NO = A.ORD_NO
										 LEFT JOIN F050103 D
										   ON D.DC_CODE = A.DC_CODE
										  AND D.GUP_CODE = A.GUP_CODE
										  AND D.CUST_CODE = A.CUST_CODE
										  AND D.ORD_NO = A.ORD_NO
										WHERE A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND A.WMS_ORD_NO = @p3
                   ";
			return SqlQuery<BoxHeaderData>(sql, parameters.ToArray()).FirstOrDefault();
		}

	}
}