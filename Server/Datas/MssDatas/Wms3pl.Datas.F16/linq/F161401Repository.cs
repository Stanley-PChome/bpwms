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
	public partial class F161401Repository : RepositoryBase<F161401, Wms3plDbContext, F161401Repository>
	{
		public F161401Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F161401 GetItem(string dcCode, string gupCode, string custCode, string returnNo)
		{
            return _db.F161401s
                .Where(x=>x.DC_CODE==dcCode)
                .Where(x=>x.GUP_CODE == gupCode)
                .Where(x=>x.CUST_CODE == custCode)
                .Where(x=>x.RETURN_NO == returnNo)
                .Select(x=>x)
                .FirstOrDefault();
		}
	}
}
