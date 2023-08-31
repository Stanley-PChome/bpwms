namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 電子訂單貨物配送狀態檔
	/// </summary>
	[Serializable]
	[DataServiceKey("STATUS_ID", "ALL_ID")]
	[Table("F194714")]
	public class F194714 : IAuditInfo
	{

		/// <summary>
		/// 狀態ID
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string STATUS_ID { get; set; }

		/// <summary>
		/// 狀態說明
		/// </summary>
		[Column(TypeName = "nvarchar(60)")]
		public string STATUS_DESC { get; set; }

		/// <summary>
		/// 速達顯示說明
		/// </summary>
		[Column(TypeName = "nvarchar(60)")]
		public string TCAT_DESC { get; set; }

		/// <summary>
		/// 0:訂單處理中未配送、2:配送中、3:配送完成、4:退貨、5:異常
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 建檔日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建檔人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建檔人員名稱
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(16)")]
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
		/// 異動人員名稱
		/// </summary>
		[Column(TypeName = "varchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 配送商
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 是否為最終貨態(0:否,1:是)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ISLASTSTATUS { get; set; }
	}
}
