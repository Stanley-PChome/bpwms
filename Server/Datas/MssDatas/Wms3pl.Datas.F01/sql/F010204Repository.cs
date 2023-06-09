using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
  public partial class F010204Repository : RepositoryBase<F010204, Wms3plDbContext, F010204Repository>
  {

    public F010204 GetData(string dcCode,string gupCode,string custCode,string stockNo,int stockSeq)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3",stockNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4",stockSeq) { SqlDbType = SqlDbType.Int }
      };
      var sql = @"SELECT * FROM F010204 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND STOCK_NO=@p3 AND STOCK_SEQ=@p4";
      return SqlQuery<F010204>(sql, para.ToArray()).FirstOrDefault();
    }
  }
}
