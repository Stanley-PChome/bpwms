namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 程式下拉選單參數設定
  /// </summary>
  [Serializable]
  [DataServiceKey("TOPIC","SUBTOPIC","VALUE")]
  [Table("F000904")]
  public class F000904 : IAuditInfo
  {

	  /// <summary>
	  /// 程式編號(資料表)
	  /// </summary>
    [Key]
    [Required]
	  public string TOPIC { get; set; }

	  /// <summary>
	  /// 選單ID
	  /// </summary>
    [Key]
    [Required]
	  public string SUBTOPIC { get; set; }

	  /// <summary>
	  /// 選單名稱
	  /// </summary>
    [Required]
	  public string SUB_NAME { get; set; }

	  /// <summary>
	  /// 參數值
	  /// </summary>
    [Key]
    [Required]
	  public string VALUE { get; set; }

	  /// <summary>
	  /// 參數名稱
	  /// </summary>
    [Required]
	  public string NAME { get; set; }

	  /// <summary>
	  /// 是否使用(0否1是)
	  /// </summary>
    [Required]
	  public string ISUSAGE { get; set; }

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
        