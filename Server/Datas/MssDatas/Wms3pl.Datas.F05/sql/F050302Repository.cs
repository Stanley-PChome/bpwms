using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
  public partial class F050302Repository : RepositoryBase<F050302, Wms3plDbContext, F050302Repository>
  {
    public void DeleteLackOrder(string gupCode, string custCode)
    {
      var sql = @" DELETE X
                         FROM F050302 X
                         WHERE EXISTS(SELECT A.GUP_CODE,A.CUST_CODE,A.ORD_NO
                          FROM F050301 A
                          WHERE A.PROC_FLAG ='0'
                          AND A.GUP_CODE = X.GUP_CODE
                          AND A.CUST_CODE = X.CUST_CODE
                          AND A.ORD_NO = X.ORD_NO)
                          AND X.GUP_CODE = @p0
                          AND X.CUST_CODE = @p1";
      ExecuteSqlCommand(sql, new object[] { gupCode, custCode });
    }

    /// <summary>
    /// 撿缺配庫時檢查原始訂單是否有指定序號用
    /// </summary>
    /// <param name="f05120601"></param>
    /// <returns></returns>
    public F050302 GetDataByF05120601SerialNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string serialNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", wmsOrdNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", serialNo) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
SELECT A.* 
FROM F050302 AS A
INNER JOIN F05030101 AS B
	ON A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.ORD_NO = B.ORD_NO 
WHERE 
A.DC_CODE = @p0
AND A.GUP_CODE = @p1
AND A.CUST_CODE = @p2
AND B.WMS_ORD_NO = @p3
AND A.SERIAL_NO = @p4";
      return SqlQuery<F050302>(sql, para.ToArray()).SingleOrDefault();
    }

    /// <summary>
    /// 揀缺配庫排程前將商品狀態改為序號綁儲位狀態，查詢可拆明細的項目
    /// </summary>
    /// <param name="f05120601"></param>
    /// <returns></returns>
    public IQueryable<F050302> GetDatasByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", wmsOrdNo) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
SELECT A.* 
FROM F050302 AS A
INNER JOIN F05030101 AS B
	ON A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.ORD_NO = B.ORD_NO 
WHERE 
A.DC_CODE = @p0
AND A.GUP_CODE = @p1
AND A.CUST_CODE = @p2
AND B.WMS_ORD_NO = @p3";
      return SqlQuery<F050302>(sql, para.ToArray());
    }

		public IQueryable<OrderWithMakeNo> GetMakeNosByOrdNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p3", ordNo) { SqlDbType = SqlDbType.VarChar },
			};

			var sql = @" SELECT ORD_NO, ORD_SEQ, MAKE_NO
                           FROM F050302
                          WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND ORD_NO = @p3 ";

			return SqlQuery<OrderWithMakeNo>(sql, para.ToArray());
		}
	}
}
