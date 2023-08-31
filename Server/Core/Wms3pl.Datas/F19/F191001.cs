namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 門市主檔效期限定天數檔
	/// </summary>
	[Serializable]
	[DataServiceKey("GUP_CODE", "CUST_CODE", "CHANNEL", "LTYPE", "MTYPE", "STYPE")]
	[Table("F191001")]
	public class F191001 : IAuditInfo
	{

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
		[Column(TypeName = "nvarchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 通路
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "nvarchar(20)")]
		public string CHANNEL { get; set; }
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string LTYPE { get; set; }
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string MTYPE { get; set; }
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string STYPE { get; set; }

		/// <summary>
		/// 允出天數
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public Int32 DELIVERY_DAY { get; set; }

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
		[Column(TypeName = "varchar(20)")]
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
