using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using System.Data;
using System.Text;

namespace Wms3pl.Datas.F19
{
	public partial class F1913Repository : RepositoryBase<F1913, Wms3plDbContext, F1913Repository>
	{
		public IQueryable<StockData> GetStockDataForP910101(string dcCode, string gupCode, string custCode, string processNo)
		{
			var param = new[] {
										new SqlParameter("@p0", dcCode),
										new SqlParameter("@p1", gupCode),
										new SqlParameter("@p2", custCode),
										new SqlParameter("@p3", processNo)
						};
			string sql = @"
				SELECT A.ITEM_CODE, A.LOC_CODE, C.WAREHOUSE_ID, C.WAREHOUSE_NAME, C.WAREHOUSE_TYPE, D.TYPE_NAME, SUM(A.QTY) AS QTY, A.VALID_DATE
				  FROM F1913 A
				  JOIN F1912 B ON A.LOC_CODE = B.LOC_CODE AND A.DC_CODE = B.DC_CODE
				  LEFT JOIN F1980 C ON C.DC_CODE = B.DC_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
				  LEFT JOIN F198001 D ON C.WAREHOUSE_TYPE = D.TYPE_ID
				 WHERE A.ITEM_CODE IN (
								/* 要取得某商品加工的品號 */
								SELECT MATERIAL_CODE FROM F910101 A1
								  LEFT JOIN F910102 B1 ON A1.BOM_NO = B1.BOM_NO AND A1.GUP_CODE = B1.GUP_CODE AND A1.CUST_CODE = B1.CUST_CODE
								 WHERE A1.ITEM_CODE IN (
												SELECT ISNULL(B.ITEM_CODE_BOM, B.ITEM_CODE) AS ITEM_CODE
												  FROM F910201 B /* 加工單, 用來取出所需的數量 */
												 WHERE B.DC_CODE = @p0 AND B.GUP_CODE = @p1 AND B.CUST_CODE = @p2 AND B.PROCESS_NO = @p3
										   )
							 )
			     AND A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 /*AND A.VALID_DATE >= TO_CHAR(sysdate, 'yyyy/MM/dd') */
				 GROUP BY A.ITEM_CODE, A.LOC_CODE, C.WAREHOUSE_ID, C.WAREHOUSE_NAME, C.WAREHOUSE_TYPE, D.TYPE_NAME, A.VALID_DATE
			";

			var result = SqlQuery<StockData>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<StockData> GetStockData2ForP910101(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var param = new[] {
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", itemCode),
                                new SqlParameter("@p4", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                        };

			string sql = @"
				SELECT A.ITEM_CODE, A.LOC_CODE, C.WAREHOUSE_ID, C.WAREHOUSE_NAME, C.WAREHOUSE_TYPE, D.TYPE_NAME, SUM(A.QTY) AS QTY, A.VALID_DATE
				  FROM F1913 A
				  JOIN F1912 B ON A.LOC_CODE = B.LOC_CODE AND A.DC_CODE = B.DC_CODE
				  LEFT JOIN F1980 C ON C.DC_CODE = B.DC_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
				  LEFT JOIN F198001 D ON C.WAREHOUSE_TYPE = D.TYPE_ID
				 WHERE A.ITEM_CODE = @p3
			     AND A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND cast(A.VALID_DATE As Date) >= CAST(@p4 As Date)
           AND C.WAREHOUSE_TYPE!='T' AND C.WAREHOUSE_TYPE!='I'
				 GROUP BY A.ITEM_CODE, A.LOC_CODE, C.WAREHOUSE_ID, C.WAREHOUSE_NAME, C.WAREHOUSE_TYPE, D.TYPE_NAME, A.VALID_DATE
			";

			var result = SqlQuery<StockData>(sql, param.ToArray()).AsQueryable();
			return result;
		}


		public int GetItemStockWithoutResupply(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string makeNo = "")
		{
			var param = new List<SqlParameter> {
																new SqlParameter("@p0", gupCode),
																new SqlParameter("@p1", custCode),
																new SqlParameter("@p2", dcCode),
																new SqlParameter("@p3", itemCode),
																new SqlParameter("@p4", warehouseType),
                                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                        };


			var makeNoF1913Cond = string.Empty;
			if (!string.IsNullOrEmpty(makeNo))
			{
				param.Add(new SqlParameter("@p" + param.Count, makeNo));
				makeNoF1913Cond += $@"
					And A.MAKE_NO=@p{param.Count}";
			}

			string sql = @"Select ISNULL(Sum(A.QTY),0)
				  From F1913 A, F1912 B, F1980 C, F1919 D
				 Where A.LOC_CODE=B.LOC_CODE And A.DC_CODE=B.DC_CODE
				   And B.WAREHOUSE_ID=C.WAREHOUSE_ID And B.DC_CODE=C.DC_CODE
				   And B.WAREHOUSE_ID=D.WAREHOUSE_ID And B.DC_CODE=D.DC_CODE And B.AREA_CODE=D.AREA_CODE
				   And A.VALID_DATE>=@p5
				   And A.GUP_CODE=@p0
				   And A.CUST_CODE=@p1
				   And A.DC_CODE=@p2
				   And A.ITEM_CODE=@p3
				   And C.WAREHOUSE_TYPE=@p4
				   And D.ATYPE_CODE<>'C'
				   And B.AREA_CODE <> '-1'
			";

			var result = SqlQuery<int>(sql, param.ToArray()).Single();
			return result;
		}

		public IQueryable<F1913Data> GetF1913Datas(string dcCode, string gupCode, string custCode, string warehouseId,
				string itemCode, string itemName)
		{
			var parameters = new List<SqlParameter>
												{
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", warehouseId),
								new SqlParameter("@p4", string.Format("%{0}%",itemCode)),
								new SqlParameter("@p5", string.Format("%{0}%",itemName)),
						};

			var sql = @"
						SELECT ROW_NUMBER ()OVER(ORDER BY A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.GUP_CODE,A.CUST_CODE ,A.VNR_CODE,A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO) ROWNUM,
							   B.WAREHOUSE_ID,
							   C.WAREHOUSE_NAME,
							   A.ITEM_CODE,
							   E.ITEM_NAME,
							   A.LOC_CODE,
							   E.ITEM_SIZE,
							   E.ITEM_SPEC,
							   E.ITEM_COLOR,
							   A.VALID_DATE,
							   A.ENTER_DATE,
							   A.VNR_CODE,
							   ISNULL (E.BUNDLE_SERIALNO, 0) BUNDLE_SERIALNO,
							   ISNULL (E.BUNDLE_SERIALLOC, 0) BUNDLE_SERIALLOC,
							   A.QTY,
							   NULL AS ADJ_QTY_IN,
							   NULL AS ADJ_QTY_OUT,
							   '' AS CAUSE,
							   '' CAUSENAME,
							   '' CAUSE_MEMO,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   '0' AS SERIALNO_SCANOK,
							   G.DC_NAME,
							   A.BOX_CTRL_NO,
							   A.PALLET_CTRL_NO,
							   A.MAKE_NO
						  FROM (  SELECT A.DC_CODE,
										 A.GUP_CODE,
										 A.CUST_CODE,
										 A.ITEM_CODE,
										 A.LOC_CODE,
										 A.VNR_CODE,
										 A.VALID_DATE,
										 A.ENTER_DATE,
										 SUM (A.QTY) QTY,
										 A.BOX_CTRL_NO,
										 A.PALLET_CTRL_NO,
										 A.MAKE_NO
									FROM F1913 A
								GROUP BY A.DC_CODE,
										 A.GUP_CODE,
										 A.CUST_CODE,
										 A.ITEM_CODE,
										 A.LOC_CODE,
										 A.VNR_CODE,
										 A.VALID_DATE,
										 A.ENTER_DATE,
										 A.BOX_CTRL_NO,
										 A.PALLET_CTRL_NO,
										 A.MAKE_NO) A
							   INNER JOIN F1912 B
								  ON B.DC_CODE = A.DC_CODE AND B.LOC_CODE = A.LOC_CODE
							   INNER JOIN F1980 C
								  ON C.DC_CODE = A.DC_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
							   LEFT JOIN F1903 E
								  ON     E.GUP_CODE = A.GUP_CODE
									 AND E.CUST_CODE = A.CUST_CODE
									 AND E.ITEM_CODE = A.ITEM_CODE
							   INNER JOIN F1901 G ON G.DC_CODE = A.DC_CODE
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND B.WAREHOUSE_ID = @p3
							   AND A.ITEM_CODE LIKE @p4
							   AND E.ITEM_NAME LIKE @p5
						";

			var result = SqlQuery<F1913Data>(sql, parameters.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<ItemMakeNoTotalStockQty> GetItemMakeNoAndSerialNoTotalStockQties(string dcCode,string gupCode,string custCode,string warehouseType,bool isForIn,List<string> itemCodes)
		{
			List<SqlParameter> param = new List<SqlParameter> {
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", warehouseType),
                                new SqlParameter("@p4", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                        };

			var sqlFilter = string.Empty;
			if (isForIn)
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'03'";

			var sqlItemFilter = string.Empty;
			sqlItemFilter += param.CombineSqlInParameters(" AND A.ITEM_CODE", itemCodes, SqlDbType.VarChar);

			var sql = $@" SELECT Z.ITEM_CODE,Z.MAKE_NO,CASE WHEN Z.SERIAL_NO = '0' THEN NULL ELSE Z.SERIAL_NO END SERIAL_NO,ISNULL(SUM(Z.QTY),0) QTY
                    FROM (
                    SELECT A.ITEM_CODE,A.MAKE_NO,A.SERIAL_NO,SUM(A.QTY) QTY
										FROM F1913 A,F1912 B,F1980 C
										WHERE A.DC_CODE = B.DC_CODE
										AND A.LOC_CODE = B.LOC_CODE
										AND B.DC_CODE = C.DC_CODE
										AND B.WAREHOUSE_ID = C.WAREHOUSE_ID
										AND B.AREA_CODE<>'-1'
										AND A.QTY >0
										And A.VALID_DATE>@p4
										AND B.NOW_STATUS_ID<>'04'
										AND A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE= @p2
										AND C.WAREHOUSE_TYPE= @p3
										{sqlFilter} 
                    {sqlItemFilter}
										GROUP BY A.ITEM_CODE,A.MAKE_NO,A.SERIAL_NO
                    UNION ALL
                    SELECT A.ITEM_CODE,A.MAKE_NO, CASE WHEN A.SERIAL_NO IS NULL OR A.SERIAL_NO='' THEN '0' ELSE A.SERIAL_NO END SERIAL_NO ,SUM(A.B_PICK_QTY) QTY
											FROM F1511 A
											WHERE EXISTS (
											SELECT 1
												FROM F05030101 C
												INNER  JOIN F050301 D
												ON D.DC_CODE = C.DC_CODE
												AND D.GUP_CODE = C.GUP_CODE
												AND D.CUST_CODE = C.CUST_CODE
												AND D.ORD_NO = C.ORD_NO
												INNER  JOIN F051202 B
												ON B.DC_CODE = C.DC_CODE
												AND B.GUP_CODE = C.GUP_CODE
												AND B.CUST_CODE = C.CUST_CODE
												AND B.WMS_ORD_NO = C.WMS_ORD_NO
												WHERE D.PROC_FLAG='9'
												AND D.TYPE_ID= @p3
												AND A.DC_CODE = B.DC_CODE
												AND A.GUP_CODE = B.GUP_CODE
												AND A.CUST_CODE = B.CUST_CODE
												AND A.ORDER_NO = B.PICK_ORD_NO
												AND A.ORDER_SEQ = B.PICK_ORD_SEQ
											)
											AND A.STATUS<>'9'
											AND A.B_PICK_QTY>0
											AND A.DC_CODE = @p0
											AND A.GUP_CODE = @p1
											AND A.CUST_CODE= @p2
											{sqlItemFilter}
											GROUP BY A.ITEM_CODE,A.MAKE_NO,CASE WHEN A.SERIAL_NO IS NULL OR A.SERIAL_NO='' THEN '0' ELSE A.SERIAL_NO END) Z
                      GROUP BY Z.ITEM_CODE,Z.MAKE_NO,Z.SERIAL_NO ";
			return SqlQuery<ItemMakeNoTotalStockQty>(sql, param.ToArray());
		}


		// 回傳有問題
		public int GetItemStockWithVirtual(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, bool isForIn, string makeNo = "")
		{
			List<SqlParameter> param = new List<SqlParameter> {
																new SqlParameter("@p0", gupCode),
																new SqlParameter("@p1", custCode),
																new SqlParameter("@p2", dcCode),
																new SqlParameter("@p3", itemCode),
																new SqlParameter("@p4", warehouseType),
                                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                        };

			var sqlFilter = string.Empty;
			if (isForIn)
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'03'";

			var makeNoF1913Cond = string.Empty;
			var makeNoF051202Cond = string.Empty;
			if (!string.IsNullOrEmpty(makeNo))
			{
				makeNoF1913Cond += $@"
					And A.MAKE_NO=@p{param.Count}";
				makeNoF051202Cond += $@"
					And B.MAKE_NO=@p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count, makeNo));
			}

			string sql = @"
				Select ISNULL(Sum(Z.SumQty), 0) Qty
				  From (Select Sum( A.QTY ) SumQty
				          From F1913 A, F1912 B, F1980 C
				         Where A.LOC_CODE=B.LOC_CODE And A.DC_CODE=B.DC_CODE
				           And B.WAREHOUSE_ID=C.WAREHOUSE_ID And B.DC_CODE=C.DC_CODE
				           And A.VALID_DATE>@p5
				           And B.NOW_STATUS_ID<>'04'
				           And A.GUP_CODE=@p0
				           And A.CUST_CODE=@p1
				           And A.DC_CODE=@p2
				           And A.ITEM_CODE=@p3
				           And C.WAREHOUSE_TYPE=@p4
						   And B.AREA_CODE <> '-1' AND A.QTY>0 " + sqlFilter + makeNoF1913Cond + @"
				        Union
				        SELECT SUM (A.B_PICK_QTY) SumQty
						  FROM F1511 A
							   JOIN F051202 B
								  ON     A.ORDER_NO = B.PICK_ORD_NO
									 AND A.ORDER_SEQ = B.PICK_ORD_SEQ
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.DC_CODE = B.DC_CODE
							   JOIN
							   (  SELECT C.WMS_ORD_NO,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.DC_CODE,
										 E.TYPE_ID
									FROM F050801 C
										 JOIN F05030101 D
											ON     C.WMS_ORD_NO = D.WMS_ORD_NO
											   AND C.GUP_CODE = D.GUP_CODE
											   AND C.CUST_CODE = D.CUST_CODE
											   AND C.DC_CODE = D.DC_CODE
										 JOIN F050301 E
											ON     D.ORD_NO = E.ORD_NO
											   AND D.GUP_CODE = E.GUP_CODE
											   AND D.CUST_CODE = E.CUST_CODE
											   AND D.DC_CODE = E.DC_CODE
								   WHERE E.PROC_FLAG = '9'
								GROUP BY C.WMS_ORD_NO,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.DC_CODE,
										 E.TYPE_ID) F
								  ON     B.WMS_ORD_NO = F.WMS_ORD_NO
									 AND B.GUP_CODE = F.GUP_CODE
									 AND B.CUST_CODE = F.CUST_CODE
									 AND B.DC_CODE = F.DC_CODE
						 WHERE     A.STATUS <> '9'
                 AND A.B_PICK_QTY > 0
							   AND A.GUP_CODE = @p0
							   AND A.CUST_CODE = @p1
							   AND A.DC_CODE = @p2
							   AND B.ITEM_CODE = @p3
							   AND F.TYPE_ID = @p4" + makeNoF051202Cond + @"
				       ) Z
			";

			var result = SqlQuery<int>(sql, param.ToArray()).AsQueryable().SingleOrDefault();
			return result;
		}

		public IQueryable<ItemWarehouseStock> GetItemWarehouseStock(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, bool isForIn)
		{
			List<SqlParameter> param = new List<SqlParameter> {
										new SqlParameter("@p0",gupCode),
										new SqlParameter("@p1",custCode),
										new SqlParameter("@p2",dcCode),
										new SqlParameter("@p3",itemCode),
										new SqlParameter("@p4",warehouseType),
                    new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sqlFilter = string.Empty;
			if (isForIn)
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'03'";

			string sql = @"
				Select WAREHOUSE_ID, SUM_QTY, ROW_NUMBER()OVER(ORDER BY J.WAREHOUSE_ID ASC) ROWNUM
				  From (Select B.WAREHOUSE_ID, Sum( A.QTY ) SUM_QTY
				          From F1913 A, F1912 B, F1980 C
				         Where A.LOC_CODE=B.LOC_CODE And A.DC_CODE=B.DC_CODE
				           And B.WAREHOUSE_ID=C.WAREHOUSE_ID And B.DC_CODE=C.DC_CODE
				           And A.VALID_DATE > @p5
				           And B.NOW_STATUS_ID<>'04'
				           And A.GUP_CODE=@p0
				           And A.CUST_CODE=@p1
				           And A.DC_CODE=@p2
				           And A.ITEM_CODE=@p3
				           And C.WAREHOUSE_TYPE=@p4 " + sqlFilter + @"
				         Group By B.WAREHOUSE_ID) J
				 Order By SUM_QTY Desc
			";

			var result = SqlQuery<ItemWarehouseStock>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<ItemLocStock> GetItemLocStock(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string aTypeCode = null, string warehouseId = null, bool isForIn = true, bool isAllowExpiredItem = false)
		{
			if (!string.IsNullOrEmpty(aTypeCode))
				return GetItemLocStockByAType(dcCode, gupCode, custCode, itemCode, warehouseType, warehouseId, aTypeCode, isForIn, isAllowExpiredItem);
			else
				return GetItemLocStockNoAType(dcCode, gupCode, custCode, itemCode, warehouseType, warehouseId, isForIn, isAllowExpiredItem);
		}

		private IQueryable<ItemLocStock> GetItemLocStockByAType(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string warehouseId, string aTypeCode, bool isForIn, bool isAllowExpiredItem)
		{

			List<SqlParameter> param = new List<SqlParameter>
						{
								new SqlParameter("@p0",gupCode),
								new SqlParameter("@p1",custCode),
								new SqlParameter("@p2",dcCode),
								new SqlParameter("@p3",itemCode),
								new SqlParameter("@p4",aTypeCode),
                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(warehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", param.Count);
				param.Add(new SqlParameter("@p" + param.Count, warehouseId));
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", param.Count);
				param.Add(new SqlParameter("@p" + param.Count, warehouseType));
			}

			if (isForIn)
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'03'";

			var sqlFilterValidDate = string.Empty;
			if (!isAllowExpiredItem)
				sqlFilterValidDate = " AND A.VALID_DATE>@p5 ";

			string sql = @"
				
				Select A.LOC_CODE, A.ITEM_CODE, A.VALID_DATE, A.ENTER_DATE, A.VNR_CODE, A.SERIAL_NO, A.MAKE_NO, A.QTY, B.WAREHOUSE_ID,
							 CASE WHEN H.BOX_SERIAL IS NULL THEN F.BOX_SERIAL ELSE '' END AS BOX_SERIAL,
               CASE WHEN G.CASE_NO IS NULL THEN F.CASE_NO ELSE '' END AS CASE_NO,
               CASE WHEN I.BATCH_NO IS NULL THEN F.BATCH_NO ELSE '' END AS BATCH_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO ,ROW_NUMBER()OVER(ORDER BY A.VALID_DATE ASC) ROWNUM
				  From F1913 A
         INNER JOIN F1912 B
            ON B.DC_CODE = A.DC_CODE
           AND B.LOC_CODE = A.LOC_CODE
         INNER JOIN F1980 C
            ON C.DC_CODE = B.DC_CODE
           AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
         INNER JOIN F1919 D
            ON D.WAREHOUSE_ID = B.WAREHOUSE_ID
           And D.DC_CODE=B.DC_CODE
           And D.AREA_CODE=B.AREA_CODE
          LEFT JOIN F2501 F
					  ON  F.GUP_CODE = A.GUP_CODE And F.CUST_CODE = A.CUST_CODE And F.SERIAL_NO = A.SERIAL_NO
					LEFT JOIN
						(Select A.GUP_CODE,A.CUST_CODE,A.CASE_NO, Count(B.SERIAL_NO) Cnt From F2501 A
																	Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
																		Where B.QTY = 0 AND A.CASE_NO IS NOT NULL
																		Group by A.GUP_CODE,A.CUST_CODE,A.CASE_NO) G
						ON G.GUP_CODE = F.GUP_CODE
					 AND G.CUST_CODE = F.CUST_CODE
					 AND G.CASE_NO = F.CASE_NO
					LEFT JOIN
						 (Select A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL, Count(B.SERIAL_NO) Cnt
							 From F2501 A
								 Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
																	 Where B.QTY = 0 AND A.BOX_SERIAL IS NOT NULL
																		Group by A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL) H
						ON  H.GUP_CODE = F.GUP_CODE
					 AND H.CUST_CODE = F.CUST_CODE
					 AND H.BOX_SERIAL = F.BOX_SERIAL
					LEFT JOIN
						 (Select A.GUP_CODE,A.CUST_CODE,A.BATCH_NO, Count(B.SERIAL_NO) Cnt From F2501 A
									Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
							 Where B.QTY = 0 AND A.BATCH_NO IS NOT NULL
							Group by A.GUP_CODE,A.CUST_CODE,A.BATCH_NO) I
			  		ON  I.GUP_CODE = F.GUP_CODE
					 AND I.CUST_CODE = F.CUST_CODE
					 AND I.BATCH_NO = F.BATCH_NO
				 Where 1=1 " + sqlFilterValidDate +
@"	 And B.NOW_STATUS_ID<>'04'
				   And A.GUP_CODE= @p0
				   And A.CUST_CODE=@p1
				   And A.DC_CODE=@p2
				   And A.ITEM_CODE=@p3 " + sqlFilter + @"
				 Order By A.VALID_DATE
			";

			var result = SqlQuery<ItemLocStock>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		private IQueryable<ItemLocStock> GetItemLocStockNoAType(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string warehouseId, bool isForIn, bool isAllowExpiredItem)
		{
			List<SqlParameter> param = new List<SqlParameter>
						{
								new SqlParameter("@p0",gupCode),
								new SqlParameter("@p1",custCode),
								new SqlParameter("@p2",dcCode),
								new SqlParameter("@p3",itemCode),
                new SqlParameter("@p4", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sqlFilter = string.Empty;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(warehouseId))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_ID=@p{0} ", param.Count);
				param.Add(new SqlParameter("@p" + param.Count, warehouseId));
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sqlFilter += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", param.Count);
				param.Add(new SqlParameter("@p" + param.Count, warehouseType));
			}

			if (isForIn)
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sqlFilter += @"
				   And B.NOW_STATUS_ID<>'03'";

			var sqlFilterValidDate = string.Empty;
			if (!isAllowExpiredItem)
				sqlFilterValidDate = " AND A.VALID_DATE>@p4 ";



			string sql = @"
				Select A.LOC_CODE, A.ITEM_CODE, A.VALID_DATE, A.ENTER_DATE, A.VNR_CODE, A.SERIAL_NO, A.MAKE_NO, A.QTY, B.WAREHOUSE_ID,
							 CASE WHEN H.BOX_SERIAL IS NULL THEN F.BOX_SERIAL ELSE '' END AS BOX_SERIAL,
               CASE WHEN G.CASE_NO IS NULL THEN F.CASE_NO ELSE '' END AS CASE_NO,
               CASE WHEN I.BATCH_NO IS NULL THEN F.BATCH_NO ELSE '' END AS BATCH_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,ROW_NUMBER()OVER(ORDER BY A.VALID_DATE ASC) ROWNUM
				  From F1913 A
         INNER JOIN F1912 B
            ON B.DC_CODE = A.DC_CODE
           AND B.LOC_CODE = A.LOC_CODE
         INNER JOIN F1980 C
            ON C.DC_CODE = B.DC_CODE
           AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
          LEFT JOIN F2501 F
					  ON  F.GUP_CODE = A.GUP_CODE And F.CUST_CODE = A.CUST_CODE And F.SERIAL_NO = A.SERIAL_NO
					LEFT JOIN
						(Select A.GUP_CODE,A.CUST_CODE,A.CASE_NO, Count(B.SERIAL_NO) Cnt From F2501 A
																	Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
																		Where B.QTY = 0 AND A.CASE_NO IS NOT NULL
																		Group by A.GUP_CODE,A.CUST_CODE,A.CASE_NO) G
						ON G.GUP_CODE = F.GUP_CODE
					 AND G.CUST_CODE = F.CUST_CODE
					 AND G.CASE_NO = F.CASE_NO
					LEFT JOIN
						 (Select A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL, Count(B.SERIAL_NO) Cnt
							 From F2501 A
								 Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
																	 Where B.QTY = 0 AND A.BOX_SERIAL IS NOT NULL
																		Group by A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL) H
						ON  H.GUP_CODE = F.GUP_CODE
					 AND H.CUST_CODE = F.CUST_CODE
					 AND H.BOX_SERIAL = F.BOX_SERIAL
					LEFT JOIN
						 (Select A.GUP_CODE,A.CUST_CODE,A.BATCH_NO, Count(B.SERIAL_NO) Cnt From F2501 A
									Join F1913 B On A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO=B.SERIAL_NO
							 Where B.QTY = 0 AND A.BATCH_NO IS NOT NULL
							Group by A.GUP_CODE,A.CUST_CODE,A.BATCH_NO) I
			  		ON  I.GUP_CODE = F.GUP_CODE
					 AND I.CUST_CODE = F.CUST_CODE
					 AND I.BATCH_NO = F.BATCH_NO
				 Where 1=1 
				  " + sqlFilterValidDate +
@" And B.NOW_STATUS_ID<>'04'
				   And A.GUP_CODE=@p0
				   And A.CUST_CODE=@p1
				   And A.DC_CODE=@p2
				   And A.ITEM_CODE=@p3" + sqlFilter + @"
				 Order By A.VALID_DATE
			";

			var result = SqlQuery<ItemLocStock>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		public void DeleteDataByKey(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			List<SqlParameter> param = new List<SqlParameter>()
						{
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
								new SqlParameter("@p3",itemCode),
								new SqlParameter("@p4",locCode),
								new SqlParameter("@p5",validDate.ToString("yyyy/MM/dd")),
								new SqlParameter("@p6",enterDate.ToString("yyyy/MM/dd")),
								new SqlParameter("@p7",vnrCode),
								new SqlParameter("@p8",string.IsNullOrEmpty(serialNo)?"0":serialNo),
								new SqlParameter("@p9",boxCtrlNo),
								new SqlParameter("@p10",palletCtrlNo),
								new SqlParameter("@p11",makeNo)
						};

			var sql = @"DELETE  FROM F1913
                            WHERE DC_CODE = @p0 
                            AND GUP_CODE = @p1 
                            AND CUST_CODE = @p2 
                            AND ITEM_CODE = @p3 
                            AND LOC_CODE =@p4
                            AND VALID_DATE = TRY_CONVERT(datetime,@p5)
                            AND ENTER_DATE = TRY_CONVERT(datetime,@p6)
                            AND VNR_CODE = @p7  
                            AND SERIAL_NO =@p8 
                            AND BOX_CTRL_NO =@p9 
                            AND PALLET_CTRL_NO =@p10
                            AND MAKE_NO =@p11";
			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void UpdateQty(string dcCode, string gupCode, string custCode, string itemCode, string locCode,
								DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, long qty, string boxCtrlNo, string palletCtrlNo,
								string makeNo)
		{
			List<SqlParameter> param = new List<SqlParameter>
						{
								new SqlParameter("@p0",qty.ToString()),
								new SqlParameter("@p1",Current.Staff),
								new SqlParameter("@p2",Current.StaffName),
								new SqlParameter("@p3", dcCode),
								new SqlParameter("@p4", gupCode),
								new SqlParameter("@p5", custCode),
								new SqlParameter("@p6", itemCode),
								new SqlParameter("@p7", locCode),
								new SqlParameter("@p8", validDate.ToString("yyyy/MM/dd")),
								new SqlParameter("@p9",enterDate.ToString("yyyy/MM/dd")),
								new SqlParameter("@p10",vnrCode),
								new SqlParameter("@p11",string.IsNullOrEmpty(serialNo)?"0":serialNo),
								new SqlParameter("@p12",boxCtrlNo),
								new SqlParameter("@p13",palletCtrlNo),
								new SqlParameter("@p14",makeNo),
                new SqlParameter("@p15", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };


			var sql = @" UPDATE F1913 
                            SET QTY = @p0,UPD_STAFF = @p1 ,UPD_NAME=@p2 , UPD_DATE = @p15  
                            WHERE DC_CODE =@p3  
                            AND GUP_CODE =@p4 
                            AND CUST_CODE = @p5  
                            AND ITEM_CODE = @p6 
                            AND LOC_CODE = @p7 
                            AND VALID_DATE = TRY_CONVERT(datetime,@p8) 
                            AND ENTER_DATE = TRY_CONVERT(datetime,@p9) 
                            AND VNR_CODE = @p10
                            AND SERIAL_NO =@p11 
                            AND BOX_CTRL_NO =@p12 
                            AND PALLET_CTRL_NO =@p13 
                            AND MAKE_NO =@p14";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void MinusQty(string dcCode, string gupCode, string custCode, string itemCode, string locCode,
				DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, int minusQty, string userId, string userName,
				string boxCtrlNo, string palletCtrlNo, string makeNo)
		{

			List<SqlParameter> parameters = new List<SqlParameter>
						{
										new SqlParameter("@p0",minusQty),
										new SqlParameter("@p1",userId),
										new SqlParameter("@p2",userName),
										new SqlParameter("@p3", dcCode),
										new SqlParameter("@p4", gupCode),
										new SqlParameter("@p5", custCode),
										new SqlParameter("@p6", itemCode),
										new SqlParameter("@p7", locCode),
										new SqlParameter("@p8", validDate),
										new SqlParameter("@p9",enterDate),
										new SqlParameter("@p10",vnrCode),
										new SqlParameter("@p11",serialNo),
										new SqlParameter("@p12",boxCtrlNo),
										new SqlParameter("@p13",palletCtrlNo),
										new SqlParameter("@p14",makeNo),
                    new SqlParameter("@p15", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sql = " UPDATE F1913 SET QTY = QTY - @p0,UPD_STAFF = @p1 ,UPD_NAME=@p2 , UPD_DATE = @p15 " +
													"  WHERE DC_CODE =@p3 " +
													"    AND GUP_CODE =@p4 " +
													"    AND CUST_CODE = @p5 " +
													"    AND ITEM_CODE = @p6 " +
													"    AND LOC_CODE = @p7 " +
													"    AND VALID_DATE = TRY_CONVERT(datetime,@p8)  " +
													"    AND ENTER_DATE = TRY_CONVERT(datetime,@p9) " +
													"    AND VNR_CODE = @p10 " +
													"    AND SERIAL_NO = @p11 " +
													"    AND BOX_CTRL_NO =@p12 " +
													"    AND PALLET_CTRL_NO =@p13 " +
													"    AND MAKE_NO =@p14 ";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		/// <summary>
		/// 取得揀位商品儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public IQueryable<ItemLocPriorityInfo> GetItemPickLocPriorityInfo(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			return GetItemLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, true, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
		}

		/// <summary>
		/// 取得補位商品儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public IQueryable<ItemLocPriorityInfo> GetItemResupplyLocPriorityInfo(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
      //return GetItemLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, false, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
      return GetItemLocPriorityInfo_Resupply(dcCode, gupCode, custCode, itemCodes, false, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
    }


    private IQueryable<ItemLocPriorityInfo> GetItemLocPriorityInfo_Resupply(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isPickLoc, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
    {
      var parameters = new List<SqlParameter>
      {
          new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
          new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
          new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
          new SqlParameter("@p3",SqlDbType.VarChar){ Value = custCode}
      };

      var inSql = string.Empty;
      if (itemCodes.Count == 1)
      {
        inSql = string.Format(@" And A.ITEM_CODE=@p{0} ", parameters.Count);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = itemCodes[0] });
      }
   
      string sql = @"
			SELECT A.ITEM_CODE, A.LOC_CODE, A.VALID_DATE, A.ENTER_DATE, A.VNR_CODE,
						 A.SERIAL_NO, A.QTY, A.GUP_CODE, A.CUST_CODE, A.DC_CODE,
						 B.WAREHOUSE_ID, B.HOR_DISTANCE, B.FLOOR, B.USEFUL_VOLUMN, B.USED_VOLUMN,
						 D.ATYPE_CODE, E.HANDY, C.WAREHOUSE_TYPE, C.TMPR_TYPE,  A.BOX_CTRL_NO,
             A.PALLET_CTRL_NO, ROW_NUMBER()OVER(ORDER BY A.LOC_CODE ASC) ROWNUM, D.AREA_CODE, Case When A.MAKE_NO='' Then null Else A.MAKE_NO End MAKE_NO, C.PICK_FLOOR,
             C.DEVICE_TYPE, null, null, C.WAREHOUSE_NAME 
			 From F1913 A
			 INNER JOIN F1912 B     on B.DC_CODE = A.DC_CODE and B.LOC_CODE = A.LOC_CODE
			 INNER JOIN F1980 C     on C.DC_CODE = B.DC_CODE and C.WAREHOUSE_ID = B.WAREHOUSE_ID
			 INNER JOIN F1919 D     on D.DC_CODE = B.DC_CODE and D.WAREHOUSE_ID = B.WAREHOUSE_ID and D.AREA_CODE = B.AREA_CODE
			 INNER JOIN F1942 E     on B.LOC_TYPE_ID=E.LOC_TYPE_ID       
       INNER JOIN F198001 J   on J.TYPE_ID = C.WAREHOUSE_TYPE                       
    
       Where        
              A.DC_CODE=@p0
				 And  A.GUP_CODE=@p1
				 And  A.CUST_CODE=@p2
			   And  B.NOW_STATUS_ID<>'04' and (J.LOC_MUSTSAME_NOWCUSTCODE = '0' or B.NOW_CUST_CODE = '0' or  B.NOW_CUST_CODE = @p3)
         " + inSql;

      // 若有填倉別 Id，就只針對該倉別 Id 做篩選
      if (!string.IsNullOrEmpty(targetWarehouseId))
      {
        sql += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
        //parameters.Add(targetWarehouseId);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = targetWarehouseId });
      }
      else if (!string.IsNullOrEmpty(warehouseType))
      {
        sql += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
        //parameters.Add(warehouseType);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = warehouseType });
      }

      // 若不是報廢倉的話，則加上效期條件
      if (!(warehouseType == "D" || (targetWarehouseId != null && targetWarehouseId.StartsWith("D"))))
      {
        sql += string.Format(" AND A.VALID_DATE > @p{0} ", parameters.Count);
        //parameters.Add(DateTime.Now);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.DateTime2) { Value = DateTime.Now });
      }

      if (isPickLoc)
        sql += @"
				   And (D.ATYPE_CODE='A' Or D.ATYPE_CODE='B')";
      else
        sql += @"
				   And D.ATYPE_CODE='C'";

      if (isForIn)
        sql += @"
				   And B.NOW_STATUS_ID<>'02'";
      else
        sql += @"
				   And B.NOW_STATUS_ID<>'03'";

      if (!string.IsNullOrEmpty(wareHouseTmpr))
      {
        sql += parameters.CombineSqlInParameters(" AND   C.TMPR_TYPE ", wareHouseTmpr.Split(','), SqlDbType.VarChar);
      }

      var result = SqlQuery<ItemLocPriorityInfo>(sql, parameters.ToArray()).AsQueryable();
      return result;
    }


    private IQueryable<ItemLocPriorityInfo> GetItemLocPriorityInfo(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isPickLoc, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			var parameters = new List<SqlParameter>
      {
          new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
          new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
          new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},         
          new SqlParameter("@p3",SqlDbType.VarChar){ Value = custCode}
      };

			var inSql = string.Empty;
			if (itemCodes.Count == 1)
			{
				inSql = string.Format(@" And A.ITEM_CODE=@p{0} ", parameters.Count);
        //parameters.Add(itemCodes[0]);      
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = itemCodes[0] });
			}
			//else
			//{
			//    int paramStartIndex = parameters.Count;
			//    inSql = " And " + parameters.CombineSqlInParameters("A.ITEM_CODE", itemCodes, ref paramStartIndex);
			//}
			//已排除凍結儲位
			string sql = @"
			SELECT A.ITEM_CODE, A.LOC_CODE, A.VALID_DATE, A.ENTER_DATE, A.VNR_CODE,
						 A.SERIAL_NO, A.QTY, A.GUP_CODE, A.CUST_CODE, A.DC_CODE,
						 B.WAREHOUSE_ID, B.HOR_DISTANCE, B.FLOOR, B.USEFUL_VOLUMN, B.USED_VOLUMN,
						 D.ATYPE_CODE, E.HANDY, C.WAREHOUSE_TYPE, C.TMPR_TYPE,  A.BOX_CTRL_NO,
             A.PALLET_CTRL_NO, ROW_NUMBER()OVER(ORDER BY A.LOC_CODE ASC) ROWNUM, D.AREA_CODE, Case When A.MAKE_NO='' Then null Else A.MAKE_NO End MAKE_NO, C.PICK_FLOOR,
             C.DEVICE_TYPE, L.PK_AREA, M.PK_NAME, C.WAREHOUSE_NAME 
			 From F1913 A
			 INNER JOIN F1912 B     on B.DC_CODE = A.DC_CODE and B.LOC_CODE = A.LOC_CODE
			 INNER JOIN F1980 C     on C.DC_CODE = B.DC_CODE and C.WAREHOUSE_ID = B.WAREHOUSE_ID
			 INNER JOIN F1919 D     on D.DC_CODE = B.DC_CODE and D.WAREHOUSE_ID = B.WAREHOUSE_ID and D.AREA_CODE = B.AREA_CODE
			 INNER JOIN F1942 E     on B.LOC_TYPE_ID=E.LOC_TYPE_ID       
       INNER JOIN F198001 J   on J.TYPE_ID = C.WAREHOUSE_TYPE                       
       LEFT JOIN F19120602 L  on L.DC_CODE = A.DC_CODE  and L.CHK_LOC_CODE = SUBSTRING(A.LOC_CODE,1,5)
       LEFT JOIN F191206 M    on M.DC_CODE = L.DC_CODE  and M.PK_AREA = L.PK_AREA
       Where        
              A.DC_CODE=@p0
				 And  A.GUP_CODE=@p1
				 And  A.CUST_CODE=@p2
			   And  B.NOW_STATUS_ID<>'04' and (J.LOC_MUSTSAME_NOWCUSTCODE = '0' or B.NOW_CUST_CODE = '0' or  B.NOW_CUST_CODE = @p3)
         " + inSql;

			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				sql += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				//parameters.Add(targetWarehouseId);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = targetWarehouseId });
      }
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				sql += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				//parameters.Add(warehouseType);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.VarChar) { Value = warehouseType });
      }

			// 若不是報廢倉的話，則加上效期條件
			if (!(warehouseType == "D" || (targetWarehouseId != null && targetWarehouseId.StartsWith("D"))))
			{
        sql += string.Format(" AND A.VALID_DATE > @p{0} ", parameters.Count);
        //parameters.Add(DateTime.Now);
        parameters.Add(new SqlParameter("@p" + parameters.Count, SqlDbType.DateTime2) { Value = DateTime.Now });
      }

			if (isPickLoc)
				sql += @"
				   And (D.ATYPE_CODE='A' Or D.ATYPE_CODE='B')";
			else
				sql += @"
				   And D.ATYPE_CODE='C'";

			if (isForIn)
				sql += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				sql += @"
				   And B.NOW_STATUS_ID<>'03'";

			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				sql += parameters.CombineSqlInParameters(" AND   C.TMPR_TYPE ", wareHouseTmpr.Split(','), SqlDbType.VarChar);    
      }

			var result = SqlQuery<ItemLocPriorityInfo>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}



		/// <summary>
		/// 取得在調撥明細(F151002)有此商品、效期、進貨日資料的商品儲位(F1913)
		/// 效期的比較使用 F151002 的 VALID_DATE
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <returns></returns>
		public IQueryable<F1913> GetDatasByAllocationTarget(string dcCode, string gupCode, string custCode, string allocationNo)
		{
			var param = new List<object> {
																gupCode,
																custCode,
																dcCode,
																allocationNo
												};
			string sql = @"
				Select A.*
				  From F1913 A, F151002 B
				 Where A.LOC_CODE=B.TAR_LOC_CODE And A.DC_CODE=B.DC_CODE And A.GUP_CODE=B.GUP_CODE
				   And A.CUST_CODE=B.CUST_CODE And A.ITEM_CODE=B.ITEM_CODE And A.VALID_DATE=B.VALID_DATE
				   And A.ENTER_DATE=B.ENTER_DATE And A.VNR_CODE=B.VNR_CODE And A.SERIAL_NO=ISNULL(B.SERIAL_NO,'0')
				   And A.GUP_CODE=@p0
				   And A.CUST_CODE=@p1
				   And A.DC_CODE=@p2
				   And B.ALLOCATION_NO=@p3
			";
			var result = SqlQuery<F1913>(sql, param.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<StockQueryData1> GetStockQueryData1(string gupCode, string custCode, string dcCode,
								string typeBegin, string typeEnd,
								string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
								DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
								string locCodeBegin, string locCodeEnd, string[] itemCodes, string[] wareHouseIds,
								string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
								string expend, string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd, string[] makeNo,string vnrCode)
		{

			// 展開效期與入庫日的SQL
			var sqlExpend = @"
        SELECT ROW_NUMBER()OVER(ORDER BY A.LOC_CODE ASC) ROWNUM, A.*
          FROM (SELECT F1913.DC_CODE,
        			   F1913.GUP_CODE,
        			   F1913.CUST_CODE,
        			   F1909.CUST_NAME,
        			   F1912.WAREHOUSE_ID,
        			   F1980.WAREHOUSE_NAME,
        			   F1913.LOC_CODE,
        			   F1903.ITEM_CODE,
        			   F1903.ITEM_NAME,
        			   F1903.LTYPE,
        			   CONCAT( F1903.LTYPE , ' ' , F1915.CLA_NAME) AS LTYPE_NAME,
        			   F1903.MTYPE,
        			    CONCAT( F1903.MTYPE , ' ' , F1916.CLA_NAME) AS MTYPE_NAME,
        			   F1903.STYPE,
        			   CONCAT( F1903.STYPE , ' ' , F1917.CLA_NAME) AS STYPE_NAME,
        			   F1903.ITEM_SIZE,
        			   F1903.ITEM_SPEC,
        			   F1903.ITEM_COLOR,
        			   F1913.QTY,
        			   F1912.NOW_STATUS_ID,
        			   F1943.LOC_STATUS_NAME NOW_STATUS_NAME,
        			  DATEDIFF(day,dbo.GetSysDate(),F1913.VALID_DATE) AS LeftDay,
        			   F1913.VALID_DATE,
        			   F1913.ENTER_DATE,
        			   F1912.USEFUL_VOLUMN,
        			   F1912.USED_VOLUMN,
                        CASE WHEN F1913.MAKE_NO ='0' THEN NULL ELSE F1913.MAKE_NO END MAKE_NO,
        			   (CASE F1913.SERIAL_NO
        				   WHEN '0' THEN NULL
        				   ELSE F1913.SERIAL_NO
        				END)
        				AS SERIAL_NO,
                       CASE WHEN F1913.BOX_CTRL_NO ='0' THEN NULL ELSE F1913.BOX_CTRL_NO END BOX_CTRL_NO,
        			  CASE WHEN F1913.PALLET_CTRL_NO ='0' THEN NULL ELSE F1913.PALLET_CTRL_NO END PALLET_CTRL_NO,    
                       F1903.VNR_CODE,
                       F91000302.ACC_UNIT_NAME ITEM_UNIT,
                       F1905.PACK_LENGTH * F1905.PACK_WIDTH * F1905.PACK_HIGHT
                         AS TOTAL_VOLUME,
                       case when(SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE  AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_LEVEL = 1 ) is null then
        						  (SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE  AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_ID = '03' )
        					 else (SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE  AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_LEVEL = 1 ) END
                         as UNIT
        		  FROM F1913
        			   LEFT OUTER JOIN F1912 
        				  ON     F1913.DC_CODE = F1912.DC_CODE
        					 AND F1913.LOC_CODE = F1912.LOC_CODE
        			   LEFT OUTER JOIN F1980
        				  ON     F1912.DC_CODE = F1980.DC_CODE
        					 AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID
        			   LEFT OUTER JOIN F198001
        				  ON F1980.WAREHOUSE_TYPE = F198001.TYPE_ID        			  
        			   LEFT OUTER JOIN F1903
        				  ON     F1913.GUP_CODE = F1903.GUP_CODE
        					 AND F1913.CUST_CODE = F1903.CUST_CODE
        					 AND F1913.ITEM_CODE = F1903.ITEM_CODE
        			   LEFT OUTER JOIN F1915
        				  ON     F1903.GUP_CODE = F1915.GUP_CODE
        					 AND F1903.LTYPE = F1915.ACODE
                          AND (F1915.CUST_CODE = '0' OR F1915.CUST_CODE = F1913.CUST_CODE)
        			   LEFT OUTER JOIN F1916
        				  ON     F1903.GUP_CODE = F1916.GUP_CODE
        					 AND F1903.LTYPE = F1916.ACODE
        					 AND F1903.MTYPE = F1916.BCODE
                          AND (F1916.CUST_CODE = '0' OR F1916.CUST_CODE = F1913.CUST_CODE)
        			   LEFT OUTER JOIN F1917
        				  ON     F1903.GUP_CODE = F1917.GUP_CODE
        					 AND F1903.LTYPE = F1917.ACODE
        					 AND F1903.MTYPE = F1917.BCODE
        					 AND F1903.STYPE = F1917.CCODE
        					AND (F1917.CUST_CODE = '0' OR F1917.CUST_CODE = F1913.CUST_CODE)
        			   LEFT OUTER JOIN F1943
        				  ON F1912.NOW_STATUS_ID = F1943.LOC_STATUS_ID
        			   LEFT OUTER JOIN F1909
        				  ON     F1913.GUP_CODE = F1909.GUP_CODE
        					 AND F1913.CUST_CODE = F1909.CUST_CODE
                                            LEFT OUTER JOIN F1905
        				  ON     F1905.GUP_CODE = F1913.GUP_CODE
        					 AND F1905.ITEM_CODE = F1913.ITEM_CODE
                             AND F1905.CUST_CODE = F1913.CUST_CODE
        			   INNER JOIN F91000302
        					ON  F1903.ITEM_UNIT =F91000302.ACC_UNIT
                                                 AND F91000302.ITEM_TYPE_ID = '001'
        		 WHERE     F1913.GUP_CODE = @P0
        			   AND F1913.CUST_CODE = @P1
        			   AND F1913.DC_CODE = @P2
        			   {0}) A
         WHERE A.QTY > 0";
			// 沒有展開效期與入庫日的SQL
			var sqlGroup = @"
        SELECT ROW_NUMBER()OVER(ORDER BY A.LOC_CODE ASC) ROWNUM, A.*
          FROM (  SELECT F1913.DC_CODE,
        				 F1913.GUP_CODE,
        				 F1913.CUST_CODE,
        				 F1909.CUST_NAME,
        				 F1912.WAREHOUSE_ID,
        				 F1980.WAREHOUSE_NAME,
        				 F1913.LOC_CODE,
        				 F1903.ITEM_CODE,
        				 F1903.ITEM_NAME,
        				 F1903.LTYPE,
        				 CONCAT(F1903.LTYPE,' ',F1915.CLA_NAME)  AS LTYPE_NAME,
        				 F1903.MTYPE,
        				 CONCAT(F1903.MTYPE,' ',F1916.CLA_NAME) AS MTYPE_NAME,
        				 F1903.STYPE,
        				  CONCAT( F1903.STYPE,' ',F1917.CLA_NAME) AS STYPE_NAME,
        				 F1903.ITEM_SIZE,
        				 F1903.ITEM_SPEC,
        				 F1903.ITEM_COLOR,
        				 SUM (F1913.QTY) QTY,
        				 F1912.NOW_STATUS_ID,
        				 F1943.LOC_STATUS_NAME NOW_STATUS_NAME,
        				 F1912.USEFUL_VOLUMN,
        				 F1912.USED_VOLUMN,
        				 CASE WHEN F1913.BOX_CTRL_NO ='0' THEN NULL ELSE F1913.BOX_CTRL_NO END BOX_CTRL_NO,
        			   CASE WHEN F1913.PALLET_CTRL_NO ='0' THEN NULL ELSE F1913.PALLET_CTRL_NO END PALLET_CTRL_NO,    
        				 F91000302.ACC_UNIT_NAME ITEM_UNIT,
        				 F1905.PACK_LENGTH * F1905.PACK_WIDTH * F1905.PACK_HIGHT AS TOTAL_VOLUME,
        				 CASE WHEN F1913.MAKE_NO ='0' THEN NULL ELSE F1913.MAKE_NO END MAKE_NO,
        					case when(SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_LEVEL = 1 ) is null then
        						  (SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_ID = '03' )
        					 else (SELECT F190301.UNIT_QTY FROM F190301  WHERE F190301.GUP_CODE = F1913.GUP_CODE AND F190301.ITEM_CODE = F1903.ITEM_CODE AND F190301.CUST_CODE = F1913.CUST_CODE AND F190301.UNIT_LEVEL = 1 ) END
                                                as UNIT,
                 F1903.VNR_CODE
        			FROM F1913
        				 LEFT OUTER JOIN F1912
        					ON     F1913.DC_CODE = F1912.DC_CODE
        					   AND F1913.LOC_CODE = F1912.LOC_CODE
        				 LEFT OUTER JOIN F1980
        					ON     F1912.DC_CODE = F1980.DC_CODE
        					   AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID
        				 LEFT OUTER JOIN F198001
        					ON F1980.WAREHOUSE_TYPE = F198001.TYPE_ID
        				 LEFT OUTER JOIN F1903
        					ON     F1913.GUP_CODE = F1903.GUP_CODE
        					   AND F1913.ITEM_CODE = F1903.ITEM_CODE
                               AND F1913.CUST_CODE = F1903.CUST_CODE
        				 LEFT OUTER JOIN F1915
        					ON     F1903.GUP_CODE = F1915.GUP_CODE
        					   AND F1903.LTYPE = F1915.ACODE
                            AND (F1915.CUST_CODE = '0' OR F1915.CUST_CODE = F1913.CUST_CODE)
        				 LEFT OUTER JOIN F1916
        					ON     F1903.GUP_CODE = F1916.GUP_CODE
        					   AND F1903.LTYPE = F1916.ACODE
        					   AND F1903.MTYPE = F1916.BCODE
                            AND (F1916.CUST_CODE = '0' OR F1916.CUST_CODE = F1913.CUST_CODE)
        				 LEFT OUTER JOIN F1917
        					ON     F1903.GUP_CODE = F1917.GUP_CODE
        					   AND F1903.LTYPE = F1917.ACODE
        					   AND F1903.MTYPE = F1917.BCODE
        					   AND F1903.STYPE = F1917.CCODE
                            AND (F1917.CUST_CODE = '0' OR F1917.CUST_CODE = F1913.CUST_CODE) 
        				 LEFT OUTER JOIN F1943
        					ON F1912.NOW_STATUS_ID = F1943.LOC_STATUS_ID
        				 LEFT OUTER JOIN F1909
        					ON     F1913.GUP_CODE = F1909.GUP_CODE
        					   AND F1913.CUST_CODE = F1909.CUST_CODE
        				 LEFT OUTER JOIN F1905
        					ON     F1905.GUP_CODE = F1913.GUP_CODE
        					   AND F1905.ITEM_CODE = F1913.ITEM_CODE
                               AND F1905.CUST_CODE = F1913.CUST_CODE
        				 INNER JOIN F91000302
        					ON  F1903.ITEM_UNIT =F91000302.ACC_UNIT
                                                 AND F91000302.ITEM_TYPE_ID = '001'
        		   WHERE     F1913.GUP_CODE = @p0
        				 AND F1913.CUST_CODE = @p1
        				 AND F1913.DC_CODE = @p2
        				 {0}
        		GROUP BY F1913.DC_CODE,
        				 F1913.GUP_CODE,
        				 F1913.CUST_CODE,
        				 F1909.CUST_NAME,
        				 F1912.WAREHOUSE_ID,
        				 F1980.WAREHOUSE_NAME,
        				 F1913.LOC_CODE,
        				 F1903.ITEM_CODE,
                 F1903.VNR_CODE,
        				 F1903.ITEM_NAME,
        				 F1903.LTYPE,
        				 F1915.CLA_NAME,
        				 F1903.MTYPE,
        				 F1916.CLA_NAME,
        				 F1903.STYPE,
        				 F1917.CLA_NAME,
        				 F1903.ITEM_SIZE,
        				 F1903.ITEM_SPEC,
        				 F1903.ITEM_COLOR,
        				 F1912.NOW_STATUS_ID,
        				 F1943.LOC_STATUS_NAME,
        				 F1912.USEFUL_VOLUMN,
        				 F1912.USED_VOLUMN,
                 F1913.BOX_CTRL_NO,
                 F1913.PALLET_CTRL_NO,
                 F91000302.ACC_UNIT_NAME,
                 F1913.MAKE_NO,
                 F1905.PACK_LENGTH,
                 F1905.PACK_WIDTH,
                 F1905.PACK_HIGHT) A
                         WHERE A.QTY > 0";
			var paramList = new List<object>
												 {
																 gupCode,custCode, dcCode
												 };

			var condition = paramList.CombineSqlInParameters(" AND F1912.WAREHOUSE_ID", wareHouseIds);

			if (!string.IsNullOrWhiteSpace(typeBegin) && !string.IsNullOrWhiteSpace(typeEnd))
			{
				condition += paramList.Combine(" AND F1903.TYPE BETWEEN @p{0} AND @p{1} ", typeBegin, typeEnd);
			}

			if (!string.IsNullOrWhiteSpace(lTypeBegin) && !string.IsNullOrWhiteSpace(lTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.LTYPE BETWEEN @p{0} AND @p{1} ", lTypeBegin, lTypeEnd);
			}

			if (!string.IsNullOrWhiteSpace(mTypeBegin) && !string.IsNullOrWhiteSpace(mTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.MTYPE BETWEEN @p{0} AND @p{1} ", mTypeBegin, mTypeEnd);
			}

			if (!string.IsNullOrWhiteSpace(sTypeBegin) && !string.IsNullOrWhiteSpace(sTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.STYPE BETWEEN @p{0} AND @p{1} ", sTypeBegin, sTypeEnd);
			}

			if (enterDateBegin.HasValue && enterDateEnd.HasValue)
			{
				condition += paramList.Combine(" AND F1913.ENTER_DATE BETWEEN @p{0} AND @p{1} ", enterDateBegin, enterDateEnd);
			}

			if (validDateBegin.HasValue && validDateEnd.HasValue)
			{
				condition += paramList.Combine(" AND F1913.VALID_DATE BETWEEN @p{0} AND @p{1} ", validDateBegin, validDateEnd);
			}

			if (!string.IsNullOrWhiteSpace(locCodeBegin) && !string.IsNullOrWhiteSpace(locCodeEnd))
			{
				condition += paramList.Combine(" AND F1913.LOC_CODE BETWEEN @p{0} AND @p{1} ", locCodeBegin, locCodeEnd);
			}

			if (!string.IsNullOrWhiteSpace(boxCtrlNoBegin) && !string.IsNullOrWhiteSpace(boxCtrlNoEnd))
			{
				condition += paramList.Combine(" AND F1913.BOX_CTRL_NO BETWEEN @p{0} AND @p{1} ", boxCtrlNoBegin, boxCtrlNoEnd);
			}

			if (!string.IsNullOrWhiteSpace(palletCtrlNoBegin) && !string.IsNullOrWhiteSpace(palletCtrlNoEnd))
			{
				condition += paramList.Combine(" AND F1913.PALLET_CTRL_NO BETWEEN @p{0} AND @p{1} ", palletCtrlNoBegin, palletCtrlNoEnd);
			}

			//makeNo
			if (makeNo.Any())
			{
				condition += paramList.CombineSqlInParameters(" AND F1913.MAKE_NO ", makeNo);
			}

			if (itemCodes.Any())
			{
				condition += paramList.CombineSqlInParameters(" AND F1913.ITEM_CODE", itemCodes);
			}

      if (!string.IsNullOrWhiteSpace(vnrCode))
        condition += paramList.Combine(" AND F1903.VNR_CODE=@p{0}", vnrCode);

      if (boundleSerialNo == "1")
				condition += " AND F1903.BUNDLE_SERIALNO = '1' ";
			if (boundleSerialLoc == "1")
				condition += " AND F1903.BUNDLE_SERIALLOC = '1' ";
			if (multiFlag == "1")
				condition += " AND F1903.MULTI_FLAG = '1' ";
			if (packWareW == "1")
				condition += " AND F1903.PICK_WARE = 'W' ";
			if (virtualType == "1")
				condition += " AND F1903.VIRTUAL_TYPE IS NOT NULL ";

			string sql;
			if (expend == "1" || boundleSerialLoc == "1")
				sql = string.Format(sqlExpend, condition);
			else
				sql = string.Format(sqlGroup, condition);

			var result = SqlQuery<StockQueryData1>(sql, paramList.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<StockQueryData1> GetStockQueryData2(string gupCode, string custCode, string dcCode,
				string typeBegin, string typeEnd,
				string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
				DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
				string[] itemCodes, string[] wareHouseIds,
				string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
				string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd,string vnrCode)
		{
			var sql = @"
						SELECT ROW_NUMBER ()OVER(ORDER BY B.ITEM_CODE,B.DC_CODE,B.GUP_CODE,B.CUST_CODE) ROWNUM,
                  B.*,
                  (B.PICK_SUM_QTY + B.ALLOCATION_SUM_QTY)
                    RESERVATION_QTY
            FROM (SELECT A.*,
                          ISNULL (
                            (  SELECT SUM (VW_VirtualStock.B_PICK_QTY)
                                  FROM VW_VirtualStock
                                WHERE     VW_VirtualStock.ITEM_CODE = A.ITEM_CODE
                                      AND VW_VirtualStock.DC_CODE = A.DC_CODE
                                      AND VW_VirtualStock.GUP_CODE = A.GUP_CODE
                                      AND VW_VirtualStock.CUST_CODE = A.CUST_CODE
                                      AND VW_VirtualStock.STATUS IN ('0','1')
                                      AND VW_VirtualStock.ORDER_NO LIKE 'P%'
                              GROUP BY VW_VirtualStock.ITEM_CODE,
                                      VW_VirtualStock.DC_CODE,
                                      VW_VirtualStock.GUP_CODE,
                                      VW_VirtualStock.CUST_CODE),
                            0)
                            PICK_SUM_QTY,
                          ISNULL (
                            (  SELECT SUM (VW_VirtualStock.B_PICK_QTY)
                                  FROM VW_VirtualStock
                                WHERE     VW_VirtualStock.ITEM_CODE = A.ITEM_CODE
                                      AND VW_VirtualStock.DC_CODE = A.DC_CODE
                                      AND VW_VirtualStock.GUP_CODE = A.GUP_CODE
                                      AND VW_VirtualStock.CUST_CODE = A.CUST_CODE
                                      AND VW_VirtualStock.STATUS IN ('0','1')
                                      AND VW_VirtualStock.ORDER_NO LIKE 'T%'
                              GROUP BY VW_VirtualStock.ITEM_CODE,
                                      VW_VirtualStock.DC_CODE,
                                      VW_VirtualStock.GUP_CODE,
                                      VW_VirtualStock.CUST_CODE),
                            0)
                            ALLOCATION_SUM_QTY,
                          ISNULL (
                            (  SELECT SUM (F91020501.PICK_QTY)
                                  FROM F91020501
                                WHERE     F91020501.ITEM_CODE = A.ITEM_CODE
                                      AND F91020501.DC_CODE = A.DC_CODE
                                      AND F91020501.GUP_CODE = A.GUP_CODE
                                      AND F91020501.CUST_CODE = A.CUST_CODE
                              GROUP BY F91020501.ITEM_CODE,
                                      F91020501.DC_CODE,
                                      F91020501.GUP_CODE,
                                      F91020501.CUST_CODE),
                            0)
                            PROCESS_PICK_SUM_QTY,
                          ISNULL (
                            (  SELECT SUM (F151002.TAR_QTY - F151002.A_TAR_QTY)
                                  FROM F151001
                                  JOIN F151002
                                     ON F151002.DC_CODE = F151001.DC_CODE
                                    AND F151002.GUP_CODE = F151001.GUP_CODE
                                    AND F151002.CUST_CODE = F151001.CUST_CODE
                                    AND F151002.ALLOCATION_NO = F151001.ALLOCATION_NO
                                WHERE     F151002.ITEM_CODE = A.ITEM_CODE
                                      AND F151002.DC_CODE = A.DC_CODE
                                      AND F151002.GUP_CODE = A.GUP_CODE
                                      AND F151002.CUST_CODE = A.CUST_CODE
                                      AND F151001.ALLOCATION_TYPE = '1'
                                      AND F151002.STATUS = '1'
                              GROUP BY F151002.ITEM_CODE,
                                      F151002.DC_CODE,
                                      F151002.GUP_CODE,
                                      F151002.CUST_CODE),
                            0)
                            VIRTUAL_STOCK_QTY,
                          ISNULL (
                            (  SELECT SUM (F151002.TAR_QTY - F151002.A_TAR_QTY)
                                  FROM F151001
                                  JOIN F151002
                                     ON F151002.DC_CODE = F151001.DC_CODE
                                    AND F151002.GUP_CODE = F151001.GUP_CODE
                                    AND F151002.CUST_CODE = F151001.CUST_CODE
                                    AND F151002.ALLOCATION_NO = F151001.ALLOCATION_NO
                                WHERE     F151002.ITEM_CODE = A.ITEM_CODE
                                      AND F151002.DC_CODE = A.DC_CODE
                                      AND F151002.GUP_CODE = A.GUP_CODE
                                      AND F151002.CUST_CODE = A.CUST_CODE
                                      AND F151001.ALLOCATION_TYPE = '6'
                                      AND F151002.STATUS = '1'
                              GROUP BY F151002.ITEM_CODE,
                                      F151002.DC_CODE,
                                      F151002.GUP_CODE,
                                      F151002.CUST_CODE),
                            0)
                            A7_PRE_TAR_QTY
                    FROM (  SELECT F1913.DC_CODE,
                                    F1913.GUP_CODE,
                                    F1913.CUST_CODE,
                                    F1909.CUST_NAME,
                                    F1912.WAREHOUSE_ID,
                                    F1980.WAREHOUSE_NAME,
                                    F1980.WAREHOUSE_TYPE,
                                    F1903.ITEM_CODE,
                                    F1903.ITEM_NAME,
                                    F1903.LTYPE,
                                     CONCAT( F1903.LTYPE , ' ' , F1915.CLA_NAME) AS LTYPE_NAME,
                                    F1903.MTYPE,
                                    CONCAT( F1903.MTYPE , ' ' , F1916.CLA_NAME) AS MTYPE_NAME,
                                    F1903.STYPE,
                                    CONCAT( F1903.STYPE , ' ' , F1917.CLA_NAME) AS STYPE_NAME,
                                    F1903.ITEM_SIZE,
                                    F1903.ITEM_SPEC,
                                    F1903.ITEM_COLOR,
                                    SUM (F1913.QTY) QTY,
                                    F1912.NOW_STATUS_ID,
                                    F1943.LOC_STATUS_NAME NOW_STATUS_NAME,
                                    F1903.VNR_CODE
                              FROM F1913
                                    LEFT OUTER JOIN F1912
                                      ON     F1913.DC_CODE = F1912.DC_CODE
                                          AND F1913.LOC_CODE = F1912.LOC_CODE
                                    LEFT OUTER JOIN F1980
                                      ON     F1912.DC_CODE = F1980.DC_CODE
                                          AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID
                                    LEFT OUTER JOIN F198001
                                      ON F1980.WAREHOUSE_TYPE = F198001.TYPE_ID                                    
                                    LEFT OUTER JOIN F1903
                                      ON     F1913.GUP_CODE = F1903.GUP_CODE
                                          AND F1913.CUST_CODE = F1903.CUST_CODE
                                          AND F1913.ITEM_CODE = F1903.ITEM_CODE
                                    LEFT OUTER JOIN F1915
                                      ON     F1903.GUP_CODE = F1915.GUP_CODE
                                          AND F1903.LTYPE = F1915.ACODE
                                          AND (F1915.CUST_CODE = '0' OR F1915.CUST_CODE = F1913.CUST_CODE)
                                    LEFT OUTER JOIN F1916
                                      ON     F1903.GUP_CODE = F1916.GUP_CODE
                                          AND F1903.LTYPE = F1916.ACODE
                                          AND F1903.MTYPE = F1916.BCODE
																			    AND (F1916.CUST_CODE = '0' OR F1916.CUST_CODE = F1913.CUST_CODE)
                                    LEFT OUTER JOIN F1917
                                      ON     F1903.GUP_CODE = F1917.GUP_CODE
                                          AND F1903.LTYPE = F1917.ACODE
                                          AND F1903.MTYPE = F1917.BCODE
                                          AND F1903.STYPE = F1917.CCODE
                                          AND (F1917.CUST_CODE = '0' OR F1917.CUST_CODE = F1913.CUST_CODE)
                                    LEFT OUTER JOIN F1943
                                      ON F1912.NOW_STATUS_ID = F1943.LOC_STATUS_ID
                                    LEFT OUTER JOIN F1909
                                      ON     F1913.GUP_CODE = F1909.GUP_CODE
                                          AND F1913.CUST_CODE = F1909.CUST_CODE
                              WHERE     F1913.GUP_CODE = @p0
                                    AND F1913.CUST_CODE = @p1
                                    AND F1913.DC_CODE = @p2
                                    {0}
                          GROUP BY F1913.DC_CODE,
                                    F1913.GUP_CODE,
                                    F1913.CUST_CODE,
                                    F1909.CUST_NAME,
                                    F1912.WAREHOUSE_ID,
                                    F1980.WAREHOUSE_NAME,
                                    F1980.WAREHOUSE_TYPE,
                                    F1903.ITEM_CODE,
                                    F1903.ITEM_NAME,
                                    F1903.LTYPE,
                                    F1915.CLA_NAME,
                                    F1903.MTYPE,
                                    F1916.CLA_NAME,
                                    F1903.STYPE,
                                    F1917.CLA_NAME,
                                    F1903.ITEM_SIZE,
                                    F1903.ITEM_SPEC,
                                    F1903.ITEM_COLOR,
                                    F1912.NOW_STATUS_ID,
                                    F1943.LOC_STATUS_NAME,
                                    F1903.VNR_CODE) A
                    ) B
					WHERE (B.QTY > 0 OR B.PICK_SUM_QTY > 0 OR B.ALLOCATION_SUM_QTY > 0 OR B.PROCESS_PICK_SUM_QTY > 0 /*OR B.LEND_QTY > 0*/)";

			var paramList = new List<SqlParameter>
			{
				new SqlParameter("@p0", gupCode) {SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1", custCode) {SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2", dcCode) {SqlDbType = SqlDbType.VarChar},
			};

			var condition = paramList.CombineSqlInParameters(" AND F1912.WAREHOUSE_ID", wareHouseIds, SqlDbType.VarChar);

			if (!string.IsNullOrWhiteSpace(typeBegin) && !string.IsNullOrWhiteSpace(typeEnd))
			{
				condition += paramList.Combine(" AND F1903.TYPE BETWEEN @p{0} AND @p{1} ", typeBegin, typeEnd);
			}

			if (!string.IsNullOrWhiteSpace(lTypeBegin) && !string.IsNullOrWhiteSpace(lTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.LTYPE BETWEEN @p{0} AND @p{1} ", lTypeBegin, lTypeEnd);
			}

			if (!string.IsNullOrWhiteSpace(mTypeBegin) && !string.IsNullOrWhiteSpace(mTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.MTYPE BETWEEN @p{0} AND @p{1} ", mTypeBegin, mTypeEnd);
			}

			if (!string.IsNullOrWhiteSpace(sTypeBegin) && !string.IsNullOrWhiteSpace(sTypeEnd))
			{
				condition += paramList.Combine(" AND F1903.STYPE BETWEEN @p{0} AND @p{1} ", sTypeBegin, sTypeEnd);
			}

			if (enterDateBegin.HasValue && enterDateEnd.HasValue)
			{
				condition += paramList.Combine(" AND F1913.ENTER_DATE BETWEEN @p{0} AND @p{1} ", enterDateBegin, enterDateEnd);
			}

			if (validDateBegin.HasValue && validDateEnd.HasValue)
			{
				condition += paramList.Combine(" AND F1913.VALID_DATE BETWEEN @p{0} AND @p{1} ", validDateBegin, validDateEnd);
			}

			if (!string.IsNullOrWhiteSpace(palletCtrlNoBegin) && !string.IsNullOrWhiteSpace(palletCtrlNoEnd))
			{
				condition += paramList.Combine(" AND F1913.PALLET_CTRL_NO BETWEEN @p{0} AND @p{1} ", palletCtrlNoBegin, palletCtrlNoEnd);
			}

			if (itemCodes.Any())
			{
				condition += paramList.CombineSqlInParameters(" AND F1913.ITEM_CODE", itemCodes, SqlDbType.VarChar);
			}
      if (!string.IsNullOrWhiteSpace(vnrCode))
        condition += paramList.Combine(" AND F1903.VNR_CODE=@p{0}", vnrCode);

      if (boundleSerialNo == "1")
				condition += " AND F1903.BUNDLE_SERIALNO = '1' ";
			if (boundleSerialLoc == "1")
				condition += " AND F1903.BUNDLE_SERIALLOC = '1' ";
			if (multiFlag == "1")
				condition += " AND F1903.MULTI_FLAG = '1' ";
			if (packWareW == "1")
				condition += " AND F1903.PICK_WARE = 'W' ";
			if (virtualType == "1")
				condition += " AND F1903.VIRTUAL_TYPE IS NOT NULL ";


			var result = SqlQuery<StockQueryData1>(string.Format(sql, condition), paramList.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<StockQueryData3> GetStockQueryData3(string gupCode, string custCode, string dcCode,
				string typeBegin, string typeEnd,
				string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
				string enterDateBegin, string enterDateEnd, string validDateBegin, string validDateEnd,
				string closeDateBegin, string closeDateEnd, string itemCodes,
				string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
				string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd)
		{
			var sql = @"SELECT  ROW_NUMBER ()OVER(ORDER BY A.ITEM_CODE,A.CUST_CODE) ROWNUM,
       A.*,
       --商品出庫總和
       (SELECT ISNULL (SUM (F050802.A_DELV_QTY), 0)
          FROM F050801
               INNER JOIN F050802
                  ON     F050801.DC_CODE = F050802.DC_CODE
                     AND F050801.GUP_CODE = F050802.GUP_CODE
                     AND F050801.CUST_CODE = F050802.CUST_CODE
                     AND F050801.WMS_ORD_NO = F050802.WMS_ORD_NO
         WHERE     F050801.GUP_CODE = @p0
               AND F050801.CUST_CODE = @p1
               AND F050801.DC_CODE = @p2
               AND F050802.ITEM_CODE = A.ITEM_CODE
               {0})
          DELV_QTY,
       --日結庫存數加總
       (SELECT ISNULL (SUM (END_QTY), 0)
          FROM F5101
         WHERE     ITEM_CODE = A.ITEM_CODE
               {1})
          END_QTY
  FROM (  SELECT F1913.CUST_CODE,
                 F1909.CUST_NAME,
                 F1903.ITEM_CODE,
                 F1903.ITEM_NAME,
				 F1903.LTYPE,
				 CONCAT ( F1903.LTYPE , ' ' , F1915.CLA_NAME) AS LTYPE_NAME,
				 F1903.MTYPE,
				 CONCAT( F1903.MTYPE , ' ' , F1916.CLA_NAME) AS MTYPE_NAME,
				 F1903.STYPE,
				CONCAT( F1903.STYPE , ' ' , F1917.CLA_NAME) AS STYPE_NAME,
                 F1903.ITEM_SIZE,
                 F1903.ITEM_SPEC,
                 F1903.ITEM_COLOR,
                 SUM (F1913.QTY) QTY
            FROM F1913
                 LEFT OUTER JOIN F1912
                    ON     F1913.DC_CODE = F1912.DC_CODE
                       AND F1913.LOC_CODE = F1912.LOC_CODE
                 LEFT OUTER JOIN F1980
                    ON     F1912.DC_CODE = F1980.DC_CODE
                       AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID
                 LEFT OUTER JOIN F198001
                    ON F1980.WAREHOUSE_TYPE = F198001.TYPE_ID                
                 LEFT OUTER JOIN F1903
                    ON     F1913.GUP_CODE = F1903.GUP_CODE
                       AND F1913.CUST_CODE = F1903.CUST_CODE
                       AND F1913.ITEM_CODE = F1903.ITEM_CODE
                 LEFT OUTER JOIN F1915
                    ON     F1903.GUP_CODE = F1915.GUP_CODE
                       AND F1903.LTYPE = F1915.ACODE
                       AND (F1915.CUST_CODE = '0' OR F1915.CUST_CODE = F1913.CUST_CODE)
                 LEFT OUTER JOIN F1916
                    ON     F1903.GUP_CODE = F1916.GUP_CODE
                       AND F1903.LTYPE = F1916.ACODE
                       AND F1903.MTYPE = F1916.BCODE
                       AND (F1916.CUST_CODE = '0' OR F1916.CUST_CODE = F1913.CUST_CODE)
                 LEFT OUTER JOIN F1917
                    ON     F1903.GUP_CODE = F1917.GUP_CODE
                       AND F1903.LTYPE = F1917.ACODE
                       AND F1903.MTYPE = F1917.BCODE
                       AND F1903.STYPE = F1917.CCODE
											AND (F1917.CUST_CODE = '0' OR F1917.CUST_CODE = F1913.CUST_CODE)
                 LEFT OUTER JOIN F1943
                    ON F1912.NOW_STATUS_ID = F1943.LOC_STATUS_ID
                 LEFT OUTER JOIN F1909
                    ON     F1913.GUP_CODE = F1909.GUP_CODE
                       AND F1913.CUST_CODE = F1909.CUST_CODE
           WHERE     F1913.GUP_CODE = @p19
                 AND F1913.CUST_CODE = @p20
                 AND F1913.DC_CODE = @p21
                 {2}
        GROUP BY F1913.CUST_CODE,
                 F1909.CUST_NAME,
                 F1903.ITEM_CODE,
                 F1903.ITEM_NAME,
                 F1903.LTYPE,
                 F1915.CLA_NAME,
                 F1903.MTYPE,
                 F1916.CLA_NAME,
                 F1903.STYPE,
                 F1917.CLA_NAME,
                 F1903.ITEM_SIZE,
                 F1903.ITEM_SPEC,
                 F1903.ITEM_COLOR) A
";

			var parameters = new List<SqlParameter>
												{
																new SqlParameter("@p0", gupCode),
																new SqlParameter("@p1", custCode),
																new SqlParameter("@p2", dcCode)
												};
			var condition = "";
			if (!string.IsNullOrWhiteSpace(typeBegin) && !string.IsNullOrWhiteSpace(typeEnd))
			{
				condition += " AND F1903.TYPE BETWEEN @p3 AND @p4 ";
				parameters.Add(new SqlParameter("@p3", typeBegin));
				parameters.Add(new SqlParameter("@p4", typeEnd));
			}
			if (!string.IsNullOrWhiteSpace(lTypeBegin) && !string.IsNullOrWhiteSpace(lTypeEnd))
			{
				condition += " AND F1903.LTYPE BETWEEN @p5 AND @p6 ";
				parameters.Add(new SqlParameter("@p5", lTypeBegin));
				parameters.Add(new SqlParameter("@p6", lTypeEnd));
			}
			if (!string.IsNullOrWhiteSpace(mTypeBegin) && !string.IsNullOrWhiteSpace(mTypeEnd))
			{
				condition += " AND F1903.MTYPE BETWEEN @p7 AND @p8 ";
				parameters.Add(new SqlParameter("@p7", mTypeBegin));
				parameters.Add(new SqlParameter("@p8", mTypeEnd));
			}
			if (!string.IsNullOrWhiteSpace(sTypeBegin) && !string.IsNullOrWhiteSpace(sTypeEnd))
			{
				condition += " AND F1903.STYPE BETWEEN @p9 AND @p10 ";
				parameters.Add(new SqlParameter("@p9", sTypeBegin));
				parameters.Add(new SqlParameter("@p10", sTypeEnd));
			}
			if (!string.IsNullOrWhiteSpace(enterDateBegin) && !string.IsNullOrWhiteSpace(enterDateEnd))
			{
				condition +=
						" AND F1913.ENTER_DATE BETWEEN CONVERT (datetime, @p11) AND CONVERT (datetime,@p12) ";
				parameters.Add(new SqlParameter("@p11", enterDateBegin));
				parameters.Add(new SqlParameter("@p12", enterDateEnd));
			}
			if (!string.IsNullOrWhiteSpace(validDateBegin) && !string.IsNullOrWhiteSpace(validDateEnd))
			{
				condition +=
						" AND F1913.VALID_DATE BETWEEN CONVERT (datetime,@p13) AND CONVERT (datetime,@p14) ";
				parameters.Add(new SqlParameter("@p13", validDateBegin));
				parameters.Add(new SqlParameter("@p14", validDateEnd));
			}

			if (!string.IsNullOrWhiteSpace(boxCtrlNoBegin) && !string.IsNullOrWhiteSpace(boxCtrlNoEnd))
			{
				condition += " AND F1913.BOX_CTRL_NO BETWEEN @p15 AND @p16 ";
				parameters.Add(new SqlParameter("@p15", boxCtrlNoBegin));
				parameters.Add(new SqlParameter("@p16", boxCtrlNoEnd));
			}

			if (!string.IsNullOrWhiteSpace(palletCtrlNoBegin) && !string.IsNullOrWhiteSpace(palletCtrlNoEnd))
			{
				condition += " AND F1913.PALLET_CTRL_NO BETWEEN @p17 AND @p18 ";
				parameters.Add(new SqlParameter("@p17", palletCtrlNoBegin));
				parameters.Add(new SqlParameter("@p18", palletCtrlNoEnd));
			}

			if (!string.IsNullOrWhiteSpace(itemCodes))
			{
				var codes = string.Join("','", itemCodes.Split(','));
				codes = string.Format("'{0}'", codes);
				condition += string.Format(" AND F1913.ITEM_CODE IN ({0}) ", codes);
			}
			if (boundleSerialNo == "1")
				condition += " AND F1903.BUNDLE_SERIALNO = '1' ";
			if (boundleSerialLoc == "1")
				condition += " AND F1903.BUNDLE_SERIALLOC = '1' ";
			if (multiFlag == "1")
				condition += " AND F1903.MULTI_FLAG = '1' ";
			if (packWareW == "1")
				condition += " AND F1903.PICK_WARE = 'W' ";
			if (virtualType == "1")
				condition += " AND F1903.VIRTUAL_TYPE IS NOT NULL ";

			parameters.Add(new SqlParameter("@p19", gupCode));
			parameters.Add(new SqlParameter("@p20", custCode));
			parameters.Add(new SqlParameter("@p21", dcCode));

			string sql1 = "", sql2 = "";
			int calDay = 0;
			if (!string.IsNullOrWhiteSpace(closeDateBegin) && !string.IsNullOrWhiteSpace(closeDateEnd))
			{
				sql1 =
						string.Format(
								" AND F050801.DELV_DATE BETWEEN CONVERT (datetime,'{0}') AND CONVERT (datetime,'{1}')",
								closeDateBegin, closeDateEnd);
				sql2 = string.Format(
								" AND CAL_DATE BETWEEN CONVERT (datetime,'{0}') AND CONVERT (datetime,'{1}')",
								closeDateBegin, closeDateEnd);
				calDay = (int)Math.Ceiling((Convert.ToDateTime(closeDateEnd).Date - Convert.ToDateTime(closeDateBegin).Date).TotalDays) + 1;
			}

			sql = string.Format(sql, sql1, sql2, condition);

			var query = SqlQuery<StockQueryData3>(sql, parameters.ToArray()).ToList();

			foreach (var q in query)
			{
				if (q.DELV_QTY == 0 || q.END_QTY == 0)
				{
					q.RETURN_RATE = 0;
					q.RETURN_DAY = 0;
				}
				else
				{
					q.RETURN_RATE = q.DELV_QTY / q.END_QTY / calDay;
					q.RETURN_DAY = 365 / q.RETURN_RATE;
				}
			}

			return query.AsQueryable();
		}

    /// <summary>
    /// 取得盤點前商品庫存數值 For商品抽盤、循環盤、全盤、半年盤
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="inventoryWareHouses"></param>
    /// <param name="itemCodeList"></param>
    /// <param name="inventoryDate"></param>
    /// <returns></returns>
		public IQueryable<F1913Ex> GetDatasByInventoryWareHouseList(string dcCode, string gupCode, string custCode,
				List<InventoryWareHouse> inventoryWareHouses, List<string> itemCodeList, DateTime inventoryDate)
		{
			var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar },

      };
			var sql = @" SELECT ISNULL(SUM(B.B_PICK_QTY),0) UNMOVE_STOCK_QTY,
                            A.*
                     FROM ( ";

			var f1912Condition = string.Empty;
			if (inventoryWareHouses.Any())
			{
				var sqlList = new List<string>();
				foreach (var inventoryWareHouse in inventoryWareHouses)
				{
					var sqlStr = " (B.DC_CODE =@p0 AND B.WAREHOUSE_ID = '" + inventoryWareHouse.WAREHOUSE_ID + "' ";
					if (!string.IsNullOrWhiteSpace(inventoryWareHouse.AREA_CODE))
						sqlStr += " AND B.AREA_CODE = '" + inventoryWareHouse.AREA_CODE + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_BEGIN))
						sqlStr += " AND B.FLOOR >= '" + inventoryWareHouse.FLOOR_BEGIN + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_END))
						sqlStr += " AND B.FLOOR <= '" + inventoryWareHouse.FLOOR_END + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_BEGIN))
						sqlStr += " AND B.CHANNEL >= '" + inventoryWareHouse.CHANNEL_BEGIN + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_END))
						sqlStr += " AND B.CHANNEL <= '" + inventoryWareHouse.CHANNEL_END + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_BEGIN))
						sqlStr += " AND B.PLAIN >= '" + inventoryWareHouse.PLAIN_BEGIN + "' ";
					if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_END))
            sqlStr += " AND B.PLAIN <= '" + inventoryWareHouse.PLAIN_END + "' ";
          sqlStr += " ) ";
          sqlList.Add(sqlStr);
        }
        f1912Condition += " AND ( " + string.Join(" OR ", sqlList.ToArray()) + ") ";
      }

      sql += $@" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,B.WAREHOUSE_ID,A.SERIAL_NO,A.QTY,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
				             FROM F1913 A
				            INNER JOIN F1912 B
				               ON B.DC_CODE = A.DC_CODE
				              AND B.LOC_CODE = A.LOC_CODE
				            WHERE A.DC_CODE = @p0
				              AND A.GUP_CODE = @p1
				              AND A.CUST_CODE = @p2
				              AND A.QTY >= 0 
											{f1912Condition} ";

      var itemSql = "";
      if (itemCodeList.Any())
        itemSql = param.CombineSqlInParameters("AND A.ITEM_CODE", itemCodeList, SqlDbType.VarChar);
      
      sql += itemSql;
      sql += " ) A ";
      sql += @"  
          LEFT JOIN 
          VW_VirtualStock  B ON A.DC_CODE = B.DC_CODE 
          AND A.GUP_CODE = B.GUP_CODE 
          AND A.CUST_CODE = B.CUST_CODE 
          AND A.LOC_CODE = B.LOC_CODE 
          AND A.ITEM_CODE = B.ITEM_CODE 
          AND A.VALID_DATE = B.VALID_DATE 
          AND A.ENTER_DATE = B.ENTER_DATE 
          AND A.MAKE_NO = B.MAKE_NO 
          AND A.BOX_CTRL_NO = B.BOX_CTRL_NO 
          AND A.PALLET_CTRL_NO = B.PALLET_CTRL_NO 
          AND A.SERIAL_NO = B.SERIAL_NO
          AND B.STATUS = '0' 
        GROUP BY 
          A.DC_CODE, 
          A.GUP_CODE, 
          A.CUST_CODE, 
          A.LOC_CODE, 
          A.ITEM_CODE, 
          A.VALID_DATE, 
          A.ENTER_DATE, 
          A.WAREHOUSE_ID, 
          A.SERIAL_NO, 
          A.QTY, 
          A.BOX_CTRL_NO, 
          A.PALLET_CTRL_NO, 
          A.MAKE_NO
";

      #region 撈出F1913沒有但虛擬庫存有的資料
      sql += $@" 
UNION ALL
SELECT 
       A.B_PICK_QTY AS UNMOVE_STOCK_QTY,
       A.DC_CODE,
       A.GUP_CODE,
       A.CUST_CODE,
       A.LOC_CODE,
       A.ITEM_CODE,
       A.VALID_DATE,
       A.ENTER_DATE,
       B.WAREHOUSE_ID,
       A.SERIAL_NO,
       0            AS QTY,
       A.BOX_CTRL_NO,
       A.PALLET_CTRL_NO,
       A.MAKE_NO
FROM   VW_VIRTUALSTOCK A
       INNER JOIN F1912 B
               ON B.DC_CODE = A.DC_CODE
                  AND B.LOC_CODE = A.LOC_CODE
WHERE  A.DC_CODE = @p0
       AND A.GUP_CODE = @p1
       AND A.CUST_CODE = @p2
       AND A.STATUS = '0'
       {f1912Condition}
       {itemSql}
       AND NOT EXISTS (SELECT TOP 1 1
                       FROM   F1913 C
                       WHERE  A.DC_CODE = C.DC_CODE
                              AND A.GUP_CODE = C.GUP_CODE
                              AND A.CUST_CODE = C.CUST_CODE
                              AND A.LOC_CODE = C.LOC_CODE
                              AND A.ITEM_CODE = C.ITEM_CODE
                              AND A.VALID_DATE = C.VALID_DATE
                              AND A.ENTER_DATE = C.ENTER_DATE
                              AND A.MAKE_NO = C.MAKE_NO
                              AND A.BOX_CTRL_NO = C.BOX_CTRL_NO
                              AND A.PALLET_CTRL_NO = C.PALLET_CTRL_NO
                              AND C.SERIAL_NO = C.SERIAL_NO) 
";
      #endregion

      #region 產生ROWNUM
      sql = $@"
SELECT   ROW_NUMBER ()OVER(ORDER BY LOC_CODE,ITEM_CODE,VALID_DATE,ENTER_DATE,DC_CODE,GUP_CODE,CUST_CODE,SERIAL_NO,MAKE_NO,BOX_CTRL_NO,PALLET_CTRL_NO) ROWNUM,
         SUM(UNMOVE_STOCK_QTY) UNMOVE_STOCK_QTY,
         DC_CODE,
         GUP_CODE,
         CUST_CODE,
         LOC_CODE,
         ITEM_CODE,
         VALID_DATE,
         ENTER_DATE,
         WAREHOUSE_ID,
         SERIAL_NO,
         SUM(QTY) QTY,
         BOX_CTRL_NO,
         PALLET_CTRL_NO,
         MAKE_NO
FROM ({sql}) AS RNT
GROUP BY DC_CODE,
         GUP_CODE,
         CUST_CODE,
         LOC_CODE,
         ITEM_CODE,
         VALID_DATE,
         ENTER_DATE,
         WAREHOUSE_ID,
         SERIAL_NO,
         BOX_CTRL_NO,
         PALLET_CTRL_NO,
         MAKE_NO";
      #endregion

      var result = SqlQuery<F1913Ex>(sql, param.ToArray());
      return result;
		}

    /// <summary>
    /// 取得盤點前商品庫存數值 For異動盤
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="inventoryWareHouses"></param>
    /// <param name="itemCodeList"></param>
    /// <param name="inventoryDate"></param>
    /// <returns></returns>
    public IQueryable<F1913Ex> GetDatasByInventoryWareHouseChangeList(string dcCode, string gupCode, string custCode,
    List<InventoryWareHouse> inventoryWareHouses, List<string> itemCodeList, DateTime inventoryDate)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar },

      };
      var sql = @" SELECT  ROW_NUMBER ()OVER(ORDER BY A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.SERIAL_NO ,A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO) ROWNUM,ISNULL(SUM(B.B_PICK_QTY),0) UNMOVE_STOCK_QTY,
                            A.*
                     FROM ( ";

      var f1912Condition = string.Empty;
      if (inventoryWareHouses.Any())
      {
        var sqlList = new List<string>();
        foreach (var inventoryWareHouse in inventoryWareHouses)
        {
          var sqlStr = " (B.DC_CODE =@p0 AND B.WAREHOUSE_ID = '" + inventoryWareHouse.WAREHOUSE_ID + "' ";
          if (!string.IsNullOrWhiteSpace(inventoryWareHouse.AREA_CODE))
            sqlStr += " AND B.AREA_CODE = '" + inventoryWareHouse.AREA_CODE + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_BEGIN))
            sqlStr += " AND B.FLOOR >= '" + inventoryWareHouse.FLOOR_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_END))
            sqlStr += " AND B.FLOOR <= '" + inventoryWareHouse.FLOOR_END + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_BEGIN))
            sqlStr += " AND B.CHANNEL >= '" + inventoryWareHouse.CHANNEL_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_END))
            sqlStr += " AND B.CHANNEL <= '" + inventoryWareHouse.CHANNEL_END + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_BEGIN))
            sqlStr += " AND B.PLAIN >= '" + inventoryWareHouse.PLAIN_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_END))
            sqlStr += " AND B.PLAIN <= '" + inventoryWareHouse.PLAIN_END + "' ";
          sqlStr += " ) ";
          sqlList.Add(sqlStr);
        }
        f1912Condition += " AND ( " + string.Join(" OR ", sqlList.ToArray()) + ") ";
      }

      param.Add(new SqlParameter("@p3", inventoryDate) { SqlDbType = SqlDbType.DateTime2 });
      param.Add(new SqlParameter("@p4", inventoryDate.AddDays(1).AddSeconds(-1)) { SqlDbType = SqlDbType.DateTime2 });
      var mainSql = @" SELECT DISTINCT M.*
										FROM (
											 SELECT   A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_LOC AS LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
														FROM F051202 A --揀貨
													 INNER JOIN F051201 B
															ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
														 AND B.CUST_CODE = A.CUST_CODE
														 AND B.PICK_ORD_NO = A.PICK_ORD_NO
													 WHERE A.DC_CODE = @p0
														 AND A.GUP_CODE =@p1
														 AND A.CUST_CODE =@p2
														 AND B.DELV_DATE >= @p3
														 AND B.DELV_DATE <=  @p4
														 AND A.PICK_STATUS ='1' --揀貨完成
										 UNION ALL
										 SELECT B.SRC_DC_CODE AS DC_CODE,A.GUP_CODE,A.CUST_CODE,A.SRC_LOC_CODE AS LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
														 FROM F151002 A --調撥(下架)
															INNER JOIN F151001 B
															ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
														 AND B.CUST_CODE = A.CUST_CODE
														 AND B.ALLOCATION_NO = A.ALLOCATION_NO
														WHERE A.DC_CODE =@p0
															AND A.GUP_CODE =@p1
															AND A.CUST_CODE =@p2
															AND A.UPD_DATE >= @p3
															AND A.UPD_DATE <= @p4
															AND A.SRC_LOC_CODE <>'000000000'
															AND A.STATUS >=1 --已下架
										 UNION ALL
										 SELECT B.TAR_DC_CODE AS DC_CODE,A.GUP_CODE,A.CUST_CODE,A.TAR_LOC_CODE AS LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
														 FROM F151002 A
															INNER JOIN F151001 B --調撥(上架)
															ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
														 AND B.CUST_CODE = A.CUST_CODE
														 AND B.ALLOCATION_NO = A.ALLOCATION_NO
														WHERE A.DC_CODE =@p0
															AND A.GUP_CODE =@p1
															AND A.CUST_CODE =@p2
															AND A.UPD_DATE >= @p3
															AND A.UPD_DATE <= @p4
															AND A.TAR_LOC_CODE <>'000000000'
															AND A.STATUS =2 --已上架
										 UNION ALL
										 SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
														 FROM F200103 A --異動調整
														WHERE A.DC_CODE =@p0
															AND A.GUP_CODE =@p1
															AND A.CUST_CODE =@p2
															AND A.CRT_DATE >=@p3
															AND A.CRT_DATE <=@p4
										 UNION ALL
										 SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_LOC AS LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
														 FROM F91020501 A --加工揀料單
														WHERE A.DC_CODE =@p0
															AND A.GUP_CODE =@p1
															AND A.CUST_CODE =@p2
															AND A.CRT_DATE >=@p3
															AND A.CRT_DATE <=@p4
										) M ";

      sql += string.Format(@"
                    SELECT A.*,B.WAREHOUSE_ID
                      FROM (
                      --取得有異動的儲位和品項但有庫存資料(不看入庫日和生效日)
                      SELECT C.DC_CODE,C.GUP_CODE,C.CUST_CODE,C.LOC_CODE,C.ITEM_CODE,C.VALID_DATE ,C.ENTER_DATE ,C.SERIAL_NO,C.QTY,C.BOX_CTRL_NO,C.PALLET_CTRL_NO,C.MAKE_NO
												FROM F1913 C
												WHERE EXISTS (
													{0}
													WHERE M.DC_CODE =C.DC_CODE
																			AND M.GUP_CODE = C.GUP_CODE
																			AND M.CUST_CODE = C.CUST_CODE
																			AND M.LOC_CODE = C.LOC_CODE
																			AND M.ITEM_CODE = C.ITEM_CODE
											)
											UNION ALL
                      --取得有異動的儲位和品項但無庫存資料
											SELECT A2.DC_CODE,A2.GUP_CODE,A2.CUST_CODE,A2.LOC_CODE,A2.ITEM_CODE,A2.VALID_DATE ,A2.ENTER_DATE ,'0' AS SERIAL_NO,0 AS QTY,A2.BOX_CTRL_NO,A2.PALLET_CTRL_NO AS PALLET_CTRL_NO,A2.MAKE_NO
												FROM (
												 {0}
												 ) A2
												 LEFT JOIN F1913 C
													 ON C.DC_CODE = A2.DC_CODE
													AND C.GUP_CODE = A2.GUP_CODE
													AND C.CUST_CODE = A2.CUST_CODE
													AND C.LOC_CODE = A2.LOC_CODE
													AND C.ITEM_CODE = A2.ITEM_CODE
												WHERE C.LOC_CODE  IS NULL
                      ) A
                      INNER JOIN F1912 B
										    ON B.DC_CODE =A.DC_CODE
										   AND B.LOC_CODE = A.LOC_CODE
											{1}
										 WHERE 1 = 1
               ", mainSql, f1912Condition);


      if (itemCodeList.Any())
        sql += param.CombineSqlInParameters("AND A.ITEM_CODE", itemCodeList, SqlDbType.VarChar);

      sql += " ) A ";
      sql += @"  
          LEFT JOIN 
          VW_VirtualStock  B ON A.DC_CODE = B.DC_CODE 
          AND A.GUP_CODE = B.GUP_CODE 
          AND A.CUST_CODE = B.CUST_CODE 
          AND A.LOC_CODE = B.LOC_CODE 
          AND A.ITEM_CODE = B.ITEM_CODE 
          AND A.VALID_DATE = B.VALID_DATE 
          AND A.ENTER_DATE = B.ENTER_DATE 
          AND A.MAKE_NO = B.MAKE_NO 
          AND A.BOX_CTRL_NO = B.BOX_CTRL_NO 
          AND A.PALLET_CTRL_NO = B.PALLET_CTRL_NO 
          AND (
            A.SERIAL_NO =
            CASE 
              WHEN B.SERIAL_NO IS NULL THEN '0' 
              WHEN B.SERIAL_NO ='' THEN '0'
              ELSE B.SERIAL_NO
            END
          ) 
          AND B.STATUS = '0' 
        GROUP BY 
          A.DC_CODE, 
          A.GUP_CODE, 
          A.CUST_CODE, 
          A.LOC_CODE, 
          A.ITEM_CODE, 
          A.VALID_DATE, 
          A.ENTER_DATE, 
          A.WAREHOUSE_ID, 
          A.SERIAL_NO, 
          A.QTY, 
          A.BOX_CTRL_NO, 
          A.PALLET_CTRL_NO, 
          A.MAKE_NO";
      var result = SqlQuery<F1913Ex>(sql, param.ToArray());
      return result;
    }

    public IQueryable<P710705BackWarehouseInventory> GetP710705BackWarehouseInventory(
				string dcCode, string gupCode, string custCode, string vnrCode, string account)
		{
			string sql = @"
							SELECT ROW_NUMBER ()OVER(ORDER BY T.LOC_CODE,T.ITEM_CODE,T.GUP_CODE,T.CUST_CODE ,T.VNR_CODE) ROWNUM,
								   T.*,
								   F1909.CUST_NAME,
								   F1929.GUP_NAME,
								   F1908.VNR_NAME
							  FROM (  SELECT top 100 percent F1913.GUP_CODE,
											 F1913.CUST_CODE,
											 F190303.VNR_CODE,
											 F1913.LOC_CODE,
											 F1913.ITEM_CODE,
											 F1903.ITEM_NAME,
											 SUM (F1913.QTY) TOTAL_QTY
										FROM F1913
										JOIN F1903 ON F1903.ITEM_CODE = F1913.ITEM_CODE
										JOIN (
                                                SELECT top 100 percent
                                                    F1.GUP_CODE, F1.CUST_CODE, F1.ITEM_CODE
                                                    ,MAX(VNR_CODE) AS VNR_CODE
                                                FROM F190303 F1
                                                INNER JOIN 
												(SELECT GUP_CODE,CUST_CODE,ITEM_CODE,MAX(CRT_DATE) MAX_CRT_DATE
												FROM F190303 F1
												GROUP BY GUP_CODE,CUST_CODE,ITEM_CODE) F2
												ON F2.GUP_CODE = F1.GUP_CODE 
												AND F2.CUST_CODE = F1.CUST_CODE
												AND F2.ITEM_CODE = F1.ITEM_CODE
												AND F2.MAX_CRT_DATE = F1.CRT_DATE
												GROUP BY  F1.GUP_CODE,F1.CUST_CODE,F1.ITEM_CODE
                                                ORDER BY F1.ITEM_CODE
                                             ) F190303 ON F1913.GUP_CODE = F190303.GUP_CODE
                                                   AND F1913.CUST_CODE = F190303.CUST_CODE
                                                   AND F1913.ITEM_CODE = F190303.ITEM_CODE

									   WHERE F1913.QTY > 0 AND F1913.DC_CODE = @p0 {0}
									GROUP BY F1913.GUP_CODE,
											 F1913.CUST_CODE,
											 F190303.VNR_CODE,
											 F1913.LOC_CODE,
											 F1913.ITEM_CODE,
											 F1903.ITEM_NAME
									ORDER BY GUP_CODE,
											 CUST_CODE,
											 VNR_CODE,
											 LOC_CODE,
											 ITEM_CODE) T
								   LEFT JOIN F1909 ON F1909.GUP_CODE = T.GUP_CODE AND F1909.CUST_CODE = T.CUST_CODE
								   LEFT JOIN F1929 ON F1929.GUP_CODE = T.GUP_CODE
								   LEFT JOIN F1908 ON F1908.VNR_CODE = T.VNR_CODE AND F1908.GUP_CODE = T.GUP_CODE
			";

			var param = new List<object>() {
																dcCode
												};

			var whereSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(gupCode))
			{
				whereSql += param.Combine(" AND F1913.GUP_CODE = @p{0} ", gupCode);
			}
			else //業主全部要去篩選只有此物流中心業主或業主設為共用
			{
				whereSql += param.Combine(@" AND ((F1913.GUP_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 aa
																				 INNER JOIN (SELECT *
																											 FROM F192402
																										  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE
																						  WHERE aa.DC_CODE = F1913.DC_CODE AND aa.GUP_CODE = F1913.GUP_CODE))) ", account);
			}

			if (!string.IsNullOrWhiteSpace(custCode))
			{
				whereSql += param.Combine(" AND F1913.CUST_CODE = @p{0} ", custCode);
			}
			else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
			{
				whereSql += param.Combine(@" AND ((F1913.CUST_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 cc
																				 INNER JOIN (SELECT *
																											 FROM F192402
																											WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
																							WHERE cc.DC_CODE = F1913.DC_CODE AND cc.GUP_CODE = F1913.GUP_CODE AND cc.CUST_CODE = F1913.CUST_CODE))) ", account);
			}

			whereSql += param.CombineNotNullOrEmpty(" AND F190303.VNR_CODE = @p{0} ", vnrCode);

			sql = string.Format(sql, whereSql);

			var result = SqlQuery<P710705BackWarehouseInventory>(sql, param.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<P710705MergeExecution> GetP710705MergeExecution(string dcCode, int? qty)
		{
			//
			// 儲位合併執行表 不是用這 SQL，method name一樣而已，真正在用的是 GetP710705LocMergeExecution 在最下面，
			// 因為這個原本這個是真的要額外呼叫建議儲位的，但會 Timeout，就先改成簡易版本。
			//
			string sql = string.Format(@"
										SELECT ROW_NUMBER ()OVER(ORDER BY T.LOC_CODE,T.ITEM_CODE,T.VALID_DATE,T.ENTER_DATE,T.DC_CODE,T.GUP_CODE,T.CUST_CODE ) ROWNUM,
											   T.DC_CODE,
											   F1901.DC_NAME,
											   T.GUP_CODE,
											   F1929.GUP_NAME,
											   T.CUST_CODE,
											   F1909.CUST_NAME,
											   T.LOC_CODE,
											   T.ITEM_CODE,
											   F1903.ITEM_NAME,
											   T.VALID_DATE,
											   T.ENTER_DATE,
											   T.TOTAL_QTY,
											   F1912.WAREHOUSE_ID,
													 '' TAR_LOC_CODE
										  FROM (  SELECT F1913.DC_CODE,
														 F1913.GUP_CODE,
														 F1913.CUST_CODE,
														 F1913.LOC_CODE,
														 F1913.ITEM_CODE,
														 F1913.VALID_DATE,
														 F1913.ENTER_DATE,
														 SUM (F1913.QTY ) TOTAL_QTY
													FROM F1913
												   WHERE DC_CODE = @p0
												GROUP BY DC_CODE,
														 GUP_CODE,
														 CUST_CODE,
														 LOC_CODE,
														 ITEM_CODE,
														 VALID_DATE,
														 ENTER_DATE) T
											   JOIN F1912 ON T.DC_CODE = F1912.DC_CODE AND T.LOC_CODE = F1912.LOC_CODE
											   JOIN F1903 ON T.GUP_CODE = F1903.GUP_CODE AND T.ITEM_CODE = F1903.ITEM_CODE AND T.CUST_CODE = F1903.CUST_CODE
											   LEFT JOIN F1901 ON F1901.DC_CODE = T.DC_CODE
											   LEFT JOIN F1909 ON F1909.GUP_CODE = T.GUP_CODE AND F1909.CUST_CODE = T.CUST_CODE
											   LEFT JOIN F1929 ON F1929.GUP_CODE = T.GUP_CODE
											WHERE T.TOTAL_QTY > 0
			");

			var parameter = new List<object>() {
																dcCode
												};
			if (qty.HasValue)
			{
				sql += " AND T.TOTAL_QTY < @p" + parameter.Count;
				parameter.Add(qty);
			}
			var result = SqlQuery<P710705MergeExecution>(sql, parameter.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<P710705Availability> GetP710705Availability(string dcCode, string gupCode, string custCode, string inventoryDate, string account)
		{
			var parameter = new List<object>() {
																dcCode
												};
			string sql = @"
SELECT ROW_NUMBER ()OVER(ORDER BY F1912.DC_CODE,F1912.LOC_CODE) ROWNUM,
       F1912.GUP_CODE,
       ISNULL(F1929.GUP_NAME, '共用') GUP_NAME,
       F1912.CUST_CODE,
       ISNULL(F1909.CUST_NAME, '共用') CUST_NAME,
       F1912.WAREHOUSE_ID,
       F1980.WAREHOUSE_NAME,
       F1912.USED_VOLUMN,
       F1912.USEFUL_VOLUMN,
       CONVERT(CHAR,F1912.RENT_END_DATE, 111) RENT_END_DATE,
       F1912.LOC_CODE F1912_LOC_CODE,
       F1913.LOC_CODE F1913_LOC_CODE,
       '{2}' INVENTORYDATE,
       0 CountF1912,
       0 CountF1913,
       0 CountF1913Null,
       0 FillRate
  FROM F1912
       LEFT JOIN F1913
          ON     F1912.DC_CODE = F1913.DC_CODE
             AND F1912.LOC_CODE = F1913.LOC_CODE
        LEFT JOIN F1909 ON F1909.GUP_CODE = F1912.GUP_CODE AND F1909.CUST_CODE = F1912.CUST_CODE
        LEFT JOIN F1929 ON F1929.GUP_CODE = F1912.GUP_CODE
        LEFT JOIN F1980 ON F1980.DC_CODE = F1912.DC_CODE AND F1980.WAREHOUSE_ID = F1912.WAREHOUSE_ID
 WHERE     F1912.DC_CODE = @p0
					 {0} {1} ";

			var whereSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(gupCode))
			{
				whereSql += parameter.Combine(" AND F1913.GUP_CODE = @p{0} ", gupCode);
			}
			else //業主全部要去篩選只有此物流中心業主或業主設為共用
			{
				whereSql += parameter.Combine(@" AND ((F1913.GUP_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 aa
																				 INNER JOIN (SELECT *
																											 FROM F192402
																										  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE
																						  WHERE aa.DC_CODE = F1913.DC_CODE AND aa.GUP_CODE = F1913.GUP_CODE))) ", account);
			}

			if (!string.IsNullOrWhiteSpace(custCode))
			{
				whereSql += parameter.Combine(" AND F1913.CUST_CODE = @p{0} ", custCode);
			}
			else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
			{
				whereSql += parameter.Combine(@" AND ((F1913.CUST_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 cc
																				 INNER JOIN (SELECT *
																											 FROM F192402
																											WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
																							WHERE cc.DC_CODE = F1913.DC_CODE AND cc.GUP_CODE = F1913.GUP_CODE AND cc.CUST_CODE = F1913.CUST_CODE))) ", account);
			}

			var inSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(inventoryDate))
			{
				inSql += String.Format(@"
AND F1912.LOC_CODE IN (SELECT DISTINCT F140104.LOC_CODE
                        FROM F140101
                              JOIN F140104
                                ON     F140101.DC_CODE = F140104.DC_CODE
                                    AND F140101.GUP_CODE = F140104.GUP_CODE
                                    AND F140101.CUST_CODE = F140104.CUST_CODE
												WHERE F140101.INVENTORY_DATE = CONVERT(CHAR,@p{0},111) )", parameter.Count);
				parameter.Add(inventoryDate);
			}

			sql = string.Format(sql, whereSql, inSql, inventoryDate);
			var result = SqlQuery<P710705Availability>(sql, parameter.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<P710705ChangeDetail> GetP710705ChangeDetail(string warehouseId,
				string startLocCode, string endLocCode, string itemCodes, DateTime? updateDateBegin,
				DateTime? updateDateEnd)
		{
			var paramList = new List<object>() { };
			string formatSql = @"SELECT ROW_NUMBER ()OVER(ORDER BY F.LOC_CODE,F.ITEM_CODE) ROWNUM, F.*
								  FROM (  SELECT TOP 100 PERCENT A.UPDATE_DATE,
												 B.WAREHOUSE_ID,
												 D.WAREHOUSE_NAME,
												 A.SRC_LOC_CODE AS LOC_CODE,
												 A.ITEM_CODE,
												 C.ITEM_NAME,
												 A.ACTION,
												 A.SRC_LOC_CODE,
												 A.TAR_LOC_CODE,
												 SUM (ISNULL (A.A_TAR_QTY, 0)) AS A_TAR_QTY,
												 SUM (ISNULL (E.QTY, 0)) AS SUM_QTY
											FROM (  SELECT E.DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.SRC_LOC_CODE,
														   E.TAR_LOC_CODE,
														   E.ITEM_CODE,
														   '調撥' AS ACTION,
														   DATEADD(DAY,DATEDIFF(DAY,0,F.CRT_DATE),0) AS UPDATE_DATE ,
														   SUM (
															  CASE
																 WHEN F.TAR_WAREHOUSE_ID IS NOT NULL
																 THEN
																	E.A_TAR_QTY
																 ELSE
																	E.A_SRC_QTY
															  END)
															  AS A_TAR_QTY
													  FROM F151002 E
														   JOIN F151001 F
															  ON     F.DC_CODE = E.DC_CODE
																 AND F.GUP_CODE = E.GUP_CODE
																 AND F.CUST_CODE = E.CUST_CODE
																 AND F.ALLOCATION_NO = E.ALLOCATION_NO
																 AND F.STATUS = '5'
												  GROUP BY E.DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.SRC_LOC_CODE,
														   E.TAR_LOC_CODE,
														   E.ITEM_CODE,
														   DATEADD(DAY,DATEDIFF(DAY,0,F.CRT_DATE ),0)
												  UNION ALL
													SELECT E.DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.PICK_LOC,
														   '',
														   E.ITEM_CODE,
														   '揀貨',
														   DATEADD(DAY,DATEDIFF(DAY,0,E.UPD_DATE ),0),
														   SUM (E.A_PICK_QTY) AS A_PICK_QTY
													  FROM F051202 E
														   JOIN F051201 F
															  ON     F.DC_CODE = E.DC_CODE
																 AND F.GUP_CODE = E.GUP_CODE
																 AND F.CUST_CODE = E.CUST_CODE
																 AND F.PICK_ORD_NO = E.PICK_ORD_NO
																 AND F.PICK_STATUS = '2'
												  GROUP BY E.DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.PICK_LOC,
														   E.ITEM_CODE,
														   DATEADD(DAY,DATEDIFF(DAY,0,E.UPD_DATE ),0)
												  UNION ALL
													SELECT DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.PICK_LOC,
														   '',
														   E.ITEM_CODE,
														   '加工揀料',
														   DATEADD(DAY,DATEDIFF(DAY,0,E.CRT_DATE ),0),
														   SUM (E.PICK_QTY) AS PICK_QTY
													  FROM F91020501 E
												  GROUP BY E.DC_CODE,
														   E.GUP_CODE,
														   E.CUST_CODE,
														   E.PICK_LOC,
														   E.ITEM_CODE,
														   DATEADD(DAY,DATEDIFF(DAY,0,E.CRT_DATE ),0)) A
												 LEFT JOIN (  SELECT DC_CODE,
																	 GUP_CODE,
																	 CUST_CODE,
																	 ITEM_CODE,
																	 LOC_CODE,
																	 SUM (QTY) AS QTY
																FROM F1913
															GROUP BY DC_CODE,
																	 GUP_CODE,
																	 CUST_CODE,
																	 ITEM_CODE,
																	 LOC_CODE) E
													ON     A.DC_CODE = E.DC_CODE
													   AND A.GUP_CODE = E.GUP_CODE
													   AND A.CUST_CODE = E.CUST_CODE
													   AND A.ITEM_CODE = E.ITEM_CODE
													   AND A.SRC_LOC_CODE = E.LOC_CODE
												 LEFT JOIN F1912 B
													ON A.DC_CODE = B.DC_CODE AND A.SRC_LOC_CODE = B.LOC_CODE
												 LEFT JOIN F1903 C
													ON A.GUP_CODE = C.GUP_CODE AND A.ITEM_CODE = C.ITEM_CODE AND A.CUST_CODE = C.CUST_CODE
												 LEFT JOIN F1980 D
													ON     B.DC_CODE = D.DC_CODE
													   AND B.WAREHOUSE_ID = D.WAREHOUSE_ID
										   WHERE 1 = 1  {0}
										GROUP BY A.UPDATE_DATE,
												 B.WAREHOUSE_ID,
												 D.WAREHOUSE_NAME,
												 A.ITEM_CODE,
												 C.ITEM_NAME,
												 A.ACTION,
												 A.SRC_LOC_CODE,
												 A.TAR_LOC_CODE
										ORDER BY A.UPDATE_DATE,
												 B.WAREHOUSE_ID,
												 A.ITEM_CODE,
												 A.ACTION) F";

			var whereSql = paramList.CombineNotNullOrEmpty(@" AND @p{0} <= A.SRC_LOC_CODE ", startLocCode);
			whereSql += paramList.CombineNotNullOrEmpty(@" AND A.SRC_LOC_CODE <= @p{0} ", endLocCode);
			whereSql += paramList.CombineNotNullOrEmpty(@" AND (@p{0} <= A.TAR_LOC_CODE OR A.TAR_LOC_CODE IS NULL)", startLocCode);
			whereSql += paramList.CombineNotNullOrEmpty(@" AND (A.TAR_LOC_CODE <= @p{0} OR A.TAR_LOC_CODE IS NULL)", endLocCode);

			var itemCodeData = string.IsNullOrWhiteSpace(itemCodes) ? null : itemCodes.Split('^');
			if (itemCodeData != null && itemCodeData.Any())
			{
				whereSql += paramList.CombineSqlInParameters("AND A.ITEM_CODE", itemCodeData);
			}

			if (updateDateBegin.HasValue)
			{
				whereSql += paramList.Combine(@" AND convert(datetime,@p{0}) <= A.UPDATE_DATE ", updateDateBegin);
			}

			if (updateDateEnd.HasValue)
			{
				whereSql += paramList.Combine(@" AND A.UPDATE_DATE <= convert(datetime,@p{0})", updateDateEnd);
			}

			whereSql += paramList.CombineNotNullOrEmpty(" AND B.WAREHOUSE_ID = @p{0}", warehouseId);

			var sql = string.Format(formatSql, whereSql);
			var result = SqlQuery<P710705ChangeDetail>(sql, paramList.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<P710705WarehouseDetail> GetP710705WarehouseDetail(string gupCode, string custCode, string warehouseId, string srcLocCode, string tarLocCode, string itemCode, string account)
		{
			var parameter = new List<object>() { };
			string sql = @"
SELECT ROW_NUMBER ()OVER(ORDER BY T.LOC_CODE,T.ITEM_CODE,T.GUP_CODE,T.CUST_CODE ) ROWNUM, T.* FROM (
SELECT F1913.GUP_CODE,F1929.GUP_NAME,F1913.CUST_CODE,F1909.CUST_NAME,F1912.WAREHOUSE_ID,F1980.WAREHOUSE_NAME,F1913.LOC_CODE,F1913.ITEM_CODE,F1903.ITEM_NAME,SUM(F1913.QTY) TOTAL_QTY ,MIN(F1913.CRT_DATE) CRT_DATE
FROM F1913
JOIN F1912 ON F1913.DC_CODE = F1912.DC_CODE AND F1913.LOC_CODE = F1912.LOC_CODE
JOIN F1903 ON F1913.ITEM_CODE = F1903.ITEM_CODE AND F1903.GUP_CODE = F1913.GUP_CODE AND F1903.CUST_CODE = F1913.CUST_CODE 
JOIN F1980 ON F1980.DC_CODE = F1913.DC_CODE AND F1980.WAREHOUSE_ID = F1912.WAREHOUSE_ID
LEFT JOIN F1909 ON F1909.GUP_CODE = F1913.GUP_CODE AND F1909.CUST_CODE = F1913.CUST_CODE
LEFT JOIN F1929 ON F1929.GUP_CODE = F1913.GUP_CODE
WHERE 1=1 {0}
GROUP BY F1913.GUP_CODE,F1929.GUP_NAME,F1913.CUST_CODE,F1909.CUST_NAME,F1912.WAREHOUSE_ID,F1980.WAREHOUSE_NAME,F1913.LOC_CODE,F1913.ITEM_CODE,F1903.ITEM_NAME
) T
";
			var whereSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(gupCode))
			{
				whereSql += parameter.Combine(" AND F1913.GUP_CODE = @p{0} ", gupCode);
			}
			else //業主全部要去篩選只有此物流中心業主或業主設為共用
			{
				whereSql += parameter.Combine(@" AND ((F1913.GUP_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 aa
																				 INNER JOIN (SELECT *
																											 FROM F192402
																										  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE
																						  WHERE aa.DC_CODE = F1913.DC_CODE AND aa.GUP_CODE = F1913.GUP_CODE))) ", account);
			}

			if (!string.IsNullOrWhiteSpace(custCode))
			{
				whereSql += parameter.Combine(" AND F1913.CUST_CODE = @p{0} ", custCode);
			}
			else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
			{
				whereSql += parameter.Combine(@" AND ((F1913.CUST_CODE ='0') OR (EXISTS (SELECT 1
																							 FROM F190101 cc
																				 INNER JOIN (SELECT *
																											 FROM F192402
																											WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
																							WHERE cc.DC_CODE = F1913.DC_CODE AND cc.GUP_CODE = F1913.GUP_CODE AND cc.CUST_CODE = F1913.CUST_CODE))) ", account);
			}

			if (!string.IsNullOrWhiteSpace(warehouseId))
			{
				whereSql += " AND F1912.WAREHOUSE_ID = @p" + parameter.Count;
				parameter.Add(warehouseId);
			}
			if (!string.IsNullOrWhiteSpace(srcLocCode))
			{
				whereSql += " AND F1913.LOC_CODE >= @p" + parameter.Count;
				parameter.Add(srcLocCode);
			}
			if (!string.IsNullOrWhiteSpace(tarLocCode))
			{
				whereSql += " AND F1913.LOC_CODE <= @p" + parameter.Count;
				parameter.Add(tarLocCode);
			}
			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				whereSql += " AND F1913.ITEM_CODE = @p" + parameter.Count;
				parameter.Add(itemCode);
			}
			sql = string.Format(sql, whereSql);

			var result = SqlQuery<P710705WarehouseDetail>(sql, parameter.ToArray()).ToList();
			return result.AsQueryable();
		}

		/// <summary>
		/// 取出貨單(黃金儲位) 相關資料
		/// </summary>
		/// <param name="checkDay1">過去未來幾天 訂單數量</param>
		/// <param name="checkDay2">過去未來幾天 出貨次數</param>
		public IQueryable<SchF700501Data> GetSchOrderData(int checkDay1 = 7, int checkDay2 = 14)
		{
			var sql = @"

						SELECT
							A.DC_CODE , A.DC_NAME  ,A.GUP_CODE , A.GUP_NAME ,A.CUST_CODE , A.CUST_NAME
							, A.AREA_CODE , A.WAREHOUSE_ID , A.ITEM_CODE
							, A.ITEM_NAME , A.ITEM_COLOR , A.ITEM_SIZE , A.ITEM_SPEC
							, A.LAST_DAY , A.DIFF_7DAY , A.A_DELV_QTY
							, CASE WHEN A.A_DELV_QTY IS NULL THEN '連續7天未有出貨記錄'
									WHEN A.A_DELV_QTY IS NOT NULL AND A.A_DELV_QTY < 30 THEN '累計出貨數量低於30pcs'
									WHEN A.ORDER_COUNT IS NOT NULL AND A.A_DELV_QTY <= 3 THEN '累計出貨次數小於等於三次'
							  END MEMO
							, '移出黃金揀貨區' MEMO1
							, A.ORDER_COUNT
						FROM (
								SELECT
									 A.DC_CODE , DC.DC_NAME ,A.GUP_CODE , GUP.GUP_NAME ,A.CUST_CODE , CUST.CUST_NAME
									, A.ITEM_CODE ,A.LOC_CODE , A.VALID_DATE ,A.ENTER_DATE ,A.SERIAL_NO ,A.QTY
									, B.AREA_CODE , B.WAREHOUSE_ID
									, E.ITEM_NAME , E.ITEM_COLOR , E.ITEM_SIZE , E.ITEM_SPEC
									, F.LAST_DAY , F.DIFF_7DAY , F.A_DELV_QTY
									, G.ORDER_COUNT
								FROM F1913 A
								JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE  AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
								JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('B')
								JOIN F1980 D ON D.DC_CODE= A.DC_CODE AND D.WAREHOUSE_ID = B.WAREHOUSE_ID AND D.WAREHOUSE_TYPE ='G'
								JOIN F1901 DC ON DC.DC_CODE =A.DC_CODE
                                JOIN F1929 GUP ON GUP.GUP_CODE = A.GUP_CODE
                                JOIN F1909 CUST ON CUST.GUP_CODE = A.GUP_CODE AND CUST.CUST_CODE = A.CUST_CODE
								LEFT JOIN F1903 E ON E.ITEM_CODE =A.ITEM_CODE AND E.GUP_CODE = A.GUP_CODE AND E.CUST_CODE = A.CUST_CODE 
								LEFT JOIN
										(
										--過去7天以來，累計出貨數量低於30pcs。
										SELECT
												A.DC_CODE , A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE
												, MAX(A.DELV_DATE) LAST_DAY
                                                , DATEDIFF(DAY,MAX(A.DELV_DATE),@p2) DIFF_7DAY
												, SUM(A.A_DELV_QTY) A_DELV_QTY ,
												'符合過去7天以來，累計出貨數量大於30pcs' MEMO
										FROM
										(
											SELECT
													A.WMS_ORD_NO , A.DC_CODE , A.GUP_CODE , A.CUST_CODE , B.ITEM_CODE , C.PICK_LOC
													, E.AREA_CODE , D.WAREHOUSE_ID  , B.A_DELV_QTY , A.DELV_DATE
													, DATEDIFF(DAY, A.DELV_DATE,@p2) DIFF_7DAY
											FROM F050801 A
											JOIN F050802 B ON A.DC_CODE=B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.WMS_ORD_NO =B.WMS_ORD_NO
											JOIN F051202 C ON C.WMS_ORD_NO = A.WMS_ORD_NO AND A.DC_CODE = C.DC_CODE AND A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
											JOIN F1912 D ON D.DC_CODE = A.DC_CODE AND D.GUP_CODE = A.GUP_CODE AND D.CUST_CODE = A.CUST_CODE AND D.LOC_CODE = C.PICK_LOC
											JOIN F1919 E ON E.AREA_CODE =D.AREA_CODE AND E.WAREHOUSE_ID = D.WAREHOUSE_ID AND E.DC_CODE =A.DC_CODE AND E.ATYPE_CODE IN ('B')  --儲位類型
											JOIN F1980 F ON F.DC_CODE= A.DC_CODE AND F.WAREHOUSE_ID = D.WAREHOUSE_ID AND F.WAREHOUSE_TYPE ='G'
											WHERE  A.STATUS in (5,6)
											AND DATEDIFF(DAY,A.DELV_DATE,@p2) <= @p0 --過去幾天 參數 (7 OR 14)
										)  A
											GROUP BY A.DC_CODE , A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE
								) F ON F.DC_CODE = A.DC_CODE AND F.GUP_CODE = A.GUP_CODE AND F.CUST_CODE = A.CUST_CODE AND F.ITEM_CODE  = A.ITEM_CODE
								LEFT JOIN
								(
									-- 符合	商品已在黃金揀貨區，過去14天以來，累計出貨次數小於等於三次。
									SELECT
											A.DC_CODE , A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE
											,COUNT(A.DC_CODE) ORDER_COUNT
									FROM
									(
										SELECT
											A.WMS_ORD_NO , A.DC_CODE , A.GUP_CODE , A.CUST_CODE , B.ITEM_CODE , C.PICK_LOC , E.AREA_CODE
											, D.WAREHOUSE_ID , B.A_DELV_QTY , A.DELV_DATE
										FROM F050801 A
										JOIN F050802 B ON A.DC_CODE=B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.WMS_ORD_NO =B.WMS_ORD_NO
										JOIN F051202 C ON C.WMS_ORD_NO = A.WMS_ORD_NO AND A.DC_CODE = C.DC_CODE AND A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
										JOIN F1912 D ON D.DC_CODE = A.DC_CODE AND D.GUP_CODE = A.GUP_CODE AND D.CUST_CODE = A.CUST_CODE AND D.LOC_CODE = C.PICK_LOC
										JOIN F1919 E ON E.AREA_CODE =D.AREA_CODE AND E.WAREHOUSE_ID = D.WAREHOUSE_ID AND E.DC_CODE =A.DC_CODE AND E.ATYPE_CODE IN ('B')  --儲位類型
										JOIN F1980 F ON F.DC_CODE= A.DC_CODE AND F.WAREHOUSE_ID = D.WAREHOUSE_ID AND F.WAREHOUSE_TYPE ='G'
										WHERE  A.STATUS in(5,6)
											   AND DATEDIFF(DAY,A.DELV_DATE,@p2) <= @p1   --過去幾天 參數 (7 OR 14)
									) A
									GROUP BY
											A.DC_CODE , A.GUP_CODE ,A.CUST_CODE , A.ITEM_CODE

								) G ON G.DC_CODE = A.DC_CODE AND G.GUP_CODE = A.GUP_CODE AND G.CUST_CODE = A.CUST_CODE AND G.ITEM_CODE  = A.ITEM_CODE

						) A
						GROUP BY
							A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE , A.AREA_CODE , A.WAREHOUSE_ID , A.ITEM_CODE
							, A.ITEM_NAME , A.ITEM_COLOR , A.ITEM_SIZE , A.ITEM_SPEC
							, A.LAST_DAY , A.DIFF_7DAY , A.A_DELV_QTY
							, A.ORDER_COUNT , A.DC_NAME , A.GUP_NAME , A.CUST_NAME

					";

			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", checkDay1));
			sqlParamers.Add(new SqlParameter("@p1", checkDay2));
      sqlParamers.Add(new SqlParameter("@p2", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var result = SqlQuery<SchF700501Data>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		/// <summary>
		/// 取出貨單(一般儲位) 相關資料
		/// </summary>
		/// <param name="baseDay">取過去幾天日期參數</param>
		public IQueryable<SchF700501Data> GetSchOrderNormalData(int baseDay = 90)
		{
			var sql = @"

					SELECT  A.DC_CODE , A.DC_NAME ,A.GUP_CODE , A.GUP_NAME ,A.CUST_CODE , A.CUST_NAME
                            , A.ITEM_CODE ,A.ITEM_NAME , A.ITEM_COLOR , A.ITEM_SIZE , A.ITEM_SPEC
                            , B.DELV_DATE , '移入黃金揀貨區' MEMO1
                    FROM
                    (
                        SELECT
                                A.DC_CODE , A.DC_NAME ,A.GUP_CODE , A.GUP_NAME ,A.CUST_CODE , A.CUST_NAME , A.ITEM_CODE
                                ,A.ITEM_NAME , A.ITEM_COLOR , A.ITEM_SIZE , A.ITEM_SPEC
                        FROM
                        (
                            -- 所有一搬儲位-商品資訊 (排除掉有在黃金揀貨區的商品)
                            SELECT
                                 A.DC_CODE , DC.DC_NAME ,A.GUP_CODE , GUP.GUP_NAME ,A.CUST_CODE , CUST.CUST_NAME
                                , A.ITEM_CODE ,A.LOC_CODE , A.VALID_DATE ,A.ENTER_DATE ,A.SERIAL_NO ,A.QTY
                                , B.AREA_CODE  , B.WAREHOUSE_ID
                                , E.ITEM_NAME , E.ITEM_COLOR , E.ITEM_SIZE , E.ITEM_SPEC
                            FROM F1913 A
                            JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE  AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
                            JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('A','B')
                            JOIN F1980 D ON D.DC_CODE= A.DC_CODE AND D.WAREHOUSE_ID = B.WAREHOUSE_ID AND D.WAREHOUSE_TYPE ='G'
                            JOIN F1901 DC ON DC.DC_CODE =A.DC_CODE
                            JOIN F1929 GUP ON GUP.GUP_CODE = A.GUP_CODE
                            JOIN F1909 CUST ON CUST.GUP_CODE = A.GUP_CODE AND CUST.CUST_CODE = A.CUST_CODE
                            LEFT JOIN F1903 E ON E.ITEM_CODE =A.ITEM_CODE AND E.GUP_CODE = A.GUP_CODE AND E.CUST_CODE = A.CUST_CODE
                            WHERE A.ITEM_CODE NOT IN (
                                                        -- 排除有在黃金儲位
                                                        SELECT  ITEM_CODE
                                                                FROM F1913 A1
                                                                JOIN F1912 B1 ON A1.LOC_CODE =B1.LOC_CODE AND A1.DC_CODE =B1.DC_CODE  AND A1.GUP_CODE = B1.GUP_CODE AND A1.CUST_CODE =B1.CUST_CODE
                                                                JOIN F1919 C1 ON C1.AREA_CODE =B1.AREA_CODE AND C1.WAREHOUSE_ID = B1.WAREHOUSE_ID AND C1.DC_CODE =A1.DC_CODE AND C1.ATYPE_CODE IN ('B')
                                                                WHERE A1.ITEM_CODE =A.ITEM_CODE
                                                                GROUP BY A1.ITEM_CODE
                                                     )
                        ) A GROUP BY
                                    A.DC_CODE , A.DC_NAME ,A.GUP_CODE , A.GUP_NAME ,A.CUST_CODE , A.CUST_NAME  , A.ITEM_CODE
                                    ,A.ITEM_NAME , A.ITEM_COLOR , A.ITEM_SIZE , A.ITEM_SPEC
                    ) A
                    LEFT JOIN (
                                SELECT  A.DC_CODE , A.GUP_CODE , A.CUST_CODE , A.ITEM_CODE , A.DELV_DATE
                                FROM
                                (
                                    -- 一搬揀貨區出貨商品記錄
                                    SELECT
                                            A.WMS_ORD_NO , A.DC_CODE , A.GUP_CODE , A.CUST_CODE , B.ITEM_CODE , C.PICK_LOC
                                            , E.AREA_CODE , D.WAREHOUSE_ID  , B.A_DELV_QTY , A.DELV_DATE ,  CEILING(DATEDIFF(ss,A.DELV_DATE,@p1)/(24*60.0*60.0)) AVERAGE
                                    FROM F050801 A
                                    JOIN F050802 B ON A.DC_CODE=B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.WMS_ORD_NO =B.WMS_ORD_NO
                                    JOIN F051202 C ON C.WMS_ORD_NO = A.WMS_ORD_NO AND A.DC_CODE = C.DC_CODE AND A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
                                    JOIN F1912 D ON D.DC_CODE = A.DC_CODE AND D.GUP_CODE = A.GUP_CODE AND D.CUST_CODE = A.CUST_CODE AND D.LOC_CODE = C.PICK_LOC
                                    JOIN F1919 E ON E.AREA_CODE =D.AREA_CODE AND E.WAREHOUSE_ID = D.WAREHOUSE_ID AND E.DC_CODE =A.DC_CODE AND E.ATYPE_CODE IN ('A')  --儲位類型
                                    JOIN F1980 F ON F.DC_CODE= A.DC_CODE AND F.WAREHOUSE_ID = D.WAREHOUSE_ID AND F.WAREHOUSE_TYPE ='G'
                                    WHERE  A.STATUS in (5,6)
                                    AND CEILING(DATEDIFF(ss,A.DELV_DATE,@p1)/(24*60.0*60.0)) <= @p0  --過去x個月以來，平均出貨間隔天數<=3天  ex:三個月 = 90
                                ) A
                                GROUP BY  A.DC_CODE , A.GUP_CODE , A.CUST_CODE , A.ITEM_CODE , A.DELV_DATE
                            ) B ON A.DC_CODE =B.DC_CODE AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
                                    AND A.ITEM_CODE = B.ITEM_CODE
                    WHERE B.ITEM_CODE IS NOT NULL
                    ORDER BY A.ITEM_CODE, B.DELV_DATE

					";

			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", baseDay));
      sqlParamers.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var result = SqlQuery<SchF700501Data>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		/// <summary>
		/// 取出貨單(不分) 相關資料
		/// </summary>
		/// <param name="baseDay">取過去幾天日期參數</param>
		public IQueryable<SchF700501Data> GetSchOrderAllData(int baseDay = 90)
		{
			var sql = @"
						SELECT	A.DC_CODE  ,A.GUP_CODE  ,A.CUST_CODE , DC.DC_NAME , GUP.GUP_NAME  , CUST.CUST_NAME , A.ITEM_CODE
								, B.WMS_ORD_NO ,  B.A_DELV_QTY , B.DELV_DATE
								, C.ITEM_NAME , C.ITEM_COLOR , C.ITEM_SIZE , C.ITEM_SPEC
								, CASE WHEN B.WMS_ORD_NO IS NULL THEN '商品連續90天無出貨記錄' END MEMO
						FROM
						(
							--所有儲位商品資料 (黃區/一般揀貨區)
							SELECT
								 A.DC_CODE  ,A.GUP_CODE  ,A.CUST_CODE , A.ITEM_CODE
							FROM F1913 A
							JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE  AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE =B.CUST_CODE
							JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('A','B')
							JOIN F1980 D ON D.DC_CODE= A.DC_CODE AND D.WAREHOUSE_ID = B.WAREHOUSE_ID AND D.WAREHOUSE_TYPE ='G'
							GROUP BY  A.DC_CODE  ,A.GUP_CODE  ,A.CUST_CODE , A.ITEM_CODE
						) A
						LEFT JOIN
							(
								-- 所有出貨資料 (黃區/一般揀貨區)
								SELECT
										A.WMS_ORD_NO , A.DC_CODE , A.GUP_CODE , A.CUST_CODE , B.ITEM_CODE , C.PICK_LOC
										, B.A_DELV_QTY , A.DELV_DATE

								FROM F050801 A
								JOIN F050802 B ON A.DC_CODE=B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.WMS_ORD_NO =B.WMS_ORD_NO
								JOIN F051202 C ON C.WMS_ORD_NO = A.WMS_ORD_NO AND A.DC_CODE = C.DC_CODE AND A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE AND C.ITEM_CODE = B.ITEM_CODE
								JOIN F1912 D ON D.DC_CODE = A.DC_CODE AND D.GUP_CODE = A.GUP_CODE AND D.CUST_CODE = A.CUST_CODE AND D.LOC_CODE = C.PICK_LOC
								JOIN F1919 E ON E.AREA_CODE =D.AREA_CODE AND E.WAREHOUSE_ID = D.WAREHOUSE_ID AND E.DC_CODE =A.DC_CODE AND E.ATYPE_CODE IN ('A','B')  --儲位類型
								JOIN F1980 F ON F.DC_CODE= A.DC_CODE AND F.WAREHOUSE_ID = D.WAREHOUSE_ID AND F.WAREHOUSE_TYPE ='G'
								WHERE  A.STATUS in (5,6)
								AND ceiling(datediff(ss,A.DELV_DATE,@p1)/(24*60.0*60.0)) -1 <= @p0 --過去幾天
						   ) B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE AND B.ITEM_CODE  = A.ITEM_CODE
						JOIN F1901 DC ON DC.DC_CODE =A.DC_CODE
						JOIN F1929 GUP ON GUP.GUP_CODE = A.GUP_CODE
						JOIN F1909 CUST ON CUST.GUP_CODE = A.GUP_CODE AND CUST.CUST_CODE = A.CUST_CODE
						LEFT JOIN F1903 C ON C.ITEM_CODE =A.ITEM_CODE AND C.GUP_CODE = A.GUP_CODE AND C.CUST_CODE = A.CUST_CODE
						ORDER BY A.ITEM_CODE, B.DELV_DATE

					";

			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", baseDay));
      sqlParamers.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var result = SqlQuery<SchF700501Data>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		public void DeleteDataByItemZeroQty(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var sql =
					@" DELETE FROM F1913 WHERE DC_CODE = @p0 AND GUP_CODE =@p1 AND CUST_CODE =@p2 AND ITEM_CODE =@p3 AND QTY<=0 ";
			var param = new object[] { dcCode, gupCode, custCode, itemCode };
			ExecuteSqlCommand(sql, param);
		}

		/// <summary>
		/// 清除庫存本次調撥商品庫存數為0的資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="f1913"></param>
		public void DeleteDataByItemZeroQty(string dcCode, string gupCode, string custCode, string itemCode, F1913 f1913)
		{
			var sql =
					@" DELETE FROM F1913 
						WHERE DC_CODE = @p0 
						AND GUP_CODE =@p1 
						AND CUST_CODE =@p2 
						AND ITEM_CODE =@p3 
						AND QTY <= 0 
						AND 
						(LOC_CODE <> @p4
						AND ITEM_CODE <> @p5
						AND VALID_DATE <> @p6
						AND ENTER_DATE <> @p7
						AND MAKE_NO <> @p8
						AND DC_CODE <> @p9
						AND GUP_CODE <> @p10
						AND CUST_CODE <> @p11
						AND SERIAL_NO <> @p12
						AND VNR_CODE <> @p13
						AND BOX_CTRL_NO <> @p14
						AND PALLET_CTRL_NO <> @p15)
						";
			var param = new object[] {
				dcCode,
				gupCode,
				custCode,
				itemCode,
				f1913.LOC_CODE,
				f1913.ITEM_CODE,
				f1913.VALID_DATE.ToString("yyyy/MM/dd"),
				f1913.ENTER_DATE.ToString("yyyy/MM/dd"),
				f1913.MAKE_NO,
				f1913.DC_CODE,
				f1913.GUP_CODE,
				f1913.CUST_CODE,
				f1913.SERIAL_NO,
				f1913.VNR_CODE,
				f1913.BOX_CTRL_NO,
				f1913.PALLET_CTRL_NO
			};
			ExecuteSqlCommand(sql, param);
		}

		/// <summary>
		/// 儲位合併執行表 簡易版本，主要針對是否可混商品與找最大數量的相同品項，且為相同倉別來當作合併後儲位號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="qty"></param>
		/// <returns></returns>
		public IQueryable<P710705MergeExecution> GetP710705LocMergeExecution(string dcCode, int? qty)
		{
			var sql = @"SELECT ROW_NUMBER ()over(order by T.GUP_CODE, T.CUST_CODE, T.ITEM_CODE, T.LOC_CODE) ROWNUM,
							   T.DC_CODE,
							   T.GUP_CODE,
							   T.CUST_CODE,
							   T.LOC_CODE,
							   T.ITEM_CODE,
							   C.ITEM_NAME,
							   T.TOTAL_QTY,
							   --B.WAREHOUSE_ID,
							   H.CUST_NAME,
							   I.GUP_NAME,
							   (SELECT top(1) G.LOC_CODE
								  FROM (  SELECT top 100 percent D.DC_CODE,
												 D.GUP_CODE,
												 D.CUST_CODE,
												 D.LOC_CODE,
												 D.ITEM_CODE,
												 B.WAREHOUSE_ID
											FROM F1913 D
												 JOIN F1912 B
													ON     D.DC_CODE = B.DC_CODE
													   AND D.LOC_CODE = B.LOC_CODE
												 JOIN F1903 E
													ON     D.GUP_CODE = E.GUP_CODE
													   AND D.CUST_CODE = E.CUST_CODE
													   AND D.ITEM_CODE = E.ITEM_CODE
													   AND (   E.LOC_MIX_ITEM = '1'
															OR NOT EXISTS
																	  (SELECT 1
																		 FROM F1913 F
																		WHERE     D.DC_CODE = F.DC_CODE
																			  AND D.LOC_CODE =
																					 F.LOC_CODE
																			  AND D.ITEM_CODE <>
																					 F.ITEM_CODE))
										GROUP BY D.DC_CODE,
												 D.GUP_CODE,
												 D.CUST_CODE,
												 D.LOC_CODE,
												 D.ITEM_CODE,
												 B.WAREHOUSE_ID
										ORDER BY SUM (QTY) DESC) G
								 WHERE     G.ITEM_CODE = T.ITEM_CODE
									   AND G.DC_CODE = T.DC_CODE
									   AND G.GUP_CODE = T.GUP_CODE
									   AND G.CUST_CODE = T.CUST_CODE
									   AND G.WAREHOUSE_ID = B.WAREHOUSE_ID)
								  TAR_LOC_CODE
						  FROM (  SELECT A.DC_CODE,
										 A.GUP_CODE,
										 A.CUST_CODE,
										 A.LOC_CODE,
										 A.ITEM_CODE,
										 SUM (A.QTY) TOTAL_QTY
									FROM F1913 A
								   WHERE DC_CODE = @p0
								GROUP BY DC_CODE,
										 GUP_CODE,
										 CUST_CODE,
										 LOC_CODE,
										 ITEM_CODE) T
							   JOIN F1912 B ON T.DC_CODE = B.DC_CODE AND T.LOC_CODE = B.LOC_CODE
							   JOIN F1903 C ON T.GUP_CODE = C.GUP_CODE AND T.ITEM_CODE = C.ITEM_CODE AND T.CUST_CODE = C.CUST_CODE
							   JOIN F1909 H ON T.GUP_CODE = H.GUP_CODE AND T.CUST_CODE = H.CUST_CODE
							   JOIN F1929 I ON T.GUP_CODE = I.GUP_CODE
							WHERE T.TOTAL_QTY > 0";

			var paramList = new List<object> { dcCode };
			if (qty.HasValue)
			{
				sql += paramList.Combine(" AND T.TOTAL_QTY < @p{0} ", qty);
			}

			sql += " ORDER BY T.GUP_CODE, T.CUST_CODE, T.ITEM_CODE, T.LOC_CODE";

			var result = SqlQuery<P710705MergeExecution>(sql, paramList.ToArray());
			return result;
		}

		public void UpdateF1913ValidDate(List<string> listSerialNo, DateTime validDate, string gupCode, string custCode,
				string userId, string userName)
		{
			var parameters = new List<SqlParameter>
												{
								new SqlParameter("@p0", validDate),
								new SqlParameter("@p1", userId),
								new SqlParameter("@p2", userName),
								new SqlParameter("@p3", gupCode),
								new SqlParameter("@p4", custCode),
                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sql = @"
UPDATE F1913
   SET VALID_DATE = @p0,
       UPD_STAFF = @p1,
       UPD_NAME = @p2,
       UPD_DATE = @p5
 WHERE GUP_CODE = @p3 AND CUST_CODE = @p4
  AND SERIAL_NO IN ({0})
";

			var serialNos = string.Format("'{0}'", string.Join("','", listSerialNo));
			ExecuteSqlCommand(string.Format(sql, serialNos), parameters.ToArray());
		}

        public void UpdateF1913SerialNo(string oldSerialNo, string newSerialNo, string gupCode, string custCode,
            string userId, string userName)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", newSerialNo),
                new SqlParameter("@p1", string.IsNullOrWhiteSpace(userId) ? (object)DBNull.Value : userId ),
                new SqlParameter("@p2", string.IsNullOrWhiteSpace(userName) ? (object)DBNull.Value : userName),
                new SqlParameter("@p3", gupCode),
                new SqlParameter("@p4", custCode),
                new SqlParameter("@p5", oldSerialNo),
                new SqlParameter("@p6", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sql = @"
UPDATE F1913
   SET SERIAL_NO = @p0,
       UPD_STAFF = @p1,
       UPD_NAME = @p2,
       UPD_DATE = @p6
 WHERE GUP_CODE = @p3 AND CUST_CODE = @p4
  AND SERIAL_NO = @p5
";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
		{
			var parameter = new List<SqlParameter>
															{
																			new SqlParameter("@p0", dcCode),
																			new SqlParameter("@p1", gupCode),
																			new SqlParameter("@p2", custCode),
																			new SqlParameter("@p3", settleDate)
															};
			var sql = @"SELECT  ROW_NUMBER ()OVER(ORDER BY TB.ITEM_CODE,TB.DC_CODE,TB.GUP_CODE,TB.CUST_CODE ) ROWNUM,
                    TB.* FROM (
             SELECT @p3 CAL_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
                 A.ITEM_CODE,SUM (A.QTY) QTY,'01' DELV_ACC_TYPE
              FROM F1913 A
              WHERE (A.DC_CODE = @p0 OR @p0 = '000') AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
               AND A.VALID_DATE <> CONVERT(datetime,'9999/12/31')
            GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE) TB";
			var result = SqlQuery<SettleData>(sql, parameter.ToArray()).ToList();
			return result.AsQueryable();
		}

		public IQueryable<F160502Data> GetF1913ScrapData(string dcCode, string gupCode, string custCode)
		{
			var param = new object[] { dcCode, gupCode, custCode };
			var sql = @"

						SELECT DISTINCT
							A.DC_CODE , A.GUP_CODE , A.CUST_CODE
							, B.LOC_CODE , A.SERIAL_NO  ITEM_SERIALNO
							, D.ITEM_CODE , D.ITEM_NAME , D.ITEM_COLOR , D.ITEM_SIZE , D.ITEM_SPEC
							, CASE WHEN D.VIRTUAL_TYPE IS NULL OR D.VIRTUAL_TYPE ='' THEN '否' ELSE '是' END VIRTUAL_TYPE
							, E.BUNDLE_SERIALNO
							,  A.QTY SCRAP_QTY ,  A.QTY DESTROY_QTY
						FROM F1913 A
						JOIN F1912 B ON B.DC_CODE = A.DC_CODE AND B.LOC_CODE = A.LOC_CODE
						JOIN F1980 C ON C.DC_CODE = A.DC_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.WAREHOUSE_TYPE ='D'
						JOIN F1903 D ON D.GUP_CODE = A.GUP_CODE AND D.ITEM_CODE = A.ITEM_CODE AND D.ITEM_CODE = A.ITEM_CODE
						JOIN F1903 E ON E.GUP_CODE = A.GUP_CODE AND E.CUST_CODE = A.CUST_CODE AND E.CUST_CODE = A.CUST_CODE
						WHERE  NOT EXISTS (
											SELECT D1.ITEM_CODE , D1.SERIAL_NO FROM F160504 D1
																				WHERE D1.ITEM_CODE = A.ITEM_CODE AND D1.SERIAL_NO = A.SERIAL_NO
																						AND D1.DC_CODE = A.DC_CODE AND D1.GUP_CODE = A.GUP_CODE
																						AND D1.CUST_CODE = A.CUST_CODE
										  )
						AND A.QTY > 0
						AND A.DC_CODE =@p0
						AND A.GUP_CODE =@p1
						AND A.CUST_CODE =@p2
						AND NOT (
									(D.VIRTUAL_TYPE <> '' OR D.VIRTUAL_TYPE IS NOT NULL) AND E.BUNDLE_SERIALNO = '1'
										AND (A.SERIAL_NO ='0' OR A.SERIAL_NO IS NULL )
								)
						";
			return SqlQuery<F160502Data>(sql, param).AsQueryable();
		}


		/// <summary>
		/// 取得虛擬儲位F1511可配庫的某商品總庫存數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="typeId"></param>
		/// <returns></returns>
		public IQueryable<ItemQty> GetVirtualQtyItems(string dcCode, string gupCode, string custCode, string typeId, IEnumerable<string> itemCodes)
		{
			var param = new List<object>
												{
																dcCode,
																gupCode,
																custCode,
																typeId
												};

			var sql = @"  SELECT B.ITEM_CODE AS ItemCode, Case When B.MAKE_NO='' Then null Else B.MAKE_NO End AS MakeNo,Case When B.SERIAL_NO='' Then null Else B.SERIAL_NO End AS SerialNo, ISNULL (SUM (B.B_PICK_QTY), 0) AS Qty
							FROM F1511 A
								 JOIN F051202 B
									ON     A.ORDER_NO = B.PICK_ORD_NO
									   AND A.ORDER_SEQ = B.PICK_ORD_SEQ
									   AND A.GUP_CODE = B.GUP_CODE
									   AND A.CUST_CODE = B.CUST_CODE
									   AND A.DC_CODE = B.DC_CODE
								 JOIN
								 (  SELECT C.WMS_ORD_NO,
										   C.GUP_CODE,
										   C.CUST_CODE,
										   C.DC_CODE,
										   E.TYPE_ID
									  FROM F050801 C
										   JOIN F05030101 D
											  ON     C.WMS_ORD_NO = D.WMS_ORD_NO
												 AND C.GUP_CODE = D.GUP_CODE
												 AND C.CUST_CODE = D.CUST_CODE
												 AND C.DC_CODE = D.DC_CODE
										   JOIN F050301 E
											  ON     D.ORD_NO = E.ORD_NO
												 AND D.GUP_CODE = E.GUP_CODE
												 AND D.CUST_CODE = E.CUST_CODE
												 AND D.DC_CODE = E.DC_CODE
									 WHERE E.PROC_FLAG = '9'
								  GROUP BY C.WMS_ORD_NO,
										   C.GUP_CODE,
										   C.CUST_CODE,
										   C.DC_CODE,
										   E.TYPE_ID) F
									ON     B.WMS_ORD_NO = F.WMS_ORD_NO
									   AND B.GUP_CODE = F.GUP_CODE
									   AND B.CUST_CODE = F.CUST_CODE
									   AND B.DC_CODE = F.DC_CODE
						   WHERE     A.STATUS <> '9'
								 AND A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND F.TYPE_ID = @p3"
								+ param.CombineSqlInParameters("AND B.ITEM_CODE", itemCodes)
								+ "GROUP BY B.ITEM_CODE, Case When B.MAKE_NO='' Then null Else B.MAKE_NO End,Case When B.SERIAL_NO='' Then null Else B.SERIAL_NO End";

			return SqlQuery<ItemQty>(sql, param.ToArray()).AsQueryable();
		}

		public IQueryable<SuggestLocItem> GetSuggestLocsByStock(string dcCode, string gupCode, string custCode, List<string> itemCodes)
		{
			var sql = @" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,C.WAREHOUSE_ID
										 FROM F1913 A
										INNER JOIN F1912 B
										   ON B.DC_CODE = A.DC_CODE
										  AND B.LOC_CODE=A.LOC_CODE
										INNER JOIN F1980 C
										   ON C.DC_CODE = B.DC_CODE
										  AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
										INNER JOIN F1919 D
										   ON D.DC_CODE = C.DC_CODE
										  AND D.WAREHOUSE_ID =  C.WAREHOUSE_ID
                    INNER JOIN F1903 E
                       ON E.GUP_CODE = A.GUP_CODE
                      AND E.CUST_CODE= A.CUST_CODE
                      AND E.ITEM_CODE = A.ITEM_CODE
										WHERE C.WAREHOUSE_TYPE='G' --良品倉
										  AND D.ATYPE_CODE ='A' -- 揀貨區
                      AND E.MIX_BATCHNO = '1' --允許混批(效期)擺放儲位
                      AND A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND B.NOW_STATUS_ID NOT IN('02','04') --排除儲位凍結進(02)和凍結進出(04)
                      ";
			var param = new List<object>() { dcCode, gupCode, custCode };
			sql += param.CombineSqlInParameters("AND A.ITEM_CODE ", itemCodes);
			sql += " ORDER BY A.LOC_CODE ";
			return SqlQuery<SuggestLocItem>(sql, param.ToArray()).AsQueryable();
		}

		#region 檢查目前庫存量

		public IQueryable<F1913WithF1912Qty> GetF1913WithF1912Qty(string dcCode, string gupCode, string custCode,
						 string itemCode, string dataTable)
		{
			var parameters = new List<SqlParameter>
												{
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", itemCode)
												};
			var sql = $@"
                            Select ROW_NUMBER()
                                     OVER(
                                       ORDER BY A.LOC_CODE) ROWNUM,
                                   B.WAREHOUSE_ID,
                                   A.LOC_CODE,
                                   A.ITEM_CODE,
                                   D.ITEM_NAME,
                                   A.QTY
                            From   (SELECT A.DC_CODE,
                                           A.GUP_CODE,
                                           A.CUST_CODE,
                                           A.ITEM_CODE,
                                           A.LOC_CODE,
                                           Sum(A.QTY) QTY
                                    FROM   F1913 A
                                    GROUP  BY A.DC_CODE,
                                              A.GUP_CODE,
                                              A.CUST_CODE,
                                              A.ITEM_CODE,
                                              A.LOC_CODE) A
                                   Join F1912 B
                                     On A.DC_CODE = B.DC_CODE
                                        And A.LOC_CODE = B.LOC_CODE
                                   Left Join F1903 D
                                          On A.GUP_CODE = D.GUP_CODE
                                             And A.ITEM_CODE = D.ITEM_CODE
                                             AND A.CUST_CODE = D.CUST_CODE
                            WHERE  A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND B.WAREHOUSE_ID NOT LIKE '%N%' --不等於不良品倉
                               AND B.NOW_STATUS_ID <> '04'
                               AND B.NOW_STATUS_ID <> '03'
                               AND A.ITEM_CODE = @p3
                            ";
			return SqlQuery<F1913WithF1912Qty>(sql, parameters.ToArray()).AsQueryable();
		}
		#endregion


		public IQueryable<StockInfo> GetStockInfies(string dcCode, string gupCode, string custCode, string itemCode,
		string locCode = null, string warehouseType = null, string warehouseId = null, string aTypeCode = null,
		bool isForIn = false, List<string> serialNos = null, List<DateTime> validDates = null,
		List<DateTime> enterDates = null, List<string> vnrCodes = null, List<string> boxCtrlNos = null,
		List<string> palletCtrlNos = null, List<string> makeNos = null, bool isAllowExpiredItem = false)
		{
			var filterSql = string.Empty;
			var param = new List<object> {
								dcCode,
								gupCode,
								custCode,
								itemCode
						};
			filterSql += param.CombineNotNullOrEmpty(" AND A.LOC_CODE = @p{0}", locCode);
			filterSql += param.CombineNotNullOrEmpty(" AND C.WAREHOUSE_ID = @p{0}", warehouseId);
			filterSql += param.CombineNotNullOrEmpty(" AND C.WAREHOUSE_TYPE = @p{0}", warehouseType);
			filterSql += param.CombineNotNullOrEmpty(" AND D.ATYPE_CODE = @p{0}", aTypeCode);

			if (isForIn)
				filterSql += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				filterSql += @"
				   And B.NOW_STATUS_ID<>'03'";

			if (serialNos != null && serialNos.Any())
				filterSql += param.CombineSqlInParameters(" AND A.SERIAL_NO ", serialNos);

			if (validDates == null || !validDates.Any())
			{
				//是否允許過期商品調撥
				if (!isAllowExpiredItem)
          filterSql += param.CombineNotNullOrEmpty(" AND A.VALID_DATE> @p{0} ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
      }

      else
				filterSql += param.CombineSqlInParameters(" AND A.VALID_DATE ", validDates);

			if (enterDates != null && enterDates.Any())
				filterSql += param.CombineSqlInParameters(" AND A.ENTER_DATE ", enterDates);
			if (vnrCodes != null && vnrCodes.Any())
				filterSql += param.CombineSqlInParameters(" AND A.VNR_CODE ", vnrCodes);
			if (boxCtrlNos != null && boxCtrlNos.Any())
				filterSql += param.CombineSqlInParameters(" AND A.BOX_CTRL_NO ", boxCtrlNos);
			if (palletCtrlNos != null && palletCtrlNos.Any())
				filterSql += param.CombineSqlInParameters(" AND A.PALLET_CTRL_NO ", palletCtrlNos);
			if (makeNos != null && makeNos.Any())
				filterSql += param.CombineSqlInParameters(" AND A.MAKE_NO ", makeNos);

			var sql = $@" SELECT ROW_NUMBER ()OVER (ORDER BY A.VALID_DATE ) ROWNUM,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.VNR_CODE,A.SERIAL_NO,A.QTY,B.WAREHOUSE_ID,
														CASE WHEN F.CASE_NO IS NULL THEN E.CASE_NO ELSE '' END AS CASE_NO,
														CASE WHEN G.BOX_SERIAL IS NULL THEN E.BOX_SERIAL ELSE '' END AS BOX_SERIAL,
														CASE WHEN H.BATCH_NO IS NULL THEN E.BATCH_NO ELSE '' END AS BATCH_NO,
														A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO
											FROM F1913 A
											INNER JOIN F1912 B
											ON B.DC_CODE = A.DC_CODE
											AND B.LOC_CODE = A.LOC_CODE
											INNER JOIN F1980 C
											ON C.DC_CODE = B.DC_CODE
											AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
                      LEFT JOIN F1919 D
												ON D.WAREHOUSE_ID = B.WAREHOUSE_ID
											 And D.DC_CODE=B.DC_CODE
											 And D.AREA_CODE=B.AREA_CODE
											LEFT JOIN F2501 E
											  ON E.GUP_CODE = A.GUP_CODE
											 AND E.CUST_CODE = A.CUST_CODE
											 AND E.SERIAL_NO = A.SERIAL_NO
											LEFT JOIN (SELECT A.GUP_CODE,A.CUST_CODE,A.CASE_NO, Count(B.SERIAL_NO) Cnt 
																	 FROM F2501 A
														 JOIN F1913 B 
															 ON A.GUP_CODE = B.GUP_CODE 
															AND A.CUST_CODE = B.CUST_CODE 
															AND A.SERIAL_NO=B.SERIAL_NO
														WHERE B.QTY = 0 AND A.CASE_NO IS NOT NULL
														GROUP BY A.GUP_CODE,A.CUST_CODE,A.CASE_NO) F
											 ON F.GUP_CODE = E.GUP_CODE
											AND F.CUST_CODE = E.CUST_CODE
											AND F.CASE_NO = E.CASE_NO
											LEFT JOIN (SELECT A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL, Count(B.SERIAL_NO) Cnt 
																	 FROM F2501 A
														 JOIN F1913 B 
															 ON A.GUP_CODE = B.GUP_CODE 
															AND A.CUST_CODE = B.CUST_CODE 
															AND A.SERIAL_NO=B.SERIAL_NO
														WHERE B.QTY = 0 AND A.BOX_SERIAL IS NOT NULL
														GROUP BY A.GUP_CODE,A.CUST_CODE,A.BOX_SERIAL) G
											 ON G.GUP_CODE = E.GUP_CODE
											AND G.CUST_CODE = E.CUST_CODE
											AND G.BOX_SERIAL = E.BOX_SERIAL
											LEFT JOIN (SELECT A.GUP_CODE,A.CUST_CODE,A.BATCH_NO, Count(B.SERIAL_NO) Cnt 
																	 FROM F2501 A
														 JOIN F1913 B 
															 ON A.GUP_CODE = B.GUP_CODE 
															AND A.CUST_CODE = B.CUST_CODE 
															AND A.SERIAL_NO=B.SERIAL_NO
														WHERE B.QTY = 0 AND A.BATCH_NO IS NOT NULL
														GROUP BY A.GUP_CODE,A.CUST_CODE,A.BATCH_NO) H
											 ON H.GUP_CODE = E.GUP_CODE
											AND H.CUST_CODE = E.CUST_CODE
											AND H.BATCH_NO = E.BATCH_NO
											WHERE A.DC_CODE = @p0
											AND A.GUP_CODE = @p1
											AND A.CUST_CODE = @p2
											AND A.ITEM_CODE = @p3
											And B.NOW_STATUS_ID<>'04'
                      {filterSql}
											ORDER BY A.VALID_DATE ";
			return SqlQuery<StockInfo>(sql, param.ToArray()).AsQueryable();
		}

		public void UpdateF1913ValidDateAndMakeNo(F1913 f1913s, DateTime newValidDate, string makeNo,Int64 NewQty)
		{
			var parameters = new List<SqlParameter>
												{
																new SqlParameter("@p0", newValidDate.ToString("yyyy/MM/dd")),
																new SqlParameter("@p1", makeNo),
																new SqlParameter("@p2", Current.Staff),
																new SqlParameter("@p3", Current.StaffName),
																new SqlParameter("@p4", f1913s.LOC_CODE),
																new SqlParameter("@p5", f1913s.ITEM_CODE),
																new SqlParameter("@p6", f1913s.DC_CODE),
																new SqlParameter("@p7",f1913s.GUP_CODE),
																new SqlParameter("@p8",f1913s.CUST_CODE),
																new SqlParameter("@p9",string.IsNullOrEmpty(f1913s.BOX_CTRL_NO) ? "0" :f1913s.BOX_CTRL_NO),
																new SqlParameter("@p10",string.IsNullOrEmpty(f1913s.PALLET_CTRL_NO) ? "0" :f1913s.PALLET_CTRL_NO),
																new SqlParameter("@p11",f1913s.ENTER_DATE.ToString("yyyy/MM/dd")),
																new SqlParameter("@p12",f1913s.VALID_DATE.ToString("yyyy/MM/dd")),
																new SqlParameter("@p13",string.IsNullOrEmpty(f1913s.SERIAL_NO) ? "0" :f1913s.SERIAL_NO),
																new SqlParameter("@p14",string.IsNullOrEmpty(f1913s.MAKE_NO) ? "0" :f1913s.MAKE_NO),
                                new SqlParameter("@p15",NewQty),
                                new SqlParameter("@p16", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                        };

			var sql = " UPDATE F1913 " +
                      " SET VALID_DATE  = CONVERT(datetime, @p0 ),MAKE_NO= @p1, QTY=@p15,UPD_DATE = @p16,UPD_STAFF = @p2, UPD_NAME = @p3 " +
											" WHERE LOC_CODE = @p4 AND ITEM_CODE = @p5 " +
											" AND DC_CODE = @p6 AND GUP_CODE =@p7 " +
											" AND CUST_CODE = @p8 AND BOX_CTRL_NO = @p9 " +
											" AND PALLET_CTRL_NO =@p10 AND ENTER_DATE = CONVERT(datetime,@p11)  " +
											" AND VALID_DATE = CONVERT(datetime, @p12) AND SERIAL_NO = @p13  AND MAKE_NO = @p14 ";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<P060202PickStock> GetStocks(string dcCode, string gupCode, string custCode, List<string> locCodes, List<string> itemCodes)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.WAREHOUSE_ID,A.LOC_CODE,A.ITEM_CODE,SUM(A.QTY) QTY
										 FROM F1913 A
                     JOIN F1912 B
                       ON B.DC_CODE = A.DC_CODE 
                      AND B.LOC_CODE = A.LOC_CODE
										WHERE A.DC_CODE = @p0
										  AND A.GUP_CODE = @p1
										  AND A.CUST_CODE = @p2 
                      AND A.QTY >0
                      ";
			sql += parms.CombineSqlInParameters(" AND A.LOC_CODE", locCodes);
			sql += parms.CombineSqlInParameters(" AND A.ITEM_CODE", itemCodes);
			sql += "  GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.WAREHOUSE_ID,A.LOC_CODE,A.ITEM_CODE ";
			return SqlQuery<P060202PickStock>(sql, parms.ToArray());
		}

		public IQueryable<P081301StockSumQty> GetP081301StockSumQties(string dcCode, string gupCode, string custCode, string scanItemOrLocCode)
		{
			var sqlFilter = string.Empty;
			var parms = new List<SqlParameter>();
			parms.Add(new SqlParameter("@p0", dcCode));
			parms.Add(new SqlParameter("@p1", gupCode));
			parms.Add(new SqlParameter("@p2", custCode));
			parms.Add(new SqlParameter("@p3", scanItemOrLocCode.Replace("-","").ToUpper()));
			parms.Add(new SqlParameter("@p4", scanItemOrLocCode.ToUpper()));

			for(var i=0;i<parms.Count;i++)
			{
				parms[i].SqlDbType = SqlDbType.VarChar;
			}

			sqlFilter += " AND a.DC_CODE = @p0 ";
			sqlFilter += " AND a.GUP_CODE = @p1";
			sqlFilter += " AND a.CUST_CODE =@p2";
      sqlFilter += " AND (a.LOC_CODE = @p3 OR a.ITEM_CODE = @p4 OR b.EAN_CODE1 = @p4 OR b.EAN_CODE2 = @p4 OR b.EAN_CODE3 = @p4";
      sqlFilter += " OR (b.BUNDLE_SERIALLOC ='1' AND a.SERIAL_NO =  @p4 )";
      sqlFilter += " OR (b.BUNDLE_SERIALLOC ='0' AND EXISTS (SELECT 1 FROM F2501 WHERE GUP_CODE = a.GUP_CODE AND CUST_CODE = a.CUST_CODE AND ITEM_CODE = a.ITEM_CODE AND SERIAL_NO = @p4)))";

			var sql = $@"SELECT a.LOC_CODE,a.ITEM_CODE,b.ITEM_NAME,SUM(a.QTY) QTY,a.SERIAL_NO
                     FROM F1913 a
                     JOIN F1903 b
                       ON b.GUP_CODE = a.GUP_CODE 
                      AND b.CUST_CODE = a.CUST_CODE
                      AND b.ITEM_CODE = a.ITEM_CODE
											LEFT JOIN F1912 d 
																on a.DC_CODE  = d.DC_CODE 
																AND a.LOC_CODE  = d.LOC_CODE 
																LEFT  JOIN F1980 e 
																on d.DC_CODE  = e.DC_CODE 
																AND d.WAREHOUSE_ID  = e.WAREHOUSE_ID 
															 WHERE 1 = 1
																 AND a.QTY > 0
											
											{sqlFilter}
											GROUP BY a.LOC_CODE,a.ITEM_CODE,b.ITEM_NAME,a.SERIAL_NO";

			var result = SqlQuery<P081301StockSumQty>(sql, parms.ToArray());
      var resultWithSerial = result.Where(o => o.SERIAL_NO == scanItemOrLocCode);

      if (resultWithSerial.Any())
        return resultWithSerial;

      return result;
		}

		public IQueryable<P08130101Stock> GetP08130101Stocks(string dcCode, string gupCode, string custCode, string locCode, string itemCode)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, locCode };

			var filterSql = string.Empty;
			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				filterSql += "AND (A.ITEM_CODE = @p" + parms.Count + " OR B.EAN_CODE1 = @p" + parms.Count + " OR B.EAN_CODE2 = @p" + parms.Count + " OR B.EAN_CODE3 = @p" + parms.Count + ")";
				parms.Add(itemCode);
			}
			var sql = $@"SELECT ROW_NUMBER ()OVER (ORDER BY A.ITEM_CODE,A.VALID_DATE,A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.SERIAL_NO ) ROWNUM,A.LOC_CODE,A.ITEM_CODE,B.ITEM_NAME,A.VALID_DATE,A.ENTER_DATE,
														CASE WHEN A.MAKE_NO = '0' THEN '' ELSE A.MAKE_NO END MAKE_NO,
														CASE WHEN A.BOX_CTRL_NO = '0' THEN '' ELSE A.BOX_CTRL_NO END BOX_CTRL_NO,
													  CASE WHEN A.PALLET_CTRL_NO ='0' THEN '' ELSE A.PALLET_CTRL_NO END PALLET_CTRL_NO,
                            CASE WHEN A.SERIAL_NO='0' THEN '' ELSE A.SERIAL_NO END SERIAL_NO,
														SUM(A.QTY) QTY,SUM(A.QTY) MOVE_QTY
										 	 FROM F1913 A
											 JOIN F1903 B
											 	 ON B.GUP_CODE = A.GUP_CODE
											  AND B.CUST_CODE = A.CUST_CODE
											  AND B.ITEM_CODE = A.ITEM_CODE
										  WHERE A.DC_CODE = @p0
											  AND A.GUP_CODE = @p1
											  AND A.CUST_CODE = @p2
											  AND A.LOC_CODE = @p3
                        AND A.QTY > 0
                        {filterSql}
										  GROUP BY A.LOC_CODE,A.ITEM_CODE,B.ITEM_NAME,A.VALID_DATE,A.ENTER_DATE,A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.SERIAL_NO
										  ORDER BY A.ITEM_CODE,A.VALID_DATE,A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.SERIAL_NO";
			return SqlQuery<P08130101Stock>(sql, parms.ToArray());

		}







		public IQueryable<F1913WithF1912Moved> GetF1913WithF1912MovedsByIsExpendDate(string dcCode, string gupCode, string custCode, string srcLocCodeS, string srcLocCodeE, string itemCode, string itemName, string srcWarehouseId,List<string> makeNoList)
		{
			var parameters = new List<object>
												{
																dcCode,gupCode,custCode,srcWarehouseId
												};
			var sql = @"
					SELECT ROW_NUMBER()OVER(ORDER BY A.LOC_CODE) ROWNUM,
						   B.WAREHOUSE_ID,
						   C.WAREHOUSE_NAME,
						   A.LOC_CODE,
						   A.ITEM_CODE,
						   D.ITEM_NAME,
						   D.ITEM_SIZE,
						   D.ITEM_SPEC,
						   D.ITEM_COLOR,
						   A.QTY,
						   0 MOVE_QTY,
						   A.VALID_DATE,
						   A.ENTER_DATE,
						   A.MAKE_NO,
               CASE WHEN A.SERIAL_NO='0' THEN NULL ELSE A.SERIAL_NO END AS SERIAL_NO
					  FROM (  SELECT A.DC_CODE,
									 A.GUP_CODE,
									 A.CUST_CODE,
									 A.ITEM_CODE,
									 A.LOC_CODE,
									 A.VALID_DATE,
									 A.ENTER_DATE,
									 SUM (A.QTY) QTY,
									 A.MAKE_NO,
                   A.SERIAL_NO
								FROM F1913 A
							GROUP BY A.DC_CODE,
									 A.GUP_CODE,
									 A.CUST_CODE,
									 A.ITEM_CODE,
									 A.LOC_CODE,
									 A.VALID_DATE,
									 A.ENTER_DATE,
									 A.MAKE_NO,
                   A.SERIAL_NO) A
						   JOIN F1912 B ON A.DC_CODE = B.DC_CODE AND A.LOC_CODE = B.LOC_CODE
						   LEFT JOIN F1980 C
							  ON A.DC_CODE = C.DC_CODE AND B.WAREHOUSE_ID = C.WAREHOUSE_ID
						   LEFT JOIN F1903 D
							  ON A.GUP_CODE = D.GUP_CODE AND A.ITEM_CODE = D.ITEM_CODE AND A.CUST_CODE = D.CUST_CODE
					 WHERE     A.DC_CODE = @p0
						   AND A.GUP_CODE = @p1
						   AND A.CUST_CODE = @p2
						   AND B.WAREHOUSE_ID = @p3
						   AND B.NOW_STATUS_ID <> '04'
						   AND B.NOW_STATUS_ID <> '03' 
						   AND A.QTY <> 0";

			sql += parameters.CombineNotNullOrEmpty(" AND A.LOC_CODE >= @p{0} ", srcLocCodeS);
			sql += parameters.CombineNotNullOrEmpty(" AND A.LOC_CODE <= @p{0} ", srcLocCodeE);
			sql += parameters.CombineNotNullOrEmpty(" AND A.ITEM_CODE = @p{0} ", itemCode);
			sql += parameters.CombineNotNullOrEmpty(" AND D.ITEM_NAME LIKE CONCAT('%' , @p{0} , '%')", itemName);
			if(makeNoList!=null && makeNoList.Any())
				sql += parameters.CombineNotNullOrEmptySqlInParameters("AND A.MAKE_NO", makeNoList);
			sql += " ORDER BY A.LOC_CODE ";

			var result = SqlQuery<F1913WithF1912Moved>(sql, parameters.ToArray());
			return result;
		}

		public IQueryable<F1913WithF1912Moved> GetF1913WithF1912Moveds(string dcCode, string gupCode, string custCode, string srcLocCodeS, string srcLocCodeE, string itemCode, string itemName, string srcWarehouseId)
		{
			var parameters = new List<SqlParameter>
												{
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode),
																new SqlParameter("@p3", srcWarehouseId)
												};
			var sql = @"  SELECT ROW_NUMBER()OVER(ORDER BY A.LOC_CODE) ROWNUM,
								 B.WAREHOUSE_ID,
								 C.WAREHOUSE_NAME,
								 A.LOC_CODE,
								 A.ITEM_CODE,
								 D.ITEM_NAME,
								 D.ITEM_SIZE,
								 D.ITEM_SPEC,
								 D.ITEM_COLOR,
								 A.QTY,
								 0 MOVE_QTY,
                 CASE WHEN A.SERIAL_NO='0' THEN NULL ELSE A.SERIAL_NO END AS SERIAL_NO
							FROM (  SELECT A.DC_CODE,
										   A.GUP_CODE,
										   A.CUST_CODE,
										   A.ITEM_CODE,
										   A.LOC_CODE,
										   SUM (A.QTY) QTY,
                       A.SERIAL_NO
									  FROM F1913 A
								  GROUP BY A.DC_CODE,
										   A.GUP_CODE,
										   A.CUST_CODE,
										   A.ITEM_CODE,
										   A.LOC_CODE,
                       A.SERIAL_NO) A
								 JOIN F1912 B ON A.DC_CODE = B.DC_CODE AND A.LOC_CODE = B.LOC_CODE
								 LEFT JOIN F1980 C
									ON A.DC_CODE = C.DC_CODE AND B.WAREHOUSE_ID = C.WAREHOUSE_ID
								 LEFT JOIN F1903 D
									ON A.GUP_CODE = D.GUP_CODE AND A.ITEM_CODE = D.ITEM_CODE AND　A.CUST_CODE = D.CUST_CODE
						   WHERE     A.DC_CODE = @p0
								 AND A.GUP_CODE = @p1
								 AND A.CUST_CODE = @p2
								 AND B.WAREHOUSE_ID = @p3
								 AND B.NOW_STATUS_ID <> '04'
								 AND B.NOW_STATUS_ID <> '03'
								 AND A.QTY<>0";

			sql += parameters.CombineNotNullOrEmpty(" AND A.LOC_CODE >= @p{0} ", srcLocCodeS);
			sql += parameters.CombineNotNullOrEmpty(" AND A.LOC_CODE <= @p{0} ", srcLocCodeE);
			sql += parameters.CombineNotNullOrEmpty(" AND A.ITEM_CODE = @p{0} ", itemCode);
			sql += parameters.CombineNotNullOrEmpty(" AND D.ITEM_NAME LIKE CONCAT('%' , @p{0} , '%')", itemName);

			sql += " ORDER BY A.LOC_CODE ";

			var result = SqlQuery<F1913WithF1912Moved>(sql, parameters.ToArray());
			return result;
		}

		/// <summary>
		/// 調整庫存數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="makeNo"></param>
		/// <param name="serialNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="changeQty">異動數量(增加用正數 減少用負數)</param>
		public void AdjustStockQty(string dcCode, string gupCode, string custCode,
			string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
			string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo,long changeQty)
		{

			List<SqlParameter> parameters = new List<SqlParameter>
						{
										new SqlParameter("@p0",SqlDbType.BigInt){ Value = changeQty },
										new SqlParameter("@p1",SqlDbType.VarChar){Value = Current.Staff },
										new SqlParameter("@p2",SqlDbType.NVarChar){Value = Current.StaffName },
										new SqlParameter("@p3",SqlDbType.DateTime2){Value = DateTime.Now },
										new SqlParameter("@p4", SqlDbType.VarChar){Value = dcCode },
										new SqlParameter("@p5", SqlDbType.VarChar){Value = gupCode },
										new SqlParameter("@p6", SqlDbType.VarChar) {Value = custCode },
										new SqlParameter("@p7", SqlDbType.VarChar){ Value = itemCode },
										new SqlParameter("@p8", SqlDbType.VarChar) {Value  = locCode },
										new SqlParameter("@p9", SqlDbType.DateTime2){Value = validDate },
										new SqlParameter("@p10",SqlDbType.DateTime2){Value = enterDate },
										new SqlParameter("@p11",SqlDbType.VarChar){Value = vnrCode },
										new SqlParameter("@p12",SqlDbType.VarChar){Value = serialNo },
										new SqlParameter("@p13",SqlDbType.VarChar){Value = boxCtrlNo },
										new SqlParameter("@p14",SqlDbType.VarChar){Value = palletCtrlNo },
										new SqlParameter("@p15",SqlDbType.VarChar){ Value = makeNo }
						};

			var sql = " UPDATE F1913 SET QTY = QTY + @p0,UPD_STAFF = @p1 ,UPD_NAME=@p2 , UPD_DATE = @p3 " +
													"  WHERE DC_CODE =@p4 " +
													"    AND GUP_CODE =@p5 " +
													"    AND CUST_CODE = @p6 " +
													"    AND ITEM_CODE = @p7 " +
													"    AND LOC_CODE = @p8 " +
													"    AND VALID_DATE = @p9 " +
													"    AND ENTER_DATE = @p10 " +
													"    AND VNR_CODE = @p11 " +
													"    AND SERIAL_NO = @p12 " +
													"    AND BOX_CTRL_NO =@p13 " +
													"    AND PALLET_CTRL_NO =@p14 " +
													"    AND MAKE_NO =@p15 ";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parameters.ToArray());
		}

		public void UpdateF1913ByP2501030000(F2501WcfData data)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", data.VALID_DATE),
				new SqlParameter("@p1", Current.Staff),
				new SqlParameter("@p2", Current.StaffName),
				new SqlParameter("@p3", data.LOC_CODE),
				new SqlParameter("@p4", data.DC_CODE),
				new SqlParameter("@p5", data.GUP_CODE),
				new SqlParameter("@p6", data.CUST_CODE),
				new SqlParameter("@p7", data.SERIAL_NO),
        new SqlParameter("@p8", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			var sqlTmp = string.Empty;

			if (data.IsChangeItemCode)
			{
				sqlTmp = " , ITEM_CODE = @p9 ";
				parameters.Add(new SqlParameter("@p9", data.ITEM_CODE));
			}

			var sql = $@"
									UPDATE F1913
									   SET VALID_DATE = @p0,
									       UPD_STAFF = @p1,
									       UPD_NAME = @p2,
									       UPD_DATE = @p8,
												 LOC_CODE = @p3 
									{sqlTmp}
									 WHERE DC_CODE = @p4
										AND GUP_CODE = @p5 
									  AND CUST_CODE = @p6
									  AND SERIAL_NO = @p7
									";
			
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

        public Boolean CheckF1913HasData(string dcCode, List<string> locCodes)
        {
            List<F1912> result = new List<F1912>();
            string sql = @"SELECT TOP 1 * FROM F1913 WHERE DC_CODE=@p0 AND LOC_CODE IN({0})";

            var splitLocCodes = SplitList(locCodes);
            foreach (var item in splitLocCodes)
            {
                StringBuilder sbSQLIN = new StringBuilder();
                var para = new List<SqlParameter>();
                para.Add(new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode });

                foreach (var locitem in locCodes)
                {
                    sbSQLIN.Append($"@p{para.Count},");
                    para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = locitem });
                }
                sbSQLIN.Remove(sbSQLIN.Length - 1, 1);
                result.AddRange(SqlQuery<F1912>(string.Format(sql, sbSQLIN.ToString()), para.ToArray()));

            }

            return result.Any();
        }

        private List<List<string>> SplitList(List<string> source, int chunkSize = 2000)
        {
            var result = new List<List<string>>();
            var sourceCount = source.Count;
            for (var i = 0; i < sourceCount; i += chunkSize)
            {
                result.Add(source.GetRange(i, Math.Min(chunkSize, sourceCount - i)));
            }
            return result;
        }

    public IQueryable<ReplensihModel> GetReplensihData(string dcCode, string gupCode, string custCode, string itemCode, string makeNo, string serialNo)
    {
      var parms = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode)         { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p1", gupCode)        { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p2", custCode)       { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p3", itemCode)       { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p4", DateTime.Now)   { SqlDbType = SqlDbType.DateTime2 }
            };

      var inSql = string.Empty;
      if (!string.IsNullOrWhiteSpace(serialNo))
      {
        inSql += string.Format(" AND A.SERIAL_NO = @p{0} ", parms.Count);
        parms.Add(new SqlParameter(string.Format("@p{0}", parms.Count), serialNo) { SqlDbType = SqlDbType.VarChar });
      }
      else if (!string.IsNullOrWhiteSpace(makeNo))
      {
        inSql += string.Format(" AND A.MAKE_NO = @p{0} ", parms.Count);
        parms.Add(new SqlParameter(string.Format("@p{0}", parms.Count), makeNo) { SqlDbType = SqlDbType.VarChar });
      }
      var sql = $@" SELECT 
						 A.ITEM_CODE ItemCode, 
						 A.MAKE_NO MakeNo, 
						 A.SERIAL_NO SerialNo, 
						 SUM(A.QTY) ReplensihQty 
								FROM 
								(
									SELECT A.ITEM_CODE, A.MAKE_NO ,A.LOC_CODE , A.VALID_DATE ,A.ENTER_DATE ,A.SERIAL_NO ,A.QTY                
									FROM F1913 A
									JOIN F1912 B ON A.LOC_CODE =B.LOC_CODE AND A.DC_CODE =B.DC_CODE
									JOIN F1919 C ON C.AREA_CODE =B.AREA_CODE AND C.WAREHOUSE_ID = B.WAREHOUSE_ID AND C.DC_CODE =A.DC_CODE AND C.ATYPE_CODE IN ('C')        
									JOIN F1980 E ON E.DC_CODE= A.DC_CODE AND E.WAREHOUSE_ID = B.WAREHOUSE_ID AND E.WAREHOUSE_TYPE ='G'
									WHERE @p4 >= A.ENTER_DATE 
									AND @p4 <= A.VALID_DATE 
									AND B.NOW_STATUS_ID IN ('01', '02')
									AND A.DC_CODE = @p0
									AND A.GUP_CODE = @p1
									AND A.CUST_CODE = @p2
                                    AND A.ITEM_CODE = @p3
                                    { inSql }
								) A GROUP BY A.ITEM_CODE, A.MAKE_NO, A.SERIAL_NO
								ORDER BY A.ITEM_CODE, A.MAKE_NO, A.SERIAL_NO
                      ";
      return SqlQuery<ReplensihModel>(sql, parms.ToArray());
    }

    public IQueryable<string> GetNeedReplenishItemCodes(string dcCode,string gupCode,string custCode)
			 {
			     var sql = @" 
											SELECT A.ITEM_CODE
											FROM (
											SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.ITEM_CODE,B.PICK_SAVE_QTY,
											SUM(CASE WHEN D.ATYPE_CODE IN ('A','B') THEN A.QTY ELSE 0 END) PICK_QTY,
											SUM(CASE WHEN D.ATYPE_CODE IN ('C') THEN A.QTY ELSE 0 END) REPLENISH_QTY     
											FROM F1913 A
											JOIN F1903 B
											ON B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.ITEM_CODE = A.ITEM_CODE
											JOIN F1912 C
											ON C.DC_CODE = A.DC_CODE
											AND C.LOC_CODE = A.LOC_CODE
											JOIN F1919 D
											ON D.DC_CODE = C.DC_CODE
											AND D.WAREHOUSE_ID = C.WAREHOUSE_ID
											AND D.AREA_CODE = C.AREA_CODE
											GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.ITEM_CODE,B.PICK_SAVE_QTY) A
											WHERE A.REPLENISH_QTY>0  --有補貨庫存
											AND A.PICK_QTY <A.PICK_SAVE_QTY --揀區庫存小於補貨安全庫存數
                      AND A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2 ";
					var parms = new List<SqlParameter>
					{
						new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
						new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
						new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
					};
			    return SqlQuery<string>(sql,parms.ToArray());

			 }

    //public F1913 FindDataByKey(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
    //{
    //	List<SqlParameter> parameters = new List<SqlParameter>
    //				{
    //								new SqlParameter("@p0", SqlDbType.VarChar){Value = dcCode },
    //								new SqlParameter("@p1", SqlDbType.VarChar){Value = gupCode },
    //								new SqlParameter("@p2", SqlDbType.VarChar) {Value = custCode },
    //								new SqlParameter("@p3", SqlDbType.VarChar){ Value = itemCode },
    //								new SqlParameter("@p4", SqlDbType.VarChar) {Value  = locCode },
    //								new SqlParameter("@p5", SqlDbType.DateTime2){Value = validDate },
    //								new SqlParameter("@p6",SqlDbType.DateTime2){Value = enterDate },
    //								new SqlParameter("@p7",SqlDbType.VarChar){Value = vnrCode },
    //								new SqlParameter("@p8",SqlDbType.VarChar){Value = serialNo },
    //								new SqlParameter("@p9",SqlDbType.VarChar){Value = boxCtrlNo },
    //								new SqlParameter("@p10",SqlDbType.VarChar){Value = palletCtrlNo },
    //								new SqlParameter("@p11",SqlDbType.VarChar){ Value = makeNo }
    //				};
    //	var sql = @" SELECT  *
    //								 FROM F1913 
    //								 WHERE DC_CODE = @p0
    //									 AND GUP_CODE = @p1
    //									 AND CUST_CODE = @p2
    //									 AND ITEM_CODE = @p3
    //									 AND LOC_CODE = @p4
    //									 AND VALID_DATE = @p5
    //									 AND ENTER_DATE = @p6
    //									 AND VNR_CODE = @p7
    //									 AND SERIAL_NO = @p8
    //									 AND BOX_CTRL_NO = @p9
    //									 AND PALLET_CTRL_NO = @p10
    //									 AND MAKE_NO = @p11 ";

    //	return SqlQuery<F1913>(sql, parameters.ToArray()).FirstOrDefault();
    //}

    /// <summary>
    /// 取得倉庫商品庫存數
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="makeNo"></param>
    /// <param name="validDate"></param>
    /// <param name="warehouseid"></param>
    public int GetLocCodeStockQty(string dcCode, string gupCode, string custCode, string itemCode, string makeNo, DateTime validDate, string warehouseid,string locCode)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3",itemCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4",makeNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5",validDate) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p6",warehouseid) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p7",locCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"
