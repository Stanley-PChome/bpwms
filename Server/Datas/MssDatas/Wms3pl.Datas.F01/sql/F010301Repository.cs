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
            var sql = "SELECT DISTINCT a.* FROM F010301 a LEFT JOIN f010302 b ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO WHERE a.RECV_DATE <= CONVERT(VARCHAR, DATEADD(DAY, -1, dbo.getsysdate()), 111) AND b.id IS NOT NULL";
            return SqlQuery<F010301>(sql);
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
            /* // 判斷資料是否重複的檢查，經確認後無須檢查此項目
            //var sql = @"SELECT TOP 1 * FROM F010301 WHERE DC_CODE=@p0 AND ALL_ID=@p1 AND SHIP_ORD_NO=@p2";
            //var para = new List<SqlParameter>()
            //{
            //    new SqlParameter("@p0",SqlDbType.VarChar){Value=f010301Data.DC_CODE},
            //    new SqlParameter("@p1",SqlDbType.VarChar){Value=f010301Data.ALL_ID},
            //    new SqlParameter("@p2",SqlDbType.VarChar){Value=f010301Data.SHIP_ORD_NO}
            //};
            //var CheckDupResult = SqlQuery<F010301>(sql, para.ToArray());
            //if (CheckDupResult?.Any() ?? true)
            //    return new ScanCargoData() { ID = -1 };
            */
            f010301Data.CHECK_STATUS = "0"; //在F010302資料寫入、更新才會更動此欄位
            Add(f010301Data);
            _wmsTransaction.Complete();
            var sql = @"SELECT TOP 1 * FROM F010301 ORDER BY ID DESC";
            var result = SqlQuery<ScanCargoData>(sql).First();
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
            var sql =
@"SELECT a.RECV_DATE,c.LOGISTIC_NAME,a.SHIP_ORD_NO,SUM(a.BOX_CNT) BOX_CNT
FROM F010301 a 
LEFT JOIN F010302 AS b ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO 
LEFT JOIN F0002 c ON a.ALL_ID=c.LOGISTIC_CODE
WHERE b.ID IS NULL AND a.DC_CODE=@p0 AND a.ALL_ID=@p1 GROUP BY a.RECV_DATE,a.ALL_ID,c.LOGISTIC_NAME,a.SHIP_ORD_NO;
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
