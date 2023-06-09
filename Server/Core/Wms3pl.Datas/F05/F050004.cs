namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨批次產生參數設定
  /// </summary>
  [Serializable]
  [DataServiceKey("TICKET_ID","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F050004")]
  public class F050004 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類別主檔Id
	  /// </summary>
    [Key]
    [Required]
	  public Decimal TICKET_ID { get; set; }

	  /// <summary>
	  /// 中南部優先處理數量
	  /// </summary>
    [Required]
	  public Int32 SOUTH_PRIORITY_QTY { get; set; }

	  /// <summary>
	  /// 批次訂單上限
	  /// </summary>
    [Required]
	  public Int32 ORDER_LIMIT { get; set; }

	  /// <summary>
	  /// 出貨單處理D+N?天
	  /// </summary>
    [Required]
	  public Int32 DELV_DAY { get; set; }

	  /// <summary>
	  /// 是否依樓層拆單(否0是1)
	  /// </summary>
    [Required]
	  public string SPLIT_FLOOR { get; set; }

	  /// <summary>
	  /// 是否併單(否0是1)
	  /// </summary>
    [Required]
	  public string MERGE_ORDER { get; set; }

	  /// <summary>
	  /// 貨主代號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主代號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心代號
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 訂單品項數警示
	  /// </summary>
	  public Int16? ITEM_LIMIT { get; set; }

	  /// <summary>
	  /// 拆揀貨單方式F000904 TOPIC=F050004 SUBTOPIC=SPLIT_PICK_TYPE
	  /// </summary>
    [Required]
	  public string SPLIT_PICK_TYPE { get; set; }
  }
}
        