using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
	public partial class F055004Repository : RepositoryBase<F055004, Wms3plDbContext, F055004Repository>
	{
		public F055004Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F055004 GetDatasById(long id)
		{
			var result = _db.F055004s.Where(x => x.ID == id).FirstOrDefault();
			return result;
		}
	}
}