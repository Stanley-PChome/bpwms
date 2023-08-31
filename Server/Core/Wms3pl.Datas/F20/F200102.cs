namespace Wms3pl.Datas.F20
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(訂單調整明細)
  /// </summary>
  [Serializable]
  [DataServiceKey("ADJUST_NO", "ADJUST_SEQ", "GUP_CODE", "CUST_CODE", "DC_CODE")]
  [Table("F200102")]
  public class F200102 : IAuditInfo
  {

    /// <summary>
    /// 調整單單號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ADJUST_NO { get; set; }

    /// <summary>
    /// 調整單序號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 ADJUST_SEQ { get; set; }

    /// <summary>
    /// 作業類別F000904
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string WORK_TYPE { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 新收件地址
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string ADDRESS { get; set; }

    /// <summary>
    /// 配送商F1947
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string ALL_ID { get; set; }

    /// <summary>
    /// 調整原因
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CAUSE { get; set; }

    /// <summary>
    /// 調整原因備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string CAUSE_MEMO { get; set; }

    /// <summary>
    /// 新出貨物流中心
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string NEW_DC_CODE { get; set; }

    /// <summary>
    /// 訂單原始狀態(F050301.STATUS)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ORG_STATUS { get; set; }

    /// <summary>
    /// 出貨單原批次時段
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ORG_PICK_TIME { get; set; }

    /// <summary>
    /// 處理狀態(0:已處理 9:刪除)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

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
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

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
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

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

    /// <summary>
    /// 新訂單編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string NEW_ORD_NO { get; set; }

    /// <summary>
    /// 原收件地址
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string ORG_ADDRESS { get; set; }
  }
}
