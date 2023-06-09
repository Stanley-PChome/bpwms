using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.F54
{
	public partial class F540101Repository : RepositoryBase<F540101, Wms3plDbContext, F540101Repository>
	{
		public F540101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{

		}

		public IQueryable<F540101TransactionData> GetTransactionDatas(string dcCode, string gupCode, string custCode, DateTime? trans_SDate, DateTime? trans_EDate)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)
			};

			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY a.TRANSACTION_NO)ROWNUM,
	                    a.TRANSACTION_NO,a.SELL_CUST_CODE,d.CUST_NAME as SELL_CUST_NAME,a.BUY_CUST_CODE,e.CUST_NAME as BUY_CUST_NAME,
												 b.ITEM_CODE,f.ITEM_NAME,b.ITEM_PRICE,b.TRANSACTION_QTY,'' as TaxType, a.MEMO,b.ITEM_PRICE * b.TRANSACTION_QTY as  SUBTOTAL,
												 CASE WHEN c.CYCLE_DATE IS NULL THEN NULL 
													    WHEN DATEPART(MONTH,a.TRANSACTION_DATE) > c.CYCLE_DATE THEN LEFT(CONVERT(varchar,DATEADD(month,1,a.TRANSACTION_DATE),111),8) + RIGHT(REPLICATE('0', 2)+LTRIM(c.CYCLE_DATE),2)
															ELSE LEFT(CONVERT(varchar,a.TRANSACTION_DATE,111),8) +RIGHT(REPLICATE('0', 2)+LTRIM(c.CYCLE_DATE),2) 
													END as PaymentRequestDay 
					          FROM F540101 a
					     LEFT JOIN F540102 b ON a.TRANSACTION_NO = b.TRANSACTION_NO 
					     LEFT JOIN F910301 c ON a.BUY_DC_CODE = c.DC_CODE AND a.BUY_GUP_CODE = c.GUP_CODE AND a.TRANSACTION_DATE = c.ENABLE_DATE 
					     LEFT JOIN F1909 d ON a.SELL_GUP_CODE = d.GUP_CODE AND a.SELL_CUST_CODE = d.CUST_CODE
					     LEFT JOIN F1909 e ON a.BUY_GUP_CODE = e.GUP_CODE AND a.BUY_CUST_CODE = e.CUST_CODE 
					     LEFT JOIN F1903 f ON a.SELL_GUP_CODE = f.GUP_CODE AND  b.ITEM_CODE = f.ITEM_CODE AND a.BUY_CUST_CODE = f.CUST_CODE
									 WHERE a.BUY_DC_CODE = @p0 ";

			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += string.Format("AND a.BUY_GUP_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += string.Format("AND a.BUY_CUST_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
			}
			if (trans_SDate != null)
			{
				sql += string.Format(" AND a.TRANSACTION_DATE >= @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, trans_SDate));

			}

			if (trans_EDate != null)
			{
				sql += string.Format(" AND a.TRANSACTION_DATE < @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, trans_EDate));

			}

			return SqlQuery<F540101TransactionData>(sql, parameters.ToArray()).AsQueryable();
		}
	}
}
