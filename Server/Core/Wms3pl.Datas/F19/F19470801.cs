namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配送商計費代碼區域設定表
  /// </summary>
  [Serializable]
  [DataServiceKey("ALL_ID","DC_CODE","ACC_AREA_ID","ZIP_CODE")]
  [Table("F19470801")]
  public class F19470801 : IAuditInfo
  {

	  /// <summary>
	  /// 配送商編號
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 物流中心編號
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 計費區域代碼
	  /// </summary>
    [Key]
    [Required]
	  public Decimal ACC_AREA_ID { get; set; }

	  /// <summary>
	  /// 縣市郵遞區號(3碼)F1934
	  /// </summary>
    [Key]
    [Required]
	  public string ZIP_CODE { get; set; }

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
        