namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配庫試算頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","CAL_NO")]
  [Table("F05080503")]
  public class F05080503 : IAuditInfo
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
	  /// 試算編號
	  /// </summary>
    [Key]
    [Required]
	  public string CAL_NO { get; set; }

	  /// <summary>
	  /// 預計試算總訂單數
	  /// </summary>
    [Required]
	  public Int32 TTL_B_ORD_CNT { get; set; }

	  /// <summary>
	  /// 實際配庫總訂單數
	  /// </summary>
    [Required]
	  public Int32 TTL_A_ORD_CNT { get; set; }

	  /// <summary>
	  /// 預計試算總門店數
	  /// </summary>
    [Required]
	  public Int32 TTL_B_RETAIL_CNT { get; set; }

	  /// <summary>
	  /// 實際配庫總門店數
	  /// </summary>
    [Required]
	  public Int32 TTL_A_RETAIL_CNT { get; set; }

	  /// <summary>
	  /// 預計試算總品項數
	  /// </summary>
    [Required]
	  public Int32 TTL_B_ITEM_CNT { get; set; }

	  /// <summary>
	  /// 實際配庫總品項數
	  /// </summary>
    [Required]
	  public Int32 TTL_A_ITEM_CNT { get; set; }

	  /// <summary>
	  /// 預計試算總出貨數
	  /// </summary>
    [Required]
	  public Int32 TTL_B_DELV_QTY { get; set; }

	  /// <summary>
	  /// 實際配庫總出貨數
	  /// </summary>
    [Required]
	  public Int32 TTL_A_DELV_QTY { get; set; }

	  /// <summary>
	  /// 實際配庫總貨架數
	  /// </summary>
    [Required]
	  public Int32 TTL_A_SHELF_CNT { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        