namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 專車計費區域代碼表
	/// </summary>
	[Serializable]
	[DataServiceKey("ACC_AREA_ID", "DC_CODE")]
	[Table("F1948")]
	public class F1948 : IAuditInfo
	{

		/// <summary>
		/// 計費區域代碼
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal ACC_AREA_ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 計費區域名稱
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(30)")]
		public string ACC_AREA { get; set; }

		/// <summary>
		/// 狀態(0使用中,9刪除)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

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
	}
}
