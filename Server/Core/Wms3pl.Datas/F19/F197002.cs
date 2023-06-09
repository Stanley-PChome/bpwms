namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 板箱標籤主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("YEAR")]
  [Table("F197002")]
  public class F197002 : IAuditInfo
  {

	  /// <summary>
	  /// 年度
	  /// </summary>
    [Key]
    [Required]
	  public string YEAR { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
	  public string BOX_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
	  public string PALLET_NO { get; set; }

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
        