namespace Wms3pl.Datas.F50
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 結算表主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","CONTRACT_NO","QUOTE_NO")]
  [Table("F500201")]
  public class F500201 : IAuditInfo
  {

	  /// <summary>
	  /// 結算日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CNT_DATE { get; set; }

	  /// <summary>
	  /// 合約單號
	  /// </summary>
    [Key]
    [Required]
	  public string CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 報價單號(計價項目)
	  /// </summary>
    [Key]
    [Required]
	  public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 計價項目類別F910003
	  /// </summary>
	  public string ITEM_TYPE_ID { get; set; }

	  /// <summary>
	  /// 計價項目名稱
	  /// </summary>
	  public string ACC_ITEM_NAME { get; set; }

	  /// <summary>
	  /// 成本總計
	  /// </summary>
	  public decimal? COST { get; set; }

	  /// <summary>
	  /// 結算金額
	  /// </summary>
	  public decimal? AMOUNT { get; set; }

	  /// <summary>
	  /// 是否含稅(0否1是)
	  /// </summary>
    [Required]
	  public string IS_TAX { get; set; }

	  /// <summary>
	  /// 單據狀態(0待請款1已關帳)
	  /// </summary>
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 關帳時間
	  /// </summary>
	  public DateTime? CLOSE_DATE { get; set; }

	  /// <summary>
	  /// 請款總表列印時間
	  /// </summary>
	  public DateTime? PRINT_DATE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 委外商編號F1928
	  /// </summary>
	  public string OUTSOURCE_ID { get; set; }

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
	  /// 結算開始日期
	  /// </summary>
    [Required]
	  public DateTime CNT_DATE_S { get; set; }

	  /// <summary>
	  /// 計費方式明細說明
	  /// </summary>
	  public string PRICE_DETAIL { get; set; }
  }
}
        