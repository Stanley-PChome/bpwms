namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訂單作業統計
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","ORD_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F700709")]
  public class F700709 : IAuditInfo
  {

	  /// <summary>
	  /// 統計日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CNT_DATE { get; set; }

	  /// <summary>
	  /// 統計星期(0~6=>日~一)
	  /// </summary>
    [Required]
	  public string CNT_DAY { get; set; }

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 總揀貨時間
	  /// </summary>
    [Required]
	  public Int32 TOTAL_PICK_TIME { get; set; }

	  /// <summary>
	  /// 物流中心
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
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
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
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        