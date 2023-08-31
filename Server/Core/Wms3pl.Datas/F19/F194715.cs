namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 客代主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("CUSTOMER_ID", "ISTEST", "ALL_ID")]
	[Table("F194715")]
	public class F194715 : IAuditInfo
	{

		/// <summary>
		/// 客戶代號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CUSTOMER_ID { get; set; }

		/// <summary>
		/// FTP連線地址
		/// </summary>
		[Column(TypeName = "varchar(30)")]
		public string FTP_IP { get; set; }

		/// <summary>
		/// FTP帳號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string FTP_ACCOUNT { get; set; }

		/// <summary>
		/// FTP密碼
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string FTP_PASSWORD { get; set; }

		/// <summary>
		/// FTP上傳資料夾路徑_正物流
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string FTP_UPLOADPATH { get; set; }

		/// <summary>
		/// FTP下載資料夾路徑_回檔
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string FTP_DOWNLOADPATH { get; set; }

		/// <summary>
		/// 產生檔案或下載本機暫存路徑
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string LOCAL_TEMPPATH { get; set; }

		/// <summary>
		/// 備份資料夾路徑
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string LOCAL_BACKUPPATH { get; set; }

		/// <summary>
		/// 本機LOG資料夾路徑
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string LOCAL_LOGPATH { get; set; }

		/// <summary>
		/// MAIL主旨
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string MAILSUBJECT { get; set; }

		/// <summary>
		/// MAIL 收件者
		/// </summary>
		[Column(TypeName = "varchar(2000)")]
		public string MAILTO { get; set; }

		/// <summary>
		/// MAIL CC
		/// </summary>
		[Column(TypeName = "varchar(2000)")]
		public string MAILCC { get; set; }

		/// <summary>
		/// ZIP壓縮檔密碼
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string ZIP_PASSWORD { get; set; }

		/// <summary>
		/// 是否測試(0:否;1:是)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "char(1)")]
		public string ISTEST { get; set; }

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
		/// 異動人員名稱
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 配送商
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// FTP上傳資料夾路徑_逆物流
		/// </summary>
		[Column(TypeName = "varchar(100)")]
		public string FTP_UPLOADBACKPATH { get; set; }
	}
}
