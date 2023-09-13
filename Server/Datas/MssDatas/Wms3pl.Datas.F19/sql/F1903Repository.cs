using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1903Repository : RepositoryBase<F1903, Wms3plDbContext, F1903Repository>
	{
		public decimal? GetRatio(string itemCode, string gupCode, string custCode, string orderNo)
		{
			var sql = @"
                        Select 
                        CHECK_PERCENT
                          From (Select CHECK_PERCENT , SortNo
                                  From (Select A.CHECK_PERCENT, 1 SortNo
                                          From F1903 A 
                                         Where A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                           And A.CUST_CODE=@p2
                                        Union
                                        Select B.CHECK_PERCENT, 2 SortNo
                                          From F1917 B, F1903 A
                                         Where B.ACODE=A.LTYPE
                                           And B.BCODE=A.MTYPE
                                           And B.CCODE=A.STYPE
                                           And B.GUP_CODE=A.GUP_CODE
                                           And B.CUST_CODE=A.CUST_CODE
                                           And A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                        Union
                                        Select B.CHECK_PERCENT, 3 SortNo
                                          From F1916 B, F1903 A
                                         Where B.ACODE=A.LTYPE
                                           And B.BCODE=A.MTYPE
                                           And B.GUP_CODE=A.GUP_CODE
                                           And B.CUST_CODE=A.CUST_CODE
                                           And A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                        Union
                                        Select B.CHECK_PERCENT, 4 SortNo
                                          From F1915 B, F1903 A
                                         Where B.ACODE=A.LTYPE
                                           And B.GUP_CODE=A.GUP_CODE
                                           And B.CUST_CODE=A.CUST_CODE
                                           And A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                        Union
                                        Select A.CHECK_PERCENT, 5 SortNo
                                          From F1908 C, F010201 B, F010202 D, F1903 A
                                         Where B.VNR_CODE=C.VNR_CODE
                                           And B.GUP_CODE=C.GUP_CODE
                                           And D.STOCK_NO=B.STOCK_NO
                                           And D.GUP_CODE=B.GUP_CODE
                                           And D.CUST_CODE=D.CUST_CODE
                                           And D.ITEM_CODE=A.ITEM_CODE
                                           And B.GUP_CODE=A.GUP_CODE
                                           And B.CUST_CODE=A.CUST_CODE
                                           And A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                           And A.CUST_CODE=@p2
                                           And B.STOCK_NO =@p3
                                        Union
                                        Select B.CHECK_PERCENT, 6 SortNo
                                          From F1909 B, F1903 A
                                         Where B.GUP_CODE=A.GUP_CODE
                                           And B.CUST_CODE=A.CUST_CODE
                                           And A.ITEM_CODE=@p0
                                           And A.GUP_CODE=@p1
                                           And A.CUST_CODE=@p2
                                       ) P
                                 Where P.CHECK_PERCENT IS NOT NULL
                              ) P2
                        Where SortNo = 1
                        Order By SortNo";

			var sqlParameters = new[] {
				new SqlParameter("@p0", itemCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", orderNo)
			};
			return SqlQuery<decimal?>(sql, sqlParameters).SingleOrDefault();
		}

		//取出該商品所在儲位有混品的儲位
		public IList<string> GetItemMixItemLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
		{

			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", dcCode));
			sqlParamers.Add(new SqlParameter("@p1", gupCode));
			sqlParamers.Add(new SqlParameter("@p2", custCode));
			sqlParamers.Add(new SqlParameter("@p3", itemCode));

			var sql = @"					
        					select distinct B.LOC_CODE 
        					from F1903 A
        					join F1913 B 
                    on A.ITEM_CODE =B.ITEM_CODE 
                   and A.GUP_CODE = B.GUP_CODE
                   and A.CUST_CODE = B.CUST_CODE
        					where B.DC_CODE =@p0
        						and A.GUP_CODE=@p1
        						and A.CUST_CODE =@p2	
        						and B.ITEM_CODE <> @p3
                    and B.QTY > 0
        				";

			//Loc 是否有指定判斷儲位
			if (!string.IsNullOrEmpty(locCode))
			{
				sql += " AND B.LOC_CODE = @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, locCode));
			}

			sql += " group by B.LOC_CODE order by B.LOC_CODE ";

			var result = SqlQuery<string>(sql, sqlParamers.ToArray()).ToList();
			return result.ToList();
		}


		//取出該商品所在儲位有混批(期效)的儲位
		public IList<string> GetItemMixBatchLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime? validDate)
		{
			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", dcCode));
			sqlParamers.Add(new SqlParameter("@p1", gupCode));
			sqlParamers.Add(new SqlParameter("@p2", custCode));
			sqlParamers.Add(new SqlParameter("@p3", itemCode));
			sqlParamers.Add(new SqlParameter("@p4", locCode));

			var sql = @"					
        						select distinct B.LOC_CODE 
        						from F1903 A
        						join F1913 B on A.ITEM_CODE =B.ITEM_CODE 
                    and A.GUP_CODE = B.GUP_CODE
                    and A.CUST_CODE = B.CUST_CODE
        						where B.DC_CODE =@p0
        							and A.GUP_CODE =@p1
        							and A.CUST_CODE =@p2
        							and A.ITEM_CODE =@p3						
        					    and B.LOC_CODE= @p4
                      and B.QTY > 0
        				";

			//效期
			if (validDate.HasValue)
			{
				sql += " AND B.VALID_DATE <> @p" + sqlParamers.Count;
				sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, validDate));
			}

			sql += " group by B.LOC_CODE ,B.VALID_DATE	order by B.LOC_CODE";

			var result = SqlQuery<string>(sql, sqlParamers.ToArray()).ToList();
			return result.ToList();
		}

		/// <summary>
		/// 取得商品資訊(以F1903欄位為主)
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F1903Plus> GetItemInfo(string gupCode, string custCode, string itemCode)
		{
			var sqlParamers = new List<SqlParameter>
							{
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode),
								new SqlParameter("@p2", itemCode)
							};

			var sql = @"	
                          SELECT A.ITEM_CODE, A.ITEM_NAME, 
        		                           A.EAN_CODE1, A.ITEM_ENGNAME, A.ITEM_COLOR, A.ITEM_SIZE, 
        		                           A.TYPE, A.ITEM_HUMIDITY, A.ITEM_NICKNAME, 
        		                           A.ITEM_ATTR, A.ITEM_SPEC, A.TMPR_TYPE, 
        		                           A.FRAGILE, A.SPILL, A.ITEM_TYPE, 
        			                         A.ITEM_UNIT, A.ITEM_CLASS, A.SIM_SPEC, A.VIRTUAL_TYPE,A.CUST_ITEM_CODE
                                  FROM F1903 A                                   
                                 WHERE A.GUP_CODE = @p0 AND A.CUST_CODE = @p1 
                                   AND (A.ITEM_CODE = @p2 OR A.EAN_CODE1 = @p2 OR A.EAN_CODE2 = @p2 OR A.EAN_CODE3 = @p2)
                            ";


			var result = SqlQuery<F1903Plus>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;
		}

		/// <summary>
		/// 取得非此貨主同業主同商品
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="exceptCustCode">排除貨主編號</param>
		/// <returns></returns>
		public IQueryable<F1903> GetDatas(string gupCode, string itemCode, string exceptCustCode)
		{
			var sql = @" 
                        SELECT * 
                             FROM F1903 A
                            WHERE A.GUP_CODE = @p0 
                              AND A.ITEM_CODE = @p1 
                              AND A.CUST_CODE <> @p2
                        ";
			var param = new object[] { gupCode, itemCode, exceptCustCode };
			return SqlQuery<F1903>(sql, param);
		}

		public IQueryable<StockSettleData> GetRecvSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                            SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,                         
        							                 ISNULL(SUM(C.RECV_QTY),0) RECV_QTY                         
        					                FROM F1903 A 
        		                 LEFT JOIN F020201 C ON A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE 
        			 		                 AND A.ITEM_CODE = C.ITEM_CODE AND C.DC_CODE = @p0 AND C.RECE_DATE = @p3
        				                 WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
        		                  GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE 
        				 ";
			var param = new SqlParameter[]
			{
												new SqlParameter("@p0",dcCode),
												new SqlParameter("@p1",gupCode),
												new SqlParameter("@p2",custCode),
												new SqlParameter("@p3",calDate)
			};
			return SqlQuery<StockSettleData>(sql, param);
		}

		public IQueryable<StockSettleData> GetDeliverySettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                    SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,                         
                                ISNULL(SUM(H.A_DELV_QTY),0) DELV_QTY
                            FROM F1903 A      
                        LEFT JOIN F050801 G ON A.GUP_CODE = G.GUP_CODE AND A.CUST_CODE = G.CUST_CODE  AND G.DC_CODE = @p0 AND G.STATUS IN ('6','5') AND G.APPROVE_DATE >=@p3 AND G.APPROVE_DATE < @p4
                        LEFT JOIN F050802 H ON G.DC_CODE = H.DC_CODE AND G.GUP_CODE = H.GUP_CODE AND G.CUST_CODE = H.CUST_CODE AND A.ITEM_CODE = H.ITEM_CODE 
                            AND G.WMS_ORD_NO = H.WMS_ORD_NO                                                   
                            WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
                        GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE 
                ";

			var param = new SqlParameter[]
			{
												new SqlParameter("@p0",dcCode),
												new SqlParameter("@p1",gupCode),
												new SqlParameter("@p2",custCode),
												new SqlParameter("@p3",calDate),
												new SqlParameter("@p4",calDate.AddDays(1))
			};
			return SqlQuery<StockSettleData>(sql, param);
		}

		public IQueryable<StockSettleData> GetReturnSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                     SELECT @p0 DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                A.ITEM_CODE,
                                ISNULL (SUM (TB.MOVED_QTY), 0) RTN_QTY
                        FROM F1903 A
                                LEFT JOIN
                                (   SELECT D.DC_CODE,
                                    D.GUP_CODE,
                                    D.CUST_CODE,
                                    D.WMS_NO,
                                    D.ITEM_CODE,
                                    SUM (D.QTY ) MOVED_QTY
                            FROM F510101 D
                            WHERE     D.DC_CODE = @p0
                                    AND D.GUP_CODE = @p1
                                    AND D.CUST_CODE = @p2
                                    AND D.POSTING_DATE >= @p3
                                    AND D.POSTING_DATE < @p4
                        GROUP BY D.DC_CODE,
                                    D.GUP_CODE,
                                    D.CUST_CODE,
                                    D.WMS_NO,
                                    D.ITEM_CODE) TB
                                ON A.GUP_CODE = TB.GUP_CODE AND A.CUST_CODE = TB.CUST_CODE AND A.ITEM_CODE = TB.ITEM_CODE
                    GROUP BY A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE
                   ";

			var param = new SqlParameter[]
			{
												new SqlParameter("@p0",dcCode),
												new SqlParameter("@p1",gupCode),
												new SqlParameter("@p2",custCode),
												new SqlParameter("@p3",calDate),
												new SqlParameter("@p4",calDate.AddDays(1))
			};
			return SqlQuery<StockSettleData>(sql, param);
		}

		public IQueryable<StockSettleData> GetMoveOutSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                        SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,
                               SUM(ISNULL(J.A_SRC_QTY,0) + ISNULL(L.ADJ_QTY,0)) SRC_QTY
                          FROM F1903 A
                          LEFT JOIN F151001 I ON A.GUP_CODE = I.GUP_CODE AND A.CUST_CODE = I.CUST_CODE AND I.STATUS = '5'
                           AND I.SRC_DC_CODE = @p0 AND I.SRC_DC_CODE <> I.TAR_DC_CODE AND I.POSTING_DATE = @p3
                          LEFT JOIN F151002 J ON I.DC_CODE = J.DC_CODE AND I.GUP_CODE = J.GUP_CODE AND I.CUST_CODE = J.CUST_CODE
                           AND I.ALLOCATION_NO = J.ALLOCATION_NO AND A.ITEM_CODE = J.ITEM_CODE
                          LEFT JOIN F200101 K ON A.GUP_CODE = K.GUP_CODE AND A.CUST_CODE = K.CUST_CODE
                           AND K.DC_CODE = @p0 AND K.ADJUST_TYPE = '1' AND K.STATUS = '0' AND K.CRT_DATE >=@p3 AND K.CRT_DATE < @p4
                          LEFT JOIN F200103 L ON K.DC_CODE = L.DC_CODE AND K.GUP_CODE = L.GUP_CODE AND K.CUST_CODE = L.CUST_CODE
                           AND K.ADJUST_NO = L.ADJUST_NO AND A.ITEM_CODE = L.ITEM_CODE AND L.WORK_TYPE = '1'
                         WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
                         GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE
                ";

			var param = new SqlParameter[]
			{
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
								new SqlParameter("@p3",calDate),
								new SqlParameter("@p4",calDate.AddDays(1))
			};
			return SqlQuery<StockSettleData>(sql, param);
		}

		public IQueryable<StockSettleData> GetMoveInSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
		{
			var sql = @"
                SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,
                       SUM(ISNULL(L.A_TAR_QTY,0) + ISNULL(Y.ADJ_QTY,0)) TAR_QTY
                  FROM F1903 A
                  LEFT JOIN F151001 K ON A.GUP_CODE = K.GUP_CODE AND A.CUST_CODE = K.CUST_CODE
                   AND K.STATUS = '5' AND K.TAR_DC_CODE = @p0 AND K.SRC_DC_CODE <> K.TAR_DC_CODE AND K.POSTING_DATE = @p3
                  LEFT JOIN F151002 L ON K.DC_CODE = L.DC_CODE AND K.GUP_CODE = L.GUP_CODE AND K.CUST_CODE = L.CUST_CODE
                   AND K.ALLOCATION_NO = L.ALLOCATION_NO AND A.ITEM_CODE = L.ITEM_CODE
                  LEFT JOIN F200101 X ON A.GUP_CODE = X.GUP_CODE AND A.CUST_CODE = X.CUST_CODE
                   AND X.DC_CODE = @p0 AND X.ADJUST_TYPE = '1' AND X.STATUS = '0' AND X.CRT_DATE >=@p3 AND X.CRT_DATE < @p4
                  LEFT JOIN F200103 Y ON X.DC_CODE = Y.DC_CODE AND X.GUP_CODE = Y.GUP_CODE AND X.CUST_CODE = Y.CUST_CODE
                   AND X.ADJUST_NO = Y.ADJUST_NO AND A.ITEM_CODE = Y.ITEM_CODE AND Y.WORK_TYPE = '0'
                 WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
                 GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE
                ";

			var param = new SqlParameter[]
			{
								new SqlParameter("@p0",dcCode),
								new SqlParameter("@p1",gupCode),
								new SqlParameter("@p2",custCode),
								new SqlParameter("@p3",calDate),
								new SqlParameter("@p4",calDate.AddDays(1))
			};

			return SqlQuery<StockSettleData>(sql, param);
		}

		public void DeleteAllCustData(string gupCode, string custCode)
		{
			var sql = @" DELETE FROM F1903 
                           WHERE GUP_CODE =@p0 
                             AND CUST_CODE =@p1 ";
			var param = new object[] { gupCode, custCode };
			ExecuteSqlCommand(sql, param);
		}





		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReport(string dcCode, string gupCode, string custCode, string retailCode, List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			string sql = @"
        					SELECT RIGHT('00000'+A.ORD_SEQ, 5) ORD_SEQ,
        						   A.ORD_QTY,
        						   C.A_DELV_QTY,
        						   D.ITEM_CODE,
        						   D.ITEM_NAME,
        						   (SELECT ACC_UNIT_NAME
        							  FROM F91000302
        							 WHERE ITEM_TYPE_ID = '001' AND ACC_UNIT = D.ITEM_UNIT)
        							  ITEM_UNIT,
        						   C.WMS_ORD_NO,
        						   E.SPILT_OUTCHECK,
        						   G.PICK_UNIT,
        						   H.RETAIL_CODE
        					  FROM F050102 A
        						   INNER JOIN F05030101 B
        							  ON     A.DC_CODE = B.DC_CODE
        								 AND A.GUP_CODE = B.GUP_CODE
        								 AND A.CUST_CODE = B.CUST_CODE
        								 AND A.ORD_NO = B.ORD_NO
        						   INNER JOIN F050802 C
        							  ON     A.DC_CODE = C.DC_CODE
        								 AND A.GUP_CODE = C.GUP_CODE
        								 AND A.CUST_CODE = C.CUST_CODE
        								 AND B.WMS_ORD_NO = C.WMS_ORD_NO
        								 AND A.ITEM_CODE = C.ITEM_CODE
        						   INNER JOIN F1903 D
        							  ON     A.CUST_CODE = D.CUST_CODE
        								 AND A.ITEM_CODE = D.ITEM_CODE
        								 AND A.GUP_CODE = D.GUP_CODE
        						   INNER JOIN F1909 E 
        							  ON A.CUST_CODE = E.CUST_CODE
        								 AND A.GUP_CODE = E.GUP_CODE
        						   INNER JOIN F050801 H
        							  ON     A.DC_CODE = H.DC_CODE
        								AND A.GUP_CODE = H.GUP_CODE
        								AND A.CUST_CODE = H.CUST_CODE
        								AND B.WMS_ORD_NO = H.WMS_ORD_NO
        						   LEFT JOIN 
                        (SELECT DISTINCT H.WMS_ORD_NO,G.*  
                           FROM F05120101 G
                           JOIN F051202 H
                             ON H.DC_CODE = G.DC_CODE
                            AND H.GUP_CODE = G.GUP_CODE
                            AND H.CUST_CODE = G.CUST_CODE
                            AND H.PICK_ORD_NO = G.PICK_ORD_NO) G
        							  ON     H.WMS_ORD_NO = G.WMS_ORD_NO
        								AND H.DC_CODE = G.DC_CODE
        								AND H.GUP_CODE = G.GUP_CODE
        								AND H.CUST_CODE = G.CUST_CODE
        					 WHERE     A.DC_CODE   = @p0
        						   AND A.GUP_CODE  = @p1
        						   AND A.CUST_CODE = @p2 
        					";

			if (!string.IsNullOrEmpty(retailCode))
			{
				sql += " AND H.RETAIL_CODE = @p" + parms.Count;
				parms.Add(retailCode);
			}

			sql += parms.CombineSqlInParameters(" AND A.ORD_NO", wmsOrdNos);

			return SqlQuery<RetailDeliverReportDetail>(sql, parms.ToArray());
		}

		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReportByRetail(string dcCode, string gupCode, string custCode, string retailCode, DateTime delvDate)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, delvDate };
			string sql = @"
        					SELECT RIGHT('00000'+A.ORD_SEQ, 5) ORD_SEQ,
        						   A.ORD_QTY,
        						   C.A_DELV_QTY,
        						   D.ITEM_CODE,
        						   D.ITEM_NAME,
        						   (SELECT ACC_UNIT_NAME
        							  FROM F91000302
        							 WHERE ITEM_TYPE_ID = '001' AND ACC_UNIT = D.ITEM_UNIT)
        							  ITEM_UNIT,
        						   C.WMS_ORD_NO,
        						   E.SPILT_OUTCHECK,
        						   G.PICK_UNIT,
        						   H.RETAIL_CODE,H.STATUS
        					  FROM F050102 A
        						   INNER JOIN F05030101 B
        							  ON     A.DC_CODE = B.DC_CODE
        								 AND A.GUP_CODE = B.GUP_CODE
        								 AND A.CUST_CODE = B.CUST_CODE
        								 AND A.ORD_NO = B.ORD_NO
        						   INNER JOIN F050802 C
        							  ON     A.DC_CODE = C.DC_CODE
        								 AND A.GUP_CODE = C.GUP_CODE
        								 AND A.CUST_CODE = C.CUST_CODE
        								 AND B.WMS_ORD_NO = C.WMS_ORD_NO
        								 AND A.ITEM_CODE = C.ITEM_CODE
        						   INNER JOIN F1903 D
        							  ON     A.CUST_CODE = D.CUST_CODE
        								 AND A.ITEM_CODE = D.ITEM_CODE
        								 AND A.GUP_CODE = D.GUP_CODE
        						   INNER JOIN F1909 E
        							  ON A.CUST_CODE = E.CUST_CODE AND A.GUP_CODE = E.GUP_CODE
        						   INNER JOIN F050801 H
        							  ON     A.DC_CODE = H.DC_CODE
        								 AND A.GUP_CODE = H.GUP_CODE
        								 AND A.CUST_CODE = H.CUST_CODE
        								 AND B.WMS_ORD_NO = H.WMS_ORD_NO
        						   LEFT JOIN 
                        (SELECT DISTINCT H.WMS_ORD_NO,G.*  
                           FROM F05120101 G
                           JOIN F051202 H
                             ON H.DC_CODE = G.DC_CODE
                            AND H.GUP_CODE = G.GUP_CODE
                            AND H.CUST_CODE = G.CUST_CODE
                            AND H.PICK_ORD_NO = G.PICK_ORD_NO) G
        							  ON     H.WMS_ORD_NO = G.WMS_ORD_NO
        								 AND H.DC_CODE = G.DC_CODE
        								 AND H.GUP_CODE = G.GUP_CODE
        								 AND H.CUST_CODE = G.CUST_CODE
        					 WHERE     A.DC_CODE = @p0
        						   AND A.GUP_CODE = @p1
        						   AND A.CUST_CODE = @p2
        						   AND H.DELV_DATE = @p3
        						   AND H.STATUS in ('2' ,'5')
        					";

			if (!string.IsNullOrEmpty(retailCode))
			{
				sql += " AND H.RETAIL_CODE = @p" + parms.Count;
				parms.Add(retailCode);
			}
			return SqlQuery<RetailDeliverReportDetail>(sql, parms.ToArray());
		}

		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReportByRetailIntact(string dcCode, string gupCode, string custCode, DateTime arrivalDate, string carPeriod, string delvNo, string retailCode)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, delvNo, carPeriod, arrivalDate };
			string sql = @"
                        SELECT 
	                        A.ITEM_CODE,
                            A.RETAIL_CODE,
                            E.ITEM_NAME,
                            (SELECT ACC_UNIT_NAME
                                FROM F91000302
                                WHERE ITEM_TYPE_ID = '001' AND ACC_UNIT = E.ITEM_UNIT)
                                ITEM_UNIT,
                            F.ORD_QTY,
                            F.A_DELV_QTY,
                            F.WMS_ORD_SEQ ORD_SEQ
                        FROM F055003 A
                            INNER JOIN F050803 B
                                ON     A.DC_CODE = B.DC_CODE
        	                        AND A.GUP_CODE = B.GUP_CODE
        	                        AND A.CUST_CODE = B.CUST_CODE
        	                        AND A.WMS_ORD_NO = B.WMS_ORD_NO
                            INNER JOIN F050801 C
                                ON     C.DC_CODE = B.DC_CODE
        	                        AND C.GUP_CODE = B.GUP_CODE
        	                        AND C.CUST_CODE = B.CUST_CODE
        	                        AND C.WMS_ORD_NO = B.WMS_ORD_NO
                             LEFT JOIN 
														(SELECT DISTINCT H.WMS_ORD_NO,G.*  
															 FROM F05120101 G
															 JOIN F051202 H
																 ON H.DC_CODE = G.DC_CODE
																AND H.GUP_CODE = G.GUP_CODE
																AND H.CUST_CODE = G.CUST_CODE
																AND H.PICK_ORD_NO = G.PICK_ORD_NO) D
                                ON     D.DC_CODE = C.DC_CODE
        	                        AND D.GUP_CODE = C.GUP_CODE
        	                        AND D.CUST_CODE = C.CUST_CODE
        	                        AND D.WMS_ORD_NO = C.WMS_ORD_NO
                            INNER JOIN F1903 E
                                ON     A.GUP_CODE = E.GUP_CODE
        	                        AND A.CUST_CODE = E.CUST_CODE
        	                        AND A.ITEM_CODE = E.ITEM_CODE
                            INNER JOIN F050802 F
                                ON     A.WMS_ORD_NO = F.WMS_ORD_NO
        	                        AND A.ITEM_CODE = F.ITEM_CODE
        	                        AND A.DC_CODE = F.DC_CODE
        	                        AND A.GUP_CODE = F.GUP_CODE
        	                        AND A.CUST_CODE = F.CUST_CODE
                        WHERE     A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND B.DELV_NO = @p3
                            AND B.CAR_PERIOD = @p4
                            AND ISNULL(C.A_ARRIVAL_DATE,C.ARRIVAL_DATE) = @p5
                            AND D.PICK_UNIT = '03'
        						";
			if (!string.IsNullOrEmpty(retailCode))
			{
				sql += " AND B.RETAIL_CODE = @p" + parms.Count;
				parms.Add(retailCode);
			}
			return SqlQuery<RetailDeliverReportDetail>(sql, parms.ToArray());
		}

		public IQueryable<ItemBarcode> GetItemBarcodes(string gupCode, string custCode)
		{
			var parms = new List<object> { gupCode, custCode, gupCode, custCode, gupCode, custCode };
			var sql = @"SELECT DISTINCT A.ITEM_CODE,A.BARCODE
        										FROM (
        										select ITEM_CODE,TRIM(EAN_CODE1) BARCODE
        										from F1903
        										WHERE GUP_CODE = @p0 AND CUST_CODE = @p1 and TRIM(EAN_CODE1) IS NOT NULL
        										UNION ALL
        										select ITEM_CODE,TRIM(EAN_CODE2) BARCODE
        										from F1903
        										WHERE GUP_CODE = @p2 AND CUST_CODE = @p3 and TRIM(EAN_CODE2) IS NOT NULL
        										UNION ALL
        										select ITEM_CODE,TRIM(EAN_CODE3) BARCODE
        										from F1903 
        										WHERE GUP_CODE = @p4 AND CUST_CODE = @p5 and TRIM(EAN_CODE3) IS NOT NULL) A ";
			return SqlQuery<ItemBarcode>(sql, parms.ToArray());
		}

		public IQueryable<P1903Data> GetDataPprocessing1(string cust_code, string gup_code, DateTime upd_date, int rownum)
		{
			string sql = @"SELECT a.CUST_CODE as CustNo,
                                a.ITEM_CODE as ItemNo,
                                a.ITEM_NAME as ItemName ,
                                a.ITEM_UNIT as Unit ,
                                a.ITEM_SIZE as ItemSize,
                                a.CTNS as BoxQty,
                                a.ITEM_COLOR as ItemColor,
                                a.ITEM_SPEC as ItemSpec,
                                b.PACK_LENGTH as ItemLength,
                                b.PACK_WIDTH as ItemWidth,
                                b.PACK_HIGHT as ItemHeight,
																b.PACK_WEIGHT as ItemWeight,
                                CASE WHEN a.BUNDLE_SERIALLOC = '1' THEN '2' WHEN a.BUNDLE_SERIALNO = '1' THEN '1' ELSE '0' END SnType,
                                a.EAN_CODE1 as Barcode1,
                                a.EAN_CODE2 as Barcode2,
                                a.EAN_CODE3 as Barcode3,
                                a.UPD_DATE as UpdDate,
                                a.CRT_DATE as CrtDate
                            FROM F1903 a,F1905 b
                            WHERE a.ITEM_CODE  = b.ITEM_CODE 
                            AND a.GUP_CODE  = b.GUP_CODE 
                            AND a.ITEM_CODE  = b.ITEM_CODE 
                             AND a.CUST_CODE  = b.CUST_CODE 
                            AND a.CUST_CODE  = @p0 
                            AND a.GUP_CODE = @p1
                            AND ISNULL(a.UPD_DATE, a.CRT_DATE) > @p2
                            ORDER BY ISNULL(a.UPD_DATE, a.CRT_DATE)
                            offset (@p3) rows
                            fetch next (1000) rows only";
			var paramers = new[] {
								new SqlParameter("@p0", cust_code),
								new SqlParameter("@p1", gup_code),
								new SqlParameter("@p2", upd_date.ToString("yyyy-MM-dd HH:mm:ss")),
								new SqlParameter("@p3", rownum>1000?rownum-1:rownum)
						};
			var result = SqlQuery<P1903Data>(sql, paramers);
			return result;
		}

		public IQueryable<P1903Data> GetDataPprocessing2(string cust_code, string gup_code, string[] itemNoList)
		{
			string sql = $@"SELECT a.CUST_CODE as CustNo,
                                a.ITEM_CODE as ItemNo,
                                a.ITEM_NAME as ItemName,
                                a.ITEM_UNIT as Unit,
                                a.ITEM_SIZE as ItemSize,
                                a.CTNS as BoxQty,
                                a.ITEM_COLOR as ItemColor,
                                a.ITEM_SPEC as ItemSpec,
                                b.PACK_LENGTH as ItemLength,
                                b.PACK_WIDTH as ItemWidth,
                                b.PACK_HIGHT as ItemHeight,
																b.PACK_WEIGHT as ItemWeight,
                                CASE WHEN a.BUNDLE_SERIALLOC = '1' THEN '2' WHEN a.BUNDLE_SERIALNO = '1' THEN '1' ELSE '0' END SnType,
                                a.EAN_CODE1 as Barcode1,
                                a.EAN_CODE2 as Barcode2,
                                a.EAN_CODE3 as Barcode3,
                                a.UPD_DATE as UpdDate,
                                a.CRT_DATE as CrtDate
                            FROM F1903 a,F1905 b
                            WHERE a.ITEM_CODE = b.ITEM_CODE 
                            AND a.GUP_CODE  = b.GUP_CODE 
                            AND a.ITEM_CODE  = b.ITEM_CODE 
                            AND a.CUST_CODE  = b.CUST_CODE 
                            AND a.CUST_CODE = @p0
                            AND a.GUP_CODE  = @p1 ";


			var paramers = new[] {
								 new SqlParameter("@p0", cust_code),
								new SqlParameter("@p1", gup_code)
						};

			if (itemNoList.Length > 0)
			{
				sql += " AND a.ITEM_CODE IN ('" + string.Join("','", itemNoList) + "') ";
			}

			var result = SqlQuery<P1903Data>(sql, paramers);
			return result;
		}

		#region 原F1902Repository移到F1903Repository

		/// <summary>
		/// 新增退貨單時，可用出貨單號來匯入出貨單的 Item
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo">F050802的出貨單號</param>
		/// <returns></returns>
		public IQueryable<F161201DetailDatas> GetItemsByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			string sql = @" SELECT ROW_NUMBER()OVER(ORDER BY  B.ITEM_CODE,B.GUP_CODE, B.CUST_CODE)ROWNUM,
								   A.WMS_ORD_NO,
								   A.ITEM_CODE,
								   A.GUP_CODE,
								   A.CUST_CODE,
								   A.DC_CODE,
								   B.ITEM_NAME,
								   B.ITEM_SIZE,
								   B.ITEM_SPEC,
								   B.ITEM_COLOR
							  FROM (  SELECT C.WMS_ORD_NO,
											 C.ITEM_CODE,
											 C.GUP_CODE,
											 C.CUST_CODE,
											 C.DC_CODE
										FROM F050802 C
									   WHERE     C.DC_CODE = @p0
											 AND C.GUP_CODE = @p1
											 AND C.CUST_CODE = @p2
											 AND C.WMS_ORD_NO = @p3
									GROUP BY C.WMS_ORD_NO,
											 C.ITEM_CODE,
											 C.GUP_CODE,
											 C.CUST_CODE,
											 C.DC_CODE) A
								   LEFT JOIN F1903 B
									  ON A.ITEM_CODE = B.ITEM_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE";

			return SqlQuery<F161201DetailDatas>(sql, new object[] { dcCode, gupCode, custCode, wmsOrdNo });
		}

		public IQueryable<BomSameItem> GetSameItemWithBom(string gupCode, string custCode, string itemCode)
		{
			var sqlParameters = new[] {
								new SqlParameter("@p0", itemCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode)
						};

			var sql = @"
				Select A.ITEM_CODE, G.BOM_NO, G.BOM_TYPE, G.BOM_QTY
				          From F1903 A, 
				               (Select B.GUP_CODE, B.CUST_CODE, B.BOM_NO, B.CRT_DATE, B.BOM_TYPE, 1 Cnt, B.ITEM_CODE, C.BOM_QTY
				                  From F910101 B, F910102 C
				                 Where B.GUP_CODE=C.GUP_CODE And B.CUST_CODE=C.CUST_CODE And B.BOM_NO=C.BOM_NO
				                   And B.BOM_TYPE='1'
				                   And C.MATERIAL_CODE=@p0
				                   And B.GUP_CODE=@p1
				                   And B.CUST_CODE=@p2
				                 Union
				                Select E.*,D.MATERIAL_CODE ITEM_CODE, D.BOM_QTY
				                  From F910102 D,(Select B.GUP_CODE, B.CUST_CODE, B.BOM_NO, B.CRT_DATE, B.BOM_TYPE, Count(C.MATERIAL_CODE) Cnt
				                                    From F910101 B, F910102 C
				                                   Where B.GUP_CODE=C.GUP_CODE And B.CUST_CODE=C.CUST_CODE And B.BOM_NO=C.BOM_NO
				                                     And B.BOM_TYPE='0'
				                                     And B.ITEM_CODE=@p0
				                                     And B.GUP_CODE=@p1
				                                     And B.CUST_CODE=@p2
				                                   Group By B.GUP_CODE, B.CUST_CODE, B.BOM_NO, B.CRT_DATE, B.BOM_TYPE) E
				                 Where D.GUP_CODE=E.GUP_CODE And D.CUST_CODE=E.CUST_CODE And D.BOM_NO=E.BOM_NO
				                   And E.Cnt=1
				                 ) G
				         Where A.ITEM_CODE=G.ITEM_CODE And A.GUP_CODE=G.GUP_CODE And A.CUST_CODE = G.CUST_CODE
				           And A.ITEM_CODE<>@p0
				           And A.GUP_CODE=@p1
				           And A.EAN_CODE1=(Select EAN_CODE1
				                              From F1903
				                             Where ITEM_CODE=@p0
				                               And GUP_CODE=@p1
				                               And CUST_CODE = @p2)
				           And G.Cnt=1
				         Order By G.CRT_DATE";

			var result = SqlQuery<BomSameItem>(sql, sqlParameters).AsQueryable();
			return result;
		}

		public IQueryable<InventoryItem> GetInventoryItems(string gupCode, string custCode, string type, string lType, string mType, string sType, string vnrCode, string oriVnrCode, string vnrName, string itemCode)
		{
			var param = new List<SqlParameter> {
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode)
						};
			//var param = new List<object> { gupCode, custCode, type, lType };
			var joinF1908 = !string.IsNullOrWhiteSpace(vnrName) ? "JOIN F1908 B ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.VNR_CODE = B.VNR_CODE" : string.Empty;
			var sql = " SELECT ROW_NUMBER()OVER(ORDER BY A.ITEM_CODE,A.ITEM_NAME)ROWNUM,A.ITEM_CODE,A.ITEM_NAME " +
									"   FROM F1903 A " +
									$" {joinF1908} " +
									"  WHERE A.GUP_CODE = @p0 " +
									"    AND A.CUST_CODE = @p1 " +
									"    AND A.SND_TYPE = '0' ";

			if (!string.IsNullOrWhiteSpace(type))
			{
				sql += " AND A.TYPE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, type));
			}
			if (!string.IsNullOrWhiteSpace(lType))
			{
				sql += " AND A.LTYPE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, lType));
			}
			if (!string.IsNullOrWhiteSpace(mType))
			{
				sql += " AND A.MTYPE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, mType));
			}
			if (!string.IsNullOrWhiteSpace(sType))
			{
				sql += " AND A.STYPE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, sType));
			}
			if (!string.IsNullOrWhiteSpace(vnrCode))
			{
				sql += " AND A.VNR_CODE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, vnrCode));
			}
      if (!string.IsNullOrWhiteSpace(oriVnrCode))
      {
        sql += " AND A.ORI_VNR_CODE = @p" + param.Count;
        param.Add(new SqlParameter("@p" + param.Count, oriVnrCode));
      }
      if (!string.IsNullOrWhiteSpace(vnrName))
			{
				sql += $" AND B.VNR_NAME LIKE '%{vnrName}%'";
			}
			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				sql += " AND A.ITEM_CODE = @p" + param.Count;
				param.Add(new SqlParameter("@p" + param.Count, itemCode));
			}

			var result = SqlQuery<InventoryItem>(sql, param.ToArray());
			return result;
		}
		#endregion


		public IQueryable<F1903> GetDatasByBarCode(string gupCode, string custCode, string barcode)
		{
			var parms = new List<object> { gupCode, custCode, barcode, barcode, barcode, barcode };
			var sql = @" SELECT *
                     FROM F1903 
                    WHERE GUP_CODE = @p0
                      AND CUST_CODE = @p1
                      AND (ITEM_CODE = @p2 OR EAN_CODE1 = @p3 OR EAN_CODE2 = @p4 OR EAN_CODE3 =@p5 )";
			return SqlQuery<F1903>(sql, parms.ToArray());
		}

		public IQueryable<F1903WithF161402> GetDataByEanCode(string dcCode,string gupCode,string cuatCode,string returnNo,string barCode)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", cuatCode),
				new SqlParameter("@p3", returnNo),
				new SqlParameter("@p4", barCode),
			};

			var sql = @"SELECT  A.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.ITEM_CODE,A.RTN_QTY,A.AUDIT_QTY FROM F161402 A
						LEFT JOIN F1903 B
						ON A.GUP_CODE = B.GUP_CODE 
						AND A.CUST_CODE  = B.CUST_CODE 
						AND A.ITEM_CODE =B.ITEM_CODE 
						WHERE A.DC_CODE = @p0
						AND A.GUP_CODE = @p1
						AND A.CUST_CODE = @p2
						AND A.RETURN_NO = @p3
						AND (B.ITEM_CODE = @p4 OR B.EAN_CODE1 =@p4 OR B.EAN_CODE2=@p4 OR B.EAN_CODE3=@p4 )";

			return SqlQuery<F1903WithF161402>(sql, parms.ToArray()).AsQueryable();
		}

        public F1903 GetDatasByBarCode(string gupCode, string custCode, string itemCode, string inputItemCode)
        {
            var parms = new List<object> { gupCode, custCode, itemCode, inputItemCode, inputItemCode, inputItemCode };
            var sql = @" SELECT *
                     FROM F1903 
                    WHERE GUP_CODE = @p0
                      AND CUST_CODE = @p1
                      AND ITEM_CODE = @p2
                      AND (EAN_CODE1 = @p3 OR EAN_CODE2 = @p4 OR EAN_CODE3 =@p5 )";
            return SqlQuery<F1903>(sql, parms.ToArray()).FirstOrDefault();
        }
    

		public void UpdateIsAsync(string isAsync, string  gupCode,string custCode, List<string> itemCodes)
		{
			var sql = @"UPDATE F1903
        					SET IS_ASYNC = @p0    
                               WHERE GUP_CODE = @p1
								AND CUST_CODE = @p2";

			var parameters = new List<object>
			{
					isAsync,
					gupCode,
					custCode
			};

			sql += parameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes);
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

    public IQueryable<CommonProduct> GetCommonProductsByCustItemCodes(string gupCode, string custCode, List<string> custItemCodes)
    {
      var sql = @"SELECT GUP_CODE,CUST_CODE,ITEM_CODE,ALL_DLN,CUST_ITEM_CODE,ALLOWORDITEM,BUNDLE_SERIALLOC,BUNDLE_SERIALNO,LOC_MIX_ITEM,
　　　SERIALNO_DIGIT,SERIAL_BEGIN,SERIAL_RULE,SAVE_DAY,PICK_SAVE_QTY,ISCARTON,ISAPPLE,ITEM_NAME,EAN_CODE1,EAN_CODE2,
      EAN_CODE3,EAN_CODE4,TYPE,ITEM_SPEC,TMPR_TYPE,FRAGILE,SPILL,LG,VIRTUAL_TYPE,LTYPE,ITEM_COLOR,ITEM_SIZE,STOP_DATE,NEED_EXPIRED,
	  ALL_SHP,FIRST_IN_DATE,VNR_CODE,IS_EASY_LOSE,IS_PRECIOUS,IS_MAGNETIC,IS_PERISHABLE,IS_TEMP_CONTROL,VNR_ITEM_CODE,RCV_MEMO,
	  ORI_VNR_CODE,MIX_BATCHNO,C_D_FLAG,MAKENO_REQU,CRT_DATE,UPD_DATE
   FROM F1903 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1";

      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
      };

      sql += parameters.CombineSqlInParameters(" AND CUST_ITEM_CODE", custItemCodes, System.Data.SqlDbType.VarChar);
      return SqlQuery<CommonProduct>(sql, parameters.ToArray());

    }

    public bool CheckItemAsync(string gupCode, string custCode, List<string> itemCodes)
    {
      var sql = @"SELECT IS_ASYNC FROM F1903 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1";

      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };

      sql += param.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, System.Data.SqlDbType.VarChar);

      var result = SqlQuery<string>(sql, param.ToArray());

      if (result != null && result.Any() && result.All(o => o == "Y"))
        return true;

      return false;
    }

    public IQueryable<F1903> GetDatasByItems(string gupCode, string custCode, List<string> itemCodes)
    {
      var sql = @"SELECT * FROM F1903 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1";

      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };
      if (itemCodes.Any())
        sql += param.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, System.Data.SqlDbType.VarChar);
      else
        return null;

      return SqlQuery<F1903>(sql, param.ToArray());

      #region 原LINQ語法
      /*
      var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                    && x.CUST_CODE == custCode
                                    && itemCodes.Contains(x.ITEM_CODE));
      return result;
      */
      #endregion
    }

		public IQueryable<CommonProduct> GetCommonProductsByItemCodes(string gupCode, string custCode, List<string> itemCodes)
		{
			var sql = @"SELECT GUP_CODE,CUST_CODE,ITEM_CODE,ALL_DLN,CUST_ITEM_CODE,ALLOWORDITEM,BUNDLE_SERIALLOC,BUNDLE_SERIALNO,LOC_MIX_ITEM,
　　　SERIALNO_DIGIT,SERIAL_BEGIN,SERIAL_RULE,SAVE_DAY,PICK_SAVE_QTY,ISCARTON,ISAPPLE,ITEM_NAME,EAN_CODE1,EAN_CODE2,
      EAN_CODE3,EAN_CODE4,TYPE,ITEM_SPEC,TMPR_TYPE,FRAGILE,SPILL,LG,VIRTUAL_TYPE,LTYPE,ITEM_COLOR,ITEM_SIZE,STOP_DATE,NEED_EXPIRED,
	  ALL_SHP,FIRST_IN_DATE,VNR_CODE,IS_EASY_LOSE,IS_PRECIOUS,IS_MAGNETIC,IS_PERISHABLE,IS_TEMP_CONTROL,VNR_ITEM_CODE,RCV_MEMO,
	  ORI_VNR_CODE,MIX_BATCHNO,C_D_FLAG,MAKENO_REQU,CRT_DATE,UPD_DATE
     FROM F1903 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1";

			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
			};
			if (itemCodes.Any())
				sql += param.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, System.Data.SqlDbType.VarChar);
			else
				return null;

			return SqlQuery<CommonProduct>(sql, param.ToArray());

			#region 原LINQ語法
			/*
      var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                    && x.CUST_CODE == custCode
                                    && itemCodes.Contains(x.ITEM_CODE));
      return result;
      */
			#endregion
		}

		public IQueryable<F1903> GetDatasByWcsItemAsync(string gupCode, string custCode, int maxRecord)
		{
			var sqlParamers = new List<SqlParameter>();
			sqlParamers.Add(new SqlParameter("@p0", gupCode));
			sqlParamers.Add(new SqlParameter("@p1", custCode));

			var sql = $@" SELECT TOP ({maxRecord}) A.*
                            FROM F1903 A
                            JOIN F1903_ASYNC B ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.ITEM_CODE = B.ITEM_CODE
                           WHERE B.GUP_CODE = @p0
                             AND B.CUST_CODE = @p1
                             AND B.IS_ASYNC IN ('N', 'F')
                           ORDER BY B.CRT_DATE ";

			return SqlQuery<F1903>(sql, sqlParamers);
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="itemName">起始字串為模糊查詢</param>
    /// <param name="itemSpec"></param>
    /// <returns></returns>
    public IQueryable<F1903> GetF1903(string gupCode, string custCode, string[] itemCodes, string itemName, string itemSpec, string lType, string oriVnrCode)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
      };

      var sql = @"SELECT * FROM F1903 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1 AND SND_TYPE!='9'";

      if (!string.IsNullOrWhiteSpace(itemSpec))
      {
        sql += $" AND ITEM_SPEC=@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.NVarChar) { Value = itemSpec });
      }

      if (!string.IsNullOrWhiteSpace(itemName))
      {
        sql += $" AND ITEM_NAME LIKE @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.NVarChar) { Value = itemName + "%" });
      }

      if (itemCodes.Any())
      {
        List<string> itemCodesSqls = new List<string>();
        sql += " AND (";
        foreach (var item in itemCodes)
        {
          itemCodesSqls.Add($"(ITEM_CODE=@p{para.Count} OR EAN_CODE1=@p{para.Count} OR EAN_CODE2=@p{para.Count} OR EAN_CODE3=@p{para.Count})");
          para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = item });
        }
        sql += string.Join(" OR", itemCodesSqls);
        sql += ")";
      }

      if (!string.IsNullOrWhiteSpace(oriVnrCode))
      {
        sql += $" AND ORI_VNR_CODE =@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = oriVnrCode });
      }

      return SqlQuery<F1903>(sql, para.ToArray());
      #region 原LINQ
      /*
     var result = _db.F1903s.Where(x => x.SND_TYPE != "9"
                              && x.CUST_CODE == custCode);
     if (!string.IsNullOrWhiteSpace(itemSpec))
     {
       result = result.Where(x => x.ITEM_SPEC == itemSpec);
     }

     if (!string.IsNullOrWhiteSpace(lType))
     {
       result = result.Where(x => x.LTYPE == lType);
     }

     if (!string.IsNullOrWhiteSpace(itemName))
     {
       result = result.Where(x => x.ITEM_NAME.StartsWith(itemName));
     }


     if (itemCodes.Any())
     {
       result = result.Where(x => itemCodes.Contains(x.ITEM_CODE) ||
                                  itemCodes.Contains(x.EAN_CODE1) ||
                                  itemCodes.Contains(x.EAN_CODE2) ||
                                  itemCodes.Contains(x.EAN_CODE3));
     }

     return result;
     // */
      #endregion
    }
		public void UpdateRcvMemo(string gupCode, string custCode, string itemCode, string rcvMemo)
		{
			var sqlParameters = new List<SqlParameter>()
			{
				new SqlParameter("@p0", rcvMemo) { SqlDbType = System.Data.SqlDbType.NVarChar },
				new SqlParameter("@p1", Current.Staff) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", DateTime.Now) { SqlDbType = System.Data.SqlDbType.DateTime2 },
				new SqlParameter("@p3", Current.StaffName) { SqlDbType = System.Data.SqlDbType.NVarChar },
				new SqlParameter("@p4", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p5", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p6", itemCode) { SqlDbType = System.Data.SqlDbType.VarChar },
			};

			var sql = @"UPDATE F1903
                     SET RCV_MEMO = @p0, UPD_STAFF = @p1, UPD_DATE = @p2, UPD_NAME = @p3
                   WHERE GUP_CODE = @p4
                     AND CUST_CODE = @p5
                     AND ITEM_CODE = @p6 ";

			ExecuteSqlCommand(sql, sqlParameters.ToArray());
		}

    public List<WcsSkuCodeModel> GetDatasByWcsSnapshotStocks(string custCode, List<string> itemCodeList)
    {
      int range = 500;
      int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemCodeList.Count()) / range));

      var result = new List<WcsSkuCodeModel>();

      for (int i = 0; i < index; i++)
      {
        var currList = itemCodeList.Skip(i * range).Take(range).ToList();

        var sql = @"
                  SELECT 
                    ITEM_CODE SkuCode, 
                    CUST_CODE OwnerCode, 
                    GUP_CODE GupCode 
                  FROM F1903 
                  WHERE 
                    CUST_CODE = '{0}'
                    AND ITEM_CODE IN({1})
                  ";

        StringBuilder itemSqlIn = new StringBuilder();

        foreach (var item in currList)
        {
          itemSqlIn.Append($"'{item}',");
        }

        itemSqlIn.Remove(itemSqlIn.Length - 1, 1);

        sql = string.Format(sql, custCode, itemSqlIn.ToString());
        result.AddRange(SqlQuery<WcsSkuCodeModel>(sql).ToList());
      }

      return result;
    }
  

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="f1903"></param>
    public void UpdatePostItemData(F1903 f1903)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0",  SqlDbType.Int)       { Value = f1903.VEN_ORD },
        new SqlParameter("@p1",  SqlDbType.SmallInt)  { Value = f1903.ALL_DLN },
        new SqlParameter("@p2",  SqlDbType.VarChar)   { Value = f1903.PICK_WARE },
        new SqlParameter("@p3",  SqlDbType.VarChar)   { Value = f1903.C_D_FLAG },
        new SqlParameter("@p4",  SqlDbType.SmallInt)  { Value = f1903.ALLOW_ALL_DLN },
        new SqlParameter("@p5",  SqlDbType.Char)      { Value = f1903.MULTI_FLAG },
        new SqlParameter("@p6",  SqlDbType.Char)      { Value = f1903.MIX_BATCHNO },
        new SqlParameter("@p7",  SqlDbType.Char)      { Value = f1903.ALLOWORDITEM },
        new SqlParameter("@p8",  SqlDbType.Char)      { Value = f1903.BUNDLE_SERIALLOC },
        new SqlParameter("@p9",  SqlDbType.Char)      { Value = f1903.BUNDLE_SERIALLOC == "1" ? "1" : f1903.BUNDLE_SERIALNO },
        new SqlParameter("@p10", SqlDbType.BigInt)    { Value = f1903.ORD_SAVE_QTY },
        new SqlParameter("@p11", SqlDbType.BigInt)    { Value = f1903.PICK_SAVE_QTY },
        new SqlParameter("@p12", SqlDbType.Char)      { Value = f1903.ITEM_RETURN },
        new SqlParameter("@p13", SqlDbType.Char)      { Value = f1903.LOC_MIX_ITEM },
        new SqlParameter("@p14", SqlDbType.SmallInt)  { Value = f1903.SERIALNO_DIGIT },
        new SqlParameter("@p15", SqlDbType.VarChar)   { Value = f1903.SERIAL_BEGIN },
        new SqlParameter("@p16", SqlDbType.Char)      { Value = f1903.SERIAL_RULE },
        new SqlParameter("@p17", SqlDbType.Int)       { Value = f1903.SAVE_DAY },
        new SqlParameter("@p18", SqlDbType.VarChar)   { Value = f1903.ITEM_STAFF },
        new SqlParameter("@p19", SqlDbType.Decimal)   { Value = f1903.CHECK_PERCENT },
        new SqlParameter("@p20", SqlDbType.Int)       { Value = f1903.PICK_SAVE_ORD },
        new SqlParameter("@p21", SqlDbType.Char)      { Value = f1903.ISCARTON },
        new SqlParameter("@p22", SqlDbType.VarChar)   { Value = f1903.LTYPE },
        new SqlParameter("@p23", SqlDbType.VarChar)   { Value = f1903.MTYPE },
        new SqlParameter("@p24", SqlDbType.VarChar)   { Value = f1903.STYPE },
        new SqlParameter("@p25", SqlDbType.NVarChar)  { Value = f1903.ITEM_NAME },
        new SqlParameter("@p26", SqlDbType.VarChar)   { Value = f1903.EAN_CODE1 },
        new SqlParameter("@p27", SqlDbType.VarChar)   { Value = f1903.EAN_CODE2 },
        new SqlParameter("@p28", SqlDbType.VarChar)   { Value = f1903.ITEM_ENGNAME },
        new SqlParameter("@p29", SqlDbType.NVarChar)  { Value = f1903.ITEM_COLOR },
        new SqlParameter("@p30", SqlDbType.NVarChar)  { Value = f1903.ITEM_SIZE },
        new SqlParameter("@p31", SqlDbType.VarChar)   { Value = f1903.TYPE },
        new SqlParameter("@p32", SqlDbType.SmallInt)  { Value = f1903.ITEM_HUMIDITY },
        new SqlParameter("@p33", SqlDbType.NVarChar)  { Value = f1903.ITEM_SPEC },
        new SqlParameter("@p34", SqlDbType.VarChar)   { Value = f1903.TMPR_TYPE },
        new SqlParameter("@p35", SqlDbType.Char)      { Value = f1903.FRAGILE },
        new SqlParameter("@p36", SqlDbType.Char)      { Value = f1903.SPILL },
        new SqlParameter("@p37", SqlDbType.VarChar)   { Value = f1903.ITEM_UNIT },
        new SqlParameter("@p38", SqlDbType.NVarChar)  { Value = f1903.MEMO },
        new SqlParameter("@p39", SqlDbType.VarChar)   { Value = f1903.PICK_WARE_ID },
        new SqlParameter("@p40", SqlDbType.NVarChar)  { Value = f1903.CUST_ITEM_NAME },
        new SqlParameter("@p41", SqlDbType.Char)      { Value = f1903.MAKENO_REQU },
        new SqlParameter("@p42", SqlDbType.Char)      { Value = f1903.NEED_EXPIRED },
        new SqlParameter("@p43", SqlDbType.Int)       { Value = f1903.ALL_SHP },
        new SqlParameter("@p44", SqlDbType.VarChar)   { Value = f1903.EAN_CODE4 },
        new SqlParameter("@p45", SqlDbType.VarChar)   { Value = f1903.CUST_ITEM_CODE },
        new SqlParameter("@p46", SqlDbType.VarChar)   { Value = f1903.VNR_CODE },
        new SqlParameter("@p47", SqlDbType.Int)       { Value = f1903.RET_ORD },
        new SqlParameter("@p48", SqlDbType.Char)      { Value = f1903.IS_EASY_LOSE },
        new SqlParameter("@p49", SqlDbType.Char)      { Value = f1903.IS_PRECIOUS },
        new SqlParameter("@p50", SqlDbType.Char)      { Value = f1903.IS_MAGNETIC },
        new SqlParameter("@p51", SqlDbType.Char)      { Value = f1903.IS_PERISHABLE },
        new SqlParameter("@p52", SqlDbType.Char)      { Value = f1903.IS_TEMP_CONTROL },
        new SqlParameter("@p53", SqlDbType.VarChar)   { Value = f1903.VNR_ITEM_CODE },
        new SqlParameter("@p54", SqlDbType.VarChar)   { Value = f1903.ORI_VNR_CODE },
        new SqlParameter("@p55", SqlDbType.VarChar)   { Value = Current.Staff },
        new SqlParameter("@p56", SqlDbType.NVarChar)  { Value = Current.StaffName },
        new SqlParameter("@p57", SqlDbType.DateTime2) { Value = DateTime.Now },

        new SqlParameter("@p58", SqlDbType.VarChar)   { Value = f1903.GUP_CODE },
        new SqlParameter("@p59", SqlDbType.VarChar)   { Value = f1903.CUST_CODE },
        new SqlParameter("@p60", SqlDbType.VarChar)   { Value = f1903.ITEM_CODE },
      };

      var sql2 = "";
      if (!string.IsNullOrWhiteSpace(f1903.EAN_CODE3))
      {
        sql2 += $",EAN_CODE3 = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = f1903.EAN_CODE3 } );
      }

      if (f1903.FIRST_IN_DATE.HasValue)
      {
        sql2 += $",FIRST_IN_DATE = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.Date) { Value = f1903.FIRST_IN_DATE });
      }

      var sql = $@"UPDATE F1903 SET 
