namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 儲區主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("AREA_CODE", "WAREHOUSE_ID", "DC_CODE")]
	[Table("F1919")]
	public class F1919 : IAuditInfo
	{

		/// <summary>
		/// 儲區編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string AREA_CODE { get; set; }

		/// <summary>
		/// 儲區名稱
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string AREA_NAME { get; set; }

		/// <summary>
		/// 儲區型態(F191901)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ATYPE_CODE { get; set; }

		/// <summary>
		/// 倉別(F1980)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(8)")]
		public string WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

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
