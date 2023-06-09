namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨車次路線檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","WMS_ORD_NO")]
  [Table("F050803")]
  public class F050803 : IAuditInfo
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
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
	  public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 門市編號(F1910)
	  /// </summary>
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 批次日期
	  /// </summary>
    [Required]
	  public DateTime DELV_DATE { get; set; }

	  /// <summary>
	  /// 指定到貨日期
	  /// </summary>
	  public DateTime? ARRIVAL_DATE { get; set; }

	  /// <summary>
	  /// 車次/路線
	  /// </summary>
    [Required]
	  public string DELV_NO { get; set; }

	  /// <summary>
	  /// 出車時段(F00904，0早、1午、2晚)
	  /// </summary>
    [Required]
	  public string CAR_PERIOD { get; set; }

	  /// <summary>
	  /// 車行
	  /// </summary>
    [Required]
	  public string CAR_GUP { get; set; }

	  /// <summary>
	  /// 司機編號
	  /// </summary>
    [Required]
	  public string DRIVER_ID { get; set; }

	  /// <summary>
	  /// 司機名稱
	  /// </summary>
    [Required]
	  public string DRIVER_NAME { get; set; }

	  /// <summary>
	  /// 加價費用
	  /// </summary>
    [Required]
	  public decimal EXTRA_FEE { get; set; }

	  /// <summary>
	  /// 路順
	  /// </summary>
    [Required]
	  public string DELV_WAY { get; set; }

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
  }
}
        