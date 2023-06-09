namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配送費用設定表
  /// </summary>
  [Serializable]
  [DataServiceKey("ALL_ID","DC_CODE","ACC_AREA_ID","DELV_EFFIC","DELV_TMPR","CUST_TYPE","LOGI_TYPE","ACC_KIND","ACC_DELVNUM_ID","ACC_TYPE","BASIC_VALUE")]
  [Table("F194707")]
  public class F194707 : IAuditInfo
  {

	  /// <summary>
	  /// 配送商編號(F1947)
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
	  /// 計費區域代碼(F194708)
	  /// </summary>
    [Key]
    [Required]
	  public Decimal ACC_AREA_ID { get; set; }

	  /// <summary>
	  /// 配送效率F190102
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_EFFIC { get; set; }

	  /// <summary>
	  /// 配送溫層(A:常溫、B:低溫)F000904
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_TMPR { get; set; }

	  /// <summary>
	  /// 單據類型(0:B2B 1:B2C)  F000904
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_TYPE { get; set; }

	  /// <summary>
	  /// 物流類別(01:正物流 02:逆物流)  F000904
	  /// </summary>
    [Key]
    [Required]
	  public string LOGI_TYPE { get; set; }

	  /// <summary>
	  /// 計價方式(C:實際尺寸 D:材積 E:重量,F:均一價 )F000904
	  /// </summary>
    [Key]
    [Required]
	  public string ACC_KIND { get; set; }

	  /// <summary>
	  /// 配送單量範圍代碼(F194709)
	  /// </summary>
    [Key]
    [Required]
	  public Decimal ACC_DELVNUM_ID { get; set; }

	  /// <summary>
	  /// 計費類型(A:平日、B：假日)F000904
	  /// </summary>
    [Key]
    [Required]
	  public string ACC_TYPE { get; set; }

	  /// <summary>
	  /// 基本值(cm/材/kg)
	  /// </summary>
    [Key]
    [Required]
	  public decimal BASIC_VALUE { get; set; }

	  /// <summary>
	  /// 最大重量(kg)
	  /// </summary>
    [Required]
	  public decimal MAX_WEIGHT { get; set; }

	  /// <summary>
	  /// 費用
	  /// </summary>
    [Required]
	  public decimal FEE { get; set; }

	  /// <summary>
	  /// 超標計費單位量
	  /// </summary>
	  public decimal? OVER_VALUE { get; set; }

	  /// <summary>
	  /// 超標每單位費用
	  /// </summary>
	  public decimal? OVER_UNIT_FEE { get; set; }

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
	  /// 是否含稅(0:未稅、1:含稅)
	  /// </summary>
    [Required]
	  public string IN_TAX { get; set; }

	  /// <summary>
	  /// 狀態(0使用中9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }
  }
}
        