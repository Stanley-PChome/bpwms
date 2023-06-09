using System.Collections.Generic;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckSerialNoService
	{
		private TransApiBaseService _tacService;

		public CheckSerialNoService()
		{
			_tacService = new TransApiBaseService();
		}

		/// <summary>
		/// 檢核品號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckItemCode(ref List<ApiResponse> res, GetItemSerialsReq req, string gupCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);

			var f1903 = f1903Repo.Find(o => o.GUP_CODE == gupCode &&
			o.CUST_CODE == req.CustCode &&
			o.ITEM_CODE == req.ItemCode);

			if (f1903 == null)
				res.Add(new ApiResponse { MsgCode = "20119", MsgContent = _tacService.GetMsg("20119"), ErrorColumn = "ItemCode" });
		}

		/// <summary>
		/// 檢核搜尋的序號狀態
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckSearchType(ref List<ApiResponse> res, GetItemSerialsReq req)
		{
			var searchType = new List<string> { "0", "1", "2", "3" };

			if (!searchType.Contains(req.SearchType))
				res.Add(new ApiResponse { MsgCode = "20011", MsgContent = string.Format(_tacService.GetMsg("20011"), "搜尋的序號狀態", "(0: 全部(包含已出貨、已銷毀)、1: 在庫內、2: 已出貨、3: 已銷毀)"), ErrorColumn = "SearchType" });
		}
	}
}
