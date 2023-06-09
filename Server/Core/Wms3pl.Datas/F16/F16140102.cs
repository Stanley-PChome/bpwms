namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨檢驗刷驗紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_ID")]
  [Table("F16140102")]
  public class F16140102 : IAuditInfo
  {

	  /// <summary>
	  /// 紀錄序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 LOG_ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
    [Required]
	  public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 商品品號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 檢驗數量
	  /// </summary>
	  public Int32? AUDIT_QTY { get; set; }

	  /// <summary>
	  /// 檢驗人員
	  /// </summary>
    [Required]
	  public string AUDIT_STAFF { get; set; }

	  /// <summary>
	  /// 檢驗人名
	  /// </summary>
    [Required]
	  public string AUDIT_NAME { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
	  public string ISPASS { get; set; }

	  /// <summary>
	  /// 訊息
	  /// </summary>
	  public string MESSAGE { get; set; }

	  /// <summary>
	  /// 電腦名稱
	  /// </summary>
	  public string CLIENT_PC { get; set; }

	  /// <summary>
	  /// 錄影機台
	  /// </summary>
	  public string VIDEO_NO { get; set; }

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
        