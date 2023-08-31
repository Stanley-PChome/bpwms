namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 商品廠商對應表
	/// </summary>
	[Serializable]
	[DataServiceKey("ITEM_CODE", "VNR_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F190303")]
	public class F190303 : IAuditInfo
	{

		/// <summary>
		/// 商品編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 廠商編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string VNR_CODE { get; set; }

		/// <summary>
		/// 來源單號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string SOURCE_NO { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

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
		[Column(TypeName = "varchar(40)")]
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
	}
}
