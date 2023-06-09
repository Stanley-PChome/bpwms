using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 商品序號查詢_傳入
	/// </summary>
	public class GetItemSerialsReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 搜尋的序號狀態(0: 全部(包含已出貨、已銷毀)、1: 在庫內、2: 已出貨、3: 已銷毀)
		/// </summary>
		public string SearchType { get; set; }
	}

	/// <summary>
	/// 商品序號查詢_回傳
	/// </summary>
	public class GetItemSerialsRes
	{
		/// <summary>
		/// 搜尋的序號狀態
		/// </summary>
		public string SearchType { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public List<string> SnList { get; set; }
	}

	public class SearchTypeModel
	{
		public string SearchType { get; set; }
		public string STATUS { get; set; }
	}
}
