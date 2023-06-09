namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 商品棧板疊法設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GUP_CODE","CUST_CODE","ITEM_CODE")]
  [Table("F190305")]
  public class F190305 : IAuditInfo
  {

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
	  /// 品號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 棧板每層箱數
	  /// </summary>
    [Required]
	  public Int32 PALLET_LEVEL_CASEQTY { get; set; }

	  /// <summary>
	  /// 棧板層數
	  /// </summary>
    [Required]
	  public Int32 PALLET_LEVEL_CNT { get; set; }

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
        