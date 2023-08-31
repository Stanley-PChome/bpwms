namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 中介商品主檔同步處理紀錄
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F1903_ASYNC")]
	public class F1903_ASYNC : IAuditInfo
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
		[Column(TypeName = "varchar(12)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 同步狀態(N:未同步 Y:已同步 F:同步失敗 9:重複取消同步)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IS_ASYNC { get; set; }

		/// <summary>
		/// API處理批次號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string BATCH_NO { get; set; }

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
