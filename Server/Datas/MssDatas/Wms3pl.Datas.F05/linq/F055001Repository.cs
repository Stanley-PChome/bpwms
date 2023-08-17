using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
	public partial class F055001Repository : RepositoryBase<F055001, Wms3plDbContext, F055001Repository>
	{
		public F055001Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F055001> GetDatas(string wmsOrdNo, string gupCode, string custCode, string dcCode)
		{
			var result = _db.F055001s.Where(x => x.WMS_ORD_NO == wmsOrdNo &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.DC_CODE == dcCode);

			return result;
		}

		public IQueryable<F055001> GetF055001DatasByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F055001s.Where(x => x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.DC_CODE == dcCode &&
																					 x.PRINT_FLAG == null &&
																					 wmsOrdNos.Contains(x.WMS_ORD_NO));

			return result;
		}

		/// <summary>
		/// 取得新的包裝箱號
		/// </summary>
		/// <param name="wmsOrdNo"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		public short GetNewPackageBoxNo(string wmsOrdNo, string gupCode, string custCode, string dcCode)
		{
			var f055001Data = _db.F055001s.AsNoTracking().Where(x => x.WMS_ORD_NO == wmsOrdNo &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.DC_CODE == dcCode);

			var result = Convert.ToInt16((f055001Data.Count() > 0 ? f055001Data.Max(x => x.PACKAGE_BOX_NO) : 0) + 1);

			return result;
		}

		public IQueryable<F055001NewPackageBox> GetNewPackageBoxNos(string gupCode, string custCode, string dcCode, List<string> wmsOrdNos)
		{
			var result = _db.F055001s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
																													x.CUST_CODE == custCode &&
																													x.DC_CODE == dcCode &&
																													wmsOrdNos.Contains(x.WMS_ORD_NO))
																							.GroupBy(x => x.WMS_ORD_NO)
																							.Select(x => new F055001NewPackageBox
																							{
																								PACKAGE_BOX_NO = Convert.ToInt16(x.Max(z => z.PACKAGE_BOX_NO) + 1),
																								WMS_ORD_NO = x.Key
																							});

			return result;
		}

		/// <summary>
		/// 尋找自己最後一筆尚未列印的出貨包裝
		/// </summary>
		/// <param name="wmsOrdNo"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		public F055001 FindSelfF055001ByNoPrint(string wmsOrdNo, string gupCode, string custCode, string dcCode)
		{
			var result = _db.F055001s.Where(x => x.WMS_ORD_NO == wmsOrdNo &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.DC_CODE == dcCode &&
																					 x.PACKAGE_STAFF == Current.Staff &&
																					 x.PACKAGE_NAME == Current.StaffName)
																.OrderByDescending(x => x.PACKAGE_BOX_NO)
																.SingleOrDefault();

			return result;
		}

		public int GetCountByNotStatus(string wmsOrdNo, string gupCode, string custCode, string dcCode, string status)
		{
			var result = _db.F055001s.AsNoTracking().Where(x => x.WMS_ORD_NO == wmsOrdNo &&
																													x.GUP_CODE == gupCode &&
																													x.CUST_CODE == custCode &&
																													x.DC_CODE == dcCode &&
																													x.STATUS != status)
																							.Select(x => x.PACKAGE_BOX_NO)
																							.Count();

			return result;
		}


		public IQueryable<F055001> GetF055001(string dcCode, string gupCode, string custCode, string pastNo)
		{
			var result = _db.F055001s.Where(x => x.DC_CODE == dcCode
			&& x.GUP_CODE == gupCode
			&& x.CUST_CODE == custCode
			&& x.PAST_NO == pastNo).OrderByDescending(x => x.CRT_DATE);
			return result;
		}

        public F055001 GetData(string dcCode, string gupCode, string custCode, string wmsOrdNo, Int16 packageBoxNo)
        {
            return _db.F055001s.Where(x => x.DC_CODE == dcCode
            && x.GUP_CODE == gupCode
            && x.CUST_CODE == custCode
            && x.WMS_ORD_NO == wmsOrdNo
            && x.PACKAGE_BOX_NO == packageBoxNo).FirstOrDefault();
        }

        public F055001 GetDataExdulePackageBoxNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string isClosed, string isOribox, Int16 excludePackageBoxNo)
        {
            return _db.F055001s.Where(x => x.DC_CODE == dcCode
            && x.GUP_CODE == gupCode
            && x.CUST_CODE == custCode
            && x.WMS_ORD_NO == wmsOrdNo
            && x.IS_CLOSED == isClosed
            && x.IS_ORIBOX == isOribox
            && x.PACKAGE_BOX_NO != excludePackageBoxNo).FirstOrDefault();
        }
    }
}