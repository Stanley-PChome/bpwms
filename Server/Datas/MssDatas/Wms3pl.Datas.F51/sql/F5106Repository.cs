using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F51
{
	public partial class F5106Repository : RepositoryBase<F5106, Wms3plDbContext, F5106Repository>
	{
		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5106 WHERE CAL_DATE = @p0 ";
			var param = new SqlParameter[]
			{				
				new SqlParameter("@p0", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}

		public IQueryable<SettleMonFeeData> GetSettleMonFee(DateTime calDate, string quoteNo)
		{
            var parameters = new SqlParameter[]
			{
				new SqlParameter("@p0", calDate.AddMonths(-1)),
				new SqlParameter("@p1", calDate),
				new SqlParameter("@p2", quoteNo)
			};

			var sql = @"
  SELECT ROW_NUMBER()OVER(ORDER BY tb.ACC_ITEM_NAME, tb.UNIT_FEE,tb.OUTSOURCE_ID) ROWNUM,tb.* FROM (
  SELECT B.QUOTE_NAME ACC_ITEM_NAME ,
         1 ACC_NUM,
         ISNULL(B.APPROVED_PRICE,0) UNIT_FEE,
         0 BASIC_FEE,
         0 OVER_FEE,
         SUM (A.QTY) PRICE_CNT,
         SUM (A.AMT) COST,
         SUM (A.AMT ) AMOUNT,
         '1' IN_TAX,
         'A' ACC_KIND, B.OUTSOURCE_ID          
    FROM F5106 A JOIN F910401 B ON A.QUOTE_NO = B.QUOTE_NO
   WHERE A.CAL_DATE >= @p0 AND A.CAL_DATE < @p1 AND A.QUOTE_NO = @p2
GROUP BY B.QUOTE_NAME, B.APPROVED_PRICE,B.OUTSOURCE_ID ) tb";

			return SqlQuery<SettleMonFeeData>(sql, parameters).AsQueryable();
		}
	}
}
