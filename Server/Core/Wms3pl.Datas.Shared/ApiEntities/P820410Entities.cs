using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 盤點調整任務中介_傳入
	/// </summary>
	public class WcsStockCheckAdjustmentReq
	{
		/// <summary>
		/// 業主編號=貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 調整任務單號
		/// </summary>
		public string AdjustCode { get; set; }
		/// <summary>
		/// 原盤點任務單號
		/// </summary>
		public string CheckCode { get; set; }
		/// <summary>
		/// 明細數
		/// </summary>
		public int SkuTotal { get; set; }
		/// <summary>
		/// 明細資料
		/// </summary>
		public List<WcsStockCheckAdjustmentSkuModel> SkuList { get; set; }
	}

	/// <summary>
	/// 明細資料
	/// </summary>
	public class WcsStockCheckAdjustmentSkuModel
	{
		/// <summary>
		/// 庫內品號=WMS商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 預計調整數量
		/// </summary>
		public int SkuQty { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品)
		/// </summary>
		public int SkuLevel { get; set; }
		/// <summary>
		/// 調整狀態(1=調整, 2=不調整)
		/// </summary>
		public int AdjustType { get; set; }

		/// <summary>
		/// 貨架號碼
		/// </summary>
		public string ShelfCode { get; set; }
		/// <summary>
		/// 儲位號碼
		/// </summary>
		public string BinCode { get; set; }
	}

	/// <summary>
	/// 盤點調整任務中介_回傳
	/// </summary>
	public class WcsStockCheckAdjustmentResData
	{
		/// <summary>
		/// 調整單號
		/// </summary>
		public string AdjustCode { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public string ErrorColumn { get; set; }
		/// <summary>
		/// AGV錯誤回應
		/// </summary>
		public List<WcsErrorModel> errors { get; set; }
	}
}
