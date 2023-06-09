namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 貨主發票登錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CUST_CODE","GUP_CODE","INVO_YEAR","INVO_MON","INVO_TITLE","INVO_NO_BEGIN","INVO_NO_END")]
  [Table("F190901")]
  public class F190901 : IAuditInfo
  {

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 發票年份
	  /// </summary>
    [Key]
    [Required]
	  public string INVO_YEAR { get; set; }

	  /// <summary>
	  /// 發票月份 F000904
	  /// </summary>
    [Key]
    [Required]
	  public string INVO_MON { get; set; }

	  /// <summary>
	  /// 發票字軌
	  /// </summary>
    [Key]
    [Required]
	  public string INVO_TITLE { get; set; }

	  /// <summary>
	  /// 發票起始號碼
	  /// </summary>
    [Key]
    [Required]
	  public string INVO_NO_BEGIN { get; set; }

	  /// <summary>
	  /// 發票結束號碼
	  /// </summary>
    [Key]
    [Required]
	  public string INVO_NO_END { get; set; }

	  /// <summary>
	  /// 發票目前已使用號碼
	  /// </summary>
	  public string INVO_NO { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
        