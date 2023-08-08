using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191202Repository : RepositoryBase<F191202, Wms3plDbContext, F191202Repository>
	{
		public F191202Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
