namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 下位系統的庫存原始總表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060601")]
	public class F060601 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// 對接系統(0: Geek、1: 盟立)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string SRC_SYSTEM { get; set; }
		/// <summary>
		/// 對帳時間(yyyy/MM/dd HH:ii:ss)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(19)")]
    public string AUDIT_DATE { get; set; }
		/// <summary>
		/// 備份日期
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CAL_DATE { get; set; }
		/// <summary>
		/// 總頁數
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int TOTAL_PAGE { get; set; }
		/// <summary>
		/// 當前頁
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int CURRENT_PAGE { get; set; }
		/// <summary>
		/// 一頁筆數
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int PAGE_SIZE { get; set; }
		/// <summary>
		/// 處理狀態 (0: 待處理 1: 處理中 2: 已匯出 9: 重複匯入取消)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }
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
    [Column(TypeName = "varchar(20)")]
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
