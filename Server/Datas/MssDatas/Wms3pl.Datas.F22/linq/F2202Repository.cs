using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F22
{
	public partial class F2202Repository : RepositoryBase<F2202, Wms3plDbContext, F2202Repository>
	{
		public F2202Repository(string connName, WmsTransaction wmsTransaction = null) 
			: base(connName, wmsTransaction)
		{
		}
	}
}
