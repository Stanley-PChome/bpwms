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
	public partial class F161502Repository : RepositoryBase<F161502, Wms3plDbContext, F161502Repository>
	{
		public void DeleteGatherDataDetails(string dcCode, List<string> listGatherNo)
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
				foreach (var gatherNo in listGatherNo)
				{
					list.Add(string.Format("@p{0}", paramList.Count()));
					paramList.Add(gatherNo);
				}
				gatherNoCondition += string.Format("{0}) ", string.Join(",", list.ToArray()));
			}

			var sql = @"DELETE FROM F161502
									 WHERE DC_CODE = @p0 ";

			sql += gatherNoCondition;

			ExecuteSqlCommand(sql, paramList.ToArray());
		}
	}
}
