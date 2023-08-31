namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 班次配送區域設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ALL_ID", "DC_CODE", "DELV_TIME", "DELV_TMPR", "ZIP_CODE", "DELV_EFFIC", "PAST_TYPE")]
	[Table("F19470101")]
	public class F19470101 : IAuditInfo
	{

		/// <summary>
		/// 配送商編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 出車時段
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string DELV_TIME { get; set; }

		/// <summary>
		/// 配送溫層(A:常溫、B:低溫)F000904
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "char(1)")]
		public string DELV_TMPR { get; set; }

		/// <summary>
		/// 配送郵遞區號(3碼)F1934
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string ZIP_CODE { get; set; }

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
		/// 建立人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

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
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 配送效率
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string DELV_EFFIC { get; set; }

		/// <summary>
		/// 0正物流1逆物流F000904
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "char(1)")]
		public string PAST_TYPE { get; set; }
	}
}
