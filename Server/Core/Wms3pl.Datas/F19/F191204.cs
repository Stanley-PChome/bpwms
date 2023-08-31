namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 建議儲位紀錄表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F191204")]
	public class F191204 : IAuditInfo
	{

		/// <summary>
		/// 編號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(TypeName = "bigint")]
		public Int64 ID { get; set; }

		/// <summary>
		/// 調撥單號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string ALLOCATION_NO { get; set; }

		/// <summary>
		/// 調撥單序號
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? ALLOCATION_SEQ { get; set; }

		/// <summary>
		/// 儲位
		/// </summary>
		[Column(TypeName = "varchar(14)")]
		public string LOC_CODE { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 狀態(0:待處理 1:已使用)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

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
