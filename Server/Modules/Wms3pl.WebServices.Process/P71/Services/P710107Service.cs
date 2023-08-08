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
	public partial class P710107Service
	{
		private WmsTransaction _wmsTransaction;
		public P710107Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F1912StatisticReport> GetLocStatisticForLocControl(string dcCode, string gupCode, string custCode
			, string warehouseType, string warehouseId, string account)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = repo.GetLocStatisticForLocControl(dcCode, gupCode, custCode, warehouseType, warehouseId, account).ToList();

			// 計算百分比
			foreach (var p in result)
			{
				// 找到DC層的資料
				var dc = result.Find(x => x.DC_CODE.Equals(p.DC_CODE) && string.IsNullOrEmpty(x.GUP_CODE));
				if (dc != null)
				{
					p.PERCENTAGE = 100 * (p.LOCCOUNT / dc.LOCCOUNT);
				}
				// 如果GUP_NAME/ CUST_NAME為NULL, 表示為共用的資料 (在F1912裡的GUP是0)
				p.GUP_NAME = string.IsNullOrEmpty(p.GUP_NAME) ? "共用" : p.GUP_NAME;
				p.CUST_NAME = string.IsNullOrEmpty(p.CUST_NAME) ? "共用" : p.CUST_NAME;
			}

			return result.AsQueryable();
		}
	}
}
