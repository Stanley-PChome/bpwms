using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using System;

namespace Wms3pl.Datas.F06
{
  public partial class F060202Repository
  {
    /// <summary>
    /// 檢查是否有尚未資料匯入的容器 true:無資料該容器可使用 false:有資料該容器不可使用
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public Boolean CheckUnporcessContainerData(string dcCode, string ContainerCode)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", ContainerCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"
SELECT 
  TOP 1 1 
FROM F060205 A
INNER JOIN F060202 B
	ON A.DOC_ID = B.DOC_ID
WHERE 
	B.DC_CODE = @p0
	AND A.CONTAINERCODE = @p1
	AND B.STATUS NOT IN ('2','9')";
      return !SqlQuery<int>(sql, para.ToArray()).Any();
    }

  }
	public partial class F060202Repository
	{

		/// <summary>
		/// 取得自動倉揀貨序號資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="WmsNos"></param>
		/// <returns></returns>
		public IQueryable<AutoAllotSerialNos> GetAutoPickSerialNosByWmsNos(string dcCode, string gupCode, string custCode, List<string> wmsNos)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
			};

			var sql = @"SELECT A.WMS_NO,B.SERIALNUMLIST
										FROM F060202 A 
										INNER JOIN F060203 B
										   ON A.DOC_ID = B.DOC_ID
										WHERE A.DC_CODE=@p0
											AND A.GUP_CODE=@p1
											AND A.CUST_CODE=@p2 ";
			sql += para.CombineSqlInParameters(" AND A.WMS_NO", wmsNos, SqlDbType.VarChar);

			return  SqlQuery<AutoAllotSerialNos>(sql, para.ToArray());
		}
	}
}
