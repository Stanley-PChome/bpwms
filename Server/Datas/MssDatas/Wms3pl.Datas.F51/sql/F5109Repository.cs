using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F51
{
	public partial class F5109Repository : RepositoryBase<F5109, Wms3plDbContext, F5109Repository>
	{
		public void DeleteByDate(DateTime settleDate)
		{
			var sql = @"
					DELETE F5109 WHERE CAL_DATE = @p0 ";
			var param = new SqlParameter[]
			{				
				new SqlParameter("@p0", settleDate)
			};
			ExecuteSqlCommand(sql, param);
		}
	}
}
