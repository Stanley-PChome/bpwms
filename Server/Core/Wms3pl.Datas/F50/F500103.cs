namespace Wms3pl.Datas.F50
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨報價單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F500103")]
  public class F500103 : IAuditInfo
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
	  /// 計價項目名稱F199003
	  /// </summary>
    [Required]
	  public string ACC_ITEM_NAME { get; set; }

	  /// <summary>
	  /// 計價項目
	  /// </summary>
    [Required]
	  public string ACC_ITEM_KIND_ID { get; set; }

	  /// <summary>
	  /// 計價方式(A:單一費用 B：條件計費)F000904
	  /// </summary>
    [Required]
	  public string ACC_KIND { get; set; }

	  /// <summary>
	  /// 計價單位(F91000302 Where ItemTypeId=004)
	  /// </summary>
    [Required]
	  public string ACC_UNIT { get; set; }

	  /// <summary>
	  /// 計價數量
	  /// </summary>
    [Required]
	  public Int32 ACC_NUM { get; set; }

	  /// <summary>
	  /// 含稅(0:未稅 1:含稅)F000904
	  /// </summary>
    [Required]
	  public string IN_TAX { get; set; }

	  /// <summary>
	  /// 單一費用
	  /// </summary>
    [Required]
	  public decimal FEE { get; set; }

	  /// <summary>
	  /// <=設定數量費用
	  /// </summary>
    [Required]
	  public decimal BASIC_FEE { get; set; }

	  /// <summary>
	  /// >設定數量每單位加收費用
	  /// </summary>
    [Required]
	  public decimal OVER_FEE { get; set; }

	  /// <summary>
	  /// 配送計價類別 F91000301
	  /// </summary>
    [Required]
	  public string DELV_ACC_TYPE { get; set; }

	  /// <summary>
	  /// 貨主核定單一費用
	  /// </summary>
	  public decimal? APPROV_FEE { get; set; }

	  /// <summary>
	  /// 貨主核定設定基本費用
	  /// </summary>
	  public decimal? APPROV_BASIC_FEE { get; set; }

	  /// <summary>
	  /// 貨主核定加收費用
	  /// </summary>
	  public decimal? APPROV_OVER_FEE { get; set; }

	  /// <summary>
	  /// 計價類別編號(固定為004)
	  /// </summary>
    [Required]
	  public string ITEM_TYPE_ID { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        