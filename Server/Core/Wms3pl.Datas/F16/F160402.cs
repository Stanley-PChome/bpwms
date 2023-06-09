namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 報廢單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("SCRAP_NO","SCRAP_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F160402")]
  public class F160402 : IAuditInfo
  {

	  /// <summary>
	  /// 報廢單號
	  /// </summary>
    [Key]
    [Required]
	  public string SCRAP_NO { get; set; }

	  /// <summary>
	  /// 報廢單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 SCRAP_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 報廢數量
	  /// </summary>
    [Required]
	  public Int32 SCRAP_QTY { get; set; }

	  /// <summary>
	  /// 商品效期
	  /// </summary>
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 來源倉別F1980
	  /// </summary>
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 來源儲位
	  /// </summary>
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 報廢原因F1951 WhereUctId=SC
	  /// </summary>
    [Required]
	  public string SCRAP_CAUSE { get; set; }

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
        