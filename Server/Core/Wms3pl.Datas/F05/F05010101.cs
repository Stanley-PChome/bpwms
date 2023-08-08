namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 原始出貨訂單小單關聯檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_NO","SMALL_ORD_SEQ","GUP_CODE","CUST_CODE","DC_CODE")]
  [Table("F05010101")]
  public class F05010101 : IAuditInfo
  {

	  /// <summary>
	  /// 大單訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 小單訂單編號
	  /// </summary>
	  public string SMALL_ORD_NO { get; set; }

	  /// <summary>
	  /// 小單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 SMALL_ORD_SEQ { get; set; }

	  /// <summary>
	  /// 收件人地址
	  /// </summary>
	  public string ADDRESS { get; set; }

	  /// <summary>
	  /// 收件人連絡電話
	  /// </summary>
	  public string TEL { get; set; }

	  /// <summary>
	  /// 收件人
	  /// </summary>
    [Required]
	  public string CONSIGNEE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 訂購數量
	  /// </summary>
    [Required]
	  public Int32 ORD_QTY { get; set; }

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
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        