namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// RF盤點效期與批號檔
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","ITEM_CODE","LOC_CODE","ISSECOND","DC_CODE","GUP_CODE","CUST_CODE","INVENTORY_SEQ")]
  [Table("F140113")]
  public class F140113 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
	  public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品名稱
	  /// </summary>
	  public string ITEM_NAME { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 是否為複盤作業
	  /// </summary>
    [Key]
    [Required]
	  public string ISSECOND { get; set; }

	  /// <summary>
	  /// 盤點數
	  /// </summary>
	  public Int32? INVENTORY_QTY { get; set; }

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
	  /// 建立人員/盤點人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名/盤點人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建立日期/開始盤點時間
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
	  /// 盤點狀態(0未更新1已更新)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 有效日期
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
	  public string MAKE_NO { get; set; }

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 INVENTORY_SEQ { get; set; }
  }
}
        