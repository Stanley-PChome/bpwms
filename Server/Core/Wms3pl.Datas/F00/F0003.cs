namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("AP_NAME","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F0003")]
  public class F0003 : IAuditInfo
  {

	  /// <summary>
	  /// 設定名稱
	  /// </summary>
    [Key]
    [Required]
	  public string AP_NAME { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 設定值
	  /// </summary>
	  public string SYS_PATH { get; set; }

	  /// <summary>
	  /// 設定檔名
	  /// </summary>
	  public string FILENAME { get; set; }

	  /// <summary>
	  /// 設定類型
	  /// </summary>
	  public string FILETYPE { get; set; }

	  /// <summary>
	  /// 設定描述
	  /// </summary>
	  public string DESCRIPT { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

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
	  /// 建立時間
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }
  }
}
        