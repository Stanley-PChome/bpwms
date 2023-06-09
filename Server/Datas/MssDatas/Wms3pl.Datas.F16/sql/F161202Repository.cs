using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161202Repository : RepositoryBase<F161202, Wms3plDbContext, F161202Repository>
	{
		public IQueryable<F161402Data> GetF161202ReturnDetails(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", returnNo)
			};
			var sql = @"SELECT  ROW_NUMBER()OVER(ORDER BY TMP.RETURN_NO)ROWNUM,TMP.*
									  FROM (
													SELECT a.RETURN_NO,a.ITEM_CODE,a.RTN_QTY,c.ITEM_NAME,c.BUNDLE_SERIALNO,a.DC_CODE,a.GUP_CODE,a.CUST_CODE,
																 CASE WHEN LEN(d.LOC_CODE) = 9 
																		  THEN SUBSTRING(d.LOC_CODE,1,1) + '-' + SUBSTRING(d.LOC_CODE,2,2)  + '-' + SUBSTRING(d.LOC_CODE,4,2)  + '-' + SUBSTRING(d.LOC_CODE,6,2)  + '-' + SUBSTRING(d.LOC_CODE,8,2) 
																		  ELSE d.LOC_CODE 
																 END as LOC_CODE
																 ,0 as MOVED_QTY ,0 as AUDIT_QTY,0 as TOTAL_AUDIT_QTY, a.RTN_QTY as DIFFERENT_QTY, null as CAUSE, null as MEMO ,c.EAN_CODE1 ,c.EAN_CODE2 ,c.EAN_CODE3 
													  FROM F161202 a 
											 LEFT JOIN F1903 c ON a.GUP_CODE = c.GUP_CODE AND a.CUST_CODE = c.CUST_CODE AND a.ITEM_CODE = c.ITEM_CODE
											 LEFT JOIN (SELECT Min(f.LOC_CODE) LOC_CODE,Min(f.WAREHOUSE_ID) WAREHOUSE_ID,f.DC_CODE  FROM F1912 f LEFT JOIN F1980 g ON f.WAREHOUSE_ID = g.WAREHOUSE_ID AND f.DC_CODE = g.DC_CODE
																	 WHERE f.DC_CODE = @p0 AND f.GUP_CODE in (@p1,'0') AND f.CUST_CODE in (@p2,'0') AND g.WAREHOUSE_TYPE = 'T' 
																GROUP BY f.DC_CODE) d ON a.DC_CODE = d.DC_CODE
													 WHERE a.DC_CODE = @p0
														 AND a.GUP_CODE = @p1
														 AND a.CUST_CODE = @p2
														 AND a.RETURN_NO = @p3) TMP ";

			var result = SqlQuery<F161402Data>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<F161202SelectedData> GetReturnItems(string dcCode, string gupCode, string custCode, string returnDateStart, string returnDateEnd, string returnNoStart, string returnNoEnd, string itemCode, string itemName)
		{
			string dcCodeCondition = string.Empty; 
			string gupCodeCondition = string.Empty; 
			string custCodeCondition = string.Empty;
			string returnDateCondition = string.Empty;
			string returnNoCondition = string.Empty;
			string itemCodeCondition = string.Empty;
			string itemNameCondition = (string.IsNullOrEmpty(itemName)) ? string.Empty : "AND c.ITEM_NAME LIKE '" + itemName + "%' ";
			var paramers = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(dcCode) && dcCode != "0")
			{
				dcCodeCondition = string.Format("AND a.DC_CODE = @p{0} ", paramers.Count());
				paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), dcCode));
			}
			if (!string.IsNullOrEmpty(gupCode) && gupCode != "0")
			{ 
				gupCodeCondition = string.Format("AND a.GUP_CODE = @p{0} ", paramers.Count());
				paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gupCode));
			}
			if (!string.IsNullOrEmpty(custCode) && custCode != "0") 
			{
				custCodeCondition = string.Format("AND a.CUST_CODE = @p{0} ", paramers.Count());
				paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), custCode));
			}
			if (!string.IsNullOrEmpty(returnDateStart) || !string.IsNullOrEmpty(returnDateEnd)) 
			{
				if(!string.IsNullOrEmpty(returnDateStart))
				{
					returnDateCondition = string.Format("AND a.RETURN_DATE >= convert(varchar,@p{0},111) ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), returnDateStart));
				}
				if(!string.IsNullOrEmpty(returnDateEnd))
				{
					returnDateCondition += string.Format("AND a.RETURN_DATE <= convert(varchar,@p{0},111) ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), returnDateEnd));
				}
			}
			if (!string.IsNullOrEmpty(returnNoStart) || !string.IsNullOrEmpty(returnNoEnd)) 
			{
				if(!string.IsNullOrEmpty(returnNoStart))
				{
					returnNoCondition = string.Format("AND a.RETURN_NO >= @p{0} ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), returnNoStart));
				}
				if(!string.IsNullOrEmpty(returnNoEnd))
				{
					returnNoCondition += string.Format("AND a.RETURN_NO <= @p{0} ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), returnNoEnd));
				}
			}
			if (!string.IsNullOrEmpty(itemCode) && itemCode != "0")
			{
				itemCodeCondition = string.Format("AND b.ITEM_CODE = @p{0} ", paramers.Count());
				paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), itemCode));
			}
				var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY a.RETURN_DATE)ROWNUM,a.RETURN_DATE,a.RETURN_NO,b.ITEM_CODE,c.ITEM_NAME,c.ITEM_SIZE,c.ITEM_SPEC,c.ITEM_COLOR,b.RTN_QTY 
										FROM F161201 a 
							 LEFT JOIN F161202 b ON a.DC_CODE = b.DC_CODE AND a.GUP_CODE = b.GUP_CODE AND a.CUST_CODE = b.CUST_CODE AND a.RETURN_NO = b.RETURN_NO
							 LEFT JOIN F1903 c ON b.GUP_CODE = c.GUP_CODE AND b.ITEM_CODE = c.ITEM_CODE AND b.CUST_CODE = c.CUST_CODE 
						     WHERE 1=1";
						sql += dcCodeCondition;
						sql += gupCodeCondition;
						sql += custCodeCondition;
						sql += returnDateCondition;
						sql += returnNoCondition;
						sql += itemCodeCondition;
						sql += itemNameCondition;

			var result = SqlQuery<F161202SelectedData>(sql, paramers.ToArray()).AsQueryable();
			return result;
		}

		public void DeleteF161202(string gupCode, string custCode, string dcCode, string returnNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", gupCode),
				new SqlParameter("@p1",custCode ),
				new SqlParameter("@p2", dcCode),
				new SqlParameter("@p3", returnNo),
			};

			var sql = " DELETE F161202 WHERE GUP_CODE =@p0" +
								"   AND CUST_CODE = @p1" +
								"   AND DC_CODE = @p2" +
								"   AND RETURN_NO = @p3 ";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<F161202> GetDatasByDc(string dcCode, string ordProp, DateTime returnDate)
		{
            var query = _db.F161202s
                     .Join(_db.F161201s, A => new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RETURN_NO }, B => new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.RETURN_NO }, (A, B) => new { A, B })
                     .Where(x => x.A.DC_CODE == dcCode)
                     .Where(x => x.B.ORD_PROP == ordProp)
                     .Where(x => x.B.RETURN_DATE == returnDate)
                     .Where(x => x.B.STATUS != "9")
                     .Select(x => x.A);
            return query;
        }


		/// <summary>
		/// 取得退貨回檔分配明細
		/// </summary>
		/// <returns></returns>
		public IQueryable<F161202> GetAllotReturnData(string dcCode, string gupCode, string custCode)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @"
					SELECT B.*
					  FROM F161201 A
						   INNER JOIN F161202 B
							  ON     A.RETURN_NO = B.RETURN_NO
								 AND A.GUP_CODE = B.GUP_CODE
								 AND A.DC_CODE = B.DC_CODE
								 AND A.CUST_CODE = B.CUST_CODE
					 WHERE     A.DC_CODE = @p0
						   AND A.GUP_CODE = @p1
						   AND A.CUST_CODE = @p2
						   AND A.STATUS = '2'
						   AND A.PROC_FLAG = '0'
					";
			return SqlQuery<F161202>(sql, parms.ToArray());
		}
	}
}
