using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F16;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05030202Repository : RepositoryBase<F05030202, Wms3plDbContext, F05030202Repository>
	{
		public F05030202Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
		{
		}
		public IQueryable<F05030202> GetDatas(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
            var result = _db.F05030202s.Where(x => x.DC_CODE == dcCode &&
                                                   x.GUP_CODE == gupCode &&
                                                   x.CUST_CODE == custCode &&
                                                   wmsOrdNos.Contains(x.WMS_ORD_NO));

            return result;
		}

		public IQueryable<F05030202> GetDatasByOrdNos(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			return _db.F05030202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.CUST_CODE == custCode &&
			ordNos.Contains(x.ORD_NO));
		}
	}
}
