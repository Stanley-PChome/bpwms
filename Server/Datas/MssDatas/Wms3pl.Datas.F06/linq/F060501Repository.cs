using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060501Repository : RepositoryBase<F060501, Wms3plDbContext, F060501Repository>
	{
		public F060501Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F060501> GetWcsExecuteDatas(string dcCode, string gupCode, string custCode, List<string> statusList, int midApiRelmt)
		{
			return _db.F060501s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			statusList.Contains(x.STATUS) &&
			x.RESENT_CNT < midApiRelmt);
		}

		public List<F060501> GetCreateDatas(string dcCode, string gupCode, string custCode, List<string> wmsNos)
		{
			var result = new List<F060501>();
			
				var f055002s = _db.F055002s.AsNoTracking().Where(x =>
				x.DC_CODE == dcCode &&
				x.GUP_CODE == gupCode &&
				x.CUST_CODE == custCode &&
				wmsNos.Contains(x.WMS_ORD_NO) &&
				!string.IsNullOrWhiteSpace(x.SERIAL_NO));

				if (f055002s.Any())
				{
					var f051202s = _db.F051202s.AsNoTracking().Where(x =>
				x.DC_CODE == dcCode &&
				x.GUP_CODE == gupCode &&
				x.CUST_CODE == custCode &&
				wmsNos.Contains(x.WMS_ORD_NO) &&
				x.PICK_STATUS != "9");

					var f1912s = _db.F1912s.AsNoTracking().Where(x =>
					x.DC_CODE == dcCode &&
					f051202s.Select(z => z.PICK_LOC).Contains(x.LOC_CODE));

					result = (from A in f055002s
										join B in f051202s
										on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.WMS_ORD_NO }
										join C in f1912s
										on new { B.DC_CODE, LOC_CODE = B.PICK_LOC } equals new { C.DC_CODE, C.LOC_CODE }
										select new
										{
											DC_CODE = dcCode,
											GUP_CODE = gupCode,
											CUST_CODE = custCode,
											WMS_NO = A.WMS_ORD_NO,
											WAREHOUSE_ID = C.WAREHOUSE_ID
										}).GroupBy(x => new { x.WMS_NO, x.WAREHOUSE_ID }).Select(x => new F060501 {
											DC_CODE = dcCode,
											GUP_CODE = gupCode,
											CUST_CODE = custCode,
											WMS_NO = x.Key.WMS_NO,
											WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
											STATUS = "0"
										}).ToList();

					if (result.Any())
					{
						// 找出目前尚未執行的序號取消任務
						var statusList = new List<string> { "0", "1", "T" };
						var f060501s = _db.F060501s.AsNoTracking().Where(x =>
						result.Any(z => z.DC_CODE == x.DC_CODE &&
						z.GUP_CODE == x.GUP_CODE &&
						z.CUST_CODE == x.CUST_CODE &&
						z.WMS_NO == x.WMS_NO &&
						z.WAREHOUSE_ID == x.WAREHOUSE_ID) &&
						statusList.Contains(x.STATUS));

						if (f060501s.Any())
						{
							// 排除尚未執行的序號取消任務
							result = result.Where(x => !f060501s.Any(z =>
							x.WAREHOUSE_ID == z.WAREHOUSE_ID &&
							x.WMS_NO == z.WMS_NO)).ToList();
						}
					}
				}

			return result.OrderBy(x => x.WAREHOUSE_ID).ToList();
		}
	}
}
