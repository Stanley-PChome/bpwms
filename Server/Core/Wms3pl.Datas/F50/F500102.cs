namespace Wms3pl.Datas.F50
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 運費報價單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F500102")]
  public class F500102 : IAuditInfo
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
	  /// 計價項目名稱F199005
	  /// </summary>
    [Required]
	  public string ACC_ITEM_NAME { get; set; }

	  /// <summary>
	  /// 單據類型(0:B2B 1:B2C)F000904 Where Topic=F190005 Subtopic=CUST_TYPE
	  /// </summary>
    [Required]
	  public string CUST_TYPE { get; set; }

	  /// <summary>
	  /// 物流類別(01:正物流 02:逆物流)F000904 Where Topic=F190005 Subtopic=LOGI_TYPE
	  /// </summary>
    [Required]
	  public string LOGI_TYPE { get; set; }

	  /// <summary>
	  /// 計價方式(C:實際尺寸 D:材積 E:重量,F:均一價)F000904 Where Topic=F190005 Subtopic=ACC_KIND
	  /// </summary>
    [Required]
	  public string ACC_KIND { get; set; }

	  /// <summary>
	  /// 是否為專車(0:否1是)
	  /// </summary>
    [Required]
	  public string IS_SPECIAL_CAR { get; set; }

	  /// <summary>
	  /// 車輛種類ID F194702
	  /// </summary>
	  public Decimal? CAR_KIND_ID { get; set; }

	  /// <summary>
	  /// 計費區域代碼 F1948
	  /// </summary>
	  public Decimal? ACC_AREA_ID { get; set; }

	  /// <summary>
	  /// 配送溫層(A:常溫、B：低溫)F000904 Where Topic=F1980 Subtopic=TMPR_TYPE
	  /// </summary>
    [Required]
	  public string DELV_TMPR { get; set; }

	  /// <summary>
	  /// 配送效率 F190102
	  /// </summary>
    [Required]
	  public string DELV_EFFIC { get; set; }

	  /// <summary>
	  /// 計價單位(F91000302 Where ItemTypeId=005)
	  /// </summary>
    [Required]
	  public string ACC_UNIT { get; set; }

	  /// <summary>
	  /// 含稅(0:未稅 1:含稅)F000904
	  /// </summary>
    [Required]
	  public string IN_TAX { get; set; }

	  /// <summary>
	  /// 計價數量
	  /// </summary>
    [Required]
	  public decimal ACC_NUM { get; set; }

	  /// <summary>
	  /// 最大重量(kg)
	  /// </summary>
	  public decimal? MAX_WEIGHT { get; set; }

	  /// <summary>
	  /// 費用
	  /// </summary>
    [Required]
	  public decimal FEE { get; set; }

	  /// <summary>
	  /// 貨主核定費用
	  /// </summary>
	  public decimal? APPROV_FEE { get; set; }

	  /// <summary>
	  /// 超標計費單位量
	  /// </summary>
	  public decimal? OVER_VALUE { get; set; }

	  /// <summary>
	  /// 超標每單位費用
	  /// </summary>
	  public decimal? OVER_UNIT_FEE { get; set; }

	  /// <summary>
	  /// 貨主核定超標每單位費用
	  /// </summary>
	  public decimal? APPROV_OVER_UNIT_FEE { get; set; }

	  /// <summary>
	  /// 配送計價類別 F91000301
	  /// </summary>
    [Required]
	  public string DELV_ACC_TYPE { get; set; }

	  /// <summary>
	  /// 計費類別編號(固定005)
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
        