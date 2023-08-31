namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 物流中心主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE")]
	[Table("F1901")]
	public class F1901 : IAuditInfo
	{

		/// <summary>
		/// 編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(80)")]
		public string DC_NAME { get; set; }

		/// <summary>
		/// 電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string TEL { get; set; }

		/// <summary>
		/// 傳真
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string FAX { get; set; }

		/// <summary>
		/// 地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string ADDRESS { get; set; }

		/// <summary>
		/// 地坪
		/// </summary>
		[Column(TypeName = "int")]
		public Int32? LAND_AREA { get; set; }

		/// <summary>
		/// 建坪
		/// </summary>
		[Column(TypeName = "int")]
		public Int32? BUILD_AREA { get; set; }

		/// <summary>
		/// 簡碼
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string SHORT_CODE { get; set; }

		/// <summary>
		/// 負責人
		/// </summary>
		[Column(TypeName = "nvarchar(40)")]
		public string BOSS { get; set; }

		/// <summary>
		/// 電子郵件
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string MAIL_BOX { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

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
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 擴增預留欄位A
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_A { get; set; }

		/// <summary>
		/// 擴增預留欄位B
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_B { get; set; }

		/// <summary>
		/// 擴增預留欄位C
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_C { get; set; }

		/// <summary>
		/// 擴增預留欄位D
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_D { get; set; }

		/// <summary>
		/// 擴增預留欄位E
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_E { get; set; }

		/// <summary>
		/// 郵遞區號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string ZIP_CODE { get; set; }
	}
}
