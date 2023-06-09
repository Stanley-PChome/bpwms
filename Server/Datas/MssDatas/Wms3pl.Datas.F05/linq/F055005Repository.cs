using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F055005Repository: RepositoryBase<F055005, Wms3plDbContext, F055005Repository>
	{
		public IQueryable<HomeDeliveryNo> GetDatasByHomeDelivery(string dcCode, string gupCode, string custCode)
		{
			var result = _db.F055005s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROC_FLAG == "0")
				.Select(x => new HomeDeliveryNo
				{
					WmsNo = x.WMS_NO,
					TransportCode = x.PAST_NO,
					ShipmentTime = x.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss")
				});

			return result;
		}

	}
}
