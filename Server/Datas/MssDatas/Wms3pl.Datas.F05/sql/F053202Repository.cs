using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F053202Repository : RepositoryBase<F053202, Wms3plDbContext, F053202Repository>
	{
    public IQueryable<F053202Ex> GetF053202Ex(long F0531_ID)
    {
      var para = new List<SqlParameter> { new SqlParameter("@p0", F0531_ID) { SqlDbType = SqlDbType.BigInt } };

      var sql = @"SELECT 
	                  A.ITEM_CODE,
	                  B.ITEM_NAME,
	                  SUM(A.QTY) QTY
                  FROM F053202 A
                  LEFT JOIN F1903 B WITH(NOLOCK)
	                  ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE  AND A.ITEM_CODE = B.ITEM_CODE 
                  WHERE 
	                  A.F0531_ID = @p0
                  GROUP BY
                    A.ITEM_CODE, B.ITEM_NAME";

      return SqlQuery<F053202Ex>(sql, para.ToArray());
    }

		public IQueryable<F053202> GetDataByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT * FROM F053202
						 WHERE F0531_ID = @p0
						";

			return SqlQuery<F053202>(sql, sqlParameter.ToArray());
		}

		public IQueryable<MoveOutContainerDtl> GetMoveOutDtlByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new List<SqlParameter>();
			parms.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT F0532.OUT_CONTAINER_CODE, F0532.OUT_CONTAINER_SEQ,F0532.SOW_TYPE, F053202.F0531_ID, F053202.F0701_ID, F053202.ITEM_CODE, F053202.SERIAL_NO, SUM(F053202.QTY) QTY, F053202.WMS_ORD_NO, F053202.WMS_ORD_SEQ
                      FROM F053202
                      JOIN F0532 ON F0532.F0531_ID = F053202.F0531_ID
                                AND F0532.DC_CODE = F053202.DC_CODE
                                AND F0532.GUP_CODE = F053202.GUP_CODE
                                AND F0532.CUST_CODE = F053202.CUST_CODE
                     WHERE F053202.DC_CODE = @p0
                       AND F053202.GUP_CODE = @p1
                       AND F053202.CUST_CODE = @p2
                       AND F053202.PICK_ORD_NO = @p3
                     GROUP BY F0532.OUT_CONTAINER_CODE, F0532.OUT_CONTAINER_SEQ,F0532.SOW_TYPE, F053202.F0531_ID, F053202.F0701_ID, F053202.ITEM_CODE, F053202.SERIAL_NO, F053202.WMS_ORD_NO, F053202.WMS_ORD_SEQ
                     ORDER BY  F053202.SERIAL_NO DESC,F053202.WMS_ORD_NO DESC, F053202.F0531_ID, F053202.ITEM_CODE";

			return SqlQuery<MoveOutContainerDtl>(sql, parms.ToArray());
		}
	}
}
