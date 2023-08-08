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
	public partial class P1980Service
	{
		private WmsTransaction _wmsTransaction;
		public P1980Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region P1601020000 使用者被設定的作業區(倉別清單)
		public IQueryable<UserWarehouse> GetUserWarehouse(string userId, string gupCode, string custCode)
		{
			var repo = new F1980Repository(Schemas.CoreSchema);
			var result = repo.GetUserWarehouse(userId, gupCode,  custCode);
			return result;
		}
		#endregion
	}
}

