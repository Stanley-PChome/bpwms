namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥(退貨上架)申請頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_APPLY_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161601")]
  public class F161601 : IAuditInfo
  {

	  /// <summary>
	  /// 調撥申請單號
	  /// </summary>
    [Key]
    [Required]
		[Column(TypeName = "varchar(20)")]
	  public string RTN_APPLY_NO { get; set; }

	  /// <summary>
	  /// 調撥申請單建立日期
	  /// </summary>
    [Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime RTN_APPLY_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理  2:結案 9:取消)
	  /// </summary>
    [Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 備註
		/// </summary>
		[Column(TypeName = "nvarchar(300)")]
		public string MEMO { get; set; }

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
		[Column(TypeName = "varchar(12)")]
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

		/// <summary>
		/// 核准日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? APPROVE_DATE { get; set; }

		/// <summary>
		/// 核准人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string APPROVE_STAFF { get; set; }

		/// <summary>
		/// 核准人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string APPROVE_NAME { get; set; }
  }
}
        