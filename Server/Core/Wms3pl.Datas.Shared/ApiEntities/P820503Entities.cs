using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 盤點完成結果回傳_傳入
	/// </summary>
	public class InventoryReceiptReq
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
		/// 盤點單號
		/// </summary>
		public string CheckCode { get; set; }
		/// <summary>
		/// 盤點單狀態(3: 盤點完成 9:取消)
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 盤點開始時間(yyyy/MM/dd HH:mi:ss)
		/// </summary>
		public string StartTime { get; set; }
		/// <summary>
		/// 盤點完成時間(yyyy/MM/dd HH:mi:ss)
		/// </summary>
		public string CompleteTime { get; set; }
		/// <summary>
		/// 盤點人員(工號)
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 品項數
		/// </summary>
		public int SkuTotal { get; set; }
		/// <summary>
		/// (盤點明細)保留人員盤點紀錄列表
		/// </summary>
		public List<InventoryReceiptModel> SkuList { get; set; } = new List<InventoryReceiptModel>();
	}

	/// <summary>
	/// (盤點明細)保留人員盤點紀錄
	/// </summary>
	public class InventoryReceiptModel
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 系統記錄數量
		/// </summary>
		public int? SkuSysQty { get; set; }
		/// <summary>
		/// 實際盤點數量
		/// </summary>
		public int? SkuQty { get; set; }
		/// <summary>
		/// 盤點人員(工號)
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 盤點時間 yyyy/MM/dd HH:mm:ss
		/// </summary>
		public string OperatorTime { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品)
		/// </summary>
		public int SkuLevel { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 商品批次號
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string ShelfCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BinCode { get; set; }
	}

	public class WcsApiInventoryReceiptResultData
	{
		public string CheckCode { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}
}
