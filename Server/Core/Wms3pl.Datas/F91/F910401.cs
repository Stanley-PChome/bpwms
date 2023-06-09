namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 加工報價單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910401")]
  public class F910401 : IAuditInfo
  {

	  /// <summary>
	  /// 報價單項目編號
	  /// </summary>
    [Key]
    [Required]
	  public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 報價項目名稱
	  /// </summary>
    [Required]
	  public string QUOTE_NAME { get; set; }

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
	  /// 成本價(%)
	  /// </summary>
    [Required]
	  public decimal COST_PRICE { get; set; }

	  /// <summary>
	  /// 貨主加工申請價
	  /// </summary>
    [Required]
	  public decimal APPLY_PRICE { get; set; }

	  /// <summary>
	  /// 貨主加工核定價
	  /// </summary>
	  public decimal? APPROVED_PRICE { get; set; }

	  /// <summary>
	  /// 委外商 F1928
	  /// </summary>
    [Required]
	  public string OUTSOURCE_ID { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理  1:已核准 2:結案 9:取消)F000904
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

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
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }
  }
}
        