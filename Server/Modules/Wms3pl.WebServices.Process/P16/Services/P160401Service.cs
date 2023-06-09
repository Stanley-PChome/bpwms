
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160401Service
	{
		private WmsTransaction _wmsTransaction;
		public P160401Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteP160401(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var f160401Repo = new F160401Repository(Schemas.CoreSchema, _wmsTransaction);

			var f160401 = f160401Repo.Find(item => item.DC_CODE == dcCode
							&& item.GUP_CODE == gupCode
							&& item.CUST_CODE == custCode
							&& item.SCRAP_NO == scrapNo);

			if (f160401 == null)
				return new ExecuteResult(false, Properties.Resources.P160401Service_Scrap_No_NotFound);

			if (f160401.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P160401Service_Scrap_No_Status_CannotDeleted);

			f160401.STATUS = "9";
			f160401Repo.Update(f160401);
			return new ExecuteResult(true, Properties.Resources.P190109Service_HasDeleted);
		}
	}
}

