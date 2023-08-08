namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 上架回倉調撥單關聯檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PROCESS_NO","BACK_NO","SERIAL_NO","ITEM_CODE","ALLOCATION_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F91020601")]
  public class F91020601 : IAuditInfo
  {

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Key]
    [Required]
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 回倉號碼
	  /// </summary>
    [Key]
    [Required]
	  public Int64 BACK_NO { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
    [Key]
    [Required]
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 回倉數量
	  /// </summary>
    [Required]
	  public Int32 QTY { get; set; }

	  /// <summary>
	  /// 回倉商品類型(0成品1揀料)
	  /// </summary>
    [Required]
	  public string BACK_ITEM_TYPE { get; set; }

	  /// <summary>
	  /// 調撥單號(F151001)
	  /// </summary>
    [Key]
    [Required]
	  public string ALLOCATION_NO { get; set; }

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
	  /// 是否為良品(1:良品，0:報廢)
	  /// </summary>
    [Required]
	  public string IS_GOOD { get; set; }
  }
}
        