namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲位料架主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOC_TYPE_ID")]
  [Table("F1942")]
  public class F1942 : IAuditInfo
  {

	  /// <summary>
	  /// 儲位料架編號
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_TYPE_ID { get; set; }

	  /// <summary>
	  /// 料架名稱
	  /// </summary>
    [Required]
	  public string LOC_TYPE_NAME { get; set; }

	  /// <summary>
	  /// 儲位長度
	  /// </summary>
    [Required]
	  public Int32 LENGTH { get; set; }

	  /// <summary>
	  /// 儲位深度
	  /// </summary>
    [Required]
	  public Int32 DEPTH { get; set; }

	  /// <summary>
	  /// 儲位高度
	  /// </summary>
    [Required]
	  public Int32 HEIGHT { get; set; }

	  /// <summary>
	  /// 儲位負荷重量
	  /// </summary>
    [Required]
	  public Single WEIGHT { get; set; }

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
	  /// 容積率(%)
	  /// </summary>
    [Required]
	  public Decimal VOLUME_RATE { get; set; }

	  /// <summary>
	  /// 便利性(1:最低 2:適中 3:最高)
	  /// </summary>
    [Required]
	  public string HANDY { get; set; }

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
        