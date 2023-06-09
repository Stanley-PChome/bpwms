namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總調撥主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_ALLOC_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F151201")]
  public class F151201 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_ALLOC_NO { get; set; }

	  /// <summary>
	  /// 彙總日期
	  /// </summary>
    [Required]
	  public DateTime BATCH_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0: 待處理  1: 作業處理中 2: 作業已完成  9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 指定工作站
	  /// </summary>
    [Required]
	  public string STATION_NO { get; set; }

	  /// <summary>
	  /// 貨架數
	  /// </summary>
    [Required]
	  public Decimal SHELF_CNT { get; set; }

	  /// <summary>
	  /// 品項數
	  /// </summary>
    [Required]
	  public Decimal ITEM_CNT { get; set; }

	  /// <summary>
	  /// 總數量
	  /// </summary>
    [Required]
	  public Decimal TOTAL_QTY { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主(0:共用)
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主(0:共用)
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
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        