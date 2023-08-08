using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 倉別總庫存資料_傳入
	/// </summary>
	public class GetItemStocksReq
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
		/// 搜尋條件(0: 品號、1: 廠商編號)
		/// </summary>
		public string SearchRule { get; set; }
		/// <summary>
		/// 搜尋清單(品號、廠商編號)
		/// </summary>
		public List<string> CodeList { get; set; }
	}

	public class GetItemStocksRes
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
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 庫存數
		/// </summary>
		public long StockQty { get; set; }
		public List<StockByWhDatasModel> StockByWhDatas { get; set; }
	}

	public class StockByWhDatasModel
	{
		/// <summary>
		/// WMS倉別編號
		/// </summary>
		public string WhNo { get; set; }
		/// <summary>
		/// WMS 倉別類型
		/// </summary>
		public string WhType { get; set; }
		/// <summary>
		/// 庫存數
		/// </summary>
		public long StockQty { get; set; }
	}
}
