namespace Wms3pl.Datas.Schedule
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 排程執行結果紀錄明細
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("SCHEDULE_JOB_RESULT_DETAIL")]
  public class SCHEDULE_JOB_RESULT_DETAIL
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

	  /// <summary>
	  /// SCHEDULE_JOB_RESULT：Name
	  /// </summary>
    [Required]
	  public string NAME { get; set; }

	  /// <summary>
	  /// 單號
	  /// </summary>
	  public string ITEMNUMBER { get; set; }

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
	  public string EXTERNORDERKEY { get; set; }

	  /// <summary>
	  /// 傳出時間
	  /// </summary>
	  public DateTime? D_INSERTTIME { get; set; }

	  /// <summary>
	  /// 目前狀態
	  /// </summary>
	  public string NOWFLAG { get; set; }

	  /// <summary>
	  /// 傳送ID(32碼亂數)
	  /// </summary>
	  public string RQ_UUID { get; set; }

	  /// <summary>
	  /// 來源ID(固定寫入A01)
	  /// </summary>
	  public string SOURCE_ID { get; set; }

	  /// <summary>
	  /// 數量
	  /// </summary>
	  public Int32? PIECES { get; set; }

	  /// <summary>
	  /// 傳送日
	  /// </summary>
	  public DateTime? SEND_DATE { get; set; }

	  /// <summary>
	  /// 更新時間
	  /// </summary>
	  public DateTime? UPDATE_DTM { get; set; }

	  /// <summary>
	  /// 更新狀態
	  /// </summary>
	  public string STATUS_CODE { get; set; }

	  /// <summary>
	  /// 回傳訊息
	  /// </summary>
	  public string FEEDBACK_LOG { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心編號
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }
  }
}
        