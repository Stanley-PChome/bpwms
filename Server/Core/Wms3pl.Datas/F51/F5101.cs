namespace Wms3pl.Datas.F51
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心貨主商品日結檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CAL_DATE","ITEM_CODE","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F5101")]
  public class F5101 : IAuditInfo
  {

	  /// <summary>
	  /// 結算日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CAL_DATE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 本日期初數量
	  /// </summary>
    [Required]
	  public Int32 BEGIN_QTY { get; set; }

	  /// <summary>
	  /// 本日進貨數量
	  /// </summary>
    [Required]
	  public Int32 RECV_QTY { get; set; }

	  /// <summary>
	  /// 本日退貨上架數量
	  /// </summary>
    [Required]
	  public Int32 RTN_QTY { get; set; }

	  /// <summary>
	  /// 本日出貨數量
	  /// </summary>
    [Required]
	  public Int32 DELV_QTY { get; set; }

	  /// <summary>
	  /// 本日跨DC調出數量
	  /// </summary>
    [Required]
	  public Int32 SRC_QTY { get; set; }

	  /// <summary>
	  /// 本日跨DC調入數量
	  /// </summary>
    [Required]
	  public Int32 TAR_QTY { get; set; }

	  /// <summary>
	  /// 本日借出外送庫存數
	  /// </summary>
    [Required]
	  public Int32 LEND_QTY { get; set; }

	  /// <summary>
	  /// 本日期末數量
	  /// </summary>
    [Required]
	  public Int32 END_QTY { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

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
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        