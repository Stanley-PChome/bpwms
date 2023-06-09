namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總批號播種主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_NO","ITEM_CODE","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F051601")]
  public class F051601 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總日期
	  /// </summary>
    [Required]
	  public DateTime BATCH_DATE { get; set; }

	  /// <summary>
	  /// 彙總批號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 狀態(0: 待處理 1:已下傳 2: 2: 已回傳 3: 已結案(播種完成))
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 品名
	  /// </summary>
    [Required]
	  public string ITEM_NAME { get; set; }

	  /// <summary>
	  /// 單位
	  /// </summary>
    [Required]
	  public string ITEM_UNIT { get; set; }

	  /// <summary>
	  /// 總揀貨量
	  /// </summary>
    [Required]
	  public Int64 PACK_QTY { get; set; }

	  /// <summary>
	  /// 分貨總店數
	  /// </summary>
    [Required]
	  public Int64 PACK_LABEL { get; set; }

	  /// <summary>
	  /// 物流中心編號
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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
        