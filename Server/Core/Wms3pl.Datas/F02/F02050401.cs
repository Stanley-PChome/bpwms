namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 複驗異常處理歷程記錄
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F02050401")]

  public class F02050401 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

    /// <summary>
    /// F020504.ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 F020504_ID { get; set; }

    /// <summary>
    /// 項目說明
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public String PROC_DESC { get; set; }

    /// <summary>
    /// 複驗原因代碼
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public String RECHECK_CAUSE { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
    public String MEMO { get; set; }

    /// <summary>
    /// 處理人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String PROC_STAFF { get; set; }

    /// <summary>
    /// 處理人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public String PROC_NAME { get; set; }

    /// <summary>
    /// 處理時間
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime PROC_TIME { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public String CRT_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public String UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public String UPD_NAME { get; set; }
  }
}
