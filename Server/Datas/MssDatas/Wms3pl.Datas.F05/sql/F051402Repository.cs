using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051402Repository : RepositoryBase<F051402, Wms3plDbContext, F051402Repository>
	{
    public IQueryable<CollectionRecord> GetCollectionRecord(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar){Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar){Value = gupCode },
        new SqlParameter("@p2",SqlDbType.VarChar){Value = custCode },
        new SqlParameter("@p3",SqlDbType.VarChar){Value = wmsNo }
      };

      var sql = $@"
                SELECT
                  COLLECTION_CODE,
                  CELL_CODE,
                  WMS_ORD_NO,
                  CONTAINER_CODE,
                  (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F051402' AND SUBTOPIC='STATUS' AND VALUE=F051402.STATUS AND LANG='zh-TW') STATUS,
                  CRT_NAME,
                  CRT_DATE
                  FROM F051402
                WHERE
                  DC_CODE = @p0
                  AND GUP_CODE = @p1
                  AND CUST_CODE = @p2
                  AND WMS_ORD_NO = @p3
                ORDER BY
                  CRT_DATE
                ";

      return SqlQuery<CollectionRecord>(sql, parms.ToArray());
    }
  }
}