SELECT 
  ISNULL(SUM(QTY),0) 
FROM F1913 a
  INNER JOIN F1912 b
    ON a.DC_CODE=b.DC_CODE
      AND a.LOC_CODE=b.LOC_CODE
WHERE 
  a.DC_CODE=@p0 
  AND a.GUP_CODE=@p1 
  AND a.CUST_CODE=@p2 
  AND a.ITEM_CODE=@p3 
  AND a.MAKE_NO=@p4 
  AND a.VALID_DATE=@p5 
  AND b.WAREHOUSE_ID=@p6
  AND b.LOC_CODE=@p7";

      return SqlQuery<int>(sql,para.ToArray()).Single();
    }

		public IQueryable<string> GetItemMixOtherItemLocs(string dcCode,string gupCode,string custCode,string itemCode)
		{
			var para = new List<SqlParameter>()
			{
				new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p3",itemCode) { SqlDbType = SqlDbType.VarChar }
			};
			var sql = @" SELECT LOC_CODE
										FROM F1913
										WHERE LOC_CODE IN (
										SELECT DISTINCT LOC_CODE
										FROM F1913
										WHERE DC_CODE = @p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 
										AND ITEM_CODE = @p3) 
										GROUP BY LOC_CODE
										HAVING COUNT(DISTINCT ITEM_CODE) > 1";
			return SqlQuery<string>(sql, para.ToArray());
		}

		/// <summary>
		/// 取得揀位商品儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public IQueryable<NewItemLocPriorityInfo> GetNewItemPickLocPriorityInfo(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			return GetNewItemLocPriorityInfies(dcCode, gupCode, custCode, itemCodes, true, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
		}

		/// <summary>
		/// 取得補位商品儲位優先順序資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseType"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public IQueryable<NewItemLocPriorityInfo> GetNewItemResupplyLocPriorityInfo(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			return GetNewItemLocPriorityInfies(dcCode, gupCode, custCode, itemCodes, false, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
		}

		public IQueryable<NewItemLocPriorityInfo> GetNewItemLocPriorityInfies(string dcCode, string gupCode, string custCode, List<string> itemCodes, bool isPickLoc, bool isForIn, string warehouseType = "", string targetWarehouseId = "", string wareHouseTmpr = "")
		{
			var filterSql = string.Empty;
			var parameters = new List<object>()
			{
				dcCode,gupCode,custCode,custCode
      };

			if (itemCodes.Count == 1)
			{
				filterSql = string.Format(@" And A.ITEM_CODE=@p{0} ", parameters.Count);
				parameters.Add(itemCodes[0]);
			}
			// 若有填倉別 Id，就只針對該倉別 Id 做篩選
			if (!string.IsNullOrEmpty(targetWarehouseId))
			{
				filterSql += string.Format(" And C.WAREHOUSE_ID=@p{0} ", parameters.Count);
				parameters.Add(targetWarehouseId);
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				filterSql += string.Format(" And C.WAREHOUSE_TYPE=@p{0} ", parameters.Count);
				parameters.Add(warehouseType);
			}

			// 若不是報廢倉的話，則加上效期條件
			if (!(warehouseType == "D" || (targetWarehouseId != null && targetWarehouseId.StartsWith("D"))))
			{
        filterSql += string.Format(" AND A.VALID_DATE > @p{0} ", parameters.Count);
        parameters.Add(DateTime.Now);
      }

			if (isPickLoc)
				filterSql += @"
				   And (D.ATYPE_CODE='A' Or D.ATYPE_CODE='B')";
			else
				filterSql += @"
				   And D.ATYPE_CODE='C'";

			if (isForIn)
				filterSql += @"
				   And B.NOW_STATUS_ID<>'02'";
			else
				filterSql += @"
				   And B.NOW_STATUS_ID<>'03'";

			if (!string.IsNullOrEmpty(wareHouseTmpr))
			{
				filterSql += parameters.CombineSqlInParameters(" AND   C.TMPR_TYPE ", wareHouseTmpr.Split(','));
			}

			var sql = $@"SELECT  ROW_NUMBER()OVER(ORDER BY A.LOC_CODE ASC) ROWNUM,A.*
									FROM (
									SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.MAKE_NO,
									B.WAREHOUSE_ID,B.HOR_DISTANCE,B.USEFUL_VOLUMN,B.USED_VOLUMN,B.AREA_CODE,C.WAREHOUSE_TYPE,C.TMPR_TYPE,
									D.ATYPE_CODE,E.HANDY,SUM(A.QTY) QTY
									FROM F1913 A
									INNER JOIN F1912 B
									ON B.DC_CODE = A.DC_CODE
									AND B.LOC_CODE = A.LOC_CODE
									INNER JOIN F1980 C
									ON C.DC_CODE = B.DC_CODE
									AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
									INNER JOIN F1919 D
									ON D.DC_CODE = B.DC_CODE
									AND D.WAREHOUSE_ID = B.WAREHOUSE_ID
									AND D.AREA_CODE = B.AREA_CODE
									INNER JOIN F1942 E
									ON E.LOC_TYPE_ID = B.LOC_TYPE_ID
									INNER JOIN F198001 F
									ON F.TYPE_ID = C.WAREHOUSE_TYPE
									WHERE A.DC_CODE= @p0
									AND A.GUP_CODE = @p1
									AND A.CUST_CODE =@p2
									AND ((F.LOC_MUSTSAME_NOWCUSTCODE ='1' AND B.NOW_CUST_CODE =@p3) OR  F.LOC_MUSTSAME_NOWCUSTCODE ='0' OR B.NOW_CUST_CODE='0' )
									{filterSql}
                  GROUP BY  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.MAKE_NO,
									B.WAREHOUSE_ID,B.HOR_DISTANCE,B.USEFUL_VOLUMN,B.USED_VOLUMN,B.AREA_CODE,C.WAREHOUSE_TYPE,C.TMPR_TYPE,
									D.ATYPE_CODE,E.HANDY) A
			";
			return SqlQuery<NewItemLocPriorityInfo>(sql, parameters.ToArray());

		}


    public List<F1913ByLocStauts> GetF1913ByLocStauts(string dcCode, string gupCode, string custCode, List<string> itemList)
        {
            List<F1913ByLocStauts> result = new List<F1913ByLocStauts>();

            var sql = @"SELECT A.DC_CODE, 
                        A.GUP_CODE, 
                        A.CUST_CODE, 
                        A.ITEM_CODE, 
                        A.MAKE_NO, 
                        A.SERIAL_NO, 
                        C.ATYPE_CODE, 
                        SUM(A.QTY) QTY
                        FROM F1913 A 
                        JOIN F1912 B ON A.LOC_CODE = B.LOC_CODE AND A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
                        JOIN F1919 C ON B.AREA_CODE = C.AREA_CODE AND B.DC_CODE = C.DC_CODE
                        WHERE A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.ITEM_CODE IN ({0})
                        AND A.VALID_DATE > @p3
                        AND B.NOW_STATUS_ID NOT IN ('03','04')
						            AND B.WAREHOUSE_ID LIKE 'G%'
                        GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE, A.MAKE_NO, A.SERIAL_NO, C.ATYPE_CODE";

            var splitItemList = SplitList(itemList, 500);

            foreach (var list in splitItemList)
            {
                StringBuilder sqlIn = new StringBuilder();

                var param = new List<SqlParameter>()
                {
                    new SqlParameter("@p0", SqlDbType.VarChar) {Value = dcCode},
                    new SqlParameter("@p1", SqlDbType.VarChar) {Value = gupCode},
                    new SqlParameter("@p2", SqlDbType.VarChar) {Value = custCode},
                    new SqlParameter("@p3", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                };

                foreach (var item in list)
                {
                    sqlIn.Append($"@p{param.Count},");
                    param.Add(new SqlParameter($"@p{param.Count}", SqlDbType.VarChar) { Value = item });
                }

                sqlIn.Remove(sqlIn.Length - 1, 1);
                sql = string.Format(sql, sqlIn.ToString());
                result.AddRange(SqlQuery<F1913ByLocStauts>(sql, param.ToArray()));
            }

            return result;
        }

    /// <summary>
    /// 從GetDatasByInventoryWareHouseList簡化，單純只檢查
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="inventoryWareHouses"></param>
    /// <param name="itemCodeList"></param>
    /// <returns></returns>
    public IQueryable<F1913Ex> P180201CheckInventoryStockQty(string dcCode, string gupCode, string custCode, IEnumerable<InventoryWareHouse> inventoryWareHouses, IEnumerable<string> itemCodeList)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",dcCode){SqlDbType=SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType=SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType=SqlDbType.VarChar},
      };

      var f1912Condition = string.Empty;
      if (inventoryWareHouses.Any())
      {
        var sqlList = new List<string>();
        foreach (var inventoryWareHouse in inventoryWareHouses)
        {
          var sqlStr = " (B.DC_CODE =@p0 AND B.WAREHOUSE_ID = '" + inventoryWareHouse.WAREHOUSE_ID + "' ";
          if (!string.IsNullOrWhiteSpace(inventoryWareHouse.AREA_CODE))
            sqlStr += " AND B.AREA_CODE = '" + inventoryWareHouse.AREA_CODE + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_BEGIN))
            sqlStr += " AND B.FLOOR >= '" + inventoryWareHouse.FLOOR_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.FLOOR_END))
            sqlStr += " AND B.FLOOR <= '" + inventoryWareHouse.FLOOR_END + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_BEGIN))
            sqlStr += " AND B.CHANNEL >= '" + inventoryWareHouse.CHANNEL_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.CHANNEL_END))
            sqlStr += " AND B.CHANNEL <= '" + inventoryWareHouse.CHANNEL_END + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_BEGIN))
            sqlStr += " AND B.PLAIN >= '" + inventoryWareHouse.PLAIN_BEGIN + "' ";
          if (!string.IsNullOrEmpty(inventoryWareHouse.PLAIN_END))
            sqlStr += " AND B.PLAIN <= '" + inventoryWareHouse.PLAIN_END + "' ";
          sqlStr += " ) ";
          sqlList.Add(sqlStr);
        }
        f1912Condition += " AND ( " + string.Join(" OR ", sqlList.ToArray()) + ") ";
      }

      var sql = $@"

