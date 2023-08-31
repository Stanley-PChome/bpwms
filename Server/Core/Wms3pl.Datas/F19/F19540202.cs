namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 系統選單樣式分類排序檔
	/// </summary>
	[Serializable]
	[DataServiceKey("MENU_CODE", "CATEGORY_LEVEL", "CATEGORY")]
	[Table("F19540202")]
	public class F19540202 : IAuditInfo
	{

		/// <summary>
		/// 系統選單樣式編號F195402
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string MENU_CODE { get; set; }

		/// <summary>
		/// 分類階層(1:主階層F000904 TOPIC=F195401 SUBTOPIC=CATEOGRY,2:子階層F000904 TOPIC=F195401 SUBTOPIC=SUB_CATEGORY)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "smallint")]
		public Int16 CATEGORY_LEVEL { get; set; }

		/// <summary>
		/// 分類編號(主階層F000904 TOPIC=F195401 SUBTOPIC=CATEOGRY,子階層F000904 TOPIC=F195401 SUBTOPIC=SUB_CATEGORY)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string CATEGORY { get; set; }

		/// <summary>
		/// 分類排序
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(38,0)")]
		public Decimal CATEGORY_SORT { get; set; }

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
