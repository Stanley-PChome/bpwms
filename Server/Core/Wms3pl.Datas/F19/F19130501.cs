namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("ID")]
	[Table("F19130501")]
	public class F19130501 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "bigint")]
		public long ID { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(50)")]
		public string SERIAL_NO { get; set; }
		/// <summary>
		/// 執行動作(A:新增 D:刪除)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ACTION_TYPE { get; set; }
		/// <summary>
		/// 處理狀態 (0: 待處理、1:處理成功、2:處理失敗)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { set; get; }
		/// <summary>
		/// 處理批次號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string BATCH_NO { get; set; }
		/// <summary>
		/// 處理失敗訊息
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string MESSAGE { get; set; }

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
	}
}
