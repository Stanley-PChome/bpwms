using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700201Repository : RepositoryBase<F700201, Wms3plDbContext, F700201Repository>
	{
		public IQueryable<F700201Ex> GetF700201SearchData(string gupCode, string custCode, string dcCode,
		  string compDateBegin, string compDateEnd, string compNo, string depId, string compType,
		  string status)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)
			};
			var sql = $@"
                            SELECT ROW_NUMBER()OVER(ORDER BY COMPLAINT_NO,DC_CODE)ROWNUM,
							                               F700201.*,
							                               F1951.CAUSE AS COMPLAINT_NAME1,
							                               F1925.DEP_NAME,
							                               VW_F000904_LANG.NAME STATUS_DESC
						                              FROM F700201
							                               LEFT OUTER JOIN F1951
								                              ON F700201.COMPLAINT_TYPE = F1951.UCC_CODE AND F1951.UCT_ID = 'CS'
							                               LEFT OUTER JOIN F1925 ON F700201.DEP_ID = F1925.DEP_ID
							                               LEFT OUTER JOIN VW_F000904_LANG
								                              ON     F700201.STATUS = VW_F000904_LANG.VALUE
									                             AND VW_F000904_LANG.TOPIC = 'F700201'
									                             AND VW_F000904_LANG.SUBTOPIC = 'STATUS'
									                             AND VW_F000904_LANG.LANG = '{Current.Lang}'
						                             WHERE F700201.DC_CODE = @p0
";

			if (!string.IsNullOrWhiteSpace(gupCode))
			{
				sql += " AND F700201.GUP_CODE = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
			}
			else //業主全部要去篩選只有此物流中心業主或業主設為共用
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																							 FROM F190101 aa 
																				 INNER JOIN (SELECT * 
																											 FROM F192402 
																										  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
																						  WHERE aa.DC_CODE = F700201.DC_CODE AND aa.GUP_CODE = F700201.GUP_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, Current.Staff));
			}

			if (!string.IsNullOrWhiteSpace(custCode))
			{
				sql += " AND F700201.CUST_CODE = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
			}
			else
			{
				sql += string.Format(@" AND (EXISTS (SELECT 1 
																				       FROM F190101 cc 
																				 INNER JOIN (SELECT * 
																				               FROM F192402 
																				  						WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
																				  		WHERE cc.DC_CODE = F700201.DC_CODE AND cc.GUP_CODE = F700201.GUP_CODE AND cc.CUST_CODE = F700201.CUST_CODE)) ", parameters.Count);
				parameters.Add(new SqlParameter("@p" + parameters.Count, Current.Staff));
			}
			if (!string.IsNullOrWhiteSpace(compDateBegin) && !string.IsNullOrWhiteSpace(compDateEnd))
			{
				sql += parameters.Combine(" AND F700201.COMPLAINT_DATE BETWEEN CONVERT (varchar,@p{0},111) AND convert(varchar,@p{1},111) ", compDateBegin, compDateEnd);
			}

			if (!string.IsNullOrWhiteSpace(status))
			{
				sql += " AND F700201.STATUS = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, status));
			}

			if (!string.IsNullOrWhiteSpace(compNo))
			{
				sql += " AND F700201.COMPLAINT_NO = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, compNo));
			}

			if (!string.IsNullOrWhiteSpace(compType))
			{
				sql += " AND F700201.COMPLAINT_TYPE = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, compType));
			}

			if (!string.IsNullOrWhiteSpace(depId))
			{
				sql += " AND F700201.DEP_ID = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, depId));
			}

			var query = SqlQuery<F700201Ex>(sql, parameters.ToArray());
			return query;
		}
	}
}