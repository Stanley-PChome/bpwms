using Wms3pl.WebServices.DataCommon;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Wms3pl.Datas.F16
{
	public partial class F16140201Repository : RepositoryBase<F16140201, Wms3plDbContext, F16140201Repository>
	{
		public F16140201Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		} 
    
	}
}
