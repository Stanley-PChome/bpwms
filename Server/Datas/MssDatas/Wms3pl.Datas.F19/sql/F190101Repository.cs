using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F190101Repository : RepositoryBase<F190101, Wms3plDbContext, F190101Repository>
  {
    #region P1905 權限功能
    /// <summary>
    /// 傳回貨主所屬的物流中心 (F190101), 包含DC層
    /// </summary>
    /// <returns></returns>
    public IQueryable<F190101Data> GetF190101MappingTable(string dcCode, string gupId)
    {
      var sql = @"SELECT A.DC_CODE, C.DC_NAME, A.CUST_CODE, B.CUST_NAME, B.GUP_CODE
				  FROM F190101 A
				  join F1909 B on  A.CUST_CODE = B.CUST_CODE
				  LEFT join  F1901 C on A.DC_CODE = C.DC_CODE
				   WHERE (B.GUP_CODE = @p0 or @p0 ='')
           AND (A.DC_CODE = @p1 or @p1 ='')
				 UNION ALL SELECT ' ', N' ', DC_CODE, DC_NAME, ' ' 
				  FROM F1901
         WHERE (DC_CODE = @p1 or @p1 ='')
			";
      var param = new[] {
                new SqlParameter("@p0", gupId),
                new SqlParameter("@p1", dcCode),
            };

      var result = SqlQuery<F190101Data>(sql, param).AsQueryable();

      return result;
    }
    #endregion

    public IQueryable<F190101> GetF190101With192402Data1(string account, string dcCode)
    {
      var param = new List<SqlParameter>
            {
                new SqlParameter("@p0",SqlDbType.VarChar){ Value = account},
                new SqlParameter("@p1",SqlDbType.VarChar){ Value = dcCode}
            };

      var sql = @"SELECT * FROM F190101 A
                        JOIN F192402 B
                        ON A.DC_CODE = B.DC_CODE AND B.GUP_CODE = B.GUP_CODE AND B.EMP_ID = @p0
                        WHERE A.DC_CODE = @p1" ;


      return SqlQuery<F190101>(sql, param.ToArray());
    }

    public IQueryable<F190101> GetF190101With192402Data2(string account, string dcCode)
    {
      var param = new List<SqlParameter>
            {
                new SqlParameter("@p0",SqlDbType.VarChar){ Value = account},
                new SqlParameter("@p1",SqlDbType.VarChar){ Value = dcCode}
            };

      var sql = @"SELECT * FROM F190101 A
                        JOIN F192402 B
                        ON A.DC_CODE = B.DC_CODE AND B.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND B.EMP_ID = @p0
                        WHERE A.DC_CODE = @p1";


      return SqlQuery<F190101>(sql, param.ToArray());
    }
  }
}
