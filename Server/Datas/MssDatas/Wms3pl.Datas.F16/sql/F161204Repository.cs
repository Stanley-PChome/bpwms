using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161204Repository : RepositoryBase<F161204, Wms3plDbContext, F161204Repository>
	{
		public IQueryable<F161204> GetReurnItem(string dcCode, string gupCode, string custCode, string returnNo, string pastNo, string account)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0",  dcCode),
			};
			var sql = @"SELECT * 
										FROM F161204 A
									 WHERE A.DC_CODE = @p0 ";

			if (!string.IsNullOrEmpty(gupCode))
			{
				sql += "    AND A.GUP_CODE    = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
			}
			else //業主全部要去篩選只有此物流中心業主
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																		           FROM F190101 aa 
																		     INNER JOIN (SELECT * 
																		     				       FROM F192402 
																		     						  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
																		     		  WHERE aa.DC_CODE = A.DC_CODE AND aa.GUP_CODE = A.GUP_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account));
			}
			if (!string.IsNullOrEmpty(custCode))
			{
				sql += "    AND A.CUST_CODE    = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
			}
			else //雇主全部要去篩選只有此物流中心且為此業主的雇主
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																								FROM F190101 cc 
																					INNER JOIN (SELECT * 
																		    								  FROM F192402 
																		    								 WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE 
																		    					WHERE cc.DC_CODE = A.DC_CODE AND cc.GUP_CODE = A.GUP_CODE AND cc.CUST_CODE = A.CUST_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, account));
			}
			if (!string.IsNullOrEmpty(returnNo))
			{
				sql += "    AND A.RETURN_NO    = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, returnNo));
			}
			if (!string.IsNullOrEmpty(pastNo))
			{
				sql += "    AND A.PAST_NO    = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, pastNo));
			}

			return SqlQuery<F161204>(sql, parameters.ToArray()).AsQueryable();
		}
	}
}
