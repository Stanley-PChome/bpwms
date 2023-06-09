namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據類型對照表(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_TYPE")]
  [Table("F000901")]
  public class F000901 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類型
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_TYPE { get; set; }

	  /// <summary>
	  /// 單據類型名稱
	  /// </summary>
    [Required]
	  public string ORD_NAME { get; set; }

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

	  /// <summary>
	  /// 系統是否顯示(0否1是)
	  /// </summary>
    [Required]
	  public string ISVISABLE { get; set; }
  }
}
        