namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 車次門市明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","DELV_NO","RETAIL_CODE")]
  [Table("F19471601")]
  public class F19471601 : IAuditInfo
  {

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
	  /// 車次/路線
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_NO { get; set; }

	  /// <summary>
	  /// 門市編號
	  /// </summary>
    [Key]
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 到貨時間(起)
	  /// </summary>
    [Required]
	  public string ARRIVAL_TIME_S { get; set; }

	  /// <summary>
	  /// 到貨時間(訖)
	  /// </summary>
    [Required]
	  public string ARRIVAL_TIME_E { get; set; }

	  /// <summary>
	  /// 路順
	  /// </summary>
    [Required]
	  public string DELV_WAY { get; set; }

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
	  /// 建立時間
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
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        