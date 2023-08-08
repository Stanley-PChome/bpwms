using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050301Service
	{
		private WmsTransaction _wmsTransaction;
		public P050301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 未出貨訂單查詢
		public IQueryable<F050801NoShipOrders> GetF050801NoShipOrders(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string status, string ordNo, string custOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801NoShipOrders(dcCode, gupCode, custCode, DateTime.Parse(delvDate), pickTime, status, ordNo, custOrdNo);
		}
		#endregion

	}
}