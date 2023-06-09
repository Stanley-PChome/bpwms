namespace Wms3pl.Datas.F20
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(商品庫存調整明細)
  /// </summary>
  [Serializable]
  [DataServiceKey("ADJUST_NO","ADJUST_SEQ","GUP_CODE","CUST_CODE","DC_CODE")]
  [Table("F200103")]
  public class F200103 : IAuditInfo
  {

	  /// <summary>
	  /// 調整單單號
	  /// </summary>
    [Key]
    [Required]
	  public string ADJUST_NO { get; set; }

	  /// <summary>
	  /// 調整單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 ADJUST_SEQ { get; set; }

	  /// <summary>
	  /// 作業類別(0調入1調出)
	  /// </summary>
    [Required]
	  public string WORK_TYPE { get; set; }

	  /// <summary>
	  /// 倉別編號(F1980)
	  /// </summary>
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 廠商編號
	  /// </summary>
    [Required]
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 有效日期
	  /// </summary>
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日期
	  /// </summary>
    [Required]
	  public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 調整數
	  /// </summary>
    [Required]
	  public Int32 ADJ_QTY { get; set; }

	  /// <summary>
	  /// 調整原因
	  /// </summary>
    [Required]
	  public string CAUSE { get; set; }

	  /// <summary>
	  /// 調整原因備註
	  /// </summary>
	  public string CAUSE_MEMO { get; set; }

	  /// <summary>
	  /// 處理狀態(0:已處理)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

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
        