namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 里程碑主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("MILESTONE_NO")]
  [Table("F19000102")]
  public class F19000102 : IAuditInfo
  {

	  /// <summary>
	  /// 里程碑編號
	  /// </summary>
    [Key]
    [Required]
	  public string MILESTONE_NO { get; set; }

	  /// <summary>
	  /// 里程碑名稱
	  /// </summary>
    [Required]
	  public string MILESTONE_NAME { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        