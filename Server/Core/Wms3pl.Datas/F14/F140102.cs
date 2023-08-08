namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單設定倉別
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","WAREHOUSE_ID","DC_CODE","GUP_CODE","CUST_CODE","AREA_CODE")]
  [Table("F140102")]
  public class F140102 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
	  public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 倉別編號(F1980)
	  /// </summary>
    [Key]
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 通道起
	  /// </summary>
	  public string CHANNEL_BEGIN { get; set; }

	  /// <summary>
	  /// 通道迄
	  /// </summary>
	  public string CHANNEL_END { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 樓層起
	  /// </summary>
	  public string FLOOR_BEGIN { get; set; }

	  /// <summary>
	  /// 樓層迄
	  /// </summary>
	  public string FLOOR_END { get; set; }

	  /// <summary>
	  /// 座起
	  /// </summary>
	  public string PLAIN_BEGIN { get; set; }

	  /// <summary>
	  /// 座迄
	  /// </summary>
	  public string PLAIN_END { get; set; }

	  /// <summary>
	  /// 儲區
	  /// </summary>
    [Key]
    [Required]
	  public string AREA_CODE { get; set; }
  }
}
        