namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 商品中分類主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("BCODE", "ACODE", "GUP_CODE", "CUST_CODE")]
	[Table("F1916")]
	public class F1916 : IAuditInfo
	{

		/// <summary>
		/// 中分類編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string BCODE { get; set; }

		/// <summary>
		/// 分類名稱
		/// </summary>
		[Column(TypeName = "nvarchar(80)")]
		public string CLA_NAME { get; set; }

		/// <summary>
		/// 所屬大分類編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ACODE { get; set; }

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

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 抽驗比例
		/// </summary>
		[Column(TypeName = "decimal(14,11)")]
		public decimal? CHECK_PERCENT { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(12)")]
		public string CUST_CODE { get; set; }
	}
}
