﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075110Repository : RepositoryBase<F075110, Wms3plDbContext, F075110Repository>
	{
		public F075110Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

	}
}
