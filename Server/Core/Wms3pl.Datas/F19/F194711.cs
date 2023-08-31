namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 配送商託運單號區間設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ALL_ID")]
	[Table("F194711")]
	public class F194711 : IAuditInfo
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
		/// 配送商編號(F1947)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 配送商客戶代號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CONSIGN_ID { get; set; }

		/// <summary>
		/// 託單號(起)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(15)")]
		public string START_NUMBER { get; set; }

		/// <summary>
		/// 託單號(迄)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(15)")]
		public string END_NUMBER { get; set; }

		/// <summary>
		/// 目前使用到的託單號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(15)")]
		public string NOW_NUMBER { get; set; }

		/// <summary>
		/// 備註
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string MEMO { get; set; }

		/// <summary>
		/// 狀態(0:使用中，9:不使用)
		/// </summary>
		[Required]
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
		/// 建檔人名
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
		/// 配送商客戶代號名稱
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string CONSIGN_NAME { get; set; }
	}
}
