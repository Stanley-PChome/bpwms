namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據流水號紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_TYPE")]
  [Table("F0009")]
  public class F0009 : IAuditInfo
  {

    /// <summary>
    /// 單據類型(F000901)
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 單據日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ORD_DATE { get; set; }

    /// <summary>
    /// 單據目前已使用流水號
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ORD_SEQ { get; set; }

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
    /// 建檔人名
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
