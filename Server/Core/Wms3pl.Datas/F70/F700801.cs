namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 專車單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","DELIVERY_NO")]
  [Table("F700801")]
  public class F700801 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 專車單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DELIVERY_NO { get; set; }

    /// <summary>
    /// 預計出車日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? TAKE_DATE { get; set; }

	  /// <summary>
	  /// 車行編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CARRIAGE_ID { get; set; }

    /// <summary>
    /// 車行名稱
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string CARRIAGE_NAME { get; set; }

	  /// <summary>
	  /// 車輛種類編號(F194702)
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal CAR_KIND_ID { get; set; }

    /// <summary>
    /// 費用歸屬貨主
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string CHARGE_CUST { get; set; }

    /// <summary>
    /// 費用歸屬物流中心
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string CHARGE_DC { get; set; }

    /// <summary>
    /// 費用歸屬貨主時,歸屬的貨主
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CHARGE_CUST_CODE { get; set; }

    /// <summary>
    /// 費用
    /// </summary>
    [Column(TypeName = "decimal(8, 2)")]
    public decimal? FEE { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string MEMO { get; set; }

    /// <summary>
    /// 單據狀態(0: 待處理、1: 結案、9: 取消)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

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
        