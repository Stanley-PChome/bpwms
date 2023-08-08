using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F02020107Repository : RepositoryBase<F02020107, Wms3plDbContext, F02020107Repository>
	{
		public IQueryable<F151001ReportByAcceptance> GetF151001ReportByAcceptance(string dcCode, string gupCode, string custCode, string purchaseNo,string rtNo, string allocationNo)
		{
			var param = new List<object> { dcCode, gupCode, custCode, purchaseNo, rtNo };
			
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY PURCHASE_NO ASC)AS ROWNUM, a.DC_CODE ,a.GUP_CODE,a.CUST_CODE ,a.ALLOCATION_NO ,b.TAR_WAREHOUSE_ID  AS WAREHOUSE_ID,
						(SELECT c.WAREHOUSE_NAME FROM F1980 c WHERE c.WAREHOUSE_ID = b.TAR_WAREHOUSE_ID AND c.DC_CODE=b.DC_CODE) AS WAREHOUSE_NAME
						FROM F02020107 a
						JOIN F151001 b on a.DC_CODE =b.DC_CODE 
						AND a.GUP_CODE = b.GUP_CODE 
						AND a.CUST_CODE  = b.CUST_CODE 
						AND a.ALLOCATION_NO  = b.ALLOCATION_NO 
						WHERE a.DC_CODE = @p0
						AND a.GUP_CODE  = @p1
						AND a.CUST_CODE = @p2
						AND a.PURCHASE_NO = @p3
						AND a.RT_NO = @p4";
			if (!string.IsNullOrWhiteSpace(allocationNo))
			{
				sql += " AND a.ALLOCATION_NO = @p" + param.Count();
				param.Add(allocationNo);
			}

			return SqlQuery<F151001ReportByAcceptance>(sql, param.ToArray());
		}
	}
}
