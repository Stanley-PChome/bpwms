namespace Wms3pl.Datas.F50
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 倉租報價單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F500101")]
  public class F500101 : IAuditInfo
  {

	  /// <summary>
	  /// 報價單編號
	  /// </summary>
    [Key]
    [Required]
	  public string QUOTE_NO { get; set; }

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
	  /// 毛利率
	  /// </summary>
    [Required]
	  public Single NET_RATE { get; set; }

	  /// <summary>
	  /// 計價項目
	  /// </summary>
    [Required]
	  public string ACC_ITEM_KIND_ID { get; set; }

	  /// <summary>
	  /// 計價項目名稱F199001
	  /// </summary>
    [Required]
	  public string ACC_ITEM_NAME { get; set; }

	  /// <summary>
	  /// 料架編號(F1942)
	  /// </summary>
    [Required]
	  public string LOC_TYPE_ID { get; set; }

	  /// <summary>
	  /// 溫層(01:常溫 02:低溫 03:冷凍)F000904 Where Topic=F1980 Subtopic=TMPR_TYPE
	  /// </summary>
    [Required]
	  public string TMPR_TYPE { get; set; }

	  /// <summary>
	  /// 計價數量
	  /// </summary>
    [Required]
	  public Int32 ACC_NUM { get; set; }

	  /// <summary>
	  /// 計價單位 F91000302 Where ItemTypeId=002
	  /// </summary>
    [Required]
	  public string ACC_UNIT { get; set; }

	  /// <summary>
	  /// 單位費用
	  /// </summary>
    [Required]
	  public decimal UNIT_FEE { get; set; }

	  /// <summary>
	  /// 貨主核定價
	  /// </summary>
	  public decimal? APPROV_UNIT_FEE { get; set; }

	  /// <summary>
	  /// 含稅(0:未稅 1:含稅)F000904
	  /// </summary>
    [Required]
	  public string IN_TAX { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理  1:已核准 2:結案 9:取消)F000904
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 服務內容
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 物流中心(000:不指定)
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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
        