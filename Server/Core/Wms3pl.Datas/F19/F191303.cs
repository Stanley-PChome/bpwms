namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 庫存跨倉移動紀錄表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F191303")]
	public class F191303 : IAuditInfo
	{

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "bigint")]
		public long ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 調整單號(T開頭為調撥單、I開頭為調整單)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string SHIFT_WMS_NO { get; set; }

		/// <summary>
		/// 調整類型(0: 調撥、1: 庫存調整)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string SHIFT_TYPE { get; set; }

		/// <summary>
		/// 來源倉別類型
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string SRC_WAREHOUSE_TYPE { get; set; }

		/// <summary>
		/// 來源倉別編號
		/// </summary>
		[Column(TypeName = "varchar(3)")]
		public string SRC_WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 目的倉別類型
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string TAR_WAREHOUSE_TYPE { get; set; }

		/// <summary>
		/// 目的倉別編號
		/// </summary>
		[Column(TypeName = "varchar(3)")]
		public string TAR_WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 調整類別代碼
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string SHIFT_CAUSE { get; set; }

		/// <summary>
		/// 調整類別原因
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string SHIFT_CAUSE_MEMO { get; set; }

		/// <summary>
		/// 調整時間 (調撥為上架完成時間，庫調為庫存調整完成時間)
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? SHIFT_TIME { get; set; }

		/// <summary>
		/// 調整數量
		/// </summary>
		[Required]
		[Column(TypeName = "bigint")]
		public long SHIFT_QTY { get; set; }

		/// <summary>
		/// 明細資料處理狀態(0:待處理 1:已處理)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PROC_FLAG { get; set; }

		/// <summary>
		/// 紀錄轉出時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? TRANS_DATE { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string MAKE_NO { get; set; }
	}
}
