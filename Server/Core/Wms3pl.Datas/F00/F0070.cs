namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統登入記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CONNECTID")]
  [Table("F0070")]
  public class F0070 : IAuditInfo
  {

    /// <summary>
    /// 連線ID
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string CONNECTID { get; set; }

    /// <summary>
    /// 登入帳號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string USERNAME { get; set; }

    /// <summary>
    /// 登入主機名稱
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string HOSTNAME { get; set; }

    /// <summary>
    /// 解鎖時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UNLOCKTIME { get; set; }

    /// <summary>
    /// 群組HUB名稱
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string GROUPNAME { get; set; }

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔人名
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
