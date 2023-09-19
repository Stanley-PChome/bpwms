using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010301Repository : RepositoryBase<F010301, Wms3plDbContext, F010301Repository>
    {
        public IQueryable<F010301> GetOldF010301Datas()
        {
            var param = new List<SqlParameter>
            {
              new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            var sql = "SELECT DISTINCT a.* FROM F010301 a LEFT JOIN f010302 b ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO WHERE a.RECV_DATE <= CONVERT(VARCHAR, DATEADD(DAY, -1, @p0), 111) AND b.id IS NOT NULL";

            return SqlQuery<F010301>(sql, param.ToArray());
        }

        public IQueryable<VW_F010301> GetALLVMF010301(string DcCode, DateTime? RecvDateS, DateTime? RecvDateE, string AllId, string EmpID, string CheckStatus, string ShipOrdNo)
        {
            var para = new List<SqlParameter>();
            para.Add(new SqlParameter("@p0", SqlDbType.VarChar) { Value = DcCode });
            StringBuilder sql = new StringBuilder(
            @"
            SELECT
	            *
            FROM
	            VW_F010301
            WHERE
                DC_CODE=@p0
            ");

            if (RecvDateS.HasValue && RecvDateE.HasValue)
            {
                sql.Append($" AND RECV_DATE BETWEEN {"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.DateTime2) { Value = RecvDateS.Value.Date });
                sql.Append($" AND {"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.DateTime2) { Value = RecvDateE.Value.Date.AddDays(1).AddSeconds(-1) });
            }

            if (!string.IsNullOrWhiteSpace(AllId))
            {
                sql.Append($" AND ALL_ID={"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.VarChar) { Value = AllId });
            }

            if (!string.IsNullOrWhiteSpace(EmpID))
            {
                sql.Append($" AND RECV_USER={"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.VarChar) { Value = EmpID });
            }

            if (!string.IsNullOrWhiteSpace(CheckStatus))
            {
                sql.Append($" AND CHECK_STATUS={"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.Char) { Value = CheckStatus });
            }

            if (!string.IsNullOrWhiteSpace(ShipOrdNo))
            {
                sql.Append($" AND SHIP_ORD_NO={"@p" + para.Count}");
                para.Add(new SqlParameter("@p" + para.Count, SqlDbType.VarChar) { Value = ShipOrdNo });
            }
            sql.Append($" ORDER BY RECV_DATE,ALL_ID");
            var result = SqlQuery<VW_F010301>(sql.ToString(), para.ToArray());
            foreach (var item in result)
                item.CHECK_STATUS = item.CHECK_STATUS == "0" ? "X" : "O";

            return result;
        }

        public ScanCargoData InsertF010301AndGetNewID(F010301 f010301Data)
        {
            var param = new List<SqlParameter>
            {
              new SqlParameter("p0", f010301Data.DC_CODE) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p1", f010301Data.ALL_ID) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p2", f010301Data.RECV_DATE) { SqlDbType = SqlDbType.DateTime2 },
              new SqlParameter("p3", f010301Data.RECV_TIME) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p4", f010301Data.RECV_USER) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p5", f010301Data.RECV_NAME) { SqlDbType = SqlDbType.NVarChar },
              new SqlParameter("p6", f010301Data.SHIP_ORD_NO) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p7", f010301Data.BOX_CNT) { SqlDbType = SqlDbType.SmallInt },
              new SqlParameter("p8", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 },
              new SqlParameter("p9", Current.Staff) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p10", Current.StaffName) { SqlDbType = SqlDbType.NVarChar }
            };

            var sql = @"
                      DECLARE @a INT;

                      BEGIN
                      INSERT INTO F010301(DC_CODE, ALL_ID, RECV_DATE, RECV_TIME, RECV_USER, RECV_NAME, SHIP_ORD_NO, BOX_CNT, CHECK_STATUS, CRT_DATE, CRT_STAFF, CRT_NAME)
                      VALUES(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, '0', @p8, @p9, @p10)

                      SELECT @a = CAST(CURRENT_VALUE AS BIGINT)
                      FROM SYS.SEQUENCES  
                      WHERE NAME = 'SEQ_F010301_ID'; 
                      SELECT @a ID
                      END
                      ";

            var f010301ID = SqlQuery<long>(sql, param.ToArray()).First();

            _wmsTransaction.Complete();

            var param2 = new List<SqlParameter>
            {
              new SqlParameter("@p0", f010301ID) { SqlDbType = SqlDbType.BigInt }
            };

            var sql2 = @"SELECT * FROM F010301 WHERE ID = @p0";

            var result = SqlQuery<ScanCargoData>(sql2, param2.ToArray()).First();

            if (result != null)
                return result;
            else
                return new ScanCargoData() { ID = -9999 };
        }

        public IQueryable<ScanCargoData> GetF010301UncheckedScanCargoDatas(string dcCode, string LogisticCode, string RecvUser)
        {
            var sql = @"SELECT a.*,b.LOGISTIC_NAME,CAST(0 AS bit) IsSelected FROM F010301 a LEFT JOIN F0002 b ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.LOGISTIC_CODE WHERE a.CHECK_STATUS=0 AND a.DC_CODE=@p0 AND a.ALL_ID=@p1";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=LogisticCode}
            };

            if (!string.IsNullOrWhiteSpace(RecvUser))
            {
                sql += $" AND RECV_USER=@p{para.Count}";
                para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = RecvUser });
            }

            return SqlQuery<ScanCargoData>(sql, para.ToArray());
        }

        public IQueryable<ScanCargoStatistic> GetF010301ScanCargoStatistic(string dcCode, string LogisticCode)
        {
            var sql = @"SELECT DC_CODE,ALL_ID,SHIP_ORD_NO,COUNT(*) ORD_CNT,SUM(BOX_CNT) BOX_QTY FROM F010301 WHERE DC_CODE=@p0 AND ALL_ID=@p1 GROUP BY DC_CODE,ALL_ID,SHIP_ORD_NO";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=LogisticCode}
            };
            var result = SqlQuery<ScanCargoStatistic>(sql, para.ToArray());
            return result;
        }

        /// <summary>
        /// 查詢碼頭收貨作業-刷單作業-未核貨單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="LogisticCode"></param>
        /// <returns></returns>
        public IQueryable<ReceiptUnCheckData> GetF010301UncheckReceiptShipOrdNo(string dcCode, string LogisticCode)
        {
            var sql = @"
                      SELECT 
                        a.RECV_DATE,
                        c.LOGISTIC_NAME,
                        a.SHIP_ORD_NO,SUM(a.BOX_CNT) BOX_CNT
                      FROM F010301 a 
                        LEFT JOIN F010302 AS b 
                          ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO 
                        LEFT JOIN F0002 c 
                          ON a.DC_CODE=c.DC_CODE AND a.ALL_ID=c.LOGISTIC_CODE
                      WHERE 
                        b.ID IS NULL 
                        AND a.DC_CODE=@p0 
                        AND a.ALL_ID=@p1 
                      GROUP BY 
                        a.RECV_DATE,
                        a.ALL_ID,
                        c.LOGISTIC_NAME,
                        a.SHIP_ORD_NO;
                      ";

            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=LogisticCode}
            };

            var result = SqlQuery<ReceiptUnCheckData>(sql, para.ToArray());
            return result;
        }
    }
}
