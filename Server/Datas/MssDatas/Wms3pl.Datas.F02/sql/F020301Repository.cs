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
namespace Wms3pl.Datas.F02
{
    public partial class F020301Repository : RepositoryBase<F020301, Wms3plDbContext, F020301Repository>
    {
        public int GetF020301FileSeq(string dcCode, string gupCode, string custCode, string purchaseNo, string shopNo)
        {
            string sql = @"			

						SELECT TOP (1) CONVERT(INT,SUBSTRING(A.FILE_NAME,LEN(A.FILE_NAME)-1,2)) SEQCTN
						FROM F020301 A 
						WHERE	A.DC_CODE =@p0 
								AND A.GUP_CODE =@p1 
								AND A.CUST_CODE =@p2
								AND (A.PURCHASE_NO =@p3 or A.PURCHASE_NO = @p4) 
                ORDER BY CRT_DATE DESC
			";

            var param = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p3", purchaseNo) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p4", shopNo) { SqlDbType = SqlDbType.VarChar }
            };

            return SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();
        }

        /// <summary>
        /// 取得待匯入的進倉驗收明細檔，其中品號為可選擇性過濾條件
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="itemCode">有填才過濾</param>
        /// <returns></returns>
        public IQueryable<F020302Data> GetF020302Data(string dcCode, string gupCode, string custCode, string purchaseNo, string itemCode = "")
        {
            List<SqlParameter> prms = new List<SqlParameter>();
            prms.Add(new SqlParameter("@p0",dcCode));
            prms.Add(new SqlParameter("@p1",gupCode));
            prms.Add(new SqlParameter("@p2",custCode));
            prms.Add(new SqlParameter("@p3", purchaseNo));
            var sql = @"SELECT B.*
						  FROM F020301 A
							   JOIN F020302 B
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.FILE_NAME = B.FILE_NAME
							   JOIN F010201 C
								  ON     C.DC_CODE = A.DC_CODE
									 AND C.GUP_CODE = A.GUP_CODE
									 AND C.CUST_CODE = A.CUST_CODE
									 AND B.PO_NO = C.SHOP_NO
						 WHERE     B.STATUS = '0'
							   AND A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND C.STOCK_NO = @p3
							    ";
            if (!string.IsNullOrEmpty(itemCode))
            {
                sql += @" AND B.ITEM_CODE = @p4";
                prms.Add(new SqlParameter("@p4", itemCode));
            }
            return SqlQuery<F020302Data>(sql, prms.ToArray());
        }

        public IQueryable<F020302> GetF020302s(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var parameter = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p3", purchaseNo) { SqlDbType = SqlDbType.VarChar },
            };

            var sql = @"SELECT B.*
						  FROM F020301 A
							   JOIN F020302 B
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.FILE_NAME = B.FILE_NAME
							   JOIN F010201 C
								  ON     C.DC_CODE = A.DC_CODE
									 AND C.GUP_CODE = A.GUP_CODE
									 AND C.CUST_CODE = A.CUST_CODE
									 AND B.PO_NO = C.SHOP_NO
						 WHERE     B.STATUS = '0'
							   AND A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND C.STOCK_NO = @p3";

            return SqlQuery<F020302>(sql, parameter.ToArray());
        }

        public IQueryable<P020205Main> GetJincangNoFileMain(string dcCode, string gupCode, string custCode, DateTime importStartDate, DateTime importEndDate, string poNo)
        {
            var paramList = new List<object> { dcCode, gupCode, custCode, importStartDate, importEndDate };

            // 查詢結果只顯示檔名
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A.FILE_NAME ASC) AS [ROWNUM],
							   A.FILE_NAME,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   '' PO_NO
						  FROM F020301 A
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND CONVERT(DATE,A.CRT_DATE) BETWEEN @p3 AND @p4 
							   ";
            sql += paramList.CombineNotNullOrEmpty(@"AND EXISTS
													  (SELECT 1
														 FROM F020302 B
														WHERE     B.PO_NO = @p{0}
															  AND A.DC_CODE = B.DC_CODE
															  AND A.GUP_CODE = B.GUP_CODE
															  AND A.CUST_CODE = B.CUST_CODE
															  AND A.FILE_NAME = B.FILE_NAME)", poNo);
            return SqlQuery<P020205Main>(sql, paramList.ToArray());
        }
        public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> stockNos)
        {
            var sql = @" DELETE FROM F020301
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
						var parms = new List<object> { dcCode,gupCode,custCode };
						sql += parms.CombineSqlInParameters("AND PURCHASE_NO", stockNos);
						ExecuteSqlCommand(sql, parms.ToArray());
        }

        public void Delete(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p3", purchaseNo) { SqlDbType = SqlDbType.VarChar },
            };
            var sql = @" DELETE FROM F020301
										WHERE DC_CODE = @p0
										AND GUP_CODE = @p1
										AND CUST_CODE = @p2
										AND PURCHASE_NO = @p3 
										AND NOT EXISTS
										(SELECT 1
										 FROM F020302
										 WHERE F020301.DC_CODE = F020302.DC_CODE
										 AND F020301.GUP_CODE = F020302.GUP_CODE
										 AND F020301.CUST_CODE = F020302.CUST_CODE
										 AND F020301.FILE_NAME = F020302.FILE_NAME) ";
            ExecuteSqlCommand(sql, parameter.ToArray());
        }

        public void CancelNotProcessWarehouseInF020301(string dcCode, string gupCode, string custCode, string stockNo)
        {
            string sql = @"
				           update F020301 set STATUS = '9', UPD_DATE = @p6, UPD_STAFF = @p0, UPD_NAME = @p1
                           Where DC_CODE =@p2
				             and GUP_CODE =@p3
				             and CUST_CODE =@p4
                             and PURCHASE_NO=@p5
                             and STATUS <> '9'
			               ";
            var sqlParams = new SqlParameter[]
            {
                 new SqlParameter("@p0", Current.Staff),
                 new SqlParameter("@p1", Current.StaffName),
                 new SqlParameter("@p2", dcCode),
                 new SqlParameter("@p3", gupCode),
                 new SqlParameter("@p4", custCode),
                 new SqlParameter("@p5", stockNo),
                 new SqlParameter("@p6", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
            };

            ExecuteSqlCommand(sql, sqlParams);
        }
    }
}
