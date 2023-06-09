namespace Wms3pl.Datas.F20
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ADJUST_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F200101")]
  public class F200101 : IAuditInfo
  {

	  /// <summary>
	  /// 調整單號
	  /// </summary>
    [Key]
    [Required]
	  public string ADJUST_NO { get; set; }

	  /// <summary>
	  /// 調整單類別F000904
	  /// </summary>
    [Required]
	  public string ADJUST_TYPE { get; set; }

	  /// <summary>
	  /// 作業類別F000904
	  /// </summary>
	  public string WORK_TYPE { get; set; }

	  /// <summary>
	  /// 調整單建立日期
	  /// </summary>
    [Required]
	  public DateTime ADJUST_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0待處理 9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

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

	  /// <summary>
	  /// 來源單據(F000902)
	  /// </summary>
	  public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 來源單號
	  /// </summary>
	  public string SOURCE_NO { get; set; }
  }
}
        