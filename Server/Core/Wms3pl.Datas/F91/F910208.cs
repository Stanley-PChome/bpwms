namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 快速加工加工記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F910208")]
  public class F910208 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int32 ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Required]
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 加工動作 F000904 TOPIC=F910208 SUBTOPIC=PROCESS_ACTION
	  /// </summary>
    [Required]
	  public string PROCESS_ACTION { get; set; }

	  /// <summary>
	  /// 成品數
	  /// </summary>
    [Required]
	  public Int32 GOOD_QTY { get; set; }

	  /// <summary>
	  /// 累積成品數
	  /// </summary>
    [Required]
	  public Int32 SUM_GOOD_QTY { get; set; }

	  /// <summary>
	  /// 原料異動數
	  /// </summary>
    [Required]
	  public Int32 ORGINAL_CHANGE_QTY { get; set; }

	  /// <summary>
	  /// 剩餘原料數
	  /// </summary>
    [Required]
	  public Int32 ORGINAL_REMAIN_QTY { get; set; }

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
	  public string ALLOCATION_NO { get; set; }

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
	  /// 建立人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 修改日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 修改人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 修改人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        