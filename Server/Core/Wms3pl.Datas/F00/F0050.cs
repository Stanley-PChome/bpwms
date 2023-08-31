namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 使用者登入程式記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ_NO")]
  [Table("F0050")]
  public class F0050 : IAuditInfo
  {

    /// <summary>
    /// 紀錄流水號
    /// </summary>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal SEQ_NO { get; set; }

    /// <summary>
    /// 紀錄時間
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime LOG_DATE { get; set; }

    /// <summary>
    /// 電腦IP
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string MACHINE { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string MESSAGE { get; set; }

    /// <summary>
    /// 程式編號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string FUN_ID { get; set; }

    /// <summary>
    /// 程式名稱
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string FUNCTION_NAME { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建檔人名
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
