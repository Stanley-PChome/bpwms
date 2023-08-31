namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 專案計價設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ACC_PROJECT_NO")]
	[Table("F199007")]
	public class F199007 : IAuditInfo
	{

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 專案編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string ACC_PROJECT_NO { get; set; }

		/// <summary>
		/// 專案名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string ACC_PROJECT_NAME { get; set; }

		/// <summary>
		/// 生效日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime ENABLE_DATE { get; set; }

		/// <summary>
		/// 失效日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime DISABLE_DATE { get; set; }

		/// <summary>
		/// 報價單號
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(30)")]
		public string QUOTE_NO { get; set; }

		/// <summary>
		/// 含稅(0:未稅 1:含稅)F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string IN_TAX { get; set; }

		/// <summary>
		/// 費用
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public Int32 FEE { get; set; }

		/// <summary>
		/// 計價方式(A:一次性收費 B:月結)F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ACC_KIND { get; set; }

		/// <summary>
		/// 服務內容
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(300)")]
		public string SERVICES { get; set; }

		/// <summary>
		/// 狀態(0待處理1處理中2結案,9刪除)F000904
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
	}
}
