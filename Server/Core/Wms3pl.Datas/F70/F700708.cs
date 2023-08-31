namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心整體作業績效統計
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","DC_CODE","GUP_CODE","CUST_CODE","GRP_ID")]
  [Table("F700708")]
  public class F700708 : IAuditInfo
  {

	  /// <summary>
	  /// 統計日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CNT_DATE { get; set; }

	  /// <summary>
	  /// 統計星期(0~6=>日~一)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string CNT_DAY { get; set; }

	  /// <summary>
	  /// 投入總人力
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PERSON_NUMBER { get; set; }

	  /// <summary>
	  /// 投入總工時
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 WORK_HOUR { get; set; }

	  /// <summary>
	  /// 收貨件數(pcs)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 STOCK_QTY { get; set; }

	  /// <summary>
	  /// 調撥完成件數(pcs)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ALLOCATION_QTY { get; set; }

	  /// <summary>
	  /// 揀貨件數(pcs)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PICK_QTY { get; set; }

	  /// <summary>
	  /// 包裝件數(箱)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PACKAGE_QTY { get; set; }

	  /// <summary>
	  /// 稽核件數(箱)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 AUDIT_QTY { get; set; }

	  /// <summary>
	  /// 裝車件數(箱)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 INCAR_QTY { get; set; }

	  /// <summary>
	  /// 總件數(前面各欄位加總)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 TOTAL_QTY { get; set; }

	  /// <summary>
	  /// 平均每單位小時效率
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(9, 2)")]
    public decimal WORK_AVG { get; set; }

	  /// <summary>
	  /// 物流中心
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
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 工作群組F1953
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal GRP_ID { get; set; }
  }
}
        