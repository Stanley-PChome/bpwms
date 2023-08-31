namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 通路配送商主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ALL_ID", "CHANNEL", "SUBCHANNEL")]
	[Table("F190905")]
	public class F190905 : IAuditInfo
	{

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 配送商
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 通路上編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CHANNEL { get; set; }

		/// <summary>
		/// 通路商名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(80)")]
		public string CHANNEL_NAME { get; set; }

		/// <summary>
		/// 通路商地址
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(120)")]
		public string CHANNEL_ADDRESS { get; set; }

		/// <summary>
		/// 通路商電話
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(40)")]
		public string CHANNEL_TEL { get; set; }

		/// <summary>
		/// 預設配送箱子尺寸(cm)
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? DEFAULT_BOXSIZE { get; set; }

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
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 子通路編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string SUBCHANNEL { get; set; }

		/// <summary>
		/// 箱明細格式(空值表示預設報表)
		/// </summary>
		[Column(TypeName = "varchar(30)")]
		public string DELVDTL_FORMAT { get; set; }

		/// <summary>
		/// 是否自動列印箱明細 0:否 1:是
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string AUTO_PRINT_DELVDTL { get; set; }

		/// <summary>
		/// 是否自動列印託運單
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string AUTO_PRINT_CONSIGN { get; set; }
	}
}
