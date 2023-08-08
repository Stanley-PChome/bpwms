using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190110Service
	{
		private WmsTransaction _wmsTransaction;
		public P190110Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertOrUpdateF1910(F1910 data,bool isAdd)
		{
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == data.GUP_CODE && x.CUST_CODE == data.CUST_CODE);
			data.CUST_CODE = f1909.ALLOWGUP_RETAILSHARE == "1" ? "0" : data.CUST_CODE;
			var f1910Repo = new F1910Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1910 = f1910Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GUP_CODE & x.CUST_CODE == data.CUST_CODE && x.RETAIL_CODE == data.RETAIL_CODE).FirstOrDefault();
			if (isAdd && f1910 != null)
				return new ExecuteResult(false, Properties.Resources.P190110Service_Retail_Duplicate);
			if (!isAdd && f1910 == null)
				return new ExecuteResult(false, Properties.Resources.DataDelete);

			if (isAdd)
				f1910Repo.Add(data);
			else
				f1910Repo.UpdateHasKey(data, f1910.CHANNEL);

			return new ExecuteResult(true);
		}
	}
}
