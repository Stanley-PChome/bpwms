namespace Wms3pl.Datas.F51
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心加工費用結算檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CAL_DATE","SEQ_NO","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F5106")]
  public class F5106 
  {

	  /// <summary>
	  /// 結算日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CAL_DATE { get; set; }

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 SEQ_NO { get; set; }

	  /// <summary>
	  /// 加工單號
	  /// </summary>
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品組合編號
	  /// </summary>
	  public string ITEM_CODE_BOM { get; set; }

	  /// <summary>
	  /// 加工動作編號
	  /// </summary>
	  public string PROCESS_ID { get; set; }

	  /// <summary>
	  /// 加工數
	  /// </summary>
    [Required]
	  public Int32 QTY { get; set; }

	  /// <summary>
	  /// 處理費
	  /// </summary>
    [Required]
	  public decimal AMT { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
	  public DateTime? CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
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
	  /// 異動人員
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 合約編號
	  /// </summary>
	  public string CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 報價單號
	  /// </summary>
	  public string QUOTE_NO { get; set; }
  }
}
        