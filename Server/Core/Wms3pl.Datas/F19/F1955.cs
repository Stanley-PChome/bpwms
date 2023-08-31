namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 便利倉設定 
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CONVENIENT_CODE")]
	[Table("F1955")]
	public class F1955 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 便利場編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// 便利場名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(20)")]
		public string CONVENIENT_NAME { get; set; }
		/// <summary>
		/// 便利場儲格開頭編號
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string CELL_START_CODE { get; set; }
		/// <summary>
		/// 便利場儲格總數量
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int CELL_NUM { get; set; }
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
