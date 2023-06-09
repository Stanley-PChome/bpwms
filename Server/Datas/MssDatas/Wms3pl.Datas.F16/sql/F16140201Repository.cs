using Wms3pl.WebServices.DataCommon;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Wms3pl.Datas.F16
{
	public partial class F16140201Repository : RepositoryBase<F16140201, Wms3plDbContext, F16140201Repository>
	{
		public IQueryable<ReturnDetailSummary> GetReturnDetailSummary(string dcCode,string gupCode,string custCode,string returnNo)
		{
			var sql = @" SELECT 
	ROW_NUMBER()OVER(ORDER BY A.BOM_ITEM_CODE ASC,A.ITEM_CODE)ROWNUM, 
	A.*
													FROM (
													SELECT A.ITEM_CODE ,
																 B.ITEM_NAME,
                                 B.ITEM_SIZE,
                                 B.ITEM_COLOR,
                                 B.ITEM_SPEC,
																 SUM(A.RTN_QTY) RTN_QTY,
																 SUM(A.AUDIT_QTY) AUDIT_QTY,
																 SUM(A.MISS_QTY) MISS_QTY,
																 SUM(A.AUDIT_QTY-A.BAD_QTY) GOOD_QTY,
																 SUM(A.BAD_QTY) BAD_QTY,
																 A.BOM_ITEM_CODE,
																 C.BOM_NAME
														FROM F16140201 A
														LEFT JOIN  F1903 B
														ON B.GUP_CODE = A.GUP_CODE
														AND B.ITEM_CODE = A.ITEM_CODE
														AND B.CUST_CODE = A.CUST_CODE

														LEFT JOIN F910101 C
														ON C.GUP_CODE = A.GUP_CODE
														AND C.CUST_CODE = A.CUST_CODE
														AND C.ITEM_CODE = A.BOM_ITEM_CODE
														AND C.ISPROCESS='1'
														AND C.STATUS='0'
                          WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND RETURN_NO = @p3
                          GROUP BY A.ITEM_CODE,B.ITEM_NAME,B.ITEM_SIZE,B.ITEM_COLOR, B.ITEM_SPEC,A.BOM_ITEM_CODE,C.BOM_NAME
													 --ORDER BY  A.BOM_ITEM_CODE NULLS FIRST,A.ITEM_CODE
													) A";
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo),
			};
			return SqlQuery<ReturnDetailSummary>(sql, parameters.ToArray()).AsQueryable();
		}
	}
}
