namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收上傳檔案類型
  /// </summary>
  [Serializable]
  [DataServiceKey("UPLOAD_TYPE")]
  [Table("F02020106")]
  public class F02020106 : IAuditInfo
  {

	  /// <summary>
	  /// 上傳檔案類型
	  /// </summary>
    [Key]
    [Required]
	  public string UPLOAD_TYPE { get; set; }

	  /// <summary>
	  /// 上傳檔案類型名稱
	  /// </summary>
    [Required]
	  public string UPLOAD_NAME { get; set; }

	  /// <summary>
	  /// 是否必須上傳
	  /// </summary>
	  public string ISREQUIRED { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

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
        