namespace Wms3pl.Datas.F51
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 每日車次路線結算檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "CAL_DATE", "DELV_NO")]
  [Table("F5112")]
  public class F5112 : IAuditInfo
  {

    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 計算日期
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CAL_DATE { get; set; }

    /// <summary>
    /// 車次/路線
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string DELV_NO { get; set; }

    /// <summary>
    /// 結算費
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal CAL_COST { get; set; }

    /// <summary>
    /// 加價費
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal EXTRA_FEE { get; set; }

    /// <summary>
    /// 加點費
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal PLUS_FEE { get; set; }

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
    /// 建立日期
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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
  }
}
