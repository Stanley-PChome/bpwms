namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 廠退類型設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_VNR_TYPE_ID")]
  [Table("F160203")]
  public class F160203 : IAuditInfo
  {

	  /// <summary>
	  /// 廠退類型編號
	  /// </summary>
    [Key]
    [Required]
	  public string RTN_VNR_TYPE_ID { get; set; }

	  /// <summary>
	  /// 廠退類型名稱
	  /// </summary>
    [Required]
	  public string RTN_VNR_TYPE_NAME { get; set; }

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
        