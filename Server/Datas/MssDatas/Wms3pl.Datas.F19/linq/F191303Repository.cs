using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F191303Repository : RepositoryBase<F191303, Wms3plDbContext, F191303Repository>
	{
		public F191303Repository(string connName, WmsTransaction wmsTransaction = null)
						 : base(connName, wmsTransaction)
		{
		}
	}
}
