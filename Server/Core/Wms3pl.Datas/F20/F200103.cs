namespace Wms3pl.Datas.F20
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(商品庫存調整明細)
  /// </summary>
  [Serializable]
  [DataServiceKey("ADJUST_NO", "ADJUST_SEQ", "GUP_CODE", "CUST_CODE", "DC_CODE")]
  [Table("F200103")]
  public class F200103 : IAuditInfo
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
    /// 作業類別(0調入1調出)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string WORK_TYPE { get; set; }

    /// <summary>
    /// 倉別編號(F1980)
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 儲位編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 廠商編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 有效日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime VALID_DATE { get; set; }

    /// <summary>
    /// 入庫日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENTER_DATE { get; set; }

    /// <summary>
    /// 調整數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ADJ_QTY { get; set; }

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
    /// 處理狀態(0:已處理)
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
    /// 箱號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BOX_CTRL_NO { get; set; }

    /// <summary>
    /// 板號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PALLET_CTRL_NO { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
  }
}
