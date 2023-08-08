
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P711003Service
	{
		private WmsTransaction _wmsTransaction;
		public P711003Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取得出貨單批次參數維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public IQueryable<F050004WithF190001> GetF050004WithF190001s(string dcCode, string gupCode, string custCode)
		{
			var repo = new F050004Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF050004WithF190001s(dcCode, gupCode, custCode);
		}

	}
}

