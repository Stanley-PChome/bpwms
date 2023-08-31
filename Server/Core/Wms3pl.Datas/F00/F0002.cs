namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 跨庫物流中心主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE", "LOGISTIC_CODE")]
  [Table("F0002")]
  public class F0002 : IAuditInfo
  {
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 物流商編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string LOGISTIC_CODE { get; set; }

    /// <summary>
    /// 物流商名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string LOGISTIC_NAME { get; set; }

    /// <summary>
    /// 是否碼頭收貨對點作業使用
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_PIER_RECV_POINT { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人員名稱
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
    /// 異動人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 是否廠退出貨扣帳作業使用
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_VENDOR_RETURN { get; set; }
  }
}
