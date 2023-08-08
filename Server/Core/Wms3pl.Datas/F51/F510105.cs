namespace Wms3pl.Datas.F51
{
  using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 庫存比對資料
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F510105")]
	public class F510105 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 備份日期
		/// </summary>
		[Required]
		public string CAL_DATE { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LOC_CODE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		[Required]
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[Required]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// WMS的庫存
		/// </summary>
		public int? WMS_QTY { get; set; }
		/// <summary>
		/// 自動倉的庫存
		/// </summary>
		public int? WCS_QTY { get; set; }
		/// <summary>
		/// 虛擬帳的預約數
		/// </summary>
		public int? BOOKING_QTY { get; set; }
		/// <summary>
		/// 庫存差異數量
		/// </summary>
		public int? DIFF_QTY { get; set; }
		/// <summary>
		/// 處理狀態 (0: 待處理 1: 比對中 2: 比對完成)
		/// </summary>
		[Required]
		public string PROC_FLAG { get; set; }
		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
        