using System;
using System.Collections.Generic;
using Wms3pl.Datas.F05;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 批次訂單轉入排程_傳入
	/// </summary>
	public class StockAlertReturnReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }

		public List<StockAlertReturn> Data { get; set; }
	}

	public class StockAlertReturn
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 有效日期(yyyy/MM/dd)
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 實際庫存數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 警示天數
		/// </summary>
		public int AllShp { get; set; }
	}

	public class ExpStockAlert
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string ItemCode { get; set; }
		public DateTime ValidDate { get; set; }
		public string MakeNo { get; set; }
		public int Qty { get; set; }
		public int AllShp { get; set; }
	}
}
