namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥(退貨上架)申請頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_APPLY_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161601")]
  public class F161601 : IAuditInfo
  {

	  /// <summary>
	  /// 調撥申請單號
	  /// </summary>
    [Key]
    [Required]
	  public string RTN_APPLY_NO { get; set; }

	  /// <summary>
	  /// 調撥申請單建立日期
	  /// </summary>
    [Required]
	  public DateTime RTN_APPLY_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理  2:結案 9:取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

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
	  /// 核准日期
	  /// </summary>
	  public DateTime? APPROVE_DATE { get; set; }

	  /// <summary>
	  /// 核准人員
	  /// </summary>
	  public string APPROVE_STAFF { get; set; }

	  /// <summary>
	  /// 核准人名
	  /// </summary>
	  public string APPROVE_NAME { get; set; }
  }
}
        