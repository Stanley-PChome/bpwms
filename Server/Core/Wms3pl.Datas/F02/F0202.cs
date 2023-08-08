namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉驗收頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORDER_NO","GUP_CODE","CUST_CODE","DC_CODE")]
  [Table("F0202")]
  public class F0202 : IAuditInfo
  {

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Key]
    [Required]
	  public string ORDER_NO { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 廠商編號
	  /// </summary>
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 報到日期(年月日時分秒)
	  /// </summary>
	  public DateTime? CHECKIN_DATE { get; set; }

	  /// <summary>
	  /// 總重量
	  /// </summary>
	  public decimal? ORDER_WEIGHT { get; set; }

	  /// <summary>
	  /// 總箱數
	  /// </summary>
	  public Int32? BOX_QTY { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 棧板數
	  /// </summary>
	  public Int32? PALLET_QTY { get; set; }
  }
}
        