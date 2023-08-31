namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 委外商主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("OUTSOURCE_ID")]
	[Table("F1928")]
	public class F1928 : IAuditInfo
	{

		/// <summary>
		/// 委外商編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string OUTSOURCE_ID { get; set; }

		/// <summary>
		/// 委外商名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(30)")]
		public string OUTSOURCE_NAME { get; set; }

		/// <summary>
		/// 帳務聯絡人
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(40)")]
		public string CONTACT { get; set; }

		/// <summary>
		/// 電話
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(40)")]
		public string TEL { get; set; }

		/// <summary>
		/// 統一編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(16)")]
		public string UNI_FORM { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

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
		/// 負責人
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string BOSS { get; set; }

		/// <summary>
		/// 貨物聯絡人
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string ITEM_CONTACT { get; set; }

		/// <summary>
		/// 貨物聯絡人電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string ITEM_TEL { get; set; }

		/// <summary>
		/// 貨物聯絡人手機
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string ITEM_CEL { get; set; }

		/// <summary>
		/// 貨物聯絡人信箱
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string ITEM_MAIL { get; set; }

		/// <summary>
		/// 帳務聯絡人電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string BILL_TEL { get; set; }

		/// <summary>
		/// 帳務聯絡人手機
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string BILL_CEL { get; set; }

		/// <summary>
		/// 帳務聯絡人信箱
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string BILL_MAIL { get; set; }

		/// <summary>
		/// 地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string ADDRESS { get; set; }

		/// <summary>
		/// 使用幣別 (下拉選單) 台幣、美金 F000904
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string CURRENCY { get; set; }

		/// <summary>
		/// 付款條件 (下拉選單) 月結, 當月結，20天 F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PAY_FACTOR { get; set; }

		/// <summary>
		/// 付款方式 (下拉選單) 現金、票據，電匯 F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PAY_TYPE { get; set; }

		/// <summary>
		/// 銀行帳號
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string BANK_ACCOUNT { get; set; }

		/// <summary>
		/// 銀行代碼
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string BANK_CODE { get; set; }

		/// <summary>
		/// 銀行名稱
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string BANK_NAME { get; set; }

		/// <summary>
		/// 委外商狀態(0:使用中 9刪除)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 郵遞區號
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string ZIP { get; set; }

		/// <summary>
		/// 發票郵遞區號
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string INV_ZIP { get; set; }

		/// <summary>
		/// 發票地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string INV_ADDRESS { get; set; }

		/// <summary>
		/// 稅別 Y/N
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string TAX_TYPE { get; set; }

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
	}
}
