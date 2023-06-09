namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統選單樣式明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F19540201")]
  public class F19540201 : IAuditInfo
  {

	  /// <summary>
	  /// 系統選單樣式明細檔流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal ID { get; set; }

	  /// <summary>
	  /// 系統選單樣式編號F195402
	  /// </summary>
    [Required]
	  public string MENU_CODE { get; set; }

	  /// <summary>
	  /// 主分類 F000904 TOPIC=F195401 SUBTOPIC=CATEGORY
	  /// </summary>
    [Required]
	  public string CATEGORY { get; set; }

	  /// <summary>
	  /// 子分類 F000904 TOPIC=F195401 SUBTOPIC=SUB_CATEGORY
	  /// </summary>
    [Required]
	  public string SUB_CATEGORY { get; set; }

	  /// <summary>
	  /// 功能編號F1954
	  /// </summary>
    [Required]
	  public string FUN_CODE { get; set; }

	  /// <summary>
	  /// 功能順序
	  /// </summary>
    [Required]
	  public Decimal FUN_SORT { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        