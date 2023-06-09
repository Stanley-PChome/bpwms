namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 銷毀單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DESTROY_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F160501")]
  public class F160501 : IAuditInfo
  {

	  /// <summary>
	  /// 銷毀單號
	  /// </summary>
    [Key]
    [Required]
	  public string DESTROY_NO { get; set; }

	  /// <summary>
	  /// 預計銷毀日期
	  /// </summary>
    [Required]
	  public DateTime DESTROY_DATE { get; set; }

	  /// <summary>
	  /// 派車註記 (0: 自取  1: 專車  2: 派車)
	  /// </summary>
    [Required]
	  public string DISTR_CAR { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理  2已出貨 3:處理中 4:結案  9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 過帳日期
	  /// </summary>
	  public DateTime? POSTING_DATE { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
	  public string CUST_ORD_NO { get; set; }

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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 派車單號
	  /// </summary>
	  public string DISTR_CAR_NO { get; set; }

	  /// <summary>
	  /// 配送商編號(F1947)
	  /// </summary>
	  public string ALL_ID { get; set; }
  }
}
        