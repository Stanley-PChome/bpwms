namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 常用配送客戶檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CUST_NAME")]
  [Table("F70010101")]
  public class F70010101 : IAuditInfo
  {

	  /// <summary>
	  /// 客戶名稱
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_NAME { get; set; }

	  /// <summary>
	  /// 客戶電話
	  /// </summary>
    [Required]
	  public string CUST_TEL { get; set; }

	  /// <summary>
	  /// 郵遞區號
	  /// </summary>
    [Required]
	  public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 地址
	  /// </summary>
    [Required]
	  public string ADDRESS { get; set; }

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
	  /// 建議人名
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
  }
}
        