SELECT A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.LOC_CODE, A.ITEM_CODE, A.VALID_DATE, A.ENTER_DATE, B.WAREHOUSE_ID, A.SERIAL_NO, A.QTY, A.BOX_CTRL_NO, A.PALLET_CTRL_NO, A.MAKE_NO
FROM F1913 A
	INNER JOIN F1912 B
	ON B.DC_CODE = A.DC_CODE
		AND B.LOC_CODE = A.LOC_CODE
WHERE A.DC_CODE = @p0
	AND A.GUP_CODE = @p1
	AND A.CUST_CODE = @p2
	{f1912Condition}";

      if (itemCodeList.Any())
        sql += param.CombineSqlInParameters(" AND A.ITEM_CODE", itemCodeList, SqlDbType.VarChar);
      var result = SqlQuery<F1913Ex>(sql, param.ToArray()).ToList();
      return result.AsQueryable();

    }

		public F1913 FindDataByKey(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p3",itemCode){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p4",locCode){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p5",validDate){SqlDbType=SqlDbType.DateTime2},
				new SqlParameter("@p6",enterDate){SqlDbType=SqlDbType.DateTime2},
				new SqlParameter("@p7",serialNo){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p8",boxCtrlNo){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p9",palletCtrlNo){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p10",makeNo){SqlDbType=SqlDbType.VarChar},
				new SqlParameter("@p11",vnrCode){SqlDbType=SqlDbType.VarChar},
			};

			var sql = @" SELECT TOP (1) *
                    FROM F1913 
                  WHERE DC_CODE = @p0
                    AND GUP_CODE = @p1
                    AND CUST_CODE = @p2
                    AND ITEM_CODE = @p3
                    AND LOC_CODE = @p4
                    AND VALID_DATE = @p5
                    AND ENTER_DATE = @p6
                    AND SERIAL_NO = @p7
                    AND BOX_CTRL_NO = @p8
                    AND PALLET_CTRL_NO = @p9
                    AND MAKE_NO = @p10 
                    AND VNR_CODE = @p11 ";

			return SqlQuery<F1913>(sql, param.ToArray()).FirstOrDefault();
		}

    public IQueryable<StockDataByInventory> GetStockQtyByInventory(string dcCode, string gupCode, string custCode, string warehouseId, StockDataByInventoryParam param)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)           { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)          { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)         { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", warehouseId)      { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", param.ITEM_CODE)  { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5", param.VALID_DATE) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p6", param.MAKE_NO)    { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p7", param.LOC_CODE)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p8", param.ENTER_DATE) { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p9", param.BOX_CTRL_NO){ SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p10", param.PALLET_CTRL_NO){ SqlDbType = SqlDbType.VarChar },

      };

      var sql = @"
