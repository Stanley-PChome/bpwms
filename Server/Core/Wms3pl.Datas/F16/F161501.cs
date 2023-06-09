namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨彙總表頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GATHER_NO","DC_CODE")]
  [Table("F161501")]
  public class F161501 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總單號
	  /// </summary>
    [Key]
    [Required]
	  public string GATHER_NO { get; set; }

	  /// <summary>
	  /// 彙總單建立日期
	  /// </summary>
    [Required]
	  public DateTime GATHER_DATE { get; set; }

	  /// <summary>
	  /// 匯入分貨表檔名
	  /// </summary>
	  public string FILE_NAME { get; set; }

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
  }
}
        