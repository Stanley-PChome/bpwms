using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
  public partial class F05080505Repository : RepositoryBase<F05080505, Wms3plDbContext, F05080505Repository>
  {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="calNo"></param>
    /// <param name="flag">查詢模式 0:手動挑單試算 1:配庫試算結果查詢</param>
    /// <returns></returns>
    public IQueryable<F05080505Data> GetF05080505Datas(string dcCode, string gupCode, string custCode, string calNo, string flag)
    {
      List<SqlParameter> para = new List<SqlParameter>()
      {
          new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
          new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
          new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
          new SqlParameter("@p3",SqlDbType.VarChar){Value=calNo},
      };

      var dataModeQuery = "";
      if (flag == "0")
        dataModeQuery = "AND EXISTS (SELECT 1 FROM F050001 e WHERE a.DC_CODE=e.DC_CODE AND a.GUP_CODE=e.GUP_CODE AND a.CUST_CODE=e.CUST_CODE AND a.ORD_NO=e.ORD_NO)";
      var sql = $@"
SELECT a.ORD_NO,a.CUST_ORD_NO,b.NAME CUST_COST,c.NAME FAST_DEAL_TYPE,d.CROSS_NAME MOVE_OUT_TARGET,a.WAREHOUSE_INFO,CASE WHEN a.IS_LACK_ORDER='1' THEN '是' ELSE NULL END IS_LACK_ORDER 
FROM F05080505 a 
LEFT JOIN VW_F000904_LANG b 
  ON b.TOPIC ='F050101' AND b.SUBTOPIC='CUST_COST' AND a.CUST_COST =b.VALUE AND b.LANG='zh-tw' 
LEFT JOIN VW_F000904_LANG c 
  ON c.TOPIC ='F050101' AND c.SUBTOPIC='FAST_DEAL_TYPE' AND a.FAST_DEAL_TYPE =c.VALUE AND c.LANG='zh-tw' 
LEFT JOIN F0001 d 
  ON a.MOVE_OUT_TARGET = d.CROSS_CODE 
WHERE 
	a.DC_CODE =@p0
	AND a.GUP_CODE =@p1
	AND a.CUST_CODE =@p2
	AND a.CAL_NO =@p3
  {dataModeQuery}
";
      var result = SqlQuery<F05080505Data>(sql, para.ToArray());
      return result;
    }
  }
}
