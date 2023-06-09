using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public class SerialNoCancelService
	{
		private WmsTransaction _wmsTransaction;
		public SerialNoCancelService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public void CreateSerialNoCancel(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var f060501Repo = new F060501Repository(Schemas.CoreSchema, _wmsTransaction);

			var addDatas = f060501Repo.GetCreateDatas(dcCode, gupCode, custCode, wmsOrdNos);

			if (addDatas.Any())
				f060501Repo.BulkInsert(addDatas);
		}
	}
}
