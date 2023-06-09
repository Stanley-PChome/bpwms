namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訊息通知設定
  /// </summary>
  [Serializable]
  [DataServiceKey("GUP_CODE","CUST_CODE","DC_CODE","WORK_NO")]
  [Table("F190003")]
  public class F190003 : IAuditInfo
  {

	  /// <summary>
	  /// 業主代號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主代號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流中心代號
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 工作類別代號(F000904)
	  /// </summary>
    [Key]
    [Required]
	  public string WORK_NO { get; set; }

	  /// <summary>
	  /// 1天通知的工作群組(F1953)
	  /// </summary>
	  public Decimal? GRP_ID_1 { get; set; }

	  /// <summary>
	  /// 7天通知的工作群組(F1953)
	  /// </summary>
	  public Decimal? GRP_ID_7 { get; set; }

	  /// <summary>
	  /// 14天通知的工作群組(F1953)
	  /// </summary>
	  public Decimal? GRP_ID_14 { get; set; }

	  /// <summary>
	  /// 1天使用Email通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_MAIL_1 { get; set; }

	  /// <summary>
	  /// 7天使用Email通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_MAIL_7 { get; set; }

	  /// <summary>
	  /// 14天使用Email通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_MAIL_14 { get; set; }

	  /// <summary>
	  /// 1天使用簡訊通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_SMS_1 { get; set; }

	  /// <summary>
	  /// 7天使用簡訊通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_SMS_7 { get; set; }

	  /// <summary>
	  /// 14天使用簡訊通知，0:否 1是
	  /// </summary>
    [Required]
	  public string IS_SMS_14 { get; set; }

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
  }
}
        