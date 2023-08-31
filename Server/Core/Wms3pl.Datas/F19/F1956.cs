namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨優先權代碼檔 
	/// </summary>
	[Serializable]
	[DataServiceKey("PRIORITY_CODE")]
	[Table("F1956")]
	public class F1956 : IAuditInfo
	{
		/// <summary>
		/// 出貨優先權代碼
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string PRIORITY_CODE { get; set; }
		/// <summary>
		/// 出貨優先權名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(10)")]
		public string PRIORITY_NAME { get; set; }
		/// <summary>
		/// 是否提供人員選擇(0:否;1:是)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IS_SHOW { get; set; }
		/// <summary>
		/// 是否為系統預設代碼(0:否;1:是)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IS_SYSTEM { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
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
		/// 異動人員編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }
	}
}
