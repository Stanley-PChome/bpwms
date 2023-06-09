namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 原因編碼檔
  /// </summary>
  [Serializable]
  [DataServiceKey("UCC_CODE","UCT_ID")]
  [Table("F1951")]
  public class F1951 : IAuditInfo
  {

	  /// <summary>
	  /// 原因編號
	  /// </summary>
    [Key]
    [Required]
	  public string UCC_CODE { get; set; }

	  /// <summary>
	  /// 原因
	  /// </summary>
    [Required]
	  public string CAUSE { get; set; }

	  /// <summary>
	  /// 原因類別編號
	  /// </summary>
    [Key]
    [Required]
	  public string UCT_ID { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

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
        