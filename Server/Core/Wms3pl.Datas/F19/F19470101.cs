namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 班次配送區域設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ALL_ID","DC_CODE","DELV_TIME","DELV_TMPR","ZIP_CODE","DELV_EFFIC","PAST_TYPE")]
  [Table("F19470101")]
  public class F19470101 : IAuditInfo
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
	  /// 出車時段
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_TIME { get; set; }

	  /// <summary>
	  /// 配送溫層(A:常溫、B:低溫)F000904
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_TMPR { get; set; }

	  /// <summary>
	  /// 配送郵遞區號(3碼)F1934
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

	  /// <summary>
	  /// 配送效率
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_EFFIC { get; set; }

	  /// <summary>
	  /// 0正物流1逆物流F000904
	  /// </summary>
    [Key]
    [Required]
	  public string PAST_TYPE { get; set; }
  }
}
        