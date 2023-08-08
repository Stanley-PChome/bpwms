namespace Wms3pl.Datas.F51
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據材積效期明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","WMS_NO","ITEM_CODE","VALID_DATE")]
  [Table("F511002")]
  public class F511002 : IAuditInfo
  {

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
	  /// 單據編號
	  /// </summary>
    [Key]
    [Required]
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 效期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 數量
	  /// </summary>
    [Required]
	  public Int64 QTY { get; set; }

	  /// <summary>
	  /// 整數箱
	  /// </summary>
    [Required]
	  public Int64 FULL_BOX_QTY { get; set; }

	  /// <summary>
	  /// 零數箱
	  /// </summary>
    [Required]
	  public Int64 BULK_BOX_QTY { get; set; }

	  /// <summary>
	  /// 重量
	  /// </summary>
    [Required]
	  public decimal WEIGHT { get; set; }

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
        