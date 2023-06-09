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
	public partial class P1947Service
	{
		private WmsTransaction _wmsTransaction;
		public P1947Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F1947WithF194701> GetF1947WithF194701Datas(string dcCode, string gupCode, string custCode, string delvTime)
		{
			var repo = new F1947Repository(Schemas.CoreSchema);

			var result = repo.GetF1947WithF194701Datas(dcCode, gupCode, custCode, delvTime);

			return result;
		}

		public IQueryable<F1947JoinF194701> GetF1947WithF194701Datas(string ALL_ID, string DC_CODE)
        {
            var F1947 = new F1947Repository(Schemas.CoreSchema);
            var result = F1947.GetF1947JoinF194701Datas(ALL_ID, DC_CODE);
            return result;
        }
	}
}