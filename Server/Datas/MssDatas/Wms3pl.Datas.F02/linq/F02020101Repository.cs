
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
	public partial class F02020101Repository : RepositoryBase<F02020101, Wms3plDbContext, F02020101Repository>
	{
		public F02020101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
	}
}
