namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 商品標籤主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("LABEL_SEQ", "CUST_CODE", "GUP_CODE")]
	[Table("F197001")]
	public class F197001 : IAuditInfo
	{

		/// <summary>
		/// 標籤流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "int")]
		public Int32 LABEL_SEQ { get; set; }

		/// <summary>
		/// 標籤編號(與報表編號相同)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(15)")]
		public string LABEL_CODE { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 廠商編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string VNR_CODE { get; set; }

		/// <summary>
		/// 保固期限(0貳 1壹 2半)F000904
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string WARRANTY { get; set; }

		/// <summary>
		/// 保固開始年份(西元年)
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? WARRANTY_S_Y { get; set; }

		/// <summary>
		/// 保固開始月份(1~12)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string WARRANTY_S_M { get; set; }

		/// <summary>
		/// 保固年份(1碼英文字)
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string WARRANTY_Y { get; set; }

		/// <summary>
		/// 保固月份(1碼英文字)
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string WARRANTY_M { get; set; }

		/// <summary>
		/// 保固日期(2碼數字)
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? WARRANTY_D { get; set; }

		/// <summary>
		/// 委外商代碼(預設A)
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string OUTSOURCE { get; set; }

		/// <summary>
		/// 檢驗員
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string CHECK_STAFF { get; set; }

		/// <summary>
		/// 物料說明1
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string ITEM_DESC_A { get; set; }

		/// <summary>
		/// 物料說明2
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string ITEM_DESC_B { get; set; }

		/// <summary>
		/// 物料說明3
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string ITEM_DESC_C { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
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
	}
}
