namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 合約單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CONTRACT_NO","DC_CODE","GUP_CODE")]
  [Table("F910301")]
  public class F910301 : IAuditInfo
  {

	  /// <summary>
	  /// 合約單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 生效日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENABLE_DATE { get; set; }

	  /// <summary>
	  /// 失效日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime DISABLE_DATE { get; set; }

	  /// <summary>
	  /// 合約對象類型(0貨主1委外商)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string OBJECT_TYPE { get; set; }

	  /// <summary>
	  /// 統一編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string UNI_FORM { get; set; }

	  /// <summary>
	  /// 單據狀態
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(300)")]
    public string MEMO { get; set; }

	  /// <summary>
	  /// 物流中心(000:不指定)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

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
    /// 結算日(1~31 or 0~6(禮拜日~禮拜一))
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? CYCLE_DATE { get; set; }

	  /// <summary>
	  /// 結算週期(2:月)F000904
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CLOSE_CYCLE { get; set; }
  }
}
        