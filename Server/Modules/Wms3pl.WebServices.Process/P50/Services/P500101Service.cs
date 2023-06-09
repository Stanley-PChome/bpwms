
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P50.Services
{
	public partial class P500101Service
	{
		private WmsTransaction _wmsTransaction;
		public P500101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F199007Data> GetProjectValuation(string dcCode, string gupCode, string custCode,
			DateTime? creDateS, DateTime? creDateE, string accProjectNo, DateTime? enableD, DateTime? disableD,
			string quoteNo, string status, string accProjectName)
		{
			var repF199007 = new F199007Repository(Schemas.CoreSchema);
			return repF199007.GetProjectValuation(dcCode, gupCode, custCode, creDateS, creDateE,
				accProjectNo, enableD, disableD, quoteNo, status, accProjectName);
		}

		public string GetProjectNo(string ordType)
		{
			var sharedService = new SharedService(_wmsTransaction);			// 單據號碼
			var ProjectNo = sharedService.GetNewOrdCode(ordType);
			return ProjectNo;
		}

		public ExecuteResult DeleteP5001010000(string dcCode, string gupCode, string custCode, string accProjectNo)
		{
			var f199007Repo = new F199007Repository(Schemas.CoreSchema, _wmsTransaction);

			var f199007 = f199007Repo.Find(item => item.DC_CODE == dcCode
							&& item.GUP_CODE == gupCode
							&& item.CUST_CODE == custCode
							&& item.ACC_PROJECT_NO == accProjectNo);

			if (f199007 == null)
				return new ExecuteResult(false, Properties.Resources.P500101Service_AccProjectNo_NotFound);

			if (f199007.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P500101Service_AccProjectNo_Status_CannotDelete);

			f199007.STATUS = "9";
			f199007Repo.Update(f199007);
			return new ExecuteResult(true, Properties.Resources.P190109Service_HasDeleted);
		}
	}
}

