namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配庫試算訂單明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","CAL_NO","ORD_NO","ORD_SEQ")]
  [Table("F05080502")]
  public class F05080502 : IAuditInfo
  {

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
	  /// 試算編號
	  /// </summary>
    [Key]
    [Required]
	  public string CAL_NO { get; set; }

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 訂單序號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_SEQ { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 訂購數量
	  /// </summary>
	  public Int32? ORD_QTY { get; set; }

	  /// <summary>
	  /// 庫分數量
	  /// </summary>
	  public Int32? ALLOT_QTY { get; set; }

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
        