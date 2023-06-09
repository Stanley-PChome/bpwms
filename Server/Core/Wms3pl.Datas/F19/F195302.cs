namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 工作群組排程權限設定
  /// </summary>
  [Serializable]
  [DataServiceKey("SCHEDULE_ID","GRP_ID")]
  [Table("F195302")]
  public class F195302 : IAuditInfo
  {

	  /// <summary>
	  /// 排程權限編號F000904
	  /// </summary>
    [Key]
    [Required]
	  public string SCHEDULE_ID { get; set; }

	  /// <summary>
	  /// 工作群組編號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal GRP_ID { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
        