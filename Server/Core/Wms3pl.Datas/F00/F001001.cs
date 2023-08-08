namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 求救作業類別對照表(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("HELP_TYPE")]
  [Table("F001001")]
  public class F001001 : IAuditInfo
  {

	  /// <summary>
	  /// 求救類別
	  /// </summary>
    [Key]
    [Required]
	  public string HELP_TYPE { get; set; }

	  /// <summary>
	  /// 求救作業類別名稱
	  /// </summary>
    [Required]
	  public string HELP_NAME { get; set; }

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
	  /// 建檔人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        