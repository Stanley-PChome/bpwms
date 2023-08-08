namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據類別主檔(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("TICKET_CLASS")]
  [Table("F000906")]
  public class F000906 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類別
	  /// </summary>
    [Key]
    [Required]
	  public string TICKET_CLASS { get; set; }

	  /// <summary>
	  /// 單據類別名稱
	  /// </summary>
    [Required]
	  public string TICKET_CLASS_NAME { get; set; }

	  /// <summary>
	  /// 單據類型(F000901)
	  /// </summary>
    [Required]
	  public string TICKET_TYPE { get; set; }

	  /// <summary>
	  /// 來源單據F000902
	  /// </summary>
    [Required]
	  public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 訂單類別(B2B,B2C)
	  /// </summary>
    [Required]
	  public string ORD_TYPE { get; set; }

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
        