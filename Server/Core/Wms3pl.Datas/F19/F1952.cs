namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 人員登入密碼檔
	/// </summary>
	[Serializable]
	[DataServiceKey("EMP_ID")]
	[Table("F1952")]
	public class F1952 : IAuditInfo
	{

		/// <summary>
		/// 員工編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(16)")]
		public string EMP_ID { get; set; }

		/// <summary>
		/// 登入密碼
		/// </summary>
		[Column(TypeName = "varchar(48)")]
		public string PASSWORD { get; set; }

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
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 上次啟用時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? LAST_ACTIVITY_DATE { get; set; }

		/// <summary>
		/// 上次登入時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? LAST_LOGIN_DATE { get; set; }

		/// <summary>
		/// 上次更換密碼時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? LAST_PASSWORD_CHANGED_DATE { get; set; }

		/// <summary>
		/// 上次登出時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? LAST_LOCKOUT_DATE { get; set; }

		/// <summary>
		/// 密碼輸入錯誤次數
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? FAILED_PASSWORD_ATTEMPT_COUNT { get; set; }

		/// <summary>
		/// 狀態
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? STATUS { get; set; }

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
	}
}
