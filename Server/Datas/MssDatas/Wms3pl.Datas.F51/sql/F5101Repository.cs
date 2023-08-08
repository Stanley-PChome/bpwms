using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F5101Repository : RepositoryBase<F5101, Wms3plDbContext, F5101Repository>
	{
		/// <summary>
		/// 取得前一天庫存資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="calDate"></param>
		/// <returns></returns>
		public IQueryable<StockSettleData> GetLastLocQty(string dcCode, string gupCode, string custCode,DateTime calDate)
		{
			var sql = @"
			SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,
						 ISNULL(SUM(B.END_QTY),0) BEGIN_QTY
				FROM F1903 A 
	            LEFT JOIN F5101 B ON A.GUP_CODE = B.GUP_CODE AND  A.CUST_CODE = B.CUST_CODE AND A.ITEM_CODE = B.ITEM_CODE 		 		 
			            AND B.DC_CODE = @p0 AND B.CAL_DATE = @p3
	   	            WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
		        GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE  ";

			var param = new SqlParameter[]
			{
				new SqlParameter("@p0",dcCode), 
				new SqlParameter("@p1",gupCode), 
				new SqlParameter("@p2",custCode),
				new SqlParameter("@p3",calDate)
			};
			return SqlQuery<StockSettleData>(sql, param);
		}

		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5101 WHERE CAL_DATE = @p0 ";
			var param = new SqlParameter[]
			{
				new SqlParameter("@p0", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}
	}

}
