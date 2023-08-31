namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 人員錯誤狀況
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","EMP_ID","EMP_NAME","GRP_ID","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F700705")]
  public class F700705 : IAuditInfo
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
	  /// 員工編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string EMP_ID { get; set; }

	  /// <summary>
	  /// 姓名
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "nvarchar(88)")]
    public string EMP_NAME { get; set; }

	  /// <summary>
	  /// 工作群組編號F1953
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 GRP_ID { get; set; }

	  /// <summary>
	  /// 錯誤件數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ERROR_QTY { get; set; }

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
  }
}
        