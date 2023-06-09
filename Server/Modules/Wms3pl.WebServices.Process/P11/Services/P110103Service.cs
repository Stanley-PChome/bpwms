using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Process.P11.Services
{
	public partial class P110103Service
	{
		private WmsTransaction _wmsTransaction;
		public P110103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
	}
}

