
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190109Service
	{
		private WmsTransaction _wmsTransaction;
		public P190109Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteP190109(string gupCode, string vnrCode)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransaction);

			var f1908 = f1908Repo.Find(item => item.GUP_CODE == gupCode
										&& item.VNR_CODE == vnrCode);

			if (f1908 == null)
				return new ExecuteResult(false, Properties.Resources.P190109Service_VNR_NotFound);

			if (f1908.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P190109Service_VNRStatus_CannotDelete);

			//檢查進倉單中是否有該廠商的訂單，若沒有可以做刪除
			if (f010201Repo.ExistsNonCancelByVendor(gupCode, vnrCode))
				return new ExecuteResult(false, Properties.Resources.P190109Service_VNRUsing_CannotDelete);

			f1908.STATUS = "9";
			f1908Repo.Update(f1908);
			return new ExecuteResult(true, Properties.Resources.P190109Service_HasDeleted);
		}
	}
}

