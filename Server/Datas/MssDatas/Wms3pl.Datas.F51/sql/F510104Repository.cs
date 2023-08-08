using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510104Repository : RepositoryBase<F510104, Wms3plDbContext, F510104Repository>
	{
		public void InsertVirtualByDate(string dcCode, string gupCode, string custCode, DateTime settleDate)
		{
			var sql = @"INSERT INTO F510104 
SELECT  @p3 CAL_DATE,A.*
FROM VW_VirtualStock A
WHERE A.DC_CODE =  @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND A.STATUS IN('0','1')";
			var param = new SqlParameter[]
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}

	}

}
