using System;
using System.Collections.Generic;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	#region 盤盈建立調整單
	public class AdjustBySurplusParam
	{
		/// <summary>
		/// 初盤/複盤數
		/// </summary>
		public int INVENTORY_QTY { get; set; }
		/// <summary>
		/// 庫存數
		/// </summary>
		public int QTY { get; set; }
		/// <summary>
		/// 未搬移數量
		/// </summary>
		public int UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 儲位
		/// </summary>
		public string LOC_CODE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		public DateTime ENTER_DATE { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string MAKE_NO { get; set; }
		public string FLUSHBACK { get; set; }
		public List<string> SN_LIST { get; set; }
	}
	#endregion
}
