namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 集貨格設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CELL_TYPE")]
	[Table("F194501")]
	public class F194501 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨格料架類型
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string CELL_TYPE { get; set; }
		/// <summary>
		/// 集貨格料架名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(20)")]
		public string CELL_NAME { get; set; }
		/// <summary>
		/// 長
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int LENGTH { get; set; }
		/// <summary>
		/// 寬
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int DEPTH { get; set; }
		/// <summary>
		/// 高
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int HEIGHT { get; set; }
		/// <summary>
		/// 容積率
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(38,0)")]
		public decimal VOLUME_RATE { get; set; }
		/// <summary>
		/// 單個集貨格最大可放容量(已乘容積率)
		/// </summary>
		[Required]
		[Column(TypeName = "bigint")]
		public Int64 MAX_VOLUME { get; set; }
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
