namespace Wms3pl.Datas.F01
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("STOCK_NO","STOCK_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F010202")]
  public class F010202 : IAuditInfo
  {

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Key]
    [Required]
	  public string STOCK_NO { get; set; }

	  /// <summary>
	  /// 進倉單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 STOCK_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 進貨數
	  /// </summary>
    [Required]
	  public Int32 STOCK_QTY { get; set; }

	  /// <summary>
	  /// *******待刪除********
	  /// </summary>
    [Required]
	  public Int32 RECV_QTY { get; set; }

	  /// <summary>
	  /// 狀態
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

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
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
	  /// 商品有效日期
	  /// </summary>
	  public DateTime? VALI_DATE { get; set; }

	  /// <summary>
	  /// 原訂購數
	  /// </summary>
	  public Int32? ORDER_QTY { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
	  public string MAKE_NO { get; set; }
  }
}
        