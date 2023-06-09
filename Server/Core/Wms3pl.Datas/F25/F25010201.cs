namespace Wms3pl.Datas.F25
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 序號凍結管制作業
  /// </summary>
  [Serializable]
  [DataServiceKey("FREEZE_LOG_SEQ","CONTROL","GUP_CODE","CUST_CODE")]
  [Table("F25010201")]
  public class F25010201 : IAuditInfo
  {

	  /// <summary>
	  /// 流水序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 FREEZE_LOG_SEQ { get; set; }

	  /// <summary>
	  /// 管制作業(01進項02銷項03調撥04退貨05加工)(F000904)
	  /// </summary>
    [Key]
    [Required]
	  public string CONTROL { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        