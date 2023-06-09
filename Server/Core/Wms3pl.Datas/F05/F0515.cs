namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總批號總表
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F0515")]
  public class F0515 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總日期
	  /// </summary>
    [Required]
	  public DateTime BATCH_DATE { get; set; }

	  /// <summary>
	  /// 彙總批號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 揀貨狀態(0: 待處理 1: 已下傳 2: 揀貨完成 9:取消)
	  /// </summary>
    [Required]
	  public string PICK_STATUS { get; set; }

	  /// <summary>
	  /// 播種狀態(0: 待處理 1: 已下傳 2:已回傳 3:播種完成 9:取消)
	  /// </summary>
    [Required]
	  public string PUT_STATUS { get; set; }

	  /// <summary>
	  /// 下傳Caps時間
	  /// </summary>
	  public DateTime? TRANS_DATE { get; set; }

	  /// <summary>
	  /// 下傳Caps人員
	  /// </summary>
	  public string TRANS_STAFF { get; set; }

	  /// <summary>
	  /// 下傳Caps人名
	  /// </summary>
	  public string TRANS_NAME { get; set; }

	  /// <summary>
	  /// Caps回傳時間
	  /// </summary>
	  public DateTime? RECV_DATE { get; set; }

	  /// <summary>
	  /// Caps回傳人員
	  /// </summary>
	  public string RECV_STAFF { get; set; }

	  /// <summary>
	  /// Caps回傳人名
	  /// </summary>
	  public string RECV_NAME { get; set; }

	  /// <summary>
	  /// 結案時間
	  /// </summary>
	  public DateTime? CLOSED_DATE { get; set; }

	  /// <summary>
	  /// 揀貨工具 F000904 TOPIC=F191902 SUBTOPIC=PICK_TOOL
	  /// </summary>
    [Required]
	  public string PICK_TOOL { get; set; }

	  /// <summary>
	  /// 播種工具 F000904 TOPIC=F191902 SUBTOPIC=PUT_TOOL
	  /// </summary>
	  public string PUT_TOOL { get; set; }

	  /// <summary>
	  /// 分配份數
	  /// </summary>
    [Required]
	  public Int16 ALLOT_CNT { get; set; }

	  /// <summary>
	  /// 分配方式
	  /// </summary>
    [Required]
	  public string ALLOT_TYPE { get; set; }

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
	  /// 門市數
	  /// </summary>
    [Required]
	  public Int16 RETAIL_CNT { get; set; }

	  /// <summary>
	  /// 列印狀態(0:未列印 1:已列印)
	  /// </summary>
	  public string PRINT_FLAG { get; set; }

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
        