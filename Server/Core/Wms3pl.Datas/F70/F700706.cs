namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 定期工作與優化工作執行比例統計
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","DC_CODE")]
  [Table("F700706")]
  public class F700706 : IAuditInfo
  {

	  /// <summary>
	  /// 統計日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CNT_DATE { get; set; }

	  /// <summary>
	  /// 統計星期(0~6=>日~一)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string CNT_DAY { get; set; }

	  /// <summary>
	  /// 定期工作數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 TIME_QTY { get; set; }

	  /// <summary>
	  /// 定期工作完成數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 TIME_FINISH_QTY { get; set; }

	  /// <summary>
	  /// 優化工作數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 OPTIMIZE_QTY { get; set; }

	  /// <summary>
	  /// 優化工作完成數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 OPTIMIZE_FINISH_QTY { get; set; }

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
	  /// 建立時間
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
    /// 異動時間
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
        