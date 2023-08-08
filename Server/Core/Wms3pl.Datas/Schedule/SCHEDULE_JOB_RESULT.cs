namespace Wms3pl.Datas.Schedule
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 排程執行結果紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("SCHEDULE_JOB_RESULT")]
  public class SCHEDULE_JOB_RESULT 
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

	  /// <summary>
	  /// 排程名稱
	  /// </summary>
	  public string NAME { get; set; }

	  /// <summary>
	  /// 是否成功
	  /// </summary>
	  public Int16? IS_SUCCESSFUL { get; set; }

	  /// <summary>
	  /// 上一動作的流水號
	  /// </summary>
	  public Int64? PARENT_ID { get; set; }

	  /// <summary>
	  /// 執行時間
	  /// </summary>
	  public DateTime? EXEDATE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 訊息
	  /// </summary>
	  public string MESSAGE { get; set; }

	  /// <summary>
	  /// 執行日期參數
	  /// </summary>
	  public DateTime? SELECT_DATE { get; set; }
  }
}
        