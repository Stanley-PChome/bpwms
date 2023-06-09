namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 車輛種類
  /// </summary>
  [Serializable]
  [DataServiceKey("CAR_KIND_ID")]
  [Table("F194702")]
  public class F194702 : IAuditInfo
  {

	  /// <summary>
	  /// 車輛種類ID
	  /// </summary>
    [Key]
    [Required]
	  public Decimal CAR_KIND_ID { get; set; }

	  /// <summary>
	  /// 車輛種類名稱
	  /// </summary>
    [Required]
	  public string CAR_KIND_NAME { get; set; }

	  /// <summary>
	  /// 車輛大小噸數
	  /// </summary>
	  public string CAR_SIZE { get; set; }

	  /// <summary>
	  /// 溫層(A:常溫 B:低溫)F000904
	  /// </summary>
	  public string TMPR_TYPE { get; set; }

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
        