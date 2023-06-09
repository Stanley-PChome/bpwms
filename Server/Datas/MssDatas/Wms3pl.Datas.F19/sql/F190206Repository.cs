using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190206Repository : RepositoryBase<F190206, Wms3plDbContext, F190206Repository>
	{

		/// <summary>
		/// 取得商品的檢驗項目
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="checkType"></param>
		/// <returns></returns>
		public IQueryable<F190206CheckName> GetItemCheckList(string dcCode, string gupCode, string custCode, string itemCode
			, string purchaseNo, string purchaseSeq, string rtNo, string checkType = "")
		{
			var sql = @"SELECT A.ITEM_CODE,
							   A.CHECK_NO,
							   A.CHECK_TYPE,
							   A.CHECK_NAME,
							   B.UCC_CODE,
							   B.ISPASS,
							   B.MEMO
						  FROM (SELECT X.GUP_CODE,
									   X.CUST_CODE,
									   X.ITEM_CODE,
									   X.CHECK_NO,
									   X.CHECK_TYPE,
									   Y.CHECK_NAME
								  FROM F190206 X JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
								 WHERE X.GUP_CODE = @p0 AND X.CUST_CODE = @p1 AND X.ITEM_CODE = @p2
								UNION ALL
								SELECT M.GUP_CODE,
									   NULL AS CUST_CODE,
									   M.ITEM_CODE,
									   X.CHECK_NO,
									   X.CHECK_TYPE,
									   Y.CHECK_NAME
								  FROM F1903 M
									   JOIN F190205 X ON M.GUP_CODE = X.GUP_CODE AND M.CUST_CODE = x.CUST_CODE AND M.TYPE = X.TYPE 
									   JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
								 WHERE     X.GUP_CODE = @p0
									   AND M.ITEM_CODE = @p2
									   AND NOT EXISTS
												  (SELECT 1
													 FROM F190206
													WHERE     GUP_CODE = @p0
														  AND CUST_CODE = @p1
														  AND ITEM_CODE = @p2
														  AND ( CHECK_TYPE = @p3 OR @p3 =''))) A
							   LEFT JOIN F02020102 B
								  ON     A.CHECK_NO = B.CHECK_NO
									 AND A.ITEM_CODE = B.ITEM_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 --AND A.CUST_CODE = B.CUST_CODE
									 AND B.DC_CODE = @p4
									 AND B.PURCHASE_NO = @p5
									 AND B.PURCHASE_SEQ = @p6
									 AND (B.RT_NO = @p7 OR @p7 ='')
						 WHERE     A.GUP_CODE = @p0
							   AND (A.CUST_CODE = @p1 OR A.CUST_CODE IS NULL)
							   AND A.ITEM_CODE = @p2
							   AND (A.CHECK_TYPE = @p3 OR @p3 ='')";

			var param = new[] {
				new SqlParameter("@p0", gupCode),
				new SqlParameter("@p1", custCode),
				new SqlParameter("@p2", itemCode),
				new SqlParameter("@p3", checkType),
				new SqlParameter("@p4", dcCode),
				new SqlParameter("@p5", purchaseNo),
				new SqlParameter("@p6", purchaseSeq),
				new SqlParameter("@p7", rtNo)
			};

			var result = SqlQuery<F190206CheckName>(sql, param).AsQueryable();
			return result;
		}

		public IQueryable<F190206QuickCheckName> GetQuickItemCheckList(string dcCode, string gupCode, string custCode
			, string purchaseNo, string rtNo, List<string> itemCodes, string checkType = "")
		{
			string sql = @"
						SELECT A.ITEM_CODE,
							   A.CHECK_NO,
							   A.CHECK_TYPE,
							   A.CHECK_NAME,
							   B.UCC_CODE,
							   B.ISPASS,
							   B.MEMO,
							   B.PURCHASE_SEQ
						  FROM (SELECT X.GUP_CODE,
									   X.CUST_CODE,
									   X.ITEM_CODE,
									   X.CHECK_NO,
									   X.CHECK_TYPE,
									   Y.CHECK_NAME
								  FROM F190206 X JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
								 WHERE X.GUP_CODE = @p0 AND X.CUST_CODE = @p1
								UNION ALL
								SELECT M.GUP_CODE,
									   NULL AS CUST_CODE,
									   M.ITEM_CODE,
									   X.CHECK_NO,
									   X.CHECK_TYPE,
									   Y.CHECK_NAME
								  FROM F1903 M
									   JOIN F190205 X ON M.GUP_CODE = X.GUP_CODE AND M.TYPE = X.TYPE AND M.CUST_CODE = X.CUST_CODE
									   JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
								 WHERE     X.GUP_CODE = @p0
									   AND M.ITEM_CODE NOT IN (SELECT ITEM_CODE
													 FROM F190206
													WHERE     GUP_CODE = @p0
														  AND CUST_CODE = @p1
														  {0}
														  AND ( CHECK_TYPE = @p2 OR @p2 =''))) A
							   LEFT JOIN F02020102 B
								  ON     A.CHECK_NO = B.CHECK_NO
									 AND A.ITEM_CODE = B.ITEM_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND B.DC_CODE = @p3
									 AND B.PURCHASE_NO = @p4
									 AND (B.RT_NO = @p5 OR @p5 = '')
						 WHERE     A.GUP_CODE = @p0
							   AND (A.CUST_CODE = @p1 OR A.CUST_CODE IS NULL)
							   {1}
							   AND (A.CHECK_TYPE = @p2 OR @p2 ='')
			";
			var param = new List<SqlParameter> {
				new SqlParameter("@p0", gupCode),
				new SqlParameter("@p1", custCode),
				new SqlParameter("@p2", checkType),
				new SqlParameter("@p3", dcCode),
				new SqlParameter("@p4", purchaseNo),
				new SqlParameter("@p5", rtNo)
			};
			var objParam = new List<object>();
			objParam.AddRange(param);
			string condition = objParam.CombineSqlInParameters(" AND A.ITEM_CODE", itemCodes);
			foreach (var i in itemCodes)
				param.Add(new SqlParameter(string.Format("@p{0}",param.Count),i));
			var result = SqlQuery<F190206QuickCheckName>(string.Format(sql, condition.Replace("A.",""), condition), param.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<F190206CheckItemName> GetCheckItems(string gupCode, string custCode, string itemCode, string checkType = "")
		{
			string sql = @"SELECT X.GUP_CODE,
								   X.CUST_CODE,
								   X.ITEM_CODE,
								   X.CHECK_NO,
								   X.CHECK_TYPE,
								   Y.CHECK_NAME
							  FROM F190206 X JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
							 WHERE     X.GUP_CODE = @p0
								   AND X.CUST_CODE = @p1
								   AND X.ITEM_CODE = @p2
								   AND (CHECK_TYPE = @p3 OR @p3 ='')
							UNION ALL
							SELECT M.GUP_CODE,
								   NULL AS CUST_CODE,
								   M.ITEM_CODE,
								   X.CHECK_NO,
								   X.CHECK_TYPE,
								   Y.CHECK_NAME
							  FROM F1903 M
								   JOIN F190205 X ON M.GUP_CODE = X.GUP_CODE AND M.TYPE = X.TYPE AND M.CUST_CODE = X.CUST_CODE
								   JOIN F1983 Y ON X.CHECK_NO = Y.CHECK_NO
							 WHERE     X.GUP_CODE = @p0
                                    AND X.CUST_CODE = @p1
								   AND M.ITEM_CODE = @p2
								   AND (CHECK_TYPE = @p3 OR @p3 ='')
								   AND NOT EXISTS
											  (SELECT 1
												 FROM F190206
												WHERE     GUP_CODE = @p0
													  AND CUST_CODE = @p1
													  AND ITEM_CODE = @p2
													  AND (CHECK_TYPE = @p3 OR @p3 =''))";

			var param = new[] {
				new SqlParameter("@p0", gupCode),
				new SqlParameter("@p1", custCode),
				new SqlParameter("@p2", itemCode),
				new SqlParameter("@p3", checkType)
			};

			var result = SqlQuery<F190206CheckItemName>(sql, param).AsQueryable();
			return result;
		}
	}
}
