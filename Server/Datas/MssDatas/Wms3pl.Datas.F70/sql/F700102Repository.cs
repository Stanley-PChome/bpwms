using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700102Repository : RepositoryBase<F700102, Wms3plDbContext, F700102Repository>
	{
		public override void BulkInsert(IEnumerable<F700102> entities, params string[] withoutColumns)
		{
			var fieldsDefaultValue = new Dictionary<string, object>
			{
				{ "ISSEAL", "0" },
				{ "DELV_EFFIC", "01" },
				{ "DELV_TMPR", "A"}
			};
			base.BulkInsert(entities, fieldsDefaultValue, withoutColumns);
		}



		public void DeleteF700102(string wmsNo, string dcCode, string gupCode, string custCode)
		{
			var sqlParamers = new List<object>();
			sqlParamers.Add(wmsNo);
			sqlParamers.Add(dcCode);
			sqlParamers.Add(gupCode);
			sqlParamers.Add(custCode);

			string sql = @"
				 delete from F700102 Where WMS_NO=@p0 and DC_CODE =@p1 and GUP_CODE =@p2 and  CUST_CODE =@p3
			";

			ExecuteSqlCommand(sql, sqlParamers.ToArray());
		}

		/// <summary>
		/// 取得最近的出貨批次時段
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="takeDate"></param>
		/// <returns></returns>
		

		public IQueryable<F700102> GetF700102List(string dcCode, string gupCode, string custCode, DateTime takeDate)
		{
			var sql = @"SELECT A.*
						  FROM F700102 A
							   INNER JOIN F700101 B
								  ON A.DISTR_CAR_NO = B.DISTR_CAR_NO AND A.DC_CODE = B.DC_CODE
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND B.TAKE_DATE = @p3";
			return SqlQuery<F700102>(sql, new object[] { dcCode, gupCode, custCode, takeDate });
		}

		public IQueryable<F700102> GetF700102List(string dcCode, string gupCode, string custCode, DateTime delvDate, string takeTime, string allId)
		{
			var sql = @"
				SELECT A.*
						  FROM F700102 A
							   INNER JOIN F700101 B
								  ON B.DC_CODE = A.DC_CODE AND B.DISTR_CAR_NO = A.DISTR_CAR_NO
							   INNER JOIN F050801 C
								  ON     C.DC_CODE = A.DC_CODE
									 AND C.GUP_CODE = A.GUP_CODE
									 AND C.CUST_CODE = A.CUST_CODE
									 AND C.WMS_ORD_NO = A.WMS_NO
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND B.TAKE_DATE = @p3
							   AND A.TAKE_TIME = @p4
							   AND B.ALL_ID = @p5
							   AND EXISTS												-- 判斷應出貨 == 可出貨
									  (SELECT 1
										 FROM (  SELECT B.DC_CODE,
														B.GUP_CODE,
														B.CUST_CODE,
														B.DELV_DATE,
														B.PICK_TIME,
														C.TAKE_TIME,
														D.ALL_ID,
														COUNT (CASE WHEN ISNULL(B.NO_DELV,'0')='0' THEN B.WMS_ORD_NO ELSE NULL END) LOAD_OWNER_QTY,
														SUM (
														   CASE
															  WHEN  ( B.STATUS = 5
																   OR B.STATUS = 6)
                                  AND ISNULL(B.NO_DELV,'0') = '0'
															  THEN
																 1
															  ELSE
																 0
														   END)
														   LOAD_OWNER_OK_QTY
												   FROM F050801 B
														INNER JOIN F700102 C
														   ON     C.DC_CODE = B.DC_CODE
															  AND C.GUP_CODE = B.GUP_CODE
															  AND C.CUST_CODE = B.CUST_CODE
															  AND C.WMS_NO = B.WMS_ORD_NO
														INNER JOIN F700101 D
														   ON     D.DC_CODE = C.DC_CODE
															  AND D.DISTR_CAR_NO = C.DISTR_CAR_NO
												  WHERE B.VIRTUAL_ITEM = '0'
                            AND ISNULL(B.NO_DELV,'0') ='0'
                            AND NOT EXISTS                                            -- 過濾已經取消的訂單
                                          (SELECT 1
                                             FROM F05030101 P
                                                  INNER JOIN F050301 Q
                                                     ON     Q.DC_CODE = P.DC_CODE
                                                        AND Q.GUP_CODE = P.GUP_CODE
                                                        AND Q.CUST_CODE = P.CUST_CODE
                                                        AND Q.ORD_NO = P.ORD_NO
                                            WHERE     P.DC_CODE = B.DC_CODE
                                                  AND P.GUP_CODE = B.GUP_CODE
                                                  AND P.CUST_CODE = B.CUST_CODE
                                                  AND P.WMS_ORD_NO = B.WMS_ORD_NO
                                                  AND Q.PROC_FLAG = '9')
											   GROUP BY B.DC_CODE,
														B.GUP_CODE,
														B.CUST_CODE,
														B.DELV_DATE,
														B.PICK_TIME,
														C.TAKE_TIME,
														D.ALL_ID) D
										WHERE     D.LOAD_OWNER_QTY = D.LOAD_OWNER_OK_QTY
											  AND D.LOAD_OWNER_QTY > 0
											  AND D.DC_CODE = A.DC_CODE
											  AND D.GUP_CODE = A.GUP_CODE
											  AND D.CUST_CODE = A.CUST_CODE
											  AND D.DELV_DATE = C.DELV_DATE
											  AND D.PICK_TIME = C.PICK_TIME
											  AND D.TAKE_TIME = A.TAKE_TIME
											  AND D.ALL_ID = B.ALL_ID)
							   AND NOT EXISTS											-- 過濾已經取消的訂單
										  (SELECT 1
											 FROM F05030101 P
												  INNER JOIN F050301 Q
													 ON     Q.DC_CODE = P.DC_CODE
														AND Q.GUP_CODE = P.GUP_CODE
														AND Q.CUST_CODE = P.CUST_CODE
														AND Q.ORD_NO = P.ORD_NO
											WHERE     P.DC_CODE = C.DC_CODE
												  AND P.GUP_CODE = C.GUP_CODE
												  AND P.CUST_CODE = C.CUST_CODE
												  AND P.WMS_ORD_NO = C.WMS_ORD_NO
												  AND Q.PROC_FLAG = '9')		
                ";
			var param = new object[] { dcCode, gupCode, custCode, delvDate, takeTime, allId };
			return SqlQuery<F700102>(sql, param);
		}

		public IQueryable<P080901ShipReport> GetF050801WithF700102sForReport(string dcCode, string gupCode, string custCode, DateTime takeDate, string takeTime, string allId)
		{
			var parameterList = new List<object> { dcCode, gupCode, custCode, takeDate,takeTime,allId };

			string sql = @" 
                   SELECT 
	ROW_NUMBER()OVER(Order by B.DELV_DATE,B.PICK_TIME,A.CONSIGN_NO ) ROWNUM,
	A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.TAKE_DATE,C.TAKE_TIME,
                             B.ALL_ID,E.ALL_COMP,C.CAR_NO_A,C.CAR_NO_B,C.CAR_NO_C, B.DELV_DATE,B.PICK_TIME,
                             A.CONSIGN_NO,A.BOXQTY,B.WMS_ORD_NO,B.STATUS
												FROM F050901 A
												JOIN F050801 B
												  ON B.DC_CODE = A.DC_CODE
												 AND B.GUP_CODE = A.GUP_CODE
												 AND B.CUST_CODE = A.CUST_CODE
												 AND B.WMS_ORD_NO = A.WMS_NO
												JOIN F700102 C
												  ON C.DC_CODE = B.DC_CODE
												 AND C.GUP_CODE = B.GUP_CODE
												 AND C.CUST_CODE = B.CUST_CODE
												 AND C.WMS_NO = B.WMS_ORD_NO
												JOIN F700101 D
												  ON D.DC_CODE = C.DC_CODE
												 AND D.DISTR_CAR_NO   = C.DISTR_CAR_NO
												JOIN F1947 E
												  ON E.DC_CODE = B.DC_CODE
												 AND E.ALL_ID = B.ALL_ID
											 WHERE B.VIRTUAL_ITEM = '0' --排除虛擬商品
												 AND B.DC_CODE = @p0
											 	 AND B.GUP_CODE = @p1
												 AND B.CUST_CODE = @p2
												 AND ISNULL(B.NO_DELV,'0') = '0' --排除不裝車
												 AND NOT EXISTS	-- 過濾訂單已取消的出貨單
													(SELECT 1
															FROM F05030101 P
																	INNER JOIN F050301 Q
																			ON     Q.DC_CODE = P.DC_CODE
																				AND Q.GUP_CODE = P.GUP_CODE
																				AND Q.CUST_CODE = P.CUST_CODE
																				AND Q.ORD_NO = P.ORD_NO
														WHERE     P.DC_CODE = B.DC_CODE
																	AND P.GUP_CODE = B.GUP_CODE
																	AND P.CUST_CODE = B.CUST_CODE
																	AND P.WMS_ORD_NO = B.WMS_ORD_NO
																	AND Q.PROC_FLAG = '9')
													AND D.TAKE_DATE =@p3
                          AND C.TAKE_TIME = @p4
                          AND B.ALL_ID = @p5   
";

			return SqlQuery<P080901ShipReport>(sql, parameterList.ToArray());
		}


		public IQueryable<F050801WithF700102> GetF050801WithF700102s(string dcCode, string gupCode, string custCode, DateTime takeDate, string takeTime, string allId, bool checkWmsStatus)
		{
			var parameterList = new List<object> { dcCode, gupCode, custCode, takeDate };

			string sql = @"
							SELECT E.*, F.ALL_COMP, G.NEED_SEAL
							  FROM (  SELECT B.DC_CODE,
											 B.GUP_CODE,
											 B.CUST_CODE,
											 B.DELV_DATE,
											 B.PICK_TIME,
											 C.TAKE_TIME,
											 C.CAR_NO_A,
											 C.CAR_NO_B,
											 C.CAR_NO_C,
											 C.ISSEAL,
											 D.ALL_ID,
											 D.TAKE_DATE,
											 COUNT (CASE WHEN ISNULL(B.NO_DELV,'0')='0' THEN B.WMS_ORD_NO ELSE NULL END) LOAD_OWNER_QTY,
											 SUM (
												CASE
												   WHEN (B.STATUS = 5 OR B.STATUS = 6) AND ISNULL(B.NO_DELV,'0') ='0' THEN 1
												   ELSE 0
												END)
												LOAD_OWNER_OK_QTY,
											 SUM (B.PackgeBoxCount) PACKAGE_BOX_NO				-- 取得相同批次日期時段、出車時段的所有出貨單的包裝總數
										FROM (SELECT A.*,
													 (SELECT COUNT (*)
														FROM F055001 Z							-- 取得該出貨單的包裝總數
													   WHERE     A.WMS_ORD_NO = Z.WMS_ORD_NO
															 AND A.DC_CODE = Z.DC_CODE
															 AND A.GUP_CODE = Z.GUP_CODE
															 AND A.CUST_CODE = Z.CUST_CODE)
														PackgeBoxCount
												FROM F050801 A									-- 批次日期、批次時段
											   WHERE     A.VIRTUAL_ITEM = '0'
													 AND A.DC_CODE = @p0
													 AND A.GUP_CODE = @p1
													 AND A.CUST_CODE = @p2
													 AND NOT EXISTS								-- 過濾訂單已取消的出貨單
															(SELECT 1
															   FROM F05030101 P
																	INNER JOIN F050301 Q
																	   ON     Q.DC_CODE = P.DC_CODE
																		  AND Q.GUP_CODE = P.GUP_CODE
																		  AND Q.CUST_CODE = P.CUST_CODE
																		  AND Q.ORD_NO = P.ORD_NO
															  WHERE     P.DC_CODE = A.DC_CODE
																	AND P.GUP_CODE = A.GUP_CODE
																	AND P.CUST_CODE = A.CUST_CODE
																	AND P.WMS_ORD_NO = A.WMS_ORD_NO
																	AND Q.PROC_FLAG = '9')) B
											 INNER JOIN F700102 C								-- 出車時段
												ON     C.DC_CODE = B.DC_CODE
												   AND C.GUP_CODE = B.GUP_CODE
												   AND C.CUST_CODE = B.CUST_CODE
												   AND C.WMS_NO = B.WMS_ORD_NO
											 INNER JOIN F700101 D								-- 取得配送商ID用
												ON     D.DC_CODE = C.DC_CODE
												   AND D.DISTR_CAR_NO = C.DISTR_CAR_NO
											WHERE D.TAKE_DATE = @p3 "
                                            + parameterList.CombineNotNullOrEmpty(" AND C.TAKE_TIME = @p{0} ", takeTime)
											+ parameterList.CombineNotNullOrEmpty(" AND D.ALL_ID = @p{0} ", allId)
											+ @"
									GROUP BY B.DC_CODE,
											 B.GUP_CODE,
											 B.CUST_CODE,
											 B.DELV_DATE,
											 B.PICK_TIME,
											 C.TAKE_TIME,
											 C.CAR_NO_A,
											 C.CAR_NO_B,
											 C.CAR_NO_C,
											 C.ISSEAL,
											 D.ALL_ID,
											 D.TAKE_DATE) E
								   INNER JOIN F1947 F ON E.DC_CODE = F.DC_CODE AND E.ALL_ID = F.ALL_ID  -- 取得配送商名稱
								   INNER JOIN F1909 G													-- 取得出車是否需加貼封條(0否1是)
									  ON E.GUP_CODE = G.GUP_CODE AND E.CUST_CODE = G.CUST_CODE
									WHERE 1=1
			";

			// 除了裝車查詢以外，都會加入這行過濾條件
			if (checkWmsStatus)
				sql += " AND E.LOAD_OWNER_QTY = E.LOAD_OWNER_OK_QTY AND E.LOAD_OWNER_QTY > 0 ";
			return SqlQuery<F050801WithF700102>(sql, parameterList.ToArray());
		}




	}
}