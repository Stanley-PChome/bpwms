namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 商品材積階層檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ITEM_CODE", "UNIT_ID", "GUP_CODE", "CUST_CODE")]
	[Table("F190301")]
	public class F190301 : IAuditInfo
	{

		/// <summary>
		/// 商品編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 單位階層
		/// </summary>
		[Required]
		[Column(TypeName = "smallint")]
		public Int16 UNIT_LEVEL { get; set; }

		/// <summary>
		/// 計量單位F91000302 Where ITEM_TYPE_ID=001
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string UNIT_ID { get; set; }

		/// <summary>
		/// 最小單位數量
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public Int32 UNIT_QTY { get; set; }

		/// <summary>
		/// 長度
		/// </summary>
		[Column(TypeName = "decimal(8,2)")]
		public decimal? LENGTH { get; set; }

		/// <summary>
		/// 寬度
		/// </summary>
		[Column(TypeName = "decimal(8,2)")]
		public decimal? WIDTH { get; set; }

		/// <summary>
		/// 高度
		/// </summary>
		[Column(TypeName = "decimal(8,2)")]
		public decimal? HIGHT { get; set; }

		/// <summary>
		/// 重量
		/// </summary>
		[Column(TypeName = "decimal(10,2)")]
		public decimal? WEIGHT { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string GUP_CODE { get; set; }

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

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 系統單位(01:盒入數;02:箱入數)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string SYS_UNIT { get; set; }
	}
}
