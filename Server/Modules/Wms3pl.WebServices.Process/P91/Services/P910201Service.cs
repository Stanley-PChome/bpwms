
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910201Service
	{
		private WmsTransaction _wmsTransaction;
		public P910201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

	}
}

