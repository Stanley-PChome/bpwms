namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 業主FTP設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("FTP_URL","FTP_USER","FTP_TYPE","GUP_CODE","CUST_CODE")]
  [Table("F190909")]
  public class F190909 : IAuditInfo
  {

	  /// <summary>
	  /// FTP URL
	  /// </summary>
    [Key]
    [Required]
	  public string FTP_URL { get; set; }

	  /// <summary>
	  /// 登入帳號
	  /// </summary>
    [Key]
    [Required]
	  public string FTP_USER { get; set; }

	  /// <summary>
	  /// 登入密碼
	  /// </summary>
    [Required]
	  public string FTP_PASSWORD { get; set; }

	  /// <summary>
	  /// 類型(001:門市出貨單)
	  /// </summary>
    [Key]
    [Required]
	  public string FTP_TYPE { get; set; }
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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
  }
}
        