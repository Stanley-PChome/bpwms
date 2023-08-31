namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 派車單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DISTR_CAR_NO","DC_CODE")]
  [Table("F700101")]
  public class F700101 : IAuditInfo
  {

	  /// <summary>
	  /// 派車單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DISTR_CAR_NO { get; set; }

	  /// <summary>
	  /// 派車日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime TAKE_DATE { get; set; }

	  /// <summary>
	  /// 配送商編號F1947
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string ALL_ID { get; set; }

    /// <summary>
    /// 車輛編號F194703
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? CAR_KIND_ID { get; set; }

	  /// <summary>
	  /// 是否專車(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SP_CAR { get; set; }

	  /// <summary>
	  /// 費用歸屬貨主
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CHARGE_CUST { get; set; }

	  /// <summary>
	  /// 費用歸屬物流中心
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CHARGE_DC { get; set; }

	  /// <summary>
	  /// 費用
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(8, 2)")]
    public decimal FEE { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理 1:已配車 2:結案  9:取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

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
    /// 配送碼頭編號F1981
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string PIER_CODE { get; set; }

    /// <summary>
    /// 費用歸屬貨主時,歸屬的業主
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string CHARGE_GUP_CODE { get; set; }

    /// <summary>
    /// 費用歸屬貨主時,歸屬的貨主
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CHARGE_CUST_CODE { get; set; }

    /// <summary>
    /// 原派車指定配送碼頭F1981
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string ORD_PIER_CODE { get; set; }

	  /// <summary>
	  /// 派車來源0:MANUALLY、1:API
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(1)")]
    public string DISTR_SOURCE { get; set; }

	  /// <summary>
	  /// 是否為有綁定物流單據的派車單
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string HAVE_WMS_NO { get; set; }
  }
}
        