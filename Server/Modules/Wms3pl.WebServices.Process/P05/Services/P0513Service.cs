using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P0513Service
	{
		private WmsTransaction _wmsTransaction;
		public P0513Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 扣帳作業-批次
		public IQueryable<F0513WithF050801Batch> GetBatchDebitDatas(string dcCode, string gupCode, string custCode,bool notOrder,bool isB2c)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema);
			return f0513Repo.GetBatchDebitDatas(dcCode, gupCode, custCode, notOrder, isB2c);
		}
		#endregion
	}
}