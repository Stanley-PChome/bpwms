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
	public partial class P1913Service
	{
		private WmsTransaction _wmsTransaction;
		public P1913Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取報廢數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public int GetScrapItemStock(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var f1913Rep = new F1913Repository(Schemas.CoreSchema);
      return f1913Rep.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, new[] { itemCode }.ToList(), false, "D").Sum(x => (int)x.QTY);

    }

	}
}
