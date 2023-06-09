using System;
using System.Linq;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P70.Services
{
	public partial class P700105Service
	{
		private WmsTransaction _wmsTransaction;
		public P700105Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F700501Ex> GetF700501Ex(string dcCode, string dateBegin,
			string dateEnd, string scheduleType)
		{
			var repo = new F700501Repository(Schemas.CoreSchema);
			var result = repo.GetF700501Ex(dcCode, Convert.ToDateTime(dateBegin),
				Convert.ToDateTime(dateEnd), scheduleType);
			return result;
		}
	}
}
