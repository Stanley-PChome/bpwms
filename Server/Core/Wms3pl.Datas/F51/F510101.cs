namespace Wms3pl.Datas.F51
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心貨主商品庫存作業紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CAL_DATE","LOG_SEQ","WMS_NO","ITEM_CODE","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F510101")]
  public class F510101 : IAuditInfo
  {

	  /// <summary>
	  /// 結算日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CAL_DATE { get; set; }

	  /// <summary>
	  /// 流水序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 系統單號
	  /// </summary>
    [Key]
    [Required]
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 過帳日期
	  /// </summary>
    [Required]
	  public DateTime POSTING_DATE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 結算數
	  /// </summary>
    [Required]
	  public Int32 QTY { get; set; }

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
        