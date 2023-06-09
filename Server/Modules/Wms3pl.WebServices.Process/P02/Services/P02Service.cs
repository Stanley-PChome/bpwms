using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F01;

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P02Service
	{
		private WmsTransaction _wmsTransaction;
		public P02Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 依照進倉編號查找廠商資訊
		/// </summary>
		/// <param name="purchaseNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public IEnumerable<VendorInfo> GetVendorInfo(string purchaseNo, string dcCode, string gupCode, string custCode)
		{
			var repo = new F010201Repository(Schemas.CoreSchema);
			var result = repo.GetVendorInfo(purchaseNo, dcCode, gupCode, custCode);
			return result;
		}

	}
}
