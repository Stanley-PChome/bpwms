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
	public partial class F200103Repository : RepositoryBase<F200103, Wms3plDbContext, F200103Repository>
	{
		public IQueryable<F200103Data> GetF200103Datas(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", adjustNo),
			};
			var sql = @"
					  SELECT ROW_NUMBER()OVER(ORDER BY A.ADJUST_NO, ADJUST_SEQ) ROWNUM,
							 A.ADJUST_NO,
							 A.ADJUST_SEQ,
							 A.WAREHOUSE_ID,
							 B.WAREHOUSE_NAME,
							 A.ITEM_CODE,
							 C.ITEM_NAME,
							 A.LOC_CODE,
							 C.ITEM_SIZE,
							 C.ITEM_SPEC,
							 C.ITEM_COLOR,
							 ISNULL (E.QTY, 0) AS ITEM_QTY,
							 CASE WHEN A.WORK_TYPE = '0' THEN A.ADJ_QTY ELSE 0 END AS ADJ_QTY_IN,
							 CASE WHEN A.WORK_TYPE = '1' THEN A.ADJ_QTY ELSE 0 END AS ADJ_QTY_OUT,
							 A.CAUSE,
							 D.CAUSE AS CAUSENAME,
							 A.CAUSE_MEMO,
							 A.UPD_DATE,
							 A.UPD_NAME,
							 A.UPD_STAFF,
							 A.DC_CODE,
							 A.GUP_CODE,
							 A.CUST_CODE,
							 A.VALID_DATE,
							 A.ENTER_DATE,
							 G.BUNDLE_SERIALNO,
							 H.DC_NAME,
							 A.WORK_TYPE,
							 A.MAKE_NO
						FROM F200103 A
							 INNER JOIN F1980 B
								ON B.DC_CODE = A.DC_CODE AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
							 INNER JOIN F1903 C
								ON C.GUP_CODE = A.GUP_CODE AND C.ITEM_CODE = A.ITEM_CODE AND C.CUST_CODE = A.CUST_CODE
							 INNER JOIN F1951 D ON D.UCT_ID = 'AI' AND D.UCC_CODE = A.CAUSE
							 LEFT JOIN (  SELECT E.DC_CODE,
												 E.GUP_CODE,
												 E.CUST_CODE,
												 E.LOC_CODE,
												 E.ITEM_CODE,
												 E.VALID_DATE,
												 E.ENTER_DATE,
												 E.VNR_CODE,
												 SUM (E.QTY) AS QTY,
                                                 E.MAKE_NO
											FROM F1913 E
										GROUP BY E.DC_CODE,
												 E.GUP_CODE,
												 E.CUST_CODE,
												 E.LOC_CODE,
												 E.ITEM_CODE,
												 E.VALID_DATE,
												 E.ENTER_DATE,
												 E.VNR_CODE,
                                                 E.MAKE_NO) E
								ON     E.DC_CODE = A.DC_CODE
								   AND E.GUP_CODE = A.GUP_CODE
								   AND E.CUST_CODE = A.CUST_CODE
								   AND E.LOC_CODE = A.LOC_CODE
								   AND E.ITEM_CODE = A.ITEM_CODE
								   AND E.VALID_DATE = A.VALID_DATE
								   AND E.ENTER_DATE = A.ENTER_DATE
								   AND E.VNR_CODE = A.VNR_CODE
                                   AND E.MAKE_NO = A.MAKE_NO
							 LEFT JOIN F1903 G
								ON     G.GUP_CODE = A.GUP_CODE
								   AND G.CUST_CODE = A.CUST_CODE
								   AND G.ITEM_CODE = A.ITEM_CODE
							 INNER JOIN F1901 H ON H.DC_CODE = A.DC_CODE
					   WHERE      A.DC_CODE = @p0
							 AND A.GUP_CODE = @p1
							 AND A.CUST_CODE = @p2
							 AND A.ADJUST_NO = @p3
					
					";
			return SqlQuery<F200103Data>(sql, parameters.ToArray());
		}
	}
}
