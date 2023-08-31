namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  ///  複驗異常處理
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020504")]

  public class F020504 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

    /// <summary>
    /// 處理日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime PROC_DATE { get; set; }

    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public String DC_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String CUST_CODE { get; set; }

    /// <summary>
    /// 進倉單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public String STOCK_NO { get; set; }

    /// <summary>
    /// 進倉項次
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public String STOCK_SEQ { get; set; }

    /// <summary>
    /// 驗收單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public String RT_NO { get; set; }

    /// <summary>
    /// 驗收序號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public String RT_SEQ { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String ITEM_CODE { get; set; }

    /// <summary>
    /// 原驗收數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int QTY { get; set; }

    /// <summary>
    /// 容器條碼
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(32)")]
    public String CONTAINER_CODE { get; set; }

    /// <summary>
    /// 容器分格條碼
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public String BIN_CODE { get; set; }

    /// <summary>
    /// 處理方式
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public String PROC_CODE { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
    public String MEMO { get; set; }

    /// <summary>
    /// 拒絕驗收數
    /// </summary>
    [Column(TypeName = "int")]
    public int? REMOVE_RECV_QTY { get; set; }

    /// <summary>
    /// 不良品數
    /// </summary>
    [Column(TypeName = "int")]
    public int? NOTGOOD_QTY { get; set; }

    /// <summary>
    /// 處理狀態
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public String STATUS { get; set; }

    /// <summary>
    /// F020502.ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 F020502_ID { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public String CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public String CRT_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public String UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public String UPD_NAME { get; set; }
  }
}
