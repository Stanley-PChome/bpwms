using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191305Repository : RepositoryBase<F191305, Wms3plDbContext, F191305Repository>
	{
		public IQueryable<F191305> GetNoProcessDatas()
		{
			var sql = @" SELECT TOP(5000) *
                    FROM F191305
                   WHERE STATUS = '0' ";
			return SqlQuery<F191305>(sql);
		}

	}
}
