namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 門市主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GUP_CODE","RETAIL_CODE","CUST_CODE","CHANNEL")]
  [Table("F1910")]
  public class F1910 : IAuditInfo
  {

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 門市編號
	  /// </summary>
    [Key]
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 門市名稱
	  /// </summary>
    [Required]
	  public string RETAIL_NAME { get; set; }

	  /// <summary>
	  /// 聯絡人
	  /// </summary>
    [Required]
	  public string CONTACT { get; set; }

	  /// <summary>
	  /// 連絡電話
	  /// </summary>
    [Required]
	  public string TEL { get; set; }

	  /// <summary>
	  /// 聯絡信箱
	  /// </summary>
	  public string MAIL { get; set; }

	  /// <summary>
	  /// 聯絡地址
	  /// </summary>
    [Required]
	  public string ADDRESS { get; set; }

	  /// <summary>
	  /// 建立時間
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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 通路
	  /// </summary>
    [Key]
    [Required]
	  public string CHANNEL { get; set; }

	  /// <summary>
	  /// 客戶簡稱
	  /// </summary>
	  public string SHORT_SALESBASE_NAME { get; set; }

	  /// <summary>
	  /// 統一編號
	  /// </summary>
	  public string UNIFIED_BUSINESS_NO { get; set; }

	  /// <summary>
	  /// 銷售據點群組
	  /// </summary>
	  public string SALES_BASE_GROUP { get; set; }

	  /// <summary>
	  /// 聯絡電話2
	  /// </summary>
	  public string TEL2 { get; set; }

	  /// <summary>
	  /// 傳真號碼
	  /// </summary>
	  public string FAX { get; set; }

	  /// <summary>
	  /// 客製允出類型
	  /// </summary>
	  public string CUSTOM_DELVDAYS_TYPE { get; set; }

	  /// <summary>
	  /// 允出天數
	  /// </summary>
	  public Decimal? DELV_DAYS { get; set; }

	  /// <summary>
	  /// 允出限制
	  /// </summary>
	  public string DELV_DAYS_LIMIT { get; set; }

	  /// <summary>
	  /// 允出訊息
	  /// </summary>
	  public string DELV_DAYS_INFO { get; set; }

	  /// <summary>
	  /// 自取
	  /// </summary>
    [Required]
	  public string SELF_TAKE { get; set; }

	  /// <summary>
	  /// 是否貼嘜頭
	  /// </summary>
    [Required]
	  public string NEED_SHIPPING_MARK { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string NOTE { get; set; }

	  /// <summary>
	  /// 客戶車次或路線編號
	  /// </summary>
	  public string DELV_NO { get; set; }
  }
}
        