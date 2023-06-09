using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
	public partial class F055002Repository : RepositoryBase<F055002, Wms3plDbContext, F055002Repository>
	{
		public F055002Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 依出貨單號/ 包裝箱號取得最大一筆包裝箱序號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <returns></returns>
		public int GetLatestPackageBoxSeq(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo)
		{
			var result = _db.F055002s.AsNoTracking().Where(x => x.WMS_ORD_NO == wmsOrdNo &&
																													x.DC_CODE == dcCode &&
																													x.GUP_CODE == gupCode &&
																													x.CUST_CODE == custCode &&
																													x.PACKAGE_BOX_NO == packageBoxNo)
																							.OrderByDescending(x => x.PACKAGE_BOX_SEQ)
																							.FirstOrDefault();

			return result == null ? 0 : result.PACKAGE_BOX_SEQ;
		}

		public IQueryable<F055002> GetPackageBoxSeqsByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F055002s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 wmsOrdNos.Contains(x.WMS_ORD_NO));

			return result;
		}

		public IQueryable<F055002WithGridLog> GetF055002WithGridLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var result = _db.F055002s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																												 x.GUP_CODE == gupCode &&
																												 x.CUST_CODE == custCode &&
																												 x.WMS_ORD_NO == wmsOrdNo)
																							.GroupBy(x => new { x.CLIENT_PC, x.CRT_NAME, x.CRT_STAFF })
																							.Select(x => new F055002WithGridLog
																							{
																								CRT_STAFF = x.Key.CRT_STAFF,
																								CRT_NAME = x.Key.CRT_NAME,
																								CLIENT_PC = x.Key.CLIENT_PC,
																								CRT_DATE = x.Min(z => z.CRT_DATE),
																								UPD_DATE = (x.Max(z => z.CRT_DATE) > x.Max(z => z.UPD_DATE) ?
																									x.Max(z => z.CRT_DATE) : x.Max(z => z.UPD_DATE)) ?? x.Max(z => z.CRT_DATE)
																							});

			return result;
		}

		public IQueryable<F055002> GetDatasByWcsExecute(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			return _db.F055002s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.WMS_ORD_NO == wmsOrdNo &&
			!string.IsNullOrWhiteSpace(x.SERIAL_NO));
		}

		public IQueryable<F055002> GetDatasByOrdSeqs(string dcCode, string gupCode, string custCode, string ordNo, List<string> ordSeqs)
		{
			return _db.F055002s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.ORD_NO == ordNo &&
			ordSeqs.Contains(x.ORD_SEQ));
		}
	}
}