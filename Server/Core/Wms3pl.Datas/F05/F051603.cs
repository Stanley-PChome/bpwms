namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總批號播種結果檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_NO","ITEM_CODE","RETAIL_CODE","WMS_ORD_NO","LOC_CODE","CARTON","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F051603")]
  public class F051603 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總日期
	  /// </summary>
    [Required]
	  public DateTime BATCH_DATE { get; set; }

	  /// <summary>
	  /// 彙總批號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 品名
	  /// </summary>
    [Required]
	  public string ITEM_NAME { get; set; }

	  /// <summary>
	  /// 單位
	  /// </summary>
    [Required]
	  public string ITEM_UNIT { get; set; }

	  /// <summary>
	  /// 門市編號
	  /// </summary>
    [Key]
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 門市名稱
	  /// </summary>
    [Required]
	  public string RETAIL_NAME { get; set; }

	  /// <summary>
	  /// 預定配貨量
	  /// </summary>
    [Required]
	  public Int64 PLAN_QTY { get; set; }

	  /// <summary>
	  /// 重分配貨量
	  /// </summary>
    [Required]
	  public Int64 ORDER_QTY { get; set; }

	  /// <summary>
	  /// 實際配貨量
	  /// </summary>
	  public Int64? PACK_QTY { get; set; }

	  /// <summary>
	  /// 作業員
	  /// </summary>
	  public string EMP_ID { get; set; }

	  /// <summary>
	  /// 分貨完成時間
	  /// </summary>
	  public DateTime? PACK_TIME { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
	  public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
    [Required]
	  public string CUST_ORD_LIST { get; set; }

	  /// <summary>
	  /// 出車時段
	  /// </summary>
    [Required]
	  public string CAR_PERIOD { get; set; }

	  /// <summary>
	  /// 車次
	  /// </summary>
    [Required]
	  public string DELV_NO { get; set; }

	  /// <summary>
	  /// 路順
	  /// </summary>
    [Required]
	  public string DELV_WAY { get; set; }

	  /// <summary>
	  /// 揀貨儲位
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Key]
    [Required]
	  public string CARTON { get; set; }

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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        