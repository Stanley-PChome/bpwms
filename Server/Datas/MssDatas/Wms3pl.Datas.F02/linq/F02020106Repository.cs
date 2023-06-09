
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F02
{
	public partial class F02020106Repository : RepositoryBase<F02020106, Wms3plDbContext, F02020106Repository>
	{
		public F02020106Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

    }
}
