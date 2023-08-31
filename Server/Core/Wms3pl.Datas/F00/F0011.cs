namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 人員單據作業記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID", "DC_CODE", "CUST_CODE", "GUP_CODE")]
  [Table("F0011")]
  public class F0011 : IAuditInfo
  {

    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 員工編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string EMP_ID { get; set; }

    /// <summary>
    /// 單據編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORDER_NO { get; set; }

    /// <summary>
    /// 狀態 0待處理、１已完成
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 作業開始時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? START_DATE { get; set; }

    /// <summary>
    /// 作業結束時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? CLOSE_DATE { get; set; }

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
