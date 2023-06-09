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
	public partial class F16050301Repository : RepositoryBase<F16050301, Wms3plDbContext, F16050301Repository>
	{
		public F16050301Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{

		}
	}
}
