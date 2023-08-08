using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P161201Service
	{
		private WmsTransaction _wmsTransaction;
		public P161201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F161201DetailDatas> GetF161201DetailDatas(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema);
			return f161201Repo.GetF161201DetailDatas(dcCode, gupCode, custCode, returnNo);
		}

	}
}

