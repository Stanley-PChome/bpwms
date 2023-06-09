using System.Collections.Generic;
using System.Data.SqlClient;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;
using Wms3pl.Datas.F00.Interfaces;
using System.Linq;

namespace Wms3pl.Datas.F00
{
	public partial class F0091Repository : RepositoryBase<F0091, Wms3plDbContext, F0091Repository>, IApiLogRepository<F0091>
	{
		public F0091Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		}
	}
}
