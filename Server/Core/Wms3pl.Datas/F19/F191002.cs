namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 門市附屬欄位檔
	/// </summary>
	[Serializable]
	[DataServiceKey("RETAIL_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F191002")]
	public class F191002 : IAuditInfo
	{

		/// <summary>
		/// 門市編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string RETAIL_CODE { get; set; }

		/// <summary>
		/// 業務員
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string SALESMAN { get; set; }

		/// <summary>
		/// 區域
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string REGION { get; set; }

		/// <summary>
		/// 上樓層
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string FLOOR { get; set; }

		/// <summary>
		/// 協助上架/翻堆
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string HELP { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 建立時間
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
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }
	}
}
