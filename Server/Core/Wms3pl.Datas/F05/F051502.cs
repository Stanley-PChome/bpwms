namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 批量揀貨明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_PICK_NO","BATCH_PICK_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F051502")]
  public class F051502 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總單號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_PICK_NO { get; set; }

	  /// <summary>
	  /// 彙總單序號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal BATCH_PICK_SEQ { get; set; }

	  /// <summary>
	  /// 貨架編號
	  /// </summary>
	  public string SHELF_NO { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 預計揀貨數量
	  /// </summary>
    [Required]
	  public Int64 B_PICK_QTY { get; set; }

	  /// <summary>
	  /// 實際揀貨數量
	  /// </summary>
    [Required]
	  public Int64 A_PICK_QTY { get; set; }

	  /// <summary>
	  /// 揀貨批號狀態(0: 待處理 1: 呼叫貨架中 2: 揀貨完成 9: 取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 物流中心編號
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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        