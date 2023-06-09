namespace Wms3pl.Datas.F20
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(訂單調整)刪除託運單備份
  /// </summary>
  [Serializable]
  [DataServiceKey("CONSIGN_ID")]
  [Table("F20010202")]
  public class F20010202 : IAuditInfo
  {

	  /// <summary>
	  /// 託運單 ID
	  /// </summary>
    [Key]
    [Required]
	  public Decimal CONSIGN_ID { get; set; }

	  /// <summary>
	  /// 託運單號
	  /// </summary>
    [Required]
	  public string CONSIGN_NO { get; set; }

	  /// <summary>
	  /// 物流單號
	  /// </summary>
    [Required]
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 路線代碼
	  /// </summary>
	  public string ROUTE_CODE { get; set; }

	  /// <summary>
	  /// 業主代碼
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主代碼
	  /// </summary>
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流中心代碼
	  /// </summary>
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

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
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 新竹貨運到著站編號
	  /// </summary>
	  public string ERST_NO { get; set; }

	  /// <summary>
	  /// 路線(ex.當日配北區or2A)
	  /// </summary>
	  public string ROUTE { get; set; }

	  /// <summary>
	  /// 配次
	  /// </summary>
	  public string DELV_TIMES { get; set; }

	  /// <summary>
	  /// 配送商回覆的配達時間
	  /// </summary>
	  public DateTime? PAST_DATE { get; set; }

	  /// <summary>
	  /// 配送狀態(0未配送,2配送中,3已配達)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// To貨主配送回檔狀態(0未回檔1已回檔)
	  /// </summary>
    [Required]
	  public string CUST_EDI_STATUS { get; set; }

	  /// <summary>
	  /// To配送商回檔狀態(0未回檔1已回檔)
	  /// </summary>
    [Required]
	  public string DISTR_EDI_STATUS { get; set; }

	  /// <summary>
	  /// 配送回覆結果
	  /// </summary>
	  public string RESULT { get; set; }

	  /// <summary>
	  /// To貨主配送回檔次數
	  /// </summary>
    [Required]
	  public Int32 CUST_EDI_QTY { get; set; }

	  /// <summary>
	  /// 配送商回覆的發送(商品)日期
	  /// </summary>
	  public DateTime? SEND_DATE { get; set; }
  }
}
        