namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 流通加工揀料單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("PICK_NO","PICK_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F91020501")]
  public class F91020501 : IAuditInfo
  {

	  /// <summary>
	  /// 揀料單號
	  /// </summary>
    [Key]
    [Required]
	  public string PICK_NO { get; set; }

	  /// <summary>
	  /// 揀料序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 PICK_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 揀料儲位
	  /// </summary>
    [Required]
	  public string PICK_LOC { get; set; }

	  /// <summary>
	  /// 揀料數
	  /// </summary>
    [Required]
	  public Int32 PICK_QTY { get; set; }

	  /// <summary>
	  /// 效期
	  /// </summary>
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 廠商編號
	  /// </summary>
    [Required]
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 入庫日
	  /// </summary>
    [Required]
	  public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
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
        