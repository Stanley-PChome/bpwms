namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 禾頡通路別代碼表
  /// </summary>
  [Serializable]
  [DataServiceKey("CHANNEL")]
  [Table("F194706")]
  public class F194706 : IAuditInfo
  {

	  /// <summary>
	  /// 通路別代碼
	  /// </summary>
    [Key]
    [Required]
	  public string CHANNEL { get; set; }

	  /// <summary>
	  /// 地址條印製類別
	  /// </summary>
    [Required]
	  public string ADDRESS_TYPE { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        