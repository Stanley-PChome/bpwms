namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 車次主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_NO")]
	[Table("F194716")]
	public class F194716 : IAuditInfo
	{

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

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
		[Column(TypeName = "varchar(12)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 車次/路線
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string DELV_NO { get; set; }

		/// <summary>
		/// 出車時段(F00904，0早、1午、2晚)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string CAR_PERIOD { get; set; }

		/// <summary>
		/// 車行
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string CAR_GUP { get; set; }

		/// <summary>
		/// 司機編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string DRIVER_ID { get; set; }

		/// <summary>
		/// 司機名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(10)")]
		public string DRIVER_NAME { get; set; }

		/// <summary>
		/// 加價費用
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal EXTRA_FEE { get; set; }

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
		/// 建立時間
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
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 區域加價
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal REGION_FEE { get; set; }

		/// <summary>
		/// 油資補貼
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal OIL_FEE { get; set; }

		/// <summary>
		/// 超點加價
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(13,2)")]
		public decimal OVERTIME_FEE { get; set; }

		/// <summary>
		/// 分配場
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string PACK_FIELD { get; set; }
	}
}
