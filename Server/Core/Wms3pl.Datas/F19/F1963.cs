namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 作業群組主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("WORK_ID")]
	[Table("F1963")]
	public class F1963 : IAuditInfo
	{

		/// <summary>
		/// 作業群組編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal WORK_ID { get; set; }

		/// <summary>
		/// 作業群組名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(30)")]
		public string WORK_NAME { get; set; }

		/// <summary>
		/// 說明
		/// </summary>
		[Column(TypeName = "nvarchar(60)")]
		public string WORK_DESC { get; set; }

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
		/// 註記是否已刪除(1:已刪除)
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string ISDELETED { get; set; }

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
