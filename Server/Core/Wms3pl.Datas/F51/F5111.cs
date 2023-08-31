namespace Wms3pl.Datas.F51
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 計價設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "CAL_TYPE", "RANGE_LEVEL")]
  [Table("F5111")]
  public class F5111 : IAuditInfo
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
    /// 計價類型F000904(01:材積;02:重量)
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CAL_TYPE { get; set; }

    /// <summary>
    /// 級距階層
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 RANGE_LEVEL { get; set; }

    /// <summary>
    /// 級距階層最大值
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal RANGE_MAX { get; set; }

    /// <summary>
    /// 是否包含級距階層最大值(0:不包含 1:包含)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string INCLOUDE { get; set; }

    /// <summary>
    /// 每趟基本運價
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 COST { get; set; }

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
