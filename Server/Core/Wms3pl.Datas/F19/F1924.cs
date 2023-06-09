namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 人員主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("EMP_ID")]
  [Table("F1924")]
  public class F1924 : IAuditInfo
  {

	  /// <summary>
	  /// 員工編號
	  /// </summary>
    [Key]
    [Required]
	  public string EMP_ID { get; set; }

	  /// <summary>
	  /// 姓名
	  /// </summary>
    [Required]
	  public string EMP_NAME { get; set; }

	  /// <summary>
	  /// 電子郵件
	  /// </summary>
	  public string EMAIL { get; set; }

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
	  /// 註記是否刪除 1:刪除
	  /// </summary>
	  public string ISDELETED { get; set; }

	  /// <summary>
	  /// 包裝解鎖權限
	  /// </summary>
	  public string PACKAGE_UNLOCK { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 是否為共用帳號
	  /// </summary>
    [Required]
	  public string ISCOMMON { get; set; }

	  /// <summary>
	  /// 單位編號(F1925)
	  /// </summary>
    [Required]
	  public string DEP_ID { get; set; }

	  /// <summary>
	  /// 手機號碼
	  /// </summary>
	  public string MOBILE { get; set; }

	  /// <summary>
	  /// 手機簡碼
	  /// </summary>
	  public string SHORT_MOBILE { get; set; }

	  /// <summary>
	  /// 分機
	  /// </summary>
	  public string TEL_EXTENSION { get; set; }

	  /// <summary>
	  /// 擴增預留欄位A
	  /// </summary>
	  public string EXTENSION_A { get; set; }

	  /// <summary>
	  /// 擴增預留欄位B
	  /// </summary>
	  public string EXTENSION_B { get; set; }

	  /// <summary>
	  /// 擴增預留欄位C
	  /// </summary>
	  public string EXTENSION_C { get; set; }

	  /// <summary>
	  /// 擴增預留欄位D
	  /// </summary>
	  public string EXTENSION_D { get; set; }

	  /// <summary>
	  /// 擴增預留欄位E
	  /// </summary>
	  public string EXTENSION_E { get; set; }

	  /// <summary>
	  /// 0: 系統預設功能表分類及排序。1: 使用者自訂功能表分類及排序
	  /// </summary>
    [Required]
	  public string MENUSTYLE { get; set; }

	  /// <summary>
	  /// 系統選單樣式編號F195402
	  /// </summary>
	  public string MENU_CODE { get; set; }
    }
}
        