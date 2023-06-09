namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 查詢轉檔權限設定
  /// </summary>
  [Serializable]
  [DataServiceKey("GRP_ID","QID")]
  [Table("F190704")]
  public class F190704 : IAuditInfo
  {

	  /// <summary>
	  /// 工作群組編號F1953
	  /// </summary>
    [Key]
    [Required]
	  public Decimal GRP_ID { get; set; }

	  /// <summary>
	  /// 查詢編號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal QID { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建立時間
	  /// </summary>
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        