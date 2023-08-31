namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 貨主DM設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("CUST_CODE", "GUP_CODE", "DM_SEQ")]
	[Table("F190902")]
	public class F190902 : IAuditInfo
	{

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(12)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// DM名稱
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "int")]
		public Int32 DM_SEQ { get; set; }

		/// <summary>
		/// 起始日期
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string DM_NAME { get; set; }

		/// <summary>
		/// 起始日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime BEGIN_DATE { get; set; }

		/// <summary>
		/// 結束日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime END_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string UPD_STAFF { get; set; }

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
