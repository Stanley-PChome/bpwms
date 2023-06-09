
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
	public partial class F1511Repository : RepositoryBase<F1511, Wms3plDbContext, F1511Repository>
	{
        /// <summary>
        /// 將訂單編號關聯的虛擬儲位的狀態與實際揀貨量做取消。
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="ordNoList"></param>
        public void UpdateStatusCancel(string gupCode, string custCode, string dcCode, IEnumerable<string> ordNoList)
        {
            var parameters = new List<object>
            {
                Current.Staff,
                Current.StaffName,
                gupCode,
                custCode,
                dcCode
            };


            int paramStartIndex = parameters.Count;
            var inSql = parameters.CombineSqlInParameters("A.ORD_NO", ordNoList, ref paramStartIndex);
            var sql = @"UPDATE F1511 SET A_PICK_QTY = 0, STATUS = '9', UPD_STAFF = @p0, UPD_NAME = @p1, UPD_DATE = dbo.GetSysDate()
						WHERE EXISTS (
						SELECT ORDER_NO, ORDER_SEQ, A.DC_CODE, A.GUP_CODE, A.CUST_CODE
						FROM 
						F05030101 A,F050301 B , F050801 D, F051202 F, F1511 G
						WHERE A.ORD_NO = B.ORD_NO
							AND A.CUST_CODE = B.CUST_CODE 
						  AND A.GUP_CODE = B.GUP_CODE 
							AND A.DC_CODE = B.DC_CODE
							AND A.WMS_ORD_NO = D.WMS_ORD_NO
							AND A.CUST_CODE = D.CUST_CODE 
							AND A.GUP_CODE = D.GUP_CODE 
							AND A.DC_CODE = D.DC_CODE
							AND D.WMS_ORD_NO = F.WMS_ORD_NO
							AND A.CUST_CODE = F.CUST_CODE 
							AND A.GUP_CODE = F.GUP_CODE 
							AND A.DC_CODE = F.DC_CODE
							AND F.PICK_ORD_NO = G.ORDER_NO
							AND F.PICK_ORD_SEQ = G.ORDER_SEQ
							AND A.CUST_CODE = G.CUST_CODE 
							AND A.GUP_CODE = G.GUP_CODE
							AND A.DC_CODE = G.DC_CODE
              AND F1511.ORDER_NO = ORDER_NO
							AND F1511.ORDER_SEQ = ORDER_SEQ
							AND F1511.DC_CODE = A.DC_CODE
							AND F1511.GUP_CODE = A.GUP_CODE
							AND F1511.CUST_CODE = A.CUST_CODE
							AND A.GUP_CODE = @p2                                         -- 以下為查詢過濾條件
							AND A.CUST_CODE = @p3
							AND A.DC_CODE = @p4
							AND " + inSql + " )";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void DeleteDatas(string ordNo, string gupCode, string custCode, string dcCode)
        {
            var parameters = new List<object>
            {
                ordNo,
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				Delete From F1511 
				 Where ORDER_NO=@p0
					 And GUP_CODE = @p1
					 And CUST_CODE = @p2
					 And DC_CODE = @p3";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void UpdateDatasForCancel(string ordNo, string gupCode, string custCode, string dcCode)
        {
            var parameters = new List<object>
            {
                ordNo,
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				UPDATE  F1511  SET STATUS='9'
				 Where ORDER_NO=@p0
					 And GUP_CODE = @p1
					 And CUST_CODE = @p2
					 And DC_CODE = @p3";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void DeleteData(string dcCode, string gupCode, string custCode, string ordNo, string ordSeq)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", ordNo),
                new SqlParameter("@p4", ordSeq)
            };
            var sql = " DELETE " +
                            "   FROM F1511 " +
                            "  WHERE DC_CODE = @p0 " +
                            "    AND GUP_CODE = @p1 " +
                            "    AND CUST_CODE = @p2 " +
                            "    AND ORDER_NO = @p3 " +
                            "    AND ORDER_SEQ = @p4 ";
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void UpdateData(string dcCode, string gupCode, string custCode, string ordNo, string ordSeq, long bPickQty, long aPickQty, string status)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", ordNo),
                new SqlParameter("@p4", ordSeq),
                new SqlParameter("@p5", bPickQty),
                new SqlParameter("@p6", aPickQty),
                new SqlParameter("@p7", Current.Staff),
                new SqlParameter("@p8", Current.StaffName),
                new SqlParameter("@p9", status),
            };
            var sql = @"
						UPDATE F1511
						   SET B_PICK_QTY = @p5,
							   A_PICK_QTY = @p6,
							   UPD_STAFF = @p7,
							   UPD_NAME = @p8,
							   UPD_DATE = dbo.GetSysDate(),
							   STATUS = @p9
						 WHERE     DC_CODE = @p0
							   AND GUP_CODE = @p1
							   AND CUST_CODE = @p2
							   AND ORDER_NO = @p3
							   AND ORDER_SEQ = @p4";
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        /// <summary>
		/// 將該出貨單關聯的虛擬儲位(F1511)狀態設為已扣帳(2)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		public void SetAlreadyDebitByWmsOrdNos(string dcCode, string gupCode, string custCode, IEnumerable<string> wmsOrdNos)
        {
            var paramList = new List<object> { Current.Staff, Current.StaffName, dcCode, gupCode, custCode };

            int paramStartIndex = paramList.Count;
            var inSql = paramList.CombineSqlInParameters("A.WMS_ORD_NO", wmsOrdNos, ref paramStartIndex);

            var sql = $@"UPDATE F1511
						SET STATUS = '2'
							, UPD_DATE =dbo.GetSysDate()  
							, UPD_STAFF = @p0  
							, UPD_NAME = @p1 
                        FROM (SELECT C.DC_CODE,C.GUP_CODE ,C.CUST_CODE ,C.ORDER_NO ORDER_NO,C.ORDER_SEQ ORDER_SEQ
                                      FROM F050801 A
                                           JOIN F051202 B
                                              ON     A.WMS_ORD_NO = B.WMS_ORD_NO
                                                 AND A.GUP_CODE = B.GUP_CODE
                                                 AND A.CUST_CODE = B.CUST_CODE
                                                 AND A.DC_CODE = B.DC_CODE
                                          JOIN F1511 C  
                                              ON     B.PICK_ORD_NO = C.ORDER_NO
                                           AND B.PICK_ORD_SEQ = C.ORDER_SEQ
                                           AND B.GUP_CODE = C.GUP_CODE
                                           AND B.CUST_CODE = C.CUST_CODE
                                           AND B.DC_CODE = C.DC_CODE
                                      WHERE C.DC_CODE = @p2
                                           AND C.GUP_CODE = @p3
                                           AND C.CUST_CODE = @p4
                                           AND {inSql} ) D
                                        WHERE F1511.DC_CODE  = D.DC_CODE
                                    AND F1511.GUP_CODE = D.GUP_CODE
                                    AND F1511.CUST_CODE = D.CUST_CODE
                                    AND F1511.ORDER_NO = D.ORDER_NO
                                    AND F1511.ORDER_SEQ = D.ORDER_SEQ";

            ExecuteSqlCommand(sql, paramList.ToArray());
        }

    

        public void BulkDeleteData(string gupCode, string custCode, string dcCode, List<string> ordNos)
        {
            var parameters = new List<object>
            {
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				Delete From F1511 
				 Where   GUP_CODE = @p0
					 And CUST_CODE = @p1
					 And DC_CODE = @p2";
            sql += parameters.CombineSqlInParameters(" AND ORDER_NO ", ordNos);
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void UpdateDatasForCancel(string gupCode, string custCode, string dcCode, List<string> ordNos)
        {
            var parameters = new List<object>
            {
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				UPDATE  F1511  SET STATUS='9'
				 Where   GUP_CODE = @p0
					 And CUST_CODE = @p1
					 And DC_CODE = @p2";

            sql += parameters.CombineSqlInParameters(" AND ORDER_NO ", ordNos);
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        

        public void UpdateAPickQtyForWcsApi(string dcCode, string gupCode, string custCode, string wmsNo, string rowNum, int skuQty)
        {
            var paramList = new List<object> { skuQty, Current.Staff, Current.StaffName, dcCode, gupCode, custCode, wmsNo, rowNum };

            var sql = @"UPDATE F1511
						   SET STATUS = '2', A_PICK_QTY = @p0
						    , UPD_DATE = dbo.GetSysDate()  
							, UPD_STAFF = @p1  
							, UPD_NAME = @p2
						 WHERE     DC_CODE = @p3
							   AND GUP_CODE = @p4
							   AND CUST_CODE = @p5
							   AND ORDER_NO = @p6
							   AND ORDER_SEQ = @p7
                    ";

            ExecuteSqlCommand(sql, paramList.ToArray());
        }

				public IQueryable<F1511> GetDatas(string dcCode, string gupCode, string custCode, List<string> orderNos)
				{
					var parms = new List<object> { dcCode, gupCode, custCode };
					var sql = @" SELECT *
														 FROM F1511
														WHERE DC_CODE = @p0
															AND GUP_CODE = @p1
															AND CUST_CODE = @p2 ";
					sql += parms.CombineNotNullOrEmptySqlInParameters(" AND ORDER_NO", orderNos);

					return SqlQuery<F1511>(sql,parms.ToArray());
				}

		public IQueryable<F1511> GetDatas(string dcCode, string gupCode, string custCode, string orderNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode,orderNo };
			var sql = @" SELECT *
														 FROM F1511
														WHERE DC_CODE = @p0
															AND GUP_CODE = @p1
															AND CUST_CODE = @p2 
                              AND ORDER_NO = @p3";

			return SqlQuery<F1511>(sql, parms.ToArray());
		}

		public IQueryable<F1511> GetDatasByWmsOrdNo(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @" SELECT A.*
										FROM F1511 A
                    JOIN F051202 B
                      ON B.DC_CODE = A.DC_CODE
                     AND B.GUP_CODE = A.GUP_CODE
                     AND B.CUST_CODE = A.CUST_CODE
                     AND B.PICK_ORD_NO = A.ORDER_NO
                     AND B.PICK_ORD_SEQ = A.ORDER_SEQ
										WHERE B.DC_CODE = @p0
											AND B.GUP_CODE = @p1
											AND B.CUST_CODE = @p2 
											AND B.WMS_ORD_NO = @p3";

			return SqlQuery<F1511>(sql, parms.ToArray());
		}

		public IQueryable<F1511> GetDatasByWmsOrdNos(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT *
                     FROM F1511 A
                     JOIN F051202 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.ORDER_NO
                      AND B.PICK_ORD_SEQ = A.ORDER_SEQ
                    WHERE B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2 ";
			if (!wmsOrdNos.Any())
				sql += " AND 1=0 ";
			else
				sql += parms.CombineSqlInParameters("AND B.WMS_ORD_NO", wmsOrdNos);
			return SqlQuery<F1511>(sql, parms.ToArray());
		}

	}
}
