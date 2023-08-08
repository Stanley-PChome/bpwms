
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
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
      var parms = new List<SqlParameter> {
            new SqlParameter("@p0",SqlDbType.VarChar){Value= pickOrdNo },
            new SqlParameter("@p1",SqlDbType.VarChar){Value= dcCode },
            new SqlParameter("@p2",SqlDbType.VarChar){Value= gupCode },
            new SqlParameter("@p3",SqlDbType.VarChar){Value= custCode },
            new SqlParameter("@p4",SqlDbType.DateTime2){ Value = DateTime.Now },
                                };
      //var parms = new List<object> { pickOrdNo, dcCode, gupCode, custCode };
      var sql = @"SELECT @p0 PICK_ORD_NO,WMS_NO AS WMS_ORD_NO,
                         ROW_NUMBER() OVER(ORDER BY WMS_NO) PICK_LOC_NO,
                         NULL CONTAINER_CODE,DC_CODE,GUP_CODE,CUST_CODE,NEXT_STEP,COLLECTION_CODE,'0' STATUS,
												@p4 'CRT_DATE' ,CRT_STAFF,CRT_NAME,NULL AS UPD_DATE,NULL AS UPD_STAFF,NULL AS UPD_NAME 
									 FROM F051301
                   WHERE DC_CODE = @p1
                     AND GUP_CODE = @p2
                     AND CUST_CODE = @p3";
      //sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_NO", wmsOrdNos, SqlDbType.VarChar);

      sql +=  parms.CombineSqlInParameters("and WMS_NO", wmsOrdNos, SqlDbType.VarChar);
			return SqlQuery<F052903>(sql, parms.ToArray());
		}
	}
}
