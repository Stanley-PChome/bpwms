using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F052903Repository : RepositoryBase<F052903, Wms3plDbContext, F052903Repository>
	{
		public IQueryable<F052903> GetDatasByPick(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
			var sql = @" SELECT  *
                      FROM F052903
                     WHERE DC_CODE = @p0
                       AND GUP_CODE = @p1
                       AND CUST_CODE = @p2
                       AND PICK_ORD_NO = @p3";
			return SqlQuery<F052903>(sql, parms);
		}


		public IQueryable<F052903> GetDatasFromF051301(string dcCode,string gupCode,string custCode,string pickOrdNo,List<string> wmsOrdNos)
		{
			var parms = new List<object> { pickOrdNo, dcCode, gupCode, custCode };
			var sql = @"SELECT @p0 PICK_ORD_NO,WMS_NO AS WMS_ORD_NO,
                         ROW_NUMBER() OVER(ORDER BY WMS_NO) PICK_LOC_NO,
                         NULL CONTAINER_CODE,DC_CODE,GUP_CODE,CUST_CODE,NEXT_STEP,COLLECTION_CODE,'0' STATUS,
												CRT_DATE,CRT_STAFF,CRT_NAME,NULL AS UPD_DATE,NULL AS UPD_STAFF,NULL AS UPD_NAME 
									 FROM F051301
                   WHERE DC_CODE = @p1
                     AND GUP_CODE = @p2
                     AND CUST_CODE = @p3";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_NO", wmsOrdNos);
			return SqlQuery<F052903>(sql, parms.ToArray());
		}
	}
}
