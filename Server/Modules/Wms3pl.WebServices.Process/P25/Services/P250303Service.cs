using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P25.Services
{
	public partial class P250303Service
	{
		private WmsTransaction _wmsTransaction;
		public P250303Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteSerialNo(string gupCode, string custCode, List<string> SnList)
		{
			var repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			repo.DeleteBySnList(gupCode, custCode, SnList);
			return new ExecuteResult(true);
		}
	}
}
