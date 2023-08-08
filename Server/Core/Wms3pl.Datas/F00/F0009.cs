namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據流水號紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_TYPE")]
  [Table("F0009")]
  public class F0009 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類型(F000901)
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_TYPE { get; set; }

	  /// <summary>
	  /// 單據日期
	  /// </summary>
    [Required]
	  public DateTime ORD_DATE { get; set; }

	  /// <summary>
	  /// 單據目前已使用流水號
	  /// </summary>
    [Required]
	  public Int32 ORD_SEQ { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔日期
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
	  /// 建檔人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        