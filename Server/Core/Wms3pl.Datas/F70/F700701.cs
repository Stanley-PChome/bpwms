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
	  public DateTime IMPORT_DATE { get; set; }

	  /// <summary>
	  /// 工作群組F1953
	  /// </summary>
    [Key]
    [Required]
	  public Decimal GRP_ID { get; set; }

	  /// <summary>
	  /// 群組總人數
	  /// </summary>
    [Required]
	  public Int32 PERSON_NUMBER { get; set; }

	  /// <summary>
	  /// 群組總工時
	  /// </summary>
    [Required]
	  public Int32 WORK_HOUR { get; set; }

	  /// <summary>
	  /// 群組總薪資
	  /// </summary>
    [Required]
	  public decimal SALARY { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        