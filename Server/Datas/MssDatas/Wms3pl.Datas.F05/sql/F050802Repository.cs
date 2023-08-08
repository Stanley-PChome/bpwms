using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050802Repository : RepositoryBase<F050802, Wms3plDbContext, F050802Repository>
	{


		public IQueryable<F050802ItemName> GetShippingItem(string dcCode, string gupCode, string custCode, string itemCodes, string wmsOrderNos)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", itemCodes),
						};
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY tmp.WMS_ORD_NO, tmp.GUP_CODE, tmp.CUST_CODE, tmp.DC_CODE ASC) ROWNUM, tmp.*
						FROM 
                            (SELECT a.WMS_ORD_NO,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,a.ITEM_CODE,b.ITEM_NAME,ISNULL(G.ACC_UNIT_NAME,' ') RET_UNIT
                            FROM F050802 a 
                            LEFT JOIN F1903 b on a.ITEM_CODE = b.ITEM_CODE AND a.GUP_CODE = b.GUP_CODE AND a.CUST_CODE = b.CUST_CODE
                            LEFT JOIN F91000302 G ON G.ITEM_TYPE_ID = '001' AND G.ACC_UNIT = b.ITEM_UNIT
							WHERE a.DC_CODE = @p0 
							  AND a.GUP_CODE = @p1
							  AND a.CUST_CODE = @p2
							  AND 
							  	 (b.ITEM_CODE = @p3 
							   OR b.EAN_CODE1 = @p3
							   OR b.EAN_CODE2 = @p3
							   OR b.EAN_CODE3 = @p3) ";

			if (!string.IsNullOrEmpty(wmsOrderNos))
			{
				var wmsOrderNoList = wmsOrderNos.Split(',').ToList();
				var insSqlList = new List<string>();
				foreach (var item in wmsOrderNoList)
				{
					insSqlList.Add(string.Format("@p{0}", parameters.Count));
					parameters.Add(new SqlParameter(string.Format("@p{0}", parameters.Count), item));
				}
				sql += string.Format(" AND a.WMS_ORD_NO IN ({0})", string.Join(",", insSqlList.ToArray()));
			}

			sql += @"GROUP BY a.WMS_ORD_NO,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,a.ITEM_CODE,b.ITEM_NAME,G.ACC_UNIT_NAME) tmp";

			var result = SqlQuery<F050802ItemName>(sql, parameters.ToArray());

			return result;
		}

		public IQueryable<F050802> GetDatas(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
			var sql = @" SELECT *
                     FROM F050802
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND WMS_ORD_NO = @p3 ";
			return SqlQuery<F050802>(sql, parms.ToArray());
		}

        public IQueryable<ItemModel> GetDatasByShipPackage(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };
            var sql = @" SELECT 
                        B.ITEM_CODE ItemCode,
                        B.ITEM_NAME ItemName,
                        B.ALLOWORDITEM IsOriItem,
                        ISNULL(B.EAN_CODE1, '') EanCode1,
                        ISNULL(B.EAN_CODE2, '') EanCode2,
                        ISNULL(B.EAN_CODE3, '') EanCode3,
                        B.BUNDLE_SERIALNO BundleSerialNo,
                        B.BUNDLE_SERIALLOC BundleSerialLoc
                        FROM F050802 A
                        JOIN F1903 B
                        ON A.GUP_CODE = B.GUP_CODE
                        AND A.CUST_CODE = B.CUST_CODE
                        AND A.ITEM_CODE = B.ITEM_CODE
                        WHERE A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.WMS_ORD_NO = @p3
                        ";
            return SqlQuery<ItemModel>(sql, parms.ToArray());
        }
    }
}
