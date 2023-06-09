using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	public class CommonStockService
	{
		/// <summary>
		/// 倉別總庫存資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult GetItemStocks(GetItemStocksReq req)
		{
			CommonService commonService = new CommonService();
			CheckTransApiService ctaService = new CheckTransApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			CheckStockService csService = new CheckStockService();
			ApiResult res = new ApiResult { IsSuccessed = true, MsgCode = "10004", MsgContent = tacService.GetMsg("10004") };

			#region 資料檢核
			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核搜尋條件是否為(0: 品號、1: 廠商編號)
			csService.CheckSearchRule(ref res, req.SearchRule);
			if (!res.IsSuccessed)
				return res;

			// 檢核搜尋清單(品號、廠商編號)是否有資料
			csService.CheckCodeList(ref res, req.CodeList);
			if (!res.IsSuccessed)
				return res;
			#endregion

			#region 取得資料
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			res.Data = new List<GetItemStocksRes>();

			var gupCode = commonService.GetGupCode(req.CustCode);

			var f1903s = f1903Repo.GetDatasByItemCodesOrVnrCodes(
				gupCode,
				req.CustCode,
				req.SearchRule == "0" ? req.CodeList : null,
				req.SearchRule == "1" ? req.CodeList : null);

			if (!f1903s.Any())
				return res;

			// 取得商品總庫存資料
			var f1913s = f1913Repo.GetDatasByQtyNotZero(req.DcCode, gupCode, req.CustCode, f1903s.Select(x => x.ITEM_CODE).ToList());

			var f1912s = f1912Repo.GetF1912Data(req.DcCode, f1913s.Select(x => x.LOC_CODE).ToList());

			var datas = (from A in f1913s
									 join B in f1912s
									 on A.LOC_CODE equals B.LOC_CODE
									 select new
									 {
										 A.ITEM_CODE,
										 A.QTY,
										 WhId = B.WAREHOUSE_ID
									 })
									.GroupBy(x => x.ITEM_CODE).Select(x => new GetItemStocksRes
									{
										DcCode = req.DcCode,
										CustCode = req.CustCode,
										ItemCode = x.Key,
										StockQty = x.Sum(z => z.QTY),
										StockByWhDatas = x.GroupBy(z => z.WhId).Select(z => new StockByWhDatasModel
										{
											WhNo = z.Key,
											WhType = z.Key.Substring(0, 1),
											StockQty = z.Sum(y => y.QTY)
										}).ToList()
									}).ToList();

			res.Data = datas;

			#endregion

			return res;
		}

		/// <summary>
		/// 庫存明細資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult GetItemStockDetails(GetItemStockDetailsReq req)
		{
			CommonService commonService = new CommonService();
			CheckTransApiService ctaService = new CheckTransApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			CheckStockService csService = new CheckStockService();
			ApiResult res = new ApiResult { IsSuccessed = true, MsgCode = "10004", MsgContent = tacService.GetMsg("10004") };

			#region 資料檢核
			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核搜尋條件是否為(0: 品號、1: 廠商編號)
			csService.CheckSearchRule(ref res, req.SearchRule);
			if (!res.IsSuccessed)
				return res;

			// 檢核搜尋清單(品號、廠商編號)是否有資料
			csService.CheckCodeList(ref res, req.CodeList);
			if (!res.IsSuccessed)
				return res;
			#endregion

			#region 取得資料
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			res.Data = new List<GetItemStockDetailsRes>();

			var gupCode = commonService.GetGupCode(req.CustCode);

			var f1903s = f1903Repo.GetDatasByItemCodesOrVnrCodes(
				gupCode,
				req.CustCode,
				req.SearchRule == "0" ? req.CodeList : null,
				req.SearchRule == "1" ? req.CodeList : null);

			if (!f1903s.Any())
				return res;

			// 取得組合商品品號清單
			var f910101Repo = new F910101Repository(Schemas.CoreSchema);
			var f910102Repo = new F910102Repository(Schemas.CoreSchema);

			var itemCodes = f1903s.Select(x => x.ITEM_CODE).ToList();

			var f910101s = f910101Repo.GetDatas(gupCode, req.CustCode, itemCodes);

			var f910102s = f910102Repo.GetDatas(gupCode, req.CustCode, f910101s.Select(x => x.BOM_NO).ToList());

			var bomItemList = (from A in f910101s
												 join B in f910102s
												 on new { A.GUP_CODE, A.CUST_CODE, A.BOM_NO } equals new { B.GUP_CODE, B.CUST_CODE, B.BOM_NO }
												 select new
												 {
													 A.ITEM_CODE,
													 B.MATERIAL_CODE
												 }).GroupBy(x => x.ITEM_CODE).Select(x => new
												 {
													 ITEM_CODE = x.Key,
													 BomItemList = x.Select(z => z.MATERIAL_CODE).ToList()
												 }).ToList();

			// 取得商品總庫存資料
			var f1913s = f1913Repo.GetDatasByQtyNotZero(req.DcCode, gupCode, req.CustCode, itemCodes).ToList();

			var f1912s = f1912Repo.GetF1912Data(req.DcCode, f1913s.Select(x => x.LOC_CODE).ToList()).ToList();

			var datas = (from A in f1913s
									 join B in f1912s
									 on A.LOC_CODE equals B.LOC_CODE
									 join C in f1903s
									 on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE }
									 join D in bomItemList
									 on A.ITEM_CODE equals D.ITEM_CODE into subD
									 from D in subD.DefaultIfEmpty()
									 select new
									 {
										 B.WAREHOUSE_ID,
										 A.LOC_CODE,
										 A.ITEM_CODE,
										 A.VALID_DATE,
										 A.ENTER_DATE,
										 A.SERIAL_NO,
										 A.MAKE_NO,
										 C.VNR_CODE,
										 A.QTY,
										 BomItemList = D == null ? new List<string>() : D.BomItemList
									 })
									.GroupBy(x => new
									{
										x.WAREHOUSE_ID,
										x.LOC_CODE,
										x.ITEM_CODE,
										x.VALID_DATE,
										x.ENTER_DATE,
										x.SERIAL_NO,
										x.MAKE_NO,
										x.VNR_CODE,
									}).Select(x => new GetItemStockDetailsRes
									{
										DcCode = req.DcCode,
										CustCode = req.CustCode,
										LocCode = x.Key.LOC_CODE,
										ItemCode = x.Key.ITEM_CODE,
										ValidDate = x.Key.VALID_DATE.ToString("yyyy/MM/dd"),
										EnterDate = x.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
										Sn = x.Key.SERIAL_NO,
										MakeNo = x.Key.MAKE_NO,
										VnrCode = x.Key.VNR_CODE,
										WhNo = x.Key.WAREHOUSE_ID,
										StockQty = x.Sum(z => z.QTY),
										BomItemList = x.Select(z => z.BomItemList).First()
									}).OrderBy(x => x.ItemCode).ToList();

			res.Data = datas;

			#endregion

			return res;
		}
	}
}
