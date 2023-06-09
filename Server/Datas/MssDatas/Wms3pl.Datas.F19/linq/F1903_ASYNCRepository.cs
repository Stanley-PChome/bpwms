using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1903_ASYNCRepository : RepositoryBase<F1903_ASYNC, Wms3plDbContext, F1903_ASYNCRepository>
	{
		public F1903_ASYNCRepository(string connName, WmsTransaction wmsTransaction = null)
		: base(connName, wmsTransaction)
		{

		}
	}
}
