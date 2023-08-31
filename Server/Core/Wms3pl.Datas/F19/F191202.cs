namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 儲位異動紀錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("TRANS_NO")]
	[Table("F191202")]
	public class F191202 : IAuditInfo
	{

		/// <summary>
		/// 異動紀錄編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal TRANS_NO { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime TRANS_DATE { get; set; }

		/// <summary>
		/// 異動作業類型(0新增,1刪除,2屬性修改)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string TRANS_STATUS { get; set; }

		/// <summary>
		/// 異動狀態(0修改前資料,1修改後資料)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string TRANS_WAY { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string TRANS_STAFF { get; set; }

		/// <summary>
		/// 儲位編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(14)")]
		public string LOC_CODE { get; set; }

		/// <summary>
		/// 樓層
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string FLOOR { get; set; }

		/// <summary>
		/// 儲區型態(F1919)
		/// </summary>
		[Column(TypeName = "char(3)")]
		public string AREA_CODE { get; set; }

		/// <summary>
		/// 通道別
		/// </summary>
		[Column(TypeName = "char(2)")]
		public string CHANNEL { get; set; }

		/// <summary>
		/// 座別
		/// </summary>
		[Column(TypeName = "char(2)")]
		public string PLAIN { get; set; }

		/// <summary>
		/// 層別
		/// </summary>
		[Column(TypeName = "char(2)")]
		public string LOC_LEVEL { get; set; }

		/// <summary>
		/// 儲位別
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string LOC_TYPE { get; set; }

		/// <summary>
		/// 儲位性質:1:揀貨儲位　2:暫存儲位
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string LOC_CHAR { get; set; }

		/// <summary>
		/// 儲位料架編號(F1942)
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string LOC_TYPE_ID { get; set; }

		/// <summary>
		/// 目前儲位狀態代碼(F1943)
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string NOW_STATUS_ID { get; set; }

		/// <summary>
		/// 修改前儲位狀態代碼
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string PRE_STATUS_ID { get; set; }

		/// <summary>
		/// 倉別編號(F1980)
		/// </summary>
		[Column(TypeName = "varchar(8)")]
		public string WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 建檔日期
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
		/// 異動原因
		/// </summary>
		[Column(TypeName = "varchar(6)")]
		public string UCC_CODE { get; set; }

		/// <summary>
		/// 建檔人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 業主(0:共用)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主(0:共用)
		/// </summary>
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 目前使用中業主
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string NOW_GUP_CODE { get; set; }

		/// <summary>
		/// 目前使用中貨主
		/// </summary>
		[Column(TypeName = "varchar(6)")]
		public string NOW_CUST_CODE { get; set; }

		/// <summary>
		/// 水平距離(公尺)
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? HOR_DISTANCE { get; set; }

		/// <summary>
		/// 租用期間(起)
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? RENT_BEGIN_DATE { get; set; }

		/// <summary>
		/// 租用期間(迄)
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? RENT_END_DATE { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }
	}
}
