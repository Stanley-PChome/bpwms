namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據類別里程碑明細
  /// </summary>
  [Serializable]
  [DataServiceKey("MILESTONE_ID")]
  [Table("F19000101")]
  public class F19000101 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類別里程碑明細Id
	  /// </summary>
    [Key]
    [Required]
	  public Decimal MILESTONE_ID { get; set; }

	  /// <summary>
	  /// 單據類別主檔Id
	  /// </summary>
    [Required]
	  public Decimal TICKET_ID { get; set; }

	  /// <summary>
	  /// 里程碑編號(F19000102)
	  /// </summary>
    [Required]
	  public string MILESTONE_NO { get; set; }

	  /// <summary>
	  /// 里程碑順序(預設以A,B,C,D.....)
	  /// </summary>
    [Required]
	  public string SORT_NO { get; set; }

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
        