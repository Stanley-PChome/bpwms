namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥(退貨上架)申請單身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_APPLY_NO","RTN_APPLY_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161602")]
  public class F161602 : IAuditInfo
  {

	  /// <summary>
	  /// 調撥申請單號
	  /// </summary>
    [Key]
    [Required]
	  public string RTN_APPLY_NO { get; set; }

	  /// <summary>
	  /// 調撥申請單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 RTN_APPLY_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 來源儲位
	  /// </summary>
    [Required]
	  public string SRC_LOC { get; set; }

	  /// <summary>
	  /// 建議上架儲位
	  /// </summary>
	  public string TRA_LOC { get; set; }

	  /// <summary>
	  /// 庫存數
	  /// </summary>
    [Required]
	  public Int32 LOC_QTY { get; set; }

	  /// <summary>
	  /// 目的倉別
	  /// </summary>
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 上架數
	  /// </summary>
    [Required]
	  public Int32 MOVED_QTY { get; set; }

	  /// <summary>
	  /// 退貨過帳數
	  /// </summary>
	  public Int32? POST_QTY { get; set; }

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
	  /// 廠商編號
	  /// </summary>
    [Required]
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 效期欄位
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Required]
	  public string MAKE_NO { get; set; }

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
	  /// 目的箱號
	  /// </summary>
    [Required]
	  public string TAR_BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 目的板號
	  /// </summary>
    [Required]
	  public string TAR_PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 目的批號
	  /// </summary>
    [Required]
	  public string TAR_MAKE_NO { get; set; }

	  /// <summary>
	  /// 目的效期
	  /// </summary>
	  public DateTime? TAR_VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日
	  /// </summary>
	  public DateTime? ENTER_DATE { get; set; }
  }
}
        