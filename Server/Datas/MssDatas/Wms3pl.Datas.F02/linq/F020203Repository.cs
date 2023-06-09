using System;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F020203Repository : RepositoryBase<F020203, Wms3plDbContext, F020203Repository>
	{
		public F020203Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F020203 GetDataByKey(string dcCode, string gupCode, string custCode, string itemCode, DateTime rtDate)
		{
			return _db.F020203s.Where(x => x.DC_CODE == dcCode &&
																		 x.GUP_CODE == gupCode &&
																		 x.CUST_CODE == custCode &&
																		 x.ITEM_CODE == itemCode &&
																		 x.RT_DATE == rtDate).FirstOrDefault();
		}
	}
}
