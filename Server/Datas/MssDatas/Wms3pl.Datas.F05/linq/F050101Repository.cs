using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
	public partial class F050101Repository : RepositoryBase<F050101, Wms3plDbContext, F050101Repository>
	{
		public F050101Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050101> GetDatasByCustOrdNoAndRetailCodeNotCancel(string dcCode, string gupCode, string custCode, string custOrdNo, string retailCode)
		{
			var result = _db.F050101s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.CUST_ORD_NO == custOrdNo &&
																					 x.RETAIL_CODE == retailCode &&
																					 x.STATUS != "9");

			return result;
		}

		public IQueryable<F050101> GetDatasByUnApprove(string gupCode, string custCode)
		{
			var result = _db.F050101s.Where(x => x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.STATUS == "0");

			return result;
		}


		public F050101 GetDataByOrdNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var result = _db.F050101s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.ORD_NO == ordNo &&
																					 x.STATUS != "9").FirstOrDefault();

			return result;
		}

		public IQueryable<P8202050000_F050101> GetDatasWithF050301(string dcCode, string gupCode, string custCode, List<string> custOrdNos)
		{
			var f050101Data = _db.F050101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.STATUS != "9" &&
																															 custOrdNos.Contains(x.CUST_ORD_NO));

			var ordNos = f050101Data.Select(z => z.ORD_NO).ToList();

			var f050301Data = _db.F050301s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 ordNos.Contains(x.ORD_NO));

			var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																																	 x.GUP_CODE == gupCode &&
																																	 x.CUST_CODE == custCode &&
																																	 ordNos.Contains(x.ORD_NO));


			var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																																x.GUP_CODE == gupCode &&
																																x.CUST_CODE == custCode &&
																																f05030101Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

			var data = (from C in f05030101Data
									join D in f050801Data
									on new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.WMS_ORD_NO } equals new { D.DC_CODE, D.GUP_CODE, D.CUST_CODE, D.WMS_ORD_NO }
									join B in f050301Data
									on new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO }
									select new
									{
										F050301 = B,
										F050801 = D
									}).GroupBy(x => x.F050301).Select(x => new { F050301 = x.Key, F050801StatusList = x.Select(z => z.F050801.STATUS).ToList() });


			var result = from A in f050101Data
									 join B in data
									 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.F050301.DC_CODE, B.F050301.GUP_CODE, B.F050301.CUST_CODE, B.F050301.ORD_NO } into subB
									 from B in subB.DefaultIfEmpty()
									 select new P8202050000_F050101
									 {
										 ORD_NO = A.ORD_NO,
										 CUST_ORD_NO = A.CUST_ORD_NO,
										 F050801_STATUS_List = B == null ? new List<decimal>() : B.F050801StatusList,
										 PROC_FLAG = B != null ? B.F050301.PROC_FLAG ?? "-1" : "-1",
										 F050301 = B == null ? null : B.F050301
									 };

			return result;
		}
	}
}