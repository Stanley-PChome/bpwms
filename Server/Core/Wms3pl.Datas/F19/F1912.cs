namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲位主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOC_CODE","DC_CODE")]
  [Table("F1912")]
  public class F1912 : IAuditInfo
  {

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 樓層
	  /// </summary>
	  public string FLOOR { get; set; }

	  /// <summary>
	  /// 儲區型態(F1919)
	  /// </summary>
    [Required]
	  public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 通道別
	  /// </summary>
    [Required]
	  public string CHANNEL { get; set; }

	  /// <summary>
	  /// 座別
	  /// </summary>
    [Required]
	  public string PLAIN { get; set; }

	  /// <summary>
	  /// 層別
	  /// </summary>
    [Required]
	  public string LOC_LEVEL { get; set; }

	  /// <summary>
	  /// 儲格
	  /// </summary>
	  public string LOC_TYPE { get; set; }

	  /// <summary>
	  /// 儲位料架編號(F1942)
	  /// </summary>
	  public string LOC_TYPE_ID { get; set; }

	  /// <summary>
	  /// 目前儲位狀態代碼(F1943)
	  /// </summary>
	  public string NOW_STATUS_ID { get; set; }

	  /// <summary>
	  /// 修改前儲位狀態代碼
	  /// </summary>
	  public string PRE_STATUS_ID { get; set; }

	  /// <summary>
	  /// 倉別編號(F1980)
	  /// </summary>
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動原因
	  /// </summary>
	  public string UCC_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 業主(0:共用)
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主(0:共用)
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 水平距離(公尺)
	  /// </summary>
	  public Decimal? HOR_DISTANCE { get; set; }

	  /// <summary>
	  /// 租用期間(起)
	  /// </summary>
	  public DateTime? RENT_BEGIN_DATE { get; set; }

	  /// <summary>
	  /// 租用期間(迄)
	  /// </summary>
	  public DateTime? RENT_END_DATE { get; set; }

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
	  /// 儲位可用容積(cm)
	  /// </summary>
	  public decimal? USEFUL_VOLUMN { get; set; }

	  /// <summary>
	  /// 儲位已用容積(cm)
	  /// </summary>
	  public Decimal? USED_VOLUMN { get; set; }

	  /// <summary>
	  /// 目前使用的貨主編號
	  /// </summary>
    [Required]
	  public string NOW_CUST_CODE { get; set; }

    /// <summary>
    /// 上次計算儲位容積時間
    /// </summary>
    public DateTime? LAST_CALVOLUMN_TIME { get; set; }
  }
}
        