SELECT 
  ITEM_CODE, 
  VALID_DATE, 
  MAKE_NO, 
  LOC_CODE, 
  ENTER_DATE, 
  BOX_CTRL_NO, 
  PALLET_CTRL_NO, 
  SUM(QTY) AS QTY, 
  SUM(UNMOVE_STOCK_QTY) AS UNMOVE_STOCK_QTY 
FROM 
  (
    SELECT 
      A.ITEM_CODE, 
      A.VALID_DATE, 
      A.MAKE_NO, 
      A.LOC_CODE, 
      A.ENTER_DATE, 
      A.BOX_CTRL_NO, 
      A.PALLET_CTRL_NO, 
      A.QTY, 
      0 AS UNMOVE_STOCK_QTY 
    FROM 
      F1913 A 
      INNER JOIN(
        SELECT 
          DC_CODE, 
          LOC_CODE 
        FROM 
          F1912 
        WHERE 
          DC_CODE = @p0 
          AND GUP_CODE = @p1 
          AND CUST_CODE = @p2 
          AND WAREHOUSE_ID = @p3
      ) B ON A.DC_CODE = B.DC_CODE 
      AND A.LOC_CODE = B.LOC_CODE 
    WHERE 
      A.DC_CODE = @p0 
      AND A.GUP_CODE = @p1 
      AND A.CUST_CODE = @p2 
      AND A.ITEM_CODE = @p4 
      AND A.VALID_DATE = @p5 
      AND A.MAKE_NO = @p6 
      AND A.LOC_CODE = @p7 
      AND A.ENTER_DATE = @p8 
      AND A.BOX_CTRL_NO = @p9 
      AND A.PALLET_CTRL_NO = @p10 
    UNION ALL 
    SELECT 
      A.ITEM_CODE, 
      A.VALID_DATE, 
      A.MAKE_NO, 
      A.LOC_CODE, 
      A.ENTER_DATE, 
      A.BOX_CTRL_NO, 
      A.PALLET_CTRL_NO, 
      0 AS QTY, 
      A.B_PICK_QTY AS UNMOVE_STOCK_QTY 
    FROM 
      VW_VirtualStock A 
      INNER JOIN(
        SELECT 
          DC_CODE, 
          LOC_CODE 
        FROM 
          F1912 
        WHERE 
          DC_CODE = @p0 
          AND GUP_CODE = @p1 
          AND CUST_CODE = @p2 
          AND WAREHOUSE_ID = @p3
      ) B ON A.DC_CODE = B.DC_CODE 
      AND A.LOC_CODE = B.LOC_CODE 
    WHERE 
      A.DC_CODE = @p0 
      AND A.GUP_CODE = @p1 
      AND A.CUST_CODE = @p2 
      AND A.ITEM_CODE = @p4 
      AND A.VALID_DATE = @p5 
      AND A.MAKE_NO = @p6 
      AND A.LOC_CODE = @p7 
      AND A.ENTER_DATE = @p8 
      AND A.BOX_CTRL_NO = @p9 
      AND A.PALLET_CTRL_NO = @p10
      AND A.STATUS='0'
  ) U 
