namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 報價單上傳紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","UPLOAD_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910404")]
  public class F910404 : IAuditInfo
  {

	  /// <summary>
	  /// 報價單項目編號
	  /// </summary>
    [Key]
    [Required]
	  public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 上傳檔案編號
	  /// </summary>
    [Key]
    [Required]
	  public Int16 UPLOAD_NO { get; set; }

	  /// <summary>
	  /// 上傳檔案主機路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_S_PATH { get; set; }

	  /// <summary>
	  /// 上傳檔案原路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_C_PATH { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

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
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }
  }
}
        