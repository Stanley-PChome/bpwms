using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 倉別總庫存資料_傳入
	/// </summary>
	public class GetItemStockDetailsReq
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

	public class GetItemStockDetailsRes
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
		/// 倉別編號
		/// </summary>
		public string WhNo { get; set; }
		/// <summary>
		/// 儲位
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 入庫日期
		/// </summary>
		public string EnterDate { get; set; }
		/// <summary>
		/// 庫存數
		/// </summary>
		public long StockQty { get; set; }
		/// <summary>
		/// 商品序號(序號綁儲位商品)
		/// </summary>
		public string Sn { get; set; }
		/// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 組合商品品號清單
		/// </summary>
		public List<string> BomItemList { get; set; }
	}
}
