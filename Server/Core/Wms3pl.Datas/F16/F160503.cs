namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 銷毀單上傳影像紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("UPLOAD_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F160503")]
  public class F160503 : IAuditInfo
  {

	  /// <summary>
	  /// 上傳檔案序號
	  /// </summary>
    [Key]
    [Required]
	  public string UPLOAD_SEQ { get; set; }

	  /// <summary>
	  /// 上傳檔案目的路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_S_PATH { get; set; }

	  /// <summary>
	  /// 上傳檔案原路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_C_PATH { get; set; }

	  /// <summary>
	  /// 上傳檔案說明
	  /// </summary>
    [Required]
	  public string UPLOAD_DESC { get; set; }
    [Key]
    [Required]
	  public string DC_CODE { get; set; }
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
        