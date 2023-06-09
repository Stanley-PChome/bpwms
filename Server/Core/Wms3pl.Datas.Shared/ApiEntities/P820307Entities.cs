using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 快速移轉庫存調整單_傳入
	/// </summary>
	public class FlashStockTransferDataReq
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
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 交易編號(不可重複)
		/// </summary>
		public string TransactionNo { get; set; }
		/// <summary>
		/// 商品清單
		/// </summary>
		public List<FlashStockTransferDataResult> Result { get; set; }
	}

	/// <summary>
	/// 快速移轉庫存調整單商品清單_傳入
	/// </summary>
	public class FlashStockTransferDataResult
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 調整數量
		/// </summary>
		public int AdjQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public DateTime? ValidDate { get; set; }
		/// <summary>
		/// 驗收批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public List<string> SnList { get; set; }
	}

	/// <summary>
	/// 快速移轉庫存調整單_傳入
	/// </summary>
	public class FlashStockTransferData
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string MsgCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string MsgContent { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
	}
}
