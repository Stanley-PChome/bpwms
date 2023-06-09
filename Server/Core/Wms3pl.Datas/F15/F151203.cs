namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總調撥單與調撥單對照表
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_ALLOC_NO","ALLOCATION_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F151203")]
  public class F151203 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string BATCH_ALLOC_NO { get; set; }

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Required]
	  public string PURCHASE_NO { get; set; }

	  /// <summary>
	  /// 驗收單號
	  /// </summary>
    [Required]
	  public string RT_NO { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主(0:共用)
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主(0:共用)
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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        