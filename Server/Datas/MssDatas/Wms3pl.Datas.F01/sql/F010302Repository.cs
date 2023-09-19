using System;
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
            var param = new List<SqlParameter>
            {
              new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };
            var sql = "SELECT DISTINCT b.* FROM f010302 b LEFT JOIN F010301 a ON a.DC_CODE=b.DC_CODE AND a.ALL_ID=b.ALL_ID AND a.SHIP_ORD_NO=b.SHIP_ORD_NO WHERE a.RECV_DATE <= CONVERT(VARCHAR, DATEADD(DAY, -1, @p0), 111) AND a.id IS NOT NULL";
            return SqlQuery<F010302>(sql, param.ToArray());
        }

        public ScanReceiptData GetF010302ByID(long id)
        {
            var param = new List<SqlParameter>()
            {
                new SqlParameter("@p0", id) { SqlDbType = SqlDbType.BigInt }
            };

            var sql = @"SELECT * FROM F010302 WHERE ID = @p0";

            var result = SqlQuery<ScanReceiptData>(sql, param.ToArray()).FirstOrDefault();

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

        public long InsertAndReturnID(ScanReceiptData data)
		    {
			      var param = new List<SqlParameter>
            {
              new SqlParameter("p0", data.DC_CODE) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p1", data.ALL_ID) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p2", data.CHECK_DATE) { SqlDbType = SqlDbType.DateTime2 },
              new SqlParameter("p3", data.CHECK_TIME) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p4", data.CHECK_USER) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p5", data.CHECK_NAME) { SqlDbType = SqlDbType.NVarChar },
              new SqlParameter("p6", data.SHIP_ORD_NO) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p7", data.CHECK_BOX_CNT) { SqlDbType = SqlDbType.SmallInt },
              new SqlParameter("p8", data.SHIP_BOX_CNT) { SqlDbType = SqlDbType.SmallInt },
              new SqlParameter("p9", data.CHECK_STATUS) { SqlDbType = SqlDbType.Char },
              new SqlParameter("p10", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 },
              new SqlParameter("p11", Current.Staff) { SqlDbType = SqlDbType.VarChar },
              new SqlParameter("p12", Current.StaffName) { SqlDbType = SqlDbType.NVarChar }
            };

            var sql = @"
                      DECLARE @a INT;

                      BEGIN
                      INSERT INTO F010302(DC_CODE, ALL_ID, CHECK_DATE, CHECK_TIME, CHECK_USER, CHECK_NAME, SHIP_ORD_NO, CHECK_BOX_CNT, SHIP_BOX_CNT, CHECK_STATUS, CRT_DATE, CRT_STAFF, CRT_NAME)
                      VALUES(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)

                      SELECT @a = CAST(CURRENT_VALUE AS BIGINT)
                      FROM SYS.SEQUENCES  
                      WHERE NAME = 'SEQ_F010302_ID'; 
                      SELECT @a ID
                      END
                      ";

			      return SqlQuery<long>(sql, param.ToArray()).Single();
		    }
    }
}
