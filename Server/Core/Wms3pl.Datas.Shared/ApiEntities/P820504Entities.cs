using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 盤點調整完成結果回傳_傳入
	/// </summary>
	public class InventoryAdjustReceiptReq
	{
		/// <summary>
		/// 業主編號=WMS貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 儲區編號
		/// </summary>
		public string ZoneCode { get; set; }
		/// <summary>
		/// 盤點調整單號
		/// </summary>
		public string AdjustCode { get; set; }
		/// <summary>
		/// 盤點任務單號
		/// </summary>
		public string CheckCode { get; set; }
		/// <summary>
		/// 調整單狀態(1:成功 0:失敗)
		/// </summary>
		public int? Status { get; set; }
		/// <summary>
		/// 品項數
		/// </summary>
		public int SkuTotal { get; set; }
		/// <summary>
		/// (盤點明細)保留人員盤點紀錄列表
		/// </summary>
		public List<InventoryAdjustReceiptSkuModel> SkuList { get; set; } = new List<InventoryAdjustReceiptSkuModel>();
	}

	/// <summary>
	/// 品項明細
	/// </summary>
	public class InventoryAdjustReceiptSkuModel
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 商品入庫日(yyyyMMdd)+序號(3碼數字)
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 調整數量
		/// </summary>
		public int? SkuQty { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品)
		/// </summary>
		public int SkuLevel { get; set; } = 1;
		/// <summary>
		/// 調整狀態( 1:成功 0:失敗)
		/// </summary>
		public int? Status { get; set; }
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string ShelfCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BinCode { get; set; }
	}

	public class WcsApiInventoryAdjustReceiptResultData
	{
		public string AdjustCode { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}
}
