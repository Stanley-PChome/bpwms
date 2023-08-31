namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 配送商客代名稱檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F19471001")]
	public class F19471001 : IAuditInfo
	{

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal ID { get; set; }

		/// <summary>
		/// 配送商編號(F1947)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 客代設定編號(F194710)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(15)")]
		public string LOG_ID { get; set; }

		/// <summary>
		/// 客代設定名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string LOGCENTER_NAME { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

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
	}
}
