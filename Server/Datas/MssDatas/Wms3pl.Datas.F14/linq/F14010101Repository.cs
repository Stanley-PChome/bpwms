using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F14010101Repository : RepositoryBase<F14010101, Wms3plDbContext, F14010101Repository>
	{
		public F14010101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F14010101> GetDatasByHasSerialNo(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
            var q = from f in _db.F14010101s
                    where f.DC_CODE == dcCode 
                            && f.GUP_CODE == gupCode 
                            && f.CUST_CODE == custCode 
                            && f.INVENTORY_NO == inventoryNo 
                            && f.SERIAL_NO != "0"
                    select f;
            return q;
		}
	}
}
