
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710502Service
	{
		private WmsTransaction _wmsTransaction;
		public P710502Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteP7105020000(string dcCode, string accItemKindId, string ordType, string accKind, string accUnit, string delvAccType)
		{
			var f199002Repo = new F199002Repository(Schemas.CoreSchema, _wmsTransaction);

			var f199002 = f199002Repo.Find(item => item.DC_CODE == dcCode
							&& item.ACC_ITEM_KIND_ID == accItemKindId
							&& item.ORD_TYPE == ordType
							&& item.ACC_KIND == accKind
							&& item.ACC_UNIT == accUnit
							&& item.DELV_ACC_TYPE == delvAccType );

			if (f199002 == null)
				return new ExecuteResult(false, "此作業計價項目不存在，已無法刪除");

			if (f199002.STATUS != "0")
				return new ExecuteResult(false, "此此作業計價項目狀態已無法刪除");

			f199002.STATUS = "9";
			f199002Repo.Update(f199002);
			return new ExecuteResult(true, "已刪除");
		}
	}
}

