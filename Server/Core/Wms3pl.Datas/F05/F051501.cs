namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 批量揀貨主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_PICK_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F051501")]
  public class F051501 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總批號
	  /// </summary>
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 彙總單號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_PICK_NO { get; set; }

	  /// <summary>
	  /// 指定工作站
	  /// </summary>
	  public string STATION_NO { get; set; }

	  /// <summary>
	  /// 單據狀態(0: 待處理 1: 已下傳 2: 揀貨中 3: 揀貨完成  9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 品項數
	  /// </summary>
    [Required]
	  public Int64 ITEM_CNT { get; set; }

	  /// <summary>
	  /// 總數量
	  /// </summary>
    [Required]
	  public Int64 TOTAL_QTY { get; set; }

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
        