GROUP BY 
  ITEM_CODE, 
  VALID_DATE, 
  MAKE_NO, 
  LOC_CODE, 
  ENTER_DATE, 
  BOX_CTRL_NO, 
  PALLET_CTRL_NO
";

      return SqlQuery<StockDataByInventory>(sql, para.ToArray());
      /*
      var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                    x.GUP_CODE == gupCode &&
                    x.CUST_CODE == custCode &&
                    x.WAREHOUSE_ID == warehouseId);

      var locCodes = f1912s.Select(z => z.LOC_CODE);

      #region 實際庫存數
      var f1913s = _db.F1913s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      locCodes.Contains(x.LOC_CODE) &&
      param.Any(z =>
      z.ITEM_CODE == x.ITEM_CODE &&
      z.VALID_DATE == x.VALID_DATE &&
      z.MAKE_NO == x.MAKE_NO &&
            z.LOC_CODE == x.LOC_CODE &&
            z.ENTER_DATE == x.ENTER_DATE &&
            z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
            z.PALLET_CTRL_NO == x.PALLET_CTRL_NO));

      var stockDatas = f1913s.GroupBy(x => new StockDataByInventoryParam
      {
        ITEM_CODE = x.ITEM_CODE,
        VALID_DATE = Convert.ToDateTime(x.VALID_DATE),
        MAKE_NO = x.MAKE_NO,
        LOC_CODE = x.LOC_CODE,
        ENTER_DATE = x.ENTER_DATE,
        BOX_CTRL_NO = x.BOX_CTRL_NO,
        PALLET_CTRL_NO = x.PALLET_CTRL_NO
      })
      .Select(x => new StockDataByInventory
      {
        ITEM_CODE = x.Key.ITEM_CODE,
        VALID_DATE = x.Key.VALID_DATE,
        MAKE_NO = x.Key.MAKE_NO,
        LOC_CODE = x.Key.LOC_CODE,
        ENTER_DATE = x.Key.ENTER_DATE,
        BOX_CTRL_NO = x.Key.BOX_CTRL_NO,
        PALLET_CTRL_NO = x.Key.PALLET_CTRL_NO,
        QTY = Convert.ToInt32(x.Sum(z => z.QTY))
      });
      #endregion

      #region 虛擬未搬動庫存數
      var f1511s = _db.F1511s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      locCodes.Contains(x.LOC_CODE) &&
      x.STATUS == "0" &&
      param.Any(z =>
      z.ITEM_CODE == x.ITEM_CODE &&
      z.VALID_DATE == x.VALID_DATE &&
      z.MAKE_NO == x.MAKE_NO &&
            z.LOC_CODE == x.LOC_CODE &&
            z.ENTER_DATE == x.ENTER_DATE &&
            z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
            z.PALLET_CTRL_NO == x.PALLET_CTRL_NO));

      var virtualDatas = f1511s.GroupBy(x => new
      {
        x.ITEM_CODE,
        VALID_DATE = Convert.ToDateTime(x.VALID_DATE),
        x.MAKE_NO,
        x.LOC_CODE,
        x.ENTER_DATE,
        x.BOX_CTRL_NO,
        x.PALLET_CTRL_NO
      }).ToList()
      .Select(x => new StockDataByInventory
      {
        ITEM_CODE = x.Key.ITEM_CODE,
        VALID_DATE = x.Key.VALID_DATE,
        MAKE_NO = x.Key.MAKE_NO,
        LOC_CODE = x.Key.LOC_CODE,
        ENTER_DATE = Convert.ToDateTime(x.Key.ENTER_DATE),
        BOX_CTRL_NO = x.Key.BOX_CTRL_NO,
        PALLET_CTRL_NO = x.Key.PALLET_CTRL_NO,
        QTY = x.Sum(z => z.B_PICK_QTY)
      });
      #endregion

      var datas = from A in param
                  join B in stockDatas
                  on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE, A.BOX_CTRL_NO, A.PALLET_CTRL_NO } equals new { B.ITEM_CODE, B.VALID_DATE, B.MAKE_NO, B.LOC_CODE, B.ENTER_DATE, B.BOX_CTRL_NO, B.PALLET_CTRL_NO } into subB
                  from B in subB.DefaultIfEmpty()
                  join C in virtualDatas
                  on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE, A.BOX_CTRL_NO, A.PALLET_CTRL_NO } equals new { C.ITEM_CODE, C.VALID_DATE, C.MAKE_NO, C.LOC_CODE, C.ENTER_DATE, C.BOX_CTRL_NO, C.PALLET_CTRL_NO } into subC
                  from C in subC.DefaultIfEmpty()
                  select new StockDataByInventory
                  {
                    ITEM_CODE = A.ITEM_CODE,
                    VALID_DATE = A.VALID_DATE,
                    MAKE_NO = A.MAKE_NO,
                    LOC_CODE = A.LOC_CODE,
                    ENTER_DATE = A.ENTER_DATE,
                    BOX_CTRL_NO = A.BOX_CTRL_NO,
                    PALLET_CTRL_NO = A.PALLET_CTRL_NO,
                    QTY = B == null ? 0 : B.QTY,
                    UNMOVE_STOCK_QTY = C == null ? 0 : C.QTY
                  };

      return datas.AsQueryable();
      */
    }

		public void ExecSpStockChange(DataTable dtStockChange,string batchNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.Structured){ Value = dtStockChange,TypeName = "dbo.UT_StockChange"},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = Current.Staff},
				new SqlParameter("@p2",SqlDbType.NVarChar){Value = Current.StaffName},
				new SqlParameter("@p3",SqlDbType.VarChar){Value = batchNo}
			};

			var sql = " exec SP_MergeStockChange @utSC = @p0, @execStaff= @p1,@execName =@p2,@batchNo = @p3 ";
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

    /// <summary>
    /// 取得3PL貨主良品倉或管理倉即期品商品庫存資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="withoutWhId">不要取得庫存的倉別編號</param>
    /// <returns></returns>
    public IQueryable<ProcImmediateItem> GetProcImmediateItem(string dcCode, string gupCode, string custCode, List<string> withoutWhId)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.DateTime2) { Value = DateTime.Today },

      };

      var sql = $@"SELECT
	A.DC_CODE,
	A.GUP_CODE,
	A.CUST_CODE,
	B.WAREHOUSE_ID,
	A.ITEM_CODE,
	A.VALID_DATE,
	A.LOC_CODE,
	SUM(A.QTY) QTY
