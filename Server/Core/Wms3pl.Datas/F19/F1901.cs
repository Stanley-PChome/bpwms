namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE")]
  [Table("F1901")]
  public class F1901 : IAuditInfo
  {

	  /// <summary>
	  /// 編號
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 名稱
	  /// </summary>
    [Required]
	  public string DC_NAME { get; set; }

	  /// <summary>
	  /// 電話
	  /// </summary>
	  public string TEL { get; set; }

	  /// <summary>
	  /// 傳真
	  /// </summary>
	  public string FAX { get; set; }

	  /// <summary>
	  /// 地址
	  /// </summary>
	  public string ADDRESS { get; set; }

	  /// <summary>
	  /// 地坪
	  /// </summary>
	  public Int32? LAND_AREA { get; set; }

	  /// <summary>
	  /// 建坪
	  /// </summary>
	  public Int32? BUILD_AREA { get; set; }

	  /// <summary>
	  /// 簡碼
	  /// </summary>
	  public string SHORT_CODE { get; set; }

	  /// <summary>
	  /// 負責人
	  /// </summary>
	  public string BOSS { get; set; }

	  /// <summary>
	  /// 電子郵件
	  /// </summary>
	  public string MAIL_BOX { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

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
	  /// 郵遞區號
	  /// </summary>
    [Required]
	  public string ZIP_CODE { get; set; }
	}
}
        