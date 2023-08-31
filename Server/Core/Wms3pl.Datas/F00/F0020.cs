namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訊息代碼表
  /// </summary>
  [Serializable]
  [DataServiceKey("MSG_NO")]
  [Table("F0020")]
  public class F0020 : IAuditInfo
  {

    /// <summary>
    /// 訊息編號(頭2碼判斷訊息類別,第3碼:M訊息E錯誤)
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string MSG_NO { get; set; }

    /// <summary>
    /// 訊息主旨
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string MSG_SUBJECT { get; set; }

    /// <summary>
    /// 重要度(0低1一般2高)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MSG_LEVEL { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(500)")]
    public string MSG_CONTENT { get; set; }

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
