namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心當日投入人力
  /// </summary>
  [Serializable]
  [DataServiceKey("IMPORT_DATE","GRP_ID","DC_CODE")]
  [Table("F700701")]
  public class F700701 : IAuditInfo
  {

	  /// <summary>
	  /// 投入日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime IMPORT_DATE { get; set; }

	  /// <summary>
	  /// 工作群組F1953
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal GRP_ID { get; set; }

	  /// <summary>
	  /// 群組總人數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PERSON_NUMBER { get; set; }

	  /// <summary>
	  /// 群組總工時
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 WORK_HOUR { get; set; }

	  /// <summary>
	  /// 群組總薪資
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(13, 2)")]
    public decimal SALARY { get; set; }

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
        