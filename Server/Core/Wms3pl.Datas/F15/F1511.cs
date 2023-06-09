namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 虛擬儲位檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORDER_NO","ORDER_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F1511")]
  public class F1511 : IAuditInfo
  {

	  /// <summary>
	  /// 單據號碼
	  /// </summary>
    [Key]
    [Required]
	  public string ORDER_NO { get; set; }

	  /// <summary>
	  /// 序號
	  /// </summary>
    [Key]
    [Required]
	  public string ORDER_SEQ { get; set; }

	  /// <summary>
	  /// 狀態(0:已配庫,1:已搬動 2:已扣帳  9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 預計揀貨數量
	  /// </summary>
    [Required]
	  public Int32 B_PICK_QTY { get; set; }

	  /// <summary>
	  /// 實際揀貨數量
	  /// </summary>
    [Required]
	  public Int32 A_PICK_QTY { get; set; }

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

	  /// <summary>
	  /// 商品編號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 有效期限
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日
	  /// </summary>
	  public DateTime? ENTER_DATE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 揀貨儲位
	  /// </summary>
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Required]
	  public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
    [Required]
	  public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Required]
	  public string MAKE_NO { get; set; }
  }
}
        