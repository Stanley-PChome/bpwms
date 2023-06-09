using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F16140102Repository : RepositoryBase<F16140102, Wms3plDbContext, F16140102Repository>
	{
		public F16140102Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		} 
	}
}
