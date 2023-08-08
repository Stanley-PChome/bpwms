using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 每日庫存快照回傳_傳入
	/// </summary>
	public class SnapshotStocksReceiptReq
	{
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 對接系統(0: Geek、1: 盟立)
		/// </summary>
		public string SrcSystem { get; set; }
		/// <summary>
		/// 對帳時間(yyyy/MM/dd HH:ii:ss)
		/// </summary>
		public string AuditDate { get; set; }
		/// <summary>
		/// 總頁數
		/// </summary>
		public int? TotalPage { get; set; }
		/// <summary>
		/// 當前頁
		/// </summary>
		public int? CurrentPage { get; set; }
		/// <summary>
		/// 一頁筆數
		/// </summary>
		public int? PageSize { get; set; }
		/// <summary>
		/// 庫存明細
		/// </summary>
		public List<SnapshotStocksReceiptSkuModel> StockList { get; set; } = new List<SnapshotStocksReceiptSkuModel>();
	}

	/// <summary>
	/// 品項明細
	/// </summary>
	public class SnapshotStocksReceiptSkuModel
	{
		/// <summary>
		/// 儲區編號=倉別編號
		/// </summary>
		public string ZoneCode { get; set; }
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string ShelfCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BinCode { get; set; }
		/// <summary>
		/// 業主編號=WMS貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 庫內品號=WMS商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 商品等級(0=殘品、1=正品)
		/// </summary>
		public int? SkuLevel { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		public int? SkuQty { get; set; }
	}

	public class WcsApiSnapshotStocksReceiptResultData
	{
		public int? Index { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}

	public class WcsSkuCodeModel
	{
		public string SkuCode { get; set; }
		public string OwnerCode { get; set; }
		public string GupCode { get; set; }
	}
}
