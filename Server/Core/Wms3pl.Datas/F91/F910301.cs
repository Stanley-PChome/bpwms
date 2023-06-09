namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 合約單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CONTRACT_NO","DC_CODE","GUP_CODE")]
  [Table("F910301")]
  public class F910301 : IAuditInfo
  {

	  /// <summary>
	  /// 合約單號
	  /// </summary>
    [Key]
    [Required]
	  public string CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 生效日期
	  /// </summary>
    [Required]
	  public DateTime ENABLE_DATE { get; set; }

	  /// <summary>
	  /// 失效日期
	  /// </summary>
    [Required]
	  public DateTime DISABLE_DATE { get; set; }

	  /// <summary>
	  /// 合約對象類型(0貨主1委外商)
	  /// </summary>
    [Required]
	  public string OBJECT_TYPE { get; set; }

	  /// <summary>
	  /// 統一編號
	  /// </summary>
    [Required]
	  public string UNI_FORM { get; set; }

	  /// <summary>
	  /// 單據狀態
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 物流中心(000:不指定)
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

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

	  /// <summary>
	  /// 結算日(1~31 or 0~6(禮拜日~禮拜一))
	  /// </summary>
	  public Decimal? CYCLE_DATE { get; set; }

	  /// <summary>
	  /// 結算週期(2:月)F000904
	  /// </summary>
    [Required]
	  public string CLOSE_CYCLE { get; set; }
  }
}
        