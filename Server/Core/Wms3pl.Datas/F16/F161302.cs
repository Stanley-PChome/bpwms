namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨點收身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_CHECK_NO","RTN_CHECK_SEQ","DC_CODE")]
  [Table("F161302")]
  public class F161302 : IAuditInfo
  {

	  /// <summary>
	  /// 退貨點收編號
	  /// </summary>
    [Key]
    [Required]
	  public string RTN_CHECK_NO { get; set; }

	  /// <summary>
	  /// 退貨點收序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 RTN_CHECK_SEQ { get; set; }

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
	  public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 託運單號
	  /// </summary>
	  public string PAST_NO { get; set; }

	  /// <summary>
	  /// 刷讀條碼
	  /// </summary>
	  public string EAN_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
	  /// 是否強制結案(0否1是)
	  /// </summary>
    [Required]
	  public string ISCLOSE { get; set; }

	  /// <summary>
	  /// 強制結案原因F1951 Where UctId=RP
	  /// </summary>
	  public string UCC_CODE { get; set; }

	  /// <summary>
	  /// 強制結案其他原因
	  /// </summary>
	  public string REASON { get; set; }
  }
}
        