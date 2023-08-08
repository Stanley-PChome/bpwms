
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710705Service
	{
		private WmsTransaction _wmsTransaction;
		public P710705Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P710705BackWarehouseInventory> GetP710705BackWarehouseInventory(string dcCode, string gupCode, string custCode, string vnrCode, string account)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetP710705BackWarehouseInventory(dcCode, gupCode, custCode, vnrCode, account);
		}


		public IQueryable<P710705Availability> GetP710705Availability(string dcCode, string gupCode, string custCode, string inventoryDate, string account)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			var data = repF1913.GetP710705Availability(dcCode, gupCode, custCode, inventoryDate, account).ToList();
			var index = 1;
			var result = (from d in data
				group d by new {d.CUST_CODE, d.CUST_NAME, d.GUP_CODE, d.GUP_NAME, d.WAREHOUSE_ID, d.WAREHOUSE_NAME}
				into g
				let sumUsefulVolumn = g.Sum(x => x.USEFUL_VOLUMN)
				let i = index++
				select new P710705Availability
				{
					ROWNUM = i,
					GUP_NAME = g.Key.GUP_NAME,
					GUP_CODE = g.Key.GUP_CODE,
					CUST_CODE = g.Key.CUST_CODE,
					CUST_NAME = g.Key.CUST_NAME,
					WAREHOUSE_NAME = g.Key.WAREHOUSE_NAME,
					WAREHOUSE_ID = g.Key.WAREHOUSE_ID,
					INVENTORYDATE = g.Max(x => x.INVENTORYDATE),
					COUNTF1912 = g.Count(),
					COUNTF1913NULL = g.Count(x => x.F1913_LOC_CODE == null),
					COUNTF1913 = g.Count(x => x.F1913_LOC_CODE != null),
					//儲位已用容積/儲位可用容積
					FILLRATE = sumUsefulVolumn == 0 ? 0 : Math.Round((g.Sum(x => x.USED_VOLUMN) / sumUsefulVolumn)*100, 2),
					RENT_END_DATE = g.Max(x => x.RENT_END_DATE)
				}).ToList();

			return result.AsQueryable();
		}

		public IQueryable<P710705ChangeDetail> GetP710705ChangeDetail(string warehouseId, string srcLocCode, string tarLocCode, string itemCodes, DateTime? enterDateBegin, DateTime? enterDateEnd)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetP710705ChangeDetail(warehouseId, srcLocCode, tarLocCode, itemCodes, enterDateBegin, enterDateEnd);
		}

		public IQueryable<P710705WarehouseDetail> GetP710705WarehouseDetail(string gupCode, string custCode, string warehouseId, string srcLocCode, string tarLocCode, string itemCode, string account)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetP710705WarehouseDetail(gupCode, custCode, warehouseId, srcLocCode, tarLocCode, itemCode, account);
		}

	}
}

