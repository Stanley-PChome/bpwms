using System.Collections.Generic;
using System.Data.SqlClient;

namespace Wms3pl.Datas.F06
{
	public partial class F060602Repository
	{
		public void DeleteData(long f060601Id)
		{
			var param = new List<SqlParameter> { new SqlParameter("@p0", f060601Id) };
			var sql = $" DELETE FROM F060602 WHERE F060601_ID = @p0 ";
			ExecuteSqlCommand(sql, param.ToArray());
		}
	}
}
