namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 專車單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","DELIVERY_NO")]
  [Table("F700801")]
  public class F700801 : IAuditInfo
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
	  /// 預計出車日期
	  /// </summary>
	  public DateTime? TAKE_DATE { get; set; }

	  /// <summary>
	  /// 車行編號
	  /// </summary>
    [Required]
	  public string CARRIAGE_ID { get; set; }

	  /// <summary>
	  /// 車行名稱
	  /// </summary>
	  public string CARRIAGE_NAME { get; set; }

	  /// <summary>
	  /// 車輛種類編號(F194702)
	  /// </summary>
    [Required]
	  public Decimal CAR_KIND_ID { get; set; }

	  /// <summary>
	  /// 費用歸屬貨主
	  /// </summary>
	  public string CHARGE_CUST { get; set; }

	  /// <summary>
	  /// 費用歸屬物流中心
	  /// </summary>
	  public string CHARGE_DC { get; set; }

	  /// <summary>
	  /// 費用歸屬貨主時,歸屬的貨主
	  /// </summary>
	  public string CHARGE_CUST_CODE { get; set; }

	  /// <summary>
	  /// 費用
	  /// </summary>
	  public decimal? FEE { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 單據狀態(0: 待處理、1: 結案、9: 取消)
	  /// </summary>
	  public string STATUS { get; set; }

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
        