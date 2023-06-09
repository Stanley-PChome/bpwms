namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 庫存異常明細表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F191302")]
	public class F191302 : IAuditInfo
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
		/// 來源單號
		/// </summary>
		[Required]
		public string SRC_WMS_NO { get; set; }

        /// <summary>
        /// 來源類型 (0: 揀貨缺貨 1:盤點 2:調撥缺貨)
        /// </summary>
        [Required]
		public string SRC_TYPE { get; set; }

		/// <summary>
		/// 調撥單號
		/// </summary>
		[Required]
		public string ALLOCATION_NO { get; set; }

		/// <summary>
		/// 調撥單序號
		/// </summary>
		[Required]
		public int ALLOCATION_SEQ { get; set; }

		/// <summary>
		/// 異常倉別(來源倉別)
		/// </summary>
		[Required]
		public string SRC_WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 異常儲位(來源儲位)
		/// </summary>
		[Required]
		public string SRC_LOC_CODE { get; set; }

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
		public string MAKE_NO { get; set; }

		/// <summary>
		/// 入庫日期
		/// </summary>
		[Required]
		public DateTime ENTER_DATE { get; set; }

		/// <summary>
		/// 序號綁儲位的商品序號
		/// </summary>
		public string SERIAL_NO { get; set; }

		/// <summary>
		/// 箱號
		/// </summary>
		[Required]
		public string BOX_CTRL_NO { get; set; }

		/// <summary>
		/// 板號
		/// </summary>
		[Required]
		public string PALLET_CTRL_NO { get; set; }

		/// <summary>
		/// 廠商編號
		/// </summary>
		[Required]
		public string VNR_CODE { get; set; }

		/// <summary>
		/// 異常數量
		/// </summary>
		[Required]
		public int QTY { get; set; }

		/// <summary>
		/// 目前倉別(目的倉別)
		/// </summary>
		public string TAR_WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 目前儲位(目的儲位)
		/// </summary>
		public string TAR_LOC_CODE { get; set; }

		/// <summary>
		/// 處理方式 (0: 待處理、1: 找到商品、2: 確定盤損)
		/// </summary>
		[Required]
		public string PROC_FLAG { get; set; }

		/// <summary>
		/// 備註
		/// </summary>
		public string MEMO { get; set; }

		/// <summary>
		/// 處理單號
		/// </summary>
		public string PROC_WMS_NO { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

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
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
	}
}
