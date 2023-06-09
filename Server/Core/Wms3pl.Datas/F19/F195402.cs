namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統選單樣式主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("MENU_CODE")]
  [Table("F195402")]
  public class F195402 : IAuditInfo
  {

	  /// <summary>
	  /// 系統選單樣式編號
	  /// </summary>
    [Key]
    [Required]
	  public string MENU_CODE { get; set; }

	  /// <summary>
	  /// 系統選單樣式說明
	  /// </summary>
    [Required]
	  public string MENU_DESC { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        