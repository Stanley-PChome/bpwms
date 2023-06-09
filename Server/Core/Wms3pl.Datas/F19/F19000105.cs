namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("TICKET_ID","DELV_EFFIC")]
  [Table("F19000105")]
  public class F19000105 : IAuditInfo
  {

	  /// <summary>
	  /// 單據主檔Id
	  /// </summary>
    [Key]
    [Required]
	  public Decimal TICKET_ID { get; set; }

	  /// <summary>
	  /// 配送效率(01:一般、02:3小時、03:6小時、04:9小時)
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_EFFIC { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

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
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        