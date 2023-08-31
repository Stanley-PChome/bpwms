namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 倉別主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("WAREHOUSE_ID", "DC_CODE")]
	[Table("F1980")]
	public class F1980 : IAuditInfo
	{

		/// <summary>
		/// 倉別編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 倉別名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(80)")]
		public string WAREHOUSE_NAME { get; set; }

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
		/// 倉別型態(F198001)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string WAREHOUSE_TYPE { get; set; }

		/// <summary>
		/// 溫層(01:常溫 02:低溫 03:冷凍)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string TMPR_TYPE { get; set; }

		/// <summary>
		/// 是否計算庫存
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string CAL_STOCK { get; set; }

		/// <summary>
		/// 是否計算費用
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string CAL_FEE { get; set; }

		/// <summary>
		/// 料架編號(F1942)
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string LOC_TYPE_ID { get; set; }

		/// <summary>
		/// 水平距離
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? HOR_DISTANCE { get; set; }

		/// <summary>
		/// 租用日期(起)
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? RENT_BEGIN_DATE { get; set; }

		/// <summary>
		/// 租用日期(迄)
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? RENT_END_DATE { get; set; }

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
		/// 擴增預留欄位A
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_A { get; set; }

		/// <summary>
		/// 擴增預留欄位B
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_B { get; set; }

		/// <summary>
		/// 擴增預留欄位C
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_C { get; set; }

		/// <summary>
		/// 擴增預留欄位D
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_D { get; set; }

		/// <summary>
		/// 擴增預留欄位E
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_E { get; set; }

		/// <summary>
		/// 裝置類型 MANUAL:人工, AGV:AGV, SHUTTLE:穿梭車
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string DEVICE_TYPE { get; set; }

		/// <summary>
		/// 揀貨樓層
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string PICK_FLOOR { get; set; }
	}
}
