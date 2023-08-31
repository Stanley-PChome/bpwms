namespace Wms3pl.Datas.F25
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 序號與儲值卡盒/盒/箱號關聯紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_SEQ")]
  [Table("F250104")]
  public class F250104 : IAuditInfo
  {

    /// <summary>
    /// 紀錄流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 LOG_SEQ { get; set; }

    /// <summary>
    /// 紀錄時間
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime LOG_DATE { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

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
    /// 箱號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CASE_NO { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
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
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
