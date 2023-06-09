namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨貼紙主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","STICKER_NO")]
  [Table("F050804")]
  public class F050804 : IAuditInfo
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
	  /// 出貨貼紙編號(出貨單號+箱號)
	  /// </summary>
    [Key]
    [Required]
	  public string STICKER_NO { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
	  public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 箱號(流水號4碼)
	  /// </summary>
    [Required]
	  public string BOX_NO { get; set; }

	  /// <summary>
	  /// 揀貨單號
	  /// </summary>
	  public string PICK_ORD_NO { get; set; }

	  /// <summary>
	  /// 批次日期
	  /// </summary>
	  public DateTime? DELV_DATE { get; set; }

	  /// <summary>
	  /// 批次時間
	  /// </summary>
	  public string PICK_TIME { get; set; }

	  /// <summary>
	  /// 出車時段
	  /// </summary>
	  public string CAR_PERIOD { get; set; }

	  /// <summary>
	  /// 車次/路線
	  /// </summary>
	  public string DELV_NO { get; set; }

	  /// <summary>
	  /// 倉庫編號
	  /// </summary>
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲區編號
	  /// </summary>
	  public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 門市編號
	  /// </summary>
	  public string RETAIL_CODE { get; set; }

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
        