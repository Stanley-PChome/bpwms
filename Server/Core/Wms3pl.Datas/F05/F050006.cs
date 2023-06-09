namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 郵遞區號與DC對應
  /// </summary>
  [Serializable]
  [DataServiceKey("ZIP_CODE","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F050006")]
  public class F050006 : IAuditInfo
  {

	  /// <summary>
	  /// 郵遞區號
	  /// </summary>
    [Key]
    [Required]
	  public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心代號
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
        