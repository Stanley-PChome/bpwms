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
	public partial class F161501Repository : RepositoryBase<F161501, Wms3plDbContext, F161501Repository>
	{
		public IQueryable<F161501> GetGatherItems(string dcCode, string gatherDateStart, string gatherDateEnd, string gatherNoStart, string gatherNoEnd, string fileName)
		{
			string gatherDateCondition = string.Empty;
			string gatherNoCondition = string.Empty;
			string fileNameCondition = (string.IsNullOrEmpty(fileName)) ? string.Empty : "AND FILE_NAME LIKE '" + fileName + "%' ";
			var paramers = new List<SqlParameter>();
			paramers.Add(new SqlParameter("@p0", dcCode));

			if (!string.IsNullOrEmpty(gatherDateStart) || !string.IsNullOrEmpty(gatherDateEnd))
			{
				if (!string.IsNullOrEmpty(gatherDateStart))
				{
					gatherDateCondition = string.Format("AND convert(varchar,GATHER_DATE,111) >= convert(varchar,@p{0},111) ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gatherDateStart));
				}
				if (!string.IsNullOrEmpty(gatherDateEnd))
				{
					gatherDateCondition += string.Format("AND convert(varchar,GATHER_DATE,111) <= convert(varchar,@p{0},111) ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gatherDateEnd));
				}
			}
			if (!string.IsNullOrEmpty(gatherNoStart) || !string.IsNullOrEmpty(gatherNoEnd))
			{
				if (!string.IsNullOrEmpty(gatherNoStart))
				{
					gatherNoCondition = string.Format("AND GATHER_NO >= @p{0} ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gatherNoStart));
				}
				if (!string.IsNullOrEmpty(gatherNoEnd))
				{
					gatherNoCondition += string.Format("AND GATHER_NO <= @p{0} ", paramers.Count());
					paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gatherNoEnd));
				}
			}
			
			var sql = @"SELECT * 
										FROM F161501
									 WHERE DC_CODE = @p0  ";

			sql += gatherDateCondition;
			sql += gatherNoCondition;
			sql += fileNameCondition;

			var result = SqlQuery<F161501>(sql, paramers.ToArray()).AsQueryable();
			return result;
		}

		public void DeleteGatherDatas(string dcCode, List<string> listGatherNo)
		{
			var paramList = new List<object>
			{
				dcCode
			};

			string gatherNoCondition = string.Empty;
			if (listGatherNo != null && listGatherNo.Any())
			{
				var list = new List<string>();
				gatherNoCondition = "AND GATHER_NO IN ( ";
				foreach(var gatherNo in listGatherNo)
				{
					list.Add(string.Format("@p{0}", paramList.Count()));
					paramList.Add(gatherNo);
				}
				gatherNoCondition += string.Format("{0}) ",string.Join(",", list.ToArray()));
			}

			var sql = @"DELETE FROM F161501
									 WHERE DC_CODE = @p0 ";

			sql += gatherNoCondition;

			ExecuteSqlCommand(sql, paramList.ToArray());
		}
	}
}
