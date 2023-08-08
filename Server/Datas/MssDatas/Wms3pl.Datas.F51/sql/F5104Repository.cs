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
	public partial class F5104Repository : RepositoryBase<F5104, Wms3plDbContext, F5104Repository>
	{
		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5104 WHERE CAL_DATE = @p0 ";
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
  
SELECT 
	ROW_NUMBER()OVER(ORDER BY tb.ACC_ITEM_NAME,tb.ACC_NUM, tb.UNIT_FEE,tb.BASIC_FEE,tb.OVER_FEE, tb.IN_TAX,tb.ACC_KIND,ACC_UNIT_NAME) ROWNUM,
	tb.* 
    FROM (
          SELECT B.ACC_ITEM_NAME,
                 B.ACC_NUM,
                 B.APPROV_FEE UNIT_FEE,
                 ISNULL(B.APPROV_BASIC_FEE,0) BASIC_FEE,
                 ISNULL(B.APPROV_OVER_FEE,0) OVER_FEE,                        
                 CASE WHEN ACC_UNIT_NAME = 'ORDER' OR ACC_UNIT_NAME = '次'  THEN COUNT(A.WMS_NO)
                     WHEN ACC_UNIT_NAME = 'PCS'  THEN COUNT(A.QTY)
                     WHEN ACC_UNIT_NAME = '件'  THEN COUNT(DISTINCT A.ITEM_CODE)                          
                     END PRICE_CNT,
                 SUM (A.AMT) COST,
                 SUM (A.AMT ) AMOUNT,
                 B.IN_TAX,
                 B.ACC_KIND,
				 C.ACC_UNIT_NAME         
              FROM F5104 A 
              JOIN F500104 B ON A.QUOTE_NO = B.QUOTE_NO
              JOIN F91000302 C ON B.ACC_UNIT = C.ACC_UNIT AND B.ITEM_TYPE_ID = C.ITEM_TYPE_ID
            WHERE A.CAL_DATE >= @p0 AND A.CAL_DATE < @p1 AND A.QUOTE_NO = @p2
        GROUP BY B.ACC_ITEM_NAME,B.ACC_NUM, B.APPROV_FEE,B.APPROV_BASIC_FEE,B.APPROV_OVER_FEE, B.IN_TAX,B.ACC_KIND,ACC_UNIT_NAME ) tb";

			return SqlQuery<SettleMonFeeData>(sql, parameters).AsQueryable();
		}
	}
}
