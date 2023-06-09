
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710808Service
	{
		private WmsTransaction _wmsTransaction;
		public P710808Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F051201Progress> GetOrderProcessProgress(string dcCode, string gupCode, string custCode, string pickTime, string delvDate)
		{
			var f051201Repository = new F051201Repository(Schemas.CoreSchema);
			var delv_Date = (!string.IsNullOrEmpty(delvDate)) ? Convert.ToDateTime(delvDate).Date : (DateTime?)null;
			return f051201Repository.GetOrderProcessProgress(dcCode, gupCode, custCode, pickTime, delv_Date);
		}

		public IQueryable<F050301ProgressData> GetProgressData(string dcCode, string gupCode, string custCode, string pickTime, string delvDate, string pickOrdNo)
		{
			var f050301Repository = new F050301Repository(Schemas.CoreSchema);
			var delv_Date = (!string.IsNullOrEmpty(delvDate)) ? Convert.ToDateTime(delvDate).Date : (DateTime?)null;
			return f050301Repository.GetProgressData(dcCode, gupCode, custCode, pickTime, delv_Date, pickOrdNo);
		}
	}
}

