using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Common.Security;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P192018Service
	{
		private WmsTransaction _wmsTransaction;
		public P192018Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult UpdateF0003(F0003 f0003Data)
		{
			var repo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);

			var data = repo.Find(item => item.AP_NAME == f0003Data.AP_NAME
												&& item.DC_CODE == f0003Data.DC_CODE
												&& item.GUP_CODE == f0003Data.GUP_CODE
												&& item.CUST_CODE == f0003Data.CUST_CODE);

			if ((f0003Data.AP_NAME == "PackageLockPW" || f0003Data.AP_NAME == "DefaultPassword") &&
					!String.IsNullOrEmpty(f0003Data.SYS_PATH))
			{				
				data.SYS_PATH = CryptoUtility.GetHashString(f0003Data.SYS_PATH);
			}
			else
			{
				data.SYS_PATH = f0003Data.SYS_PATH;
			}

			data.FILENAME = f0003Data.FILENAME;
			data.FILETYPE = f0003Data.FILETYPE;
			data.DESCRIPT = f0003Data.DESCRIPT;
			data.UPD_NAME = f0003Data.UPD_NAME;
			data.UPD_STAFF = f0003Data.UPD_STAFF;
			data.UPD_DATE = f0003Data.UPD_DATE;
			repo.Update(data);

			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P192018Service_Update};
		}
	}
}
