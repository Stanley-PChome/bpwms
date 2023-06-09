using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  public partial class F070104Repository
  {
    public IQueryable<F070104> GetDatasByF0701Ids(List<long> f0701Ids)
    {
      var para = new List<SqlParameter>();
      var sql = @"SELECT * FROM F070104 WHERE";
      if (f0701Ids.Any())
        sql += para.CombineSqlInParameters(" F0701_ID", f0701Ids, System.Data.SqlDbType.BigInt);
      else
        sql += "1=0";
      var result = SqlQuery<F070104>(sql, para.ToArray());
      return result;
    }

    public IQueryable<ContainerMixModel> GetCollectionMixDatas(long f0701Id)
    {
      var parms = new object[] { f0701Id };

      var sql = $@"SELECT 
												 A.STATUS,
                         A.WMS_NO, 
                         B.ITEM_CODE, 
                         B.CUST_ITEM_CODE, 
                         B.ITEM_NAME,
                         SUM(A.QTY) QTY
                         FROM F070104 A
                         JOIN F1903 B
                         ON A.GUP_CODE = B.GUP_CODE
                         AND A.CUST_CODE = B.CUST_CODE
                         AND A.ITEM_CODE = B.ITEM_CODE
                         WHERE A.F0701_ID = @p0
                         GROUP BY A.STATUS, A.WMS_NO, B.ITEM_CODE, B.CUST_ITEM_CODE, B.ITEM_NAME
									";

            var result = SqlQuery<ContainerMixModel>(sql, parms.ToArray());
            return result;
        }

    public string LockF070104()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F070104';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public long GetF070104NextId()
    {
      var sql = @"SELECT NEXT VALUE FOR SEQ_F070104_ID";

      return SqlQuery<long>(sql).Single();
    }

  }
}