VEN_ORD = @p0,
ALL_DLN = @p1,
PICK_WARE = @p2,
C_D_FLAG = @p3,
ALLOW_ALL_DLN = @p4,
MULTI_FLAG = @p5,
MIX_BATCHNO = @p6,
ALLOWORDITEM = @p7,
BUNDLE_SERIALLOC = @p8,
BUNDLE_SERIALNO = @p9,
ORD_SAVE_QTY = @p10,
PICK_SAVE_QTY = @p11,
ITEM_RETURN = @p12,
LOC_MIX_ITEM = @p13,
SERIALNO_DIGIT = @p14,
SERIAL_BEGIN = @p15,
SERIAL_RULE = @p16,
SAVE_DAY = @p17,
ITEM_STAFF = @p18,
CHECK_PERCENT = @p19,
PICK_SAVE_ORD = @p20,
ISCARTON = @p21,
LTYPE = @p22,
MTYPE = @p23,
STYPE = @p24,
ITEM_NAME = @p25,
EAN_CODE1 = @p26,
EAN_CODE2 = @p27,
ITEM_ENGNAME = @p28,
ITEM_COLOR = @p29,
ITEM_SIZE = @p30,
TYPE = @p31,
ITEM_HUMIDITY = @p32,
ITEM_SPEC = @p33,
TMPR_TYPE = @p34,
FRAGILE = @p35,
SPILL = @p36,
ITEM_UNIT = @p37,
MEMO = @p38,
PICK_WARE_ID = @p39,
CUST_ITEM_NAME = @p40,
MAKENO_REQU = @p41,
NEED_EXPIRED = @p42,
ALL_SHP = @p43,
EAN_CODE4 = @p44,
CUST_ITEM_CODE = @p45,
VNR_CODE = @p46,
RET_ORD = @p47,
IS_EASY_LOSE = @p48,
IS_PRECIOUS = @p49,
IS_MAGNETIC = @p50,
IS_PERISHABLE = @p51,
IS_TEMP_CONTROL = @p52,
VNR_ITEM_CODE = @p53,
ORI_VNR_CODE = @p54,
IS_ASYNC = 'N',
UPD_STAFF = @p55,
UPD_NAME = @p56,
UPD_DATE = @p57
{sql2}
WHERE GUP_CODE = @p58 AND CUST_CODE=@p59 AND ITEM_CODE = @p60";
      ExecuteSqlCommandWithSqlParameterSetDbType(sql, para.ToArray());
    }

  }
}
