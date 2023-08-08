namespace Wms3pl.Datas.F50
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 請款明細表報表設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("REPORT_NO")]
  [Table("F500202")]
  public class F500202 : IAuditInfo
  {

	  /// <summary>
	  /// 報表編號
	  /// </summary>
    [Key]
    [Required]
	  public string REPORT_NO { get; set; }

	  /// <summary>
	  /// 報表名稱
	  /// </summary>
    [Required]
	  public string REPORT_NAME { get; set; }

	  /// <summary>
	  /// 計費類型(0:B2B 1:B2C)
	  /// </summary>
    [Required]
	  public string REPORT_TYPE { get; set; }

	  /// <summary>
	  /// 報表類型(0:總表 1:明細表)
	  /// </summary>
    [Required]
	  public string CNT_TYPE { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        