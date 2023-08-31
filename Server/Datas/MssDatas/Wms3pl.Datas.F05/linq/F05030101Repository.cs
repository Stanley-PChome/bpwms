using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.F05
{
	public partial class F05030101Repository : RepositoryBase<F05030101, Wms3plDbContext, F05030101Repository>
	{
		public F05030101Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 以訂單編號來取得尚未揀貨的揀貨明細
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNo"></param>
		/// <returns></returns>
		public IQueryable<F051202> GetNonPickF051202ByOrdNo(string dcCode, string gupCode, string custCode, List<string> ordNoList)
		{
			var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																																	 x.GUP_CODE == gupCode &&
																																	 x.CUST_CODE == custCode &&
																																	 ordNoList.Contains(x.ORD_NO));

			var f051202Data = _db.F051202s.Where(x => x.DC_CODE == dcCode &&
																								x.GUP_CODE == gupCode &&
																								x.CUST_CODE == custCode &&
																								x.PICK_STATUS == "0" &&
																								f05030101Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

			var result = (from C in f05030101Data
										join D in f051202Data
										on new { C.WMS_ORD_NO, C.CUST_CODE, C.GUP_CODE, C.DC_CODE } equals new { D.WMS_ORD_NO, D.CUST_CODE, D.GUP_CODE, D.DC_CODE }
										select D).Distinct();

			return result;
		}

		/// <summary>
		/// 取得訂單併單的所有訂單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <returns></returns>
		public IQueryable<F05030101> GetMergerOrders(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																											 x.GUP_CODE == gupCode &&
																											 x.CUST_CODE == custCode &&
																											 x.ORD_NO == ordNo);

			var f050801Data = _db.F050801s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 f05030101Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

			var f05030101Data2 = _db.F05030101s.Where(x => x.DC_CODE == dcCode &&
																										x.GUP_CODE == gupCode &&
																										x.CUST_CODE == custCode &&
																										f050801Data.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO));

			var result = from A in f05030101Data
									 join B in f050801Data
									 on new { A.WMS_ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.WMS_ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
									 join C in f05030101Data2
									 on new { B.WMS_ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE } equals new { C.WMS_ORD_NO, C.DC_CODE, C.GUP_CODE, C.CUST_CODE }
									 select C;

			return result;

		}
		#region 新增<<重新取得託運單號>>
		public IQueryable<F05030101> GetDatas(string dcCode, string gupCode, string custCode, string wmsNo, string ordNo)
		{
			var result = _db.F05030101s.Where(x => x.GUP_CODE == gupCode &&
																						 x.CUST_CODE == custCode &&
																						 x.DC_CODE == dcCode &&
																						 x.WMS_ORD_NO == wmsNo &&
																						 x.ORD_NO == ordNo);

			return result;
		}

		#endregion

		public IQueryable<F05030101> GetDatas(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F05030101s.Where(x => x.DC_CODE == dcCode &&
																						 x.GUP_CODE == gupCode &&
																						 x.CUST_CODE == custCode &&
																						 wmsOrdNos.Contains(x.WMS_ORD_NO));

			return result;
		}

		public IQueryable<F05030101> GetDatasForF0011BindDatas(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F05030101s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && wmsOrdNos.Contains(x.WMS_ORD_NO));

			return result;
		}

		public IQueryable<F05030101> GetDatasForWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F05030101s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && wmsOrdNos.Contains(x.WMS_ORD_NO));

			return result;
		}

		public IQueryable<F05030101> GetDatasForOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var ordNos = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && wmsOrdNos.Contains(x.WMS_ORD_NO)).Select(x => x.ORD_NO).Distinct();

			return _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && ordNos.Contains(x.ORD_NO));
		}

		public IQueryable<F05030101> GetDatasForOrdNoList(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			var result = _db.F05030101s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && ordNos.Contains(x.ORD_NO));

			return result;
		}
	}
}