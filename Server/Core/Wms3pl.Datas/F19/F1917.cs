namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 商品小分類主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CCODE","BCODE","ACODE","GUP_CODE","CUST_CODE")]
  [Table("F1917")]
  public class F1917 : IAuditInfo
  {

	  /// <summary>
	  /// 小分類編號
	  /// </summary>
    [Key]
    [Required]
	  public string CCODE { get; set; }

	  /// <summary>
	  /// 所屬中分類編號
	  /// </summary>
    [Key]
    [Required]
	  public string BCODE { get; set; }

	  /// <summary>
	  /// 分類名稱
	  /// </summary>
	  public string CLA_NAME { get; set; }

	  /// <summary>
	  /// 所屬大分類編號
	  /// </summary>
    [Key]
    [Required]
	  public string ACODE { get; set; }

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
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 抽驗比例
	  /// </summary>
	  public decimal? CHECK_PERCENT { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }
  }
}
        