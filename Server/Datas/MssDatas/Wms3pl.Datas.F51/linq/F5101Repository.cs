using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F5101Repository : RepositoryBase<F5101, Wms3plDbContext, F5101Repository>
	{
		public F5101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
		
	}

}
