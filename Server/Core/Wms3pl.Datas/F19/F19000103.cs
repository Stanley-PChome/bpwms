namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據對應里程碑設定檔(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("TICKET_TYPE","TICKET_CLASS")]
  [Table("F19000103")]
  public class F19000103 : IAuditInfo
  {

	  /// <summary>
	  /// 單據類型(F000901)
	  /// </summary>
    [Key]
    [Required]
	  public string TICKET_TYPE { get; set; }

	  /// <summary>
	  /// 單據類別(F000906)
	  /// </summary>
    [Key]
    [Required]
	  public string TICKET_CLASS { get; set; }

	  /// <summary>
	  /// 里程碑一(F19000102)
	  /// </summary>
	  public string MILESTONE_NO_A { get; set; }

	  /// <summary>
	  /// 里程碑二
	  /// </summary>
	  public string MILESTONE_NO_B { get; set; }

	  /// <summary>
	  /// 里程碑三
	  /// </summary>
	  public string MILESTONE_NO_C { get; set; }

	  /// <summary>
	  /// 里程碑四
	  /// </summary>
	  public string MILESTONE_NO_D { get; set; }

	  /// <summary>
	  /// 里程碑五
	  /// </summary>
	  public string MILESTONE_NO_E { get; set; }

	  /// <summary>
	  /// 里程碑六
	  /// </summary>
	  public string MILESTONE_NO_F { get; set; }

	  /// <summary>
	  /// 里程碑七
	  /// </summary>
	  public string MILESTONE_NO_G { get; set; }

	  /// <summary>
	  /// 里程碑八
	  /// </summary>
	  public string MILESTONE_NO_H { get; set; }

	  /// <summary>
	  /// 里程碑九
	  /// </summary>
	  public string MILESTONE_NO_I { get; set; }

	  /// <summary>
	  /// 里程碑十
	  /// </summary>
	  public string MILESTONE_NO_J { get; set; }

	  /// <summary>
	  /// 里程碑十一
	  /// </summary>
	  public string MILESTONE_NO_K { get; set; }

	  /// <summary>
	  /// 里程碑十二
	  /// </summary>
	  public string MILESTONE_NO_L { get; set; }

	  /// <summary>
	  /// 里程碑十三
	  /// </summary>
	  public string MILESTONE_NO_M { get; set; }

	  /// <summary>
	  /// 里程碑十四
	  /// </summary>
	  public string MILESTONE_NO_N { get; set; }

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
        