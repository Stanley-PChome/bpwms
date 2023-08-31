namespace Wms3pl.Datas.F25
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 序號凍結紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_SEQ", "GUP_CODE", "CUST_CODE")]
  [Table("F250102")]
  public class F250102 : IAuditInfo
  {

    /// <summary>
    /// 流水序號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 LOG_SEQ { get; set; }

    /// <summary>
    /// 凍結日期(起)
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime FREEZE_BEGIN_DATE { get; set; }

    /// <summary>
    /// 凍結日期(迄)
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime FREEZE_END_DATE { get; set; }

    /// <summary>
    /// 凍結0 解凍1
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string FREEZE_TYPE { get; set; }

    /// <summary>
    /// 序號起
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO_BEGIN { get; set; }

    /// <summary>
    /// 序號迄
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO_END { get; set; }

    /// <summary>
    /// 盒號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BOX_SERIAL { get; set; }

    /// <summary>
    /// 儲值卡盒號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }

    /// <summary>
    /// 凍結原因 F1951.UCT_ID=SF
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string CAUSE { get; set; }

    /// <summary>
    /// 原因備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MEMO { get; set; }

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
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

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
