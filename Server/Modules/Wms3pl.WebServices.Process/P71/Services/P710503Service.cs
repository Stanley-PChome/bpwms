
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710503Service
	{
		private WmsTransaction _wmsTransaction;
		public P710503Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteP7105030000(string dcCode, string accItemKindId, string accKind, string accUnit, string delvAccType)
		{
			var f199003Repo = new F199003Repository(Schemas.CoreSchema, _wmsTransaction);

			var f199003 = f199003Repo.Find(item => item.DC_CODE == dcCode
							&& item.ACC_ITEM_KIND_ID == accItemKindId
							&& item.ACC_KIND == accKind
							&& item.ACC_UNIT == accUnit
							&& item.DELV_ACC_TYPE == delvAccType && item.STATUS =="0");

			if (f199003 == null)
				return new ExecuteResult(false, "此出貨計價項目不存在，已無法刪除");

			if (f199003.STATUS != "0")
				return new ExecuteResult(false, "此此出貨計價項目狀態已無法刪除");

			f199003.STATUS = "9";
			f199003Repo.Update(f199003);
			return new ExecuteResult(true, "已刪除");
		}
	}
}

