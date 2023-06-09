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
	public partial class F5108Repository : RepositoryBase<F5108, Wms3plDbContext, F5108Repository>
	{
		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5108 WHERE CAL_DATE = @p0 ";
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
                SELECT ROW_NUMBER()OVER(ORDER BY tb.ACC_ITEM_NAME, tb.UNIT_FEE, tb.IN_TAX,tb.ACC_KIND) ROWNUM,tb.* FROM (
                SELECT B.ACC_PROJECT_NAME ACC_ITEM_NAME ,
                        1 ACC_NUM,
                        B.FEE UNIT_FEE,
                        0 BASIC_FEE,
                        0 OVER_FEE,
                        1 PRICE_CNT,
                        SUM (A.AMT) COST,
                        SUM (A.AMT ) AMOUNT,
                        B.IN_TAX,
                        B.ACC_KIND         
                FROM F5108 A JOIN F199007 B ON A.QUOTE_NO = B.QUOTE_NO
                WHERE A.CAL_DATE >= @p0 AND A.CAL_DATE < @p1 AND A.QUOTE_NO = @p2
            GROUP BY B.ACC_PROJECT_NAME, B.FEE, B.IN_TAX,B.ACC_KIND ) tb";

			return SqlQuery<SettleMonFeeData>(sql, parameters).AsQueryable();
		}
	}
}
