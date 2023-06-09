namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨單身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RETURN_NO","RETURN_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161202")]
  public class F161202 : IAuditInfo
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
	  /// 退貨數量
	  /// </summary>
    [Required]
	  public Int32 RTN_QTY { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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

	  /// <summary>
	  /// 回傳貨主FLAG('0' = 未回傳,'1' = 已回傳)
	  /// </summary>
    [Required]
	  public string RTN_CUS_FLAG { get; set; }
  }
}
        