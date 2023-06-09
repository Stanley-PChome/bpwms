using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.ApiEntities;

namespace Wms3pl.Datas.F01
{
	public partial class F010201Repository : RepositoryBase<F010201, Wms3plDbContext, F010201Repository>
	{
		public F010201Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F010201QueryData> GetF010201SourceNo(string sourceNo)
		{
			var f010201Datas = _db.F010201s.AsNoTracking()
					.Where(x => x.SOURCE_NO == sourceNo)
					.Select(x => new F010201QueryData
					{
						STOCK_NO = x.STOCK_NO
					}).ToList();

			// RowNum
			for (int i = 0; i < f010201Datas.Count; i++) { f010201Datas[i].ROWNUM = i + 1; }

			return f010201Datas.AsQueryable();
		}

		public IEnumerable<VendorInfo> GetVendorInfo(string purchaseNo, string dcCode, string gupCode, string custCode)
		{
			var f010201sData = _db.F010201s.AsNoTracking().Where(x => x.STOCK_NO == purchaseNo &&
																																x.DC_CODE == dcCode &&
																																x.GUP_CODE == gupCode &&
																																x.CUST_CODE == custCode);

			var f1908sData = _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
																														x.CUST_CODE == custCode);

			var result = (from A in f010201sData
										join B in f1908sData
										on A.VNR_CODE equals B.VNR_CODE into subB
										from B in subB.DefaultIfEmpty()
										select new VendorInfo
										{
											VNR_CODE = A.VNR_CODE,
											VNR_NAME = B.VNR_NAME ?? null
										}).AsEnumerable();

			return result;
		}

		/// <summary>
		/// 存在尚未取消的進倉單商品
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public bool ExistsNonCancelByItemCode(string gupCode, string custCode, string itemCode)
		{
			var f010201sData = _db.F010201s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
																																x.CUST_CODE == custCode &&
																																x.STATUS != "9" &&
																																x.STOCK_DATE >= DateTime.Today.AddYears(-1));

			var f010202sData = _db.F010202s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
																																x.CUST_CODE == custCode &&
																																x.ITEM_CODE == itemCode);


			var data = (from A in f010201sData
									join B in f010202sData
									on new { A.STOCK_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.STOCK_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
									select new { A });

			return data.Any();
		}

		/// <summary>
		/// 存在尚未取消的進倉單廠商
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public bool ExistsNonCancelByVendor(string gupCode, string vnrCode)
		{
			var f010201Data = _db.F010201s.AsNoTracking().Where(x => x.STATUS != "9" &&
																															 x.GUP_CODE == gupCode &&
																															 x.VNR_CODE == vnrCode &&
																															 x.STOCK_DATE >= DateTime.Today.AddYears(-1)).FirstOrDefault();
			return f010201Data != null;
		}

		public bool ExistsF020301Data(string dcCode, string gupCode, string custCode, string purchaseNo, string itemCode)
		{
			var f010201Data = _db.F010201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.STOCK_NO == purchaseNo);

			var f010202Data = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.STOCK_NO == purchaseNo &&
																															 x.ITEM_CODE == itemCode);

			var f020302Data = _db.F020302s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																															 x.GUP_CODE == gupCode &&
																															 x.CUST_CODE == custCode &&
																															 x.STATUS == "0" &&
																															 f010201Data.Select(z => z.SHOP_NO).Distinct().Contains(x.PO_NO));

			var data = (from A in f010201Data
									join B in f010202Data
									on new { A.STOCK_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.STOCK_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
									join C in f020302Data
									on new { PO_NO = A.SHOP_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE, B.ITEM_CODE } equals new { C.PO_NO, C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE }
									select new { A }).FirstOrDefault();
			return data != null;
		}

		/// <summary>
		/// 確認是否為重覆進倉單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="vnrCode"></param>
		/// <returns></returns>
		public IQueryable<F010201> GetDatasByCustOrdNoAndVnrCodeNotCancel(string dcCode, string gupCode, string custCode, string custOrdNo, string vnrCode)
		{
			var result = _db.F010201s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.CUST_ORD_NO == custOrdNo &&
																					 x.VNR_CODE == vnrCode &&
																					 x.STATUS != "9");

			return result;
		}

		public F010201 GetEnabledStockData(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var result = _db.F010201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 (x.STOCK_NO == stockNo || x.CUST_ORD_NO == stockNo || x.CHECK_CODE == stockNo) &&
																					 x.STATUS != "9").OrderByDescending(x => x.CRT_DATE);

			return result.FirstOrDefault();
		}




		public List<ThirdPartOrders> GetThirdPartOrdersData(string dcCode, string gupCode, string custCode, List<PostCreateWarehouseInsModel> warehouseInsList)
		{
			var f010201Data = _db.F010201s.Where(x => x.DC_CODE == dcCode &&
																								x.GUP_CODE == gupCode &&
																								x.CUST_CODE == custCode &&
																								x.STATUS != "9" &&
																								warehouseInsList.Select(z => z.CustInNo).Contains(x.CUST_ORD_NO))
																		.Select(x => new ThirdPartOrders
																		{
																			STOCK_NO = x.STOCK_NO,
																			CUST_ORD_NO = x.CUST_ORD_NO,
																			STATUS = x.STATUS,
																			CUST_COST = x.CUST_COST
																		}).ToList();

			f010201Data.ForEach(o =>
			{
				var param = warehouseInsList.Where(x => x.CustInNo == o.CUST_ORD_NO).LastOrDefault();
				if (param != null)
					o.PROC_FLAG = param.ProcFlag;
			});

			return f010201Data;
		}

		/// <summary>
		/// 取得一筆進倉單資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		/// <returns></returns>
		public F010201 GetData(string dcCode, string gupCode, string custCode, string stockNo)
		{
			return _db.F010201s.Where(x => x.DC_CODE == dcCode &&
																		 x.GUP_CODE == gupCode &&
																		 x.CUST_CODE == custCode &&
																		 x.STOCK_NO == stockNo &&
																		 x.STATUS != "9").SingleOrDefault();
		}

	}
}
