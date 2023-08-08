
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
  public partial class F051202Repository : RepositoryBase<F051202, Wms3plDbContext, F051202Repository>
  {
    public IQueryable<F051202Data> GetF051202Datas(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string ordType)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", delvDate),
                new SqlParameter("@p4", pickTime),
                new SqlParameter("@p5", ordType),
            };

      var sql = @"SELECT C.WAREHOUSE_NAME,
                                				C.TMPR_TYPE, 
                             B.FLOOR, 
                             COUNT(DISTINCT A.ITEM_CODE) ITEMCOUNT, 
                                       SUM(ISNULL(A.B_PICK_QTY,0)) TOTALPICK_QTY 
                        FROM F051202 A 
                       INNER JOIN F1912 B ON  B.DC_CODE = A.DC_CODE AND B.LOC_CODE = A.PICK_LOC 
                       INNER JOIN F1980 C ON C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE = @p0 
                       WHERE EXISTS ( 
                       			SELECT 1 
                               FROM F051201 D 
                              WHERE D.PICK_ORD_NO = A.PICK_ORD_NO 
                                         AND D.DC_CODE = @p0 
                                         AND D.GUP_CODE = @p1 
                                         AND D.CUST_CODE = @p2 
                                         AND D.DELV_DATE = @p3
                               AND D.PICK_TIME = @p4 
                                          AND D.ORD_TYPE = @p5 
                                          AND D.ISDEVICE = '0'  --排除臺車揀次
                                    ) 
                                   AND EXISTS (
                                       SELECT 1 
                               FROM F050801 F 
                                        INNER JOIN F05030101 G 
                                           ON G.DC_CODE = F.DC_CODE 
                                          AND G.GUP_CODE = F.GUP_CODE 
                                          AND G.CUST_CODE = F.CUST_CODE 
                                          AND G.WMS_ORD_NO = F.WMS_ORD_NO 
                                        INNER JOIN F050301 H 
                                           ON H.DC_CODE = G.DC_CODE 
                                          AND H.GUP_CODE = G.GUP_CODE 
                                          AND H.CUST_CODE = G.CUST_CODE 
                                          AND H.ORD_NO = G.ORD_NO 
                                        WHERE F.VIRTUAL_ITEM <>'1'  --排除虛擬商品出貨單
                                          AND H.PROC_FLAG <>'9'     --排除取消的訂單
                                          AND F.DC_CODE = A.DC_CODE 
                                          AND F.GUP_CODE = A.GUP_CODE
                                          AND F.CUST_CODE = A.CUST_CODE
                                          AND F.WMS_ORD_NO = A.WMS_ORD_NO) 
             GROUP BY C.WAREHOUSE_NAME,C.TMPR_TYPE,B.FLOOR ";

      var result = SqlQuery<F051202Data>(sql, parameters.ToArray());

      return result;
    }

    /// <summary>
    /// 取得尚未訂單取消且尚未揀貨完成的揀貨單明細
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <param name="itemCode"></param>
    /// <param name="validDate"></param>
    /// <param name="pickLoc"></param>
    /// <param name="serialNo"></param>
    /// <returns></returns>
    public IQueryable<F051202> GetF051202ByOrderNonCancel(string dcCode, string gupCode, string custCode, string pickOrdNo, string itemCode, DateTime validDate, string pickLoc, string serialNo)
    {
      var parameters = new object[] { pickOrdNo, dcCode, gupCode, custCode, itemCode, validDate, pickLoc, serialNo };

      var sql = @"SELECT C.*
							FROM F051202 C
								 JOIN F050801 W
									ON     C.WMS_ORD_NO = W.WMS_ORD_NO
									   AND C.DC_CODE = W.DC_CODE
									   AND C.GUP_CODE = W.GUP_CODE
									   AND C.CUST_CODE = W.CUST_CODE
						   WHERE     C.PICK_ORD_NO = @p0
								 AND C.DC_CODE = @p1
								 AND C.GUP_CODE = @p2
								 AND C.CUST_CODE = @p3
								 AND C.ITEM_CODE = @p4
								 AND C.VALID_DATE = @p5
								 AND C.PICK_LOC = @p6
								 AND ISNULL (C.SERIAL_NO, ' ') = ISNULL ( @p7, ' ')
								 AND C.PICK_STATUS = '0'                                -- 取得尚未訂單取消且尚未揀貨完成的揀貨單明細
								 AND W.VIRTUAL_ITEM <> '1'
								 AND NOT EXISTS
											(SELECT 1
											   FROM F05030101 D
													INNER JOIN F050301 E
													   ON     E.DC_CODE = D.DC_CODE
														  AND E.GUP_CODE = D.GUP_CODE
														  AND E.CUST_CODE = D.CUST_CODE
														  AND E.ORD_NO = D.ORD_NO
											  WHERE     E.DC_CODE = C.DC_CODE
													AND E.GUP_CODE = C.GUP_CODE
													AND E.CUST_CODE = C.CUST_CODE
													AND D.WMS_ORD_NO = C.WMS_ORD_NO
													AND E.PROC_FLAG = '9')
						ORDER BY C.WMS_ORD_NO";

      var result = SqlQuery<F051202>(sql, parameters);

      return result;
    }

    public IQueryable<F051202> GetF051202ByOrderNonCancelAllWmsOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo, string itemCode)
    {
      var parameters = new object[] { pickOrdNo, dcCode, gupCode, custCode, itemCode };
      var sql = @"SELECT C.*
							FROM F051202 C
								 JOIN F050801 W
									ON     C.WMS_ORD_NO = W.WMS_ORD_NO
									   AND C.DC_CODE = W.DC_CODE
									   AND C.GUP_CODE = W.GUP_CODE
									   AND C.CUST_CODE = W.CUST_CODE
						   WHERE     C.PICK_ORD_NO = @p0
								 AND C.DC_CODE = @p1
								 AND C.GUP_CODE = @p2
								 AND C.CUST_CODE = @p3
								 AND C.ITEM_CODE = @p4
								 AND C.PICK_STATUS = '0'                                -- 取得尚未訂單取消且尚未揀貨完成的揀貨單明細
								 AND W.VIRTUAL_ITEM <> '1'
								 AND NOT EXISTS
											(SELECT 1
											   FROM F05030101 D
													INNER JOIN F050301 E
													   ON     E.DC_CODE = D.DC_CODE
														  AND E.GUP_CODE = D.GUP_CODE
														  AND E.CUST_CODE = D.CUST_CODE
														  AND E.ORD_NO = D.ORD_NO
											  WHERE     E.DC_CODE = C.DC_CODE
													AND E.GUP_CODE = C.GUP_CODE
													AND E.CUST_CODE = C.CUST_CODE
													AND D.WMS_ORD_NO = C.WMS_ORD_NO
													AND E.PROC_FLAG = '9')
						ORDER BY C.WMS_ORD_NO";

      var result = SqlQuery<F051202>(sql, parameters);

      return result;
    }

    public IQueryable<GetPickDetailDetail> GetSinglePickDetail(
        string dcNo, string gupCode, string custNo, string pickOrdNo)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcNo),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custNo),
                new SqlParameter("@p3",pickOrdNo)
            };

      var sql = $@"SELECT 
                        	A.PICK_ORD_NO AS WmsNo,
                          A.PICK_ORD_NO AS PickNo,
                        	A.PICK_ORD_SEQ AS WmsSeq,
                        	A.ITEM_CODE AS ItemNo,
                        	C.WAREHOUSE_NAME AS WHName,
                        	A.PICK_LOC AS Loc,
                        	A.VALID_DATE AS ValidDate,
                        	A.ENTER_DATE AS EnterDate,
                        	A.B_PICK_QTY AS ShipQty,
                        	CASE WHEN A.SERIAL_NO = '0' THEN '' ELSE A.SERIAL_NO END AS Sn,
                        	ISNULL(A.MAKE_NO,'') AS MkNo,
                        	ISNULL(A.PALLET_CTRL_NO,'') AS PalletNo,
                          A.CRT_DATE AS CrtDate,
                          A.ROUTE_SEQ AS Route
                        FROM F051202 A
                        JOIN F1912 B ON A.DC_CODE = B.DC_CODE 
                        			AND A.PICK_LOC = B.LOC_CODE 
                        JOIN F1980 C ON B.DC_CODE = C.DC_CODE 
                        			AND B.WAREHOUSE_ID = C.WAREHOUSE_ID 
                        WHERE A.DC_CODE = @p0
                          AND A.GUP_CODE = @p1
                          AND A.CUST_CODE = @p2
                          AND A.PICK_ORD_NO = @p3
                          AND A.PICK_STATUS = '0'
                        ORDER BY A.ROUTE_SEQ,A.ITEM_CODE";

      var result = SqlQuery<GetPickDetailDetail>(sql, parameters.ToArray());
      return result;
    }

    public IQueryable<GetPickDetailDetail> GetSinglePickDetailAllCol(string dcNo, string gupCode, string custNo, string pickOrdNo)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcNo),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custNo),
                new SqlParameter("@p3",pickOrdNo)
            };

      var sql = $@"SELECT 
                        A.PICK_ORD_NO AS WmsNo,
                        A.PICK_ORD_NO AS PickNo,
                        A.PICK_ORD_SEQ AS WmsSeq,
                        A.ITEM_CODE AS ItemNo,
                        C.WAREHOUSE_NAME AS WHName,
                        A.PICK_LOC AS Loc,
                        A.VALID_DATE AS ValidDate,
                        A.ENTER_DATE AS EnterDate,
                        A.B_PICK_QTY AS ShipQty,
                        CASE WHEN A.SERIAL_NO = '0' THEN '' ELSE A.SERIAL_NO END AS Sn,
                        ISNULL(A.MAKE_NO,'') AS MkNo,
                        ISNULL(A.PALLET_CTRL_NO,'') AS PalletNo,
                        A.CRT_DATE AS CrtDate,
                        A.ROUTE_SEQ AS Route,
                        D.ITEM_UNIT Unit,        
                        D.ITEM_NAME ProductName ,        
                        D.ITEM_SIZE ProductSize ,        
                        D.ITEM_COLOR ProductColor,        
                        D.ITEM_SPEC ProductSpec ,        
                        D.EAN_CODE1 Barcode1,        
                        D.EAN_CODE2 Barcode2 ,        
                        D.EAN_CODE3 Barcode3 ,        
                        E.PACK_WEIGHT Weight,        
                        D.CTNS BoxQty,        
                        A.CUST_CODE CustNo
                        FROM F051202 A
                        JOIN F1912 B ON A.DC_CODE = B.DC_CODE 
                        			AND A.PICK_LOC = B.LOC_CODE 
                        JOIN F1980 C ON B.DC_CODE = C.DC_CODE 
                        			AND B.WAREHOUSE_ID = C.WAREHOUSE_ID 
                        LEFT JOIN F1903 D ON A.GUP_CODE = D.GUP_CODE
                        			AND A.CUST_CODE = D.CUST_CODE
                        			AND A.ITEM_CODE = D.ITEM_CODE
                        LEFT JOIN F1905 E ON A.GUP_CODE = E.GUP_CODE
                        			AND A.CUST_CODE = E.CUST_CODE
                        			AND A.ITEM_CODE = E.ITEM_CODE
                        WHERE A.DC_CODE = @p0
                          AND A.GUP_CODE = @p1
                          AND A.CUST_CODE = @p2
                          AND A.PICK_ORD_NO = @p3
                          AND A.PICK_STATUS = '0'
                        ORDER BY A.ROUTE_SEQ,A.ITEM_CODE";

      var result = SqlQuery<GetPickDetailDetail>(sql, parameters.ToArray());
      return result;
    }

    public IQueryable<WcsOutboundSkuModel> GetWcsDetail(string dcCode, string gupCode, string custCode, string pickOrdNo, string ordNo, string warehouseId)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", pickOrdNo),
                new SqlParameter("@p4", warehouseId)
            };

      var inSql = string.Empty;
      if (!string.IsNullOrWhiteSpace(ordNo))
      {
        inSql = " AND A.WMS_ORD_NO = @p5 ";
        parameters.Add(new SqlParameter("@p5", ordNo));
      }

      var sql = $@"SELECT 
                         A.PICK_ORD_SEQ RowNum,
                         A.ITEM_CODE SkuCode,
                         A.B_PICK_QTY SkuQty,
                         1 SkuLevel,
                         convert(varchar, A.VALID_DATE, 111) ExpiryDate,
                         A.MAKE_NO OutBatchCode
                         FROM F051202 A
                         JOIN F1903 B
                         ON A.ITEM_CODE = B.ITEM_CODE
                         AND A.GUP_CODE = B.GUP_CODE
                         AND A.CUST_CODE = B.CUST_CODE
												 JOIN F1912 C
												 ON A.DC_CODE = C.DC_CODE
												 AND A.PICK_LOC = C.LOC_CODE
                         WHERE A.DC_CODE = @p0
                         AND A.GUP_CODE = @p1
                         AND A.CUST_CODE = @p2
                         AND A.PICK_ORD_NO = @p3
                         AND A.PICK_STATUS = 0
												 AND C.WAREHOUSE_ID = @p4
                         {inSql}
                         ";

      var result = SqlQuery<WcsOutboundSkuModel>(sql, parameters.ToArray());
      return result;
    }

    public IQueryable<WmsOrdNoCnt> GetWmsOrdNoCnt(string pickOrdNo)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", pickOrdNo)
            };

      var sql = $@"
                        SELECT DISTINCT A.WMS_ORD_NO WmsOrdNo, (SELECT COUNT(1) FROM F050802 B WHERE B.WMS_ORD_NO = A.WMS_ORD_NO) Cnt FROM 
                        F051202 A
                        WHERE A.PICK_ORD_NO = @p0
                         ";

      var result = SqlQuery<WmsOrdNoCnt>(sql, parameters.ToArray());
      return result;
    }

    public IQueryable<F051202> GetDataByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",pickOrdNo){ SqlDbType = SqlDbType.VarChar},
			};
      var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3";
      return SqlQuery<F051202>(sql, parms.ToArray());
    }
		public IQueryable<F051202> GetDataByPickNos(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar},
			};
			
			var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2 ";
			sql += parms.CombineSqlInParameters(" AND PICK_ORD_NO", pickOrdNos, SqlDbType.VarChar);
			return SqlQuery<F051202>(sql, parms.ToArray());
		}

		public IQueryable<F051202> GetDatasByPickNosNotStatus(string dcCode, string gupCode, string custCode, string pickStatus, List<string> pickOrdNos)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",pickStatus){SqlDbType = SqlDbType.Char}
			};
			var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_STATUS <> @p3 ";
			sql += parms.CombineSqlInParameters(" AND PICK_ORD_NO ", pickOrdNos, SqlDbType.VarChar);
			return SqlQuery<F051202>(sql, parms.ToArray());
		}


		public IQueryable<F051202> GetCollectionOutboundDatas()
    {
      var parms = new object[] { };

      var sql = $@"SELECT * FROM F051202 A WHERE EXISTS ( 
										  SELECT 1 
										   FROM F051301 B
										  WHERE B.STATUS = '2' --集貨中
										  AND A.WMS_ORD_NO = B.WMS_NO 
										  AND A.CUST_CODE = B.CUST_CODE
										  AND A.GUP_CODE = B.GUP_CODE
										) AND A.PICK_STATUS = '0' -- 待揀貨";

      var result = SqlQuery<F051202>(sql, parms.ToArray());
      return result;
    }

    public IQueryable<F051202> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
      var sql = @" SELECT *
                     FROM F051202
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND WMS_ORD_NO = @p3";
      return SqlQuery<F051202>(sql, parms.ToArray());
    }

    public IQueryable<F051202> GetDatasByPickOrdNos(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
    {
      var parms = new List<object> { dcCode, gupCode, custCode };
      var sql = @" SELECT * FROM F051202 B
                    WHERE B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2 
								";
      sql += parms.CombineNotNullOrEmptySqlInParameters("AND B.PICK_ORD_NO", pickOrdNos);
      return SqlQuery<F051202>(sql, parms.ToArray());
    }

    public bool IsBatchFinished(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string excludePickOrdNo)
    {
      var parms = new object[] { dcCode, gupCode, custCode, delvDate, pickTime, excludePickOrdNo };
      var sql = @" SELECT A.*
                      FROM F051202 A
                      JOIN F051201 B
                        ON B.DC_CODE = A.DC_CODE
                       AND B.GUP_CODE = A.GUP_CODE
                       AND B.CUST_CODE = A.CUST_CODE
                       AND B.PICK_ORD_NO = A.PICK_ORD_NO
                     WHERE B.DC_CODE = @p0
                       AND B.GUP_CODE = @p1
                       AND B.CUST_CODE = @p2
                       AND B.DELV_DATE = @p3
                       AND B.PICK_TIME = @p4
                       AND B.PICK_ORD_NO <> @p5 
                       AND A.PICK_STATUS='0'";
      return !SqlQuery<F051202>(sql, parms).Any();
    }

    public IQueryable<F051202> GetDatasByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var parms = new List<SqlParameter>()
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
			};
      var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2 ";
      if (wmsOrdNos.Any())
        sql += parms.CombineSqlInParameters(" AND WMS_ORD_NO", wmsOrdNos, SqlDbType.VarChar);

      return SqlQuery<F051202>(sql, parms.ToArray());
    }

		public IQueryable<F051202> GetDatasWithPickByWmsOrdNos(string dcCode, string gupCode, string custCode,string pickOrdNo, List<string> wmsOrdNos)
		{
			var parms = new List<SqlParameter>()
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar },
			};
			var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3 ";
			if (wmsOrdNos.Any())
				sql += parms.CombineSqlInParameters(" AND WMS_ORD_NO", wmsOrdNos, SqlDbType.VarChar);

			return SqlQuery<F051202>(sql, parms.ToArray());
		}


		public bool AnyWmsOrdIntAudit(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var parms = new List<SqlParameter>()
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p3", ordNo) { SqlDbType = SqlDbType.VarChar },
			};

			var sql = @"SELECT TOP(1) 1
                          FROM VW_CROSSORDERISPROCESS
                         WHERE DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2
                           AND ORD_NO = @p3 ";

			return SqlQuery<int>(sql, parms.ToArray()).Any();
		}


    public IQueryable<string> GetCanShipWmsNosByPick(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
      var sql = @" SELECT DISTINCT B.WMS_ORD_NO
                     FROM F051202 A
                     JOIN F050801 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.WMS_ORD_NO = A.WMS_ORD_NO
                    WHERE B.STATUS <> '9'
                      AND A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND A.PICK_ORD_NO = @p3 ";
      return SqlQuery<string>(sql, parms);
    }
    public IQueryable<F051202> GetNotCacnelDataByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var parms = new List<SqlParameter>();
      parms.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
      parms.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
      parms.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
      parms.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

      var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3
                       AND PICK_STATUS <> '9'";
      return SqlQuery<F051202>(sql, parms.ToArray());
    }
    public IQueryable<F051202> GetNotFinishDataByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
      var sql = @" SELECT *
                      FROM F051202
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3
                       AND PICK_STATUS = '0' ";
      return SqlQuery<F051202>(sql, parms);
    }

    // 揀貨單明細資料
    public IQueryable<PickDetail> GetPickDetails(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var param = new object[] { dcCode, gupCode, custCode, dcCode, gupCode, custCode, wmsOrdNo};
      var sql = @"SELECT PICK_ORD_NO, 
						PICK_ORD_SEQ,
						PICK_LOC, 
						ITEM_CODE, 
						B_PICK_QTY, 
						A_PICK_QTY,
						CASE WHEN (SELECT PICK_STATUS FROM F051201 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND PICK_ORD_NO=A.PICK_ORD_NO)='2' AND A.PICK_STATUS='0' THEN '未分貨'
						ELSE (SELECT NAME  FROM F000904 WHERE TOPIC='F051202' AND SUBTOPIC='PICK_STATUS' AND VALUE = PICK_STATUS) END PICK_STATUS,
						MAKE_NO,
						ROUTE_SEQ,
						PK_AREA_NAME,
            SERIAL_NO
						FROM F051202 A
						WHERE DC_CODE = @p3
						AND GUP_CODE = @p4
						AND CUST_CODE= @p5
						AND WMS_ORD_NO = @p6";
      var result = SqlQuery<PickDetail>(sql, param);
      return result;
    }

    // 取得揀貨單對應的出貨單單號及狀態
    public IQueryable<F051202WithF050801> GetOrderStatusByPickNos(string dcCode, string gupCode, string custCode, List<string> pickOrdNoList)
    {
      var param = new List<SqlParameter> {
                                        new SqlParameter("@p0",SqlDbType.VarChar){Value= dcCode },
                                        new SqlParameter("@p1",SqlDbType.VarChar){Value= gupCode },
                                        new SqlParameter("@p2",SqlDbType.VarChar){Value= custCode },
                                };
			var sqlIn = param.CombineSqlInParameters("AND a.PICK_ORD_NO ", pickOrdNoList, SqlDbType.VarChar);

      var sql = $@"
SELECT
	a.DC_CODE,
	a.GUP_CODE,
	a.CUST_CODE,
	a.PICK_ORD_NO,
	a.WMS_ORD_NO,
	b.STATUS
FROM F051202 a
	JOIN F050801 b
	ON a.DC_CODE=b.DC_CODE
		AND a.GUP_CODE = b.GUP_CODE
		AND a.CUST_CODE = b.CUST_CODE
		AND a.WMS_ORD_NO = b.WMS_ORD_NO
WHERE a.DC_CODE = @p0 AND a.GUP_CODE = @p1 AND a.CUST_CODE = @p2 {sqlIn}
GROUP BY
	a.DC_CODE,
	a.GUP_CODE,
	a.CUST_CODE,
	a.PICK_ORD_NO,
	a.WMS_ORD_NO,
	b.STATUS";
      var result = SqlQuery<F051202WithF050801>(sql, param.ToArray());
      return result;
    }


    public IQueryable<F051202WithF055002> GetF051202WithF055002s(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode){SqlDbType=SqlDbType.VarChar},
                new SqlParameter("@p1", gupCode){SqlDbType=SqlDbType.VarChar},
                new SqlParameter("@p2", custCode){SqlDbType=SqlDbType.VarChar},
                new SqlParameter("@p3", wmsOrdNo){SqlDbType=SqlDbType.VarChar}
            };

      var sql = @"SELECT A.ITEM_CODE,A.SERIAL_NO
                        FROM F055002 A
                        WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND A.WMS_ORD_NO = @p3
                            AND A.SERIAL_NO IS NOT NULL
                            AND A.SERIAL_NO <> ''";

      var result = SqlQuery<F051202WithF055002>(sql, parameters.ToArray());

      return result;
    }

		public IQueryable<PickWithWmsMap> GetWmsOrdNoListByPickOrdNos(string dcCode,string gupCode,string custCode,List<string> pickOrdNos)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode){SqlDbType=SqlDbType.VarChar},
								new SqlParameter("@p1", gupCode){SqlDbType=SqlDbType.VarChar},
								new SqlParameter("@p2", custCode){SqlDbType=SqlDbType.VarChar},
						};
			var sql = @" SELECT DISTINCT PICK_ORD_NO,WMS_ORD_NO 
                     FROM F051202 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
			sql += parameters.CombineSqlInParameters("AND PICK_ORD_NO", pickOrdNos,SqlDbType.VarChar);
			return SqlQuery<PickWithWmsMap>(sql, parameters.ToArray());
		}

    public IQueryable<F051202> GetDatasByWmsOrdNosAndPickStatus1(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @" 
SELECT
	*
FROM
	F051202
WHERE
	DC_CODE = @p0
	AND GUP_CODE = @p1
	AND CUST_CODE = @p2
	AND PICK_STATUS = '1'";

      sql += parameters.CombineSqlInParameters("AND WMS_ORD_NO", wmsOrdNos, SqlDbType.VarChar);

      return SqlQuery<F051202>(sql, parameters.ToArray());

      #region 原LINQ語法
      /*
      return _db.F051202s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      wmsOrdNos.Contains(x.WMS_ORD_NO) &&
      x.PICK_STATUS == "1");
      */
      #endregion
    }

  }
}
