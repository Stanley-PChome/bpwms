using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F20
{
	public partial class F200102Repository : RepositoryBase<F200102, Wms3plDbContext, F200102Repository>
	{
		public IQueryable<F200102Data> GetF200102DatasNotF050801F050301(string dcCode, string gupCode, string custCode,string adjustNo, List<string> adjustSeq)
		{
			var parameters = new List<object>
			{
				dcCode,
				gupCode,
				custCode,
				adjustNo,
			};
			var condition = parameters.CombineSqlNotInParameters(" AND A.ADJUST_SEQ ", adjustSeq);
			string sql = $@" SELECT A.ADJUST_NO,
                   A.ADJUST_SEQ,
                   NULL DELV_DATE,
                   NULL PICK_TIME,
                   NULL CUST_ORD_NO,
                   A.ORG_PICK_TIME,
                   A.ALL_ID,
                   A.ADDRESS,
                   A.NEW_DC_CODE,
                   A.CAUSE,
                   A.CAUSE_MEMO,
                   A.UPD_NAME,
                   A.UPD_DATE,
                   A.UPD_STAFF,
                   A.DC_CODE,
                   A.GUP_CODE,
                   A.CUST_CODE,
                   A.STATUS,
                   A.ORD_NO,
                   A.CRT_DATE,
                   A.CRT_NAME,
                   A.CRT_STAFF,
                   A.WORK_TYPE
                   FROM F200102 A
				   WHERE  A.DC_CODE = @p0
					 AND A.GUP_CODE = @p1
					 AND A.CUST_CODE = @p2
					 AND A.ADJUST_NO = @p3
					 AND A.STATUS != '9'
					 {condition}
					 ";
			return SqlQuery<F200102Data>(sql, parameters.ToArray());
		}

		public IQueryable<F200102Data> GetF200102Datas(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", adjustNo),
			};

			var sql = $@"   SELECT ROW_NUMBER()OVER(ORDER BY A.ADJUST_NO, A.ADJUST_SEQ)AS ROWNUM,
         A.*,
         B.ALL_COMP,
         C.DC_NAME AS NEW_DC_NAME,
         CASE WHEN A.WORK_TYPE <> '5' THEN D.CAUSE ELSE F.CAUSE END
            AS CAUSENAME,
         E.CHECKOUT_TIME
    FROM (  SELECT A.ADJUST_NO,
                   A.ADJUST_SEQ,
                   D.DELV_DATE,
                   D.PICK_TIME,
                   B.CUST_ORD_NO,
                   A.ORG_PICK_TIME,
                   A.ALL_ID,
                   A.ADDRESS,
                   A.NEW_DC_CODE,
                   A.CAUSE,
                   A.CAUSE_MEMO,
                   A.UPD_NAME,
                   A.UPD_DATE,
                   A.UPD_STAFF,
                   A.DC_CODE,
                   A.GUP_CODE,
                   A.CUST_CODE,
                   A.STATUS,
                   A.ORD_NO,
                   A.CRT_DATE,
                   A.CRT_NAME,
                   A.CRT_STAFF,
                   A.WORK_TYPE
              FROM F200102 A
                   INNER JOIN F050301 B
                      ON     B.DC_CODE = A.DC_CODE
                         AND B.GUP_CODE = A.GUP_CODE
                         AND B.CUST_CODE = A.CUST_CODE
                         AND B.ORD_NO = A.ORD_NO
                   INNER JOIN F05030101 C
                      ON     C.DC_CODE = B.DC_CODE
                         AND C.GUP_CODE = B.GUP_CODE
                         AND C.CUST_CODE = B.CUST_CODE
                         AND C.ORD_NO = B.ORD_NO
                   INNER JOIN F050801 D
                      ON     D.DC_CODE = C.DC_CODE
                         AND D.GUP_CODE = C.GUP_CODE
                         AND D.CUST_CODE = C.CUST_CODE
                         AND D.WMS_ORD_NO = C.WMS_ORD_NO
          GROUP BY A.ADJUST_NO,
                   A.ADJUST_SEQ,
                   D.DELV_DATE,
                   D.PICK_TIME,
                   B.CUST_ORD_NO,
                   A.ORG_PICK_TIME,
                   A.ALL_ID,
                   A.ADDRESS,
                   A.NEW_DC_CODE,
                   A.CAUSE,
                   A.CAUSE_MEMO,
                   A.UPD_NAME,
                   A.UPD_DATE,
                   A.UPD_STAFF,
                   A.DC_CODE,
                   A.GUP_CODE,
                   A.CUST_CODE,
                   A.STATUS,
                   A.ORD_NO,
                   A.CRT_DATE,
                   A.CRT_NAME,
                   A.CRT_STAFF,
                   A.WORK_TYPE) A
         LEFT JOIN F1947 B ON B.DC_CODE = A.DC_CODE AND B.ALL_ID = A.ALL_ID
         LEFT JOIN F1901 C ON C.DC_CODE = A.NEW_DC_CODE
         LEFT JOIN F1951 D ON D.UCT_ID = 'AJ' AND D.UCC_CODE = A.CAUSE
         LEFT JOIN F0513 E
            ON     E.DC_CODE = A.DC_CODE
               AND E.GUP_CODE = A.GUP_CODE
               AND E.CUST_CODE = A.CUST_CODE
               AND E.DELV_DATE = A.DELV_DATE
               AND E.PICK_TIME = A.PICK_TIME
         LEFT JOIN F1951 F ON F.UCT_ID = 'AH' AND F.UCC_CODE = A.CAUSE
   WHERE     A.DC_CODE = @p0
         AND A.GUP_CODE = @p1
         AND A.CUST_CODE = @p2
         AND A.ADJUST_NO = @p3
         AND A.STATUS != '9'
";
			return SqlQuery<F200102Data>(sql, parameters.ToArray());
		}
	}
}
