namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單備份庫存檔
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","LOC_CODE","ITEM_CODE","VALID_DATE","ENTER_DATE","SERIAL_NO","DC_CODE","GUP_CODE","CUST_CODE","BOX_CTRL_NO","PALLET_CTRL_NO","MAKE_NO")]
  [Table("F14010101")]
  public class F14010101 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
	  public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 庫存數
	  /// </summary>
    [Required]
	  public Int64 QTY { get; set; }

	  /// <summary>
	  /// 有效日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日期(YYYY/MM/DD)
	  /// </summary>
    [Key]
    [Required]
	  public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 序號綁儲位商品序號
	  /// </summary>
    [Key]
    [Required]
	  public string SERIAL_NO { get; set; }

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

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Key]
    [Required]
	  public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
    [Key]
    [Required]
	  public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Key]
    [Required]
	  public string MAKE_NO { get; set; }
  }
}
        