FROM
	F1913 A
INNER JOIN F1912 B WITH(NOLOCK) 
	ON A.DC_CODE = B.DC_CODE
	AND A.LOC_CODE = B.LOC_CODE
INNER JOIN F1980 C WITH(NOLOCK)
	ON B.DC_CODE = C.DC_CODE
	AND B.WAREHOUSE_ID = C.WAREHOUSE_ID
INNER JOIN F1903 D WITH(NOLOCK)
		ON A.GUP_CODE = D.GUP_CODE 
	AND A.CUST_CODE = D.CUST_CODE 
	AND A.ITEM_CODE = D.ITEM_CODE 
WHERE 
	A.DC_CODE = @p0
	AND A.GUP_CODE = @p1
	AND A.CUST_CODE = @p2
	AND D.ALL_SHP IS NOT NULL 
	AND D.ALL_SHP >0
	AND DATEDIFF(DAY,@p3,A.VALID_DATE) < (D.ALL_SHP + 1)
	AND C.WAREHOUSE_TYPE IN('G','M')
	AND A.QTY > 0
  {para.CombineSqlNotInParameters("AND B.WAREHOUSE_ID", withoutWhId, SqlDbType.VarChar)}
GROUP BY A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,B.WAREHOUSE_ID ,A.ITEM_CODE ,A.VALID_DATE ,A.LOC_CODE";
      return SqlQuery<ProcImmediateItem>(sql, para.ToArray());
    }

    public F1913BoxKeyColumn GetF1913BoxKeyColumn(string dcCode, string gupCode, string custCode, string boxNum)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = boxNum }
      };

      var sql = @"
                SELECT TOP 1
                  DC_CODE, 
                  GUP_CODE, 
                  CUST_CODE, 
                  ITEM_CODE,
                  LOC_CODE,
                  VALID_DATE,
                  ENTER_DATE,
                  MAKE_NO,
                  SERIAL_NO,
                  VNR_CODE,
                  BOX_CTRL_NO,
                  PALLET_CTRL_NO
                FROM F1913 
                WHERE 
                  DC_CODE = @p0 
                  AND GUP_CODE = @p1 
                  AND CUST_CODE = @p2 
                  AND ITEM_CODE = @p3
                ";

      return SqlQuery<F1913BoxKeyColumn>(sql, param.ToArray()).FirstOrDefault();
    }

    public void UpdateBoxStock(string dcCode, string gupCode, string custCode, string boxNum, string locCode, DateTime validDate, DateTime enterDate,
      string mekeNo, string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.DateTime2) { Value = DateTime.Now },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = Current.Staff },
        new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Current.StaffName },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p4", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p5", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p6", SqlDbType.VarChar) { Value = boxNum },
        new SqlParameter("@p7", SqlDbType.VarChar) { Value = locCode },
        new SqlParameter("@p8", SqlDbType.DateTime2) { Value = validDate },
        new SqlParameter("@p9", SqlDbType.DateTime2) { Value = enterDate },
        new SqlParameter("@p10", SqlDbType.VarChar) { Value = mekeNo },
        new SqlParameter("@p11", SqlDbType.VarChar) { Value = serialNo },
        new SqlParameter("@p12", SqlDbType.VarChar) { Value = vnrCode },
        new SqlParameter("@p13", SqlDbType.VarChar) { Value = boxCtrlNo },
        new SqlParameter("@p14", SqlDbType.VarChar) { Value = palletCtrlNo }
      };

      var sql = @"
                UPDATE F1913
                SET
                  QTY -=1,
                  UPD_DATE = @p0,
                  UPD_STAFF = @p1,
                  UPD_NAME = @p2
                WHERE 
                  DC_CODE = @p3 
                  AND GUP_CODE = @p4 
                  AND CUST_CODE = @p5 
                  AND ITEM_CODE = @p6
                  AND LOC_CODE = @p7
                  AND VALID_DATE = @p8
                  AND ENTER_DATE = @p9
                  AND MAKE_NO = @p10
                  AND SERIAL_NO = @p11
                  AND VNR_CODE = @p12
                  AND BOX_CTRL_NO = @p13
                  AND PALLET_CTRL_NO = @p14
                ";

      ExecuteSqlCommand(sql, param.ToArray());
    }

    public IQueryable<F1913> GetRemoveData(string dcCode, List<string> locs)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", SqlDbType.VarChar)    { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.DateTime2)  { Value = DateTime.Today.AddDays(-30) }
      };

      var sql = $@"SELECT * FROM F1913 WHERE DC_CODE=@p0 AND (CASE WHEN UPD_DATE IS NULL THEN CRT_DATE
                    ELSE UPD_DATE END) < @p1 AND QTY=0 {para.CombineSqlInParameters("AND LOC_CODE", locs, SqlDbType.VarChar)}";

      return SqlQueryWithSqlParameterSetDbType<F1913>(sql, para.ToArray());
    }

  }
}
