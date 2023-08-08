using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700709Repository : RepositoryBase<F700709, Wms3plDbContext, F700709Repository>
	{
		/// <summary>
		/// 從揀貨單取得訂單作業統計
		/// </summary>
		/// <param name="crtDate"></param>
		/// <returns></returns>
		public IQueryable<F700709ForSchedule> GetOrderPickTimeStatistics(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var sql = @"  SELECT A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 C.ORD_NO,
								 A.CRT_DATE AS CNT_DATE,
								 CONVERT(VARCHAR(10),(DATEPART (DAY,A.CRT_DATE) - 1)) AS CNT_DAY,
								 ISNULL( CEILING( DATEDIFF(DAY,MIN (A.CRT_DATE),MAX(A.PICK_FINISH_DATE)) * 24 * 60), 0)
									AS TOTAL_PICK_TIME
							FROM F051201 A
                 JOIN F051202 D 
                   ON D.DC_CODE = A.DC_CODE
                  AND D.GUP_CODE = A.GUP_CODE
                  AND D.CUST_CODE = A.CUST_CODE
                  AND D.PICK_ORD_NO = A.PICK_ORD_NO
								 JOIN F050801 B
									ON     B.DC_CODE = D.DC_CODE
									   AND B.GUP_CODE = D.GUP_CODE
									   AND B.CUST_CODE = D.CUST_CODE
									   AND B.WMS_ORD_NO = D.WMS_ORD_NO
								 JOIN F05030101 C
									ON     B.DC_CODE = C.DC_CODE
									   AND B.GUP_CODE = C.GUP_CODE
									   AND B.CUST_CODE = C.CUST_CODE
									   AND B.WMS_ORD_NO = C.WMS_ORD_NO
						   WHERE A.CRT_DATE BETWEEN @p0 AND @p1 AND A.PICK_STATUS = 2
						GROUP BY A.DC_CODE,
								 A.GUP_CODE,
								 A.CUST_CODE,
								 C.ORD_NO,
								 A.CRT_DATE,
								 (DATEPART (DAY,A.CRT_DATE) - 1)";

			return SqlQuery<F700709ForSchedule>(sql, new object[] { beginCrtDate, endCrtDate });
		}

	}
}
