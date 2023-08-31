namespace Wms3pl.Datas.F51
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;

  /// <summary>
  /// 物流中心派車運費結算檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CAL_DATE", "SEQ_NO", "CUST_CODE", "GUP_CODE", "DC_CODE")]
  [Table("F5107")]
  public class F5107
  {

    /// <summary>
    /// 結算日期
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CAL_DATE { get; set; }

    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 SEQ_NO { get; set; }

    /// <summary>
    /// 計價項目編號F91000301 Where ItemTypeId=005
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string ACC_ITEM_KIND_ID { get; set; }

    /// <summary>
    /// 派車日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? DELV_DATE { get; set; }

    /// <summary>
    /// 出車時間
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string TAKE_TIME { get; set; }

    /// <summary>
    /// 派車單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DISTR_CAR_NO { get; set; }

    /// <summary>
    /// 物流單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 託運單號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string PAST_NO { get; set; }

    /// <summary>
    /// 箱數
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? PACKAGE_BOX_NO { get; set; }

    /// <summary>
    /// 件數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 QTY { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal AMT { get; set; }

    /// <summary>
    /// 材積
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public decimal? VOLUMN { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Column(TypeName = "decimal(16, 2)")]
    public decimal? WEIGHT { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ZIP_CODE { get; set; }

    /// <summary>
    /// 溫層
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string DELV_TMPR { get; set; }

    /// <summary>
    /// 是否快速到貨(0否1是)
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string CAN_FAST { get; set; }

    /// <summary>
    /// 派車用途(02取件(逆物流),01送件(正物流))F000904 Where Topic='F700102'
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string DISTR_USE { get; set; }

    /// <summary>
    /// 是否專車0否1是
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string SP_CAR { get; set; }

    /// <summary>
    /// 配送商編號F1947
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string ALL_ID { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

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
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人員
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 配送計價類別 F000904(01:無 02:宅配 03:統倉 04:門店)
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string DELV_ACC_TYPE { get; set; }

    /// <summary>
    /// 合約編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CONTRACT_NO { get; set; }

    /// <summary>
    /// 報價單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string QUOTE_NO { get; set; }
  }
}
