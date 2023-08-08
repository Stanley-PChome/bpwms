using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710106Service
	{
		private WmsTransaction _wmsTransaction;
		public P710106Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取得異動記錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="areaId"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<F191202Ex> GetLocTransactionLog(string dcCode, string gupCode, string custCode
			, string locCode, string startDt, string endDt, string locStatus, string warehouseType, string account)
		{
			var repo = new F191202Repository(Schemas.CoreSchema);
			DateTime start;
			DateTime end;
			DateTime.TryParse(startDt, out start);
			DateTime.TryParse(endDt, out end);
			var result = repo.GetLogs(dcCode, gupCode, custCode, locCode, start, end, locStatus, warehouseType, account);
			return result.AsQueryable();
		}
	}
}
