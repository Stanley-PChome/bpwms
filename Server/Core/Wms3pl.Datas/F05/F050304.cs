namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 接單平台訂單檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","ORD_NO")]
  [Table("F050304")]
  public class F050304 : IAuditInfo
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
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 批次號
	  /// </summary>
    [Required]
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
	  public string CUST_ORD_NO { get; set; }

	  /// <summary>
	  /// 配送商編號
	  /// </summary>
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 配送門市編號
	  /// </summary>
	  public string DELV_RETAILCODE { get; set; }

	  /// <summary>
	  /// 配送門市名稱
	  /// </summary>
	  public string DELV_RETAILNAME { get; set; }

	  /// <summary>
	  /// 託運單號
	  /// </summary>
	  public string CONSIGN_NO { get; set; }

	  /// <summary>
	  /// 門市進貨日
	  /// </summary>
	  public DateTime? DELV_DATE { get; set; }

	  /// <summary>
	  /// 門市退貨日
	  /// </summary>
	  public DateTime? RETURN_DATE { get; set; }

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

	  /// <summary>
	  /// 服務商編號
	  /// </summary>
	  public string ESERVICE { get; set; }
  }
}
        