
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
	public partial class P711002Service
	{
		private WmsTransaction _wmsTransaction;
		public P711002Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F190002Data> GetF190002Data(string dcCode, string gupCode, string custCode)
		{
			var repo = new F190002Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF190002Data(dcCode, gupCode, custCode);
		}
	}
}

