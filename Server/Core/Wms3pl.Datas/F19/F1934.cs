namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 鄉鎮行政區域檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ZIP_CODE")]
  [Table("F1934")]
  public class F1934 : IAuditInfo
  {

	  /// <summary>
	  /// 鄉鎮區域編號(郵遞區號)
	  /// </summary>
    [Key]
    [Required]
	  public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 鄉鎮區域名稱
	  /// </summary>
    [Required]
	  public string ZIP_NAME { get; set; }

	  /// <summary>
	  /// 縣市行政區編號F1933
	  /// </summary>
    [Required]
	  public string COUDIV_ID { get; set; }

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
        