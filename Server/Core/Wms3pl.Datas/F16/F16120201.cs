namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨分配實際退貨檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RETURN_NO","RETURN_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F16120201")]
  public class F16120201 : IAuditInfo
  {

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
    [Key]
    [Required]
	  public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 退貨單序號
	  /// </summary>
    [Key]
    [Required]
	  public string RETURN_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 預計退貨數
	  /// </summary>
    [Required]
	  public Int32 RTN_QTY { get; set; }

	  /// <summary>
	  /// 實際退貨數
	  /// </summary>
    [Required]
	  public Int32 A_RTN_QTY { get; set; }

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
        