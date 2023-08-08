namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 專車單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","DELIVERY_NO","GUP_CODE","CUST_CODE","WMS_NO")]
  [Table("F700802")]
  public class F700802 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 專車單號
	  /// </summary>
    [Key]
    [Required]
	  public string DELIVERY_NO { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流單號
	  /// </summary>
    [Key]
    [Required]
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 物流單據類別
	  /// </summary>
	  public string ORD_TYPE { get; set; }

	  /// <summary>
	  /// 客戶代號
	  /// </summary>
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 客戶名稱
	  /// </summary>
	  public string CUST_NAME { get; set; }

	  /// <summary>
	  /// 地址
	  /// </summary>
	  public string ADDRESS { get; set; }

	  /// <summary>
	  /// 聯絡人
	  /// </summary>
	  public string CONTACT { get; set; }

	  /// <summary>
	  /// 連絡電話
	  /// </summary>
	  public string CONTACT_TEL { get; set; }

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
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        