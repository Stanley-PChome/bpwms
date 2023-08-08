using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F20
{
	public partial class F20010201Repository : RepositoryBase<F20010201, Wms3plDbContext, F20010201Repository>
	{
		public F20010201Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
