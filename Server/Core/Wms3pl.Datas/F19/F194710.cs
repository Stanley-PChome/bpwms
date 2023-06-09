namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配送商客代託單號對應檔 (物流中心客代)
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F194710")]
  public class F194710 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal ID { get; set; }

	  /// <summary>
	  /// 配送商編號(F1947)
	  /// </summary>
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 客代設定編號(F194710)
	  /// </summary>
    [Required]
	  public string LOG_ID { get; set; }

	  /// <summary>
	  /// 客代代碼
	  /// </summary>
    [Required]
	  public string LOGCENTER_ID { get; set; }

	  /// <summary>
	  /// 託單號(起)
	  /// </summary>
    [Required]
	  public string START_NUMBER { get; set; }

	  /// <summary>
	  /// 託單號(迄)
	  /// </summary>
    [Required]
	  public string END_NUMBER { get; set; }

	  /// <summary>
	  /// 目前使用到的託單號
	  /// </summary>
    [Required]
	  public string NOW_NUMBER { get; set; }

	  /// <summary>
	  /// 狀態(0:使用中，9:不使用)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 是否測試(0:否 1:是)
	  /// </summary>
    [Required]
	  public string ISTEST { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
	  public string CUST_CODE { get; set; }
  }
}
        