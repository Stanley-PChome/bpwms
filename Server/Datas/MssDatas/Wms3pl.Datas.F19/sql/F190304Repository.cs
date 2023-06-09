using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191304Repository : RepositoryBase<F191304, Wms3plDbContext, F191304Repository>
	{
		public void DelF191304ByKey(string dcCode, string transactionNo)
		{
			var sql = @" DELETE FROM F191304 
                           WHERE DC_CODE =@p0 
                             AND TRANSACTION_NO =@p1 ";
			var param = new object[] { dcCode, transactionNo };
			ExecuteSqlCommand(sql, param);
		}
	}
}
