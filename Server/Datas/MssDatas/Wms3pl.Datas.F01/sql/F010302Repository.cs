using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010302Repository : RepositoryBase<F010302, Wms3plDbContext, F010302Repository>
    {
        /// <summary>
        /// 取得昨日之前的碼頭收貨刷貨檔
        /// </summary>
        /// <returns></returns>
        public IQueryable<F010302> GetOldF010302Datas()
        {
            var sql = "SELECT DISTINCT b.* FROM f010302 b LEFT JOIN F010301 a ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO WHERE a.RECV_DATE <= CONVERT(VARCHAR, DATEADD(DAY, -1, dbo.getsysdate()), 111) AND a.id IS NOT NULL";
            return SqlQuery<F010302>(sql);
        }

        public ScanReceiptData GetNewF010302Data()
        {
            var sql = @"SELECT TOP 1 * FROM F010302 ORDER BY ID DESC";
            var result = SqlQuery<ScanReceiptData>(sql).FirstOrDefault();
            result = ConvertCheckStatus(result);
            return result;
        }

        public IQueryable<ScanReceiptData> GetF010302TodayReceiptData(string dcCode, string LogisticCode)
        {
            var sql = @"SELECT a.*,b.LOGISTIC_NAME FROM F010302 a LEFT JOIN F0002 b ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.LOGISTIC_CODE WHERE a.DC_CODE=@p0 AND a.ALL_ID=@p1 ORDER BY a.ID DESC";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=LogisticCode}
            };
            var result = SqlQuery<ScanReceiptData>(sql,para.ToArray());
            result = ConvertCheckStatus(result);
            return result;
        }

        /// <summary>
        /// 把CHECK_STATUS轉成前端可辨認的資料
        /// </summary>
        /// <param name="f010302s"></param>
        /// <returns></returns>
        private static ScanReceiptData ConvertCheckStatus(ScanReceiptData f010302s)
        {
            f010302s.CHECK_STATUS = f010302s.CHECK_STATUS == "0" ? "X" : "";
            return f010302s;
        }
        private static IQueryable<ScanReceiptData> ConvertCheckStatus(IQueryable<ScanReceiptData> f010302s)
        {
            foreach (var item in f010302s)
            {
                item.CHECK_STATUS = item.CHECK_STATUS == "0" ? "X" : "";
            }
            return f010302s;
        }

    }
}
