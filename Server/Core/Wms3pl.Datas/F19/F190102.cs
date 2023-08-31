namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 物流中心配送效率順序設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "DELV_EFFIC")]
	[Table("F190102")]
	public class F190102 : IAuditInfo
	{

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 配送效率(01:一般、02:3小時、03:6小時、04:9小時)F000904
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string DELV_EFFIC { get; set; }

		/// <summary>
		/// 優先順序
		/// </summary>
		[Required]
		[Column(TypeName = "smallint")]
		public Int16 SORT { get; set; }

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
		[Column(TypeName = "varchar(20)")]
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
	}
}
