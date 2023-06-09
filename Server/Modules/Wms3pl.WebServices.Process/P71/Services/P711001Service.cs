
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
	public partial class P711001Service
	{
		private WmsTransaction _wmsTransaction;
		public P711001Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取得貨主單據維護的查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public IQueryable<F190001Data> GetF190001Data(string dcCode, string gupCode, string custCode, string ticketType)
		{
			var repo = new F190001Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF190001Data(dcCode, gupCode, custCode, ticketType);
		}
	}
}

