using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F070102Repository
	{
        public IQueryable<UseShipContainerItemModel> GetContainerItemData(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parms = new object[] { dcCode, gupCode, custCode, wmsOrdNo };

            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", wmsOrdNo));

            var sql = $@"SELECT SUM(B.QTY) QTY, B.ITEM_CODE,D.ALLOWORDITEM,D.BUNDLE_SERIALNO
                         FROM F070101 A
                         JOIN F070102 B
                         ON B.F070101_ID = A.ID
                         JOIN F1903 D
                         ON D.GUP_CODE = B.GUP_CODE
                         AND D.CUST_CODE = B.CUST_CODE
                         AND D.ITEM_CODE = B.ITEM_CODE
												 JOIN F0701 F 
                         ON A.F0701_ID =F.ID 
                         WHERE A.DC_CODE = @p0
                         AND A.GUP_CODE = @p1
                         AND A.CUST_CODE = @p2
                         AND A.WMS_NO = @p3
                         GROUP BY B.ITEM_CODE,D.ALLOWORDITEM,D.BUNDLE_SERIALNO
						";

            var result = SqlQuery<UseShipContainerItemModel>(sql, parms.ToArray());
            return result;
        }
		    
		    public IQueryable<F070102> GetDatasByF0701Id(long f0701Id)
				{
			      var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", f0701Id) { SqlDbType = SqlDbType.BigInt });
			       var sql = @" SELECT *
                          FROM F070101 A
                          JOIN F070102 B
                            ON B.F070101_ID = A.ID
                         WHERE A.F0701_ID = @p0";
			     return SqlQuery<F070102>(sql,parm.ToArray());
				}

		public IQueryable<PickContainerDetail> GetContainerDetailByF0701Ids(List<long> f0701Ids)
		{
			var param = new List<SqlParameter>();

			var sql = @" SELECT F070101.F0701_ID,
                                F070101.PICK_ORD_NO,
                                F070102.ITEM_CODE,
                                F1903.BUNDLE_SERIALNO,
                                F070102.SERIAL_NO,
                                F070102.QTY
                          FROM F070101
                          JOIN F070102 ON F070102.F070101_ID = F070101.ID
                          JOIN F1903 With(nolock)
                            ON F1903.GUP_CODE = F070102.GUP_CODE
                           AND F1903.CUST_CODE = F070102.CUST_CODE
                           AND F1903.ITEM_CODE = F070102.ITEM_CODE
                         WHERE ";
			sql += param.CombineSqlInParameters(" F070101.F0701_ID ", f0701Ids, SqlDbType.BigInt);

			return SqlQuery<PickContainerDetail>(sql, param.ToArray());
		}
	}
}
