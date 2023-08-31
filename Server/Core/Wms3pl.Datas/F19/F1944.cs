namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 揀貨載具檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "PICK_MARTERIAL")]
	[Table("F1944")]
	public class F1944 : IAuditInfo
	{

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(12)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 揀貨載具F000904 TOPIC=F1944 SUBTOPIC=PICK_MARTERIAL
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string PICK_MARTERIAL { get; set; }

		/// <summary>
		/// 長度(CM)
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal LENGTH { get; set; }

		/// <summary>
		/// 寬度(CM)
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal WIDTH { get; set; }

		/// <summary>
		/// 高度(CM)
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal HEIGHT { get; set; }

		/// <summary>
		/// 標準重量
		/// </summary>
		[Column(TypeName = "bigint")]
		public Int64? WEIGHT { get; set; }

		/// <summary>
		/// 乘載率
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? COVER { get; set; }

		/// <summary>
		/// 誤差率
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? ERROR { get; set; }

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
