namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 派車計價項目設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("SEQ")]
	[Table("F199005")]
	public class F199005 : IAuditInfo
	{

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 計價項目(F91000301 WhereItemTypeId=005)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ACC_ITEM_KIND_ID { get; set; }

		/// <summary>
		/// 物流類別 01送件(正物流),02取件(逆物流) F000904
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string LOGI_TYPE { get; set; }

		/// <summary>
		/// 計價方式(C:實際尺寸 D:材積 E:重量,F:均一價 )F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ACC_KIND { get; set; }

		/// <summary>
		/// 是否為專車(0:否1是)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IS_SPECIAL_CAR { get; set; }

		/// <summary>
		/// 車輛種類ID F194702
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? CAR_KIND_ID { get; set; }

		/// <summary>
		/// 計費區域代碼 F1948 (0=全部)
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal ACC_AREA_ID { get; set; }

		/// <summary>
		/// 配送溫層(A:常溫、B：低溫)F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string DELV_TMPR { get; set; }

		/// <summary>
		/// 配送效率 F190102
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string DELV_EFFIC { get; set; }

		/// <summary>
		/// 計價單位(F91000302 Where ItemTypeId=005)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ACC_UNIT { get; set; }

		/// <summary>
		/// 含稅(0:未稅 1:含稅)F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IN_TAX { get; set; }

		/// <summary>
		/// 計價數量
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(9,2)")]
		public decimal ACC_NUM { get; set; }

		/// <summary>
		/// 最大重量(kg)
		/// </summary>
		[Column(TypeName = "decimal(9,2)")]
		public decimal? MAX_WEIGHT { get; set; }

		/// <summary>
		/// 費用
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(9,2)")]
		public decimal FEE { get; set; }

		/// <summary>
		/// 超標計費單位量
		/// </summary>
		[Column(TypeName = "decimal(9,2)")]
		public decimal? OVER_VALUE { get; set; }

		/// <summary>
		/// 超標每單位費用
		/// </summary>
		[Column(TypeName = "decimal(9,2)")]
		public decimal? OVER_UNIT_FEE { get; set; }

		/// <summary>
		/// 配送計價類別 F91000301
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string DELV_ACC_TYPE { get; set; }

		/// <summary>
		/// 狀態(0使用中,9刪除)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 計費類別編號(固定005)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string ITEM_TYPE_ID { get; set; }

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
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 計價項目名稱(同DC不得重複)
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string ACC_ITEM_NAME { get; set; }

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal SEQ { get; set; }
	}
}
