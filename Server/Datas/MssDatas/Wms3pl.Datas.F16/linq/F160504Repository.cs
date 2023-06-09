using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.F16
{
	public partial class F160504Repository : RepositoryBase<F160504, Wms3plDbContext, F160504Repository>
	{

		public F160504Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}



	}
}
