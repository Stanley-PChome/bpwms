namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 加工單動作對應標籤 
  /// </summary>
  [Serializable]
  [DataServiceKey("PROCESS_NO","ACTION_NO","ORDER_BY","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910204")]
  public class F910204 : IAuditInfo
  {

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Key]
    [Required]
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 加工動作編號F910005
	  /// </summary>
    [Key]
    [Required]
	  public string ACTION_NO { get; set; }

	  /// <summary>
	  /// 標籤編號F1970
	  /// </summary>
	  public string LABEL_NO { get; set; }

	  /// <summary>
	  /// 列印順序
	  /// </summary>
    [Key]
    [Required]
	  public Int32 ORDER_BY { get; set; }

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
	  /// 貨主編號
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
  }
}
        