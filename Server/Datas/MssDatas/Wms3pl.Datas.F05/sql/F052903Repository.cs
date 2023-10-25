
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

    public IQueryable<DivideInfo> GetDivideInfo(string dcCode, string gupCode, string custCode, string wmsNo)
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
                  PICK_ORD_NO,
                  WMS_ORD_NO,
                  CONTAINER_CODE,
                  (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F052903' AND SUBTOPIC='NEXT_STEP' AND VALUE=F052903.NEXT_STEP AND LANG='{Current.Lang}') NEXT_STEP,
                  (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F052903' AND SUBTOPIC='STATUS' AND VALUE=F052903.STATUS AND LANG='{Current.Lang}') STATUS
                FROM F052903
                WHERE
                  DC_CODE = @p0
                  AND GUP_CODE = @p1
                  AND CUST_CODE = @p2
                  AND PICK_ORD_NO IN (SELECT DISTINCT PICK_ORD_NO FROM F051202 WHERE WMS_ORD_NO = @p3)
                ";

      return SqlQuery<DivideInfo>(sql, parms.ToArray());
    }

    public IQueryable<DivideDetail> GetDivideDetail(string dcCode, string gupCode, string custCode, string wmsNo)
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
                  A.PICK_ORD_NO,
                  B.PICK_ORD_SEQ,
                  A.PICK_LOC_NO,
                  A.CONTAINER_CODE,
                  B.ITEM_CODE,
                  B.B_SET_QTY,
                  B.A_SET_QTY,
                  B.UPD_NAME,
                  B.CRT_DATE,
                  B.UPD_DATE
                FROM F052903 A
                JOIN F05290301 B
                  ON A.DC_CODE=B.DC_CODE 
                  AND A.GUP_CODE=B.GUP_CODE 
                  AND A.CUST_CODE=B.CUST_CODE 
                  AND A.PICK_ORD_NO=B.PICK_ORD_NO 
                  AND A.WMS_ORD_NO=B.WMS_ORD_NO
                WHERE
                  A.DC_CODE = @p0
                  AND A.GUP_CODE = @p1
                  AND A.CUST_CODE = @p2
                  AND A.PICK_ORD_NO IN (SELECT DISTINCT PICK_ORD_NO FROM F051202 WHERE WMS_ORD_NO = @p3)
                ";

      return SqlQuery<DivideDetail>(sql, parms.ToArray());
    }
  }
}
