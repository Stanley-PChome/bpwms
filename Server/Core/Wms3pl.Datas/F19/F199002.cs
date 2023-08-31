namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 作業計價項目設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "ACC_ITEM_KIND_ID", "ORD_TYPE", "ACC_KIND", "ACC_UNIT", "DELV_ACC_TYPE")]
	[Table("F199002")]
	public class F199002 : IAuditInfo
	{

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 計價項目(F91000301)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ACC_ITEM_KIND_ID { get; set; }

		/// <summary>
		/// 物流單據(F000901)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ORD_TYPE { get; set; }

		/// <summary>
		/// 計價方式(A:單一費用 B：條件計費)F000904
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "char(1)")]
		public string ACC_KIND { get; set; }

		/// <summary>
		/// 計價單位(F91000302 Where ItemTypeId=003)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ACC_UNIT { get; set; }

		/// <summary>
		/// 計價數量
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public Int32 ACC_NUM { get; set; }

		/// <summary>
		/// 含稅(0:未稅 1:含稅)F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IN_TAX { get; set; }

		/// <summary>
		/// 單一費用
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(9,2)")]
		public decimal FEE { get; set; }

		/// <summary>
		/// <=設定數量費用
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(9,2)")]
		public decimal BASIC_FEE { get; set; }

		/// <summary>
		/// >設定數量每單位加收費用
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(9,2)")]
		public decimal OVER_FEE { get; set; }

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
		/// 配送計價類別 F91000301
		/// </summary>
		[Key]
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
		/// 計費類別編號(固定003)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string ITEM_TYPE_ID { get; set; }

		/// <summary>
		/// 計價項目名稱(同DC不得重複)
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string ACC_ITEM_NAME { get; set; }
	}
}
