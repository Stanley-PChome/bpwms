using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 盤點任務中介_傳入
	/// </summary>
	public class WcsInventoryReq
	{
		/// <summary>
		/// 業主編號=貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 任務任務單號
		/// </summary>
		public string CheckCode { get; set; }
		/// <summary>
		/// 原盤點單號
		/// </summary>
		public string OriCheckCode { get; set; }
		/// <summary>
		/// 盤點類型(2:按商品盤 3:按批次盤)
		/// </summary>
		public int CheckType { get; set; }
		/// <summary>
		/// 盤點方式(0:明盤 1:盲盤 2:盲盤(商品+數量)
		/// </summary>
		public int CheckMode { get; set; }
		/// <summary>
		/// 明細數
		/// </summary>
		public int SkuTotal { get; set; }
		/// <summary>
		/// 明細資料
		/// </summary>
		public List<WcsInventorySkuModel> SkuList { get; set; }
	}

	/// <summary>
	/// 明細資料
	/// </summary>
	public class WcsInventorySkuModel
	{
		/// <summary>
		/// 庫內品號=WMS商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品)
		/// </summary>
		public int SkuLevel { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 外部=商品入庫日(yyMMdd)+序號(3碼數字)
		/// </summary>
		public string OutBatchCode { get; set; }
	}

	/// <summary>
	/// 盤點任務中介_回傳
	/// </summary>
	public class WcsInventoryResData
	{
		/// <summary>
		/// 錯誤單號
		/// </summary>
		public string CheckCode { get; set; }
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
