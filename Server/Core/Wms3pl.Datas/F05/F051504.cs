namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總批號門店caps設定
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_NO","RETAIL_CODE","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F051504")]
  public class F051504 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總批號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 狀態(0: 待處理 1:已下傳 2: 已結案 9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 門市編號
	  /// </summary>
    [Key]
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 門市名稱
	  /// </summary>
	  public string RETAIL_NAME { get; set; }

	  /// <summary>
	  /// Caps儲位編號
	  /// </summary>
    [Required]
	  public string CAPS_LOC_CODE { get; set; }

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
        